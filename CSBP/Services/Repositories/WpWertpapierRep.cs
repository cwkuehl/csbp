// <copyright file="WpWertpapierRep.cs" company="cwkuehl.de">
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
  /// Klasse für WP_Wertpapier-Repository.
  /// </summary>
  public partial class WpWertpapierRep
  {
    public List<WpWertpapier> GetList(ServiceDaten daten, int mandantnr, string desc,
      string pattern = null, string uid = null, string neuid = null, string relation = null,
      bool onlyactive = false)
    {
      var db = GetDb(daten);
      var wl = db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr);
      if (Functions.IsLike(desc))
        wl = wl.Where(a => EF.Functions.Like(a.Bezeichnung, desc));
      if (Functions.IsLike(pattern))
        wl = wl.Where(a => EF.Functions.Like(a.Pattern, pattern));
      if (!string.IsNullOrEmpty(uid))
        wl = wl.Where(a => a.Uid == uid);
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
            a.stock.RelationAbbreviation = a.relation.Kuerzel;
          }
          return a.stock;
        });
      return l.OrderBy(a => a.Bezeichnung).ToList();
    }
  }
}
