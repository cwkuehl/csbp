// <copyright file="AdAdresse.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle AD_Adresse.
  /// </summary>
  [Serializable]
  [Table("AD_Adresse")]
  public partial class AdAdresse : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="AdAdresse"/> Klasse.</summary>
    public AdAdresse()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Staat.</summary>
    public string Staat { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Plz.</summary>
    public string Plz { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Ort.</summary>
    public string Ort { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Strasse.</summary>
    public string Strasse { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte HausNr.</summary>
    public string HausNr { get; set; }

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
