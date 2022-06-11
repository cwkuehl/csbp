// <copyright file="WpWertpapierRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Klasse für WP_Wertpapier-Repository.
/// </summary>
public partial class WpWertpapierRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of stocks.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Affected client.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="pattern">Affected Pattern.</param>
  /// <param name="uid">Affected stock ID.</param>
  /// <param name="neuid">Not affected ID.</param>
  /// <param name="onlyactive">Only active investments or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="reuid">Affected related stock ID.</param>
  /// <returns>List of stocks.</returns>
  public List<WpWertpapier> GetList(ServiceDaten daten, int mandantnr, string desc,
    string pattern = null, string uid = null, string neuid = null,
    bool onlyactive = false, string search = null, string reuid = null)
  {
    var db = GetDb(daten);
    var wl = db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr);
    if (Functions.IsLike(desc))
      wl = wl.Where(a => EF.Functions.Like(a.Bezeichnung, desc));
    if (!string.IsNullOrEmpty(uid))
      wl = wl.Where(a => a.Uid == uid);
    if (!string.IsNullOrEmpty(neuid))
      wl = wl.Where(a => a.Uid != neuid);
    if (!string.IsNullOrEmpty(reuid))
      wl = wl.Where(a => a.Relation_Uid == reuid);
    var l = wl.GroupJoin(db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr),
      a => a.Relation_Uid, b => b.Uid, (a, b) => new { stock = a, relation = b })
      .SelectMany(ab => ab.relation.DefaultIfEmpty(), (a, b) => new { a.stock, relation = b })
      .ToList()
      .Select(a =>
      {
        if (a.relation != null)
        {
          a.stock.RelationDescription = a.relation.Bezeichnung;
          a.stock.RelationSource = a.relation.Datenquelle;
          a.stock.RelationShortcut = a.relation.Kuerzel;
        }
        return a.stock;
      });
    var ls = Functions.IsLike(search);
    var lp = Functions.IsLike(pattern);
    if (ls || lp || onlyactive)
    {
      // l = l.ToList().Where(a => EF.Functions.Like(a.Bezeichnung, text) || EF.Functions.Like(a.Notiz, text)
      //   || EF.Functions.Like(a.StockDescription, text) || EF.Functions.Like(a.StockProvider, text)
      //   || EF.Functions.Like(a.StockShortcut, text) || EF.Functions.Like(a.StockType, text)
      //   || EF.Functions.Like(a.StockCurrency, text));
      var ll = new List<WpWertpapier>();
      foreach (var a in l)
      {
        if ((!ls || (Like(a.Bezeichnung, search) || Like(a.Notiz, search)
          || Like(a.Datenquelle, search) || Like(a.Kuerzel, search)
          || Like(a.Type, search) || Like(a.Currency, search) || Like(a.Sorting, search)))
          && (!lp || Like(a.Pattern, pattern))
          && (!onlyactive || a.Status != "0"))
          ll.Add(a);
      }
      l = ll;
    }
    return l.OrderBy(a => a.Bezeichnung).ToList();
  }

#pragma warning restore CA1822
}
