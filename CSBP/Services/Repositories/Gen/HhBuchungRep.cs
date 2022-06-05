// <copyright file="HhBuchungRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für HH_Buchung-Repository.
/// </summary>
public partial class HhBuchungRep : RepositoryBase
{
#pragma warning disable CA1822

  public HhBuchung Get(ServiceDaten daten, HhBuchung e)
  {
    var db = GetDb(daten);
    var b = db.HH_Buchung.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
    return b;
  }

  public HhBuchung Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.HH_Buchung.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<HhBuchung> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.HH_Buchung.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, HhBuchung e)
  {
    var db = GetDb(daten);
    e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
    MachAngelegt(e, daten);
    db.HH_Buchung.Add(e);
  }

  public void Update(ServiceDaten daten, HhBuchung e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    db.Entry(a).CurrentValues.SetValues(e);
    if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      MachGeaendert(a, daten);
      db.HH_Buchung.Update(a);
    }
  }

  public HhBuchung Save(ServiceDaten daten, int mandantnr, string uid, DateTime sollvaluta, DateTime habenvaluta, string kz, decimal betrag, decimal ebetrag, string sollkontouid, string habenkontouid, string btext, string belegnr, DateTime belegdatum, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
  {
    var db = GetDb(daten);
    var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
    var e = a ?? new HhBuchung();
    e.Mandant_Nr = mandantnr;
    e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
    e.Soll_Valuta = sollvaluta;
    e.Haben_Valuta = habenvaluta;
    e.Kz = kz;
    e.Betrag = betrag;
    e.EBetrag = ebetrag;
    e.Soll_Konto_Uid = sollkontouid;
    e.Haben_Konto_Uid = habenkontouid;
    e.BText = btext;
    e.Beleg_Nr = belegnr;
    e.Beleg_Datum = belegdatum;
      if (a == null)
    {
      MachAngelegt(e, daten, angelegtam, angelegtvon);
      if (!string.IsNullOrEmpty(geaendertvon))
        MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Buchung.Add(e);
    }
    else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
    {
      if (!string.IsNullOrEmpty(angelegtvon))
        MachAngelegt(e, daten, angelegtam, angelegtvon);
      MachGeaendert(e, daten, geaendertam, geaendertvon);
      db.HH_Buchung.Update(e);
    }
    return e;
  }

  public void Delete(ServiceDaten daten, HhBuchung e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.HH_Buchung.Remove(a);
  }

#pragma warning restore CA1822
}
