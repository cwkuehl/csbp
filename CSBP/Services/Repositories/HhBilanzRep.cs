// <copyright file="HhBilanzRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Klasse für HH_Bilanz-Repository.
/// </summary>
public partial class HhBilanzRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list of balances.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="nrfrom">Affected minimum period number.</param>
  /// <param name="nrto">Affected maximum period number.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <returns>List of balances.</returns>
  public List<HhBilanz> GetList(ServiceDaten daten, string type, int nrfrom = Constants.PN_BERECHNET,
     int nrto = Constants.PN_BERECHNET, string auid = null)
  {
    var db = GetDb(daten);
    var l = db.HH_Bilanz.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(auid))
      l = l.Where(a => a.Konto_Uid == auid);
    if (!string.IsNullOrEmpty(type))
      l = l.Where(a => a.Kz == type);
    if (nrfrom > Constants.PN_BERECHNET)
    {
      if (nrfrom == nrto)
        l = l.Where(a => a.Periode == nrfrom);
      else
        l = l.Where(a => a.Periode >= nrfrom);
    }
    if (nrto > Constants.PN_BERECHNET && nrfrom != nrto)
    {
      l = l.Where(a => a.Periode <= nrto);
    }
    return l.ToList();
  }

  /// <summary>
  /// Gets list of balances with account sums over periods.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="nrfrom">Affected minimum period number.</param>
  /// <param name="nrto">Affected maximum period number.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <returns>List of balances.</returns>
  public List<HhBilanz> GetPeriodSumList(ServiceDaten daten, string type, int nrfrom = Constants.PN_BERECHNET,
     int nrto = Constants.PN_BERECHNET, string auid = null)
  {
    var liste = new List<HhBilanz>();
    var con = GetCon(daten);
    using (var cmd = con.CreateCommand())
    {
      var sb = new StringBuilder();
      sb.Append("SELECT b.Sortierung, b.Name, a.Konto_Uid, a.SH, a.ESH, SUM(a.Betrag), SUM(a.EBetrag)")
        .Append(", MAX(a.Angelegt_Am), MAX(a.Angelegt_Von), MAX(a.Geaendert_Am), MAX(a.Geaendert_Von)")
        .Append(" FROM HH_Bilanz a")
        .Append(" INNER JOIN HH_Konto b on a.Mandant_Nr=b.Mandant_Nr and a.Konto_Uid=b.Uid")
        .Append(" WHERE a.Mandant_Nr=@Mandant_Nr");
      AddParam(cmd, "@Mandant_Nr", daten.MandantNr);
      if (!string.IsNullOrEmpty(auid))
      {
        sb.Append(" AND a.Konto_Uid=@Konto_Uid");
        AddParam(cmd, "@Konto_Uid", auid);
      }
      if (!string.IsNullOrEmpty(type))
      {
        sb.Append(" AND a.Kz=@Kz");
        AddParam(cmd, "@Kz", type);
      }
      if (nrfrom > Constants.PN_BERECHNET)
      {
        if (nrfrom == nrto)
          sb.Append(" AND a.Periode=@Periode1");
        else
          sb.Append(" AND a.Periode>=@Periode1");
        AddParam(cmd, "@Periode1", nrfrom);
      }
      if (nrto > Constants.PN_BERECHNET && nrfrom != nrto)
      {
        sb.Append(" AND a.Periode<=@Periode2");
        AddParam(cmd, "@Periode2", nrto);
      }
      sb.Append(" GROUP BY b.Sortierung, b.Name, a.Konto_Uid, a.SH, a.ESH")
        .Append(" ORDER BY b.Sortierung, b.Name, a.Konto_Uid, a.SH, a.ESH");
      cmd.CommandText = sb.ToString();
      using var rd = cmd.ExecuteReader();
      while (rd.Read())
      {
        var b = new HhBilanz
        {
          AccountSort = rd.GetString(0),
          AccountName = rd.GetString(1),
          Konto_Uid = rd.GetString(2),
          SH = rd.GetString(3),
          ESH = rd.GetString(4),
          AccountSum = rd.GetDecimal(5),
          AccountEsum = rd.GetDecimal(6),
          Angelegt_Am = rd.IsDBNull(7) ? (DateTime?)null : rd.GetDateTime(7),
          Angelegt_Von = rd.IsDBNull(8) ? null : rd.GetString(8),
          Geaendert_Am = rd.IsDBNull(9) ? (DateTime?)null : rd.GetDateTime(9),
          Geaendert_Von = rd.IsDBNull(10) ? null : rd.GetString(10),
        };
        liste.Add(b);
      }
    }
    return liste;
  }

  /// <summary>
  /// Gets list of balances with sums over accounts.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="pnr">Affected period number.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <returns>List of balances.</returns>
  public List<HhBilanz> GetAccountSum(ServiceDaten daten, string type, int pnr, string auid)
  {
    var liste = new List<HhBilanz>();
    var con = GetCon(daten);
    using (var cmd = con.CreateCommand())
    {
      var sb = new StringBuilder();
      sb.Append("SELECT a.Mandant_Nr, SUM(a.Betrag), SUM(a.EBetrag)")
        .Append(" FROM HH_Bilanz a")
        .Append(" WHERE a.Mandant_Nr=@Mandant_Nr");
      AddParam(cmd, "@Mandant_Nr", daten.MandantNr);
      if (!string.IsNullOrEmpty(auid))
      {
        sb.Append(" AND a.Konto_Uid<>@Konto_Uid");
        AddParam(cmd, "@Konto_Uid", auid);
      }
      if (!string.IsNullOrEmpty(type))
      {
        sb.Append(" AND a.Kz=@Kz");
        AddParam(cmd, "@Kz", type);
      }
      if (pnr > Constants.PN_BERECHNET)
      {
        sb.Append(" AND a.Periode=@Periode");
        AddParam(cmd, "@Periode", pnr);
      }
      sb.Append(" GROUP BY a.Mandant_Nr")
        .Append(" ORDER BY a.Mandant_Nr");
      cmd.CommandText = sb.ToString();
      using var rd = cmd.ExecuteReader();
      while (rd.Read())
      {
        var b = new HhBilanz
        {
          Mandant_Nr = rd.GetInt32(0),
          AccountSum = rd.GetDecimal(1),
          AccountEsum = rd.GetDecimal(2),
        };
        liste.Add(b);
      }
    }
    return liste;
  }

#pragma warning restore CA1822
}
