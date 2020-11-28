// <copyright file="HhBilanzRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für HH_Bilanz-Repository.
  /// </summary>
  public partial class HhBilanzRep : RepositoryBase
  {
    public HhBilanz Get(ServiceDaten daten, HhBilanz e)
    {
      var db = GetDb(daten);
      var b = db.HH_Bilanz.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Periode == e.Periode && a.Kz == e.Kz && a.Konto_Uid == e.Konto_Uid);
      return b;
    }

    public HhBilanz Get(ServiceDaten daten, int mandantnr, int periode, string kz, string kontouid, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.HH_Bilanz.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Periode == periode && a.Kz == kz && a.Konto_Uid == kontouid);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<HhBilanz> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.HH_Bilanz.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, HhBilanz e)
    {
      var db = GetDb(daten);
      MachAngelegt(e, daten);
      db.HH_Bilanz.Add(e);
    }

    public void Update(ServiceDaten daten, HhBilanz e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.HH_Bilanz.Update(a);
      }
    }

    public HhBilanz Save(ServiceDaten daten, int mandantnr, int periode, string kz, string kontouid, string sh, decimal betrag, string esh, decimal ebetrag, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
    {
      var db = GetDb(daten);
      var a = Get(daten, mandantnr, periode, kz, kontouid);
      var e = a ?? new HhBilanz();
      e.Mandant_Nr = mandantnr;
      e.Periode = periode;
      e.Kz = kz;
      e.Konto_Uid = kontouid;
      e.SH = sh;
      e.Betrag = betrag;
      e.ESH = esh;
      e.EBetrag = ebetrag;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.HH_Bilanz.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.HH_Bilanz.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, HhBilanz e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.HH_Bilanz.Remove(a);
    }
  }
}
