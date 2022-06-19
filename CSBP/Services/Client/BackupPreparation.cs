// <copyright file="BackupPreparation.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Client;

using System;

/// <summary>
/// Record class for backup preparation.
/// </summary>
internal class BackupPreparation
{
  /// <summary>Gets or sets the path.</summary>
  public string Path { get; set; }

  /// <summary>Gets or sets the file name.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets the last modified DateTime.</summary>
  public DateTime Modified { get; set; }
}
