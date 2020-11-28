// <copyright file="WpBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle WP_Buchung.
  /// </summary>
  [Serializable]
  [Table("WP_Buchung")]
  public partial class WpBuchung : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="WpBuchung"/> Klasse.</summary>
    public WpBuchung()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Wertpapier_Uid.</summary>
    public string Wertpapier_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Anlage_Uid.</summary>
    public string Anlage_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum.</summary>
    public DateTime Datum { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Zahlungsbetrag.</summary>
    public decimal Zahlungsbetrag { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Rabattbetrag.</summary>
    public decimal Rabattbetrag { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Anteile.</summary>
    public decimal Anteile { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Zinsen.</summary>
    public decimal Zinsen { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte BText.</summary>
    public string BText { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Notiz.</summary>
    public string Notiz {
      get {
        return GetExtension();
      }
      set {
        SetExtension(value);
      }
    }

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
