// <copyright file="HhBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Buchung.
  /// </summary>
  [Serializable]
  [Table("HH_Buchung")]
  public partial class HhBuchung : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="HhBuchung"/> Klasse.</summary>
    public HhBuchung()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Soll_Valuta.</summary>
    public DateTime Soll_Valuta { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Haben_Valuta.</summary>
    public DateTime Haben_Valuta { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Kz.</summary>
    public string Kz { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Betrag.</summary>
    public decimal Betrag { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte EBetrag.</summary>
    public decimal EBetrag { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Soll_Konto_Uid.</summary>
    public string Soll_Konto_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Haben_Konto_Uid.</summary>
    public string Haben_Konto_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte BText.</summary>
    public string BText { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Beleg_Nr.</summary>
    public string Beleg_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Beleg_Datum.</summary>
    public DateTime Beleg_Datum { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Angelegt_Von.</summary>
    public string Angelegt_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Angelegt_Am.</summary>
    public DateTime? Angelegt_Am { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geaendert_Von.</summary>
    public string Geaendert_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geaendert_Am.</summary>
    public DateTime? Geaendert_Am { get; set; }
  }
}
