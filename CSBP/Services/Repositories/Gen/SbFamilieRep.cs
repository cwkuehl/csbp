// <copyright file="SbFamilieRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für SB_Familie-Repository.
/// </summary>
public partial class SbFamilieRep : RepositoryBase
{
#pragma warning disable CA1822

  public SbFamilie Get(ServiceDaten daten, SbFamilie e)
  {
    var db = GetDb(daten);
    var b = db.SB_Familie.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public SbFamilie Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.SB_Familie.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<SbFamilie> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.SB_Familie.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, SbFamilie e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.SB_Familie.Add(e);
  }

  public void Update(ServiceDaten daten, SbFamilie e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.SB_Familie.Update(a);
    }
  }

  public SbFamilie Save(ServiceDaten daten, int mandantnr, string uid, string mannuid, string frauuid, int status1, int status2, int status3, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new SbFamilie();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Mann_Uid = mannuid;
    e.Frau_Uid = frauuid;
    e.Status1 = status1;
    e.Status2 = status2;
    e.Status3 = status3;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Familie.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Familie.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, SbFamilie e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.SB_Familie.Remove(a);
  }

#pragma warning restore CA1822
}
