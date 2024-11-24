// <copyright file="WpStandRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;

/// <summary>
/// Repository class for table WP_Stand.
/// </summary>
public partial class WpStandRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets latest price of a stock.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Affected client number.</param>
  /// <param name="wpuid">Affected stock id.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>Latest price or null.</returns>
  public WpStand GetLatest(ServiceDaten daten, int mandantnr, string wpuid, DateTime? to = null)
  {
    var db = GetDb(daten);
    var wl = db.WP_Stand.Where(a => a.Mandant_Nr == mandantnr && a.Wertpapier_Uid == wpuid);
    if (to.HasValue)
      wl = wl.Where(a => a.Datum <= to);
    return wl.OrderByDescending(a => a.Datum).FirstOrDefault();
  }

  /// <summary>
  /// Gets list of prices.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Affected client number.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="uid">Affected stock id.</param>
  /// <param name="max">Maximum of count of entry, 0 means all.</param>
  /// <returns>List of prices.</returns>
  public List<WpStand> GetList(ServiceDaten daten, int mandantnr, DateTime? from, DateTime? to = null,
    string uid = null, int max = 0)
  {
    var db = GetDb(daten);
    var wl = db.WP_Stand.Where(a => a.Mandant_Nr == mandantnr);
    if (from.HasValue)
      wl = wl.Where(a => a.Datum >= from);
    if (to.HasValue)
      wl = wl.Where(a => a.Datum <= to);
    if (!string.IsNullOrEmpty(uid))
      wl = wl.Where(a => a.Wertpapier_Uid == uid);
    var l = wl.GroupJoin(db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr),
      a => a.Wertpapier_Uid, b => b.Uid, (a, b) => new { price = a, stock = b })
      .SelectMany(ab => ab.stock.DefaultIfEmpty(), (a, b) => new { a.price, stock = b })
      .OrderBy(a => a.stock.Uid).ThenByDescending(a => a.price.Datum)
      .Take(max <= 0 ? int.MaxValue : max).ToList()
      .Select(a =>
      {
        if (a.stock != null)
        {
          a.price.StockDescription = a.stock.Bezeichnung;
        }
        return a.price;
      });
    return l.ToList();
  }

#pragma warning disable CA1822
}
