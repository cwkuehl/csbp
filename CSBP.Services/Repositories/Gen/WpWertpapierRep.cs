// <copyright file="WpWertpapierRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table WP_Wertpapier.
/// </summary>
public partial class WpWertpapierRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public WpWertpapier Get(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    var b = db.WP_Wertpapier.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
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
  public WpWertpapier Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.WP_Wertpapier.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
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
  public List<WpWertpapier> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.WP_Wertpapier.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.WP_Wertpapier.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
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

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="bezeichnung">Value of column Bezeichnung.</param>
  /// <param name="kuerzel">Value of column Kuerzel.</param>
  /// <param name="parameter">Value of column Parameter.</param>
  /// <param name="datenquelle">Value of column Datenquelle.</param>
  /// <param name="status">Value of column Status.</param>
  /// <param name="relationuid">Value of column Relation_Uid.</param>
  /// <param name="notiz">Value of column Notiz.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public WpWertpapier Save(ServiceDaten daten, int mandantnr, string uid, string bezeichnung, string kuerzel, string parameter, string datenquelle, string status, string relationuid, string notiz, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new WpWertpapier();
    if (a != null && a.TableName != "WP_Wertpapier")
      e = Clone(daten, a);
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

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, WpWertpapier e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    a = Clone(daten, a);
    if (a != null)
      db.WP_Wertpapier.Remove(a);
  }

  /// <summary>
  /// Detaches, Clones and Attaches an entity if it is a view entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly cloned entity.</returns>
  public WpWertpapier Clone(ServiceDaten daten, WpWertpapier e)
  {
    if (e != null && e.TableName != "WP_Wertpapier")
    {
      var db = GetDb(daten);
      db.Entry(e).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      var a = new WpWertpapier
      {
        Mandant_Nr = e.Mandant_Nr,
        Uid = e.Uid,
        Bezeichnung = e.Bezeichnung,
        Kuerzel = e.Kuerzel,
        Parameter = e.Parameter,
        Datenquelle = e.Datenquelle,
        Status = e.Status,
        Relation_Uid = e.Relation_Uid,
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
