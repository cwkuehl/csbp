// <copyright file="HhEreignis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle HH_Ereignis.
  /// </summary>
  [Serializable]
  [Table("HH_Ereignis")]
  public partial class HhEreignis : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="HhEreignis"/> Klasse.</summary>
    public HhEreignis()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Kz.</summary>
    public string Kz { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Soll_Konto_Uid.</summary>
    public string Soll_Konto_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Haben_Konto_Uid.</summary>
    public string Haben_Konto_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bezeichnung.</summary>
    public string Bezeichnung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte EText.</summary>
    public string EText { get; set; }

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
