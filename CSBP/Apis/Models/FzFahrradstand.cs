// <copyright file="FzFahrradstand.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table FZ_Fahrradstand.
/// </summary>
[Serializable]
[Table("FZ_Fahrradstand")]
public partial class FzFahrradstand : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="FzFahrradstand"/> class.</summary>
  public FzFahrradstand()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Fahrrad_Uid.</summary>
  public string Fahrrad_Uid { get; set; }

  /// <summary>Gets or sets the value of column Datum.</summary>
  public DateTime Datum { get; set; }

  /// <summary>Gets or sets the value of column Nr.</summary>
  public int Nr { get; set; }

  /// <summary>Gets or sets the value of column Zaehler_km.</summary>
  public decimal Zaehler_km { get; set; }

  /// <summary>Gets or sets the value of column Periode_km.</summary>
  public decimal Periode_km { get; set; }

  /// <summary>Gets or sets the value of column Periode_Schnitt.</summary>
  public decimal Periode_Schnitt { get; set; }

  /// <summary>Gets or sets the value of column Beschreibung.</summary>
  public string Beschreibung { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }

  /// <summary>Gets or sets the value of column Replikation_Uid.</summary>
  public string Replikation_Uid { get; set; }
}
