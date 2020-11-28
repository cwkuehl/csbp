// <copyright file="MaMandant.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle MA_Mandant.
  /// </summary>
  [Serializable]
  [Table("MA_Mandant")]
  public partial class MaMandant : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="MaMandant"/> Klasse.</summary>
    public MaMandant()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Nr.</summary>
    public int Nr { get; set; }

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
  }
}
