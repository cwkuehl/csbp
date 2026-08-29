// <copyright file="HhKontoRep.cs" company="cwkuehl.de">
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
/// Repository class for table HH_Konto.
/// </summary>
public partial class HhKontoRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list of accounts.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="nrle">Affected minimum period number.</param>
  /// <param name="nrge">Affected maximum period number.</param>
  /// <param name="art1">Affected first account type.</param>
  /// <param name="art2">Affected second account type.</param>
  /// <param name="dle">Affected minimum period date.</param>
  /// <param name="dge">Affected maximum period date.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of accounts.</returns>
  public List<HhKonto> GetList(ServiceDaten daten, TableReadModel rm, int nrle, int nrge, string art1 = null, string art2 = null,
      DateTime? dle = null, DateTime? dge = null, string search = null)
  {
    var db = GetDb(daten);
    search = Functions.TrimNull(search) ?? rm?.Search;
    var l = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (nrle >= 0)
      l = l.Where(a => a.Periode_Von <= nrle);
    if (nrge >= 0)
      l = l.Where(a => a.Periode_Bis >= nrge);
    if (!string.IsNullOrEmpty(art1) && !string.IsNullOrEmpty(art2))
      l = l.Where(a => a.Art == art1 || a.Art == art2);
    else if (!string.IsNullOrEmpty(art1))
      l = l.Where(a => a.Art == art1);
    if (dle.HasValue)
      l = l.Where(a => a.Gueltig_Von == null || a.Gueltig_Von <= dle.Value);
    if (dge.HasValue)
      l = l.Where(a => a.Gueltig_Bis == null || a.Gueltig_Bis >= dge.Value);
    if (CsbpBase.IsLike(search))
      l = l.Where(a => EF.Functions.Like(a.Uid, search) || EF.Functions.Like(a.Name, search)
        || EF.Functions.Like(a.Art, search) || EF.Functions.Like(a.Kz, search));
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (rm.NoPaging)
      {
        var l1 = SortList(l, rm.SortColumn);
        return l1.ToList();
      }
      else
      {
        rm.PageCount = rm.RowsPerPage == 0 ? 1 : (int)Math.Ceiling(l.Count() / (decimal)(rm.RowsPerPage ?? 0));
        rm.Essence = Resources.M.M1040(l.Count());
        var l1 = SortList(l, rm.SortColumn);
        var page = Math.Max(1, rm.SelectedPage ?? 1) - 1;
        var rowsPerPage = Math.Max(1, rm.RowsPerPage ?? 1);
        var l2 = l1.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
        return l2;
      }
    }
    return l.OrderBy(a => a.Mandant_Nr).ThenBy(a => a.Name).ThenBy(a => a.Uid).ToList();
  }

  /// <summary>
  /// Gets an account.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auidne">Affected ID which is not equal.</param>
  /// <param name="attr">Affected attribute.</param>
  /// <param name="sort">Affected sotring.</param>
  /// <param name="desc">Affected description.</param>
  /// <returns>Account or null.</returns>
  public HhKonto GetMin(ServiceDaten daten, string auidne, string attr = null, string sort = null, string desc = null)
  {
    var db = GetDb(daten);
    var l = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(auidne))
      l = l.Where(a => a.Uid != auidne);
    if (!string.IsNullOrEmpty(attr))
      l = l.Where(a => a.Kz == attr);
    if (!string.IsNullOrEmpty(sort))
      l = l.Where(a => a.Sortierung == sort);
    if (!string.IsNullOrEmpty(desc))
      l = l.Where(a => a.Name == desc);
    return l.OrderBy(a => a.Mandant_Nr).ThenBy(a => a.Uid).FirstOrDefault();
  }

#pragma warning restore CA1822
}
