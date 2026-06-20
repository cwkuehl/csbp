// <copyright file="WpAnlageRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table WP_Anlage.
/// </summary>
public partial class WpAnlageRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of investments.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="mandantnr">Affected client.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="onlyactive">Only active investments or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of investments.</returns>
  public List<WpAnlage> GetList(ServiceDaten daten, TableReadModel rm, int mandantnr, string desc,
      string uid = null, string stuid = null, bool onlyactive = false, string search = null)
  {
    var db = GetDb(daten);
    search = Functions.TrimNull(search) ?? rm?.Search;
    var wl = db.WP_Anlage.Where(a => a.Mandant_Nr == mandantnr);
    if (CsbpBase.IsLike(desc))
      wl = wl.Where(a => EF.Functions.Like(a.Bezeichnung, desc));
    if (!string.IsNullOrEmpty(uid))
      wl = wl.Where(a => a.Uid == uid);
    if (!string.IsNullOrEmpty(stuid))
      wl = wl.Where(a => a.Wertpapier_Uid == stuid);
    var l = wl.Join(db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr),
            a => a.Wertpapier_Uid, b => b.Uid, (a, b) => new { investment = a, stock = b })
        .ToList()
        .Select(a =>
        {
          a.investment.StockDescription = a.stock.Bezeichnung;
          a.investment.StockProvider = a.stock.Datenquelle;
          a.investment.StockShortcut = a.stock.Kuerzel;
          a.investment.StockType = a.stock.Type;
          a.investment.StockCurrency = a.stock.Currency;
          a.investment.StockMemo = a.stock.Notiz;
          return a.investment;
        })
        ;
    var ls = CsbpBase.IsLike(search);
    if (ls || onlyactive)
    {
      // l = l.ToList().Where(a => EF.Functions.Like(a.Bezeichnung, text) || EF.Functions.Like(a.Notiz, text)
      //   || EF.Functions.Like(a.StockDescription, text) || EF.Functions.Like(a.StockProvider, text)
      //   || EF.Functions.Like(a.StockShortcut, text) || EF.Functions.Like(a.StockType, text)
      //   || EF.Functions.Like(a.StockCurrency, text));
      var ll = new List<WpAnlage>();
      foreach (var a in l)
      {
        if ((!ls || Like(a.Bezeichnung, search) || Like(a.Notiz, search)
        || Like(a.StockDescription, search) || Like(a.StockProvider, search)
        || Like(a.StockShortcut, search) || Like(a.StockType, search)
        || Like(a.StockCurrency, search) || Like(a.StockMemo, search))
        && (!onlyactive || a.State != 0))
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
        var summe = anz <= 0m ? 0m : l.Sum(e => e.Payment);
        var wert = anz <= 0m ? 0m : l.Sum(e => e.Value);
        var gewinn = anz <= 0m ? 0m : l.Sum(e => e.Profit);
        var diff = anz <= 0m ? 0m : l.Sum(e => e.ValueDiff);
        var pgewinn = (wert == 0 || summe == 0) ? 0 : (gewinn < 0) ? gewinn / wert * 100 : gewinn / summe * 100;
        rm.Essence = Resources.M.WP029(anz, summe, wert, gewinn, pgewinn, diff);
        var l1 = SortList(l.AsQueryable(), rm.SortColumn);
        var page = Math.Max(1, rm.SelectedPage ?? 1) - 1;
        var rowsPerPage = Math.Max(1, rm.RowsPerPage ?? 1);
        var l2 = l1.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
        return l2;
      }
    }
    return l.OrderBy(a => a.Bezeichnung).ToList();
  }

#pragma warning disable CA1822
}
