// <copyright file="HhPeriodeRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Repositories.Base;

  /// <summary>
  /// Generierte Basis-Klasse für HH_Periode-Repository.
  /// </summary>
  public partial class HhPeriodeRep : RepositoryBase
  {
    public HhPeriode Get(ServiceDaten daten, HhPeriode e)
    {
      var db = GetDb(daten);
      var b = db.HH_Periode.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Nr == e.Nr);
      return b;
    }

    public HhPeriode Get(ServiceDaten daten, int mandantnr, int nr, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.HH_Periode.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Nr == nr);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<HhPeriode> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.HH_Periode.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, HhPeriode e)
    {
      var db = GetDb(daten);
      MachAngelegt(e, daten);
      db.HH_Periode.Add(e);
    }

    public void Update(ServiceDaten daten, HhPeriode e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.HH_Periode.Update(a);
      }
    }

    public HhPeriode Save(ServiceDaten daten, int mandantnr, int nr, DateTime datumvon, DateTime datumbis, int art, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
    {
      var db = GetDb(daten);
      var a = Get(daten, mandantnr, nr);
      var e = a ?? new HhPeriode();
      e.Mandant_Nr = mandantnr;
      e.Nr = nr;
      e.Datum_Von = datumvon;
      e.Datum_Bis = datumbis;
      e.Art = art;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.HH_Periode.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.HH_Periode.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, HhPeriode e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.HH_Periode.Remove(a);
    }
  }
}
