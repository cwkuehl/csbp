// <copyright file="SbQuelleRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für SB_Quelle-Repository.
  /// </summary>
  public partial class SbQuelleRep : RepositoryBase
  {
    public SbQuelle Get(ServiceDaten daten, SbQuelle e)
    {
      var db = GetDb(daten);
      var b = db.SB_Quelle.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
      return b;
    }

    public SbQuelle Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.SB_Quelle.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<SbQuelle> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.SB_Quelle.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, SbQuelle e)
    {
      var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
      MachAngelegt(e, daten);
      db.SB_Quelle.Add(e);
    }

    public void Update(ServiceDaten daten, SbQuelle e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.SB_Quelle.Update(a);
      }
    }

    public SbQuelle Save(ServiceDaten daten, int mandantnr, string uid, string beschreibung, string zitat, string bemerkung, string autor, int status1, int status2, int status3, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
    {
      var db = GetDb(daten);
      var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
      var e = a ?? new SbQuelle();
      e.Mandant_Nr = mandantnr;
      e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
      e.Beschreibung = beschreibung;
      e.Zitat = zitat;
      e.Bemerkung = bemerkung;
      e.Autor = autor;
      e.Status1 = status1;
      e.Status2 = status2;
      e.Status3 = status3;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.SB_Quelle.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.SB_Quelle.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, SbQuelle e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.SB_Quelle.Remove(a);
    }
  }
}
