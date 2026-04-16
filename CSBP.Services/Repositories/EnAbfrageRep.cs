// <copyright file="EnAbfrageRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table EN_Abfrage.
/// </summary>
public partial class EnAbfrageRep
{
  /// <summary>
  /// Gets list of queries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="onlyactive">Only active investments or not.</param>
  /// <param name="text">Affected posting text.</param>
  /// <returns>List of queries.</returns>
  public List<EnAbfrage> GetList(ServiceDaten daten, TableReadModel rm = null, bool onlyactive = false, string text = null)
  {
    var db = GetDb(daten);
    var l = db.EN_Abfrage.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!onlyactive)
      l = l.Where(a => a.Status == "0");
    if (CsbpBase.IsLike(text))
      l = l.Where(a => EF.Functions.Like(a.Bezeichnung, text));
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        l = l.Where(a => EF.Functions.Like(a.Sortierung, rm.Search) || EF.Functions.Like(a.Art, rm.Search)
          || EF.Functions.Like(a.Bezeichnung, rm.Search) || EF.Functions.Like(a.Host_Url, rm.Search)
          || EF.Functions.Like(a.Datentyp, rm.Search) || EF.Functions.Like(a.Einheit, rm.Search)
          || EF.Functions.Like(a.Param1, rm.Search) || EF.Functions.Like(a.Param2, rm.Search)
          || EF.Functions.Like(a.Param3, rm.Search) || EF.Functions.Like(a.Param4, rm.Search)
          || EF.Functions.Like(a.Param5, rm.Search) || EF.Functions.Like(a.Notiz, rm.Search));
      }
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
    return l.OrderBy(a => a.Bezeichnung).ToList();
  }
}
