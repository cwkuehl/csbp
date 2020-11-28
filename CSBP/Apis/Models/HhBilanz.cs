// <copyright file="HhBilanz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Bilanz.
  /// </summary>
  [Serializable]
  [Table("HH_Bilanz")]
  public partial class HhBilanz : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="HhBilanz"/> Klasse.</summary>
    public HhBilanz()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Periode.</summary>
    public int Periode { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Kz.</summary>
    public string Kz { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Konto_Uid.</summary>
    public string Konto_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte SH.</summary>
    public string SH { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Betrag.</summary>
    public decimal Betrag { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte ESH.</summary>
    public string ESH { get; set; }

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
