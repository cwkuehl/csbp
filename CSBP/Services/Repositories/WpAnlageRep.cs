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
    public List<WpAnlage> GetList(ServiceDaten daten, int mandantnr, string desc,
        string uid = null, string stuid = null)
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
            return a.investment;
          })
          ;
      return l.OrderBy(a => a.Bezeichnung).ToList();
    }
  }
}
