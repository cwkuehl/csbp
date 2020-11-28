// <copyright file="SbEreignis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SB_Ereignis.
  /// </summary>
  [Serializable]
  [Table("SB_Ereignis")]
  public partial class SbEreignis : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="SbEreignis"/> Klasse.</summary>
    public SbEreignis()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Person_Uid.</summary>
    public string Person_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Familie_Uid.</summary>
    public string Familie_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Typ.</summary>
    public string Typ { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Tag1.</summary>
    public int Tag1 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Monat1.</summary>
    public int Monat1 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Jahr1.</summary>
    public int Jahr1 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Tag2.</summary>
    public int Tag2 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Monat2.</summary>
    public int Monat2 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Jahr2.</summary>
    public int Jahr2 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum_Typ.</summary>
    public string Datum_Typ { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Ort.</summary>
    public string Ort { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bemerkung.</summary>
    public string Bemerkung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Quelle_Uid.</summary>
    public string Quelle_Uid { get; set; }

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
