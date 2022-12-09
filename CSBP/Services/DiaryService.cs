// <copyright file="DiaryService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

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

//// Bodenheim 49.9353, 8.3184

/// <summary>
/// Implementation of diary service.
/// </summary>
public class DiaryService : ServiceBase, IDiaryService
{
  /// <summary>
  /// Gets the diary entry of a date.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="pos">With Positions or not.</param>
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
        tbEintrag.Replikation_Uid ??= Functions.GetUid();
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
        tbEintrag.Replikation_Uid ??= Functions.GetUid();
        tbEintrag.Eintrag = entry;
        TbEintragRep.Update(daten, tbEintrag);
      }
    }
    else
    {
      // Deletes empty entry and keeps positions.
      TbEintragRep.Delete(daten, tbEintrag);
      //// if (pos != null)
      ////   pos.Clear();
    }
    //// Reads existing positions.
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
        liste.Remove(vo); // Do not delete.
      if (!listep.Any() || vop == null)
      {
        // Empty period.
        OptimizePositions(daten, puid, from, to);
      }
      else if (listep.Count == 1)
      {
        if (vop.Datum_Von == from && vop.Datum_Bis == to)
          Functions.MachNichts();
        else if (vop.Datum_Von <= from && vop.Datum_Bis >= to)
        {
          if (from == to)
            Functions.MachNichts(); // Case: by mistake deleted and again added.
          else
          {
            // Shortens period.
            TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, to, vop.Angelegt_Von, vop.Angelegt_Am);
            TbEintragOrtRep.Delete(daten, vop);
          }
        }
        else
        {
          // Do not shorten.
          var mfrom = vop.Datum_Von < from ? vop.Datum_Von : from;
          var mto = vop.Datum_Bis > to ? vop.Datum_Bis : to;
          if (!(vop.Datum_Von == mfrom && vop.Datum_Bis == mto))
          {
            // Maximum period.
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
        //// Maximum period.
        OptimizePositions(daten, puid, mfrom, mto, vop.Angelegt_Von, vop.Angelegt_Am);
      }
    }
    //// Deletes obsolete positions.
    foreach (var vo in liste)
    {
      if (vo.Datum_Von == vo.Datum_Bis)
        TbEintragOrtRep.Delete(daten, vo); // Deletes entry.
      else if (vo.Datum_Von == date)
      {
        // Shortens one day before.
        TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, date.AddDays(1), vo.Datum_Bis, vo.Angelegt_Von, vo.Angelegt_Am);
        TbEintragOrtRep.Delete(daten, vo);
      }
      else if (vo.Datum_Bis == date)
      {
        // Shortens one day after.
        TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, vo.Datum_Von, vo.Datum_Bis.AddDays(-1), vo.Angelegt_Von, vo.Angelegt_Am);
        TbEintragOrtRep.Delete(daten, vo);
      }
      else
      {
        // Cuts out a day.
        TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, vo.Datum_Von, date.AddDays(-1), vo.Angelegt_Von, vo.Angelegt_Am);
        TbEintragOrtRep.Save(daten, daten.MandantNr, vo.Ort_Uid, date.AddDays(1), vo.Datum_Bis, vo.Angelegt_Von, vo.Angelegt_Am);
        TbEintragOrtRep.Delete(daten, vo);
      }
    }
    return r;
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
  public ServiceErgebnis<DateTime?> SearchDate(ServiceDaten daten, SearchDirectionEnum stelle,
    DateTime? aktDatum, string[] search, string puid, DateTime? from, DateTime? to)
  {
    var r = new ServiceErgebnis<DateTime?>();
    if (aktDatum.HasValue)
    {
      CheckSearch(search);
      var datum = TbEintragRep.SearchDate(daten, stelle, aktDatum, search, puid, from, to);
      if (datum.HasValue && stelle == SearchDirectionEnum.Last
        && search[0] == "%" && search[3] == "" && search[6] == "" && string.IsNullOrEmpty(puid))
      {
        datum = datum.Value.AddDays(1);
      }
      r.Ergebnis = datum;
    }
    return r;
  }

  /// <summary>
  /// Create list of lines which contains all diary entries fitting the search string.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="search">Affected search string, possibly with placeholders, e.g. %B_den% finds Baden and Boden.
  /// To check counters use ####, e.g. %####. BGS: %.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>String array which can be stored in a text file.</returns>
  public ServiceErgebnis<List<string>> GetDiaryReport(ServiceDaten daten, string[] search,
    string puid, DateTime? from, DateTime? to)
  {
    CheckSearch(search);
    var v = new List<string>();
    var r = new ServiceErgebnis<List<string>>(v);
    var rf = false; // Sequence test
    var str = search[0];
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
    search[0] = str;

    // Diary report: {0}
    v.Add(TB002(daten.Jetzt));
    //// Search: (/{0}/ oder /{1}/ oder /{2}/) und (/{3}/ oder /{4}/ oder /{5}/) und nicht (/{6}/ oder /{7}/ oder /{8}/)
    v.Add(TB003(search));
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
    var liste = TbEintragRep.SearchEntries(daten, search, puid, from, to);
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
          sb.Append(p.Description);
          if (!plist.ContainsKey(p.Ort_Uid))
          {
            plist.Add(p.Ort_Uid, new TbOrt
            {
              Mandant_Nr = p.Mandant_Nr,
              Uid = p.Ort_Uid,
              Bezeichnung = p.Description,
              Breite = p.Latitude,
              Laenge = p.Longitude,
              Hoehe = p.Height,
              Notiz = p.Memo,
            }
            );
          }
        }
        sb.Append(']');
      }
      //// Without control characters
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
              // Wrong counter at %1$s: %2$s, erwartet: %3$s
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

  /// <summary>
  /// Optimizes the positions, i.e. lengthening or filling gaps.
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
        // New
        TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, to, angelegtvon, angelegtam);
      }
      else
      {
        // Adds periode before.
        TbEintragOrtRep.Save(daten, daten.MandantNr, puid, from, aft.Datum_Bis, aft.Angelegt_Von, aft.Angelegt_Am);
        TbEintragOrtRep.Delete(daten, aft);
      }
    }
    else if (aft == null)
    {
      // Adds periode after.
      TbEintragOrtRep.Save(daten, daten.MandantNr, puid, bef.Datum_Von, to, bef.Angelegt_Von, bef.Angelegt_Am);
      TbEintragOrtRep.Delete(daten, bef);
    }
    else
    {
      // Fills gap.
      TbEintragOrtRep.Save(daten, daten.MandantNr, puid, bef.Datum_Von, aft.Datum_Bis, bef.Angelegt_Von, bef.Angelegt_Am);
      TbEintragOrtRep.Delete(daten, bef);
      TbEintragOrtRep.Delete(daten, aft);
    }
  }

  /// <summary>
  /// Checks and packs the search pattern.
  /// </summary>
  /// <param name="search">Affected search pattern.</param>
  private static void CheckSearch(string[] search)
  {
    const int Columns = 3;
    const int Rows = 3;
    if (search == null || search.Length != Columns * Rows)
      throw new MessageException(TB001);
    for (var i = 0; i < Columns * Rows; i++)
    {
      if (!Functions.IsLike(search[i]))
        search[i] = "";
    }
    //// Packs search pattern
    for (var y = 0; y < Rows; y++)
    {
      var i = 0;
      for (var x = 0; x < Columns - 1; x++)
      {
        if (string.IsNullOrEmpty(search[(y * Columns) + x]))
        {
          if (!string.IsNullOrEmpty(search[(y * Columns) + x + 1]))
          {
            search[(y * Columns) + i] = search[(y * Columns) + x + 1];
            search[(y * Columns) + x + 1] = "";
            i++;
          }
        }
        else
          i++;
      }
    }
    if (string.IsNullOrEmpty(search[0]))
      search[0] = "%";
  }
}
