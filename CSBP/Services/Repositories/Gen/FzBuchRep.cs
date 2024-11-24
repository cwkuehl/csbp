// <copyright file="FzBuchRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table FZ_Buch.
/// </summary>
public partial class FzBuchRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public FzBuch Get(ServiceDaten daten, FzBuch e)
  {
    var db = GetDb(daten);
    var b = db.FZ_Buch.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
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
  public FzBuch Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.FZ_Buch.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
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
  public List<FzBuch> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.FZ_Buch.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, FzBuch e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.FZ_Buch.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, FzBuch e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.FZ_Buch.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="autoruid">Value of column Autor_Uid.</param>
  /// <param name="serieuid">Value of column Serie_Uid.</param>
  /// <param name="seriennummer">Value of column Seriennummer.</param>
  /// <param name="titel">Value of column Titel.</param>
  /// <param name="untertitel">Value of column Untertitel.</param>
  /// <param name="seiten">Value of column Seiten.</param>
  /// <param name="sprachenr">Value of column Sprache_Nr.</param>
  /// <param name="notiz">Value of column Notiz.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public FzBuch Save(ServiceDaten daten, int mandantnr, string uid, string autoruid, string serieuid, int seriennummer, string titel, string untertitel, int seiten, int sprachenr, string notiz, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new FzBuch();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Autor_Uid = autoruid;
    e.Serie_Uid = serieuid;
    e.Seriennummer = seriennummer;
    e.Titel = titel;
    e.Untertitel = untertitel;
    e.Seiten = seiten;
    e.Sprache_Nr = sprachenr;
    e.Notiz = notiz;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Buch.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Buch.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, FzBuch e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.FZ_Buch.Remove(a);
  }

#pragma warning restore CA1822
}
