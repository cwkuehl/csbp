// <copyright file="TbEintragOrt.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle TB_Eintrag_Ort.
  /// </summary>
  [Serializable]
  [Table("TB_Eintrag_Ort")]
  public partial class TbEintragOrt : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="TbEintragOrt"/> Klasse.</summary>
    public TbEintragOrt()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Ort_Uid.</summary>
    public string Ort_Uid { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum_Von.</summary>
    public DateTime Datum_Von { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum_Bis.</summary>
    public DateTime Datum_Bis { get; set; }

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
