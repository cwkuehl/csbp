// <copyright file="HhEreignisRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für HH_Ereignis-Repository.
/// </summary>
public partial class HhEreignisRep : RepositoryBase
{
#pragma warning disable CA1822

  public HhEreignis Get(ServiceDaten daten, HhEreignis e)
  {
    var db = GetDb(daten);
    var b = db.HH_Ereignis.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public HhEreignis Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.HH_Ereignis.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<HhEreignis> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.HH_Ereignis.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, HhEreignis e)
  {
    var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.HH_Ereignis.Add(e);
  }

  public void Update(ServiceDaten daten, HhEreignis e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.HH_Ereignis.Update(a);
    }
  }

  public HhEreignis Save(ServiceDaten daten, int mandantnr, string uid, string kz, string sollkontouid, string habenkontouid, string bezeichnung, string etext, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new HhEreignis();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Kz = kz;
    e.Soll_Konto_Uid = sollkontouid;
    e.Haben_Konto_Uid = habenkontouid;
    e.Bezeichnung = bezeichnung;
    e.EText = etext;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Ereignis.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Ereignis.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, HhEreignis e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.HH_Ereignis.Remove(a);
  }

#pragma warning restore CA1822
}
