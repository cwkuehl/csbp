// <copyright file="FzFahrradstandRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für FZ_Fahrradstand-Repository.
  /// </summary>
  public partial class FzFahrradstandRep : RepositoryBase
  {
    public FzFahrradstand Get(ServiceDaten daten, FzFahrradstand e)
    {
      var db = GetDb(daten);
      var b = db.FZ_Fahrradstand.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Fahrrad_Uid == e.Fahrrad_Uid && a.Datum == e.Datum && a.Nr == e.Nr);
      return b;
    }

    public FzFahrradstand Get(ServiceDaten daten, int mandantnr, string fahrraduid, DateTime datum, int nr, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.FZ_Fahrradstand.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Fahrrad_Uid == fahrraduid && a.Datum == datum && a.Nr == nr);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<FzFahrradstand> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.FZ_Fahrradstand.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, FzFahrradstand e)
    {
      var db = GetDb(daten);
      MachAngelegt(e, daten);
      db.FZ_Fahrradstand.Add(e);
    }

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

    public FzFahrradstand Save(ServiceDaten daten, int mandantnr, string fahrraduid, DateTime datum, int nr, decimal zaehlerkm, decimal periodekm, decimal periodeschnitt, string beschreibung, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
    {
      var db = GetDb(daten);
      var a = Get(daten, mandantnr, fahrraduid, datum, nr);
      var e = a ?? new FzFahrradstand();
      e.Mandant_Nr = mandantnr;
      e.Fahrrad_Uid = fahrraduid;
      e.Datum = datum;
      e.Nr = nr;
      e.Zaehler_km = zaehlerkm;
      e.Periode_km = periodekm;
      e.Periode_Schnitt = periodeschnitt;
      e.Beschreibung = beschreibung;
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

    public void Delete(ServiceDaten daten, FzFahrradstand e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.FZ_Fahrradstand.Remove(a);
    }
  }
}
