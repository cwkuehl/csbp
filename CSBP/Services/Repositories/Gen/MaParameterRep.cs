// <copyright file="MaParameterRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für MA_Parameter-Repository.
  /// </summary>
  public partial class MaParameterRep : RepositoryBase
  {
    public MaParameter Get(ServiceDaten daten, MaParameter e)
    {
      var db = GetDb(daten);
      var b = db.MA_Parameter.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Schluessel == e.Schluessel);
      return b;
    }

    public MaParameter Get(ServiceDaten daten, int mandantnr, string schluessel, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.MA_Parameter.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Schluessel == schluessel);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<MaParameter> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.MA_Parameter.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, MaParameter e)
    {
      var db = GetDb(daten);
      MachAngelegt(e, daten);
      db.MA_Parameter.Add(e);
    }

    public void Update(ServiceDaten daten, MaParameter e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.MA_Parameter.Update(a);
      }
    }

    public MaParameter Save(ServiceDaten daten, int mandantnr, string schluessel, string wert, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
    {
      var db = GetDb(daten);
      var a = Get(daten, mandantnr, schluessel);
      var e = a ?? new MaParameter();
      e.Mandant_Nr = mandantnr;
      e.Schluessel = schluessel;
      e.Wert = wert;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.MA_Parameter.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.MA_Parameter.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, MaParameter e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.MA_Parameter.Remove(a);
    }
  }
}
