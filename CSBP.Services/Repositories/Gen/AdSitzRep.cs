// <copyright file="AdSitzRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table AD_Sitz.
/// </summary>
public partial class AdSitzRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public AdSitz Get(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    var b = db.AD_Sitz.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Person_Uid == e.Person_Uid && a.Reihenfolge == e.Reihenfolge && a.Uid == e.Uid);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="personuid">Value of column Person_Uid.</param>
  /// <param name="reihenfolge">Value of column Reihenfolge.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public AdSitz Get(ServiceDaten daten, int mandantnr, string personuid, int reihenfolge, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.AD_Sitz.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Person_Uid == personuid && a.Reihenfolge == reihenfolge && a.Uid == uid);
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
  public List<AdSitz> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.AD_Sitz.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.AD_Sitz.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.AD_Sitz.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="personuid">Value of column Person_Uid.</param>
  /// <param name="reihenfolge">Value of column Reihenfolge.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="typ">Value of column Typ.</param>
  /// <param name="name">Value of column Name.</param>
  /// <param name="adresseuid">Value of column Adresse_Uid.</param>
  /// <param name="telefon">Value of column Telefon.</param>
  /// <param name="fax">Value of column Fax.</param>
  /// <param name="mobil">Value of column Mobil.</param>
  /// <param name="email">Value of column Email.</param>
  /// <param name="homepage">Value of column Homepage.</param>
  /// <param name="postfach">Value of column Postfach.</param>
  /// <param name="bemerkung">Value of column Bemerkung.</param>
  /// <param name="sitzstatus">Value of column Sitz_Status.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public AdSitz Save(ServiceDaten daten, int mandantnr, string personuid, int reihenfolge, string uid, int typ, string name, string adresseuid, string telefon, string fax, string mobil, string email, string homepage, string postfach, string bemerkung, int sitzstatus, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, personuid, reihenfolge, uid);
    var e = a ?? new AdSitz();
    e.Mandant_Nr = mandantnr;
    e.Person_Uid = personuid;
    e.Reihenfolge = reihenfolge;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Typ = typ;
    e.Name = name;
    e.Adresse_Uid = adresseuid;
    e.Telefon = telefon;
    e.Fax = fax;
    e.Mobil = mobil;
    e.Email = email;
    e.Homepage = homepage;
    e.Postfach = postfach;
    e.Bemerkung = bemerkung;
    e.Sitz_Status = sitzstatus;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Sitz.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Sitz.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, AdSitz e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.AD_Sitz.Remove(a);
  }

#pragma warning restore CA1822
}
