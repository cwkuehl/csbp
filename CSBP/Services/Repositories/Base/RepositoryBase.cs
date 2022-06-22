// <copyright file="RepositoryBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base
{
  using System;
  using System.Data.Common;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Basis-Klasse für Repositories.
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
    /// Liefert einen Datenbank-Context aus Service-Daten.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Datenbank-Context aus Service-Daten.</returns>
    protected static CsbpContext GetDb(ServiceDaten daten)
    {
      var db = daten.Context as CsbpContext;
      return db;
    }

    /// <summary>
    /// Liefert eine Datenbank-Connection aus Service-Daten.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Datenbank-Connection aus Service-Daten.</returns>
    protected static DbConnection GetCon(ServiceDaten daten)
    {
      var db = daten.Context as DbContext;
      var con = db.Database.GetDbConnection();
      return con;
    }

    /// <summary>
    /// Fügt einem Command einen Parameter hinzu.
    /// </summary>
    /// <param name="cmd">Betroffenes Command.</param>
    /// <param name="name">Betroffener Parameter-Name.</param>
    /// <param name="value">Betroffener Parameter-Wert.</param>
    protected static void AddParam(DbCommand cmd, string name, object value)
    {
      var p = cmd.CreateParameter();
      p.ParameterName = name;
      p.Value = value;
      cmd.Parameters.Add(p);
    }

    /// <summary>
    /// Nachbildung der Funktion Like mit StartsWith, Contains und EndsWith.
    /// </summary>
    /// <param name="s">Zu prüfender String.</param>
    /// <param name="exp">Betroffene Like-Expression.</param>
    /// <returns></returns>
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
    }

    /// <summary>
    /// Eintragungen in Spalten Angelegt_Von und Angelegt_Am.
    /// </summary>
    /// <param name="b">Entity als ModelBase.</param>
    /// <param name="daten">Service-Daten mit Zeit und Benutzer-ID.</param>
    /// <param name="now">Betroffener Zeitpunkt.</param>
    /// <param name="from">Betroffene Benutzer-ID.</param>
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
    /// Eintragungen in Spalten Geaendert_Von und Geaendert_Am,
    /// wenn der letzte Eintrag in den Spalten Angelegt_Am oder Geaendert_Am schon AEND_ZEIT Ticks her ist.
    /// </summary>
    /// <param name="b">Entity als ModelBase.</param>
    /// <param name="daten">Service-Daten mit Zeit und Benutzer-ID.</param>
    /// <param name="now">Betroffener Zeitpunkt.</param>
    /// <param name="from">Betroffene Benutzer-ID.</param>
    public static void MachGeaendert(ModelBase b, ServiceDaten daten, DateTime? now = null, string from = null)
    {
      if (b == null || daten == null)
        return;
      if (from == null)
        b.MachGeaendert(daten.Jetzt, daten.BenutzerId);
      else
        b.MachGeaendert(now, from);
    }
  }
}
