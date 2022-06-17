// <copyright file="MaParameterRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table MA_Parameter.
/// </summary>
public partial class MaParameterRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public MaParameter Get(ServiceDaten daten, MaParameter e)
  {
    var db = GetDb(daten);
    var b = db.MA_Parameter.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Schluessel == e.Schluessel);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="schluessel">Value of column Schluessel.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public MaParameter Get(ServiceDaten daten, int mandantnr, string schluessel, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.MA_Parameter.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Schluessel == schluessel);
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
  public List<MaParameter> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.MA_Parameter.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, MaParameter e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.MA_Parameter.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, MaParameter e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.MA_Parameter.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="schluessel">Value of column Schluessel.</param>
  /// <param name="wert">Value of column Wert.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <param name="replikationuid">Value of column Replikation_Uid.</param>
  /// <returns>Saved entity.</returns>
  public MaParameter Save(ServiceDaten daten, int mandantnr, string schluessel, string wert, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, schluessel);
    var e = a ?? new MaParameter();
    e.Mandant_Nr = mandantnr;
    e.Schluessel = schluessel;
    e.Wert = wert;
    Functions.MachNichts(replikationuid);
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.MA_Parameter.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.MA_Parameter.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, MaParameter e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.MA_Parameter.Remove(a);
  }

#pragma warning restore CA1822
}
