// <copyright file="WpBuchungRep.cs" company="cwkuehl.de">
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
  /// Generierte Basis-Klasse für WP_Buchung-Repository.
  /// </summary>
  public partial class WpBuchungRep : RepositoryBase
  {
    public WpBuchung Get(ServiceDaten daten, WpBuchung e)
    {
      var db = GetDb(daten);
      var b = db.WP_Buchung.FirstOrDefault(a => a.Mandant_Nr == e.Mandant_Nr && a.Uid == e.Uid);
      return b;
    }

    public WpBuchung Get(ServiceDaten daten, int mandantnr, string uid, bool detached = false)
    {
      var db = GetDb(daten);
      var b = db.WP_Buchung.FirstOrDefault(a => a.Mandant_Nr == mandantnr && a.Uid == uid);
      if (detached && b != null)
        db.Entry(b).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
      return b;
    }

    public List<WpBuchung> GetList(ServiceDaten daten, int mandantnr)
    {
      var db = GetDb(daten);
      var l = db.WP_Buchung.Where(a => a.Mandant_Nr == mandantnr);
      return l.ToList();
    }

    public void Insert(ServiceDaten daten, WpBuchung e)
    {
      var db = GetDb(daten);
      e.Uid = string.IsNullOrEmpty(e.Uid) ? Functions.GetUid() : e.Uid;
      MachAngelegt(e, daten);
      db.WP_Buchung.Add(e);
    }

    public void Update(ServiceDaten daten, WpBuchung e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      db.Entry(a).CurrentValues.SetValues(e);
      if (db.Entry(a).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        MachGeaendert(a, daten);
        db.WP_Buchung.Update(a);
      }
    }

    public WpBuchung Save(ServiceDaten daten, int mandantnr, string uid, string wertpapieruid, string anlageuid, DateTime datum, decimal zahlungsbetrag, decimal rabattbetrag, decimal anteile, decimal zinsen, string btext, string notiz, string angelegtvon = null, DateTime? angelegtam = null, string geaendertvon = null, DateTime? geaendertam = null)
    {
      var db = GetDb(daten);
      var a = string.IsNullOrEmpty(uid) ? null : Get(daten, mandantnr, uid);
      var e = a ?? new WpBuchung();
      e.Mandant_Nr = mandantnr;
      e.Uid = string.IsNullOrEmpty(uid) ? Functions.GetUid() : uid;
      e.Wertpapier_Uid = wertpapieruid;
      e.Anlage_Uid = anlageuid;
      e.Datum = datum;
      e.Zahlungsbetrag = zahlungsbetrag;
      e.Rabattbetrag = rabattbetrag;
      e.Anteile = anteile;
      e.Zinsen = zinsen;
      e.BText = btext;
      e.Notiz = notiz;
      if (a == null)
      {
        MachAngelegt(e, daten, angelegtam, angelegtvon);
        if (!string.IsNullOrEmpty(geaendertvon))
          MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.WP_Buchung.Add(e);
      }
      else if (db.Entry(e).State == Microsoft.EntityFrameworkCore.EntityState.Modified)
      {
        if (!string.IsNullOrEmpty(angelegtvon))
          MachAngelegt(e, daten, angelegtam, angelegtvon);
        MachGeaendert(e, daten, geaendertam, geaendertvon);
        db.WP_Buchung.Update(e);
      }
      return e;
    }

    public void Delete(ServiceDaten daten, WpBuchung e)
    {
      var db = GetDb(daten);
      var a = Get(daten, e);
      if (a != null)
        db.WP_Buchung.Remove(a);
    }
  }
}
