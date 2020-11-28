// <copyright file="SoKurse.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SO_Kurse.
  /// </summary>
  [Serializable]
  [Table("SO_Kurse")]
  public partial class SoKurse : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="SoKurse"/> Klasse.</summary>
    public SoKurse()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Datum.</summary>
    public DateTime Datum { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Open.</summary>
    public decimal Open { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte High.</summary>
    public decimal High { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Low.</summary>
    public decimal Low { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Close.</summary>
    public decimal Close { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Price.</summary>
    public decimal Price { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bewertung.</summary>
    public string Bewertung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Trend.</summary>
    public string Trend { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bemerkung.</summary>
    public string Bemerkung { get; set; }
  }
}
