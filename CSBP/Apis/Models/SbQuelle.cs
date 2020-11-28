// <copyright file="SbQuelle.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SB_Quelle.
  /// </summary>
  [Serializable]
  [Table("SB_Quelle")]
  public partial class SbQuelle : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="SbQuelle"/> Klasse.</summary>
    public SbQuelle()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Beschreibung.</summary>
    public string Beschreibung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Zitat.</summary>
    public string Zitat { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bemerkung.</summary>
    public string Bemerkung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Autor.</summary>
    public string Autor { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Status1.</summary>
    public int Status1 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Status2.</summary>
    public int Status2 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Status3.</summary>
    public int Status3 { get; set; }

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
