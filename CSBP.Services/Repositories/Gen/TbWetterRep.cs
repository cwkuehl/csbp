// <copyright file="TbWetterRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Apis.Services;
using CSBP.Services.Base;
using CSBP.Services.Repositories.Base;

/// <summary>
/// Generated repository base class for table TB_Wetter.
/// </summary>
public partial class TbWetterRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public TbWetter Get(ServiceDaten daten, TbWetter e)
  {
    var db = GetDb(daten);
    var b = db.TB_Wetter.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Datum == e.Datum && a.Ort_Uid == e.Ort_Uid && a.Api == e.Api);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="ortuid">Value of column Ort_Uid.</param>
  /// <param name="api">Value of column Api.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public TbWetter Get(ServiceDaten daten, int mandantnr, DateTime datum, string ortuid, string api, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.TB_Wetter.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Datum == datum && a.Ort_Uid == ortuid && a.Api == api);
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
  public List<TbWetter> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.TB_Wetter.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, TbWetter e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.TB_Wetter.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, TbWetter e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.TB_Wetter.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="ortuid">Value of column Ort_Uid.</param>
  /// <param name="api">Value of column Api.</param>
  /// <param name="werte">Value of column Werte.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public TbWetter Save(ServiceDaten daten, int mandantnr, DateTime datum, string ortuid, string api, string werte, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, datum, ortuid, api);
    var e = a ?? new TbWetter();
    if (a != null && a.TableName != "TB_Wetter")
      e = Clone(daten, a);
    e.Mandant_Nr = mandantnr;
    e.Datum = datum;
    e.Ort_Uid = ortuid;
    e.Api = api;
    e.Werte = werte;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Wetter.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Wetter.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, TbWetter e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    a = Clone(daten, a);
    if (a != null)
      db.TB_Wetter.Remove(a);
  }

  /// <summary>
  /// Detaches, Clones and Attaches an entity if it is a view entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly cloned entity.</returns>
  public TbWetter Clone(ServiceDaten daten, TbWetter e)
  {
    if (e != null && e.TableName != "TB_Wetter")
    {
      var db = GetDb(daten);
      db.Entry(e).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      var a = new TbWetter
      {
        Mandant_Nr = e.Mandant_Nr,
        Datum = e.Datum,
        Ort_Uid = e.Ort_Uid,
        Api = e.Api,
        Werte = e.Werte,
        Angelegt_Von = e.Angelegt_Von,
        Angelegt_Am = e.Angelegt_Am,
        Geaendert_Von = e.Geaendert_Von,
        Geaendert_Am = e.Geaendert_Am,
      };
      db.Entry(a).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
      return a;
    }
    return e;
  }
#pragma warning restore CA1822
}
