// <copyright file="HttpServer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server
{
  using System;
  using CSBP.Base;
  using System.Net;
  using System.Threading;
  using CSBP.Services.Base;

  public class HttpServer
  {
    private static HttpListener listener;

    private static object locking = new object();

    public static void Start(string token)
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
          lock (locking)
          {
            if (listener == null)
              break;
          }
          var context = listener.GetContext();
          ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
        }
        catch (Exception ex)
        {
          if (!(ex is HttpListenerException))
            ServiceBase.Log.Error(ex, "HttpListener");
          break;
        }
      }
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

    private static void HandleRequest(object state)
    {
      try
      {
        var context = (HttpListenerContext)state;
        var req = new HttpRequest
        {
          Verb = context.Request.HttpMethod,
          Path = context.Request.RawUrl,
          //Header = context.Request,
          //Body = "",
          Origin = context.Request.Headers["Origin"] ?? "",
          ContentLenth = (int)context.Request.ContentLength64,
        };
        using (System.IO.Stream body = context.Request.InputStream)
        using (System.IO.StreamReader reader = new System.IO.StreamReader(body, context.Request.ContentEncoding))
          req.Body = reader.ReadToEnd();
        var r = HttpsServer.Handle(req);
        var resp = context.Response;
        foreach (var h in r.Headers.Keys)
          resp.AddHeader(h, r.Headers[h]);
        resp.StatusCode = r.StatusCode;
        resp.ContentLength64 = r.ContentLenth;
        //resp.SendChunked = false;
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
}
