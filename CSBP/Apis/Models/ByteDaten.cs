// <copyright file="ByteDaten.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle Byte_Daten.
  /// </summary>
  [Serializable]
  [Table("Byte_Daten")]
  public partial class ByteDaten : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="ByteDaten"/> Klasse.</summary>
    public ByteDaten()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Typ.</summary>
    public string Typ { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Uid.</summary>
    public string Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Lfd_Nr.</summary>
    public int Lfd_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Metadaten.</summary>
    public string Metadaten { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Bytes.</summary>
    public byte[] Bytes { get; set; }

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
