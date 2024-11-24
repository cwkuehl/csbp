// <copyright file="HhBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table HH_Buchung.
/// </summary>
[Serializable]
[Table("HH_Buchung")]
public partial class HhBuchung : ModelBase
{
  /// <summary>Initializes a new instance of the <see cref="HhBuchung"/> class.</summary>
  public HhBuchung()
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the value of column Mandant_Nr.</summary>
  public int Mandant_Nr { get; set; }

  /// <summary>Gets or sets the value of column Uid.</summary>
  public string Uid { get; set; }

  /// <summary>Gets or sets the value of column Soll_Valuta.</summary>
  public DateTime Soll_Valuta { get; set; }

  /// <summary>Gets or sets the value of column Haben_Valuta.</summary>
  public DateTime Haben_Valuta { get; set; }

  /// <summary>Gets or sets the value of column Kz.</summary>
  public string Kz { get; set; }

  /// <summary>Gets or sets the value of column Betrag.</summary>
  public decimal Betrag { get; set; }

  /// <summary>Gets or sets the value of column EBetrag.</summary>
  public decimal EBetrag { get; set; }

  /// <summary>Gets or sets the value of column Soll_Konto_Uid.</summary>
  public string Soll_Konto_Uid { get; set; }

  /// <summary>Gets or sets the value of column Haben_Konto_Uid.</summary>
  public string Haben_Konto_Uid { get; set; }

  /// <summary>Gets or sets the value of column BText.</summary>
  public string BText { get; set; }

  /// <summary>Gets or sets the value of column Beleg_Nr.</summary>
  public string Beleg_Nr { get; set; }

  /// <summary>Gets or sets the value of column Beleg_Datum.</summary>
  public DateTime Beleg_Datum { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Von.</summary>
  public string Angelegt_Von { get; set; }

  /// <summary>Gets or sets the value of column Angelegt_Am.</summary>
  public DateTime? Angelegt_Am { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Von.</summary>
  public string Geaendert_Von { get; set; }

  /// <summary>Gets or sets the value of column Geaendert_Am.</summary>
  public DateTime? Geaendert_Am { get; set; }
}
