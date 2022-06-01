// <copyright file="MaMandantRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using CSBP.Apis.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Klasse für MA_Mandant-Repository.
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

#pragma warning restore CA1822
}
