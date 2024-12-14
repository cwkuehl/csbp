// <copyright file="TbOrtRep.cs" company="cwkuehl.de">
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
/// Repository class for table TB_Ort.
/// </summary>
public partial class TbOrtRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of positions.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="desc">Sorting order descending or not.</param>
  /// <param name="max">Maximal amount of records.</param>
  /// <returns>List of positions.</returns>
  public List<TbOrt> GetList(ServiceDaten daten, string puid, string text = null, bool desc = false, int max = 0)
  {
    var db = GetDb(daten);
    var wl = db.TB_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    if (CsbpBase.IsLike(text))
      wl = wl.Where(a => EF.Functions.Like(a.Bezeichnung, text) || EF.Functions.Like(a.Notiz, text));
    if (!string.IsNullOrEmpty(puid))
      wl = wl.Where(a => a.Uid == puid);
    var eo = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    var gj = wl.ToList().GroupJoin(eo, a => new { a.Mandant_Nr, a.Uid }, b => new { b.Mandant_Nr, Uid = b.Ort_Uid }, (a, b) => new { ort = a, anzahl = b.Sum(a => (a.Datum_Bis - a.Datum_Von).TotalDays + 1) });
    var l2 = desc ? gj.OrderByDescending(a => a.anzahl).ThenByDescending(a => a.ort.Bezeichnung)
        : gj.OrderBy(a => a.anzahl).ThenBy(a => a.ort.Bezeichnung);
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
        a.b.Description = a.ort.Bezeichnung;
        a.b.Latitude = a.ort.Breite;
        a.b.Longitude = a.ort.Laenge;
        a.b.Height = a.ort.Hoehe;
        a.b.Memo = a.ort.Notiz;
        return a.b;
      });
    return l1.ToList();
  }

#pragma warning restore CA1822
}
