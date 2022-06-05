// <copyright file="HhKontoRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für HH_Konto-Repository.
/// </summary>
public partial class HhKontoRep : RepositoryBase
{
#pragma warning disable CA1822

  public HhKonto Get(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    var b = db.HH_Konto.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public HhKonto Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.HH_Konto.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<HhKonto> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.HH_Konto.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.HH_Konto.Add(e);
  }

  public void Update(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.HH_Konto.Update(a);
    }
  }

  public HhKonto Save(ServiceDaten daten, int mandantnr, string uid, string sortierung, string art, string kz, string name, DateTime? gueltigvon, DateTime? gueltigbis, int periodevon, int periodebis, decimal betrag, decimal ebetrag, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new HhKonto();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Sortierung = sortierung;
    e.Art = art;
    e.Kz = kz;
    e.Name = name;
    e.Gueltig_Von = gueltigvon;
    e.Gueltig_Bis = gueltigbis;
    e.Periode_Von = periodevon;
    e.Periode_Bis = periodebis;
    e.Betrag = betrag;
    e.EBetrag = ebetrag;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Konto.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Konto.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.HH_Konto.Remove(a);
  }

#pragma warning restore CA1822
}
