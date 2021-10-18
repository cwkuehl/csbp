// <copyright file="LoginService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services
{
  using System;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using static CSBP.Resources.Messages;
  using static CSBP.Resources.M;
  using System.Text.RegularExpressions;
  using CSBP.Apis.Enums;
  using System.Collections.Generic;
  using System.Linq;

  // Bodenheim 49.9353, 8.3184

  public class DiaryService : ServiceBase, IDiaryService
  {
    /// <summary>
    /// Gets the diary entry of a date.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="pos">With Positions?</param>
    /// <returns>The entry.</returns>
    public ServiceErgebnis<TbEintrag> GetEntry(ServiceDaten daten, DateTime date, bool pos = false)
    {
      var e = TbEintragRep.Get(daten, daten.MandantNr, date);
      if (pos)
      {
        var pliste = TbOrtRep.GetPositionList(daten, date);
        if (e == null)
        {
          if (pliste.Any())
            e = new TbEintrag
            {
              Mandant_Nr = daten.MandantNr,
              Datum = date,
              Positions = pliste,
            };
        }
        else
          e.Positions = pliste;
      }
      return new ServiceErgebnis<TbEintrag>(e);
    }

    /// <summary>
    /// Gets Bool-Array of a month if there is an entry.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected month.</param>
    /// <returns>Bool-Array of a month.</returns>
    public ServiceErgebnis<bool[]> GetMonth(ServiceDaten daten, DateTime date)
    {
      var month = new bool[31];
      var list = TbEintragRep.GetMonthList(daten, daten.MandantNr, date);
      foreach (var e in list)
      {
        month[e.Datum.Day - 1] = true;
      }
      return new ServiceErgebnis<bool[]>(month);
    }

    /// <summary>
    /// Saves the diary entry.
    /// </summary>
    /// <returns>Possible errors.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="entry">Affected Entry.</param>
    /// <param name="pos">Affected list of positions.</param>
    public ServiceErgebnis SaveEntry(ServiceDaten daten, DateTime date, string entry, List<Tuple<string, DateTime, DateTime>> pos)
    {
      var r = new ServiceErgebnis();
      if (!string.IsNullOrEmpty(entry))
      {
        entry = entry.Trim();
      }
      var leer = string.IsNullOrEmpty(entry);
      var tbEintrag = TbEintragRep.Get(daten, daten.MandantNr, date);
      if (tbEintrag == null)
      {
        if (!leer)
        {
          tbEintrag = new TbEintrag();
          if (tbEintrag.Replikation_Uid == null)
          {
            tbEintrag.Replikation_Uid = Functions.GetUid();
          }
          tbEintrag.Mandant_Nr = daten.MandantNr;
          tbEintrag.Datum = date;
          tbEintrag.Eintrag = entry;
          TbEintragRep.Insert(daten, tbEintrag);
        }
      }
      else if (!leer)
      {
        if (Functions.CompString(entry, tbEintrag.Eintrag) != 0)
        {
          if (tbEintrag.Replikation_Uid == null)
          {
            tbEintrag.Replikation_Uid = Functions.GetUid();
          }
          tbEintrag.Eintrag = entry;
          TbEintragRep.Update(daten, tbEintrag);
        }
      }
      else
      {
        // leeren Eintrag löschen und Positionen lassen
        TbEintragRep.Delete(daten, tbEintrag);
        // if (pos != null)
        //   pos.Clear();
      }
      // bestehende Orte lesen
      var liste = TbEintragOrtRep.GetList(daten, date);
      foreach (var i in pos ?? new List<Tuple<string, DateTime, DateTime>>())
      {
        var puid = i.Item1;
        var from = i.Item2;
        var to = i.Item3;
        if (to < from)
          to = from;
        if (date < from || date > to)
        {
          from = date;
          to = date;
        }
        var listep = TbEintragOrtRep.GetList(daten, from, to, puid);
        var vop = listep.FirstOrDefault();
        var vo = liste.FirstOrDefault(a => a.Ort_Uid == puid);
        if (vo != null)
          liste.Remove(vo); // nicht mehr löschen
        if (!listep.Any() || vop == null)
        {
          // Zeitraum leer
          OptimizePositions(daten, puid, from, to);
        }
        else if (listep.Count == 1)
        {
          var mfrom = vop.Datum_Von < from ? vop.Datum_Von : from;
          var mto = vop.Datum_Bis > to ? vop.Datum_Bis : to;
          if (!(vop.Datum_Von == mfrom && vop.Datum_Bis == mto))
          {
            // Maximaler Zeitraum
            OptimizePositions(daten, puid, mfrom, mto, vop.Angelegt_Von, vop.Angelegt_Am);
            TbEintragOrtRep.Delete(daten, vop);
          }
        }
        else
        {
          // listep.Count >= 1
          var mfrom = from;
          var mto = to;
          foreach (var p in listep)
          {
            if (p.Datum_Von < mfrom)
              mfrom = p.Datum_Von;
            if (p.Datum_Bis > mto)
              mto = p.Datum_Bis;
            TbEintragOrtRep.Delete(daten, p);
          }
          // Maximaler Zeitraum
          OptimizePositions(daten, puid, mfrom, mto, vop.Angelegt_Von, vop.Angelegt_Am);
        }
      }
      // überflüssige Orte löschen.
      foreach (var vo in liste)
      {
        if (vo.Datum_Von == vo.Datum_Bis)
          TbEintragOrtRep.Delete(daten, vo); // Eintrag löschen
        else if (vo.Datum_Von == date)
        {
          // Einen Tag vorne verkürzen
          TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, date.AddDays(1), vo.Datum_Bis, vo.Angelegt_Von, vo.Angelegt_Am);
          TbEintragOrtRep.Delete(daten, vo);
        }
        else if (vo.Datum_Bis == date)
        {
          // Einen Tag hinten verkürzen
          TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, vo.Datum_Von, vo.Datum_Bis.AddDays(-1), vo.Angelegt_Von, vo.Angelegt_Am);
          TbEintragOrtRep.Delete(daten, vo);
        }
        else
        {
          // Einen Tag herausschneiden
          TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, vo.Datum_Von, date.AddDays(-1), vo.Angelegt_Von, vo.Angelegt_Am);
          TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, date.AddDays(1), vo.Datum_Bis, vo.Angelegt_Von, vo.Angelegt_Am);
          TbEintragOrtRep.Delete(daten, vo);
        }
      }
      return r;
    }

    /// <summary>
    /// Optimieren der Positionen, d.h. verlängern oder Lücke füllen.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="puid">Affected position id.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected to date.</param>
    /// <param name="angelegtvon">Affected creation user id.</param>
    /// <param name="angelegtam">Affection creation time.</param>
    private void OptimizePositions(ServiceDaten daten, string puid, DateTime from, DateTime to, string angelegtvon = null, DateTime? angelegtam = null)
    {
      var listeb = TbEintragOrtRep.GetList(daten, from.AddDays(-1), puid);
      var listea = TbEintragOrtRep.GetList(daten, to.AddDays(1), puid);
      var bef = listeb.FirstOrDefault();
      var aft = listea.FirstOrDefault();

      if (bef == null)
      {
        if (aft == null)
          TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, to, angelegtvon, angelegtam); // neu
        else
        {
          // Zeitraum vorne anhängen
          TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, aft.Datum_Bis, aft.Angelegt_Von, aft.Angelegt_Am);
          TbEintragOrtRep.Delete(daten, aft);
        }
      }
      else if (aft == null)
      {
        // Zeitraum hinten anhängen
        TbEintragOrtRep.Save(daten, daten.MandantNr, puid, bef.Datum_Von, to, bef.Angelegt_Von, bef.Angelegt_Am);
        TbEintragOrtRep.Delete(daten, bef);
      }
      else
      {
        // Lücke füllen
        TbEintragOrtRep.Save(daten, daten.MandantNr, puid, bef.Datum_Von, aft.Datum_Bis, bef.Angelegt_Von, bef.Angelegt_Am);
        TbEintragOrtRep.Delete(daten, bef);
        TbEintragOrtRep.Delete(daten, aft);
      }
    }

    /// <summary>
    /// Suche des nächsten passenden Eintrags in der Suchrichtung.
    /// </summary>
    /// <returns>Datum des passenden Eintrags.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="stelle">Gewünschte Such-Richtung.</param>
    /// <param name="aktDatum">Aufsetzpunkt der Suche.</param>
    /// <param name="suche">Such-Strings, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.</param>
    public ServiceErgebnis<DateTime?> SearchDate(ServiceDaten daten, SearchDirectionEnum stelle,
      DateTime? aktDatum, string[] suche)
    {
      var r = new ServiceErgebnis<DateTime?>();
      if (aktDatum.HasValue)
      {
        CheckSearch(suche);
        var datum = TbEintragRep.SearchDate(daten, stelle, aktDatum, suche);
        if (datum.HasValue && stelle == SearchDirectionEnum.Last && suche[0] == "%")
        {
          datum = datum.Value.AddDays(1);
        }
        r.Ergebnis = datum;
      }
      return r;
    }

    /// <summary>
    /// Erzeugung einer Datei, die alle Tagebuch-Einträge enthält, die dem Such-String entsprechen.
    /// </summary>
    /// <returns>String-Array, das in einer Datei gespeichert werden kann.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="suche">Such-String, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.
    /// Bei der Suche kann auch ein Zähler geprüft werden, z.B. %####. BGS: %</param>
    public ServiceErgebnis<List<string>> GetFile(ServiceDaten daten, string[] suche)
    {
      CheckSearch(suche);
      var v = new List<string>();
      var r = new ServiceErgebnis<List<string>>(v);
      var rf = false; // Reihenfolge-Test
      var str = suche[0];
      var muster = "";

      if (string.IsNullOrEmpty(str))
      {
        str = "";
      }
      else
      {
        if (str.Contains("####"))
        {
          muster = str.Replace("####", "\\D*(\\d+)");
          if (muster.StartsWith("%", StringComparison.Ordinal))
            muster = muster.Substring(1);
          if (muster.EndsWith("%", StringComparison.Ordinal))
            muster = muster.Substring(0, muster.Length - 1);
          str = str.Replace("####", "");
          rf = true;
        }
      }
      suche[0] = str;

      // Bericht vom: %1$s
      v.Add(TB002(daten.Jetzt));
      // Suche nach: ('%1$s' oder '%2$s' oder '%3$s') und ('%4$s' oder '%5$s'
      // oder '%6$s') und nicht ('%7$s' oder '%8$s' oder '%9$s')%10$s
      v.Add(TB003(suche));
      v.Add(string.Empty);
      var liste = TbEintragRep.SearchEntries(daten, suche);
      foreach (var e in liste)
      {
        v.Add(TB006(e.Datum, e.Eintrag));
      }
      if (rf)
      {
        var z = -1L;
        var p = new Regex(muster, RegexOptions.Compiled);
        foreach (var e in liste)
        {
          str = e.Eintrag;
          if (!string.IsNullOrEmpty(str))
          {
            var mlist = p.Matches(str);
            foreach (Match m in mlist)
            {
              var l = long.Parse(m.Groups[1].Value);
              if (z < 0)
              {
                z = l;
              }
              else
              {
                if (z != l)
                {
                  // Falscher Zähler am %1$s: %2$s, erwartet: %3$s
                  throw new MessageException(TB004(e.Datum, m.Groups[1].Value, z));
                }
              }
              z++;
            }
          }
        }
      }
      return r;
    }

    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected position uid.</param>
    /// <returns>The position.</returns>
    public ServiceErgebnis<TbOrt> GetPosition(ServiceDaten daten, string uid)
    {
      var e = TbOrtRep.Get(daten, daten.MandantNr, uid);
      return new ServiceErgebnis<TbOrt>(e);
    }

    /// <summary>
    /// Gets a list of positions.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="puid">Affected position ID.</param>
    /// <param name="text">Affected text.</param>
    /// <returns>List of positions.</returns>
    public ServiceErgebnis<List<TbOrt>> GetPositionList(ServiceDaten daten, string puid, string text)
    {
      var l = TbOrtRep.GetList(daten, puid, text: text, desc: true);
      return new ServiceErgebnis<List<TbOrt>>(l);
    }

    /// <summary>
    /// Saves the position.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected position uid.</param>
    /// <param name="desc">Affected description.</param>
    /// <param name="lat">Affected latitude.</param>
    /// <param name="lon">Affected longitude.</param>
    /// <param name="alt">Affected altitude.</param>
    /// <param name="memo">Affected memos.</param>
    /// <returns>Saved entity.</returns>
    public ServiceErgebnis<TbOrt> SavePosition(ServiceDaten daten, string uid, string desc, string lat, string lon, string alt, string memo)
    {
      var r = new ServiceErgebnis<TbOrt>();
      if (string.IsNullOrEmpty(desc))
        r.Errors.Add(Message.New(TB007));
      var b = Functions.ToDecimal(lat) ?? 0;
      if (b < -90 || b > 90)
        r.Errors.Add(Message.New(TB008));
      var l = Functions.ToDecimal(lon) ?? 0;
      if (l < -180 || l > 180)
        r.Errors.Add(Message.New(TB009));
      var h = Functions.ToDecimal(alt) ?? 0;
      if (!r.Ok)
        return r;
      r.Ergebnis = TbOrtRep.Save(daten, daten.MandantNr, uid, desc, b, l, h, memo, null, null, null, null);
      return r;
    }

    void CheckSearch(string[] suche)
    {
      if (suche == null || suche.Length != 9)
        throw new MessageException(TB001);
      var str = suche[0];
      if (str == null || str == "%%")
        str = "%";
      suche[0] = str;
      for (var i = 1; i < 9; i++)
      {
        str = suche[i];
        if (!Functions.IsLike(str))
          str = "";
        suche[i] = str;
      }
      if (suche[3] == "" && suche[4] != "")
        suche[3] = suche[4];
      if (suche[3] == "" && suche[5] != "")
        suche[3] = suche[5];
      if (suche[6] == "" && suche[7] != "")
        suche[6] = suche[7];
      if (suche[6] == "" && suche[8] != "")
        suche[6] = suche[8];
    }
  }
}
