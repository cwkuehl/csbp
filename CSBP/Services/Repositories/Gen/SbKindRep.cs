// <copyright file="SbKindRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table SB_Kind.
/// </summary>
public partial class SbKindRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public SbKind Get(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    var b = db.SB_Kind.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Familie_Uid == e.Familie_Uid && a.Kind_Uid == e.Kind_Uid);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="familieuid">Value of column Familie_Uid.</param>
  /// <param name="kinduid">Value of column Kind_Uid.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public SbKind Get(ServiceDaten daten, int mandantnr, string familieuid, string kinduid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.SB_Kind.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Familie_Uid == familieuid && a.Kind_Uid == kinduid);
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
  public List<SbKind> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.SB_Kind.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.SB_Kind.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.SB_Kind.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="familieuid">Value of column Familie_Uid.</param>
  /// <param name="kinduid">Value of column Kind_Uid.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <param name="replikationuid">Value of column Replikation_Uid.</param>
  /// <returns>Saved entity.</returns>
  public SbKind Save(ServiceDaten daten, int mandantnr, string familieuid, string kinduid, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, familieuid, kinduid);
    var e = a ?? new SbKind();
    e.Mandant_Nr = mandantnr;
    e.Familie_Uid = familieuid;
    e.Kind_Uid = kinduid;
    Functions.MachNichts(replikationuid);
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Kind.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Kind.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, SbKind e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.SB_Kind.Remove(a);
  }

#pragma warning restore CA1822
}
