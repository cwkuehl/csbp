// <copyright file="HttpsServer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CSBP.Services.Base;
using CSBP.Services.Factory;

/// <summary>
/// Simple https server.
/// </summary>
public partial class HttpsServer
{
  /// <summary>Lock object.</summary>
  private static readonly object Locking = new();

  /// <summary>Internal tcp listener.</summary>
  private static TcpListener listener;

  /// <summary>Authentication token.</summary>
  private static string token;

  /// <summary>Internal server certificate.</summary>
  private static X509Certificate serverCertificate = null;

  /// <summary>
  /// Starts the https server on localhost with port 4202.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  public static void Start(ServiceDaten daten)
  {
    if (listener != null)
      return;
    HttpsServer.token = daten.BenutzerId;
    serverCertificate = X509CertificateLoader.LoadCertificateFromFile("/opt/Haushalt/CSBP/cert/cert_key.pfx");
    //// Create a TCP/IP (IPv4) socket and listen for incoming connections.
    //// Use with https://jshh.cwkuehl.de/#/diary
    listener = new TcpListener(IPAddress.Any, 4202);
    listener.Start();
    while (true)
    {
      try
      {
        lock (Locking)
        {
          if (listener == null)
            break;
          if (listener.Pending())
          {
            var client = listener.AcceptTcpClient();
            ThreadPool.QueueUserWorkItem(o => ProcessClient(client, new ServiceDaten(daten.Daten)));
          }
        }
        Thread.Sleep(100);
      }
      catch (Exception ex)
      {
        ServiceBase.Log.Error(ex, "TcpListener");
        break;
      }
    }
  }

  /// <summary>
  /// Is the server already started or not.
  /// </summary>
  /// <returns>Server started or not.</returns>
  public static bool IsStarted()
  {
    return listener != null;
  }

  /// <summary>
  /// Stops the https server.
  /// </summary>
  public static void Stop()
  {
    lock (Locking)
    {
      listener?.Stop();
      listener = null;
    }
  }

  /// <summary>
  /// Handles http request.
  /// </summary>
  /// <param name="req">Affected http request.</param>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Result as http response.</returns>
  public static HttpResponse Handle(HttpRequest req, ServiceDaten daten)
  {
    var resp = new HttpResponse();
    if (req == null)
    {
      resp.HeadersAndContent = Encoding.UTF8.GetBytes("Error");
      return resp;
    }
    var r = new ServiceErgebnis();
    var rh = resp.Headers;
    var statuscode = "200 OK";
    var contenttype = "text/html; charset=utf-8";
    var options = false;
    var response = "";

    rh["Date"] = daten.Jetzt.ToUniversalTime().ToString("r");
    if (req.Path == "/stop")
    {
      response = "Stop!";
      Task.Run(() =>
       {
         Thread.Sleep(1000);
         Stop();
       });
    }
    else if (req.Path == "/favicon.ico")
    {
      // nix
    }
    else if (req.Verb == "OPTIONS")
    {
      options = true; // CORS
      rh["Access-Control-Allow-Methods"] = "POST, GET, OPTIONS";
      rh["Access-Control-Allow-Headers"] = "X-PINGOTHER, Content-Type";
      rh["Access-Control-Max-Age"] = "86400";
      if (!string.IsNullOrEmpty(req.Origin))
      {
        rh["Access-Control-Allow-Origin"] = req.Origin;
        rh["Vary"] = "Origin";
      }
    }
    else if (req.Verb == "POST")
    {
      string token = null;
      string table = null;
      string mode = null;
      string data = null;
      try
      {
        using var doc = JsonDocument.Parse(req.Body ?? "");
        var root = doc.RootElement;
        var values = root.EnumerateObject();
        while (values.MoveNext())
        {
          var v = values.Current;
          if (v.Value.ValueKind == JsonValueKind.String)
          {
            if (v.Name == "token")
              token = v.Value.GetString();
            else if (v.Name == "table")
              table = v.Value.GetString();
            else if (v.Name == "mode")
              mode = v.Value.GetString();
            else if (v.Name == "data")
              data = v.Value.GetString();
          }
        }
      }
      catch (Exception ex)
      {
        Functions.MachNichts(ex);
      }
      if (token != HttpsServer.token)
        r.Errors.Add(new Message($"Unberechtigt: {token}.", true));
      else
      {
        contenttype = "application/json; charset=utf-8";
        var r1 = FactoryService.ClientService.ReplicateTable(daten, table, mode, data);
        r.Get(r1);
        if (r.Ok)
          response = r1.Ergebnis;
        //// response = '''[{"a":"abc äöüÄÖÜß xyz", "body":"«body»"}]'''
      }
    }
    else if (req.Path == "/")
    {
      response = $"<h1>Hällo!</h1><h2>Anfrage: {req.Path}</h2><h3>{daten.Jetzt}</h3>";
    }
    else
    {
      r.Errors.Add(new Message($"Unbekannte Resource: {req.Path}", true));
    }
    if (!r.Ok)
    {
      contenttype = "text/html; charset=utf-8";
      response = r.Errors.First().MessageText;
      //// statuscode = 404 // Not Found
      statuscode = "401 Unauthorized";
      resp.StatusCode = 401;
    }
    if (!options)
    {
      rh["Content-type"] = contenttype;
      rh["Pragma"] = "no-cache";
      rh["Cache-control"] = "no-cache";
      rh["Expires"] = "-1";

      // rh["Server"] = "Microsoft-IIS/10.0";
      // rh["Vary"] = "Accept-Encoding";
      // rh["Content-Encoding"] = "identity";
      // rh["Access-Control-Allow-Origin"] = "http://localhost:4200";
      if (!string.IsNullOrEmpty(req.Origin))
      {
        rh["Access-Control-Allow-Origin"] = req.Origin;
        rh["Vary"] = "Origin";
      }
    }
    /*
Response-Header:
HTTP/1.1 200 OK
Pragma: no-cache
Date: Tue, 24 Mar 2020 17:08:49 GMT
Content-type: text/html; charset=utf-8
Expires: -1
Content-length: 72
Cache-control: no-cache
    */
    var content = Encoding.UTF8.GetBytes(response ?? "");
    if (content.Length > 0)
      rh["Content-length"] = Functions.ToString(content.Length);
    resp.Content = content;
    resp.ContentLenth = content.Length;

    var ms = new MemoryStream();
    var rb = new StringBuilder();
    rb.Append("HTTP/1.1 ").Append(statuscode).Append(Constants.CRLF);
    foreach (var d in rh)
    {
      rb.Append(d.Key).Append(": ").Append(d.Value).Append(Constants.CRLF);
    }
    rb.Append(Constants.CRLF); // Header-Ende
    ms.Write(Encoding.UTF8.GetBytes(rb.ToString()));
    ms.Write(content);
    resp.HeadersAndContent = ms.ToArray();
    return resp;
  }

  /// <summary>
  /// Processes a tcp client.
  /// </summary>
  /// <param name="client">Affected tcp client.</param>
  /// <param name="daten">Service data for database access.</param>
  private static void ProcessClient(TcpClient client, ServiceDaten daten)
  {
    // A client has connected. Create the SslStream using the client's network stream.
    var sslStream = new SslStream(client.GetStream(), false);
    //// Authenticate the server but don't require the client to authenticate.
    try
    {
      sslStream.AuthenticateAsServer(serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

      // Display the properties and settings for the authenticated stream.
      // DisplaySecurityLevel(sslStream);
      // DisplaySecurityServices(sslStream);
      // DisplayCertificateInformation(sslStream);
      // DisplayStreamProperties(sslStream);

      // Set timeouts for the read and write to 5 seconds.
      sslStream.ReadTimeout = 5000;
      sslStream.WriteTimeout = 5000;
      var req = ReadMessage(sslStream);
      //// Console.WriteLine("Received: {0}", messageData);

      // Write a message to the client.
      var rp = Handle(req, daten);
      sslStream.Write(rp.HeadersAndContent);
    }
    catch (AuthenticationException ex)
    {
      ServiceBase.Log.Error(ex, "ProcessClient");
    }
    finally
    {
      // The client stream will be closed with the sslStream because we specified this behavior when creating the sslStream.
      sslStream.Close();
      client.Close();
    }
  }

  /// <summary>
  /// Reads http request from ssl stream.
  /// </summary>
  /// <param name="sslStream">Affected ssl stream.</param>
  /// <returns>Parsed http request.</returns>
  private static HttpRequest ReadMessage(SslStream sslStream)
  {
    var r = new HttpRequest();
    var buffer = new byte[4096];
    var messageData = new StringBuilder();
    //// Use Decoder class to convert from bytes to UTF8 in case a character spans two buffers.
    var decoder = Encoding.UTF8.GetDecoder();
    var ms = new MemoryStream();
    var msheaderlength = -1;
    var headerlength = -1;
    int bytes;
    do
    {
      bytes = sslStream.Read(buffer, 0, buffer.Length);
      ms.Write(buffer, 0, bytes);
      var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
      decoder.GetChars(buffer, 0, bytes, chars, 0);
      messageData.Append(chars);
      if (r.Verb == null)
      {
        var i = messageData.IndexOf(" ");
        if (i >= 0)
        {
          r.Verb = messageData.ToString(0, i);
          var j = messageData.IndexOf(" ", i + 1);
          if (j >= i + 1)
            r.Path = messageData.ToString(i + 1, j - i - 1);
        }
      }
      if (r.Header == null)
      {
        headerlength = messageData.IndexOf("\r\n\r\n");
        if (headerlength >= 0)
        {
          r.Header = messageData.ToString(0, headerlength);
          var m = OriginRegex().Match(r.Header);
          r.Origin = m.Success ? m.Groups[1].Value : null;
          m = ContentLengthRegex().Match(r.Header);
          r.ContentLenth = m.Success ? Functions.ToInt32(m.Groups[1].Value) : 0;
          msheaderlength = ms.GetBuffer().IndexOf("\r\n\r\n"u8.ToArray());
          if (msheaderlength >= 0)
            msheaderlength += 4;
          headerlength += 4;
        }
      }
      if (r.ContentLenth >= 0 && ms.Length >= msheaderlength + r.ContentLenth)
      {
        r.Body = messageData.ToString(headerlength, messageData.Length - headerlength);
        break;
      }
    }
    while (bytes != 0);
    return r;
  }

  /// <summary>Regex for content length.</summary>
  [GeneratedRegex("\\r\\nContent-Length: *(\\d+)\\r\\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex ContentLengthRegex();

  /// <summary>Regex for origin.</summary>
  [GeneratedRegex("\\r\\nOrigin: *([^\\r]+)\\r\\n", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex OriginRegex();
}
