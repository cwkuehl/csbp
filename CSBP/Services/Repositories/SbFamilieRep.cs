// <copyright file="SbFamilieRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Services.Pedigree;

  /// <summary>
  /// Klasse für SB_Familie-Repository.
  /// </summary>
  public partial class SbFamilieRep
  {
    /// <summary>
    /// Gets a list of families.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected family uid.</param>
    /// <param name="muid">Affected father uid.</param>
    /// <param name="fuid">Affected mother uid.</param>
    /// <param name="personuid">Affected father or mother uid.</param>
    /// <param name="uidne">Not affected family uid.</param>
    /// <param name="status2">Affected Status2.</param>
    /// <returns>List of families.</returns>
    public List<SbFamilie> GetList(ServiceDaten daten, string uid = null, string muid = null, string fuid = null,
      string personuid = null, string uidne = null, int? status2 = null)
    {
      var mandantnr = daten.MandantNr;
      var db = GetDb(daten);
      var l = db.SB_Familie.Where(a => a.Mandant_Nr == mandantnr);
      if (!string.IsNullOrEmpty(uid))
        l = l.Where(a => a.Uid == uid);
      if (!string.IsNullOrEmpty(uidne))
        l = l.Where(a => a.Uid != uidne);
      if (!string.IsNullOrEmpty(muid))
        l = l.Where(a => a.Mann_Uid == muid);
      if (!string.IsNullOrEmpty(fuid))
        l = l.Where(a => a.Frau_Uid == fuid);
      if (!string.IsNullOrEmpty(personuid))
        l = l.Where(a => a.Mann_Uid == personuid || a.Frau_Uid == personuid);
      if (status2.HasValue)
        l = l.Where(a => a.Status2 == status2.Value);
      var marriage = GedcomEventEnum.MARRIAGE.ToString();
      var fl = l.GroupJoin(db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr && a.Typ == marriage),
        a => new { a.Uid }, b => new { Uid = b.Familie_Uid }, (a, b) => new { family = a, marriage = b })
        .SelectMany(ab => ab.marriage.DefaultIfEmpty(), (a, b) => new { a.family, marriage = b })
        .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == mandantnr),
        a => new { Uid = a.family.Mann_Uid }, b => new { b.Uid }, (a, b) => new { a.family, a.marriage, father = b })
        .SelectMany(ab => ab.father.DefaultIfEmpty(), (a, b) => new { a.family, a.marriage, father = b })
        .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == mandantnr),
        a => new { Uid = a.family.Frau_Uid }, b => new { b.Uid }, (a, b) => new { a.family, a.marriage, a.father, mother = b })
        .SelectMany(ab => ab.mother.DefaultIfEmpty(), (a, b) => new { a.family, a.marriage, a.father, mother = b })
        .ToList()
        .Select(a =>
        {
          if (a.marriage != null)
          {
            a.family.Marriagedate = new PedigreeTimeData(a.marriage).Deparse();
            a.family.Marriageplace = a.marriage.Ort;
            a.family.Marriagememo = a.marriage.Bemerkung;
          }
          if (a.father != null)
            a.family.Father = a.father;
          if (a.mother != null)
            a.family.Mother = a.mother;
          return a.family;
        });
      return fl.OrderBy(a => a.Father?.Geburtsname).ThenBy(a => a.Father?.Vorname)
        .ThenBy(a => a.Mother?.Geburtsname).ThenBy(a => a.Mother?.Vorname).ToList();
    }

    /// <summary>
    /// Updates for column Status2.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected operator for Status1.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <returns>Number of updates.</returns>
    public int UpdateStatus2(ServiceDaten daten, int status2)
    {
      var db = GetDb(daten);
      var l = db.SB_Familie.Where(a => a.Mandant_Nr == daten.MandantNr && a.Status2 != status2);
      var anzahl = 0;
      foreach (var e in l.ToList())
      {
        e.Status2 = status2;
        Update(daten, e);
        anzahl++;
      }
      return anzahl;
    }

    /// <summary>
    /// Updates for column Status2.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected operator for Status1.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <returns>Number of updates.</returns>
    public int UpdateParentStatus2(ServiceDaten daten, int status2)
    {
      var db = GetDb(daten);
      var l = db.SB_Familie.Where(a => a.Mandant_Nr == daten.MandantNr && a.Status2 != status2)
        .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == daten.MandantNr),
        a => new { Uid = a.Mann_Uid }, b => new { b.Uid }, (a, b) => new { family = a, father = b })
        .SelectMany(ab => ab.father.DefaultIfEmpty(), (a, b) => new { a.family, father = b })
        .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == daten.MandantNr),
        a => new { Uid = a.family.Frau_Uid }, b => new { b.Uid }, (a, b) => new { a.family, a.father, mother = b })
        .SelectMany(ab => ab.mother.DefaultIfEmpty(), (a, b) => new { a.family, a.father, mother = b })
        .Where(a => a.father.Status2 == status2 || a.mother.Status2 == status2).Select(a => a.family);
      var anzahl = 0;
      foreach (var e in l.ToList())
      {
        e.Status2 = status2;
        Update(daten, e);
        anzahl++;
      }
      return anzahl;
    }
  }
}
