// <copyright file="AdAdresseRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für AD_Adresse-Repository.
  /// </summary>
  public partial class AdAdresseRep : RepositoryBase
  {
    public AdAdresse Get(ServiceDaten daten, AdAdresse e)
    {
      var db = GetDb(daten);
      var b = db.AD_Adresse.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
      return b;
    }

    public AdAdresse Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.AD_Adresse.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<AdAdresse> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.AD_Adresse.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, AdAdresse e)
    {
      var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
      MachAngelegt(e, daten);
      db.AD_Adresse.Add(e);
    }

    public void Update(ServiceDaten daten, AdAdresse e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.AD_Adresse.Update(a);
      }
    }

    public AdAdresse Save(ServiceDaten daten, int mandantnr, string uid, string staat, string plz, string ort, string strasse, string hausnr, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
    {
      var db = GetDb(daten);
      var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
      var e = a ?? new AdAdresse();
      e.Mandant_Nr = mandantnr;
      e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
      e.Staat = staat;
      e.Plz = plz;
      e.Ort = ort;
      e.Strasse = strasse;
      e.HausNr = hausnr;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.AD_Adresse.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.AD_Adresse.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, AdAdresse e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.AD_Adresse.Remove(a);
    }
  }
}
