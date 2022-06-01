// <copyright file="TbOrtRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für TB_Ort-Repository.
/// </summary>
public partial class TbOrtRep : RepositoryBase
{
#pragma warning disable CA1822

  public TbOrt Get(ServiceDaten daten, TbOrt e)
  {
    var db = GetDb(daten);
    var b = db.TB_Ort.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public TbOrt Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.TB_Ort.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<TbOrt> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.TB_Ort.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, TbOrt e)
  {
    var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.TB_Ort.Add(e);
  }

  public void Update(ServiceDaten daten, TbOrt e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.TB_Ort.Update(a);
    }
  }

  public TbOrt Save(ServiceDaten daten, int mandantnr, string uid, string bezeichnung, decimal breite, decimal laenge, decimal hoehe, string notiz, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new TbOrt();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Bezeichnung = bezeichnung;
    e.Breite = breite;
    e.Laenge = laenge;
    e.Hoehe = hoehe;
    e.Notiz = notiz;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Ort.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Ort.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, TbOrt e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.TB_Ort.Remove(a);
  }

#pragma warning restore CA1822
}
