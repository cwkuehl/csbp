// <copyright file="TbEintragOrtRep.cs" company="cwkuehl.de">
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
/// Generierte Basis-Klasse für TB_Eintrag_Ort-Repository.
/// </summary>
public partial class TbEintragOrtRep : RepositoryBase
{
#pragma warning disable CA1822

  public TbEintragOrt Get(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    var b = db.TB_Eintrag_Ort.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Ort_Uid == e.Ort_Uid && a.Datum_Von == e.Datum_Von && a.Datum_Bis == e.Datum_Bis);
    return b;
  }

  public TbEintragOrt Get(ServiceDaten daten, int mandantnr, string ortuid, DateTime datumvon, DateTime datumbis, bool detached = false)
  {
    var db = GetDb(daten);
    var b = db.TB_Eintrag_Ort.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Ort_Uid == ortuid && a.Datum_Von == datumvon && a.Datum_Bis == datumbis);
    if (detached && b != null)
      db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
    return b;
  }

  public List<TbEintragOrt> GetList(ServiceDaten daten, int mandantnr)
  {
    var db = GetDb(daten);
    var l = db.TB_Eintrag_Ort.Where(a => a.Mandant_Nr == mandantnr);
    return l.ToList();
  }

  public void Insert(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    MachAngelegt(e, daten);
    db.TB_Eintrag_Ort.Add(e);
  }

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

  public void Delete(ServiceDaten daten, TbEintragOrt e)
  {
    var db = GetDb(daten);
    var a = Get(daten, e);
    if (a != null)
      db.TB_Eintrag_Ort.Remove(a);
  }

#pragma warning restore CA1822
}
