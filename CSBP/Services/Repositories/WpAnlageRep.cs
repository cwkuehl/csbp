// <copyright file="WpAnlageRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Klasse für WP_Anlage-Repository.
  /// </summary>
  public partial class WpAnlageRep
  {
    /// <summary>
    /// Gets a list of investments.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="mandantnr">Affected client.</param>
    /// <param name="desc">Affected Description.</param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="stuid">Affected stock ID.</param>
    /// <param name="search">Affected text search.</param>
    /// <returns>List of investments.</returns>
    public List<WpAnlage> GetList(ServiceDaten daten, int mandantnr, string desc,
        string uid = null, string stuid = null, string search = null)
    {
      var db = GetDb(daten);
      var wl = db.WP_Anlage.Where(a => a.Mandant_Nr == mandantnr);
      if (Functions.IsLike(desc))
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
      if (Functions.IsLike(search))
      {
        // l = l.ToList().Where(a => EF.Functions.Like(a.Bezeichnung, text) || EF.Functions.Like(a.Notiz, text)
        //   || EF.Functions.Like(a.StockDescription, text) || EF.Functions.Like(a.StockProvider, text)
        //   || EF.Functions.Like(a.StockShortcut, text) || EF.Functions.Like(a.StockType, text)
        //   || EF.Functions.Like(a.StockCurrency, text));
        var ll = new List<WpAnlage>();
        foreach (var a in l)
        {
          if (Like(a.Bezeichnung, search) || Like(a.Notiz, search)
          || Like(a.StockDescription, search) || Like(a.StockProvider, search)
          || Like(a.StockShortcut, search) || Like(a.StockType, search)
          || Like(a.StockCurrency, search) || Like(a.StockMemo, search))
            ll.Add(a);
        }
        l = ll;
      }
      return l.OrderBy(a => a.Bezeichnung).ToList();
    }
  }
}
