// <copyright file="SbKindRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;

  /// <summary>
  /// Klasse für SB_Kind-Repository.
  /// </summary>
  public partial class SbKindRep
  {
    /// <summary>
    /// Gets a list of children.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="fuid">Affected family uid.</param>
    /// <param name="kuid">Affected child uid.</param>
    /// <param name="fuidne">Not affected family uid.</param>
    /// <param name="kuidne">Not affected child uid.</param>
    /// <param name="kuidgt">Affected child uid which is smaller.</param>
    /// <param name="personuid">Affected father or mother uid.</param>
    /// <param name="status2">Affected Status2.</param>
    /// <param name="status3">Affected Status3.</param>
    /// <returns>List of children.</returns>
    public List<SbKind> GetList(ServiceDaten daten, string fuid = null, string kuid = null, string fuidne = null,
      string kuidne = null, string kuidgt = null, string personuid = null, int? status2 = null, int? status3 = null)
    {
      var mandantnr = daten.MandantNr;
      var db = GetDb(daten);
      var l = db.SB_Kind.Where(a => a.Mandant_Nr == mandantnr);
      if (!string.IsNullOrEmpty(fuid))
        l = l.Where(a => a.Familie_Uid == fuid);
      if (!string.IsNullOrEmpty(fuidne))
        l = l.Where(a => a.Familie_Uid != fuidne);
      if (!string.IsNullOrEmpty(kuid))
        l = l.Where(a => a.Kind_Uid == kuid);
      if (!string.IsNullOrEmpty(kuidne))
        l = l.Where(a => a.Kind_Uid != kuidne);
      if (!string.IsNullOrEmpty(kuidgt))
        l = l.Where(a => string.Compare(a.Kind_Uid, kuidgt) > 0);
      if (!string.IsNullOrEmpty(personuid) || status2.HasValue)
      {
        var fl = db.SB_Familie.Where(a => a.Mandant_Nr == mandantnr);
        if (!string.IsNullOrEmpty(personuid))
          fl = fl.Where(a => a.Mann_Uid == personuid || a.Frau_Uid == personuid);
        if (status2.HasValue)
          fl = fl.Where(a => a.Status2 == status2.Value);
        if (status3.HasValue)
          fl = fl.Where(a => a.Status3 == status3.Value);
        l = l.Join(fl, a => new { Uid = a.Familie_Uid }, b => new { b.Uid }, (a, b) => a);
      }
      var l2 = l.Join(db.SB_Person.Where(a => a.Mandant_Nr == mandantnr),
        a => new { Uid = a.Kind_Uid }, b => new { b.Uid }, (a, b) => new { kind = a, child = b })
        .ToList()
        .Select(a =>
        {
          if (a.child != null)
            a.kind.Child = a.child;
          return a.kind;
        });
      return l2.OrderBy(a => a.Familie_Uid).ThenBy(a => a.Kind_Uid).ToList();
    }
  }
}
