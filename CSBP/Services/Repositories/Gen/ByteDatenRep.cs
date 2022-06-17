// <copyright file="ByteDatenRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table Byte_Daten.
/// </summary>
public partial class ByteDatenRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public ByteDaten Get(ServiceDaten daten, ByteDaten e)
  {
    var db = GetDb(daten);
    var b = db.Byte_Daten.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Typ == e.Typ && a.Uid == e.Uid && a.Lfd_Nr == e.Lfd_Nr);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="typ">Value of column Typ.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="lfdnr">Value of column Lfd_Nr.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public ByteDaten Get(ServiceDaten daten, int mandantnr, string typ, string uid, int lfdnr, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.Byte_Daten.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Typ == typ && a.Uid == uid && a.Lfd_Nr == lfdnr);
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
  public List<ByteDaten> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.Byte_Daten.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, ByteDaten e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.Byte_Daten.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, ByteDaten e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.Byte_Daten.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="typ">Value of column Typ.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="lfdnr">Value of column Lfd_Nr.</param>
  /// <param name="metadaten">Value of column Metadaten.</param>
  /// <param name="bytes">Value of column Bytes.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public ByteDaten Save(ServiceDaten daten, int mandantnr, string typ, string uid, int lfdnr, string metadaten, byte[] bytes, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, typ, uid, lfdnr);
    var e = a ?? new ByteDaten();
    e.Mandant_Nr = mandantnr;
    e.Typ = typ;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Lfd_Nr = lfdnr;
    e.Metadaten = metadaten;
    e.Bytes = bytes;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.Byte_Daten.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.Byte_Daten.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, ByteDaten e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.Byte_Daten.Remove(a);
  }

#pragma warning restore CA1822
}
