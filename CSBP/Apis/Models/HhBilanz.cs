// <copyright file="HhBilanz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Base;

/// <summary>
/// Entity class for table HH_Bilanz.
/// </summary>
[Serializable]
[Table("HH_Bilanz")]
public partial class HhBilanz : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="HhBilanz"/> class.</summary>
  public HhBilanz()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Periode.</summary>
  public int Periode { get; set; }

  /// <summary>Gets or sets the value of column Kz.</summary>
  public string Kz { get; set; }

  /// <summary>Gets or sets the value of column Konto_Uid.</summary>
  public string Konto_Uid { get; set; }

  /// <summary>Gets or sets the value of column SH.</summary>
  public string SH { get; set; }

  /// <summary>Gets or sets the value of column Betrag.</summary>
  public decimal Betrag { get; set; }

  /// <summary>Gets or sets the value of column ESH.</summary>
  public string ESH { get; set; }

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
