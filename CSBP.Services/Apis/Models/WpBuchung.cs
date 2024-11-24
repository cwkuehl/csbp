// <copyright file="WpBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table WP_Buchung.
/// </summary>
[Serializable]
[Table("WP_Buchung")]
public partial class WpBuchung : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="WpBuchung"/> class.</summary>
  public WpBuchung()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Wertpapier_Uid.</summary>
  public string Wertpapier_Uid { get; set; }

  /// <summary>Gets or sets the value of column Anlage_Uid.</summary>
  public string Anlage_Uid { get; set; }

  /// <summary>Gets or sets the value of column Datum.</summary>
  public DateTime Datum { get; set; }

  /// <summary>Gets or sets the value of column Zahlungsbetrag.</summary>
  public decimal Zahlungsbetrag { get; set; }

  /// <summary>Gets or sets the value of column Rabattbetrag.</summary>
  public decimal Rabattbetrag { get; set; }

  /// <summary>Gets or sets the value of column Anteile.</summary>
  public decimal Anteile { get; set; }

  /// <summary>Gets or sets the value of column Zinsen.</summary>
  public decimal Zinsen { get; set; }

  /// <summary>Gets or sets the value of column BText.</summary>
  public string BText { get; set; }

  /// <summary>Gets or sets the value of column Notiz.</summary>
  public string Notiz
  {
    get { return GetExtension(); }
    set { SetExtension(value); }
  }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
