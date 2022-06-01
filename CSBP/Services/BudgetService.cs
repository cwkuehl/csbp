// <copyright file="BudgetService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using CSBP.Services.Budget;
  using CSBP.Services.Reports;
  using static CSBP.Base.Functions;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  public class BudgetService : ServiceBase, IBudgetService
  {
    /// <summary>
    /// Gets list of periods.
    /// </summary>
    /// <returns>List of periods.</returns>
    /// <param name="daten">Service data for database access.</param>
    public ServiceErgebnis<List<HhPeriode>> GetPeriodList(ServiceDaten daten)
    {
      var v = HhPeriodeRep.GetList(daten);
      var r = new ServiceErgebnis<List<HhPeriode>>(v);
      return r;
    }

    /// <summary>
    /// Gets a period with first beginning and last end.
    /// </summary>
    /// <returns>Period with first beginning and last end.</returns>
    /// <param name="daten">Service data for database access.</param>
    public ServiceErgebnis<HhPeriode> GetMinMaxPeriod(ServiceDaten daten)
    {
      var max = HhPeriodeRep.GetMaxMin(daten);
      if (max == null)
      {
        max = new HhPeriode
        {
          Mandant_Nr = daten.MandantNr,
          Datum_Von = daten.Heute,
          Datum_Bis = daten.Heute,
        };
      }
      else
      {
        var min = HhPeriodeRep.GetMaxMin(daten, true);
        if (min != null)
          max.Datum_Von = min.Datum_Von;
      }
      var r = new ServiceErgebnis<HhPeriode>(max);
      return r;
    }

    /// <summary>Saves a new period.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="months">Number of months.</param>
    /// <param name="end">At the end or at the beginning.</param>
    public ServiceErgebnis SavePeriod(ServiceDaten daten, int months, bool end)
    {
      var dVon = daten.Heute.AddDays(-daten.Heute.DayOfYear + 1);
      var dBis = dVon.AddDays(-1);
      var lNr = 0;
      var min = HhPeriodeRep.GetMaxMin(daten, true);
      var max = HhPeriodeRep.GetMaxMin(daten, false);
      int pdiff;
      if (end)
      {
        if (max != null)
          lNr = max.Nr;
        pdiff = -1;
      }
      else
      {
        if (min != null)
          lNr = min.Nr;
        pdiff = 1;
      }
      if (lNr <= 0)
      {
        lNr = Constants.START_PERIODE;
        dVon = dBis.AddDays(1);
        dBis = dBis.AddMonths(months);
      }
      else
      {
        if (min == null || max == null)
        {
          throw new MessageException(HH003);
        }
        dVon = min.Datum_Von;
        dBis = max.Datum_Bis;
        if (end)
        {
          lNr++;
          dVon = dBis.AddDays(1);
          dBis = dBis.AddMonths(months);
          dBis = new DateTime(dBis.Year, dBis.Month, DateTime.DaysInMonth(dBis.Year, dBis.Month));
        }
        else
        {
          lNr--;
          dBis = dVon.AddDays(-1);
          dVon = dVon.AddMonths(-months);
        }
      }
      HhPeriodeRep.Save(daten, daten.MandantNr, lNr, dVon, dBis, 0);
      // Bilanz aktualisieren
      InsertBilanz2(daten, lNr, pdiff);
      // Neue Periode muss neu berechnet werden.
      if (HhPeriodeRep.Get(daten, daten.MandantNr, lNr + pdiff) == null)
      {
        // Wenn Periode nicht vorhanden ist.
        pdiff = 0;
      }
      SaveChanges(daten);
      SetzeBerPeriode(daten, lNr + pdiff);
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>Deletes a period.</summary>
    /// <param name="daten"></param>
    /// <param name="end">At the end or at the beginning?</param>
    public ServiceErgebnis DeletePeriod(ServiceDaten daten, bool end)
    {
      var minNr = GetMaxMinNr(daten, true);
      var maxNr = GetMaxMinNr(daten, false);
      if (minNr >= maxNr)
        throw new MessageException(HH004);
      int lNr;
      if (end)
        lNr = maxNr;
      else
        lNr = minNr;
      var p = HhPeriodeRep.Get(daten, daten.MandantNr, lNr);
      if (p != null)
        HhPeriodeRep.Delete(daten, p);
      var list = HhBilanzRep.GetList(daten, null, lNr, lNr);
      foreach (var b in list)
      {
        HhBilanzRep.Delete(daten, b);
      }
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Gets list of accounts.
    /// </summary>
    /// <returns>List of accounts.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="text">Affected text.</param>
    /// <param name="from">Affected minimum date.</param>
    /// <param name="to">Affected maximum date.</param>
    public ServiceErgebnis<List<HhKonto>> GetAccountList(ServiceDaten daten, string text = null, DateTime? from = null, DateTime? to = null)
    {
      var l = HhKontoRep.GetList(daten, -1, -1, null, null, from, to, text);
      return new ServiceErgebnis<List<HhKonto>>(l);
    }

    /// <summary>
    /// Gets an account.
    /// </summary>
    /// <returns>An account or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auid">Affected account ID.</param>
    public ServiceErgebnis<HhKonto> GetAccount(ServiceDaten daten, string auid)
    {
      var r = new ServiceErgebnis<HhKonto>(HhKontoRep.Get(daten, daten.MandantNr, auid));
      return r;
    }

    /// <summary>Saves an account.</summary>
    /// <param name="daten"></param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="type">Affected type.</param>
    /// <param name="attr">Affected attribute.</param>
    /// <param name="desc">Affected description.</param>
    /// <param name="from">Affected type.</param>
    /// <param name="to">Affected type.</param>
    /// <param name="value">Affected value on date from.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis SaveAccount(ServiceDaten daten, string uid, string type, string attr,
        string desc, DateTime? from, DateTime? to, decimal value)
    {
      var knr = uid;
      var lVon = Constants.MIN_PERIODE;
      var lBis = Constants.MAX_PERIODE;
      var lVonAlt = Constants.MIN_PERIODE;
      var lBisAlt = Constants.MAX_PERIODE;
      // var lBer = Constants.MAX_PERIODE;
      var strN = desc;
      var anzahl = 0;
      string sort = null;
      var insert = string.IsNullOrEmpty(uid);
      HhKonto hhKonto = null;

      if (string.IsNullOrEmpty(strN))
        throw new MessageException(HH007);
      if (!(type == Constants.ARTK_AKTIVKONTO || type == Constants.ARTK_PASSIVKONTO
          || type == Constants.ARTK_AUFWANDSKONTO || type == Constants.ARTK_ERTRAGSKONTO))
        throw new MessageException(HH008(type));
      if (!(string.IsNullOrEmpty(attr) || attr == Constants.KZK_EK || attr == Constants.KZK_GV || attr == Constants.KZK_DEPOT)
          || (!string.IsNullOrEmpty(attr) && attr.Length > 1))
        throw new MessageException(HH009(attr));
      if (!IstAktivPassivKontoIntern(type) && value != 0)
        throw new MessageException(HH083);
      if (IstSpezialKontokennzeichen(attr))
      {
        if (HhKontoRep.GetMin(daten, uid, attr) == null)
          throw new MessageException(HH010(attr));
        if (from.HasValue || to.HasValue)
          throw new MessageException(HH011);
      }
      if (from.HasValue)
        lVon = GetMaxMinNr(daten, false, from);
      if (!insert)
      {
        hhKonto = GetKontoIntern(daten, uid, true);
        if (hhKonto.Art != type)
          throw new MessageException(HH012);
        var kzAlt = hhKonto.Kz;
        if (IstSpezialKontokennzeichen(kzAlt) && kzAlt != attr)
          throw new MessageException(HH013(kzAlt));
        lVonAlt = Math.Max(hhKonto.Periode_Von, Constants.MIN_PERIODE);
        lBisAlt = Math.Max(hhKonto.Periode_Bis, Constants.MIN_PERIODE);
        sort = hhKonto.Sortierung;
        if (from.HasValue)
        {
          if (HhBuchungRep.GetList(daten, uid, null, to: from.Value.AddDays(-1)).Any())
            throw new MessageException(HH014);
        }
      }
      if (to.HasValue)
      {
        if (from.HasValue && to.Value < from.Value)
          throw new MessageException(HH015);
        lBis = GetMaxMinNr(daten, true, to);
        if (lBis <= 0)
          lBis = Constants.MAX_PERIODE;
        if (!insert && HhBuchungRep.GetList(daten, uid, null, from: to.Value.AddDays(1)).Any())
          throw new MessageException(HH016);
      }
      if (string.IsNullOrEmpty(sort))
        sort = FindKontoSortierung(daten, knr);
      if (HhKontoRep.GetMin(daten, knr, null, null, strN) != null)
        throw new MessageException(HH017(strN));
      if (insert)
      {
        knr = GetUid();
        if (IstAktivPassivKontoIntern(type))
        {
          // Konto in Eröffnungsbilanz einfügen
          var hhBilanz = new HhBilanz
          {
            Mandant_Nr = daten.MandantNr,
            Konto_Uid = knr,
            Kz = Constants.KZBI_EROEFFNUNG,
            SH = HoleBilanzSH(type),
            ESH = HoleBilanzSH(type)
          };
          hhBilanz.MachAngelegt(daten.Jetzt, daten.BenutzerId);
          anzahl = InsertBilanzPerioden(daten, hhBilanz, from, to);
        }
        if (IstAktivPassivKontoIntern(type))
        {
          // Konto in Schlussbilanz einfügen
          var hhBilanz = new HhBilanz
          {
            Mandant_Nr = daten.MandantNr,
            Konto_Uid = knr,
            Kz = Constants.KZBI_SCHLUSS,
            SH = HoleBilanzSH(type),
            ESH = HoleBilanzSH(type)
          };
          hhBilanz.MachAngelegt(daten.Jetzt, daten.BenutzerId);
          anzahl = InsertBilanzPerioden(daten, hhBilanz, from, to);
        }
        if (IstAufwandErtragKontoIntern(type))
        {
          // Konto in G+V-Rechnung einfügen
          var hhBilanz = new HhBilanz
          {
            Mandant_Nr = daten.MandantNr,
            Konto_Uid = knr,
            Kz = Constants.KZBI_GV,
            SH = HoleBilanzSH(type),
            ESH = HoleBilanzSH(type)
          };
          hhBilanz.MachAngelegt(daten.Jetzt, daten.BenutzerId);
          hhBilanz.Kz = Constants.KZBI_GV;
          anzahl = InsertBilanzPerioden(daten, hhBilanz, from, to);
        }
        if (anzahl <= 0)
          throw new MessageException(HH018);
        hhKonto = HhKontoRep.Save(daten, daten.MandantNr, knr, sort, type, attr, strN,
          from, to, lVon, lBis, Functions.KonvDM(value), value);
      }
      else
      {
        // Bilanz aktualisieren
        if (lVon > lVonAlt)
        {
          DeleteKontoVonBis(daten, uid, lVon - 1);
          // lBer = Math.Min(lBer, lVonAlt);
        }
        if (lBis < lBisAlt)
        {
          DeleteKontoVonBis(daten, uid, after: lBis + 1);
          // lBer = Math.Min(lBer, lBis);
        }
        if (lVon < lVonAlt)
        {
          var lMin = GetMaxMinNr(daten, true);
          lVon = Math.Max(lVon, lMin);
          for (var pnr = lVonAlt - 1; pnr >= lVon; pnr--)
          {
            InsertBilanz2(daten, pnr, 1);
          }
          // lBer = Math.Min(lBer, lVon);
        }
        if (lBis > lBisAlt)
        {
          var lMax = GetMaxMinNr(daten, false);
          lBis = Math.Min(lBis, lMax);
          for (var pnr = lBisAlt + 1; pnr <= lBis; pnr++)
          {
            InsertBilanz2(daten, pnr, 1);
          }
        }
        hhKonto.Art = type;
        hhKonto.Kz = attr;
        hhKonto.Name = strN;
        hhKonto.Gueltig_Von = from;
        hhKonto.Gueltig_Bis = to;
        hhKonto.Periode_Von = lVon;
        hhKonto.Periode_Bis = lBis;
        hhKonto.EBetrag = value;
        hhKonto.Betrag = Functions.KonvDM(value);
        HhKontoRep.Update(daten, hhKonto);
      }
      if (IstAktivPassivKontoIntern(type))
      {
        SaveChanges(daten);
        var p0 = HhPeriodeRep.GetMaxMin(daten, true, from);
        var bi = HhBilanzRep.Get(daten, daten.MandantNr, p0.Nr, Constants.KZBI_EROEFFNUNG, hhKonto.Uid);
        if (bi.EBetrag != value)
        {
          // Anfangswerte ändern und in allen Periode
          var diff = value - bi.EBetrag;
          var blist = HhBilanzRep.GetList(daten, Constants.KZBI_EROEFFNUNG, p0.Nr, auid: hhKonto.Uid);
          blist.AddRange(HhBilanzRep.GetList(daten, Constants.KZBI_SCHLUSS, p0.Nr, auid: hhKonto.Uid));
          foreach (var b in blist)
          {
            b.EBetrag += diff;
            b.Betrag = Functions.KonvDM(b.EBetrag);
            HhBilanzRep.Update(daten, b);
          }
        }
      }
      //if (string.IsNullOrEmpty(s) && string.IsNullOrEmpty(huid) && string.IsNullOrEmpty(wuid) && string.IsNullOrEmpty(muid) && string.IsNullOrEmpty(n)) {
      //    if (vermietung && !insert) {
      //        vmkontoRep.delete(daten, new VmKontoKey(daten.mandantNr, uid));
      //    }
      //} else {
      //    vmkontoRep.iuVmKonto(daten, null, hhKonto.uid, s, huid, wuid, muid, n, null, null, null, null);
      //}
      var r = new ServiceErgebnis<HhKonto>(hhKonto);
      return r;
    }

    /// <summary>
    /// Deletes an account.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeleteAccount(ServiceDaten daten, HhKonto e)
    {
      var hhKonto = GetKontoIntern(daten, e.Uid);
      var buliste = HhBuchungRep.GetList(daten, e.Uid, null, null);
      if (buliste.Any())
        throw new MessageException(HH020);
      if (IstSpezialKontokennzeichen(hhKonto.Kz))
        throw new MessageException(HH021);
      var biliste = HhBilanzRep.GetList(daten, null, auid: e.Uid);
      foreach (var b in biliste)
      {
        HhBilanzRep.Delete(daten, b);
      }
      foreach (var b in buliste)
      {
        HhBuchungRep.Delete(daten, b);
      }
      var erliste = HhEreignisRep.GetList(daten, e.Uid);
      foreach (var b in erliste)
      {
        HhEreignisRep.Delete(daten, b);
      }
      HhKontoRep.Delete(daten, hhKonto);
      // vmkontoRep.delete(daten, new VmKontoKey(daten.mandantNr, uid))
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Get the booking span of an account.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected uid.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis<string> GetBookingSpan(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<string>(HH022);
      var min = HhBuchungRep.GetList(daten, uid, null, desc: false, max: 1).FirstOrDefault();
      if (min != null)
      {
        var max = HhBuchungRep.GetList(daten, uid, null, desc: true, max: 1).FirstOrDefault();
        if (max != null)
          r.Ergebnis = HH023(min.Soll_Valuta, max.Soll_Valuta);
      }
      return r;
    }


    /// <summary>
    /// Gets list of events.
    /// </summary>
    /// <returns>List of events.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="text">Affected text.</param>
    /// <param name="from">Affected minimum date.</param>
    /// <param name="to">Affected maximum date.</param>
    public ServiceErgebnis<List<HhEreignis>> GetEventList(ServiceDaten daten, string text = null, DateTime? from = null, DateTime? to = null)
    {
      var l = HhEreignisRep.GetList(daten, null, from, to, text);
      return new ServiceErgebnis<List<HhEreignis>>(l);
    }

    /// <summary>
    /// Gets an event.
    /// </summary>
    /// <returns>An event or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="euid">Affected event ID.</param>
    public ServiceErgebnis<HhEreignis> GetEvent(ServiceDaten daten, string euid)
    {
      var r = new ServiceErgebnis<HhEreignis>(HhEreignisRep.Get(daten, daten.MandantNr, euid));
      return r;
    }

    /// <summary>Saves an event.</summary>
    /// <param name="daten"></param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="attr">Affected attribute.</param>
    /// <param name="duid">Affected debit account ID.</param>
    /// <param name="cuid">Affected credit account ID.</param>
    /// <param name="desc">Affected description.</param>
    /// <param name="text">Affected posting text.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis SaveEvent(ServiceDaten daten, string uid, string attr, string duid,
        string cuid, string desc, string text)
    {
      const int KZ_LAENGE = 1;
      const int ETEXT_LAENGE = 100;
      var r = new ServiceErgebnis<HhEreignis>();
      var insert = string.IsNullOrEmpty(uid);
      if (insert)
        uid = Functions.GetUid();
      if (string.IsNullOrEmpty(desc))
      {
        if (!insert)
          r.Errors.Add(Message.New(HH024));
        desc = HH025(uid);
      }
      if (attr != null && attr.Length > KZ_LAENGE)
        r.Errors.Add(Message.New(HH026(KZ_LAENGE)));
      if (string.IsNullOrEmpty(text))
        r.Errors.Add(Message.New(HH027));
      if (text.Length > ETEXT_LAENGE)
        text = text[..ETEXT_LAENGE];
      if (string.IsNullOrEmpty(duid))
        r.Errors.Add(Message.New(HH028));
      if (string.IsNullOrEmpty(cuid))
        r.Errors.Add(Message.New(HH029));
      if (!string.IsNullOrEmpty(duid) && !string.IsNullOrEmpty(cuid) && duid == cuid)
        r.Errors.Add(Message.New(HH030));
      if (!r.Ok)
        return r;
      r.Ergebnis = HhEreignisRep.Save(daten, daten.MandantNr, uid, attr, duid, cuid, desc, text);
      //if (string.IsNullOrEmpty(s) && string.IsNullOrEmpty(huid) && string.IsNullOrEmpty(wuid) && string.IsNullOrEmpty(muid) && string.IsNullOrEmpty(n)) {
      //    if (v && !insert) {
      //        vmereignisRep.delete(daten, new VmEreignisKey(daten.mandantNr, uid));
      //    }
      //} else {
      //    vmereignisRep.iuVmEreignis(daten, null, ereignis.uid, s, huid, wuid, muid, n, null, null, null, null);
      //}
      return r;
    }

    /// <summary>
    /// Deletes an event.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeleteEvent(ServiceDaten daten, HhEreignis e)
    {
      HhEreignisRep.Delete(daten, e);
      // vmereignisRep.delete(daten, new VmEreignisKey(daten.mandantNr, uid))
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Gets list of bookings.
    /// </summary>
    /// <returns>List of bookings.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="valuta">Search for value date.</param>
    /// <param name="from">Affected minimum date.</param>
    /// <param name="to">Affected maximum date.</param>
    /// <param name="text">Affected posting text.</param>
    /// <param name="auid">Affected account ID.</param>
    /// <param name="value">Affected value.</param>
    public ServiceErgebnis<List<HhBuchung>> GetBookingList(ServiceDaten daten, bool valuta,
        DateTime? from = null, DateTime? to = null, string text = null, string auid = null,
        string value = null)
    {
      var r = new ServiceErgebnis<List<HhBuchung>>
      {
        Ergebnis = HhBuchungRep.GetList(daten, auid, null, null, valuta, from, to, text, value, true)
      };
      return r;
    }

    /// <summary>
    /// Gets list of bookings in csv strings.
    /// </summary>
    /// <returns>List of bookings in csv strings.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="valuta">Search for value date.</param>
    /// <param name="from">Affected minimum date.</param>
    /// <param name="to">Affected maximum date.</param>
    /// <param name="text">Affected posting text.</param>
    /// <param name="auid">Affected account ID.</param>
    /// <param name="value">Affected value.</param>
    public ServiceErgebnis<List<string>> ExportBookingList(ServiceDaten daten, bool valuta,
        DateTime? from = null, DateTime? to = null, string text = null, string auid = null,
        string value = null)
    {
      var l = HhBuchungRep.GetList(daten, auid, null, null, valuta, from, to, text, value, true);
      var list = FillBookingList(l);
      var r = new ServiceErgebnis<List<string>>(list);
      return r;
    }

    /// <summary>
    /// Imports list of bookings.
    /// </summary>
    /// <returns>Message of import.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="list">List of addresses.</param>
    /// <param name="delete">Delete all bookings?</param>
    public ServiceErgebnis<string> ImportBookingList(ServiceDaten daten, List<string> list, bool delete)
    {
      if (delete)
      {
        var listem = HhBuchungRep.GetList(daten, daten.MandantNr);
        foreach (var b in listem)
          DeleteBuchungIntern(daten, b.Uid, false);
        SaveChanges(daten);
      }
      if (list == null || list.Count <= 1)
        throw new MessageException(HH050);
      var h = new Dictionary<string, int>();
      var check = false;
      var anzahl = 0;
      foreach (var line in list)
      {
        var werte = Functions.DecodeCSV(line);
        if (check)
        {
          var valuta = Functions.ToDateTime(GetWert(werte, h, "SollValuta")) ?? daten.Heute;
          var ebetrag = Functions.ToDecimal(GetWert(werte, h, "Ebetrag")) ?? 0;
          var betrag = Functions.KonvDM(ebetrag);
          var solluid = GetWert(werte, h, "SollKontoUid");
          var sk = GetKontoIntern(daten, solluid, false);
          if (sk == null)
          {
            var wert = GetWert(werte, h, "SollKonto");
            if (!string.IsNullOrEmpty(wert))
            {
              sk = HhKontoRep.GetMin(daten, null, null, null, wert);
              solluid = sk?.Uid;
            }
          }
          var habenuid = GetWert(werte, h, "HabenKontoUid");
          sk = GetKontoIntern(daten, habenuid, false);
          if (sk == null)
          {
            var wert = GetWert(werte, h, "HabenKonto");
            if (!string.IsNullOrEmpty(wert))
            {
              sk = HhKontoRep.GetMin(daten, null, null, null, wert);
              habenuid = sk?.Uid;
            }
          }
          var btext = GetWert(werte, h, "Btext");
          var belegnr = GetWert(werte, h, "BelegNr");
          var belegdatum = Functions.ToDateTime(GetWert(werte, h, "BelegDatum")) ?? valuta;
          var angelegtAm = Functions.ToDateTime(GetWert(werte, h, "AngelegtAm")) ?? daten.Jetzt;
          var angelegtVon = GetWert(werte, h, "AngelegtVon") ?? daten.BenutzerId;
          var geaendertAm = Functions.ToDateTime(GetWert(werte, h, "GeaendertAm"));
          var geaendertVon = GetWert(werte, h, "GeaendertVon");
          SaveBookingIntern(daten, null, valuta, null, betrag, ebetrag, solluid, habenuid, btext, belegnr,
            belegdatum, angelegtVon, angelegtAm, geaendertVon, geaendertAm);
          anzahl++;
        }
        else
        {
          foreach (var e in GetBookingColumns())
            h.Add(e, -1);
          for (var i = 0; werte != null && i < werte.Count; i++)
          {
            var c = werte[i];
            if (h.ContainsKey(c))
              h[c] = i;
          }
          foreach (var c in h.Keys)
          {
            if (h[c] < 0 && (c == "SollValuta" || c == "Ebetrag" || c == "SollKonto" || c == "HabenKonto" || c == "Btext"))
              throw new MessageException(HH051(c));
          }
          check = true;
        }
      }
      var r = new ServiceErgebnis<string>(HH053(anzahl));
      return r;
    }

    private static string GetWert(List<string> werte, Dictionary<string, int> h, string c)
    {
      if (h != null && c != null && h.ContainsKey(c))
      {
        var i = h[c];
        if (i >= 0 && werte != null && werte.Count > i)
          return Functions.TrimNull(werte[i]);
      }
      return null;
    }

    /// <summary>
    /// Gets a booking.
    /// </summary>
    /// <returns>A booking or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="buid">Affected booking ID.</param>
    public ServiceErgebnis<HhBuchung> GetBooking(ServiceDaten daten, string buid)
    {
      var r = new ServiceErgebnis<HhBuchung>(HhBuchungRep.Get(daten, daten.MandantNr, buid));
      return r;
    }

    /// <summary>
    /// Gets a new receipt number.
    /// </summary>
    /// <returns>A new receipt number.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected receipt date.</param>
    public ServiceErgebnis<string> GetNewReceipt(ServiceDaten daten, DateTime date)
    {
      var bu = HhBuchungRep.GetLastReceipt(daten, date);
      string belegNr;
      if (bu == null)
        belegNr = $"{date:yyyyMMdd}01";
      else
        belegNr = Functions.ToString(ToInt64(bu.Beleg_Nr) + 1);
      var r = new ServiceErgebnis<string>(belegNr);
      return r;
    }

    /// <summary>Saves a booking.</summary>
    /// <param name="daten"></param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="vd">Affected value date.</param>
    /// <param name="vdm">Affected value in DM.</param>
    /// <param name="v">Affected value in EUR.</param>
    /// <param name="duid">Affected debit account ID.</param>
    /// <param name="cuid">Affected credit account ID.</param>
    /// <param name="text">Affected posting text.</param>
    /// <param name="rn">Affected receipt number.</param>
    /// <param name="rd">Affected receipt date.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis<HhBuchung> SaveBooking(ServiceDaten daten, string uid, DateTime vd, decimal vdm,
      decimal v, string duid, string cuid, string text, string rn, DateTime rd)
    {
      var b = SaveBookingIntern(daten, uid, vd, null, vdm, v, duid, cuid, text, rn, rd, null, null, null, null);
      var r = new ServiceErgebnis<HhBuchung>(b);
      return r;
    }

    /// <summary>
    /// Reverses a booking.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis ReverseBooking(ServiceDaten daten, HhBuchung e)
    {
      var b = HhBuchungRep.Get(daten, e);
      if (b == null)
        throw new MessageException(HH043(e.Uid));
      b.Kz = b.Kz == Constants.KZB_AKTIV ? Constants.KZB_STORNO : Constants.KZB_AKTIV;
      HhBuchungRep.Update(daten, b);
      SetzePassendeBerPeriode(daten, b.Soll_Valuta);
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Deletes a booking.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeleteBooking(ServiceDaten daten, HhBuchung e)
    {
      DeleteBuchungIntern(daten, e.Uid, true);
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Gets list of balance rows.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="type">Affected balance type.</param>
    /// <param name="from">Affected start date.</param>
    /// <param name="to">Affected end date.</param>
    /// <returns>list of balance rows.</returns>
    public ServiceErgebnis<List<HhBilanz>> GetBalanceList(ServiceDaten daten, string type, DateTime from, DateTime to)
    {
      var r = new ServiceErgebnis<List<HhBilanz>>(GetBalanceListIntern(daten, type, from, to));
      return r;
    }

    /// <summary>
    /// Neuberechnung der Bilanzen in einer oder mehreren Perioden. Funktion: aktualisiereBilanz.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="status">Status of calculation is always updated.</param>
    /// <param name="cancel">Cancel calculation if not empty.</param>
    /// <param name="alles">All periods or only one.</param>
    /// <param name="von">Affected date in period.</param>
    /// <returns>0 alles aktuell; 1 eine Periode aktualisiert; 2 noch eine weitere kann Periode aktualisiert werden.</returns>
    public ServiceErgebnis<string[]> CalculateBalances(ServiceDaten daten, StringBuilder status, StringBuilder cancel,
      bool alles = false, DateTime? von = null)
    {
      var pber = HoleBerPeriodeIntern(daten); // erste zu berechnende Periode
      if (von.HasValue)
      {
        var pnr = HolePassendePeriodeNr(daten, von.Value);
        pber = pnr;
      }
      status.Clear().Append(M0(HH060));
      var rc = AktualisierenBilanz(daten, status, cancel, pber, alles);
      status.Clear();
      cancel.Append("End");
      var r = new ServiceErgebnis<string[]>(new[] { "", "" });
      r.Ergebnis[0] = Functions.ToString(rc);
      return r;
    }

    /// <summary>
    /// Tauschen der Sortierung von 2 Konten. Funktion: tauscheKontoSortierung.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auid">Betroffenes 1. Konto.</param>
    /// <param name="auid2">Betroffenes 2. Konto.</param>
    /// <returns>Evtl. Fehlermeldungen.</returns>
    public ServiceErgebnis SwapAccountSort(ServiceDaten daten, string auid, string auid2)
    {
      var k = GetKontoIntern(daten, auid);
      var k2 = GetKontoIntern(daten, auid2);
      // if (k == null || k2 == null) {
      // 	throw new MeldungException(Meldungen::HH044)
      // }
      var s = k.Sortierung;
      var s2 = k2.Sortierung;
      if (string.IsNullOrEmpty(s))
      {
        s = FindKontoSortierung(daten);
      }
      while (string.IsNullOrEmpty(s2) || s2 == s)
      {
        s2 = FindKontoSortierung(daten);
      }
      k.Sortierung = s2;
      HhKontoRep.Update(daten, k);
      k2.Sortierung = s;
      HhKontoRep.Update(daten, k2);
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Liefert den Jahresbericht als PDF-Dokument in Bytes.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="from">Betroffenes Anfangsdatum.</param>
    /// <param name="to">Betroffenes Enddatum.</param>
    /// <param name="title">Betroffener Titel.</param>
    /// <param name="ob">Betroffene Eröffnungsbilanz einschließen.</param>
    /// <param name="pl">Betroffene Gewinn+Verlust-Rechnung einschließen.</param>
    /// <param name="fb">Betroffene Schlussbilanz einschließen.</param>
    /// <returns>Jahresbericht als PDF-Dokument in Bytes.</returns>
    public ServiceErgebnis<byte[]> GetAnnualReport(ServiceDaten daten, DateTime from, DateTime to, string title, bool ob, bool pl, bool fb)
    {
      var r = new ServiceErgebnis<byte[]>();
      if (string.IsNullOrEmpty(title))
        r.Errors.Add(Message.New(HH045));
      List<AccountRow> oblist = null;
      List<AccountRow> pllist = null;
      List<AccountRow> fblist = null;
      var periode = Functions.GetPeriod(from, to, false);
      var euro = true;
      var ueberschrift = HH046(periode, title, daten.Jetzt);
      if (ob)
      {
        var liste = GetBalanceListIntern(daten, Constants.KZBI_EROEFFNUNG, from, from);
        oblist = GetBalanceRowList(liste, euro);
      }
      if (pl)
      {
        var liste = GetBalanceListIntern(daten, Constants.KZBI_GV, from, to);
        pllist = GetBalanceRowList(liste, euro);
      }
      if (fb)
      {
        var liste = GetBalanceListIntern(daten, Constants.KZBI_SCHLUSS, to, to);
        fblist = GetBalanceRowList(liste, euro);
      }
      var rp = new AnnualReport
      {
        Caption = ueberschrift,
        Ebliste = oblist,
        Gvliste = pllist,
        Sbliste = fblist,
      };
      r.Ergebnis = rp.Generate();
      return r;
    }

    /// <summary>
    /// Liefert den Kassenbericht als PDF-Dokument in Bytes.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="from">Betroffenes Anfangsdatum.</param>
    /// <param name="to">Betroffenes Enddatum.</param>
    /// <param name="title">Betroffener Titel.</param>
    /// <returns>Jahresbericht als PDF-Dokument in Bytes.</returns>
    public ServiceErgebnis<byte[]> GetCashReport(ServiceDaten daten, DateTime from, DateTime to, string title)
    {
      var r = new ServiceErgebnis<byte[]>();
      if (string.IsNullOrEmpty(title))
        r.Errors.Add(Message.New(HH045));
      var periode = Functions.GetPeriod(from, to, false);
      var euro = true;
      var ueberschrift = HH047(periode, title, daten.Jetzt);
      var monatlich = false;
      var dbE = 0m;
      var dbA = 0m;
      var ek = HoleEkKonto(daten);
      var gv = HoleGvKonto(daten);
      var ebListe = GetBalanceListIntern(daten, Constants.KZBI_EROEFFNUNG, from, from);
      var bek = ebListe.FirstOrDefault(a => a.Konto_Uid == ek);
      var dbV = bek == null ? 0 : euro ? bek.AccountEsum : bek.AccountSum;
      var gvListe = GetBalanceListIntern(daten, Constants.KZBI_GV, from, to);
      foreach (var b in gvListe)
      {
        var db = euro ? b.AccountEsum : b.AccountSum;
        if (b.Konto_Uid == gv)
          b.AccountName = null;
        else if (b.AccountType > 0)
          dbA += db;
        else
          dbE += db;
        b.AccountEsum = db;
      }
      var dbS = dbV + dbE - dbA;
      var kListe = HhKontoRep.GetList(daten, -1, -1, Constants.ARTK_AKTIVKONTO, null, to, from);
      var kpListe = HhKontoRep.GetList(daten, -1, -1, Constants.ARTK_PASSIVKONTO, null, to, from);
      foreach (var k in kpListe)
      {
        if (k.Uid != ek)
          kListe.Add(k);
      }
      foreach (var k in kListe)
      {
        k.Betrag = -GetKontoStandIntern(daten, k.Uid, from);
        k.EBetrag = -GetKontoStandIntern(daten, k.Uid, to);
      }
      var bListe = HhBuchungRep.GetList(daten, null, null, Constants.KZB_AKTIV, true, from, to, null, null, false, euro);
      foreach (var b in bListe)
      {
        var db = euro ? b.EBetrag : b.Betrag;
        b.EBetrag = db;
        if (string.IsNullOrEmpty(b.Beleg_Nr))
        {
          b.Beleg_Nr = b.Uid;
        }
      }

      // Gruppierung nach Soll- und Habenkonto
      var bMap = new Dictionary<string, HhBuchung>();
      foreach (var b in bListe)
      {
        if (b.CreditType == Constants.ARTK_ERTRAGSKONTO || b.CreditType == Constants.ARTK_AUFWANDSKONTO)
        {
          var bKey = b.Soll_Konto_Uid + b.Haben_Konto_Uid;
          if (bMap.TryGetValue(bKey, out var b2))
          {
            b2.EBetrag += b.EBetrag;
            b2.Beleg_Nr = b2.Beleg_Nr + ", " + b.Beleg_Nr;
          }
          else
            bMap.Add(bKey, Clone(b));
        }
      }
      var bListeE2 = bMap.Values.ToList();

      bMap.Clear();
      foreach (var b in bListe)
      {
        if (b.DebitType == Constants.ARTK_ERTRAGSKONTO || b.DebitType == Constants.ARTK_AUFWANDSKONTO)
        {
          var bKey = b.Soll_Konto_Uid + b.Haben_Konto_Uid;
          if (bMap.TryGetValue(bKey, out var b2))
          {
            b2.EBetrag += b.EBetrag;
            b2.Beleg_Nr = b2.Beleg_Nr + ", " + b.Beleg_Nr;
          }
          else
            bMap.Add(bKey, Clone(b));
        }
      }
      var bListeA2 = bMap.Values.ToList();
      var rp = new CashReport
      {
        Caption = ueberschrift,
        Monatlich = monatlich,
        From = from,
        To = to,
        Titel = title,
        Vortrag = dbV,
        Einnahmen = dbE,
        Ausgaben = dbA,
        Saldo = dbS,
        Kliste = kListe,
        Gvliste = gvListe,
        BlisteA = bListeA2,
        BlisteE = bListeE2,
        Bliste = bListe,
      };
      r.Ergebnis = rp.Generate();
      return r;
    }


    /// <summary>
    /// Gets list of proprietary in the last years.
    /// </summary>
    /// <param name="from">Affected date.</param>
    /// <param name="years">Affected years.</param>
    /// <returns>List of proprietary.</returns>
    /// <param name="daten">Service data for database access.</param>
    public ServiceErgebnis<List<HhBilanz>> GetProprietaryList(ServiceDaten daten, DateTime from, int years = 10)
    {
      var v = new List<HhBilanz>();
      var r = new ServiceErgebnis<List<HhBilanz>>(v);
      var p = HolePassendePeriode(daten, from, false);
      if (p == null)
        return r;
      var pnr = p.Nr;
      var diff = 1;
      var p2 = HhPeriodeRep.Get(daten, daten.MandantNr, pnr - 1);
      if (p2 != null && p.Datum_Von.Year * 12 + (p.Datum_Von.Month - 1) == p2.Datum_Von.Year * 12 + (p2.Datum_Von.Month - 1) + 1)
        diff = 12;
      var pnr2 = Constants.PN_BERECHNET;
      p2 = HolePassendePeriode(daten, from.AddYears(-years), false);
      if (p2 != null)
        pnr2 = p2.Nr - 1;
      var kontoUid = HoleEkKonto(daten, true);
      while (pnr > pnr2)
      {
        var hhBilanz = HhBilanzRep.Get(daten, daten.MandantNr, pnr, Constants.KZBI_SCHLUSS, kontoUid);
        if (hhBilanz != null)
        {
          var hhPeriode = HhPeriodeRep.Get(daten, daten.MandantNr, pnr);
          if (hhPeriode != null)
            hhBilanz.Geaendert_Am = hhPeriode.Datum_Bis;
          v.Add(hhBilanz);
        }
        pnr -= diff;
      }
      return r;
    }

    /// <summary>
    /// Gets list of proprietary and profit and loss in the last years.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="from">Affected date.</param>
    /// <param name="years">Affected years.</param>
    /// <returns>List of proprietary and profit and loss.</returns>
    public ServiceErgebnis<List<HhBilanz>> GetProprietaryPlList(ServiceDaten daten, DateTime from, int years = 10)
    {
      var v = new List<HhBilanz>();
      var r = new ServiceErgebnis<List<HhBilanz>>(v);
      var p = HolePassendePeriode(daten, from, false);
      if (p == null)
        return r;
      var pnr = p.Nr;
      var p2 = HolePassendePeriode(daten, from.AddYears(-years), false);
      var pnr2 = p2 == null ? Constants.PN_BERECHNET : p2.Nr - 1;
      var ek = HoleEkKonto(daten, true);
      var gv = HoleGvKonto(daten, true);
      while (pnr > pnr2)
      {
        var hhBilanz = HhBilanzRep.Get(daten, daten.MandantNr, pnr, Constants.KZBI_SCHLUSS, ek);
        if (hhBilanz != null)
        {
          var bi = HhBilanzRep.Get(daten, daten.MandantNr, pnr, Constants.KZBI_GV, gv);
          if (bi != null)
            hhBilanz.Betrag = -bi.Betrag;
          var hhPeriode = HhPeriodeRep.Get(daten, daten.MandantNr, pnr);
          if (hhPeriode != null)
            hhBilanz.Geaendert_Am = hhPeriode.Datum_Bis;
          v.Add(hhBilanz);
        }
        pnr--;
      }
      return r;
    }

    /**
     * Liefert den Stand eines Kontos am Anfang einer Periode.
     */
    private static decimal GetKontoStandIntern(ServiceDaten daten, string uid, DateTime from)
    {
      var euro = true;
      HhPeriode pVon;
      string strKz;
      var betrag = 0m;

      var hhKonto = GetKontoIntern(daten, uid, true);
      if (IstAktivPassivKontoIntern(hhKonto.Art))
      {
        strKz = Constants.KZBI_EROEFFNUNG;
      }
      else
      {
        // strKz = Constants.KZBI_GV;
        return betrag;
      }
      pVon = HolePassendePeriode(daten, from, false);
      if (pVon == null)
      {
        betrag = euro ? hhKonto.EBetrag : hhKonto.Betrag;
        return betrag;
      }
      var dV = from;
      var dB = from;
      var berechnen = false;
      if (from != pVon.Datum_Von)
      {
        dV = pVon.Datum_Von;
        dB = from;
        berechnen = true;
      }
      var result = HhBilanzRep.Get(daten, daten.MandantNr, pVon.Nr, strKz, uid);
      if (result != null)
      {
        betrag = euro ? result.EBetrag : result.Betrag;
      }

      if (berechnen)
      {
        var liste = HhBuchungRep.GetSumList(daten, uid, dV, dB, Constants.KZB_AKTIV, true);
        liste.AddRange(HhBuchungRep.GetSumList(daten, uid, dV, dB, Constants.KZB_AKTIV));
        foreach (var bu in liste)
        {
          betrag += euro ? bu.EBetrag : bu.Betrag;
        }
      }
      return betrag;
    }

    /// <summary>Liefert die Zeilen eines Bilanz-Kontos. Funktion: getBilanzDruckListe</summary>
    private static List<AccountRow> GetBalanceRowList(List<HhBilanz> liste, bool euro)
    {
      var list = new List<AccountRow>();
      var listeS = liste.Where(a => a.AccountType > 0).ToList();
      var listeH = liste.Where(a => a.AccountType <= 0).ToList();
      var l = Math.Max(listeS.Count, listeH.Count);
      for (var i = 0; i < l; i++)
      {
        var z = new AccountRow();
        if (i < listeS.Count)
        {
          var bi = listeS[i];
          z.Nr = bi.Konto_Uid;
          z.Name = bi.AccountName;
          z.Value = euro ? bi.AccountEsum : bi.AccountSum;
        }
        else
        {
          z.Nr = "";
          z.Name = "";
          z.Value = null;
        }
        if (i < listeH.Count)
        {
          var bi = listeH[i];
          z.Nr2 = bi.Konto_Uid;
          z.Name2 = bi.AccountName;
          z.Value2 = euro ? bi.AccountEsum : bi.AccountSum;
        }
        else
        {
          z.Nr2 = "";
          z.Name2 = "";
          z.Value2 = null;
        }
        list.Add(z);
      }
      return list;
    }

    /**
     * Neuberechnung der Bilanzen in einer oder mehreren Perioden.
     * @param bAlles True, wenn alle neu zu berechnenden Perioden in einer Schleife berechnet werden; sonst nur eine.
     * @return 0 alles aktuell; 1 eine Periode aktualisiert; 2 noch eine weitere kann Periode aktualisiert werden.
     * @throws Exception im unerwarteten Fehlerfalle.
     */
    private static int AktualisierenBilanz(ServiceDaten daten, StringBuilder status, StringBuilder cancel, int pber, bool bAlles)
    {
      var weiter = 0;
      var pmin = GetMaxMinNr(daten, true); // erste Periode
      var pmax = GetMaxMinNr(daten, false); // letzte Periode
      pber = Math.Max(pber, pmin);
      var berechnet = -1;
      var ek = HoleEkKonto(daten, true);
      var gv = HoleGvKonto(daten, true);
      var ende = false;
      for (var p = pber; p <= pmax && !ende; p++)
      {
        var ebliste = AnlegenKontenBilanz(daten, p, Constants.KZBI_EROEFFNUNG, pmin);
        var gvliste = AnlegenKontenBilanz(daten, p, Constants.KZBI_GV, pmin);
        var sbliste = AnlegenKontenBilanz(daten, p, Constants.KZBI_SCHLUSS, pmin);
        if (p > pmin)
        {
          var sbl0 = HhBilanzRep.GetList(daten, Constants.KZBI_SCHLUSS, p - 1, p - 1);
          var sbliste0 = new Dictionary<string, HhBilanz>();
          foreach (var b in sbl0)
          {
            sbliste0.Add(b.Konto_Uid, b);
          }
          UebernehmenBilanz(p - 1, sbliste0, Constants.KZBI_EROEFFNUNG, ebliste, true, ek, gv);
        }
        else
          AktualisierenEK(Constants.KZBI_EROEFFNUNG, ebliste, ek, gv);
        AktualisierenBilanz1(daten, p, ebliste, gvliste, sbliste, ek, gv);
        SaveBanlaceList(daten, ebliste);
        SaveBanlaceList(daten, gvliste);
        SaveBanlaceList(daten, sbliste);
        berechnet = p;
        SaveChanges(daten);
        var per = HhPeriodeRep.Get(daten, daten.MandantNr, p);
        if (per != null)
          status.Clear().Append(Functions.GetPeriod(per.Datum_Von, per.Datum_Bis, false));
        if (!bAlles || cancel.Length > 0)
          ende = true; // nur einen Durchgang
      }
      if (berechnet > -1)
        SetzeBerPeriode2(daten, berechnet, true);
      if (pber == pmax)
        weiter = 1;
      else if (pber < pmax)
        weiter = 2;
      return weiter;
    }

    private static void SaveBanlaceList(ServiceDaten daten, Dictionary<string, HhBilanz> l)
    {
      foreach (var b in l.Values.ToList())
      {
        HhBilanzRep.Save(daten, b.Mandant_Nr, b.Periode, b.Kz, b.Konto_Uid, b.SH, b.Betrag, b.ESH, b.EBetrag);
      }
    }

    /*
     * Anlegen aller Konten in allen Bilanz (außer Plan) zur vorgegebenen Periode.
     * @param pnr Perioden-Nummer.
     * @param strKz Bilanz-Kennzeichen.
     * @param pmin erste Perioden-Nummer.
     */
    private static Dictionary<string, HhBilanz> AnlegenKontenBilanz(ServiceDaten daten, int pnr, string strKz, int pmin)
    {
      var l = new Dictionary<string, HhBilanz>();
      var art1 = Constants.ARTK_AKTIVKONTO;
      var art2 = Constants.ARTK_PASSIVKONTO;
      if (strKz == Constants.KZBI_GV)
      {
        art1 = Constants.ARTK_AUFWANDSKONTO;
        art2 = Constants.ARTK_ERTRAGSKONTO;
      }
      var liste = HhKontoRep.GetList(daten, pnr, pnr, art1, art2, null, null);
      foreach (var k in liste)
      {
        var strSh = HoleBilanzSH(k.Art);
        var betrag = 0m;
        var ebetrag = 0m;
        if (strKz == Constants.KZBI_EROEFFNUNG)
        {
          // Bilanz-Betrag mit Konto-Betrag aktualisieren, wenn EB in dieser Periode ist.
          if (k.Periode_Von == pnr || k.Periode_Von <= pmin)
          {
            betrag = k.Betrag;
            ebetrag = k.EBetrag;
          }
        }
        l.Add(k.Uid, new HhBilanz
        {
          Mandant_Nr = daten.MandantNr,
          Periode = pnr,
          Kz = strKz,
          Konto_Uid = k.Uid,
          SH = strSh,
          Betrag = betrag,
          ESH = strSh,
          EBetrag = ebetrag
        });
      }
      return l;
    }

    /**
     * Übertragen der Beträge von einer Bilanz in eine andere mit evtl. anschließendem Neuberechnen des Eigenkapitals.
     * @param pnr1 Quell-Perioden-Nummer.
     * @param strK2 Ziel-Bilanz-Kennzeichen.
     * @param bEKAktuell True, wenn Eigenkapital neu berechnet wird.
     * @param ek Eigenkapital-Kontonummer.
     * @param gv Gewinn+Verlust-Kontonummer.
     */
    private static void UebernehmenBilanz(int pnr1, Dictionary<string, HhBilanz> l1,
      string strK2, Dictionary<string, HhBilanz> l2, bool bEKAktuell, string ek, string gv)
    {
      var liste = l2.Values.ToList();
      if (pnr1 <= 0 || l1 == null)
      {
        foreach (var b in liste)
        {
          b.Betrag = 0;
          b.EBetrag = 0;
        }
      }
      else
      {
        foreach (var b in liste)
        {
          if (l1.TryGetValue(b.Konto_Uid, out var b1))
          {
            b.Betrag = b1.Betrag;
            b.EBetrag = b1.EBetrag;
          }
        }
      }
      if (bEKAktuell)
      {
        // Konten können hinzugekommen oder weggefallen sein!
        AktualisierenEK(strK2, l2, ek, gv);
      }
    }

    /**
     * Aktualisieren der Eigenkapital- oder G+V-Kontos in einer Bilanz.
     * @param strK Bilanzkennzeichen (EB, SB, GV oder PL).
     * @param l Liste von Bilanz-Einträgen.
     * @param ek Eigenkapital-Kontonummer.
     * @param gv Gewinn+Verlust-Kontonummer.
     */
    private static void AktualisierenEK(string strK, Dictionary<string, HhBilanz> l, string ek, string gv)
    {
      var knr = strK == Constants.KZBI_GV || strK == Constants.KZBI_PLAN ? gv : ek;
      var liste = l.Values.Where(a => a.Konto_Uid != knr).ToList();
      var dbB = liste.Sum(a => a.Betrag);
      var dbEB = liste.Sum(a => a.EBetrag);
      if (l.TryGetValue(knr, out var b))
      {
        b.Betrag = -dbB;
        b.EBetrag = -dbEB;
      }
      else
        throw new MessageException(HH031);
    }

    /**
     * Aktualisieren aller Bilanzen einer Periode außer EB: Kopieren von EB nach SB; GV auf 0 setzen; Buchungen in SB
     * und GV eintragen; Summenkonten in SB, GB und PL aktualisieren.
     * @param pnr Perioden-Nummer.
     * @param ek Eigenkapital-Kontonummer.
     * @param gv Gewinn+Verlust-Kontonummer.
     */
    private static void AktualisierenBilanz1(ServiceDaten daten, int pnr, Dictionary<string, HhBilanz> ebliste,
      Dictionary<string, HhBilanz> gvliste, Dictionary<string, HhBilanz> sbliste,
      string ek, string gv)
    {
      var vo = HhPeriodeRep.Get(daten, daten.MandantNr, pnr);
      if (vo == null)
        throw new MessageException(HH005(pnr));
      UebernehmenBilanz(pnr, ebliste, Constants.KZBI_SCHLUSS, sbliste, false, ek, gv);
      UebernehmenBilanz(0, null, Constants.KZBI_GV, gvliste, false, ek, gv);
      string knr2 = null;
      var betrag = 0m;
      var ebetrag = 0m;
      string strA = null;
      var i = -1;
      var liste = HhBuchungRep.GetSumList(daten, null, vo.Datum_Von, vo.Datum_Bis, Constants.KZB_AKTIV, true);
      liste.AddRange(HhBuchungRep.GetSumList(daten, null, vo.Datum_Von, vo.Datum_Bis, Constants.KZB_AKTIV));

      // Schleife einmal mehr durchlaufen.
      var iWeiter = true;
      while (iWeiter)
      {
        i++;

        string knr;
        if (liste != null && i < liste.Count)
        {
          knr = liste[i].Soll_Konto_Uid;
          if (string.IsNullOrEmpty(knr2))
          {
            // Beim 1. Durchlauf nichts machen.
            knr2 = knr;
          }
        }
        else
        {
          knr = null;
          iWeiter = false;
        }
        if (knr != knr2)
        {
          if ((IstAktivPassivKontoIntern(strA) && sbliste.TryGetValue(knr2, out HhBilanz b)) || gvliste.TryGetValue(knr2, out b))
          {
            b.Betrag += betrag;
            b.EBetrag += ebetrag;
          }
          betrag = 0;
          ebetrag = 0;
        }
        if (iWeiter)
        {
          strA = liste[i].DebitType;
          betrag += liste[i].Betrag;
          ebetrag += liste[i].EBetrag;
          knr2 = knr;
        }
      }
      // Eigenkapital berechnen
      var strK = Constants.KZBI_SCHLUSS;
      AktualisierenEK(strK, sbliste, ek, gv);
      // Gewinn/Verlust berechnen
      strK = Constants.KZBI_GV;
      AktualisierenEK(strK, gvliste, ek, gv);
    }

    /// <summary>
    /// Gets list of balance rows.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="type">Affected balance type.</param>
    /// <param name="from">Affected start date.</param>
    /// <param name="to">Affected end date.</param>
    /// <returns>list of balance rows.</returns>
    private static List<HhBilanz> GetBalanceListIntern(ServiceDaten daten, string type, DateTime from, DateTime to)
    {
      var euro = true;
      var dV = from;
      var dB = to;
      var berechnen = false;
      var nullen = false;
      string ek = null;
      var kz = type;

      var pVon = HolePassendePeriode(daten, dV, false);
      var pBis = pVon;
      if (pVon == null)
        return new List<HhBilanz>();
      if (dV != dB)
        pBis = HolePassendePeriode(daten, dB, true);
      if (Constants.KZBI_EROEFFNUNG == kz)
      {
        if (dV != pVon.Datum_Von)
        {
          dV = pVon.Datum_Von;
          dB = from;
          ek = HoleEkKonto(daten, false);
          berechnen = true;
        }
      }
      else if (Constants.KZBI_GV == kz)
      {
        if (dV != pVon.Datum_Von || dB != pBis.Datum_Bis)
        {
          ek = HoleGvKonto(daten, false);
          berechnen = true;
          nullen = true;
        }
      }
      else if (Constants.KZBI_SCHLUSS == kz)
      {
        if (to != pBis.Datum_Bis)
        {
          dV = pBis.Datum_Von;
          kz = Constants.KZBI_EROEFFNUNG;
          ek = HoleEkKonto(daten, false);
          berechnen = true;
        }
      }
      var result = HhBilanzRep.GetPeriodSumList(daten, kz, pVon.Nr, pBis.Nr);
      if (berechnen)
      {
        var hash = new Dictionary<string, HhBilanz>();
        foreach (var b in result)
        {
          hash.Add(b.Konto_Uid, b);
          if (nullen)
          {
            b.AccountSum = 0;
            b.AccountEsum = 0;
          }
        }
        var bEk = hash[ek];
        var liste = HhBuchungRep.GetSumList(daten, null, dV, dB, Constants.KZB_AKTIV, true);
        liste.AddRange(HhBuchungRep.GetSumList(daten, null, dV, dB, Constants.KZB_AKTIV));
        foreach (var bu in liste)
        {
          if (hash.TryGetValue(bu.Soll_Konto_Uid, out var b))
          {
            b.AccountSum += bu.Betrag;
            b.AccountEsum += bu.EBetrag;
            if (bEk != null)
            {
              bEk.AccountSum -= bu.Betrag;
              bEk.AccountEsum -= bu.EBetrag;
            }
          }
        }
      }
      foreach (var b in result)
      {
        decimal betrag;
        string sh;
        if (euro)
        {
          betrag = b.AccountEsum;
          sh = b.ESH;
        }
        else
        {
          betrag = b.AccountSum;
          sh = b.SH;
        }
        if (betrag < 0)
        {
          b.AccountType = 1;
          b.AccountEsum = -b.AccountEsum;
          b.AccountSum = -b.AccountSum;
        }
        else if (betrag > 0)
          b.AccountType = 0;
        else if (betrag == 0)
        {
          if (Constants.KZSH_A == sh)
            b.AccountType = 1;
          b.AccountEsum = 0;
          b.AccountSum = 0;
        }
      }
      return result;
    }

    /// <summary>Saves a booking with revision.</summary>
    /// <param name="daten"></param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="vd">Affected value date.</param>
    /// <param name="kz">Affected attribute.</param>
    /// <param name="vdm">Affected value in DM.</param>
    /// <param name="v">Affected value in EUR.</param>
    /// <param name="duid">Affected debit account ID.</param>
    /// <param name="cuid">Affected credit account ID.</param>
    /// <param name="text">Affected posting text.</param>
    /// <param name="rn">Affected receipt number.</param>
    /// <param name="rd">Affected receipt date.</param>
    /// <param name="angelegtVon">Affected creator.</param>
    /// <param name="angelegtAm">Affected creation date.</param>
    /// <param name="geaendertVon">Affected changer.</param>
    /// <param name="geaendertAm">Affected change date.</param>
    /// <returns>Saved entity.</returns>
    public HhBuchung SaveBookingIntern(ServiceDaten daten, string uid, DateTime vd, string kz, decimal vdm, decimal v,
      string sollUid, string habenUid, string text, string bn, DateTime bd, string angelegtVon, DateTime? angelegtAm,
      string geaendertVon, DateTime? geaendertAm)
    {
      kz = kz == Constants.KZB_STORNO ? Constants.KZB_STORNO : Constants.KZB_AKTIV;
      var strT = text;
      DateTime dValt;

      var balt = HhBuchungRep.Get(daten, daten.MandantNr, uid);
      var insert = balt == null;
      PruefBuchung(daten, vd, vdm, v, sollUid, habenUid, strT, bn, bd);
      var buchung = HhBuchungRep.Save(daten, daten.MandantNr, uid, vd, vd, kz, vdm, v, sollUid, habenUid, strT, bn, bd,
        angelegtVon, angelegtAm, geaendertVon, geaendertAm);
      if (insert)
        SetzePassendeBerPeriode(daten, vd);
      else
      {
        dValt = balt.Soll_Valuta;
        if (dValt > vd)
          dValt = vd;
        SetzePassendeBerPeriode(daten, dValt);
      }
      return buchung;
    }

    /**
     * Prüfung der Buchung.
     * @param daten Service-Daten für Datenbankzugriff.
     * @param buchung HhBuchung-Instanz.
     */
    private static void PruefBuchung(ServiceDaten daten, DateTime? sv, decimal b, decimal eb, string sollUid,
      string habenUid, string text, string bn, DateTime? bd)
    {
      if (sv == null)
        throw new MessageException(HH033);
      if (b <= 0 || eb <= 0)
        throw new MessageException(HH034);
      if (string.IsNullOrEmpty(sollUid))
        throw new MessageException(HH028);
      if (string.IsNullOrEmpty(habenUid))
        throw new MessageException(HH029);
      if (habenUid == sollUid)
        throw new MessageException(HH030);
      var ek = HoleEkKonto(daten);
      var gv = HoleGvKonto(daten);
      if (sollUid == ek || habenUid == ek || sollUid == gv || habenUid == gv)
        throw new MessageException(HH035);
      if (string.IsNullOrEmpty(text))
        throw new MessageException(HH027);
      Functions.MachNichts(bn);
      if (bd == null)
        throw new MessageException(HH036);
      var hhKonto = GetKontoIntern(daten, sollUid, false);
      if (hhKonto == null)
        throw new MessageException(HH037);
      if (hhKonto.Gueltig_Von.HasValue && sv < hhKonto.Gueltig_Von.Value)
        throw new MessageException(HH038(hhKonto.Gueltig_Von.Value));
      if (hhKonto.Gueltig_Bis.HasValue && sv > hhKonto.Gueltig_Bis.Value)
        throw new MessageException(HH039(hhKonto.Gueltig_Bis.Value));
      hhKonto = GetKontoIntern(daten, habenUid, false);
      if (hhKonto == null)
        throw new MessageException(HH040);
      if (hhKonto.Gueltig_Von.HasValue && sv < hhKonto.Gueltig_Von.Value)
        throw new MessageException(HH041(hhKonto.Gueltig_Von.Value));
      if (hhKonto.Gueltig_Bis.HasValue && sv > hhKonto.Gueltig_Bis.Value)
        throw new MessageException(HH042(hhKonto.Gueltig_Bis.Value));
    }

    /**
     * Liefert die Konto-Nummer des Eigenkapitals.
     * @param mandantNr Mandantennummer.
     * @param exception Soll eine Exception geworfen werden, wenn das Konto fehlt?
     * @return Konto-Nummer des Eigenkapitals.
     */
    private static string HoleEkKonto(ServiceDaten daten, bool exception = true)
    {
      var ek = HhKontoRep.GetMin(daten, null, Constants.KZK_EK, null, null);
      if (exception && ek == null)
        throw new MessageException(HH031);
      return ek?.Uid;
    }

    /**
     * Liefert die Konto-Nummer des G+V-Kontos.
     * @param mandantNr Mandantennummer.
     * @param exception Soll eine Exception geworfen werden, wenn das Konto fehlt?
     * @return Konto-Nummer des G+V-Kontos.
     */
    private static string HoleGvKonto(ServiceDaten daten, bool exception = true)
    {
      var gv = HhKontoRep.GetMin(daten, null, Constants.KZK_GV, null, null);
      if (exception && gv == null)
        throw new MessageException(HH032);
      return gv?.Uid;
    }

    private static void DeleteBuchungIntern(ServiceDaten daten, string uid, bool pruefen)
    {
      var b = HhBuchungRep.Get(daten, daten.MandantNr, uid);
      if (b == null && pruefen)
        throw new MessageException(HH043(uid));
      SetzePassendeBerPeriode(daten, b.Soll_Valuta);
      HhBuchungRep.Delete(daten, b);
      // vmbuchungRep.delete(daten, new VmBuchungKey(daten.mandantNr, uid))
    }

    /// <summary>Setzen der passenden berechneten Periode.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="datum">Affected date.</param>
    private static void SetzePassendeBerPeriode(ServiceDaten daten, DateTime datum)
    {
      var nr = HolePassendePeriodeNr(daten, datum); // Exception in case of error
      SetzeBerPeriode(daten, nr);
    }

    /// <summary>Liefert die Perioden-Nummer, in der das angegebene Datum liegt.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="d">Affected date.</param>
    /// <returns>Passende Perioden-Nummer.</returns>
    private static int HolePassendePeriodeNr(ServiceDaten daten, DateTime d)
    {
      var p = HolePassendePeriode(daten, d, true);
      return p.Nr;
    }

    /**
     * Liefert die Periode, in der das angegebene Datum liegt.
     * @param daten Service-Daten für Datenbankzugriff.
     * @param d Datum.
     * @return Passende Periode.
     */
    private static HhPeriode HolePassendePeriode(ServiceDaten daten, DateTime d, bool exception = false)
    {
      var p = HhPeriodeRep.GetMaxMin(daten, false, d);
      if (p == null && exception)
        throw new MessageException(HH018);
      return p;
    }

    /// <summary>Setzen der zu berechnenden Periode.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected period number.</param>
    private static void SetzeBerPeriode(ServiceDaten daten, int nr)
    {
      if (HhPeriodeRep.Get(daten, daten.MandantNr, nr) == null)
        throw new MessageException(HH005(nr));
      var lBer = HoleBerPeriodeIntern(daten);
      if (lBer >= Constants.MIN_PERIODE && lBer <= nr)
        return; // frühere Periode ist schon nicht berechnet!
      var dHeute = daten.Heute;
      HhPeriodeRep.Save(daten, daten.MandantNr, Constants.PN_BERECHNET, dHeute, dHeute, nr);
    }

    /// <summary>Setzen der berechneten Periode. Die nächste Periode ist zu berechnen.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected period number.</param>
    /// <param name="berechnet">Is the period calculated? If yes, then set the next period.</param>
    private static void SetzeBerPeriode2(ServiceDaten daten, int nr, bool berechnet = false)
    {
      var minNr = GetMaxMinNr(daten, true);
      if (nr >= minNr)
      {
        if (HhPeriodeRep.Get(daten, daten.MandantNr, nr) == null)
          throw new MessageException(HH005(nr));
        if (berechnet)
          nr++;
      }
      else
        nr = minNr;
      var dHeute = daten.Heute;
      HhPeriodeRep.Save(daten, daten.MandantNr, Constants.PN_BERECHNET, dHeute, dHeute, nr);
    }

    /// <summary>Liefert die nächste zu berechnende Perioden-Nummer.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Nächste zu berechnende Perioden-Nummer.</returns>
    private static int HoleBerPeriodeIntern(ServiceDaten daten)
    {
      var nr = 0;
      var p = HhPeriodeRep.Get(daten, daten.MandantNr, Constants.PN_BERECHNET);
      if (p != null)
        nr = p.Art;
      if (nr <= 0)
        nr = GetMaxMinNr(daten, true);
      return nr;
    }

    /// <summary>Einfügen von Bilanz-Einträgen aus anderer Periode für ein oder alle Konten.</summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="pnr">Quell-Perioden-Nummer.</param>
    /// <param name="pdiff">Differenz zwischen Quell- und Ziel-Perioden-Nummer, üblicherweise 1 oder -1.</param>
    private static void InsertBilanz2(ServiceDaten daten, int pnr, int pdiff)
    {
      var vListe = new List<string>();

      // Bilanz-Einträge für eine Periode und ein oder alle Konten erstellen
      var hhKontos = HhKontoRep.GetList(daten, pnr, pnr);
      foreach (var hhKonto in hhKontos)
      {
        if (IstAktivPassivKontoIntern(hhKonto.Art))
        {
          vListe.Add(HH006(pnr, Constants.KZBI_EROEFFNUNG, HoleBilanzSH(hhKonto.Art), hhKonto.Uid));
          vListe.Add(HH006(pnr, Constants.KZBI_SCHLUSS, HoleBilanzSH(hhKonto.Art), hhKonto.Uid));
        }
        else
          vListe.Add(HH006(pnr, Constants.KZBI_GV, HoleBilanzSH(hhKonto.Art), hhKonto.Uid));
      }

      foreach (var str in vListe)
      {
        var pnr3 = ToInt32(str[..10]);
        var strK3 = str.Substring(10, 2);
        var strS3 = str.Substring(12, 1);
        var knr3 = str[13..];
        HhBilanz hhBilanz = HhBilanzRep.Get(daten, daten.MandantNr, pnr3 + pdiff, strK3, knr3);
        decimal dbB;
        decimal dbEB;
        if (hhBilanz != null)
        {
          if (Constants.KZBI_GV == strK3)
          {
            dbB = 0;
            dbEB = 0;
          }
          else
          {
            dbB = hhBilanz.Betrag;
            dbEB = hhBilanz.EBetrag;
          }
          HhBilanzRep.Save(daten, daten.MandantNr, pnr3, strK3, knr3, strS3, dbB, strS3, dbEB);
        }
      }
    }

    /**
     * Prüfung auf Aktiv- oder Passiv-Konto.
     * @param art Konto-Art.
     * @return True, wenn Aktiv- oder Passiv-Konto.
     */
    private static bool IstAktivPassivKontoIntern(string art)
    {
      return (Constants.ARTK_AKTIVKONTO == art || Constants.ARTK_PASSIVKONTO == art);
    }

    /**
     * Prüfung auf Aktiv- oder Aufwands-Konto.
     * @param art Konto-Art.
     * @return True, wenn Aktiv- oder Aufwands-Konto.
     */
    private static bool IstAktivAufwandKontoIntern(string art)
    {
      return (Constants.ARTK_AKTIVKONTO == art || Constants.ARTK_AUFWANDSKONTO == art);
    }

    /**
     * Prüfung auf Aufwand- oder Ertrag-Konto.
     * @param strArt Konto-Art.
     * @return True, wenn Aufwand- oder Ertrag-Konto.
     */
    private static bool IstAufwandErtragKontoIntern(string art)
    {
      return (Constants.ARTK_AUFWANDSKONTO == art || Constants.ARTK_ERTRAGSKONTO == art);
    }

    /**
     * Liefert Soll/Haben-Kennzeichen für Bilanz an Hand der Konto-Art. A bei Aktiv- oder Aufwandskonto, sonst P.
     * @param art Konto-Art.
     * @return Soll/Haben-Kennzeichen für Bilanz: A oder P.
     */
    private static string HoleBilanzSH(string art)
    {
      if (IstAktivAufwandKontoIntern(art))
        return Constants.KZSH_A;
      // if(strArt == ARTK_PASSIVKONTO || strArt == ARTK_ERTRAGSKONTO)
      return Constants.KZSH_P;
    }

    /// <summary>
    /// Gets extreme period number.
    /// </summary>
    /// <returns>Number of first or last period.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="min">Minimum or maximum.</param>
    /// <param name="d">Affected minimum or maximum date.</param>
    private static int GetMaxMinNr(ServiceDaten daten, bool min, DateTime? d = null)
    {
      var p = HhPeriodeRep.GetMaxMin(daten, min, d);
      return p == null ? 0 : p.Nr;
    }

    private static HhKonto GetKontoIntern(ServiceDaten daten, string uid, bool exception = true)
    {
      var hhKonto = HhKontoRep.Get(daten, daten.MandantNr, uid);
      if (exception && hhKonto == null)
        throw new MessageException(HH019(uid));
      return hhKonto;
    }

    /// <summary>Liefert true bei Spezial-Kontokennzeichen E und G.</summary>
    /// <returns>true bei Spezial-Kontokennzeichen E und G.</returns>
    /// <param name="kz">Betroffenes Kontokennzeichen.</param>
    private static bool IstSpezialKontokennzeichen(string kz)
    {
      return kz == Constants.KZK_EK || kz == Constants.KZK_GV;
    }

    private static string FindKontoSortierung(ServiceDaten daten, string uid = null)
    {
      string sort = null;
      while (string.IsNullOrEmpty(sort))
      {
        sort = string.Format("{0:0000000000}", NextRandom(1000000, 10000000));
        var k = HhKontoRep.GetMin(daten, uid, null, sort, null);
        if (k != null)
          sort = null;
      }
      return sort;
    }

    private static int InsertBilanzPerioden(ServiceDaten daten, HhBilanz hhBilanz, DateTime? from, DateTime? to)
    {
      var list = HhPeriodeRep.GetList(daten, from, to);
      foreach (var p in list)
      {
        hhBilanz.Periode = p.Nr;
        HhBilanzRep.Insert(daten, Clone(hhBilanz));
      }
      return list.Count;
    }

    private static void DeleteKontoVonBis(ServiceDaten daten, string kontoUid, int before = -1, int after = -1)
    {
      var l = HhBilanzRep.GetList(daten, null, after, before, kontoUid);
      foreach (var b in l)
      {
        HhBilanzRep.Delete(daten, b);
      }
    }

    private static List<string> GetBookingColumns()
    {
      var columns = new List<string>{ "SollValuta", "Btext", "Ebetrag", "Uid", "Kz", "SollKontoUid",
        "SollKonto", "HabenKontoUid", "HabenKonto", "BelegNr", "BelegDatum", "HabenValuta",
        "Betrag", "AngelegtVon", "AngelegtAm", "GeaendertVon", "GeaendertAm" };
      return columns;
    }

    private static List<string> FillBookingList(List<HhBuchung> bookings)
    {
      var list = new List<string>();
      var columns = GetBookingColumns();
      list.Add(EncodeCSV(columns));
      if (bookings == null)
        return list;
      foreach (var b in bookings)
      {
        var l = new List<string> { ToStr(b.Soll_Valuta), ToStr(b.BText), ToStr(b.EBetrag),
          ToStr(b.Uid), ToStr(b.Kz), ToStr(b.Soll_Konto_Uid),
          ToStr(b.DebitName), ToStr(b.Haben_Konto_Uid), ToStr(b.CreditName),
          ToStr(b.Beleg_Nr), ToStr(b.Beleg_Datum), ToStr(b.Haben_Valuta),
          ToStr(b.Betrag), ToStr(b.Angelegt_Von), ToStr(b.Angelegt_Am), ToStr(b.Geaendert_Von),
          ToStr(b.Geaendert_Am)};
        list.Add(EncodeCSV(l));
      }
      return list;
    }

  }
}
