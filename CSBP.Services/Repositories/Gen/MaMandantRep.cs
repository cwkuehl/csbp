// <copyright file="MaMandantRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table MA_Mandant.
/// </summary>
public partial class MaMandantRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public MaMandant Get(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    var b = db.MA_Mandant.FirstOrDefault(a => a.Nr == e.Nr);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Value of column Nr.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public MaMandant Get(ServiceDaten daten, int nr, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.MA_Mandant.FirstOrDefault(a => a.Nr == nr);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  /// <summary>
  /// Gets list of entities.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of entities.</returns>
  public List<MaMandant> GetList(ServiceDaten daten)
  {
    var db = GetDb(daten);
    var l = db.MA_Mandant;
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.MA_Mandant.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.MA_Mandant.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Value of column Nr.</param>
  /// <param name="beschreibung">Value of column Beschreibung.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public MaMandant Save(ServiceDaten daten, int nr, string beschreibung, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, nr);
    var e = a ?? new MaMandant();
    e.Nr = nr;
    e.Beschreibung = beschreibung;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.MA_Mandant.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.MA_Mandant.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, MaMandant e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.MA_Mandant.Remove(a);
  }

#pragma warning restore CA1822
}
