// <copyright file="AddressService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Base;
using CSBP.Services.Reports;
using static CSBP.Base.Functions;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>
/// Implementation of address service.
/// </summary>
public class AddressService : ServiceBase, IAddressService
{
  /// <summary>
  /// Gets list of birthdays.
  /// </summary>
  /// <returns>List of birthdays.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="days">Affected number of days.</param>
  public ServiceErgebnis<List<string>> GetBirthdayList(ServiceDaten daten, DateTime date, int days)
  {
    var v = new List<string>();
    var r = new ServiceErgebnis<List<string>>(v);
    var from = date.AddDays(-Math.Abs(days));
    var to = date.AddDays(Math.Abs(days));
    var j = date.Year;
    var f = (from.Month * 100) + from.Day;
    v.Add(AD001(from, to));
    var i = j != from.Year ? 1 : j != to.Year ? 2 : 0;
    var liste = AdPersonRep.GetList(daten, from, to);
    foreach (var vo in liste)
    {
      var d = vo.GeburtK;
      var y = vo.Geburt.Value.Year;
      int j1;
      if (i == 0)
      {
        j1 = j - y;
      }
      else
      {
        if (f <= d)
        {
          j1 = from.Year - y;
        }
        else
        {
          j1 = to.Year - y;
        }
      }
      v.Add(AD002(vo.Geburt.Value, vo.Name, j1));
    }
    return r;
  }

  /// <summary>
  /// Gets list of sites with persons and addresses.
  /// </summary>
  /// <returns>List of sites.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="actual">Only actual persons or not.</param>
  /// <param name="search">Affected search string.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="puid">Affected person ID.</param>
  /// <param name="suid">Affected site ID.</param>
  public ServiceErgebnis<List<AdSitz>> GetPersonList(ServiceDaten daten, bool actual, string search,
    string name, string firstname, string puid, string suid)
  {
    var l = AdSitzRep.GetList(daten, actual, search, name, firstname, puid, suid);
    var r = new ServiceErgebnis<List<AdSitz>>(l);
    return r;
  }

  /// <summary>
  /// Gets a site with person and address.
  /// </summary>
  /// <returns>A site or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="suid">Affected site ID.</param>
  public ServiceErgebnis<AdSitz> GetSite(ServiceDaten daten, string suid)
  {
    var l = AdSitzRep.GetList(daten, false, suid: suid);
    var r = new ServiceErgebnis<AdSitz>(l.FirstOrDefault());
    return r;
  }

  /// <summary>
  /// Saves a site with person and address.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of person.</param>
  /// <param name="gender">Affected gender.</param>
  /// <param name="geburt">Affected birthday.</param>
  /// <param name="name1">Affected last name.</param>
  /// <param name="name2">Affected name part two.</param>
  /// <param name="predicate">Affected predicate.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="title">Affected title.</param>
  /// <param name="personstate">Affected state of person.</param>
  /// <param name="suid">Affected site ID.</param>
  /// <param name="name">Affected site name.</param>
  /// <param name="phone">Affected phone number.</param>
  /// <param name="fax">Affected fax number.</param>
  /// <param name="mobile">Affected mobile number.</param>
  /// <param name="email">Affected email address.</param>
  /// <param name="homepage">Affected homepage.</param>
  /// <param name="pobox">Affected PO box.</param>
  /// <param name="memo">Affected memo.</param>
  /// <param name="sitestate">Affected site state.</param>
  /// <param name="auid">Affected address ID.</param>
  /// <param name="state">Affected state.</param>
  /// <param name="postalcode">Affected postal code.</param>
  /// <param name="town">Affected town.</param>
  /// <param name="street">Affected street.</param>
  /// <param name="no">Affected street number.</param>
  /// <returns>Created or changed site.</returns>
  public ServiceErgebnis<AdSitz> SaveSite(ServiceDaten daten, string uid, string gender,
      DateTime? geburt, string name1, string name2, string predicate, string firstname,
      string title, int personstate, string suid, string name, string phone,
      string fax, string mobile, string email, string homepage, string pobox, string memo,
      int sitestate, string auid, string state, string postalcode, string town, string street,
      string no)
  {
    var r = new ServiceErgebnis<AdSitz>(SaveSiteIntern(daten, uid, gender, geburt, name1, name2,
        predicate, firstname, title, personstate, null, null, null, null,
        suid, name, phone, fax, mobile, email, homepage, pobox, memo, sitestate, null, null, null, null,
        auid, state, postalcode, town, street, no, null, null, null, null));
    return r;
  }

  /// <summary>
  /// Deletes a site, an address and a person.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteSite(ServiceDaten daten, AdSitz e)
  {
    var r = new ServiceErgebnis();
    if (e == null || string.IsNullOrEmpty(e.Uid))
      return r;
    var s = AdSitzRep.GetList(daten, false, puid: e.Person_Uid, suid: e.Uid).FirstOrDefault();
    if (!string.IsNullOrEmpty(s.Adresse_Uid))
    {
      var list = AdSitzRep.GetList(daten, false, auid: s.Adresse_Uid);
      if (list.Count <= 1)
      {
        var a = AdAdresseRep.Get(daten, daten.MandantNr, s.Adresse_Uid);
        if (a != null)
          AdAdresseRep.Delete(daten, a);
      }
    }
    if (s != null)
      AdSitzRep.Delete(daten, s);
    var plist = AdSitzRep.GetList(daten, false, puid: e.Person_Uid).Where(a => a.Uid != e.Uid).ToList();
    if (plist.Count <= 0)
    {
      var p = AdPersonRep.Get(daten, daten.MandantNr, s.Person_Uid);
      if (p != null)
        AdPersonRep.Delete(daten, p);
    }
    return r;
  }

  /// <summary>
  /// Gets the number of usages of an address.
  /// </summary>
  /// <returns>Number of usages of an address.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Affected address ID.</param>
  public ServiceErgebnis<int> GetAddressCount(ServiceDaten daten, string auid)
  {
    var r = new ServiceErgebnis<int>(string.IsNullOrEmpty(auid) ? 0
        : AdSitzRep.GetAddressCount(daten, auid));
    return r;
  }

  /// <summary>
  /// Makes the site first.
  /// </summary>
  /// <returns>Possible errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="siteuid">Affected site ID.</param>
  public ServiceErgebnis MakeSiteFirst(ServiceDaten daten, string siteuid)
  {
    var r = new ServiceErgebnis();
    var site = AdSitzRep.Get(daten, siteuid);
    if (site != null)
    {
      var o = 1;
      var l = AdSitzRep.GetList(daten, false, puid: site.Person_Uid);
      foreach (var s in l)
      {
        var order = o;
        if (s.Uid == siteuid)
          order = 1;
        else
        {
          order++;
          o++;
        }
        if (s.Reihenfolge != order)
        {
          AdSitzRep.Delete(daten, s);
          AdSitzRep.Save(daten, s.Mandant_Nr, s.Person_Uid, order, s.Uid, s.Typ, s.Name,
            s.Adresse_Uid, s.Telefon, s.Fax, s.Mobil, s.Email, s.Homepage, s.Postfach,
            s.Bemerkung, s.Sitz_Status, s.Angelegt_Von, s.Angelegt_Am, daten.BenutzerId,
            daten.Jetzt);
        }
      }
    }
    return r;
  }

  /// <summary>
  /// Gets list of addresses.
  /// </summary>
  /// <returns>List of addresses.</returns>
  /// <param name="daten">Service data for database access.</param>
  public ServiceErgebnis<List<AdAdresse>> GetAddressList(ServiceDaten daten)
  {
    var r = new ServiceErgebnis<List<AdAdresse>>(AdAdresseRep.GetList(daten, daten.MandantNr));
    return r;
  }

  /// <summary>
  /// Gets an address.
  /// </summary>
  /// <returns>An address or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Affected address ID.</param>
  public ServiceErgebnis<AdAdresse> GetAddress(ServiceDaten daten, string auid)
  {
    var r = new ServiceErgebnis<AdAdresse>(AdAdresseRep.Get(daten, daten.MandantNr, auid));
    return r;
  }

  /// <summary>
  /// Exports list of addresses.
  /// </summary>
  /// <returns>List of addresses.</returns>
  /// <param name="daten">Service data for database access.</param>
  public ServiceErgebnis<List<string>> ExportAddressList(ServiceDaten daten)
  {
    var sites = AdSitzRep.GetList(daten, false);
    var list = FillAddressList(sites);
    var r = new ServiceErgebnis<List<string>>(list);
    return r;
  }

  /// <summary>
  /// Imports list of addresses.
  /// </summary>
  /// <returns>Message of import.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="list">List of addresses.</param>
  /// <param name="delete">Delete all persons, sites and addresses or not.</param>
  public ServiceErgebnis<string> ImportAddressList(ServiceDaten daten, List<string> list, bool delete)
  {
    var r = new ServiceErgebnis<string>();
    if (delete)
    {
      var plist = AdPersonRep.GetList(daten, daten.MandantNr);
      foreach (var p in plist)
        AdPersonRep.Delete(daten, p);
      SaveChanges(daten);
      var slist = AdSitzRep.GetList(daten, daten.MandantNr);
      foreach (var s in slist)
        AdSitzRep.Delete(daten, s);
      SaveChanges(daten);
      var alist = AdAdresseRep.GetList(daten, daten.MandantNr);
      foreach (var a in alist)
        AdAdresseRep.Delete(daten, a);
      SaveChanges(daten);
      return r;
    }
    if (list == null)
      throw new ArgumentException(null, nameof(list));
    var pcount = 0;
    var perror = 0;
    var scount = 0;
    var serror = 0;
    var acount = 0;
    var aerror = 0;
    var check = false;
    var row = 0;
    try
    {
      foreach (var line in list)
      {
        row++;
        //// if (row == 18)
        ////    MachNichts();
        var f = DecodeCSV(line);
        if (f == null)
          continue;
        if (check)
        {
          var puid = FromStr(f[0]);
          var suid = FromStr(f[16]);
          var auid = FromStr(f[32]);
          var s = new AdSitz
          {
            Person = new AdPerson
            {
              Uid = FromStr(f[0]),
              Geschlecht = FromStr(f[2]),
              Geburt = ToDateTime(f[3]),
              Name1 = FromStr(f[5]),
              Name2 = FromStr(f[6]),
              Praedikat = FromStr(f[7]),
              Vorname = FromStr(f[8]),
              Titel = FromStr(f[9]),
              Person_Status = ToInt32(f[10]),
              Angelegt_Von = FromStr(f[11]),
              Angelegt_Am = ToDateTime(f[12]),
              Geaendert_Von = FromStr(f[13]),
              Geaendert_Am = ToDateTime(f[14]),
            },
            Person_Uid = FromStr(f[15]),
            Uid = FromStr(f[16]),
            Name = FromStr(f[18]),
            Adresse_Uid = FromStr(f[19]),
            Telefon = FromStr(f[20]),
            Fax = FromStr(f[21]),
            Mobil = FromStr(f[22]),
            Email = FromStr(f[23]),
            Homepage = FromStr(f[24]),
            Postfach = FromStr(f[25]),
            Bemerkung = FromStr(f[26]),
            Sitz_Status = ToInt32(f[27]),
            Angelegt_Von = FromStr(f[28]),
            Angelegt_Am = ToDateTime(f[29]),
            Geaendert_Von = FromStr(f[30]),
            Geaendert_Am = ToDateTime(f[31]),
            Address = new AdAdresse
            {
              Uid = FromStr(f[32]),
              Staat = FromStr(f[33]),
              Plz = FromStr(f[34]),
              Ort = FromStr(f[35]),
              Strasse = FromStr(f[36]),
              HausNr = FromStr(f[37]),
              Angelegt_Von = FromStr(f[38]),
              Angelegt_Am = ToDateTime(f[39]),
              Geaendert_Von = FromStr(f[40]),
              Geaendert_Am = ToDateTime(f[41]),
            },
            Reihenfolge = ToInt32(f[42]),
          };
          if (!IsPersonEmpty(s.Person.Geschlecht, s.Person.Geburt, s.Person.Name1,
              s.Person.Name2, s.Person.Praedikat, s.Person.Vorname, s.Person.Titel))
          {
            s.Person.Uid = AdPersonRep.GetUid(daten, s.Person.Geschlecht, s.Person.Geburt,
                s.Person.Name1, s.Person.Name2, s.Person.Praedikat, s.Person.Vorname,
                s.Person.Titel, s.Person.Angelegt_Von, s.Person.Angelegt_Am);
            if (!IsAddressEmpty(s.Address.Staat, s.Address.Plz, s.Address.Ort, s.Address.Strasse, s.Address.HausNr))
              s.Address.Uid = AdAdresseRep.GetUid(daten, s.Address.Staat, s.Address.Plz, s.Address.Ort,
                  s.Address.Strasse, s.Address.HausNr, s.Address.Angelegt_Von, s.Address.Angelegt_Am,
                  s.Address.Geaendert_Von, s.Address.Geaendert_Am);
            if (!string.IsNullOrEmpty(s.Person.Uid))
              s.Uid = AdSitzRep.GetUid(daten, s.Person.Uid, s.Name, s.Telefon, s.Fax, s.Mobil,
                  s.Email, s.Homepage, s.Postfach, s.Angelegt_Von, s.Angelegt_Am);
            var site = SaveSiteIntern(daten, string.IsNullOrEmpty(s.Person.Uid) ? puid : s.Person.Uid,
                s.Person.Geschlecht, s.Person.Geburt, s.Person.Name1, s.Person.Name2, s.Person.Praedikat,
                s.Person.Vorname, s.Person.Titel, s.Person.Person_Status, s.Person.Angelegt_Von,
                s.Person.Angelegt_Am, s.Person.Geaendert_Von, s.Person.Geaendert_Am,
                string.IsNullOrEmpty(s.Uid) ? suid : s.Uid, s.Name, s.Telefon, s.Fax, s.Mobil,
                s.Email, s.Homepage, s.Postfach, s.Bemerkung, s.Sitz_Status, s.Angelegt_Von,
                s.Angelegt_Am, s.Geaendert_Von, s.Geaendert_Am,
                string.IsNullOrEmpty(s.Address.Uid) ? auid : s.Address.Uid, s.Address.Staat, s.Address.Plz, s.Address.Ort, s.Address.Strasse,
                s.Address.HausNr, s.Address.Angelegt_Von, s.Address.Angelegt_Am, s.Address.Geaendert_Von, s.Address.Geaendert_Am);
            if (string.IsNullOrEmpty(s.Person.Uid))
              pcount++;
            if (string.IsNullOrEmpty(s.Person.Uid) || string.IsNullOrEmpty(s.Uid))
              scount++;
            if (!IsAddressEmpty(s.Address.Staat, s.Address.Plz, s.Address.Ort, s.Address.Strasse, s.Address.HausNr)
                && string.IsNullOrEmpty(s.Address.Uid) && !string.IsNullOrEmpty(site.Adresse_Uid))
              acount++;
            SaveChanges(daten);
          }
        }
        else
        {
          var columns = GetAddressColumns();
          if (columns.Count - 1 <= f.Count && f.Count <= columns.Count)
          {
            // Spalte Reihenfolge optional
            var j = 0;
            var x = true;
            for (; j < f.Count && x; j++)
            {
              if (f[j] != columns[j])
              {
                x = false;
              }
            }
            if (j >= columns.Count - 1)
              check = true;
          }
          if (!check)
            throw new MessageException(AD005);
        }
      }
    }
    finally
    {
      MachNichts(row);
    }
    r.Ergebnis = AD010(pcount, perror, scount, serror, acount, aerror);
    return r;
  }

  /// <summary>
  /// Gets an address report as html document in bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Address report as html document in bytes.</returns>
  public ServiceErgebnis<byte[]> GetAddressReport(ServiceDaten daten)
  {
    var r = new ServiceErgebnis<byte[]>();
    var ueberschrift = AD003(daten.Jetzt);
    var l = AdSitzRep.GetList(daten, true);
    var rp = new AddressReport
    {
      Caption = ueberschrift,
      Sites = l,
    };
    r.Ergebnis = rp.Generate();
    return r;
  }

  /// <summary>
  /// Saves a site with person and address.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of person.</param>
  /// <param name="gender">Affected gender.</param>
  /// <param name="geburt">Affected birthday.</param>
  /// <param name="name1">Affected last name.</param>
  /// <param name="name2">Affected name part two.</param>
  /// <param name="predicate">Affected predicate.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="title">Affected title.</param>
  /// <param name="personstate">Affected state of person.</param>
  /// <param name="pcreatedby">Affected person creation user.</param>
  /// <param name="pcreatedat">Affected person creation time.</param>
  /// <param name="pchangedby">Affected person change user.</param>
  /// <param name="pchangedat">Affected person change time.</param>
  /// <param name="suid">Affected site ID.</param>
  /// <param name="name">Affected site name.</param>
  /// <param name="phone">Affected phone number.</param>
  /// <param name="fax">Affected fax number.</param>
  /// <param name="mobile">Affected mobile number.</param>
  /// <param name="email">Affected email address.</param>
  /// <param name="homepage">Affected homepage.</param>
  /// <param name="pobox">Affected PO box.</param>
  /// <param name="memo">Affected memo.</param>
  /// <param name="sitestate">Affected site state.</param>
  /// <param name="screatedby">Affected site creation user.</param>
  /// <param name="screatedat">Affected site creation time.</param>
  /// <param name="schangedby">Affected site change user.</param>
  /// <param name="schangedat">Affected site change time.</param>
  /// <param name="auid">Affected address ID.</param>
  /// <param name="state">Affected state.</param>
  /// <param name="postalcode">Affected postal code.</param>
  /// <param name="town">Affected town.</param>
  /// <param name="street">Affected street.</param>
  /// <param name="no">Affected street number.</param>
  /// <param name="acreatedby">Affected address creation user.</param>
  /// <param name="acreatedat">Affected address creation time.</param>
  /// <param name="achangedby">Affected address change user.</param>
  /// <param name="achangedat">Affected address change time.</param>
  /// <returns>Created or changed site.</returns>
  private static AdSitz SaveSiteIntern(ServiceDaten daten, string uid, string gender,
      DateTime? geburt, string name1, string name2, string predicate, string firstname,
      string title, int personstate,
      string pcreatedby, DateTime? pcreatedat, string pchangedby, DateTime? pchangedat,
      string suid, string name, string phone, string fax, string mobile, string email, string homepage, string pobox,
      string memo, int sitestate,
      string screatedby, DateTime? screatedat, string schangedby, DateTime? schangedat,
      string auid, string state, string postalcode, string town, string street, string no,
      string acreatedby, DateTime? acreatedat, string achangedby, DateTime? achangedat)
  {
    name1 = name1.TrimNull();
    name2 = name2.TrimNull();
    predicate = predicate.TrimNull();
    firstname = firstname.TrimNull();
    title = title.TrimNull();
    suid = suid.TrimNull();
    name = name.TrimNull() ?? M0(AD014);
    phone = phone.TrimNull();
    fax = fax.TrimNull();
    mobile = mobile.TrimNull();
    email = email.TrimNull();
    homepage = homepage.TrimNull();
    pobox = pobox.TrimNull();
    memo = memo.TrimNull();
    auid = auid.TrimNull();
    state = state.TrimNull();
    postalcode = postalcode.TrimNull();
    town = town.TrimNull() ?? "";
    street = street.TrimNull();
    no = no.TrimNull();
    if (gender != "M" && gender != "F")
      gender = "N";
    var geburtk = geburt.HasValue ? (geburt.Value.Month * 100) + geburt.Value.Day : 0;
    if (string.IsNullOrWhiteSpace(name1))
    {
      if (string.IsNullOrEmpty(uid))
        uid = Functions.GetUid();
      name1 = AD013(uid);
    }
    AdAdresse a = null;
    var p = AdPersonRep.Save(daten, daten.MandantNr, uid, 0, gender, geburt, geburtk, 0, 0,
      name1, name2, predicate, firstname, title, personstate, pcreatedby, pcreatedat, pchangedby, pchangedat);
    uid = p?.Uid;
    if (!string.IsNullOrEmpty(auid) || !IsAddressEmpty(state, postalcode, town, street, no))
    {
      // Leere Adresse wird nicht angelegt.
      a = AdAdresseRep.Save(daten, daten.MandantNr, auid, state, postalcode, town, street, no,
        acreatedby, acreatedat, achangedby, achangedat);
      auid = a?.Uid;
    }
    var order = 1; // beginnt mit 1
    var list = AdSitzRep.GetList(daten, false, puid: uid, suid: suid);
    if (string.IsNullOrEmpty(suid))
    {
      if (list.Any())
        order = list.Max(x => x.Reihenfolge) + 1; // letzter Sitz
    }
    else if (list.Any())
      order = list.First().Reihenfolge; // bestehende Reihenfolge
    else
    {
      // Import eines bestehenden Sitzes
      list = AdSitzRep.GetList(daten, false, puid: uid);
      if (list.Any())
        order = list.Max(x => x.Reihenfolge) + 1; // letzter Sitz
    }
    var s = AdSitzRep.Save(daten, daten.MandantNr, uid, order, suid, 0, name, auid, phone, fax, mobile,
      email, homepage, pobox, memo, sitestate, screatedby, screatedat, schangedby, schangedat);
    s.Address = a;
    s.Person = p;
    return s;
  }

  /// <summary>
  /// Checks whether person is empty.
  /// </summary>
  /// <param name="gender">Affected gender.</param>
  /// <param name="birthday">Affected birthday.</param>
  /// <param name="name1">Affected last name.</param>
  /// <param name="name2">Affected  name 2.</param>
  /// <param name="predicate">Affected  predicate.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="title">Affected title.</param>
  /// <returns>Person is empty or not.</returns>
  private static bool IsPersonEmpty(string gender, DateTime? birthday, string name1, string name2,
      string predicate, string firstname, string title)
  {
    if (string.IsNullOrWhiteSpace(gender) && !birthday.HasValue && string.IsNullOrWhiteSpace(name1)
        && string.IsNullOrWhiteSpace(name2) && string.IsNullOrWhiteSpace(predicate)
        && string.IsNullOrWhiteSpace(firstname) && string.IsNullOrWhiteSpace(title))
    {
      return true;
    }
    return false;
  }

  /// <summary>
  /// Checks whetzer the address is empty.
  /// </summary>
  /// <param name="state">Affected state.</param>
  /// <param name="postalcode">Affected postcode.</param>
  /// <param name="town">Affected town.</param>
  /// <param name="street">Affected street.</param>
  /// <param name="no">Affected house number.</param>
  /// <returns>Address is empty or not.</returns>
  private static bool IsAddressEmpty(string state, string postalcode, string town, string street, string no)
  {
    if ((string.IsNullOrWhiteSpace(state) || state == "D") && string.IsNullOrWhiteSpace(postalcode)
        && string.IsNullOrWhiteSpace(town) && string.IsNullOrWhiteSpace(street)
        && string.IsNullOrWhiteSpace(no))
    {
      return true;
    }
    return false;
  }

  /// <summary>
  /// Gets list of address columns.
  /// </summary>
  /// <returns>list of address columns.</returns>
  private static List<string> GetAddressColumns()
  {
    var columns = new List<string>
    {
      "Uid", "Typ", "Geschlecht", "Geburt", "Anrede", "Name1",
      "Name2", "Praedikat", "Vorname", "Titel", "PersonStatus", "AngelegtVon", "AngelegtAm",
      "GeaendertVon", "GeaendertAm", "PersonUid", "SiUid", "SiTyp", "Name", "AdresseUid",
      "Telefon", "Fax", "Mobil", "Email", "Homepage", "Postfach", "Bemerkung", "SitzStatus",
      "SiAngelegtVon", "SiAngelegtAm", "SiGeaendertVon", "SiGeaendertAm", "AdUid", "Staat",
      "Plz", "Ort", "Strasse", "Hausnr", "AdAngelegtVon", "AdAngelegtAm", "AdGeaendertVon",
      "AdGeaendertAm", "Reihenfolge",
    };
    return columns;
  }

  /// <summary>
  /// Fills address list.
  /// </summary>
  /// <param name="sites">List of sites.</param>
  /// <returns>List of string as lines.</returns>
  private static List<string> FillAddressList(List<AdSitz> sites)
  {
    var list = new List<string>();
    var columns = GetAddressColumns();
    list.Add(EncodeCSV(columns));
    if (sites == null)
      return list;
    foreach (var s in sites)
    {
      var p = s.Person ?? new AdPerson();
      var a = s.Address ?? new AdAdresse();
      var l = new List<string>
      {
        ToStr(p.Uid), ToStr(p.Typ), ToStr(p.Geschlecht), ToStr(p.Geburt), ToStr(p.Anrede), ToStr(p.Name1),
        ToStr(p.Name2), ToStr(p.Praedikat), ToStr(p.Vorname), ToStr(p.Titel), ToStr(p.Person_Status), ToStr(p.Angelegt_Von), ToStr(p.Angelegt_Am),
        ToStr(p.Geaendert_Von), ToStr(p.Geaendert_Am), ToStr(s.Person_Uid), ToStr(s.Uid), ToStr(s.Typ), ToStr(s.Name), ToStr(s.Adresse_Uid),
        ToStr(s.Telefon), ToStr(s.Fax), ToStr(s.Mobil), ToStr(s.Email), ToStr(s.Homepage), ToStr(s.Postfach), ToStr(s.Bemerkung), ToStr(s.Sitz_Status),
        ToStr(s.Angelegt_Von), ToStr(s.Angelegt_Am), ToStr(s.Geaendert_Von), ToStr(s.Geaendert_Am), ToStr(a.Uid), ToStr(a.Staat),
        ToStr(a.Plz), ToStr(a.Ort), ToStr(a.Strasse), ToStr(a.HausNr), ToStr(a.Angelegt_Von), ToStr(a.Angelegt_Am), ToStr(a.Geaendert_Von),
        ToStr(a.Geaendert_Am), ToStr(s.Reihenfolge),
      };
      list.Add(EncodeCSV(l));
    }
    return list;
  }
}
