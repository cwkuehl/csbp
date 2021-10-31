// <copyright file="TbOrtRep.cs" company="cwkuehl.de">
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
  /// Klasse für TB_Ort-Repository.
  /// </summary>
  public partial class TbOrtRep
  {
    /// <summary>
    /// Gets a list of positions.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="puid">Affected position ID.</param>
    /// <param name="text">Affected text.</param>
    /// <param name="datege">Affected date greater or equal.</param>
    /// <param name="datele">Affected date lower or equal.</param>
    /// <param name="desc">Sorting order descending?</param>
    /// <param name="max">Maximal amount of records.</param>
    /// <returns>List of positions.</returns>
    public List<TbOrt> GetList(ServiceDaten daten, string puid, string text = null, bool desc = false, int max = 0)
    {
      var db = GetDb(daten);
      var wl = db.TB_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
      if (Functions.IsLike(text))
        wl = wl.Where(a => EF.Functions.Like(a.Bezeichnung, text) || EF.Functions.Like(a.Notiz, text));
      if (!string.IsNullOrEmpty(puid))
        wl = wl.Where(a => a.Uid == puid);
      // var l2 = desc ? wl.OrderByDescending(a => a.Bezeichnung).ThenByDescending(a => a.Uid)
      //     : wl.OrderBy(a => a.Bezeichnung).ThenBy(a => a.Uid);
      // var l3 = l2.Take(max <= 0 ? int.MaxValue : max);
      // return l3.ToList();
      var eo = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
      var gj = wl.ToList().GroupJoin(eo, a => new { a.Mandant_Nr, a.Uid }, b => new { b.Mandant_Nr, Uid = b.Ort_Uid }, (a, b) => new { ort = a, anzahl = b.Sum(a => (a.Datum_Bis - a.Datum_Von).TotalDays) });
      var l2 = desc ? gj.OrderByDescending(a => a.anzahl)
          : gj.OrderBy(a => a.anzahl);
      var l3 = l2.Take(max <= 0 ? int.MaxValue : max).Select(a => a.ort);
      return l3.ToList();
    }

    /// <summary>
    /// Gets a list of positions.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected date.</param>
    /// <returns>List of positions.</returns>
    public List<TbEintragOrt> GetPositionList(ServiceDaten daten, DateTime date)
    {
      var db = GetDb(daten);
      var wl = db.TB_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
      var l = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr
        && a.Datum_Von <= date && a.Datum_Bis >= date);
      var l1 = wl.Join(l, a => new { a.Mandant_Nr, a.Uid }, b => new { b.Mandant_Nr, Uid = b.Ort_Uid }, (a, b) => new { ort = a, b })
        .OrderBy(a => a.ort.Bezeichnung).ToList()
        .Select(a =>
        {
          a.b.Bezeichnung = a.ort.Bezeichnung;
          a.b.Breite = a.ort.Breite;
          a.b.Laenge = a.ort.Laenge;
          a.b.Hoehe = a.ort.Hoehe;
          return a.b;
        });
      return l1.ToList();
    }
  }
}
