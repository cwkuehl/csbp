// <copyright file="HhBuchungRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table HH_Buchung.
/// </summary>
public partial class HhBuchungRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list of bookings.
  /// </summary>
  /// <returns>List of bookings.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <param name="debit">Affected account for debit. null: debit and credit, true: debit, false: credit.</param>
  /// <param name="attr">Affected attribute.</param>
  /// <param name="valuta">Search for value date.</param>
  /// <param name="from">Affected minimum date.</param>
  /// <param name="to">Affected maximum date.</param>
  /// <param name="text">Affected posting text.</param>
  /// <param name="value">Affected value.</param>
  /// <param name="desc">Is the order descending or not.</param>
  /// <param name="euro">Compares euro value or not.</param>
  /// <param name="max">How many rows? 0 means all.</param>
  /// <param name="tracking">With tracking or AsNoTracking.</param>
  public List<HhBuchung> GetList(ServiceDaten daten, string auid, bool? debit, string attr = Constants.KZB_AKTIV,
    bool valuta = true, DateTime? from = null, DateTime? to = null, string text = null, string value = null,
    bool desc = true, bool euro = true, int max = 0, bool tracking = false)
  {
    var db = GetDb(daten);
    var l = db.HH_Buchung.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!tracking)
      l = l.AsNoTracking();
    if (!string.IsNullOrEmpty(auid))
    {
      if (debit.HasValue)
      {
        if (debit.Value)
          l = l.Where(a => a.Soll_Konto_Uid == auid);
        else
          l = l.Where(a => a.Haben_Konto_Uid == auid);
      }
      else
        l = l.Where(a => a.Soll_Konto_Uid == auid || a.Haben_Konto_Uid == auid);
    }
    if (!string.IsNullOrEmpty(attr))
      l = l.Where(a => a.Kz == attr);
    if (from.HasValue)
    {
      if (valuta)
        l = l.Where(a => a.Soll_Valuta >= from);
      else
        l = l.Where(a => a.Angelegt_Am >= from || a.Geaendert_Am >= from);
    }
    if (to.HasValue)
    {
      if (valuta)
        l = l.Where(a => a.Soll_Valuta <= to);
      else
        l = l.Where(a => a.Angelegt_Am <= to || a.Geaendert_Am <= to);
    }
    if (Functions.IsLike(text))
      l = l.Where(a => EF.Functions.Like(a.BText, text));
    if (!string.IsNullOrEmpty(value))
    {
      var v = Functions.ToDecimal(value);
      if (euro)
        l = l.Where(a => a.EBetrag == v);
      else
        l = l.Where(a => a.Betrag == v);
    }
    var sl = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
    var hl = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
    var l2 = l.Join(sl, a => a.Soll_Konto_Uid, b => b.Uid, (a, b) => new { bo = a, debit = b });
    var l3 = l2.Join(hl, a => a.bo.Haben_Konto_Uid, b => b.Uid, (a, b) => new { a.bo, a.debit, credit = b });
    var l4 = desc ? l3.OrderBy(a => a.bo.Mandant_Nr).ThenByDescending(a => a.bo.Soll_Valuta)
      .ThenByDescending(a => a.bo.Beleg_Nr).ThenByDescending(a => a.bo.Uid).ToList()
      : l3.OrderBy(a => a.bo.Mandant_Nr).ThenBy(a => a.bo.Soll_Valuta)
      .ThenBy(a => a.bo.Beleg_Nr).ThenBy(a => a.bo.Uid).ToList();
    var l5 = l4
      .Select(a =>
      {
        var e = a.bo;
        e.DebitName = a.debit.Name;
        e.DebitFrom = a.debit.Gueltig_Von;
        e.DebitTo = a.debit.Gueltig_Bis;
        e.DebitType = a.debit.Art;
        e.CreditName = a.credit.Name;
        e.CreditFrom = a.credit.Gueltig_Von;
        e.CreditTo = a.credit.Gueltig_Bis;
        e.CreditType = a.credit.Art;
        return e;
      });
    if (max <= 0)
      return l5.ToList();
    else
      return l5.Take(max).ToList();
  }

  /// <summary>
  /// Gets list of bookings with sums over accounts.
  /// </summary>
  /// <returns>List of bookings.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <param name="from">Affected start date.</param>
  /// <param name="to">Affected end date.</param>
  /// <param name="attr">Affected attribute.</param>
  /// <param name="debit">Debit or credit account.</param>
  public List<HhBuchung> GetSumList(ServiceDaten daten, string auid = null, DateTime? from = null,
     DateTime? to = null, string attr = null, bool debit = false)
  {
    var liste = new List<HhBuchung>();
    var con = GetCon(daten);
    using (var cmd = con.CreateCommand())
    {
      var sb = new StringBuilder();
      var uid = debit ? "Soll_Konto_Uid" : "Haben_Konto_Uid";
      var sign = debit ? "-" : "";
      sb.Append("SELECT a.Mandant_Nr, a.").Append(uid).Append(", b.Art, ").Append(sign).Append("SUM(a.Betrag), ").Append(sign).Append("SUM(a.EBetrag)")
        .Append(" FROM HH_Buchung a")
        .Append(" INNER JOIN HH_Konto b on a.Mandant_Nr=b.Mandant_Nr and a.").Append(uid).Append("=b.Uid")
        .Append(" WHERE a.Mandant_Nr=@Mandant_Nr");
      AddParam(cmd, "@Mandant_Nr", daten.MandantNr);
      if (!string.IsNullOrEmpty(auid))
      {
        sb.Append(" AND a.").Append(uid).Append("=@Konto_Uid");
        AddParam(cmd, "@Konto_Uid", auid);
      }
      if (!string.IsNullOrEmpty(attr))
      {
        sb.Append(" AND a.Kz=@Kz");
        AddParam(cmd, "@Kz", attr);
      }
      if (from.HasValue)
      {
        sb.Append(" AND a.Soll_Valuta>=@From");
        AddParam(cmd, "@From", from);
      }
      if (to.HasValue)
      {
        sb.Append(" AND a.Soll_Valuta<=@To");
        AddParam(cmd, "@To", to);
      }
      sb.Append(" GROUP BY a.Mandant_Nr, a.").Append(uid).Append(", b.Art")
        .Append(" ORDER BY a.Mandant_Nr, a.").Append(uid).Append(", b.Art");
      cmd.CommandText = sb.ToString();
      using var rd = cmd.ExecuteReader();
      while (rd.Read())
      {
        var b = new HhBuchung
        {
          Mandant_Nr = rd.GetInt32(0),
          Soll_Konto_Uid = rd.GetString(1),
          DebitType = rd.GetString(2),
          Betrag = rd.GetDecimal(3),
          EBetrag = rd.GetDecimal(4),
        };
        liste.Add(b);
      }
    }
    return liste;
  }

  /// <summary>
  /// Gets a booking with the last receipt number.
  /// </summary>
  /// <returns>Booking or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected receipt date.</param>
  public HhBuchung GetLastReceipt(ServiceDaten daten, DateTime? date)
  {
    var db = GetDb(daten);
    var l = db.HH_Buchung.Where(a => a.Mandant_Nr == daten.MandantNr && !string.IsNullOrEmpty(a.Beleg_Nr));
    if (date.HasValue)
      l = l.Where(a => a.Beleg_Datum == date);
    return l.OrderByDescending(a => a.Beleg_Datum).ThenByDescending(a => a.Uid).FirstOrDefault();
  }

#pragma warning restore CA1822
}
