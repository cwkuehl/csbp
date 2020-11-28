// <copyright file="SbPerson.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SB_Person.
  /// </summary>
  [Serializable]
  [Table("SB_Person")]
  public partial class SbPerson : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="SbPerson"/> Klasse.</summary>
    public SbPerson()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Name.</summary>
    public string Name { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Vorname.</summary>
    public string Vorname { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geburtsname.</summary>
    public string Geburtsname { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geschlecht.</summary>
    public string Geschlecht { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Titel.</summary>
    public string Titel { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Konfession.</summary>
    public string Konfession { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bemerkung.</summary>
    public string Bemerkung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Quelle_Uid.</summary>
    public string Quelle_Uid { get; set; }

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
