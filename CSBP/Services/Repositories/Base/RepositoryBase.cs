// <copyright file="RepositoryBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base;

using System;
using System.Data.Common;
using CSBP.Apis.Services;
using CSBP.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Base class for repositories.
/// </summary>
public class RepositoryBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
  /// </summary>
  public RepositoryBase()
  {
    Functions.MachNichts();
  }

  /// <summary>
  /// Sets the columns Angelegt_Von and Angelegt_Am.
  /// </summary>
  /// <param name="b">Entity as ModelBase.</param>
  /// <param name="daten">Service data with time and user id.</param>
  /// <param name="now">Affected time stamp.</param>
  /// <param name="from">Affected user id.</param>
  public static void MachAngelegt(ModelBase b, ServiceDaten daten, DateTime? now = null, string from = null)
  {
    if (b == null || daten == null)
      return;
    if (from == null)
      b.MachAngelegt(daten.Jetzt, daten.BenutzerId);
    else
      b.MachAngelegt(now, from);
  }

  /// <summary>
  /// Sets the columns Geaendert_Von and Geaendert_Am,
  /// only when the last entry in column Angelegt_Am or Geaendert_Am outdates AEND_ZEIT ticks.
  /// </summary>
  /// <param name="b">Entity as ModelBase.</param>
  /// <param name="daten">Service data with time and user id.</param>
  /// <param name="now">Affected time stamp.</param>
  /// <param name="from">Affected user id.</param>
  public static void MachGeaendert(ModelBase b, ServiceDaten daten, DateTime? now = null, string from = null)
  {
    if (b == null || daten == null)
      return;
    if (from == null)
      b.MachGeaendert(daten.Jetzt, daten.BenutzerId);
    else
      b.MachGeaendert(now, from);
  }

  /// <summary>
  /// Gets database context from service data.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Database context from service data.</returns>
  protected static CsbpContext GetDb(ServiceDaten daten)
  {
    var db = daten.Context as CsbpContext;
    return db;
  }

  /// <summary>
  /// Gets database connection string from service data.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Database connection string from service data.</returns>
  protected static DbConnection GetCon(ServiceDaten daten)
  {
    var db = daten.Context as DbContext;
    var con = db.Database.GetDbConnection();
    return con;
  }

  /// <summary>
  /// Adds parameter to command.
  /// </summary>
  /// <param name="cmd">Affected command.</param>
  /// <param name="name">Affected parameter name.</param>
  /// <param name="value">Affected parameter value.</param>
  protected static void AddParam(DbCommand cmd, string name, object value)
  {
    var p = cmd.CreateParameter();
    p.ParameterName = name;
    p.Value = value;
    cmd.Parameters.Add(p);
  }

  /// <summary>
  /// Simulation of function like with StartsWith, Contains and EndsWith.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <param name="exp">Affected like expression.</param>
  /// <returns>The string fulfills the like expression.</returns>
  protected static bool Like(string s, string exp)
  {
    // The 'Like' method is not supported because the query has switched to client-evaluation.
    // This usually happens when the arguments to the method cannot be translated to server.
    // Rewrite the query to avoid client evaluation of arguments so that method can be translated to server.
    if (!Functions.IsLike(exp))
      return true;
    if (string.IsNullOrEmpty(s))
      return false;
    var arr = exp.Split('%', StringSplitOptions.None);
    if (!string.IsNullOrEmpty(arr[0]))
      if (!s.StartsWith(arr[0], StringComparison.CurrentCultureIgnoreCase))
        return false;
    if (arr.Length > 1 && !string.IsNullOrEmpty(arr[^1]))
      if (!s.EndsWith(arr[^1], StringComparison.CurrentCultureIgnoreCase))
        return false;
    for (var i = 1; i < arr.Length - 1; i++)
    {
      if (!string.IsNullOrEmpty(arr[i]) && !s.Contains(arr[i], StringComparison.CurrentCultureIgnoreCase))
        return false;
    }
    return true;

    // return EF.Functions.Like(s, exp);
    // 'The 'Like' method is not supported because the query has switched to client-evaluation. This usually happens when the arguments to the
    // method cannot be translated to server. Rewrite the query to avoid client evaluation of arguments so that method can be translated to server.'
  }
}
