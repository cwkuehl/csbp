// <copyright file="TbEintragRep.cs" company="cwkuehl.de">
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
/// Generated repository base class for table TB_Eintrag.
/// </summary>
public partial class TbEintragRep : RepositoryBase
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  /// <returns>Entity or null.</returns>
  public TbEintrag Get(ServiceDaten daten, TbEintrag e)
  {
    var db = GetDb(daten);
    var b = db.TB_Eintrag.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Datum == e.Datum);
    return b;
  }

  /// <summary>
  /// Gets entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="detached">Detaches entity after read or not.</param>
  /// <returns>Entity or null.</returns>
  public TbEintrag Get(ServiceDaten daten, int mandantnr, DateTime datum, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.TB_Eintrag.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Datum == datum);
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
  public List<TbEintrag> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.TB_Eintrag.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  /// <summary>
  /// Inserts entity.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">New entity.</param>
  public void Insert(ServiceDaten daten, TbEintrag e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.TB_Eintrag.Add(e);
  }

  /// <summary>
  /// Updates entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Update(ServiceDaten daten, TbEintrag e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.TB_Eintrag.Update(a);
    }
  }

  /// <summary>
  /// Saves entity by separated parameters.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="mandantnr">Value of column Mandant_Nr.</param>
  /// <param name="datum">Value of column Datum.</param>
  /// <param name="eintrag">Value of column Eintrag.</param>
  /// <param name="angelegtvon">Value of column Angelegt_Von.</param>
  /// <param name="angelegtam">Value of column Angelegt_Am.</param>
  /// <param name="geaendertvon">Value of column Geaendert_Von.</param>
  /// <param name="geaendertam">Value of column Geaendert_Am.</param>
  /// <param name="replikationuid">Value of column Replikation_Uid.</param>
  /// <returns>Saved entity.</returns>
  public TbEintrag Save(ServiceDaten daten, int mandantnr, DateTime datum, string eintrag, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
  {
    var db = GetDb(daten);
    var a = Get(daten, mandantnr, datum);
    var e = a ?? new TbEintrag();
    e.Mandant_Nr = mandantnr;
    e.Datum = datum;
    e.Eintrag = eintrag;
    Functions.MachNichts(replikationuid);
    if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Eintrag.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.TB_Eintrag.Update(e);
    }
    return e;
  }

  /// <summary>
  /// Deletes entity by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Entity with primary key.</param>
  public void Delete(ServiceDaten daten, TbEintrag e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.TB_Eintrag.Remove(a);
  }

#pragma warning restore CA1822
}
