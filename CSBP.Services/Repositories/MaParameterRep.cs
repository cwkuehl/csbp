// <copyright file="MaParameterRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table MA_Parameter.
/// </summary>
public partial class MaParameterRep
{
  /// <summary>Sorts the list.</summary>
  /// <param name="l">List of MaParameter.</param>
  /// <param name="rm">Affected table read model.</param>
  /// <returns>Sorted list.</returns>
  public List<MaParameter> SortFilterList(IQueryable<MaParameter> l, TableReadModel rm)
  {
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        l = l.Where(a => EF.Functions.Like(a.Schluessel, rm.Search) || EF.Functions.Like(a.Wert, rm.Search) || EF.Functions.Like(a.Comment, rm.Search) || EF.Functions.Like(a.Default, rm.Search));
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
    return l.OrderBy(a => a.Schluessel).ToList();
  }
}
