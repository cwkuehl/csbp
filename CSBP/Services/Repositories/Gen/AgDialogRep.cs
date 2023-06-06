// <copyright file="AgDialogRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table AG_Dialog.
/// </summary>
public partial class AgDialogRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public AgDialog Get(ServiceDaten daten, AgDialog e)
  {
    var db = GetDb(daten);
    var b = db.AG_Dialog.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Api == e.Api && a.Datum == e.Datum && a.Nr == e.Nr);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="api">Value of column Api.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="nr">Value of column Nr.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public AgDialog Get(ServiceDaten daten, int mandantnr, string api, DateTime datum, int nr, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.AG_Dialog.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Api == api && a.Datum == datum && a.Nr == nr);
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
  public List<AgDialog> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.AG_Dialog.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, AgDialog e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.AG_Dialog.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, AgDialog e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.AG_Dialog.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="api">Value of column Api.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="nr">Value of column Nr.</param>
  /// <param name="url">Value of column Url.</param>
  /// <param name="frage">Value of column Frage.</param>
  /// <param name="antwort">Value of column Antwort.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public AgDialog Save(ServiceDaten daten, int mandantnr, string api, DateTime datum, int nr, string url, string frage, string antwort, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, api, datum, nr);
    var e = a ?? new AgDialog();
    e.Mandant_Nr = mandantnr;
    e.Api = api;
    e.Datum = datum;
    e.Nr = nr;
    e.Url = url;
    e.Frage = frage;
    e.Antwort = antwort;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AG_Dialog.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AG_Dialog.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, AgDialog e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.AG_Dialog.Remove(a);
  }

#pragma warning restore CA1822
}
