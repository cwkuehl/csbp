// <copyright file="WpStandRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für WP_Stand-Repository.
/// </summary>
public partial class WpStandRep : RepositoryBase
{
#pragma warning disable CA1822

  public WpStand Get(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    var b = db.WP_Stand.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Wertpapier_Uid == e.Wertpapier_Uid && a.Datum == e.Datum);
    return b;
  }

  public WpStand Get(ServiceDaten daten, int mandantnr, string wertpapieruid, DateTime datum, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.WP_Stand.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Wertpapier_Uid == wertpapieruid && a.Datum == datum);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<WpStand> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.WP_Stand.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.WP_Stand.Add(e);
  }

  public void Update(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.WP_Stand.Update(a);
    }
  }

  public WpStand Save(ServiceDaten daten, int mandantnr, string wertpapieruid, DateTime datum, decimal stueckpreis, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, wertpapieruid, datum);
    var e = a ?? new WpStand();
    e.Mandant_Nr = mandantnr;
    e.Wertpapier_Uid = wertpapieruid;
    e.Datum = datum;
    e.Stueckpreis = stueckpreis;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.WP_Stand.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.WP_Stand.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.WP_Stand.Remove(a);
  }

#pragma warning restore CA1822
}
