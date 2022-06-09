// <copyright file="WpWertpapier.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Text;
  using CSBP.Base;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>
  /// Entity class for table WP_Wertpapier.
  /// </summary>
  public partial class WpWertpapier : ModelBase
  {
    /// <summary>Holt oder setzt die erweiterte Bezeichnung.</summary>
    [NotMapped]
    public string Description { get; set; }

    /// <summary>Holt oder setzt die Relations-Bezeichnung.</summary>
    [NotMapped]
    public string RelationDescription { get; set; }

    /// <summary>Holt oder setzt die Relations-Quelle.</summary>
    [NotMapped]
    public string RelationSource { get; set; }

    /// <summary>Holt oder setzt das Relations-Kürzel.</summary>
    [NotMapped]
    public string RelationAbbreviation { get; set; }

    /// <summary>Holt oder setzt den aktuellen Kurs.</summary>
    [NotMapped]
    public decimal? CurrentPrice { get; set; }

    /// <summary>Holt oder setzt den Signal-Kurs 1.</summary>
    [NotMapped]
    public decimal? SignalPrice1 { get; set; }

    /// <summary>Holt oder setzt den Signal-Kurs 2.</summary>
    [NotMapped]
    public decimal? SignalPrice2 { get; set; }

    /// <summary>Holt oder setzt den Stopp-Kurs.</summary>
    [NotMapped]
    public decimal? StopPrice { get; set; }

    /// <summary>Holt oder setzt das Muster.</summary>
    [NotMapped]
    public string Pattern { get; set; }

    /// <summary>Holt oder setzt die Sortierung.</summary>
    [NotMapped]
    public string Sorting { get; set; }

    /// <summary>Holt oder setzt die Bewertung.</summary>
    [NotMapped]
    public string Assessment { get; set; }

    /// <summary>Holt oder setzt die Bewertung 1.</summary>
    [NotMapped]
    public string Assessment1 { get; set; }

    /// <summary>Holt oder setzt die Bewertung 2.</summary>
    [NotMapped]
    public string Assessment2 { get; set; }

    /// <summary>Holt oder setzt die Bewertung 3.</summary>
    [NotMapped]
    public string Assessment3 { get; set; }

    /// <summary>Holt oder setzt die Bewertung 4.</summary>
    [NotMapped]
    public string Assessment4 { get; set; }

    /// <summary>Holt oder setzt die Bewertung 5.</summary>
    [NotMapped]
    public string Assessment5 { get; set; }

    /// <summary>Holt oder setzt den Trend 1.</summary>
    [NotMapped]
    public string Trend1 { get; set; }

    /// <summary>Holt oder setzt den Trend 2.</summary>
    [NotMapped]
    public string Trend2 { get; set; }

    /// <summary>Holt oder setzt den Trend 3.</summary>
    [NotMapped]
    public string Trend3 { get; set; }

    /// <summary>Holt oder setzt den Trend 4.</summary>
    [NotMapped]
    public string Trend4 { get; set; }

    /// <summary>Holt oder setzt den Trend 5.</summary>
    [NotMapped]
    public string Trend5 { get; set; }

    /// <summary>Holt oder setzt den Trend.</summary>
    [NotMapped]
    public string Trend { get; set; }

    /// <summary>Holt oder setzt das X oder O.</summary>
    [NotMapped]
    public DateTime? PriceDate { get; set; }

    /// <summary>Holt oder setzt den Wechsel.</summary>
    [NotMapped]
    public string Xo { get; set; }

    /// <summary>Holt oder setzt den Signal-Bewertung.</summary>
    [NotMapped]
    public string SignalAssessment { get; set; }

    /// <summary>Holt oder setzt das X oder O.</summary>
    [NotMapped]
    public DateTime? SignalDate { get; set; }

    /// <summary>Holt oder setzt den Signal-Beschreibung.</summary>
    [NotMapped]
    public string SignalDescription { get; set; }

    /// <summary>Holt oder setzt den Index 1.</summary>
    [NotMapped]
    public string Index1 { get; set; }

    /// <summary>Holt oder setzt den Index 2.</summary>
    [NotMapped]
    public string Index2 { get; set; }

    /// <summary>Holt oder setzt den Index 3.</summary>
    [NotMapped]
    public string Index3 { get; set; }

    /// <summary>Holt oder setzt den Index 4.</summary>
    [NotMapped]
    public string Index4 { get; set; }

    /// <summary>Holt oder setzt den Durchschnitt 200 Tage.</summary>
    [NotMapped]
    public string Average200 { get; set; }

    /// <summary>Holt oder setzt den Typ (Aktie oder Anleihe).</summary>
    [NotMapped]
    public string Type { get; set; }

    /// <summary>Holt oder setzt die Währung.</summary>
    [NotMapped]
    public string Currency { get; set; }

    /// <summary>Holt oder setzt die Konfiguration.</summary>
    [NotMapped]
    public string Configuration { get; set; }

    protected override string GetExtension()
    {
      var sb = new StringBuilder();
      sb.Append(ToString(CurrentPrice)).Append(';');
      sb.Append(ToString(SignalPrice1)).Append(';');
      sb.Append(ToString(SignalPrice2)).Append(';');
      sb.Append(ToString(StopPrice)).Append(';');
      sb.Append(ToString(Pattern)).Append(';');
      sb.Append(ToString(Sorting)).Append(';');
      sb.Append(ToString(Assessment)).Append(';');
      sb.Append(ToString(Assessment1)).Append(';');
      sb.Append(ToString(Assessment2)).Append(';');
      sb.Append(ToString(Assessment3)).Append(';');
      sb.Append(ToString(Assessment4)).Append(';');
      sb.Append(ToString(Assessment5)).Append(';');
      sb.Append(ToString(Trend1)).Append(';');
      sb.Append(ToString(Trend2)).Append(';');
      sb.Append(ToString(Trend3)).Append(';');
      sb.Append(ToString(Trend4)).Append(';');
      sb.Append(ToString(Trend5)).Append(';');
      sb.Append(ToString(Trend)).Append(';');
      sb.Append(ToString(PriceDate)).Append(';');
      sb.Append(ToString(Xo)).Append(';');
      sb.Append(ToString(SignalAssessment)).Append(';');
      sb.Append(ToString(SignalDate)).Append(';');
      sb.Append(ToString(SignalDescription)).Append(';');
      sb.Append(ToString(Index1)).Append(';');
      sb.Append(ToString(Index2)).Append(';');
      sb.Append(ToString(Index3)).Append(';');
      sb.Append(ToString(Index4)).Append(';');
      sb.Append(ToString(Average200)).Append(';');
      sb.Append(ToString(Configuration)).Append(';'); //  ?? M0(WP010)
      sb.Append(ToString(Type)).Append(';');
      sb.Append(ToString(Currency)).Append(';');
      return sb.ToString();
    }

    protected override void SetExtension(string v)
    {
      var arr = (v ?? "").Split(';');
      CurrentPrice = arr.Length > 0 ? ToDecimal(arr[0]) : null;
      SignalPrice1 = arr.Length > 1 ? ToDecimal(arr[1]) : null;
      SignalPrice2 = arr.Length > 2 ? ToDecimal(arr[2]) : null;
      StopPrice = arr.Length > 3 ? ToDecimal(arr[3]) : null;
      Pattern = arr.Length > 4 ? arr[4] ?? "" : null;
      Sorting = arr.Length > 5 ? arr[5] ?? "" : null;
      Assessment = arr.Length > 6 ? arr[6] ?? "" : null;
      Assessment1 = arr.Length > 7 ? arr[7] ?? "" : null;
      Assessment2 = arr.Length > 8 ? arr[8] ?? "" : null;
      Assessment3 = arr.Length > 9 ? arr[9] ?? "" : null;
      Assessment4 = arr.Length > 10 ? arr[10] ?? "" : null;
      Assessment5 = arr.Length > 11 ? arr[11] ?? "" : null;
      Trend1 = arr.Length > 12 ? arr[12] ?? "" : null;
      Trend2 = arr.Length > 13 ? arr[13] ?? "" : null;
      Trend3 = arr.Length > 14 ? arr[14] ?? "" : null;
      Trend4 = arr.Length > 15 ? arr[15] ?? "" : null;
      Trend5 = arr.Length > 16 ? arr[16] ?? "" : null;
      Trend = arr.Length > 17 ? arr[17] ?? "" : null;
      PriceDate = arr.Length > 18 ? ToDateTime(arr[18]) : null;
      Xo = arr.Length > 19 ? arr[19] ?? "" : null;
      SignalAssessment = arr.Length > 20 ? arr[20] ?? "" : null;
      SignalDate = arr.Length > 21 ? ToDateTime(arr[21]) : null;
      SignalDescription = arr.Length > 22 ? arr[22] ?? "" : null;
      Index1 = arr.Length > 23 ? arr[23] ?? "" : null;
      Index2 = arr.Length > 24 ? arr[24] ?? "" : null;
      Index3 = arr.Length > 25 ? arr[25] ?? "" : null;
      Index4 = arr.Length > 26 ? arr[26] ?? "" : null;
      Average200 = arr.Length > 27 ? arr[27] ?? "" : null;
      Configuration = arr.Length > 28 ? arr[28] ?? "" : null;
      Type = arr.Length > 29 ? arr[29] ?? "" : null;
      Currency = arr.Length > 30 ? arr[30] ?? "" : null;
      ////base.SetExtension(v);
    }
  }
}
