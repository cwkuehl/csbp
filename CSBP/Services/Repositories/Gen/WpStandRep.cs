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
/// Generated repository base class for table WP_Stand.
/// </summary>
public partial class WpStandRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public WpStand Get(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    var b = db.WP_Stand.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Wertpapier_Uid == e.Wertpapier_Uid && a.Datum == e.Datum);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="wertpapieruid">Value of column Wertpapier_Uid.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public WpStand Get(ServiceDaten daten, int mandantnr, string wertpapieruid, DateTime datum, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.WP_Stand.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Wertpapier_Uid == wertpapieruid && a.Datum == datum);
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
  public List<WpStand> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.WP_Stand.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.WP_Stand.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
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

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="wertpapieruid">Value of column Wertpapier_Uid.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="stueckpreis">Value of column Stueckpreis.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
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

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, WpStand e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.WP_Stand.Remove(a);
  }

#pragma warning restore CA1822
}
