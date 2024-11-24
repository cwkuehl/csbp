// <copyright file="AdPersonRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table AD_Person.
/// </summary>
public partial class AdPersonRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public AdPerson Get(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    var b = db.AD_Person.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public AdPerson Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.AD_Person.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
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
  public List<AdPerson> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.AD_Person.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.AD_Person.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.AD_Person.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="typ">Value of column Typ.</param>
  /// <param name="geschlecht">Value of column Geschlecht.</param>
  /// <param name="geburt">Value of column Geburt.</param>
  /// <param name="geburtk">Value of column GeburtK.</param>
  /// <param name="anrede">Value of column Anrede.</param>
  /// <param name="fanrede">Value of column FAnrede.</param>
  /// <param name="name1">Value of column Name1.</param>
  /// <param name="name2">Value of column Name2.</param>
  /// <param name="praedikat">Value of column Praedikat.</param>
  /// <param name="vorname">Value of column Vorname.</param>
  /// <param name="titel">Value of column Titel.</param>
  /// <param name="personstatus">Value of column Person_Status.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public AdPerson Save(ServiceDaten daten, int mandantnr, string uid, int typ, string geschlecht, DateTime? geburt, int geburtk, int anrede, int fanrede, string name1, string name2, string praedikat, string vorname, string titel, int personstatus, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new AdPerson();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Typ = typ;
    e.Geschlecht = geschlecht;
    e.Geburt = geburt;
    e.GeburtK = geburtk;
    e.Anrede = anrede;
    e.FAnrede = fanrede;
    e.Name1 = name1;
    e.Name2 = name2;
    e.Praedikat = praedikat;
    e.Vorname = vorname;
    e.Titel = titel;
    e.Person_Status = personstatus;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Person.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.AD_Person.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, AdPerson e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.AD_Person.Remove(a);
  }

#pragma warning restore CA1822
}
