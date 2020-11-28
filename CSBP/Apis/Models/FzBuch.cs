// <copyright file="FzBuch.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle FZ_Buch.
  /// </summary>
  [Serializable]
  [Table("FZ_Buch")]
  public partial class FzBuch : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="FzBuch"/> Klasse.</summary>
    public FzBuch()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Autor_Uid.</summary>
    public string Autor_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Serie_Uid.</summary>
    public string Serie_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Seriennummer.</summary>
    public int Seriennummer { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Titel.</summary>
    public string Titel { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Seiten.</summary>
    public int Seiten { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Sprache_Nr.</summary>
    public int Sprache_Nr { get; set; }

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
