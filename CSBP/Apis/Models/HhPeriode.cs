// <copyright file="HhPeriode.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Periode.
  /// </summary>
  [Serializable]
  [Table("HH_Periode")]
  public partial class HhPeriode : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="HhPeriode"/> Klasse.</summary>
    public HhPeriode()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Nr.</summary>
    public int Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum_Von.</summary>
    public DateTime Datum_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum_Bis.</summary>
    public DateTime Datum_Bis { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Art.</summary>
    public int Art { get; set; }

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
