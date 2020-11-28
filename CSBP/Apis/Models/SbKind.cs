// <copyright file="SbKind.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle SB_Kind.
  /// </summary>
  [Serializable]
  [Table("SB_Kind")]
  public partial class SbKind : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="SbKind"/> Klasse.</summary>
    public SbKind()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Familie_Uid.</summary>
    public string Familie_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Kind_Uid.</summary>
    public string Kind_Uid { get; set; }

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
