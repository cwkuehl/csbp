// <copyright file="HhBilanzRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table HH_Bilanz.
/// </summary>
public partial class HhBilanzRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public HhBilanz Get(ServiceDaten daten, HhBilanz e)
  {
    var db = GetDb(daten);
    var b = db.HH_Bilanz.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Periode == e.Periode && a.Kz == e.Kz && a.Konto_Uid == e.Konto_Uid);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="periode">Value of column Periode.</param>
  /// <param name="kz">Value of column Kz.</param>
  /// <param name="kontouid">Value of column Konto_Uid.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public HhBilanz Get(ServiceDaten daten, int mandantnr, int periode, string kz, string kontouid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.HH_Bilanz.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Periode == periode && a.Kz == kz && a.Konto_Uid == kontouid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  /// <summary>
  /// Gets list of entities.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <returns>List of entities.</returns>
  public List<HhBilanz> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.HH_Bilanz.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, HhBilanz e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.HH_Bilanz.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, HhBilanz e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.HH_Bilanz.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="periode">Value of column Periode.</param>
  /// <param name="kz">Value of column Kz.</param>
  /// <param name="kontouid">Value of column Konto_Uid.</param>
  /// <param name="sh">Value of column SH.</param>
  /// <param name="betrag">Value of column Betrag.</param>
  /// <param name="esh">Value of column ESH.</param>
  /// <param name="ebetrag">Value of column EBetrag.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public HhBilanz Save(ServiceDaten daten, int mandantnr, int periode, string kz, string kontouid, string sh, decimal betrag, string esh, decimal ebetrag, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, periode, kz, kontouid);
    var e = a ?? new HhBilanz();
    e.Mandant_Nr = mandantnr;
    e.Periode = periode;
    e.Kz = kz;
    e.Konto_Uid = kontouid;
    e.SH = sh;
    e.Betrag = betrag;
    e.ESH = esh;
    e.EBetrag = ebetrag;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Bilanz.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Bilanz.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, HhBilanz e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.HH_Bilanz.Remove(a);
  }

#pragma warning restore CA1822
}
