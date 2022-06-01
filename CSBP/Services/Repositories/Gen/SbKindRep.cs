// <copyright file="SbKindRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für SB_Kind-Repository.
/// </summary>
public partial class SbKindRep : RepositoryBase
{
#pragma warning disable CA1822

  public SbKind Get(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    var b = db.SB_Kind.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Familie_Uid == e.Familie_Uid && a.Kind_Uid == e.Kind_Uid);
    return b;
  }

  public SbKind Get(ServiceDaten daten, int mandantnr, string familieuid, string kinduid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.SB_Kind.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Familie_Uid == familieuid && a.Kind_Uid == kinduid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<SbKind> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.SB_Kind.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.SB_Kind.Add(e);
  }

  public void Update(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.SB_Kind.Update(a);
    }
  }

  public SbKind Save(ServiceDaten daten, int mandantnr, string familieuid, string kinduid, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, familieuid, kinduid);
    var e = a ?? new SbKind();
    e.Mandant_Nr = mandantnr;
    e.Familie_Uid = familieuid;
    e.Kind_Uid = kinduid;
    Functions.MachNichts(replikationuid);
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Kind.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Kind.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.SB_Kind.Remove(a);
  }

#pragma warning restore CA1822
}
