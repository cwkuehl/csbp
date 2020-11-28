// <copyright file="FzBuchautorRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für FZ_Buchautor-Repository.
  /// </summary>
  public partial class FzBuchautorRep : RepositoryBase
  {
    public FzBuchautor Get(ServiceDaten daten, FzBuchautor e)
    {
      var db = GetDb(daten);
      var b = db.FZ_Buchautor.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
      return b;
    }

    public FzBuchautor Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.FZ_Buchautor.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<FzBuchautor> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.FZ_Buchautor.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, FzBuchautor e)
    {
      var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
      MachAngelegt(e, daten);
      db.FZ_Buchautor.Add(e);
    }

    public void Update(ServiceDaten daten, FzBuchautor e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.FZ_Buchautor.Update(a);
      }
    }

    public FzBuchautor Save(ServiceDaten daten, int mandantnr, string uid, string name, string vorname, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
    {
      var db = GetDb(daten);
      var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
      var e = a ?? new FzBuchautor();
      e.Mandant_Nr = mandantnr;
      e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
      e.Name = name;
      e.Vorname = vorname;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.FZ_Buchautor.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.FZ_Buchautor.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, FzBuchautor e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.FZ_Buchautor.Remove(a);
    }
  }
}
