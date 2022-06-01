// <copyright file="FzBuchstatusRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für FZ_Buchstatus-Repository.
/// </summary>
public partial class FzBuchstatusRep : RepositoryBase
{
#pragma warning disable CA1822

  public FzBuchstatus Get(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    var b = db.FZ_Buchstatus.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Buch_Uid == e.Buch_Uid);
    return b;
  }

  public FzBuchstatus Get(ServiceDaten daten, int mandantnr, string buchuid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.FZ_Buchstatus.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Buch_Uid == buchuid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<FzBuchstatus> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.FZ_Buchstatus.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.FZ_Buchstatus.Add(e);
  }

  public void Update(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.FZ_Buchstatus.Update(a);
    }
  }

  public FzBuchstatus Save(ServiceDaten daten, int mandantnr, string buchuid, bool istbesitz, DateTime? lesedatum, DateTime? hoerdatum, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, buchuid);
    var e = a ?? new FzBuchstatus();
    e.Mandant_Nr = mandantnr;
    e.Buch_Uid = buchuid;
    e.Ist_Besitz = istbesitz;
    e.Lesedatum = lesedatum;
    e.Hoerdatum = hoerdatum;
    Functions.MachNichts(replikationuid);
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Buchstatus.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Buchstatus.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.FZ_Buchstatus.Remove(a);
  }

#pragma warning restore CA1822
}
