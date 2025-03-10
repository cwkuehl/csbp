// <copyright file="AdSitzRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table AD_Sitz.
/// </summary>
public partial class AdSitzRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list with sites with persons and addresses.
  /// </summary>
  /// <returns>List with sites.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="actual">Only actual persons or not.</param>
  /// <param name="search">Affected search string.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="puid">Affected person ID.</param>
  /// <param name="suid">Affected site ID.</param>
  /// <param name="auid">Affected address ID.</param>
  public List<AdSitz> GetList(ServiceDaten daten, bool actual, string search = null, string name = null,
    string firstname = null, string puid = null, string suid = null, string auid = null)
  {
    var db = GetDb(daten);
    var sl = db.AD_Sitz.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(suid))
      sl = sl.Where(a => a.Uid == suid);
    if (!string.IsNullOrEmpty(auid))
      sl = sl.Where(a => a.Adresse_Uid == auid);
    if (actual)
      sl = sl.Where(a => a.Sitz_Status == 0);
    var pl = db.AD_Person.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(puid))
      pl = pl.Where(a => a.Uid == puid);
    if (CsbpBase.IsLike(name))
      pl = pl.Where(a => EF.Functions.Like(a.Name1, name) || EF.Functions.Like(a.Name2, name));
    if (CsbpBase.IsLike(firstname))
      pl = pl.Where(a => EF.Functions.Like(a.Vorname, firstname));
    if (actual)
      pl = pl.Where(a => a.Person_Status == 0);
    var al = db.AD_Adresse.Where(a => a.Mandant_Nr == daten.MandantNr);
    List<AdSitz> l;
    if (Functions.MachNichts() == 0)
    {
      // !string.IsNullOrEmpty(suid)
      // Es muss immer einen Sitz zu der Person geben. Adresse ist optional.
      l = pl.Join(sl, a => new { a.Mandant_Nr, a.Uid },
        b => new { b.Mandant_Nr, Uid = b.Person_Uid }, (a, b) => new { person = a, site = b })
        .GroupJoin(al, a => new { a.site.Mandant_Nr, Uid = a.site.Adresse_Uid },
        b => new { b.Mandant_Nr, b.Uid }, (a, b) => new { a.person, a.site, address = b })
        .SelectMany(ab => ab.address.DefaultIfEmpty(), (a, b) => new { a.person, a.site, address = b })
        .ToList()
        .Select(a =>
        {
          var site = a.site;
          site.Person = a.person;
          site.Address = a.address;
          return site;
        }).ToList();
    }
    else
    {
      l = pl.GroupJoin(sl, a => new { a.Mandant_Nr, a.Uid },
        b => new { b.Mandant_Nr, Uid = b.Person_Uid }, (a, b) => new { person = a, site = b })
        .SelectMany(ab => ab.site.DefaultIfEmpty(), (a, b) => new { a.person, site = b })
        .GroupJoin(al, a => new { a.site.Mandant_Nr, Uid = a.site.Adresse_Uid },
        b => new { b.Mandant_Nr, b.Uid }, (a, b) => new { a.person, a.site, address = b })
        .SelectMany(ab => ab.address.DefaultIfEmpty(), (a, b) => new { a.person, a.site, address = b })
        .ToList()
        .Select(a =>
        {
          var site = a.site ?? new AdSitz();
          site.Person = a.person;
          site.Address = a.address;
          return site;
        }).ToList();
    }
    if (CsbpBase.IsLike(search))
    {
      // l = l.Where(a => Like(a.Person.Name1, search) || Like(a.Person.Name2, search)
      //   || Like(a.Person.Praedikat, search) || Like(a.Person.Vorname, search)
      //   || Like(a.Person.Titel, search)
      //   || Like(a.Name, search) || Like(a.Telefon, search)
      //   || Like(a.Fax, search) || Like(a.Mobil, search)
      //   || Like(a.Email, search) || Like(a.Homepage, search)
      //   || Like(a.Postfach, search) || Like(a.Bemerkung, search)
      //   || (a.Address != null && (Like(a.Address.Staat, search) || Like(a.Address.Plz, search)
      //   || Like(a.Address.Ort, search) || Like(a.Address.Strasse, search)
      //   || Like(a.Address.HausNr, search)))).ToList();
      var ll = new List<AdSitz>();
      foreach (var a in l)
      {
        if (Like(a.Person.Name1, search) || Like(a.Person.Name2, search)
        || Like(a.Person.Praedikat, search) || Like(a.Person.Vorname, search)
        || Like(a.Person.Titel, search)
        || Like(a.Name, search) || Like(a.Telefon, search)
        || Like(a.Fax, search) || Like(a.Mobil, search)
        || Like(a.Email, search) || Like(a.Homepage, search)
        || Like(a.Postfach, search) || Like(a.Bemerkung, search)
        || (a.Address != null && (Like(a.Address.Staat, search) || Like(a.Address.Plz, search)
        || Like(a.Address.Ort, search) || Like(a.Address.Strasse, search)
        || Like(a.Address.HausNr, search))))
          ll.Add(a);
      }
      l = ll;
    }
    var l2 = l.OrderBy(a => a?.Person?.Name1).ThenBy(a => a?.Person?.Vorname).ThenBy(a => a?.Person?.Uid)
      .ThenBy(a => a?.Reihenfolge).ThenBy(a => a?.Uid).ToList();
    return l2;
  }

  /// <summary>
  /// Gets a site.
  /// </summary>
  /// <returns>Site or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="suid">Affected site ID.</param>
  public AdSitz Get(ServiceDaten daten, string suid)
  {
    var db = GetDb(daten);
    var sl = db.AD_Sitz.Where(a => a.Mandant_Nr == daten.MandantNr && a.Uid == suid);
    return sl.FirstOrDefault();
  }

  /// <summary>
  /// Gets the number of usages of an address.
  /// </summary>
  /// <returns>Number of usages of an address.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Affected address ID.</param>
  public int GetAddressCount(ServiceDaten daten, string auid)
  {
    var db = GetDb(daten);
    var sl = db.AD_Sitz.Where(a => a.Mandant_Nr == daten.MandantNr && a.Adresse_Uid == auid);
    return sl.Count();
  }

  /// <summary>
  /// Gets Uid of a site.
  /// </summary>
  /// <returns>Uid of a site or null.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="puid">Affected persion uid.</param>
  /// <param name="name">Affected site name.</param>
  /// <param name="phone">Affected phone number.</param>
  /// <param name="fax">Affected fax number.</param>
  /// <param name="mobile">Affected mobile number.</param>
  /// <param name="email">Affected email.</param>
  /// <param name="homepage">Affected homepage.</param>
  /// <param name="pobox">Affected post office box.</param>
  /// <param name="createdby">Affected creation user.</param>
  /// <param name="createdat">Affected creation time.</param>
  public string GetUid(ServiceDaten daten, string puid, string name, string phone,
      string fax, string mobile, string email, string homepage, string pobox,
      string createdby, DateTime? createdat)
  {
    var db = GetDb(daten);
    var l = db.AD_Sitz.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(puid))
      l = l.Where(a => a.Person_Uid == puid);
    if (!string.IsNullOrEmpty(name))
      l = l.Where(a => a.Name == name);
    if (!string.IsNullOrEmpty(phone))
      l = l.Where(a => a.Telefon == phone);
    if (!string.IsNullOrEmpty(fax))
      l = l.Where(a => a.Fax == fax);
    if (!string.IsNullOrEmpty(mobile))
      l = l.Where(a => a.Mobil == mobile);
    if (!string.IsNullOrEmpty(email))
      l = l.Where(a => a.Email == email);
    if (!string.IsNullOrEmpty(homepage))
      l = l.Where(a => a.Homepage == homepage);
    if (!string.IsNullOrEmpty(pobox))
      l = l.Where(a => a.Postfach == pobox);
    if (!string.IsNullOrEmpty(createdby))
      l = l.Where(a => a.Angelegt_Von == createdby);
    if (createdat.HasValue)
      l = l.Where(a => a.Angelegt_Am == createdat.Value);
    return l.FirstOrDefault()?.Uid;
  }

#pragma warning restore CA1822
}
