﻿// <copyright file="SbKind.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table SB_Kind.
/// </summary>
public partial class SbKind : ModelBase
{
  /// <summary>Gets or sets the child.</summary>
  [NotMapped]
  public SbPerson Child { get; set; }
}
