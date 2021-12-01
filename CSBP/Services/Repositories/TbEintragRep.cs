// <copyright file="TbEintragRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Klasse für TB_Eintrag-Repository.
  /// </summary>
  public partial class TbEintragRep
  {
    public List<TbEintrag> GetList(ServiceDaten daten, int mandantnr, DateTime date, int days = 1)
    {
      var db = GetDb(daten);
      var from = date.AddDays(-days);
      var l = db.TB_Eintrag.Where(a => a.Mandant_Nr == mandantnr && a.Datum > from && a.Datum <= date);
      return l.ToList();
    }

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
    /// <returns>Datum des passenden Eintrags.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="stelle">Gewünschte Such-Richtung.</param>
    /// <param name="aktDatum">Aufsetzpunkt der Suche.</param>
    /// <param name="suche">Such-Strings, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.</param>
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected from date.</param>
    public DateTime? SearchDate(ServiceDaten daten, SearchDirectionEnum stelle, DateTime? aktDatum, string[] suche,
      string puid, DateTime? from, DateTime? to)
    {
      if (stelle == SearchDirectionEnum.None)
        return null;
      var l = GetSearch(daten, stelle, aktDatum, suche, puid, from, to);
      if (stelle == SearchDirectionEnum.First || stelle == SearchDirectionEnum.Forward)
        return l.Any() ? l.Min(a => a.Datum) : (DateTime?)null;
      return l.Any() ? l.Max(a => a.Datum) : (DateTime?)null;
    }

    /// <summary>
    /// Suche aller passenden Einträge.
    /// </summary>
    /// <returns>Liste aller passenden Einträge.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="suche">Such-Strings, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.</param>
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected from date.</param>
    public List<TbEintrag> SearchEntries(ServiceDaten daten, string[] suche, string puid, DateTime? from, DateTime? to)
    {
      var l = GetSearch(daten, SearchDirectionEnum.First, null, suche, puid, from, to);
      return l.ToList();
    }

    /// <summary>
    /// Suche des nächsten passenden Eintrags in der Suchrichtung.
    /// </summary>
    /// <returns>Datum des passenden Eintrags.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="stelle">Gewünschte Such-Richtung.</param>
    /// <param name="aktDatum">Aufsetzpunkt der Suche.</param>
    /// <param name="suche">Such-Strings, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.</param>
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected from date.</param>
    IQueryable<TbEintrag> GetSearch(ServiceDaten daten, SearchDirectionEnum stelle, DateTime? aktDatum, string[] suche,
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
      if (!string.IsNullOrEmpty(suche[2]))
        l = l.Where(a => EF.Functions.Like(a.Eintrag, suche[0])
                    || EF.Functions.Like(a.Eintrag, suche[1])
                    || EF.Functions.Like(a.Eintrag, suche[2]));
      else if (!string.IsNullOrEmpty(suche[1]))
        l = l.Where(a => EF.Functions.Like(a.Eintrag, suche[0])
                    || EF.Functions.Like(a.Eintrag, suche[1]));
      else
        l = l.Where(a => EF.Functions.Like(a.Eintrag, suche[0]));
      if (!string.IsNullOrEmpty(suche[5]))
        l = l.Where(a => EF.Functions.Like(a.Eintrag, suche[3])
                    || EF.Functions.Like(a.Eintrag, suche[4])
                           || EF.Functions.Like(a.Eintrag, suche[5]));
      else if (!string.IsNullOrEmpty(suche[4]))
        l = l.Where(a => EF.Functions.Like(a.Eintrag, suche[3])
                           || EF.Functions.Like(a.Eintrag, suche[4]));
      else if (!string.IsNullOrEmpty(suche[3]))
        l = l.Where(a => EF.Functions.Like(a.Eintrag, suche[3]));
      if (!string.IsNullOrEmpty(suche[8]))
        l = l.Where(a => !(EF.Functions.Like(a.Eintrag, suche[6])
                    || EF.Functions.Like(a.Eintrag, suche[7])
                           || EF.Functions.Like(a.Eintrag, suche[8])));
      else if (!string.IsNullOrEmpty(suche[7]))
        l = l.Where(a => !(EF.Functions.Like(a.Eintrag, suche[6])
                           || EF.Functions.Like(a.Eintrag, suche[7])));
      else if (!string.IsNullOrEmpty(suche[6]))
        l = l.Where(a => !EF.Functions.Like(a.Eintrag, suche[6]));
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
  }
}
