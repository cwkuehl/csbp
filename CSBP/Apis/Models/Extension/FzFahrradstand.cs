﻿// <copyright file="FzFahrradstand.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table FZ_Fahrradstand.
/// </summary>
public partial class FzFahrradstand : ModelBase
{
  /// <summary>Gets or sets the bike description.</summary>
  [NotMapped]
  public string BikeDescription { get; set; }
}
