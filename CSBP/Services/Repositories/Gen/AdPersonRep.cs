// <copyright file="AdPersonRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Repositories.Base;

/// <summary>
/// Generierte Basis-Klasse für AD_Person-Repository.
/// </summary>
public partial class AdPersonRep : RepositoryBase
{
#pragma warning disable CA1822

  public AdPerson Get(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    var b = db.AD_Person.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public AdPerson Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.AD_Person.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<AdPerson> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.AD_Person.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.AD_Person.Add(e);
  }

  public void Update(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.AD_Person.Update(a);
    }
  }

  public AdPerson Save(ServiceDaten daten, int mandantnr, string uid, int typ, string geschlecht, DateTime? geburt, int geburtk, int anrede, int fanrede, string name1, string name2, string praedikat, string vorname, string titel, int personstatus, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new AdPerson();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Typ = typ;
    e.Geschlecht = geschlecht;
    e.Geburt = geburt;
    e.GeburtK = geburtk;
    e.Anrede = anrede;
    e.FAnrede = fanrede;
    e.Name1 = name1;
    e.Name2 = name2;
    e.Praedikat = praedikat;
    e.Vorname = vorname;
    e.Titel = titel;
    e.Person_Status = personstatus;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Person.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Person.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.AD_Person.Remove(a);
  }

#pragma warning restore CA1822
}
