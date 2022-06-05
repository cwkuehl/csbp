// <copyright file="WpWertpapierRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für WP_Wertpapier-Repository.
/// </summary>
public partial class WpWertpapierRep : RepositoryBase
{
#pragma warning disable CA1822

  public WpWertpapier Get(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    var b = db.WP_Wertpapier.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public WpWertpapier Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.WP_Wertpapier.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<WpWertpapier> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.WP_Wertpapier.Add(e);
  }

  public void Update(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.WP_Wertpapier.Update(a);
    }
  }

  public WpWertpapier Save(ServiceDaten daten, int mandantnr, string uid, string bezeichnung, string kuerzel, string parameter, string datenquelle, string status, string relationuid, string notiz, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new WpWertpapier();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Bezeichnung = bezeichnung;
    e.Kuerzel = kuerzel;
    e.Parameter = parameter;
    e.Datenquelle = datenquelle;
    e.Status = status;
    e.Relation_Uid = relationuid;
    e.Notiz = notiz;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.WP_Wertpapier.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.WP_Wertpapier.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.WP_Wertpapier.Remove(a);
  }

#pragma warning restore CA1822
}
