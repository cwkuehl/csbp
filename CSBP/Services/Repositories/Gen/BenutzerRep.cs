// <copyright file="BenutzerRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für Benutzer-Repository.
/// </summary>
public partial class BenutzerRep : RepositoryBase
{
#pragma warning disable CA1822

  public Benutzer Get(ServiceDaten daten, Benutzer e)
  {
    var db = GetDb(daten);
    var b = db.Benutzer.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Benutzer_ID == e.Benutzer_ID);
    return b;
  }

  public Benutzer Get(ServiceDaten daten, int mandantnr, string benutzerid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.Benutzer.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Benutzer_ID == benutzerid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<Benutzer> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.Benutzer.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, Benutzer e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.Benutzer.Add(e);
  }

  public void Update(ServiceDaten daten, Benutzer e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.Benutzer.Update(a);
    }
  }

  public Benutzer Save(ServiceDaten daten, int mandantnr, string benutzerid, string passwort, int berechtigung, int aktperiode, int personnr, DateTime? geburt, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, benutzerid);
    var e = a ?? new Benutzer();
    e.Mandant_Nr = mandantnr;
    e.Benutzer_ID = benutzerid;
    e.Passwort = passwort;
    e.Berechtigung = berechtigung;
    e.Akt_Periode = aktperiode;
    e.Person_Nr = personnr;
    e.Geburt = geburt;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.Benutzer.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.Benutzer.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, Benutzer e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.Benutzer.Remove(a);
  }

#pragma warning restore CA1822
}
