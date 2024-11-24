// <copyright file="BackupType.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Client;

/// <summary>
/// Backup type.
/// </summary>
internal enum BackupType
{
  /// <summary>Creates a folder.</summary>
  CreateFolder,

  /// <summary>Copies a file.</summary>
  CopyFile,

  /// <summary>Deletes a file.</summary>
  DeleteFile,

  /// <summary>Deletes a folder.</summary>
  DeleteFolder,

  /// <summary>Modifies a folder.</summary>
  ModifyFolder,

  /// <summary>Zips a folder.</summary>
  ZipFolder,
}
