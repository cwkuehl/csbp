// <copyright file="TbEintragOrtRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table TB_Eintrag_Ort.
/// </summary>
public partial class TbEintragOrtRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of positions for a date.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <returns>List of positions.</returns>
  public List<TbEintragOrt> GetList(ServiceDaten daten, DateTime? date, string puid = null)
  {
    var db = GetDb(daten);
    var l = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    if (date.HasValue)
      l = l.Where(a => a.Datum_Von <= date.Value && a.Datum_Bis >= date.Value);
    if (!string.IsNullOrEmpty(puid))
      l = l.Where(a => a.Ort_Uid == puid);
    return l.OrderBy(a => a.Ort_Uid).ThenBy(a => a.Datum_Von).ToList();
  }

  /// <summary>
  /// Gets a list of positions for a period.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <returns>List of positions.</returns>
  public List<TbEintragOrt> GetList(ServiceDaten daten, DateTime from, DateTime to, string puid = null)
  {
    var db = GetDb(daten);
    var l = db.TB_Eintrag_Ort.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr
      && a.Datum_Von <= to && a.Datum_Bis >= from);
    if (!string.IsNullOrEmpty(puid))
      l = l.Where(a => a.Ort_Uid == puid);
    return l.OrderBy(a => a.Ort_Uid).ThenBy(a => a.Datum_Von).ToList();
  }

#pragma warning restore CA1822
}
