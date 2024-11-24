// <copyright file="HhKonto.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table HH_Konto.
/// </summary>
[Serializable]
[Table("HH_Konto")]
public partial class HhKonto : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="HhKonto"/> class.</summary>
  public HhKonto()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Sortierung.</summary>
  public string Sortierung { get; set; }

  /// <summary>Gets or sets the value of column Art.</summary>
  public string Art { get; set; }

  /// <summary>Gets or sets the value of column Kz.</summary>
  public string Kz { get; set; }

  /// <summary>Gets or sets the value of column Name.</summary>
  public string Name { get; set; }

  /// <summary>Gets or sets the value of column Gueltig_Von.</summary>
  public DateTime? Gueltig_Von { get; set; }

  /// <summary>Gets or sets the value of column Gueltig_Bis.</summary>
  public DateTime? Gueltig_Bis { get; set; }

  /// <summary>Gets or sets the value of column Periode_Von.</summary>
  public int Periode_Von { get; set; }

  /// <summary>Gets or sets the value of column Periode_Bis.</summary>
  public int Periode_Bis { get; set; }

  /// <summary>Gets or sets the value of column Betrag.</summary>
  public decimal Betrag { get; set; }

  /// <summary>Gets or sets the value of column EBetrag.</summary>
  public decimal EBetrag { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
