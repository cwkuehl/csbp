// <copyright file="WpAnlageRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table WP_Anlage.
/// </summary>
public partial class WpAnlageRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public WpAnlage Get(ServiceDaten daten, WpAnlage e)
  {
    var db = GetDb(daten);
    var b = db.WP_Anlage.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
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
  public WpAnlage Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.WP_Anlage.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
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
  public List<WpAnlage> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.WP_Anlage.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, WpAnlage e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.WP_Anlage.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, WpAnlage e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.WP_Anlage.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="wertpapieruid">Value of column Wertpapier_Uid.</param>
  /// <param name="bezeichnung">Value of column Bezeichnung.</param>
  /// <param name="parameter">Value of column Parameter.</param>
  /// <param name="notiz">Value of column Notiz.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public WpAnlage Save(ServiceDaten daten, int mandantnr, string uid, string wertpapieruid, string bezeichnung, string parameter, string notiz, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new WpAnlage();
    if (a != null && a.TableName != "WP_Anlage")
      e = Clone(daten, a);
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Wertpapier_Uid = wertpapieruid;
    e.Bezeichnung = bezeichnung;
    e.Parameter = parameter;
    e.Notiz = notiz;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.WP_Anlage.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.WP_Anlage.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, WpAnlage e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    a = Clone(daten, a);
    if (a != null)
      db.WP_Anlage.Remove(a);
  }

  /// <summary>
  /// Detaches, Clones and Attaches an entity if it is a view entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly cloned entity.</returns>
  public WpAnlage Clone(ServiceDaten daten, WpAnlage e)
  {
    if (e != null && e.TableName != "WP_Anlage")
    {
      var db = GetDb(daten);
      db.Entry(e).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      var a = new WpAnlage
      {
        Mandant_Nr = e.Mandant_Nr,
        Uid = e.Uid,
        Wertpapier_Uid = e.Wertpapier_Uid,
        Bezeichnung = e.Bezeichnung,
        Parameter = e.Parameter,
        Notiz = e.Notiz,
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
