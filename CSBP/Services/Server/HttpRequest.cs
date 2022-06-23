// <copyright file="HttpRequest.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Server;

/// <summary>
/// Simple http request.
/// </summary>
public class HttpRequest
{
  /// <summary>Gets or sets the verb.</summary>
  public string Verb { get; set; }

  /// <summary>Gets or sets the path.</summary>
  public string Path { get; set; }

  /// <summary>Gets or sets the complete header.</summary>
  public string Header { get; set; }

  /// <summary>Gets or sets the body.</summary>
  public string Body { get; set; }

  /// <summary>Gets or sets the origin.</summary>
  public string Origin { get; set; }

  /// <summary>Gets or sets the content length.</summary>
  public int ContentLenth { get; set; } = -1;
}
