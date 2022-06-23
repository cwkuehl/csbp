// <copyright file="TbEintragRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table TB_Eintrag.
/// </summary>
public partial class TbEintragRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list of diary entries before a date.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Affected client number.</param>
  /// <param name="date">Affected to date.</param>
  /// <param name="days">Number of days.</param>
  /// <returns>List of diary entries.</returns>
  public List<TbEintrag> GetList(ServiceDaten daten, int mandantnr, DateTime date, int days = 1)
  {
    var db = GetDb(daten);
    var from = date.AddDays(-days);
    var l = db.TB_Eintrag.Where(a => a.Mandant_Nr == mandantnr && a.Datum > from && a.Datum <= date);
    return l.ToList();
  }

  /// <summary>
  /// Gets list of diary entries for a number of months.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Affected client number.</param>
  /// <param name="date">Date within the affected month.</param>
  /// <param name="months">Number of months.</param>
  /// <returns>List of diary entries.</returns>
  public List<TbEintrag> GetMonthList(ServiceDaten daten, int mandantnr, DateTime date, int months = 1)
  {
    var db = GetDb(daten);
    var b = new DateTime(date.Year, date.Month, 1);
    var e = b.AddMonths(months);
    var l = db.TB_Eintrag.Where(a => a.Mandant_Nr == mandantnr && a.Datum >= b && a.Datum < e);
    return l.ToList();
  }

  /// <summary>
  /// Suche des nächsten passenden Eintrags in der Suchrichtung.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stelle">Affected search direction.</param>
  /// <param name="aktDatum">Starting point of search.</param>
  /// <param name="search">Affected search string, possibly with placeholders, e.g. %B_den% finds Baden and Boden.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>Datum des passenden Eintrags.</returns>
  public DateTime? SearchDate(ServiceDaten daten, SearchDirectionEnum stelle, DateTime? aktDatum, string[] search,
    string puid, DateTime? from, DateTime? to)
  {
    if (stelle == SearchDirectionEnum.None)
      return null;
    var l = GetSearch(daten, stelle, aktDatum, search, puid, from, to);
    if (stelle == SearchDirectionEnum.First || stelle == SearchDirectionEnum.Forward)
      return l.Any() ? l.Min(a => a.Datum) : (DateTime?)null;
    return l.Any() ? l.Max(a => a.Datum) : (DateTime?)null;
  }

  /// <summary>
  /// Gets list or diary entries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="search">Affected search string, possibly with placeholders, e.g. %B_den% finds Baden and Boden.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>List of fitting diary entries.</returns>
  public List<TbEintrag> SearchEntries(ServiceDaten daten, string[] search, string puid, DateTime? from, DateTime? to)
  {
    var l = GetSearch(daten, SearchDirectionEnum.First, null, search, puid, from, to);
    return l.ToList();
  }

  /// <summary>
  /// Searches the next fitting entry in search direction.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stelle">Affected search direction.</param>
  /// <param name="aktDatum">Starting point of search.</param>
  /// <param name="search">Affected search string, possibly with placeholders, e.g. %B_den% finds Baden and Boden.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>Next fitting date.</returns>
  public IQueryable<TbEintrag> GetSearch(ServiceDaten daten, SearchDirectionEnum stelle, DateTime? aktDatum, string[] search,
    string puid, DateTime? from, DateTime? to)
  {
    var db = GetDb(daten);
    var l = db.TB_Eintrag.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    if (aktDatum.HasValue)
    {
      if (stelle == SearchDirectionEnum.Back)
        l = l.Where(a => a.Datum < aktDatum);
      if (stelle == SearchDirectionEnum.Forward)
        l = l.Where(a => a.Datum > aktDatum);
    }
    if (!string.IsNullOrEmpty(search[2]))
      l = l.Where(a => EF.Functions.Like(a.Eintrag, search[0])
                  || EF.Functions.Like(a.Eintrag, search[1])
                  || EF.Functions.Like(a.Eintrag, search[2]));
    else if (!string.IsNullOrEmpty(search[1]))
      l = l.Where(a => EF.Functions.Like(a.Eintrag, search[0])
                  || EF.Functions.Like(a.Eintrag, search[1]));
    else
      l = l.Where(a => EF.Functions.Like(a.Eintrag, search[0]));
    if (!string.IsNullOrEmpty(search[5]))
      l = l.Where(a => EF.Functions.Like(a.Eintrag, search[3])
                  || EF.Functions.Like(a.Eintrag, search[4])
                         || EF.Functions.Like(a.Eintrag, search[5]));
    else if (!string.IsNullOrEmpty(search[4]))
      l = l.Where(a => EF.Functions.Like(a.Eintrag, search[3])
                         || EF.Functions.Like(a.Eintrag, search[4]));
    else if (!string.IsNullOrEmpty(search[3]))
      l = l.Where(a => EF.Functions.Like(a.Eintrag, search[3]));
    if (!string.IsNullOrEmpty(search[8]))
      l = l.Where(a => !(EF.Functions.Like(a.Eintrag, search[6])
                  || EF.Functions.Like(a.Eintrag, search[7])
                         || EF.Functions.Like(a.Eintrag, search[8])));
    else if (!string.IsNullOrEmpty(search[7]))
      l = l.Where(a => !(EF.Functions.Like(a.Eintrag, search[6])
                         || EF.Functions.Like(a.Eintrag, search[7])));
    else if (!string.IsNullOrEmpty(search[6]))
      l = l.Where(a => !EF.Functions.Like(a.Eintrag, search[6]));
    if (!string.IsNullOrEmpty(puid))
    {
      if (puid == "0")
      {
        var eo = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
        l = l.Where(a => !eo.Any(b => b.Datum_Von <= a.Datum && a.Datum <= b.Datum_Bis));
      }
      else
      {
        var eo = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr && a.Ort_Uid == puid);
        l = l.Where(a => eo.Any(b => b.Datum_Von <= a.Datum && a.Datum <= b.Datum_Bis));
      }
    }
    if (from.HasValue)
      l = l.Where(a => a.Datum >= from.Value);
    if (to.HasValue)
      l = l.Where(a => a.Datum <= to.Value);
    return l;
  }

#pragma warning restore CA1822
}
