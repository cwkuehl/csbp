// <copyright file="TbEintragRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für TB_Eintrag-Repository.
  /// </summary>
  public partial class TbEintragRep : RepositoryBase
  {
    public TbEintrag Get(ServiceDaten daten, TbEintrag e)
    {
      var db = GetDb(daten);
      var b = db.TB_Eintrag.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Datum == e.Datum);
      return b;
    }

    public TbEintrag Get(ServiceDaten daten, int mandantnr, DateTime datum, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.TB_Eintrag.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Datum == datum);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<TbEintrag> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.TB_Eintrag.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, TbEintrag e)
    {
      var db = GetDb(daten);
      MachAngelegt(e, daten);
      db.TB_Eintrag.Add(e);
    }

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

    public TbEintrag Save(ServiceDaten daten, int mandantnr, DateTime datum, string eintrag, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null, string replikationuid = null)
    {
      var db = GetDb(daten);
      var a = Get(daten, mandantnr, datum);
      var e = a ?? new TbEintrag();
      e.Mandant_Nr = mandantnr;
      e.Datum = datum;
      e.Eintrag = eintrag;
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

    public void Delete(ServiceDaten daten, TbEintrag e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.TB_Eintrag.Remove(a);
    }
  }
}
