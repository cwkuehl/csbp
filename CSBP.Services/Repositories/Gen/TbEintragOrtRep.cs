// <copyright file="TbEintragOrtRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table TB_Eintrag_Ort.
/// </summary>
public partial class TbEintragOrtRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public TbEintragOrt Get(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    var b = db.TB_Eintrag_Ort.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Ort_Uid == e.Ort_Uid && a.Datum_Von == e.Datum_Von && a.Datum_Bis == e.Datum_Bis);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="ortuid">Value of column Ort_Uid.</param>
  /// <param name="datumvon">Value of column Datum_Von.</param>
  /// <param name="datumbis">Value of column Datum_Bis.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public TbEintragOrt Get(ServiceDaten daten, int mandantnr, string ortuid, DateTime datumvon, DateTime datumbis, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.TB_Eintrag_Ort.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Ort_Uid == ortuid && a.Datum_Von == datumvon && a.Datum_Bis == datumbis);
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
  public List<TbEintragOrt> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.TB_Eintrag_Ort.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.TB_Eintrag_Ort.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.TB_Eintrag_Ort.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="ortuid">Value of column Ort_Uid.</param>
  /// <param name="datumvon">Value of column Datum_Von.</param>
  /// <param name="datumbis">Value of column Datum_Bis.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <returns>Saved entity.</returns>
  public TbEintragOrt Save(ServiceDaten daten, int mandantnr, string ortuid, DateTime datumvon, DateTime datumbis, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, ortuid, datumvon, datumbis);
    var e = a ?? new TbEintragOrt();
    e.Mandant_Nr = mandantnr;
    e.Ort_Uid = ortuid;
    e.Datum_Von = datumvon;
    e.Datum_Bis = datumbis;
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Eintrag_Ort.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Eintrag_Ort.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.TB_Eintrag_Ort.Remove(a);
  }

#pragma warning restore CA1822
}
