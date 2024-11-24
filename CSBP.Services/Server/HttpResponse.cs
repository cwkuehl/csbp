// <copyright file="HttpResponse.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server;

using System;
using System.Collections.Generic;

/// <summary>
/// Simple http response.
/// </summary>
public class HttpResponse
{
  /// <summary>Gets or sets the status code.</summary>
  public int StatusCode { get; set; } = 200;

  /// <summary>Gets or sets the headers dictionary.</summary>
  public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

  /// <summary>Gets or sets the content.</summary>
  public byte[] Content { get; set; } = Array.Empty<byte>();

  /// <summary>Gets or sets the headers and content.</summary>
  public byte[] HeadersAndContent { get; set; } = Array.Empty<byte>();

  /// <summary>Gets or sets the content length.</summary>
  public int ContentLenth { get; set; } = -1;
}
