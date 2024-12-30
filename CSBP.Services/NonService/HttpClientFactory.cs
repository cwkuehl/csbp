// <copyright file="HttpClientFactory.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.NonService;

using System.Net.Http;
using System.Security.Authentication;
using CSBP.Services.Base;
using Microsoft.Extensions.Http.Resilience;
using Polly;

/// <summary>Class for HTTP-Client with IHttpClientFactory.</summary>
public static class HttpClientFactory
{
  /// <summary>HttpClient factory can be null.</summary>
  private static IHttpClientFactory factory;

  /// <summary>Static resiliant HttpClient is used if factory is null.
  /// For name HttpClientWithSSLUntrusted.</summary>
  private static HttpClient httpsclient0;

  /// <summary>Initializes a new instance of the <see cref="HttpClientFactory"/> class with dependency injection.</summary>
  /// <param name="f">The HTTP client factory.</param>
  public static void SetHttpClientFactory(IHttpClientFactory f)
  {
      factory = f;
  }

  // /// <summary>Create a new HTTP client.</summary>
  // /// <returns>The HTTP client.</returns>
  // public static HttpClient CreateClient()
  // {
  //   if (factory == null)
  //     return GetClient();
  //   return factory.CreateClient();
  // }

  /// <summary> Create a new HTTP client.</summary>
  /// <param name="name">The name of the client.</param>
  /// <param name="timeout">The timeout in milliseconds or -1 for standard timeout.</param>
  /// <returns>The HTTP client.</returns>
  public static HttpClient CreateClient(string name = "HttpClientWithSSLUntrusted", int timeout = -1)
  {
    HttpClient client;
    if (factory == null || (timeout >= 0 && timeout != ServiceBase.HttpTimeout))
      client = GetClient(name, timeout);
    else
      client = factory.CreateClient(name);
    return client;
  }

  /// <summary>Get a HTTP client.</summary>
  /// <param name="name">The name of the client.</param>
  /// <param name="timeout">The timeout in milliseconds or -1 for standard timeout.</param>
  /// <returns>The HTTP client.</returns>
  private static HttpClient GetClient(string name = "HttpClientWithSSLUntrusted", int timeout = -1)
  {
    HttpClient client = null;
    if (httpsclient0 != null && timeout < 0)
    {
      client = httpsclient0;
    }
    if (client == null)
    {
      // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#resilience-policies-with-static-clients
      // var handler = new HttpClientHandler
      // {
      //   ClientCertificateOptions = ClientCertificateOption.Manual,
      //   SslProtocols = SslProtocols.Tls13,
      //   ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
      //   {
      //     return true;
      //   },
      // };
      // var httpsclient = new HttpClient(handler)
      // {
      //   Timeout = TimeSpan.FromMilliseconds(timeout),
      // };
      // httpsclient.DefaultRequestHeaders.Add("X-RapidAPI-Host", "meteostat.p.rapidapi.com'");
      var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
        .AddRetry(new HttpRetryStrategyOptions
        {
          BackoffType = DelayBackoffType.Exponential,
          MaxRetryAttempts = 3,
        })
        .Build();
      var socketHandler = new SocketsHttpHandler
      {
        PooledConnectionLifetime = TimeSpan.FromMinutes(15),
        SslOptions = new System.Net.Security.SslClientAuthenticationOptions
        {
          // ClientCertificates = null,
          // EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
          EnabledSslProtocols = SslProtocols.Tls13,
          RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
        },
      };
      #pragma warning disable EXTEXP0001 // Type or member is experimantal
      var resilienceHandler = new ResilienceHandler(retryPipeline)
      #pragma warning restore EXTEXP0001
      {
        InnerHandler = socketHandler,
      };
      client = new HttpClient(resilienceHandler)
      {
        Timeout = TimeSpan.FromMilliseconds(ServiceBase.HttpTimeout),
      };
      if (timeout >= 0)
      {
        client.Timeout = TimeSpan.FromMilliseconds(timeout);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:108.0) Gecko/20100101 Firefox/108.0");
      }
    }
    if (httpsclient0 == null && timeout < 0)
    {
      httpsclient0 = client;
    }
    return client;
  }
}
