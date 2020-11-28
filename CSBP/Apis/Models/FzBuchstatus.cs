// <copyright file="FzBuchstatus.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle FZ_Buchstatus.
  /// </summary>
  [Serializable]
  [Table("FZ_Buchstatus")]
  public partial class FzBuchstatus : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="FzBuchstatus"/> Klasse.</summary>
    public FzBuchstatus()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Buch_Uid.</summary>
    public string Buch_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Ist_Besitz.</summary>
    public bool Ist_Besitz { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Lesedatum.</summary>
    public DateTime? Lesedatum { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Hoerdatum.</summary>
    public DateTime? Hoerdatum { get; set; }

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
