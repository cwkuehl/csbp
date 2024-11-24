// <copyright file="SbPersonRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Pedigree;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table SB_Person.
/// </summary>
public partial class SbPersonRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of ancestors.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected last or birth name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="desc">Descending by name or not.</param>
  /// <param name="suid">Affected source uid.</param>
  /// <param name="fuid">Affected family uid.</param>
  /// <param name="status1">Affected Status1.</param>
  /// <param name="status2">Affected Status2.</param>
  /// <param name="status3">Affected Status3.</param>
  /// <returns>List of ancestors.</returns>
  public List<SbPerson> GetList(ServiceDaten daten, string name = null, string firstname = null, string uid = null,
    bool desc = false, string suid = null, string fuid = null, int? status1 = null, int? status2 = null, int? status3 = null)
  {
    var mandantnr = daten.MandantNr;
    var db = GetDb(daten);
    var wl = string.IsNullOrEmpty(fuid) ? db.SB_Person.Where(a => a.Mandant_Nr == mandantnr)
      : db.SB_Kind.Where(a => a.Mandant_Nr == mandantnr && a.Familie_Uid == fuid)
        .Join(db.SB_Person.Where(a => a.Mandant_Nr == mandantnr), a => new { Uid = a.Kind_Uid }, b => new { b.Uid }, (a, b) => b);
    if (Functions.IsLike(name))
      wl = wl.Where(a => EF.Functions.Like(a.Name, name) || EF.Functions.Like(a.Geburtsname, name));
    if (Functions.IsLike(firstname))
      wl = wl.Where(a => EF.Functions.Like(a.Vorname, firstname));
    if (!string.IsNullOrEmpty(uid))
      wl = wl.Where(a => a.Uid == uid);
    if (!string.IsNullOrEmpty(suid))
      wl = wl.Where(a => a.Quelle_Uid == suid);
    if (status1.HasValue)
      wl = wl.Where(a => a.Status1 == status1.Value);
    if (status2.HasValue)
      wl = wl.Where(a => a.Status2 == status2.Value);
    if (status3.HasValue)
      wl = wl.Where(a => a.Status3 == status3.Value);
    var birth = GedcomEventEnum.BIRTH.ToString();
    var death = GedcomEventEnum.DEATH.ToString();
    var christ = GedcomEventEnum.CHRIST.ToString();
    var burial = GedcomEventEnum.BURIAL.ToString();
    var l = wl.GroupJoin(db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr && a.Typ == birth),
      a => new { a.Uid }, b => new { Uid = b.Person_Uid }, (a, b) => new { person = a, birth = b })
      .SelectMany(ab => ab.birth.DefaultIfEmpty(), (a, b) => new { a.person, birth = b })
      .GroupJoin(db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr && a.Typ == christ),
      a => new { a.person.Uid }, b => new { Uid = b.Person_Uid }, (a, b) => new { a.person, a.birth, christ = b })
      .SelectMany(ab => ab.christ.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, christ = b })
      .GroupJoin(db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr && a.Typ == death),
      a => new { a.person.Uid }, b => new { Uid = b.Person_Uid }, (a, b) => new { a.person, a.birth, a.christ, death = b })
      .SelectMany(ab => ab.death.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, a.christ, death = b })
      .GroupJoin(db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr && a.Typ == burial),
      a => new { a.person.Uid }, b => new { Uid = b.Person_Uid }, (a, b) => new { a.person, a.birth, a.christ, a.death, burial = b })
      .SelectMany(ab => ab.burial.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, a.christ, a.death, burial = b })
      .GroupJoin(db.SB_Kind.Where(a => a.Mandant_Nr == mandantnr),
      a => new { a.person.Uid }, b => new { Uid = b.Kind_Uid }, (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, parents = b })
      .SelectMany(ab => ab.parents.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, parents = b })
      .GroupJoin(db.SB_Familie.Where(a => a.Mandant_Nr == mandantnr),
      a => new { Uid = a.parents.Familie_Uid }, b => new { b.Uid }, (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, a.parents, family = b })
      .SelectMany(ab => ab.family.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, a.parents, family = b })
      .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == mandantnr),
      a => new { Uid = a.family.Mann_Uid }, b => new { b.Uid }, (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, a.parents, a.family, father = b })
      .SelectMany(ab => ab.father.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, a.parents, a.family, father = b })
      .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == mandantnr),
      a => new { Uid = a.family.Frau_Uid }, b => new { b.Uid }, (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, a.parents, a.family, a.father, mother = b })
      .SelectMany(ab => ab.mother.DefaultIfEmpty(), (a, b) => new { a.person, a.birth, a.christ, a.death, a.burial, a.parents, a.family, a.father, mother = b })
      .ToList()
      .Select(a =>
      {
        if (a.birth != null)
        {
          a.person.Birthdate = new PedigreeTimeData(a.birth).Deparse();
          a.person.Birthplace = a.birth.Ort;
          a.person.Birthmemo = a.birth.Bemerkung;
        }
        if (a.christ != null)
        {
          a.person.Christdate = new PedigreeTimeData(a.christ).Deparse();
          a.person.Christplace = a.christ.Ort;
          a.person.Christmemo = a.christ.Bemerkung;
        }
        if (a.death != null)
        {
          a.person.Deathdate = new PedigreeTimeData(a.death).Deparse();
          a.person.Deathplace = a.death.Ort;
          a.person.Deathmemo = a.death.Bemerkung;
        }
        if (a.burial != null)
        {
          a.person.Burialdate = new PedigreeTimeData(a.burial).Deparse();
          a.person.Burialplace = a.burial.Ort;
          a.person.Burialmemo = a.burial.Bemerkung;
        }
        if (a.father != null)
          a.person.Father = a.father;
        if (a.mother != null)
          a.person.Mother = a.mother;
        return a.person;
      });
    if (desc)
      return l.OrderByDescending(a => a.Geburtsname).ThenByDescending(a => a.Name).ThenByDescending(a => a.Vorname).ToList();
    return l.OrderBy(a => a.Geburtsname).ThenBy(a => a.Name).ThenBy(a => a.Vorname).ThenBy(a => a.Uid).ToList();
  }

  /// <summary>
  /// Gets the next ancestor by name.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="p0">Affected ancestor.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <returns>Ancestor uid or null.</returns>
  public SbPerson GetNext(ServiceDaten daten, SbPerson p0, string name, string firstname)
  {
    var db = GetDb(daten);
    var l = db.SB_Person.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (p0 != null && !string.IsNullOrEmpty(p0.Uid) && !string.IsNullOrEmpty(p0.Geburtsname) && !string.IsNullOrEmpty(p0.Name))
    {
      if (string.IsNullOrEmpty(p0.Vorname))
      {
        l = l.Where(a => (a.Geburtsname == p0.Geburtsname && a.Name == p0.Name && (a.Vorname == null || a.Vorname == "") && string.Compare(a.Uid, p0.Uid) > 0)
          || (a.Geburtsname == p0.Geburtsname && a.Name == p0.Name && !(a.Vorname == null || a.Vorname == ""))
          || (a.Geburtsname == p0.Geburtsname && string.Compare(a.Name, p0.Name) > 0)
          || string.Compare(a.Geburtsname, p0.Geburtsname) > 0);
      }
      else
      {
        l = l.Where(a => (a.Geburtsname == p0.Geburtsname && a.Name == p0.Name && a.Vorname == p0.Vorname && string.Compare(a.Uid, p0.Uid) > 0)
          || (a.Geburtsname == p0.Geburtsname && a.Name == p0.Name && string.Compare(a.Vorname, p0.Vorname) > 0)
          || (a.Geburtsname == p0.Geburtsname && string.Compare(a.Name, p0.Name) > 0)
          || string.Compare(a.Geburtsname, p0.Geburtsname) > 0);
      }
    }
    if (Functions.IsLike(name))
      l = l.Where(a => EF.Functions.Like(a.Name, name) || EF.Functions.Like(a.Geburtsname, name));
    if (Functions.IsLike(firstname))
      l = l.Where(a => EF.Functions.Like(a.Vorname, firstname));
    var ol = l.OrderBy(a => a.Geburtsname).ThenBy(a => a.Name).ThenBy(a => a.Vorname).ThenBy(a => a.Uid);
    return ol.FirstOrDefault();
  }

  /// <summary>
  /// Count rows of value Status1.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="status1">Affected value for column Status1.</param>
  /// <returns>Number of rows.</returns>
  public int CountStatus1(ServiceDaten daten, int status1)
  {
    var db = GetDb(daten);
    var c = db.SB_Person.Count(a => a.Mandant_Nr == daten.MandantNr && a.Status1 == status1);
    return c;
  }

  /// <summary>
  /// Count rows of value Status2.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="status2">Affected value for column Status2.</param>
  /// <returns>Number of rows.</returns>
  public int CountStatus2(ServiceDaten daten, int status2)
  {
    var db = GetDb(daten);
    var c = db.SB_Person.Count(a => a.Mandant_Nr == daten.MandantNr && a.Status2 == status2);
    return c;
  }

  /// <summary>
  /// Updates for column Status1.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="status1">Affected value for column Status1.</param>
  /// <returns>Number of updates.</returns>
  public int UpdateStatus1(ServiceDaten daten, int status1)
  {
    var db = GetDb(daten);
    var l = db.SB_Person.Where(a => a.Mandant_Nr == daten.MandantNr && a.Status1 != status1);
    var anzahl = 0;
    foreach (var e in l.ToList())
    {
      e.Status1 = status1;
      //// Update(daten, e);
      anzahl++;
    }
    return anzahl;
  }

  /// <summary>
  /// Updates for column Status2.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="op">Affected operator for column Status1.</param>
  /// <param name="status1">Affected value for column Status1.</param>
  /// <param name="status2">Affected value for column Status2.</param>
  /// <returns>Number of updates.</returns>
  public int UpdateStatus2(ServiceDaten daten, string op, int status1, int status2)
  {
    var db = GetDb(daten);
    var l = db.SB_Person.Where(a => a.Mandant_Nr == daten.MandantNr && a.Status2 != status2);
    if (!string.IsNullOrEmpty(op))
    {
      if (op == "=")
        l = l.Where(a => Math.Abs(a.Status1) == status1);
      else if (op == "<")
        l = l.Where(a => Math.Abs(a.Status1) < status1);
      else if (op == "<=")
        l = l.Where(a => Math.Abs(a.Status1) <= status1);
      else if (op == ">=")
        l = l.Where(a => Math.Abs(a.Status1) >= status1);
      else if (op == ">")
        l = l.Where(a => Math.Abs(a.Status1) > status1);
      else
        throw new ArgumentException($"Operator {op} missing.");
    }
    var anzahl = 0;
    foreach (var e in l.ToList())
    {
      e.Status2 = status2;
      //// Update(daten, e);
      anzahl++;
    }
    return anzahl;
  }

#pragma warning disable CA1822
}
