// <copyright file="WpAnlage.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Models;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using CSBP.Base;

/// <summary>
/// Entity class for table WP_Anlage.
/// </summary>
public partial class WpAnlage : ModelBase
{
  /// <summary>Gets or sets the stock description.</summary>
  [NotMapped]
  public string StockDescription { get; set; }

  /// <summary>Gets or sets the provider.</summary>
  [NotMapped]
  public string StockProvider { get; set; }

  /// <summary>Gets or sets the shortcut.</summary>
  [NotMapped]
  public string StockShortcut { get; set; }

  /// <summary>Gets or sets the stock type.</summary>
  [NotMapped]
  public string StockType { get; set; }

  /// <summary>Gets or sets the stock currency.</summary>
  [NotMapped]
  public string StockCurrency { get; set; }

  /// <summary>Gets or sets stock memo.</summary>
  [NotMapped]
  public string StockMemo { get; set; }

  /// <summary>Gets or sets the data.</summary>
  [NotMapped]
  public string Data { get; set; }

  /// <summary>Gets or sets the purchase payment.</summary>
  [NotMapped]
  public decimal Payment { get; set; }

  /// <summary>Gets or sets the shares.</summary>
  [NotMapped]
  public decimal Shares { get; set; }

  /// <summary>Gets or sets the share value.</summary>
  [NotMapped]
  public decimal ShareValue { get; set; }

  /// <summary>Gets or sets the interest.</summary>
  [NotMapped]
  public decimal Interest { get; set; }

  /// <summary>Gets or sets the price per share.</summary>
  [NotMapped]
  public decimal Price { get; set; }

  /// <summary>Gets or sets the price date.</summary>
  [NotMapped]
  public DateTime? PriceDate { get; set; }

  /// <summary>Gets or sets the value of all shares.</summary>
  [NotMapped]
  public decimal Value { get; set; }

  /// <summary>Gets or sets the profit or loss.</summary>
  [NotMapped]
  public decimal Profit { get; set; }

  /// <summary>Gets or sets the profit percentage.</summary>
  [NotMapped]
  public decimal ProfitPercent { get; set; }

  /// <summary>Gets or sets the currency.</summary>
  [NotMapped]
  public string Currency { get; set; }

  /// <summary>Gets or sets the currency price to euro.</summary>
  [NotMapped]
  public decimal CurrencyPrice { get; set; }

  /// <summary>Gets or sets the first booking date.</summary>
  [NotMapped]
  public DateTime? MinDate { get; set; }

  /// <summary>Gets or sets the state.</summary>
  [NotMapped]
  public int State { get; set; }

  /// <summary>Gets or sets the previous price date.</summary>
  [NotMapped]
  public DateTime? PriceDate2 { get; set; }

  /// <summary>Gets or sets the value of all shares of previous date.</summary>
  [NotMapped]
  public decimal Value2 { get; set; }

  /// <summary>Gets or sets the portfolio account id.</summary>
  [NotMapped]
  public string PortfolioAccountUid { get; set; }

  /// <summary>Gets or sets the settlement account id.</summary>
  [NotMapped]
  public string SettlementAccountUid { get; set; }

  /// <summary>Gets or sets the income account id.</summary>
  [NotMapped]
  public string IncomeAccountUid { get; set; }

  /// <summary>
  /// Gets all extended values as string.
  /// </summary>
  /// <returns>Extended values as string.</returns>
  protected override string GetExtension()
  {
    var sb = new StringBuilder();
    sb.Append(ToString(Payment, 2)).Append(';');
    sb.Append(ToString(Shares, 5)).Append(';');
    sb.Append(ToString(ShareValue, 4)).Append(';');
    sb.Append(ToString(Interest, 2)).Append(';');
    sb.Append(ToString(Price, 4)).Append(';');
    sb.Append(ToString(PriceDate)).Append(';');
    sb.Append(ToString(Value, 2)).Append(';');
    sb.Append(ToString(Profit, 2)).Append(';');
    sb.Append(ToString(ProfitPercent, 2)).Append(';');
    sb.Append(ToString(Currency)).Append(';');
    sb.Append(ToString(CurrencyPrice, 4)).Append(';');
    sb.Append(ToString(MinDate)).Append(';');
    sb.Append(ToString(State)).Append(';');
    sb.Append(ToString(PriceDate2)).Append(';');
    sb.Append(ToString(Value2, 4)).Append(';');
    sb.Append(ToString(PortfolioAccountUid)).Append(';');
    sb.Append(ToString(SettlementAccountUid)).Append(';');
    sb.Append(ToString(IncomeAccountUid)).Append(';');
    return sb.ToString();
  }

  /// <summary>
  /// Sets all extended values from string.
  /// </summary>
  /// <param name="value">Extended values as string.</param>
  protected override void SetExtension(string value)
  {
    var arr = (value ?? "").Split(';');
    Payment = arr.Length > 0 ? ToDecimal(arr[0]) ?? 0 : 0;
    Shares = arr.Length > 1 ? ToDecimal(arr[1]) ?? 0 : 0;
    ShareValue = arr.Length > 2 ? ToDecimal(arr[2]) ?? 0 : 0;
    Interest = arr.Length > 3 ? ToDecimal(arr[3]) ?? 0 : 0;
    Price = arr.Length > 4 ? ToDecimal(arr[4]) ?? 0 : 0;
    PriceDate = arr.Length > 5 ? ToDateTime(arr[5]) : null;
    Value = arr.Length > 6 ? ToDecimal(arr[6]) ?? 0 : 0;
    Profit = arr.Length > 7 ? ToDecimal(arr[7]) ?? 0 : 0;
    ProfitPercent = arr.Length > 8 ? ToDecimal(arr[8]) ?? 0 : 0;
    Currency = arr.Length > 9 ? arr[9] ?? "" : "";
    CurrencyPrice = arr.Length > 10 ? ToDecimal(arr[10]) ?? 0 : 0;
    MinDate = arr.Length > 11 ? ToDateTime(arr[11]) : null;
    State = arr.Length > 12 ? ToInt(arr[12]) ?? 1 : 1;
    PriceDate2 = arr.Length > 13 ? ToDateTime(arr[13]) : null;
    Value2 = arr.Length > 14 ? ToDecimal(arr[14]) ?? 0 : 0;
    PortfolioAccountUid = arr.Length > 15 ? arr[15] ?? "" : "";
    SettlementAccountUid = arr.Length > 16 ? arr[16] ?? "" : "";
    IncomeAccountUid = arr.Length > 17 ? arr[17] ?? "" : "";
  }
}
