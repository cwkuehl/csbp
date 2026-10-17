// <copyright file="WpBuchungRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using BlazorSpa.Base;
using BlazorSpa.Base.Models;
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
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="mandantnr">Affected client number.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="uid">Affected booking uid.</param>
  /// <param name="stuid">Affected stock uid.</param>
  /// <param name="inuid">Affected investment uid.</param>
  /// <param name="buid">Affected budget booking uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="desc">Descending by date or not.</param>
  /// <returns>List of bookings.</returns>
  public List<WpBuchung> GetList(ServiceDaten daten, TableReadModel rm, int mandantnr, string search, string uid = null,
    string stuid = null, string inuid = null, string buid = null, DateTime? from = null, DateTime? to = null,
    bool desc = false)
  {
    var db = GetDb(daten);
    search = search.TrimNull() ?? rm?.Search;
    var wl = db.WP_Buchung.Where(a => a.Mandant_Nr == mandantnr);
    //// if (CsbpBase.IsLike(search))
    ////   wl = wl.Where(a => EF.Functions.Like(a.BText, search) || EF.Functions.Like(a.Notiz, search));
    if (!string.IsNullOrEmpty(uid))
      wl = wl.Where(a => a.Uid == uid);
    if (!string.IsNullOrEmpty(stuid))
      wl = wl.Where(a => a.Wertpapier_Uid == stuid);
    if (!string.IsNullOrEmpty(inuid))
      wl = wl.Where(a => a.Anlage_Uid == inuid);
    if (!string.IsNullOrEmpty(buid))
      wl = wl.Where(a => EF.Functions.Like(a.Notiz, buid));
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
    var ls = CsbpBase.IsLike(search);
    if (ls)
    {
      var ll = new List<WpBuchung>();
      foreach (var a in l)
      {
        if (!ls || Like(a.BText, search) || Like(a.Notiz, search)
        || Like(a.StockDescription, search) || Like(a.InvestmentDescription, search))
          ll.Add(a);
      }
      l = ll;
    }
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      // Gesucht wurde schon oben.
      if (rm.NoPaging)
      {
        var l1 = SortList(l.AsQueryable(), rm.SortColumn);
        return l1.ToList();
      }
      else
      {
        rm.PageCount = rm.RowsPerPage == 0 ? 1 : (int)Math.Ceiling(l.Count() / (decimal)(rm.RowsPerPage ?? 0));
        var anz = l.Count();
        var summe = anz <= 0m ? 0m : l.Sum(e => e.Zahlungsbetrag);
        rm.Essence = Resources.M.HH054(anz, summe);
        var l1 = SortList(l.AsQueryable(), rm.SortColumn);
        var page = Math.Max(1, rm.SelectedPage ?? 1) - 1;
        var rowsPerPage = Math.Max(1, rm.RowsPerPage ?? 1);
        var l2 = l1.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
        return l2;
      }
    }
    if (desc)
      return l.OrderByDescending(a => a.Datum).ToList();
    return l.OrderBy(a => a.Datum).ToList();
  }

#pragma warning restore CA1822
}
