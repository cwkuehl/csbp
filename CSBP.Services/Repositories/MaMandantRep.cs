// <copyright file="MaMandantRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table MA_Mandant.
/// </summary>
public partial class MaMandantRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Executes a SQL command.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="sql">SQL command.</param>
  public void Execute(ServiceDaten daten, string sql)
  {
    var db = GetDb(daten);
    db.Database.ExecuteSqlRaw(sql);
  }

  /// <summary>
  /// Executes many SQL command.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="sql">List of SQL commands.</param>
  public void Execute(ServiceDaten daten, List<string> sql)
  {
    if (sql == null)
      return;
    var db = GetDb(daten);
    foreach (var s in sql)
    {
      db.Database.ExecuteSqlRaw(s);
    }
  }

  /// <summary>
  /// Gets list of entities.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>List of entities.</returns>
  public List<MaMandant> GetList(ServiceDaten daten, TableReadModel rm)
  {
    var db = GetDb(daten);
    var l = db.MA_Mandant.AsNoTracking();
    if (!daten.Daten.Rollen.Contains(UserDaten.RoleSuperadmin))
      l = l.Where(a => a.Nr == daten.MandantNr);
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        l = l.Where(a => EF.Functions.Like(a.Beschreibung, rm.Search));
      }
      if (rm.NoPaging)
      {
        var l1 = SortList(l, rm.SortColumn);
        return l1.ToList();
      }
      else
      {
        rm.PageCount = rm.RowsPerPage == 0 ? 1 : (int)Math.Ceiling(l.Count() / (decimal)(rm.RowsPerPage ?? 0));
        var l1 = SortList(l, rm.SortColumn);
        var page = Math.Max(1, rm.SelectedPage ?? 1) - 1;
        var rowsPerPage = Math.Max(1, rm.RowsPerPage ?? 1);
        var l2 = l1.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
        return l2;
      }
    }
    return l.OrderBy(a => a.Nr).ToList();
  }

#pragma warning restore CA1822
}
