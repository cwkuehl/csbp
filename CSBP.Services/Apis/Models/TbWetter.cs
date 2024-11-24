// <copyright file="TbWetter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table TB_Wetter.
/// </summary>
[Serializable]
[Table("TB_Wetter")]
public partial class TbWetter : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="TbWetter"/> class.</summary>
  public TbWetter()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Datum.</summary>
  public DateTime Datum { get; set; }

  /// <summary>Gets or sets the value of column Ort_Uid.</summary>
  public string Ort_Uid { get; set; }

  /// <summary>Gets or sets the value of column Api.</summary>
  public string Api { get; set; }

  /// <summary>Gets or sets the value of column Werte.</summary>
  public string Werte { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
