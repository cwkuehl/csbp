// <copyright file="WpBuchung.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Models;

using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CSBP.Services.Base;

/// <summary>
/// Entity class for table WP_Buchung.
/// </summary>
public partial class WpBuchung : ModelBase
{
  /// <summary>Gets or sets the stock description.</summary>
  [NotMapped]
  public string StockDescription { get; set; }

  /// <summary>Gets or sets the investment description.</summary>
  [NotMapped]
  public string InvestmentDescription { get; set; }

  /// <summary>Gets or sets the actuel price.</summary>
  [NotMapped]
  public decimal? Price { get; set; }

  /// <summary>Gets or sets the affected booking id.</summary>
  [NotMapped]
  public string BookingUid { get; set; }

  /// <summary>
  /// Gets all extended values as string.
  /// </summary>
  /// <returns>Extended values as string.</returns>
  protected override string GetExtension()
  {
    var sb = new StringBuilder();
    sb.Append(ToString(BookingUid)).Append(';');
    return sb.ToString();
  }

  /// <summary>
  /// Sets all extended values from string.
  /// </summary>
  /// <param name="value">Extended values as string.</param>
  protected override void SetExtension(string value)
  {
    var arr = (value ?? "").Split(';');
    BookingUid = arr.Length > 0 ? arr[0] ?? "" : "";
  }
}
