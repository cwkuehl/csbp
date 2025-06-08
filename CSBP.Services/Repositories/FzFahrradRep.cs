// <copyright file="FzFahrradRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table FZ_Fahrrad.
/// </summary>
public partial class FzFahrradRep
{
  /// <summary>
  /// Gets list of bikes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="text">Affected posting text.</param>
  /// <returns>List of bikes.</returns>
  public List<FzFahrrad> GetList(ServiceDaten daten, TableReadModel rm = null, string text = null)
  {
    var db = GetDb(daten);
    var l = db.FZ_Fahrrad.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (CsbpBase.IsLike(text))
      l = l.Where(a => EF.Functions.Like(a.Bezeichnung, text));
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        l = l.Where(a => EF.Functions.Like(a.Bezeichnung, rm.Search));
      }
      if (!rm.NoPaging)
      {
        rm.PageCount = rm.RowsPerPage == 0 ? 1 : (int)Math.Ceiling(l.Count() / (decimal)(rm.RowsPerPage ?? 0));
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
