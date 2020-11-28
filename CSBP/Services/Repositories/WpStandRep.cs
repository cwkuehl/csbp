// <copyright file="WpStandRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;

  /// <summary>
  /// Klasse für WP_Stand-Repository.
  /// </summary>
  public partial class WpStandRep
  {
    public WpStand GetLatest(ServiceDaten daten, int mandantnr, string wpuid, DateTime? to = null)
    {
      var db = GetDb(daten);
      var wl = db.WP_Stand.Where(a => a.Mandant_Nr == mandantnr && a.Wertpapier_Uid == wpuid);
      if (to.HasValue)
        wl = wl.Where(a => a.Datum <= to);
      return wl.OrderByDescending(a => a.Datum).FirstOrDefault();
    }

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
  }
}
