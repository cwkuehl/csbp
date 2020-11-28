// <copyright file="HhPeriodeRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Klasse für HH_Periode-Repository.
  /// </summary>
  public partial class HhPeriodeRep
  {
    /// <summary>
    /// Gets list of periods.
    /// </summary>
    /// <returns>List of periods.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="from">Affected minimum date.</param>
    /// <param name="to">Affected maximum date.</param>
    public List<HhPeriode> GetList(ServiceDaten daten, DateTime? from = null, DateTime? to = null)
    {
      var db = GetDb(daten);
      var l = db.HH_Periode.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr && a.Nr != Constants.PN_BERECHNET);
      if (from.HasValue)
        l = l.Where(a => a.Datum_Bis >= from);
      if (to.HasValue)
        l = l.Where(a => a.Datum_Von <= to);
      return l.OrderBy(a => a.Mandant_Nr).ThenByDescending(a => a.Nr).ToList();
    }

    /// <summary>
    /// Gets the extreme period.
    /// </summary>
    /// <returns>Period or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="min">Minimum or maximum.</param>
    /// <param name="d">Affected minimum or maximum date.</param>
    public HhPeriode GetMaxMin(ServiceDaten daten, bool min = false, DateTime? d = null)
    {
      var db = GetDb(daten);
      var l = db.HH_Periode.AsNoTracking().Where(a => a.Mandant_Nr == daten.MandantNr && a.Nr != Constants.PN_BERECHNET);
      if (d.HasValue)
      {
        if (min)
          l = l.Where(a => a.Datum_Bis >= d);
        else
          l = l.Where(a => a.Datum_Von <= d);
      }
      if (min)
        return l.OrderBy(a => a.Nr).FirstOrDefault();
      return l.OrderByDescending(a => a.Nr).FirstOrDefault();
    }
  }
}
