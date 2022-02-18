// <copyright file="HhEreignisRep.cs" company="cwkuehl.de">
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
  /// Klasse für HH_Ereignis-Repository.
  /// </summary>
  public partial class HhEreignisRep
  {
    /// <summary>
    /// Gets list of events.
    /// </summary>
    /// <returns>List of events.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auid">Affected account ID.</param>
    /// <param name="from">Affected minimum date.</param>
    /// <param name="to">Affected maximum date.</param>
    /// <param name="text">Affected text.</param>
    public List<HhEreignis> GetList(ServiceDaten daten, string auid, DateTime? from = null,
        DateTime? to = null, string text = null)
    {
      var db = GetDb(daten);
      var l = db.HH_Ereignis.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (!string.IsNullOrEmpty(auid))
        l = l.Where(a => a.Soll_Konto_Uid == auid || a.Haben_Konto_Uid == auid);
      var sl = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
      var hl = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (from.HasValue)
      {
        sl = sl.Where(a => a.Gueltig_Von == null || a.Gueltig_Von <= from);
        hl = hl.Where(a => a.Gueltig_Von == null || a.Gueltig_Von <= from);
      }
      if (to.HasValue)
      {
        sl = sl.Where(a => a.Gueltig_Bis == null || a.Gueltig_Bis >= to);
        hl = hl.Where(a => a.Gueltig_Bis == null || a.Gueltig_Bis >= to);
      }
      if (Functions.IsLike(text))
        l = l.Where(a => EF.Functions.Like(a.Bezeichnung, text) || EF.Functions.Like(a.EText, text)
          || EF.Functions.Like(a.Kz, text));
      var l2 = l.Join(sl, a => a.Soll_Konto_Uid, b => b.Uid, (a, b) => new { ev = a, debit = b });
      var l3 = l2.Join(hl, a => a.ev.Haben_Konto_Uid, b => b.Uid, (a, b) => new { a.ev, a.debit, credit = b });
      var l4 = l3.OrderBy(a => a.ev.Mandant_Nr).ThenBy(a => a.ev.Bezeichnung).ThenBy(a => a.ev.Uid).ToList()
        .Select(a =>
        {
          var e = a.ev;
          e.DebitName = a.debit.Name;
          e.DebitFrom = a.debit.Gueltig_Von;
          e.DebitTo = a.debit.Gueltig_Bis;
          e.CreditName = a.credit.Name;
          e.CreditFrom = a.credit.Gueltig_Von;
          e.CreditTo = a.credit.Gueltig_Bis;
          return e;
        });
      return l4.ToList();
    }
  }
}
