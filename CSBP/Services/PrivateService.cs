// <copyright file="PrivateService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>
/// Implementation of private service.
/// </summary>
public class PrivateService : ServiceBase, IPrivateService
{
  /// <summary>Fixed bike year in statistics.</summary>
  private static readonly bool BikeYearFixed = Functions.MachNichts() != 0;

  /// <summary>Sets budget service.</summary>
  public IBudgetService BudgetService { private get; set; }

  /// <summary>
  /// Gets a list of bikes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of bikes.</returns>
  public ServiceErgebnis<List<FzFahrrad>> GetBikeList(ServiceDaten daten)
  {
    var l = FzFahrradRep.GetList(daten, daten.MandantNr).OrderBy(a => a.Bezeichnung).ToList();
    return new ServiceErgebnis<List<FzFahrrad>>(l);
  }

  /// <summary>
  /// Gets a bike.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of bike.</param>
  /// <returns>List of Bikes.</returns>
  public ServiceErgebnis<FzFahrrad> GetBike(ServiceDaten daten, string uid)
  {
    var e = FzFahrradRep.Get(daten, daten.MandantNr, uid);
    return new ServiceErgebnis<FzFahrrad>(e);
  }

  /// <summary>
  /// Saves a bike.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of bike.</param>
  /// <param name="desc">Description of bike.</param>
  /// <param name="type">Type of bike.</param>
  /// <returns>Created or changed bike.</returns>
  public ServiceErgebnis<FzFahrrad> SaveBike(ServiceDaten daten, string uid, string desc, int type)
  {
    var r = new ServiceErgebnis<FzFahrrad>();
    desc = desc.TrimNull();
    if (string.IsNullOrWhiteSpace(desc))
      throw new MessageException(FZ037);
    if (type < (int)BikeTypeEnum.Tour || type > (int)BikeTypeEnum.Weekly)
      throw new MessageException(FZ038);
    r.Ergebnis = FzFahrradRep.Save(daten, daten.MandantNr, uid, desc, type);
    return r;
  }

  /// <summary>
  /// Deletes a bike.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteBike(ServiceDaten daten, FzFahrrad e)
  {
    var liste = FzFahrradstandRep.GetList(daten, e.Uid);
    foreach (var m in liste)
    {
      FzFahrradstandRep.Delete(daten, m);
    }
    FzFahrradRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of mileages.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected bike ID.</param>
  /// <param name="text">Affected text.</param>
  /// <returns>List of mileages.</returns>
  public ServiceErgebnis<List<FzFahrradstand>> GetMileageList(ServiceDaten daten, string buid, string text)
  {
    var l = FzFahrradstandRep.GetList(daten, buid, text: text, desc: true);
    return new ServiceErgebnis<List<FzFahrradstand>>(l);
  }

  /// <summary>
  /// Gets a list of mileages for statistics.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="years">Affected years.</param>
  /// <returns>List of mileages.</returns>
  public ServiceErgebnis<List<FzFahrradstand>> GetMileages(ServiceDaten daten, DateTime date, int years = 10)
  {
    var v = new List<FzFahrradstand>();
    var r = new ServiceErgebnis<List<FzFahrradstand>>(v);
    if (BikeYearFixed)
      date = Functions.Sunday(date);
    var von = date.AddYears(-years);
    if (BikeYearFixed)
      von = von.AddDays(1 - date.DayOfYear); // separated commands because of leap year
    var min = FzFahrradstandRep.GetList(daten, null, desc: false, max: 1).FirstOrDefault();
    if (min != null)
      von = min.Datum;
    var schnitt = 0m;
    var summe = 0m;
    var d = von;
    while (d.Year < date.Year || (BikeYearFixed && d.Year == date.Year))
    {
      var dbis = d.Year < date.Year ? d.AddYears(1) : date;
      if (BikeYearFixed && d.Year < date.Year)
        dbis = dbis.AddDays(-d.DayOfYear);
      var kmJahr = FzFahrradstandRep.Count(daten, null, d, dbis);
      summe += kmJahr;
      int tage = ((dbis.Year - von.Year) * 365) + dbis.DayOfYear - von.DayOfYear;
      if (tage != 0)
        schnitt = summe * 365 / tage;
      var s = new FzFahrradstand
      {
        Datum = d,
        Periode_km = kmJahr,
        Zaehler_km = schnitt,
      };
      v.Add(s);
      d = d.AddYears(1);
      if (BikeYearFixed)
        d = d.AddDays(1 - d.DayOfYear);
      else
        d = d.AddDays(date.DayOfYear - d.DayOfYear);
    }
    return r;
  }

  /// <summary>
  /// Gets a mileage.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected bike id.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="nr">Affected number.</param>
  /// <returns>Mileage or null.</returns>
  public ServiceErgebnis<FzFahrradstand> GetMileage(ServiceDaten daten, string buid, DateTime date, int nr)
  {
    var e = FzFahrradstandRep.GetList(daten, buid, date, nr).FirstOrDefault();
    return new ServiceErgebnis<FzFahrradstand>(e);
  }

  /// <summary>
  /// Saves a mileage.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected bike id.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="nr0">Affected number.</param>
  /// <param name="odometer">Affected odometer.</param>
  /// <param name="km">Affected km.</param>
  /// <param name="average">Affected average.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="angelegtVon">Affected creator.</param>
  /// <param name="angelegtAm">Affected creation date.</param>
  /// <param name="geaendertVon">Affected changer.</param>
  /// <param name="geaendertAm">Affected change date.</param>
  /// <returns>Created or changed mileage.</returns>
  public ServiceErgebnis<FzFahrradstand> SaveMileage(ServiceDaten daten, string buid, DateTime date, int nr0,
    decimal odometer, decimal km, decimal average, string text, string angelegtVon = null, DateTime? angelegtAm = null,
    string geaendertVon = null, DateTime? geaendertAm = null)
  {
    var r = new ServiceErgebnis<FzFahrradstand>();
    var insert = nr0 < 0;
    var dAktuell = date;
    var nr = insert ? 0 : nr0;
    var besch = text.TrimNull();
    FzFahrradstand voVorher = null;
    FzFahrradstand voNachher = null;
    var zaehlerAktuell = odometer;
    var periodeAktuell = km;
    var zaehlerNull = false;
    const int protag = 10 + 1;

    if (string.IsNullOrEmpty(buid))
      r.Errors.Add(Message.New(FZ019));
    if (Functions.CompDouble(zaehlerAktuell, 0) < 0)
      r.Errors.Add(Message.New(FZ020));
    if (Functions.CompDouble(periodeAktuell, 0) < 0)
      r.Errors.Add(Message.New(FZ021));
    if (Functions.CompDouble(average, 0) < 0)
      r.Errors.Add(Message.New(FZ022));
    if (Functions.CompDouble(zaehlerAktuell, 0) == 0 && Functions.CompDouble(periodeAktuell, 0) == 0)
      zaehlerNull = true;
    //// if (date == null) {
    ////    r.Errors.Add(Message.New(FZ023));
    //// }
    var fzFahrrad = FzFahrradRep.Get(daten, daten.MandantNr, buid);
    if (fzFahrrad == null)
    {
      r.Errors.Add(Message.New(FZ024(buid)));
      return r;
    }
    var typ = fzFahrrad.Typ;
    if (typ < (int)BikeTypeEnum.Tour || typ > (int)BikeTypeEnum.Weekly)
      throw new MessageException(FZ038);
    if (!r.Ok)
      return r;
    if (insert && fzFahrrad.IsWeekly)
    {
      // Inserts new mileage.
      // Reads possibly existing mileage in same week.
      var l = FzFahrradstandRep.GetList(daten, buid, datege: dAktuell, max: protag);
      if (l.Any())
      {
        insert = false;
        var b = l.First();
        dAktuell = b.Datum;
        nr = b.Nr;
        if (string.IsNullOrEmpty(besch))
          besch = b.Beschreibung;
      }
    }
    //// getMaxVorher
    var liste = FzFahrradstandRep.GetList(daten, buid, datele: dAktuell, desc: true, max: protag);
    if (liste.Any())
    {
      var i = 0;
      do
      {
        var fzFahrradstand = liste[i];
        if (dAktuell != fzFahrradstand.Datum || (dAktuell == fzFahrradstand.Datum && nr > fzFahrradstand.Nr))
          voVorher = fzFahrradstand;
        i++;
      }
      while (voVorher == null && i < liste.Count);
    }
    //// getMinNachher
    liste = FzFahrradstandRep.GetList(daten, buid, datege: dAktuell, max: protag);
    if (liste.Any())
    {
      var i = 0;
      do
      {
        var fzFahrradstand = liste[i];
        if (dAktuell != fzFahrradstand.Datum || (dAktuell == fzFahrradstand.Datum && nr < fzFahrradstand.Nr))
          voNachher = fzFahrradstand;
        i++;
      }
      while (voNachher == null && liste.Count > i);
    }
    var zaehlerVorher = 0m;
    var zaehlerNachher = 0m;
    var periodeNachher = 0m;
    if (voVorher != null)
    {
      // vorherNr > 0
      if (!zaehlerNull)
      {
        zaehlerVorher = voVorher.Zaehler_km;
        if (Functions.CompDouble(periodeAktuell, 0) > 0)
          zaehlerAktuell = zaehlerVorher + periodeAktuell;
        else
        {
          periodeAktuell = zaehlerAktuell - zaehlerVorher;
          if (Functions.CompDouble(periodeAktuell, 0) < 0)
            throw new MessageException(FZ025(zaehlerVorher));
        }
      }
    }
    if (voNachher != null)
    {
      // nachherNr > 0
      if (insert)
      {
        if (fzFahrrad.IsWeekly)
        {
          nr = voNachher.Nr;
          dAktuell = voNachher.Datum;
        }
        else
          throw new MessageException(FZ026);
      }
      if (!zaehlerNull)
      {
        zaehlerNachher = voNachher.Zaehler_km;
        periodeNachher = voNachher.Periode_km;
      }
    }
    if (voNachher != null /* nachherNr > 0 */ && zaehlerNull)
      throw new MessageException(FZ027);
    if (!zaehlerNull)
    {
      if (Functions.CompDouble(zaehlerAktuell, periodeAktuell) < 0)
        zaehlerAktuell = periodeAktuell;
      if (voVorher == null /* vorherNr <= 0 */ && Functions.CompDouble(zaehlerAktuell, periodeAktuell) > 0)
        periodeAktuell = zaehlerAktuell;
    }
    var neueNr = true;
    if (insert && fzFahrrad.IsWeekly)
    {
      // Inserts new mileage.
      nr = 0;
      neueNr = false;
      if (voVorher != null /* vorherNr > 0 */ )
      {
        var woche = voVorher.Datum.AddDays(7);
        while (woche < dAktuell)
        {
          FzFahrradstandRep.Save(daten, daten.MandantNr, buid, woche, 0, zaehlerVorher, 0, 0, M0(FZ028),
            angelegtVon, angelegtAm, geaendertVon, geaendertAm);
          woche = woche.AddDays(7);
        }
        dAktuell = woche;
      }
    }
    var vo = FzFahrradstandRep.Get(daten, daten.MandantNr, buid, dAktuell, nr);
    if (vo == null)
    {
      // Gets next value.
      if (nr > 0 && neueNr)
        throw new MessageException(FZ029(nr));
      var l = FzFahrradstandRep.GetList(daten, buid, datege: dAktuell, datele: dAktuell, desc: true, max: 1);
      nr = 0;
      if (l.Any())
        nr = l.First().Nr + 1;
    }
    var e = FzFahrradstandRep.Save(daten, daten.MandantNr, buid, dAktuell, nr, zaehlerAktuell, periodeAktuell,
      average, besch, angelegtVon, angelegtAm, geaendertVon, geaendertAm);
    if (voNachher != null /* nachherNr > 0 */ && !(dAktuell == voNachher.Datum && nr == voNachher.Nr) &&
            Functions.CompDouble(periodeNachher, zaehlerNachher - zaehlerAktuell) != 0)
    {
      if (Functions.CompDouble(0, zaehlerNachher - zaehlerAktuell) > 0)
        throw new MessageException(FZ030);
      voNachher.Periode_km = zaehlerNachher - zaehlerAktuell;
      FzFahrradstandRep.Update(daten, voNachher);
    }
    r.Ergebnis = e;
    return r;
  }

  /// <summary>
  /// Repairs all mileages per bike, so that all odometer values are growing with time.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis RepairMileages(ServiceDaten daten)
  {
    var r = new ServiceErgebnis();
    var bikes = FzFahrradRep.GetList(daten, daten.MandantNr).OrderBy(a => a.Bezeichnung);
    foreach (var bike in bikes)
    {
      var z = 0m;
      var l = FzFahrradstandRep.GetList(daten, bike.Uid);
      foreach (var s in l)
      {
        if (z + s.Periode_km != s.Zaehler_km)
        {
          s.Zaehler_km = z + s.Periode_km;
          FzFahrradstandRep.Update(daten, s);
        }
        z = s.Zaehler_km;
      }
      SaveChanges(daten);
    }
    return r;
  }

  /// <summary>
  /// Deletes a mileage.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteMileage(ServiceDaten daten, FzFahrradstand e)
  {
    if (e == null)
      throw new MessageException(FZ029(0));
    var l = FzFahrradstandRep.GetList(daten, e.Fahrrad_Uid, datege: e.Datum, max: 10);
    foreach (var m in l)
    {
      if (m.Datum > e.Datum || m.Nr > e.Nr)
        throw new MessageException(FZ031);
    }
    FzFahrradstandRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of authors.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected name.</param>
  /// <returns>List of authors.</returns>
  public ServiceErgebnis<List<FzBuchautor>> GetAuthorList(ServiceDaten daten, string name)
  {
    var l = FzBuchautorRep.GetList(daten, name);
    return new ServiceErgebnis<List<FzBuchautor>>(l);
  }

  /// <summary>
  /// Gets a author.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Author or null.</returns>
  public ServiceErgebnis<FzBuchautor> GetAuthor(ServiceDaten daten, string uid)
  {
    var e = FzBuchautorRep.Get(daten, daten.MandantNr, uid);
    return new ServiceErgebnis<FzBuchautor>(e);
  }

  /// <summary>
  /// Saves an author.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="memo">Affected memo.</param>
  /// <returns>Created or changed author.</returns>
  public ServiceErgebnis<FzBuchautor> SaveAuthor(ServiceDaten daten, string uid, string name, string firstname, string memo)
  {
    var r = new ServiceErgebnis<FzBuchautor>();
    name = name.TrimNull();
    firstname = firstname.TrimNull();
    memo = memo.TrimNull();
    if (string.IsNullOrWhiteSpace(name))
      throw new MessageException(FZ032);
    r.Ergebnis = FzBuchautorRep.Save(daten, daten.MandantNr, uid, name, firstname, memo);
    return r;
  }

  /// <summary>
  /// Deletes an author.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteAuthor(ServiceDaten daten, FzBuchautor e)
  {
    var l = FzBuchRep.GetList(daten, e.Uid);
    if (l.Any())
      throw new MessageException(FZ039);
    FzBuchautorRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of series.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected name.</param>
  /// <returns>List of series.</returns>
  public ServiceErgebnis<List<FzBuchserie>> GetSeriesList(ServiceDaten daten, string name)
  {
    var l = FzBuchserieRep.GetList(daten, name);
    return new ServiceErgebnis<List<FzBuchserie>>(l);
  }

  /// <summary>
  /// Gets a series.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Series or null.</returns>
  public ServiceErgebnis<FzBuchserie> GetSeries(ServiceDaten daten, string uid)
  {
    var e = FzBuchserieRep.Get(daten, daten.MandantNr, uid);
    return new ServiceErgebnis<FzBuchserie>(e);
  }

  /// <summary>
  /// Saves a series.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="memo">Affected memo.</param>
  /// <returns>Created or changed series.</returns>
  public ServiceErgebnis<FzBuchserie> SaveSeries(ServiceDaten daten, string uid, string name, string memo)
  {
    var r = new ServiceErgebnis<FzBuchserie>();
    if (name != null && !name.ToLower().EndsWith(FZ034.ToLower(), StringComparison.CurrentCulture))
      name = name.TrimEnd();
    memo = memo.TrimNull();
    if (string.IsNullOrWhiteSpace(name))
      throw new MessageException(FZ033);
    r.Ergebnis = FzBuchserieRep.Save(daten, daten.MandantNr, uid, name, memo);
    return r;
  }

  /// <summary>
  /// Deletes a series.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteSeries(ServiceDaten daten, FzBuchserie e)
  {
    var l = FzBuchRep.GetList(daten, null, e.Uid);
    if (l.Any())
      throw new MessageException(FZ040);
    FzBuchserieRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of books.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auuid">Affected author ID.</param>
  /// <param name="seuid">Affected series ID.</param>
  /// <param name="bouid">Affected book ID.</param>
  /// <param name="name">Affected name.</param>
  /// <returns>List of books.</returns>
  public ServiceErgebnis<List<FzBuch>> GetBookList(ServiceDaten daten, string auuid,
      string seuid, string bouid, string name)
  {
    var max = string.IsNullOrWhiteSpace(auuid) && string.IsNullOrWhiteSpace(seuid)
      && string.IsNullOrEmpty(bouid) && !Functions.IsLike(name) ? 100 : 0;
    var l = FzBuchRep.GetList(daten, auuid, seuid, bouid, name, max: max);
    return new ServiceErgebnis<List<FzBuch>>(l);
  }

  /// <summary>
  /// Gets a book.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Book or null.</returns>
  public ServiceErgebnis<FzBuch> GetBook(ServiceDaten daten, string uid)
  {
    var e = FzBuchRep.GetList(daten, null, bouid: uid).FirstOrDefault();
    return new ServiceErgebnis<FzBuch>(e);
  }

  /// <summary>
  /// Saves a book.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="auuid">Affected author ID.</param>
  /// <param name="seuid">Affected series ID.</param>
  /// <param name="serialnr">Affected serial number.</param>
  /// <param name="title">Affected book title.</param>
  /// <param name="subtitle">Affected book subtitle.</param>
  /// <param name="pages">Affected number of pages.</param>
  /// <param name="lang">Affected language.</param>
  /// <param name="poss">Affected possession.</param>
  /// <param name="read">Affected reading date.</param>
  /// <param name="heard">Affected heard date.</param>
  /// <param name="memo">Affected memo.</param>
  /// <returns>Created or changed book.</returns>
  public ServiceErgebnis<FzBuch> SaveBook(ServiceDaten daten, string uid, string auuid, string seuid,
      int serialnr, string title, string subtitle, int pages, int lang, bool poss, DateTime? read,
      DateTime? heard, string memo)
  {
    var r = new ServiceErgebnis<FzBuch>();
    title = title.TrimNull();
    subtitle = subtitle.TrimNull();
    if (string.IsNullOrWhiteSpace(title))
      r.Errors.Add(Message.New(FZ041));
    if (string.IsNullOrWhiteSpace(auuid))
      r.Errors.Add(Message.New(FZ042));
    if (string.IsNullOrWhiteSpace(seuid))
      r.Errors.Add(Message.New(FZ043));
    if (!r.Ok)
      return r;
    if (lang < 0 || lang > 3)
      lang = 0;
    var seriennummer = serialnr;
    var seiten = pages;
    if (string.IsNullOrEmpty(uid) && !string.IsNullOrEmpty(seuid))
    {
      var keineSerie = false;
      var serie = FzBuchserieRep.Get(daten, daten.MandantNr, seuid);
      if (serie != null && serie.Name.ToLower().EndsWith(M0(FZ034).ToLower(), StringComparison.CurrentCulture))
      {
        keineSerie = true;
        seriennummer = 0;
      }
      if (!keineSerie && seriennummer > 0)
      {
        var bliste = FzBuchRep.GetList(daten, null, seuid, no: seriennummer);
        if (bliste.Any())
        {
          // Existing series number.
          seriennummer = 0;
        }
      }
      if (!keineSerie && (seriennummer <= 0 || seiten <= 0))
      {
        var bliste = FzBuchRep.GetList(daten, null, seuid, max: 1, descseries: true);
        if (bliste.Any())
        {
          if (seriennummer <= 0)
            seriennummer = bliste[0].Seriennummer + 1;
          if (seiten <= 0)
            seiten = bliste[0].Seiten;
        }
        else
        {
          if (seriennummer <= 0)
            seriennummer = 1;
          if (seiten <= 0)
            seiten = 1;
        }
      }
    }
    r.Ergebnis = FzBuchRep.Save(daten, daten.MandantNr, uid, auuid, seuid, seriennummer,
        title, subtitle, Math.Abs(seiten), lang, memo);
    FzBuchstatusRep.Save(daten, daten.MandantNr, r.Ergebnis.Uid, poss, read, heard);
    return r;
  }

  /// <summary>
  /// Deletes a book.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteBook(ServiceDaten daten, FzBuch e)
  {
    var st = FzBuchstatusRep.Get(daten, e.Mandant_Nr, e.Uid);
    if (st != null)
      FzBuchstatusRep.Delete(daten, st);
    FzBuchRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of memos.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="text">Affected text.</param>
  /// <returns>List of memos.</returns>
  public ServiceErgebnis<List<FzNotiz>> GetMemoList(ServiceDaten daten, string text = null)
  {
    var l = FzNotizRep.GetList(daten, text);
    return new ServiceErgebnis<List<FzNotiz>>(l);
  }

  /// <summary>
  /// Gets a memo.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Memo or null.</returns>
  public ServiceErgebnis<FzNotiz> GetMemo(ServiceDaten daten, string uid)
  {
    var e = FzNotizRep.Get(daten, daten.MandantNr, uid);
    return new ServiceErgebnis<FzNotiz>(e);
  }

  /// <summary>
  /// Saves a memo.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="topic">Affected topic.</param>
  /// <param name="notes">Affected notes.</param>
  /// <returns>Created or changed memo.</returns>
  public ServiceErgebnis<FzNotiz> SaveMemo(ServiceDaten daten, string uid, string topic, string notes)
  {
    var r = new ServiceErgebnis<FzNotiz>();
    topic = topic.TrimNull();
    if (string.IsNullOrWhiteSpace(topic))
      r.Errors.Add(Message.New(FZ035));
    if (!r.Ok)
      return r;
    r.Ergebnis = FzNotizRep.Save(daten, daten.MandantNr, uid, topic, notes);
    return r;
  }

  /// <summary>
  /// Deletes a memo.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteMemo(ServiceDaten daten, FzNotiz e)
  {
    FzNotizRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets statistics as string.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected statistics.</param>
  /// <param name="date">Affected date.</param>
  /// <returns>Statistics as string.</returns>
  public ServiceErgebnis<string> GetStatistics(ServiceDaten daten, int nr, DateTime date)
  {
    var r = new ServiceErgebnis<string>();
    var euro = true;
    if (nr == 1)
    {
      // Equity capital
      var r1 = BudgetService.GetProprietaryList(daten, date, 10);
      r.Get(r1);
      if (!r.Ok)
        return r;
      var hhBilanz = r1.Ergebnis;
      var sb = new StringBuilder();
      var db = 0m;
      decimal? alt = null;
      if (hhBilanz != null)
      {
        foreach (var e in hhBilanz)
        {
          db = euro ? e.EBetrag : e.Betrag;
          if (alt.HasValue)
            sb.Append(FZ002(alt.Value - db));
          if (sb.Length > 0)
            sb.Append(Constants.CRLF);
          sb.Append(FZ001(M0(HH001), e.Geaendert_Am, db));
          alt = db;
        }
      }
      r.Ergebnis = sb.ToString();
    }
    else if (nr == 2)
    {
      // Book statistics
      var prSerie = "";
      var benutzer = BenutzerRep.Get(daten, daten.MandantNr, daten.BenutzerId) ?? throw new MessageException(FZ036(daten.BenutzerId));
      var geburt = benutzer.Geburt;
      var wk = benutzer.Benutzer_ID.ToLower() == "wolfgang";
      var sb = new StringBuilder();
      var anzahlTage = 0;
      var jetzt = date.AddDays(1);
      if (wk)
      {
        var sliste = FzBuchserieRep.GetList(daten, "%Perry Rhodan");
        if (sliste.Any())
          prSerie = sliste[0].Uid;
      }
      sb.Append(FZ003(benutzer.Benutzer_ID.ToFirstUpper()));
      if (geburt.HasValue)
        anzahlTage = (int)(jetzt - geburt.Value).TotalDays;
      if (anzahlTage > 0)
        sb.Append(Constants.CRLF).Append(FZ004(anzahlTage));
      var anzahl = FzBuchRep.Count(daten, -1, null, prSerie, jetzt);
      sb.Append(Constants.CRLF).Append(FZ005(anzahl));
      var anzahl2 = FzBuchRep.Count(daten, -1, null, prSerie, jetzt, jetzt);
      sb.Append(Constants.CRLF).Append(FZ006(anzahl2));
      if (anzahl != 0)
        sb.Append(FZ008((decimal)anzahl2 / anzahl * 100));
      anzahl2 = FzBuchRep.Count(daten, -1, null, prSerie, jetzt, null, jetzt);
      sb.Append(Constants.CRLF).Append(FZ007(anzahl2));
      if (anzahl != 0)
        sb.Append(FZ008((decimal)anzahl2 / anzahl * 100));
      anzahl = FzBuchRep.Count(daten, 2, null, null, jetzt, null, null); // Englisch
      sb.Append(Constants.CRLF).Append(FZ009(anzahl));
      if (wk && !string.IsNullOrEmpty(prSerie))
      {
        anzahl = FzBuchRep.Count(daten, -1, prSerie, null, jetzt);
        sb.Append(Constants.CRLF).Append(FZ010(anzahl));
        anzahl2 = FzBuchRep.Count(daten, -1, prSerie, null, jetzt, jetzt);
        sb.Append(Constants.CRLF).Append(FZ011(anzahl2));
        if (anzahl != 0)
          sb.Append(FZ008((decimal)anzahl2 / anzahl * 100));
      }
      anzahl2 = FzBuchRep.Count(daten, -1, null, prSerie, jetzt, jetzt, pages: true);
      sb.Append(Constants.CRLF).Append(FZ012(anzahl2));
      if (wk && !string.IsNullOrEmpty(prSerie))
      {
        anzahl2 = FzBuchRep.Count(daten, -1, prSerie, null, jetzt, jetzt, pages: true);
        sb.Append(Constants.CRLF).Append(FZ013(anzahl2));
      }
      anzahl2 = FzBuchRep.Count(daten, -1, null, null, jetzt, jetzt, pages: true);
      sb.Append(Constants.CRLF).Append(FZ014(anzahl2));
      if (anzahlTage > 0)
        sb.Append(Constants.CRLF).Append(FZ015((decimal)anzahl2 / anzahlTage));
      r.Ergebnis = sb.ToString();
    }
    else if (nr == 3)
    {
      // Bike statistics
      var summe = 0M;
      var summeJahr = 0M;
      var sumyear1 = 0M;
      const int laenge = 18;
      DateTime? anfangMin = null;
      var anzahlTageMax = 0;
      const decimal jahresTage = 365.25M;
      var sb = new StringBuilder();
      var aktJahr = date;
      if (BikeYearFixed)
        aktJahr = aktJahr.AddDays(1 - date.DayOfYear); // 01.01.
      else
        aktJahr = aktJahr.AddYears(-1);
      var fliste = FzFahrradRep.GetList(daten, daten.MandantNr).OrderBy(a => a.Bezeichnung).ToList();
      foreach (var vo in fliste)
      {
        var jetzt1 = date;
        if (vo.Typ == (int)BikeTypeEnum.Weekly)
          jetzt1 = Functions.Sunday(jetzt1);
        var km = FzFahrradstandRep.Count(daten, vo.Uid, null, jetzt1);
        var kmJahr = FzFahrradstandRep.Count(daten, vo.Uid, aktJahr, jetzt1);
        var kmyear1 = FzFahrradstandRep.Count(daten, vo.Uid, aktJahr.AddYears(-1), aktJahr.AddDays(-1));
        var anzahlTage = 0;
        var liste = FzFahrradstandRep.GetList(daten, vo.Uid, datele: jetzt1, desc: false, max: 1);
        DateTime? anfang = null;
        if (liste.Any())
        {
          anfang = liste[0].Datum;
          if (!anfangMin.HasValue || anfangMin.Value > anfang)
            anfangMin = anfang;
          anzahlTage = (int)(date - anfang.Value).TotalDays;
          if (anzahlTage > anzahlTageMax)
            anzahlTageMax = anzahlTage;
        }
        summe += km;
        summeJahr += kmJahr;
        sumyear1 += kmyear1;
        if (sb.Length > 0)
          sb.Append(Constants.CRLF);
        sb.Append(FZ016(Functions.Cut((vo.Bezeichnung + ": ").PadRight(laenge, ' '), laenge), km, kmJahr, kmyear1));
        if (anzahlTage > 0)
        {
          sb.Append(Constants.CRLF).Append(FZ017(Functions.Cut((" " + Functions.ToString(anfang) + ": ").PadRight(laenge, ' '), laenge),
            km / anzahlTage, km / anzahlTage * jahresTage));
        }
      }
      if (anzahlTageMax > 0)
      {
        sb.Append(Constants.CRLF).Append(FZ016(Functions.Cut((M0(FZ018) + ": ").PadRight(laenge, ' '), laenge), summe, summeJahr, sumyear1));
        sb.Append(Constants.CRLF).Append(FZ017(Functions.Cut((" " + Functions.ToString(anfangMin) + ": ").PadRight(laenge, ' '), laenge),
          summe / anzahlTageMax, summe / anzahlTageMax * jahresTage));
      }
      r.Ergebnis = sb.ToString();
    }
    return r;
  }
}
