// <copyright file="VFzFahrradstand.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models.Views;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Apis.Models;

/// <summary>
/// Entity class for view V_FZ_Fahrradstand.
/// </summary>
[Serializable]
[Table("V_FZ_Fahrradstand")]
public partial class VFzFahrradstand : FzFahrradstand
{
  /// <summary>Gets or sets the value of column Bezeichnung.</summary>
    public string Bezeichnung { get; set; }

  /// <summary>Gets or sets the value of column Typ.</summary>
    public int Typ { get; set; }
}
