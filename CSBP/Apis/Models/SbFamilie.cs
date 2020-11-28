// <copyright file="SbFamilie.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SB_Familie.
  /// </summary>
  [Serializable]
  [Table("SB_Familie")]
  public partial class SbFamilie : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="SbFamilie"/> Klasse.</summary>
    public SbFamilie()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Mann_Uid.</summary>
    public string Mann_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Frau_Uid.</summary>
    public string Frau_Uid { get; set; }

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
