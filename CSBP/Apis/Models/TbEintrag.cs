// <copyright file="TbEintrag.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle TB_Eintrag.
  /// </summary>
  [Serializable]
  [Table("TB_Eintrag")]
  public partial class TbEintrag : ModelBase
  {
    /// <summary>Initialisiert eine neue Instanz der <see cref="TbEintrag"/> Klasse.</summary>
    public TbEintrag()
    {
      Functions.MachNichts();
    }

    /// <summary>Holt oder setzt den Wert der Spalte Mandant_Nr.</summary>
    public int Mandant_Nr { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Datum.</summary>
    public DateTime Datum { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Eintrag.</summary>
    public string Eintrag { get; set; }

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
