// <copyright file="TbEintragOrt.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.ComponentModel.DataAnnotations.Schema;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle TB_Eintrag_Ort.
  /// </summary>
  public partial class TbEintragOrt : ModelBase
  {
    /// <summary>Holt oder setzt den Wert der Spalte Bezeichnung.</summary>
    [NotMapped]
    public string Bezeichnung { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Breite.</summary>
    [NotMapped]
    public decimal Breite { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Laenge.</summary>
    [NotMapped]
    public decimal Laenge { get; set; }

    /// <summary>Holt oder setzt den Wert der Spalte Hoehe.</summary>
    [NotMapped]
    public decimal Hoehe { get; set; }

    /// <summary>
    /// Liefert Hash-Wert des Datensatzes.
    /// </summary>
    /// <returns>Hash-Wert des Datensatzes.</returns>
    public string Hash()
    {
      return $"{Ort_Uid ?? ""}#{Datum_Von.ToString()}#{Datum_Bis.ToString()}";
    }
  }
}
