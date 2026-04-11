// <copyright file="EnAbfrageRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table EN_Abfrage.
/// </summary>
public partial class EnAbfrageRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public EnAbfrage Get(ServiceDaten daten, EnAbfrage e)
  {
    var db = GetDb(daten);
    var b = db.EN_Abfrage.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
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
  public EnAbfrage Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.EN_Abfrage.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
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
  public List<EnAbfrage> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.EN_Abfrage.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, EnAbfrage e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.EN_Abfrage.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, EnAbfrage e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    a = Clone(daten, a);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.EN_Abfrage.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="sortierung">Value of column Sortierung.</param>
  /// <param name="art">Value of column Art.</param>
  /// <param name="bezeichnung">Value of column Bezeichnung.</param>
  /// <param name="hosturl">Value of column Host_Url.</param>
  /// <param name="datentyp">Value of column Datentyp.</param>
  /// <param name="schreibbarkeit">Value of column Schreibbarkeit.</param>
  /// <param name="einheit">Value of column Einheit.</param>
  /// <param name="param1">Value of column Param1.</param>
  /// <param name="param2">Value of column Param2.</param>
  /// <param name="param3">Value of column Param3.</param>
  /// <param name="param4">Value of column Param4.</param>
  /// <param name="param5">Value of column Param5.</param>
  /// <param name="status">Value of column Status.</param>
  /// <param name="notiz">Value of column Notiz.</param>
  /// <param name="parameter">Value of column Parameter.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public EnAbfrage Save(ServiceDaten daten, int mandantnr, string uid, string sortierung, string art, string bezeichnung, string hosturl, string datentyp, string schreibbarkeit, string einheit, string param1, string param2, string param3, string param4, string param5, string status, string notiz, string parameter, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new EnAbfrage();
    if (a != null && a.TableName != "EN_Abfrage")
      e = Clone(daten, a);
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Sortierung = sortierung;
    e.Art = art;
    e.Bezeichnung = bezeichnung;
    e.Host_Url = hosturl;
    e.Datentyp = datentyp;
    e.Schreibbarkeit = schreibbarkeit;
    e.Einheit = einheit;
    e.Param1 = param1;
    e.Param2 = param2;
    e.Param3 = param3;
    e.Param4 = param4;
    e.Param5 = param5;
    e.Status = status;
    e.Notiz = notiz;
    e.Parameter = parameter;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.EN_Abfrage.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.EN_Abfrage.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, EnAbfrage e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    a = Clone(daten, a);
    if (a != null)
      db.EN_Abfrage.Remove(a);
  }

  /// <summary>
  /// Detaches, Clones and Attaches an entity if it is a view entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly cloned entity.</returns>
  public EnAbfrage Clone(ServiceDaten daten, EnAbfrage e)
  {
    if (e != null && e.TableName != "EN_Abfrage")
    {
      var db = GetDb(daten);
      db.Entry(e).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      var a = new EnAbfrage
      {
        Mandant_Nr = e.Mandant_Nr,
        Uid = e.Uid,
        Sortierung = e.Sortierung,
        Art = e.Art,
        Bezeichnung = e.Bezeichnung,
        Host_Url = e.Host_Url,
        Datentyp = e.Datentyp,
        Schreibbarkeit = e.Schreibbarkeit,
        Einheit = e.Einheit,
        Param1 = e.Param1,
        Param2 = e.Param2,
        Param3 = e.Param3,
        Param4 = e.Param4,
        Param5 = e.Param5,
        Status = e.Status,
        Notiz = e.Notiz,
        Parameter = e.Parameter,
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
