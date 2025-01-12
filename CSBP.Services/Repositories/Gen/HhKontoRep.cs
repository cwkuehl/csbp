// <copyright file="HhKontoRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Repositories.Base;

/// <summary>
/// Generated repository base class for table HH_Konto.
/// </summary>
public partial class HhKontoRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public HhKonto Get(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    var b = db.HH_Konto.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public HhKonto Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.HH_Konto.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
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
  public List<HhKonto> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.HH_Konto.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.HH_Konto.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
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

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="sortierung">Value of column Sortierung.</param>
  /// <param name="art">Value of column Art.</param>
  /// <param name="kz">Value of column Kz.</param>
  /// <param name="name">Value of column Name.</param>
  /// <param name="gueltigvon">Value of column Gueltig_Von.</param>
  /// <param name="gueltigbis">Value of column Gueltig_Bis.</param>
  /// <param name="periodevon">Value of column Periode_Von.</param>
  /// <param name="periodebis">Value of column Periode_Bis.</param>
  /// <param name="betrag">Value of column Betrag.</param>
  /// <param name="ebetrag">Value of column EBetrag.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
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

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, HhKonto e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.HH_Konto.Remove(a);
  }

#pragma warning restore CA1822
}
