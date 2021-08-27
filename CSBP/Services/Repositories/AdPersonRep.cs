// <copyright file="AdPersonRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;

  /// <summary>
  /// Klasse für AD_Person-Repository.
  /// </summary>
  public partial class AdPersonRep
  {
    /// <summary>
    /// Gets birthday list with persons.
    /// </summary>
    /// <returns>Birthday list with persons.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="from">Minimum birthday.</param>
    /// <param name="to">Maximum birthday.</param>
    public List<AdPerson> GetList(ServiceDaten daten, DateTime from, DateTime to)
    {
      var f = from.Month * 100 + from.Day;
      var t = to.Month * 100 + to.Day;
      var db = GetDb(daten);
      var pl = db.AD_Person.Where(a => a.Mandant_Nr == daten.MandantNr && a.Person_Status == 0
          && a.GeburtK != 0);
      if (to.Year == from.Year)
        pl = pl.Where(a => a.GeburtK >= f && a.GeburtK <= t);
      else
        pl = pl.Where(a => a.GeburtK < t || a.GeburtK > f);
      return pl.OrderBy(a => a.GeburtK).ThenBy(a => a.Name1).ThenBy(a => a.Vorname)
          .ThenBy(a => a.Uid).ToList();
    }

    /// <summary>
    /// Gets Uid of a person.
    /// </summary>
    /// <returns>Uid of a person or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="gender">Affected gender.</param>
    /// <param name="birthday">Affected birthday.</param>
    /// <param name="name1">Affected name 1.</param>
    /// <param name="name2">Affected name 2.</param>
    /// <param name="predicate">Affected predicate.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <param name="title">Affected title.</param>
    /// <param name="createdby">Affected creation user.</param>
    /// <param name="createdat">Affected creation time.</param>
    public string GetUid(ServiceDaten daten, string gender, DateTime? birthday, string name1, string name2,
        string predicate, string firstname, string title, string createdby, DateTime? createdat)
    {
      var db = GetDb(daten);
      var l = db.AD_Person.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (!string.IsNullOrEmpty(gender))
        l = l.Where(a => a.Geschlecht == gender);
      if (birthday.HasValue)
        l = l.Where(a => a.Geburt == birthday.Value);
      if (!string.IsNullOrEmpty(name1))
        l = l.Where(a => a.Name1 == name1);
      if (!string.IsNullOrEmpty(name2))
        l = l.Where(a => a.Name2 == name2);
      if (!string.IsNullOrEmpty(predicate))
        l = l.Where(a => a.Praedikat == predicate);
      if (!string.IsNullOrEmpty(firstname))
        l = l.Where(a => a.Vorname == firstname);
      if (!string.IsNullOrEmpty(title))
        l = l.Where(a => a.Titel == title);
      if (!string.IsNullOrEmpty(createdby))
        l = l.Where(a => a.Angelegt_Von == createdby);
      if (createdat.HasValue)
        l = l.Where(a => a.Angelegt_Am == createdat.Value);
      return l.FirstOrDefault()?.Uid;
    }
  }
}
