// <copyright file="WpBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models
{
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Text;
  using CSBP.Base;

  /// <summary>
  /// Entity-Klasse für Tabelle WP_Buchung.
  /// </summary>
  public partial class WpBuchung : ModelBase
  {
    /// <summary>Holt oder setzt die Wertpapier-Bezeichnung.</summary>
    [NotMapped]
    public string StockDescription { get; set; }

    /// <summary>Holt oder setzt die Anlage-Bezeichnung.</summary>
    [NotMapped]
    public string InvestmentDescription { get; set; }

    /// <summary>Holt oder setzt den aktuellen Preis.</summary>
    [NotMapped]
    public decimal? Price { get; set; }

    /// <summary>Holt oder setzt die zugehörige Buchung.</summary>
    [NotMapped]
    public string BookingUid { get; set; }

    protected override string GetExtension()
    {
      var sb = new StringBuilder();
      sb.Append(ToString(BookingUid)).Append(";");
      return sb.ToString();
    }

    protected override void SetExtension(string value)
    {
      var arr = (value ?? "").Split(';');
      BookingUid = arr.Length > 0 ? arr[0] ?? "" : "";
    }
  }
}
