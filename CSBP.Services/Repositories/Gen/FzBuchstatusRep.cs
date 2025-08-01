// <copyright file="FzBuchstatusRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table FZ_Buchstatus.
/// </summary>
public partial class FzBuchstatusRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public FzBuchstatus Get(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    var b = db.FZ_Buchstatus.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Buch_Uid == e.Buch_Uid);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="buchuid">Value of column Buch_Uid.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public FzBuchstatus Get(ServiceDaten daten, int mandantnr, string buchuid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.FZ_Buchstatus.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Buch_Uid == buchuid);
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
  public List<FzBuchstatus> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.FZ_Buchstatus.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.FZ_Buchstatus.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.FZ_Buchstatus.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="buchuid">Value of column Buch_Uid.</param>
  /// <param name="istbesitz">Value of column Ist_Besitz.</param>
  /// <param name="lesedatum">Value of column Lesedatum.</param>
  /// <param name="hoerdatum">Value of column Hoerdatum.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <param name="replikationuid">Value of column Replikation_Uid.</param>
  /// <returns>Saved entity.</returns>
  public FzBuchstatus Save(ServiceDaten daten, int mandantnr, string buchuid, bool istbesitz, DateTime? lesedatum, DateTime? hoerdatum, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, buchuid);
    var e = a ?? new FzBuchstatus();
    if (a != null && a.TableName != "FZ_Buchstatus")
    {
      db.Entry(a).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      e = new FzBuchstatus
      {
        Mandant_Nr = a.Mandant_Nr,
        Buch_Uid = a.Buch_Uid,
        Ist_Besitz = a.Ist_Besitz,
        Lesedatum = a.Lesedatum,
        Hoerdatum = a.Hoerdatum,
        Angelegt_Von = a.Angelegt_Von,
        Angelegt_Am = a.Angelegt_Am,
        Geaendert_Von = a.Geaendert_Von,
        Geaendert_Am = a.Geaendert_Am,
        Replikation_Uid = a.Replikation_Uid,
      };
      db.Entry(e).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
    }
    e.Mandant_Nr = mandantnr;
    e.Buch_Uid = buchuid;
    e.Ist_Besitz = istbesitz;
    e.Lesedatum = lesedatum;
    e.Hoerdatum = hoerdatum;
    Functions.MachNichts(replikationuid);
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Buchstatus.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Buchstatus.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, FzBuchstatus e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.FZ_Buchstatus.Remove(a);
  }

#pragma warning restore CA1822
}
