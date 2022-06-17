// <copyright file="SbEreignisRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table SB_Ereignis.
/// </summary>
public partial class SbEreignisRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity of null.</returns>
  public SbEreignis Get(ServiceDaten daten, SbEreignis e)
  {
    var db = GetDb(daten);
    var b = db.SB_Ereignis.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Person_Uid == e.Person_Uid && a.Familie_Uid == e.Familie_Uid && a.Typ == e.Typ);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="personuid">Value of column Person_Uid.</param>
  /// <param name="familieuid">Value of column Familie_Uid.</param>
  /// <param name="typ">Value of column Typ.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity of null.</returns>
  public SbEreignis Get(ServiceDaten daten, int mandantnr, string personuid, string familieuid, string typ, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.SB_Ereignis.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Person_Uid == personuid && a.Familie_Uid == familieuid && a.Typ == typ);
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
  public List<SbEreignis> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, SbEreignis e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.SB_Ereignis.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, SbEreignis e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.SB_Ereignis.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="personuid">Value of column Person_Uid.</param>
  /// <param name="familieuid">Value of column Familie_Uid.</param>
  /// <param name="typ">Value of column Typ.</param>
  /// <param name="tag1">Value of column Tag1.</param>
  /// <param name="monat1">Value of column Monat1.</param>
  /// <param name="jahr1">Value of column Jahr1.</param>
  /// <param name="tag2">Value of column Tag2.</param>
  /// <param name="monat2">Value of column Monat2.</param>
  /// <param name="jahr2">Value of column Jahr2.</param>
  /// <param name="datumtyp">Value of column Datum_Typ.</param>
  /// <param name="ort">Value of column Ort.</param>
  /// <param name="bemerkung">Value of column Bemerkung.</param>
  /// <param name="quelleuid">Value of column Quelle_Uid.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <param name="replikationuid">Value of column Replikation_Uid.</param>
  /// <returns>Saved entity.</returns>
  public SbEreignis Save(ServiceDaten daten, int mandantnr, string personuid, string familieuid, string typ, int tag1, int monat1, int jahr1, int tag2, int monat2, int jahr2, string datumtyp, string ort, string bemerkung, string quelleuid, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, personuid, familieuid, typ);
    var e = a ?? new SbEreignis();
    e.Mandant_Nr = mandantnr;
    e.Person_Uid = personuid;
    e.Familie_Uid = familieuid;
    e.Typ = typ;
    e.Tag1 = tag1;
    e.Monat1 = monat1;
    e.Jahr1 = jahr1;
    e.Tag2 = tag2;
    e.Monat2 = monat2;
    e.Jahr2 = jahr2;
    e.Datum_Typ = datumtyp;
    e.Ort = ort;
    e.Bemerkung = bemerkung;
    e.Quelle_Uid = quelleuid;
    Functions.MachNichts(replikationuid);
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Ereignis.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.SB_Ereignis.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, SbEreignis e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.SB_Ereignis.Remove(a);
  }

#pragma warning restore CA1822
}
