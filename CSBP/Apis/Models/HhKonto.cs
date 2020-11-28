// <copyright file="HhKonto.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Konto.
  /// </summary>
  [Serializable]
  [Table("HH_Konto")]
  public partial class HhKonto : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="HhKonto"/> Klasse.</summary>
    public HhKonto()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Sortierung.</summary>
    public string Sortierung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Art.</summary>
    public string Art { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Kz.</summary>
    public string Kz { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Name.</summary>
    public string Name { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Gueltig_Von.</summary>
    public DateTime? Gueltig_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Gueltig_Bis.</summary>
    public DateTime? Gueltig_Bis { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Periode_Von.</summary>
    public int Periode_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Periode_Bis.</summary>
    public int Periode_Bis { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Betrag.</summary>
    public decimal Betrag { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte EBetrag.</summary>
    public decimal EBetrag { get; set; }

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
