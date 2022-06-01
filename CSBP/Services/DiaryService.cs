// <copyright file="LoginService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

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
        entry = Functions.FilterWindows1252(entry).Trim();
      var leer = string.IsNullOrEmpty(entry);
      var tbEintrag = TbEintragRep.Get(daten, daten.MandantNr, date);
      if (tbEintrag == null)
      {
        if (!leer)
        {
          tbEintrag = new TbEintrag();
          if (tbEintrag.Replikation_Uid == null)
            tbEintrag.Replikation_Uid = Functions.GetUid();
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
            tbEintrag.Replikation_Uid = Functions.GetUid();
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
          if (vop.Datum_Von == from && vop.Datum_Bis == to)
            Functions.MachNichts();
          else if (vop.Datum_Von <= from && vop.Datum_Bis >= to)
          {
            if (from == to)
              Functions.MachNichts(); // Fall: Aus Versehen gelöscht und wieder hinzugefügt.
            else
            {
              // Zeitraum wird verkürzt.
              TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, to, vop.Angelegt_Von, vop.Angelegt_Am);
              TbEintragOrtRep.Delete(daten, vop);
            }
          }
          else
          {
            // Nicht verkürzen.
            var mfrom = vop.Datum_Von < from ? vop.Datum_Von : from;
            var mto = vop.Datum_Bis > to ? vop.Datum_Bis : to;
            if (!(vop.Datum_Von == mfrom && vop.Datum_Bis == mto))
            {
              // Maximaler Zeitraum
              OptimizePositions(daten, puid, mfrom, mto, vop.Angelegt_Von, vop.Angelegt_Am);
              TbEintragOrtRep.Delete(daten, vop);
            }
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
    private static void OptimizePositions(ServiceDaten daten, string puid, DateTime from, DateTime to, string angelegtvon = null, DateTime? angelegtam = null)
    {
      var listeb = TbEintragOrtRep.GetList(daten, from.AddDays(-1), puid);
      var listea = TbEintragOrtRep.GetList(daten, to.AddDays(1), puid);
      var bef = listeb.FirstOrDefault();
      var aft = listea.FirstOrDefault();

      if (bef == null)
      {
        if (aft == null)
        {
          // Neu
          TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, to, angelegtvon, angelegtam);
        }
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
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected to date.</param>
    public ServiceErgebnis<DateTime?> SearchDate(ServiceDaten daten, SearchDirectionEnum stelle,
      DateTime? aktDatum, string[] suche, string puid, DateTime? from, DateTime? to)
    {
      var r = new ServiceErgebnis<DateTime?>();
      if (aktDatum.HasValue)
      {
        CheckSearch(suche);
        var datum = TbEintragRep.SearchDate(daten, stelle, aktDatum, suche, puid, from, to);
        if (datum.HasValue && stelle == SearchDirectionEnum.Last
          && (suche[0] == "%" && suche[3] == "" && suche[6] == "" && string.IsNullOrEmpty(puid)))
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
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected from date.</param>
    public ServiceErgebnis<List<string>> GetDiaryReport(ServiceDaten daten, string[] suche,
      string puid, DateTime? from, DateTime? to)
    {
      CheckSearch(suche);
      var v = new List<string>();
      var r = new ServiceErgebnis<List<string>>(v);
      var rf = false; // Reihenfolge-Test
      var str = suche[0];
      var muster = "";

      if (string.IsNullOrEmpty(str))
        str = "";
      else if (str.Contains("####"))
      {
        muster = Regex.Escape(str).Replace("\\#\\#\\#\\#", "\\D*(\\d+)");
        if (muster.StartsWith("%", StringComparison.Ordinal))
          muster = muster[1..];
        if (muster.EndsWith("%", StringComparison.Ordinal))
          muster = muster[..^1];
        str = str.Replace("####", "");
        rf = true;
      }
      suche[0] = str;

      // Bericht vom: {0}
      v.Add(TB002(daten.Jetzt));
      // Suche nach: (/{0}/ oder /{1}/ oder /{2}/) und (/{3}/ oder /{4}/ oder /{5}/) und nicht (/{6}/ oder /{7}/ oder /{8}/)
      v.Add(TB003(suche));
      if (!string.IsNullOrEmpty(puid))
      {
        var pos = TbOrtRep.Get(daten, daten.MandantNr, puid);
        if (pos != null)
        {
          // Position: {0}
          v.Add(TB010(pos.Bezeichnung));
        }
      }
      if (from.HasValue || to.HasValue)
      {
        // Zeitraum: {0} - {1}
        v.Add(TB011(from, to));
      }
      v.Add(string.Empty);
      var liste = TbEintragRep.SearchEntries(daten, suche, puid, from, to);
      var plist = new Dictionary<string, TbOrt>();
      var sb = new StringBuilder();
      foreach (var e in liste)
      {
        sb.Length = 0;
        var pl = TbOrtRep.GetPositionList(daten, e.Datum);
        if (pl.Any())
        {
          sb.Append(" [");
          foreach (var p in pl)
          {
            if (sb.Length > 2)
              sb.Append("; ");
            sb.Append(p.Bezeichnung);
            if (!plist.ContainsKey(p.Ort_Uid))
            {
              plist.Add(p.Ort_Uid, new TbOrt
              {
                Mandant_Nr = p.Mandant_Nr,
                Uid = p.Ort_Uid,
                Bezeichnung = p.Bezeichnung,
                Breite = p.Breite,
                Laenge = p.Laenge,
                Hoehe = p.Hoehe,
                Notiz = p.Notiz
              }
              );
            }
          }
          sb.Append(']');
        }
        // Without control characters
        v.Add(TB006(e.Datum, sb.ToString(), Functions.FilterWindows1252(e.Eintrag, replace: " /// ")));
      }
      if (plist.Any())
      {
        v.Add("");
        foreach (var p in plist.Values)
        {
          sb.Length = 0;
          sb.Append('[');
          _ = sb.Append(p.Bezeichnung).Append(": ").Append(Functions.ToString(p.Breite, 5)).Append(' ').Append(Functions.ToString(p.Laenge, 5));
          if (p.Hoehe != 0)
            sb.Append(' ').Append(Functions.ToString(p.Hoehe, 2));
          if (!string.IsNullOrEmpty(p.Notiz))
            sb.Append(" (").Append(p.Notiz).Append(')');
          sb.Append(']');
          v.Add(sb.ToString());
        }
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
            foreach (var m in mlist.Cast<Match>())
            {
              var l = long.Parse(m.Groups[1].Value);
              if (z < 0)
              {
                z = l;
              }
              else if (z != l)
              {
                // Falscher Zähler am %1$s: %2$s, erwartet: %3$s
                throw new MessageException(TB004(e.Datum, m.Groups[1].Value, z));
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
      desc = Functions.TrimNull(desc);
      memo = Functions.TrimNull(memo);
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

    private static void CheckSearch(string[] suche)
    {
      const int Columns = 3;
      const int Rows = 3;
      if (suche == null || suche.Length != Columns * Rows)
        throw new MessageException(TB001);
      for (var i = 0; i < Columns * Rows; i++)
      {
        if (!Functions.IsLike(suche[i]))
          suche[i] = "";
      }
      // Pack search pattern
      for (var y = 0; y < Rows; y++)
      {
        var i = 0;
        for (var x = 0; x < Columns - 1; x++)
        {
          if (string.IsNullOrEmpty(suche[y * Columns + x]))
          {
            if (!string.IsNullOrEmpty(suche[y * Columns + x + 1]))
            {
              suche[y * Columns + i] = suche[y * Columns + x + 1];
              suche[y * Columns + x + 1] = "";
              i++;
            }
          }
          else
            i++;
        }
      }
      if (string.IsNullOrEmpty(suche[0]))
        suche[0] = "%";
    }

    /// <summary>
    /// Deletes a position.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeletePosition(ServiceDaten daten, TbOrt e)
    {
      var r = new ServiceErgebnis();
      var plist = TbEintragOrtRep.GetList(daten, null, e.Uid);
      var p = plist.FirstOrDefault();
      if (p != null)
        throw new MessageException(TB013(p.Datum_Von));
      TbOrtRep.Delete(daten, e);
      return r;
    }
  }
}
