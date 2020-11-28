// <copyright file="FzFahrradstand.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle FZ_Fahrradstand.
  /// </summary>
  [Serializable]
  [Table("FZ_Fahrradstand")]
  public partial class FzFahrradstand : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="FzFahrradstand"/> Klasse.</summary>
    public FzFahrradstand()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Fahrrad_Uid.</summary>
    public string Fahrrad_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum.</summary>
    public DateTime Datum { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Nr.</summary>
    public int Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Zaehler_km.</summary>
    public decimal Zaehler_km { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Periode_km.</summary>
    public decimal Periode_km { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Periode_Schnitt.</summary>
    public decimal Periode_Schnitt { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Beschreibung.</summary>
    public string Beschreibung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Angelegt_Von.</summary>
    public string Angelegt_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Angelegt_Am.</summary>
    public DateTime? Angelegt_Am { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geaendert_Von.</summary>
    public string Geaendert_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geaendert_Am.</summary>
    public DateTime? Geaendert_Am { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Replikation_Uid.</summary>
    public string Replikation_Uid { get; set; }
  }
}
