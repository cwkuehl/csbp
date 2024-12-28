// <copyright file="HttpClientFactory.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.NonService;

using System.Net.Http;
using CSBP.Services.Base;
using Microsoft.Extensions.Http.Resilience;
using Polly;

/// <summary>Class for HTTP-Client with IHttpClientFactory.</summary>
public static class HttpClientFactory
{
  /// <summary>HttpClient factory can be null.</summary>
  private static IHttpClientFactory factory;

  /// <summary>Static resiiant HttpClient is used if factory is null.</summary>
  private static HttpClient httpsclient;

  /// <summary>Initializes a new instance of the <see cref="HttpClientFactory"/> class with dependency injection.</summary>
  /// <param name="f">The HTTP client factory.</param>
  public static void SetHttpClientFactory(IHttpClientFactory f)
  {
      factory = f;
  }

  /// <summary>Create a new HTTP client.</summary>
  /// <returns>The HTTP client.</returns>
  public static HttpClient CreateClient()
  {
    if (factory == null)
      return GetClient();
    return factory.CreateClient();
  }

  /// <summary> Create a new HTTP client.</summary>
  /// <param name="name">The name of the client.</param>
  /// <returns>The HTTP client.</returns>
  public static HttpClient CreateClient(string name)
  {
    if (factory == null)
      return GetClient();
    return factory.CreateClient(name);
  }

  /// <summary>Get a HTTP client.</summary>
  /// <returns>The HTTP client.</returns>
  private static HttpClient GetClient()
  {
    if (httpsclient == null)
    {
      // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines#resilience-policies-with-static-clients
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
          EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls13,
          RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
        },
      };
      #pragma warning disable EXTEXP0001 // Type or member is experimantal
      var resilienceHandler = new ResilienceHandler(retryPipeline)
      #pragma warning restore EXTEXP0001
      {
        InnerHandler = socketHandler,
      };
      httpsclient = new HttpClient(resilienceHandler)
      {
        Timeout = TimeSpan.FromMilliseconds(ServiceBase.HttpTimeout),
      };
      httpsclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:108.0) Gecko/20100101 Firefox/108.0");
    }
    return httpsclient;
  }
}
