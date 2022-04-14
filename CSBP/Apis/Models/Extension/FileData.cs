// <copyright file="FileData.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using CSBP.Base;

/// <summary>
/// Entity class for a file with name and contest.
/// </summary>
public partial class FileData : ModelBase
{
  /// <summary>Default constructor.</summary>
  public FileData()
  {
    Functions.MachNichts();
  }

  /// <summary>Get or set the file name.</summary>
  public string Name { get; set; }

  /// <summary>Get or set the file content.</summary>
  public byte[] Bytes { get; set; }
}
