// <copyright file="WpKonfiguration.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle WP_Konfiguration.
  /// </summary>
  [Serializable]
  [Table("WP_Konfiguration")]
  public partial class WpKonfiguration : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="WpKonfiguration"/> Klasse.</summary>
    public WpKonfiguration()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bezeichnung.</summary>
    public string Bezeichnung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Parameter.</summary>
    public string Parameter {
      get {
        return GetExtension();
      }
      set {
        SetExtension(value);
      }
    }

    /// <summary>Holt oder setzt den Wert der Spalte Status.</summary>
    public string Status { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Notiz.</summary>
    public string Notiz { get; set; }

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
