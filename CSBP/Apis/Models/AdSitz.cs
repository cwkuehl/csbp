// <copyright file="AdSitz.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle AD_Sitz.
  /// </summary>
  [Serializable]
  [Table("AD_Sitz")]
  public partial class AdSitz : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="AdSitz"/> Klasse.</summary>
    public AdSitz()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Person_Uid.</summary>
    public string Person_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Reihenfolge.</summary>
    public int Reihenfolge { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Typ.</summary>
    public int Typ { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Name.</summary>
    public string Name { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Adresse_Uid.</summary>
    public string Adresse_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Telefon.</summary>
    public string Telefon { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Fax.</summary>
    public string Fax { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Mobil.</summary>
    public string Mobil { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Email.</summary>
    public string Email { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Homepage.</summary>
    public string Homepage { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Postfach.</summary>
    public string Postfach { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bemerkung.</summary>
    public string Bemerkung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Sitz_Status.</summary>
    public int Sitz_Status { get; set; }

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
