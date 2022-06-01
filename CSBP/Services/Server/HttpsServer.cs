// <copyright file="HttpsServer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Security;
  using System.Net.Sockets;
  using System.Security.Authentication;
  using System.Security.Cryptography.X509Certificates;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using CSBP.Services.Factory;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class HttpsServer
  {
    private static TcpListener listener;

    private static string token;

    private static readonly object locking = new();

    private static X509Certificate serverCertificate = null;

    private static readonly Regex RxContentLength = new(@"\r\nContent-Length: *(\d+)\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex RxOrigin = new(@"\r\nOrigin: *([^\r]+)\r\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static void Start(string token)
    {
      if (listener != null)
        return;
      HttpsServer.token = token;
      serverCertificate = new X509Certificate2("/opt/Haushalt/CSBP/cert/cert_key.pfx", "");
      // Create a TCP/IP (IPv4) socket and listen for incoming connections.
      listener = new TcpListener(IPAddress.Any, 4202);
      listener.Start();
      while (true)
      {
        try
        {
          lock (locking)
          {
            if (listener == null)
              break;
            if (listener.Pending())
            {
              var client = listener.AcceptTcpClient();
              ThreadPool.QueueUserWorkItem(o => ProcessClient(client));
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
    /// Ist der Server schon gestartet?
    /// </summary>
    /// <returns>Server schon gestartet?</returns>
    public static bool IsStarted()
    {
      return listener != null;
    }

    public static void Stop()
    {
      lock (locking)
      {
        if (listener != null)
          listener.Stop();
        listener = null;
      }
    }

    static void ProcessClient(TcpClient client)
    {
      // A client has connected. Create the SslStream using the client's network stream.
      var sslStream = new SslStream(client.GetStream(), false);
      // Authenticate the server but don't require the client to authenticate.
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
        //Console.WriteLine("Received: {0}", messageData);

        // Write a message to the client.
        var rp = Handle(req);
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

    static HttpRequest ReadMessage(SslStream sslStream)
    {
      var r = new HttpRequest();
      var buffer = new byte[4096];
      var messageData = new StringBuilder();
      // Use Decoder class to convert from bytes to UTF8 in case a character spans two buffers.
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
            var m = RxOrigin.Match(r.Header);
            r.Origin = m.Success ? m.Groups[1].Value : null;
            m = RxContentLength.Match(r.Header);
            r.ContentLenth = m.Success ? Functions.ToInt32(m.Groups[1].Value) : 0;
            msheaderlength = ms.GetBuffer().IndexOf(new byte[] { 13, 10, 13, 10 });
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

    /// <summary>
    /// Bearbeiten einer HTTP-Anfrage.
    /// </summary>
    /// <param name="req">Betroffene HTTP-Anfrage.</param>
    /// <returns>Ergebnis als HTTP-Antwort.</returns>
    public static HttpResponse Handle(HttpRequest req)
    {
      var resp = new HttpResponse();
      if (req == null)
      {
        resp.HeadersAndContent = Encoding.UTF8.GetBytes("Error");
        return resp;
      }
      var daten = MainClass.ServiceDaten;
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
          using var reader = new JsonTextReader(new StringReader(req.Body ?? ""));
          var jo = (JObject)JToken.ReadFrom(reader);
          foreach (var jt in jo.Values())
          {
            if (jt.Type == JTokenType.String)
            {
              if (jt.Path == "token")
                token = jt.Value<string>();
              else if (jt.Path == "table")
                table = jt.Value<string>();
              else if (jt.Path == "mode")
                mode = jt.Value<string>();
              else if (jt.Path == "data")
                data = jt.Value<string>();
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
          //response = '''[{"a":"abc äöüÄÖÜß xyz", "body":"«body»"}]'''
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
        //statuscode = 404 // Not Found
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
  }

  /// <summary>
  /// Vereinfachte HTTP-Anfrage.
  /// </summary>
  public class HttpRequest
  {
    public string Verb { get; set; }
    public string Path { get; set; }
    public string Header { get; set; }
    public string Body { get; set; }
    public string Origin { get; set; }
    public int ContentLenth { get; set; } = -1;
  }

  /// <summary>
  /// Vereinfachte HTTP-Antwort.
  /// </summary>
  public class HttpResponse
  {
    public int StatusCode { get; set; } = 200;
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public byte[] HeadersAndContent { get; set; } = Array.Empty<byte>();
    public int ContentLenth { get; set; } = -1;
  }
}
