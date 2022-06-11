// <copyright file="FileData.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using CSBP.Base;

/// <summary>
/// Entity class for a file with name and content.
/// </summary>
public partial class FileData : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="FileData"/> class.</summary>
  public FileData()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the file name.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets the file content.</summary>
  public byte[] Bytes { get; set; }
}
