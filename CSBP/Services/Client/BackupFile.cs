// <copyright file="BackupFile.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Client;

using System;

/// <summary>
/// Backup file properties.
/// </summary>
internal class BackupFile
{
  /// <summary>Gets or sets the backup type.</summary>
  public BackupType Type { get; set; }

  /// <summary>Gets or sets the path.</summary>
  public string Path { get; set; }

  /// <summary>Gets or sets the second path.</summary>
  public string Path2 { get; set; }

  /// <summary>Gets or sets the last modified DateTime.</summary>
  public DateTime Modified { get; set; }
}
