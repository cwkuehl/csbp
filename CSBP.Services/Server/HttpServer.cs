// <copyright file="HttpServer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server;

using System;
using System.Net;
using System.Threading;
using CSBP.Services.Base;

/// <summary>
/// Simple http server.
/// </summary>
public class HttpServer
{
  /// <summary>Lock object.</summary>
  private static readonly object Locking = new();

  /// <summary>Internal http listener.</summary>
  private static HttpListener listener;

  /// <summary>
  /// Starts the http server on localhost with port 4201.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  public static void Start(ServiceDaten daten)
  {
    if (listener != null)
      return;

    listener = new HttpListener();
    listener.Prefixes.Add("http://localhost:4201/");
    listener.Prefixes.Add("http://127.0.0.1:4201/");
    listener.Start();

    while (true)
    {
      try
      {
        lock (Locking)
        {
          if (listener == null)
            break;
        }
        var context = listener.GetContext();
        ThreadPool.QueueUserWorkItem(o => HandleRequest(context, new ServiceDaten(daten.MandantNr, daten.BenutzerId)));
      }
      catch (Exception ex)
      {
        if (ex is not HttpListenerException)
          ServiceBase.Log.Error(ex, "HttpListener");
        break;
      }
    }
  }

  /// <summary>
  /// Stops the http server.
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
  /// Handles request.
  /// </summary>
  /// <param name="state">Affected HttpListenerContext.</param>
  /// <param name="daten">Service data for database access.</param>
  private static void HandleRequest(object state, ServiceDaten daten)
  {
    try
    {
      var context = (HttpListenerContext)state;
      var req = new HttpRequest
      {
        Verb = context.Request.HttpMethod,
        Path = context.Request.RawUrl,
        //// Header = context.Request,
        //// Body = "",
        Origin = context.Request.Headers["Origin"] ?? "",
        ContentLenth = (int)context.Request.ContentLength64,
      };
      using (System.IO.Stream body = context.Request.InputStream)
      using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
        req.Body = reader.ReadToEnd();
      var r = HttpsServer.Handle(req, daten);
      var resp = context.Response;
      foreach (var h in r.Headers.Keys)
        resp.AddHeader(h, r.Headers[h]);
      resp.StatusCode = r.StatusCode;
      resp.ContentLength64 = r.ContentLenth;
      //// resp.SendChunked = false;
      var output = resp.OutputStream;
      output.Write(r.Content, 0, r.Content.Length);
      output.Close();
    }
    catch (Exception ex)
    {
      Functions.MachNichts(ex);
    }
  }
}
