// <copyright file="Benutzer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle Benutzer.
  /// </summary>
  [Serializable]
  [Table("Benutzer")]
  public partial class Benutzer : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="Benutzer"/> Klasse.</summary>
    public Benutzer()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Benutzer_ID.</summary>
    public string Benutzer_ID { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Passwort.</summary>
    public string Passwort { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Berechtigung.</summary>
    public int Berechtigung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Akt_Periode.</summary>
    public int Akt_Periode { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Person_Nr.</summary>
    public int Person_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Geburt.</summary>
    public DateTime? Geburt { get; set; }

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
