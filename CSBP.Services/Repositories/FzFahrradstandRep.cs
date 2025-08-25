// <copyright file="FzFahrradstandRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models.Views;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table FZ_Fahrradstand.
/// </summary>
public partial class FzFahrradstandRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of mileages.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="buid">Affected bike ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="no">Affected number.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="datege">Affected date greater or equal.</param>
  /// <param name="datele">Affected date lower or equal.</param>
  /// <param name="desc">Sorting order descending or not.</param>
  /// <param name="max">Maximal amount of records.</param>
  /// <returns>List of mileages.</returns>
  public List<VFzFahrradstand> GetList(ServiceDaten daten, TableReadModel rm = null, string buid = null, DateTime? date = null,
    int no = -1, string text = null, DateTime? datege = null, DateTime? datele = null,
    bool desc = false, int max = 0)
  {
    var db = GetDb(daten);
    var wl = db.V_FZ_Fahrradstand.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    if (CsbpBase.IsLike(text))
      wl = wl.Where(a => EF.Functions.Like(a.Beschreibung, text) || EF.Functions.Like(a.Bezeichnung, text));
    if (!string.IsNullOrEmpty(buid))
      wl = wl.Where(a => a.Fahrrad_Uid == buid);
    if (date.HasValue)
      wl = wl.Where(a => a.Datum >= date.Value && a.Datum < date.Value.AddSeconds(1));
    if (datege.HasValue)
      wl = wl.Where(a => a.Datum >= datege.Value);
    if (datele.HasValue)
      wl = wl.Where(a => a.Datum <= datele.Value);
    if (no >= 0)
      wl = wl.Where(a => a.Nr == no);
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        wl = wl.Where(a => EF.Functions.Like(a.Beschreibung, rm.Search) || EF.Functions.Like(a.Bezeichnung, rm.Search));
      }
      if (rm.NoPaging)
      {
        var l1 = SortList(wl, rm.SortColumn);
        return l1.ToList();
      }
      else
      {
        rm.PageCount = rm.RowsPerPage == 0 ? 1 : (int)Math.Ceiling(wl.Count() / (decimal)(rm.RowsPerPage ?? 0));
        var l1 = SortList(wl, rm.SortColumn);
        var page = Math.Max(1, rm.SelectedPage ?? 1) - 1;
        var rowsPerPage = Math.Max(1, rm.RowsPerPage ?? 1);
        var l2 = l1.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
        return l2;
      }
    }
    {
      var l2 = desc ? wl.OrderByDescending(a => a.Datum).ThenByDescending(a => a.Nr) : wl.OrderBy(a => a.Datum).ThenBy(a => a.Nr);
      var l3 = l2.Take(max <= 0 ? int.MaxValue : max).ToList();
      return l3;
    }
  }

  /// <summary>
  /// Gets number of kilometers.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected bike ID.</param>
  /// <param name="datege">Affected min. date.</param>
  /// <param name="datele">Affected max. date.</param>
  /// <returns>Number of kilometers.</returns>
  public decimal Count(ServiceDaten daten, string buid = null, DateTime? datege = null, DateTime? datele = null)
  {
    var db = GetDb(daten);
    var l = db.FZ_Fahrradstand.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(buid))
      l = l.Where(a => a.Fahrrad_Uid == buid);
    if (datege.HasValue)
      l = l.Where(a => a.Datum >= datege.Value);
    if (datele.HasValue)
      l = l.Where(a => a.Datum <= datele.Value);
    var c = l.ToList().Sum(a => a.Periode_km);
    return c;
  }

#pragma warning restore CA1822
}
