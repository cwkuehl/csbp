// <copyright file="AdSitzRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für AD_Sitz-Repository.
/// </summary>
public partial class AdSitzRep : RepositoryBase
{
#pragma warning disable CA1822

  public AdSitz Get(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    var b = db.AD_Sitz.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Person_Uid == e.Person_Uid && a.Reihenfolge == e.Reihenfolge && a.Uid == e.Uid);
    return b;
  }

  public AdSitz Get(ServiceDaten daten, int mandantnr, string personuid, int reihenfolge, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.AD_Sitz.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Person_Uid == personuid && a.Reihenfolge == reihenfolge && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<AdSitz> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.AD_Sitz.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.AD_Sitz.Add(e);
  }

  public void Update(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.AD_Sitz.Update(a);
    }
  }

  public AdSitz Save(ServiceDaten daten, int mandantnr, string personuid, int reihenfolge, string uid, int typ, string name, string adresseuid, string telefon, string fax, string mobil, string email, string homepage, string postfach, string bemerkung, int sitzstatus, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, personuid, reihenfolge, uid);
    var e = a ?? new AdSitz();
    e.Mandant_Nr = mandantnr;
    e.Person_Uid = personuid;
    e.Reihenfolge = reihenfolge;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Typ = typ;
    e.Name = name;
    e.Adresse_Uid = adresseuid;
    e.Telefon = telefon;
    e.Fax = fax;
    e.Mobil = mobil;
    e.Email = email;
    e.Homepage = homepage;
    e.Postfach = postfach;
    e.Bemerkung = bemerkung;
    e.Sitz_Status = sitzstatus;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Sitz.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Sitz.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.AD_Sitz.Remove(a);
  }

#pragma warning restore CA1822
}
