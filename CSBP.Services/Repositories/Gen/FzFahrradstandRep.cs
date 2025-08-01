// <copyright file="FzFahrradstandRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table FZ_Fahrradstand.
/// </summary>
public partial class FzFahrradstandRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public FzFahrradstand Get(ServiceDaten daten, FzFahrradstand e)
  {
    var db = GetDb(daten);
    var b = db.FZ_Fahrradstand.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Fahrrad_Uid == e.Fahrrad_Uid && a.Datum == e.Datum && a.Nr == e.Nr);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="fahrraduid">Value of column Fahrrad_Uid.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="nr">Value of column Nr.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public FzFahrradstand Get(ServiceDaten daten, int mandantnr, string fahrraduid, DateTime datum, int nr, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.FZ_Fahrradstand.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Fahrrad_Uid == fahrraduid && a.Datum == datum && a.Nr == nr);
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
  public List<FzFahrradstand> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.FZ_Fahrradstand.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, FzFahrradstand e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.FZ_Fahrradstand.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, FzFahrradstand e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.FZ_Fahrradstand.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="fahrraduid">Value of column Fahrrad_Uid.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="nr">Value of column Nr.</param>
  /// <param name="zaehlerkm">Value of column Zaehler_km.</param>
  /// <param name="periodekm">Value of column Periode_km.</param>
  /// <param name="periodeschnitt">Value of column Periode_Schnitt.</param>
  /// <param name="beschreibung">Value of column Beschreibung.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <param name="replikationuid">Value of column Replikation_Uid.</param>
  /// <returns>Saved entity.</returns>
  public FzFahrradstand Save(ServiceDaten daten, int mandantnr, string fahrraduid, DateTime datum, int nr, decimal zaehlerkm, decimal periodekm, decimal periodeschnitt, string beschreibung, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, fahrraduid, datum, nr);
    var e = a ?? new FzFahrradstand();
    if (a != null && a.TableName != "FZ_Fahrradstand")
    {
      db.Entry(a).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      e = new FzFahrradstand
      {
        Mandant_Nr = a.Mandant_Nr,
        Fahrrad_Uid = a.Fahrrad_Uid,
        Datum = a.Datum,
        Nr = a.Nr,
        Zaehler_km = a.Zaehler_km,
        Periode_km = a.Periode_km,
        Periode_Schnitt = a.Periode_Schnitt,
        Beschreibung = a.Beschreibung,
        Angelegt_Von = a.Angelegt_Von,
        Angelegt_Am = a.Angelegt_Am,
        Geaendert_Von = a.Geaendert_Von,
        Geaendert_Am = a.Geaendert_Am,
        Replikation_Uid = a.Replikation_Uid,
      };
      db.Entry(e).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
    }
    e.Mandant_Nr = mandantnr;
    e.Fahrrad_Uid = fahrraduid;
    e.Datum = datum;
    e.Nr = nr;
    e.Zaehler_km = zaehlerkm;
    e.Periode_km = periodekm;
    e.Periode_Schnitt = periodeschnitt;
    e.Beschreibung = beschreibung;
    Functions.MachNichts(replikationuid);
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Fahrradstand.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.FZ_Fahrradstand.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, FzFahrradstand e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.FZ_Fahrradstand.Remove(a);
  }

#pragma warning restore CA1822
}
