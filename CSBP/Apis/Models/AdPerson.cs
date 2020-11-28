// <copyright file="AdPerson.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle AD_Person.
  /// </summary>
  [Serializable]
  [Table("AD_Person")]
  public partial class AdPerson : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="AdPerson"/> Klasse.</summary>
    public AdPerson()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Typ.</summary>
    public int Typ { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geschlecht.</summary>
    public string Geschlecht { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geburt.</summary>
    public DateTime? Geburt { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte GeburtK.</summary>
    public int GeburtK { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Anrede.</summary>
    public int Anrede { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte FAnrede.</summary>
    public int FAnrede { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Name1.</summary>
    public string Name1 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Name2.</summary>
    public string Name2 { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Praedikat.</summary>
    public string Praedikat { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Vorname.</summary>
    public string Vorname { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Titel.</summary>
    public string Titel { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Person_Status.</summary>
    public int Person_Status { get; set; }

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
