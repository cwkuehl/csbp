// <copyright file="MaMandantRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für MA_Mandant-Repository.
/// </summary>
public partial class MaMandantRep : RepositoryBase
{
#pragma warning disable CA1822

  public MaMandant Get(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    var b = db.MA_Mandant.FirstOrDefault(a => a.Nr == e.Nr);
    return b;
  }

  public MaMandant Get(ServiceDaten daten, int nr, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.MA_Mandant.FirstOrDefault(a => a.Nr == nr);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<MaMandant> GetList(ServiceDaten daten)
  {
    var db = GetDb(daten);
    var l = db.MA_Mandant;
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.MA_Mandant.Add(e);
  }

  public void Update(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.MA_Mandant.Update(a);
    }
  }

  public MaMandant Save(ServiceDaten daten, int nr, string beschreibung, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, nr);
    var e = a ?? new MaMandant();
    e.Nr = nr;
    e.Beschreibung = beschreibung;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.MA_Mandant.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.MA_Mandant.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.MA_Mandant.Remove(a);
  }

#pragma warning restore CA1822
}
