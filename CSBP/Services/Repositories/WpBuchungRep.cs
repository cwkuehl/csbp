// <copyright file="WpBuchungRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table WP_Buchung.
/// </summary>
public partial class WpBuchungRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of bookings.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Affected client number.</param>
  /// <param name="text">Affected description.</param>
  /// <param name="uid">Affected uid.</param>
  /// <param name="stuid">Affected stock uid.</param>
  /// <param name="inuid">Affected investment uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="desc">Descending by date or not.</param>
  /// <returns>List of bookings.</returns>
  public List<WpBuchung> GetList(ServiceDaten daten, int mandantnr, string text, string uid = null,
      string stuid = null, string inuid = null, DateTime? from = null, DateTime? to = null, bool desc = false)
  {
    var db = GetDb(daten);
    var wl = db.WP_Buchung.Where(a => a.Mandant_Nr == mandantnr);
    if (Functions.IsLike(text))
      wl = wl.Where(a => EF.Functions.Like(a.BText, text));
    if (!string.IsNullOrEmpty(uid))
      wl = wl.Where(a => a.Uid == uid);
    if (!string.IsNullOrEmpty(stuid))
      wl = wl.Where(a => a.Wertpapier_Uid == stuid);
    if (!string.IsNullOrEmpty(inuid))
      wl = wl.Where(a => a.Anlage_Uid == inuid);
    if (from.HasValue)
      wl = wl.Where(a => a.Datum >= from.Value);
    if (to.HasValue)
      wl = wl.Where(a => a.Datum <= to.Value);
    var l = wl.Join(db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr),
      a => a.Wertpapier_Uid, b => b.Uid, (a, b) => new { booking = a, stock = b })
      .Join(db.WP_Anlage.Where(a => a.Mandant_Nr == mandantnr),
      a => a.booking.Anlage_Uid, b => b.Uid, (a, b) => new { a.booking, a.stock, investment = b })
      .GroupJoin(db.WP_Stand.Where(a => a.Mandant_Nr == mandantnr),
      a => new { a.booking.Wertpapier_Uid, a.booking.Datum },
      b => new { b.Wertpapier_Uid, b.Datum }, (a, b) => new { a.booking, a.stock, a.investment, price = b })
      .SelectMany(ab => ab.price.DefaultIfEmpty(), (a, b) => new { a.booking, a.stock, a.investment, price = b })
      .ToList()
      .Select(a =>
      {
        a.booking.StockDescription = a.stock.Bezeichnung;
        a.booking.InvestmentDescription = a.investment.Bezeichnung;
        a.booking.Price = a.price?.Stueckpreis;
        return a.booking;
      });
    if (desc)
      return l.OrderByDescending(a => a.Datum).ToList();
    return l.OrderBy(a => a.Datum).ToList();
  }

#pragma warning restore CA1822
}
