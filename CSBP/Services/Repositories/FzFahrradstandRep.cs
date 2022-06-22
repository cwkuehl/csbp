// <copyright file="FzFahrradstandRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Klasse für FZ_Fahrradstand-Repository.
/// </summary>
public partial class FzFahrradstandRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of mileages.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected bike ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="no">Affected number.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="datege">Affected date greater or equal.</param>
  /// <param name="datele">Affected date lower or equal.</param>
  /// <param name="desc">Sorting order descending or not.</param>
  /// <param name="max">Maximal amount of records.</param>
  /// <returns>List of mileages.</returns>
  public List<FzFahrradstand> GetList(ServiceDaten daten, string buid, DateTime? date = null,
    int no = -1, string text = null, DateTime? datege = null, DateTime? datele = null,
    bool desc = false, int max = 0)
  {
    var db = GetDb(daten);
    var wl = db.FZ_Fahrradstand.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr);
    if (Functions.IsLike(text))
      wl = wl.Where(a => EF.Functions.Like(a.Beschreibung, text));
    if (!string.IsNullOrEmpty(buid))
      wl = wl.Where(a => a.Fahrrad_Uid == buid);
    if (date.HasValue)
      wl = wl.Where(a => a.Datum >= date.Value && a.Datum < date.Value.AddSeconds(1));
    if (datege.HasValue)
      wl = wl.Where(a => a.Datum >= datege.Value);
    if (datele.HasValue)
      wl = wl.Where(a => a.Datum <= datele.Value);
    if (no >= 0)
      wl = wl.Where(a => a.Nr == no);
    var l = wl.Join(db.FZ_Fahrrad.Where(a => a.Mandant_Nr == daten.MandantNr),
            a => a.Fahrrad_Uid, b => b.Uid, (a, b) => new { mileage = a, bike = b });
    var l2 = desc ? l.OrderByDescending(a => a.mileage.Datum).ThenByDescending(a => a.mileage.Nr)
        : l.OrderBy(a => a.mileage.Datum).ThenBy(a => a.mileage.Nr);
    var l3 = l2.Take(max <= 0 ? int.MaxValue : max).ToList()
        .Select(a =>
        {
          a.mileage.BikeDescription = a.bike.Bezeichnung;
          return a.mileage;
        });
    return l3.ToList();
  }

  /// <summary>
  /// Gets number of kilometers.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected bike ID.</param>
  /// <param name="datege">Affected min. date.</param>
  /// <param name="datele">Affected max. date.</param>
  /// <returns>Number of kilometers.</returns>
  public decimal Count(ServiceDaten daten, string buid = null, DateTime? datege = null, DateTime? datele = null)
  {
    var db = GetDb(daten);
    var l = db.FZ_Fahrradstand.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(buid))
      l = l.Where(a => a.Fahrrad_Uid == buid);
    if (datege.HasValue)
      l = l.Where(a => a.Datum >= datege.Value);
    if (datele.HasValue)
      l = l.Where(a => a.Datum <= datele.Value);
    var c = l.ToList().Sum(a => a.Periode_km);
    return c;
  }

#pragma warning restore CA1822
}
