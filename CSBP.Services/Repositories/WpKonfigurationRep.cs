// <copyright file="WpKonfigurationRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table WP_Konfiguration.
/// </summary>
public partial class WpKonfigurationRep
{
  /// <summary>
  /// Gets a list of stocks.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="state">Affected state.</param>
  /// <returns>List of configurations.</returns>
  public List<WpKonfiguration> GetList(ServiceDaten daten, TableReadModel rm, string state = null)
  {
    var db = GetDb(daten);
    var l = db.WP_Konfiguration.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (CsbpBase.IsLike(state))
      l = l.Where(a => EF.Functions.Like(a.Status, state));
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        l = l.Where(a => EF.Functions.Like(a.Bezeichnung, rm.Search) || EF.Functions.Like(a.Notiz, rm.Search));
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
