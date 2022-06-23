// <copyright file="SbQuelleRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;

/// <summary>
/// Repository class for table SB_Quelle.
/// </summary>
public partial class SbQuelleRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of sources.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected source uid.</param>
  /// <param name="status2">Affected Status2.</param>
  /// <returns>List of sources.</returns>
  public List<SbQuelle> GetList(ServiceDaten daten, string uid = null, int? status2 = null)
  {
    var mandantnr = daten.MandantNr;
    var db = GetDb(daten);
    var l = db.SB_Quelle.Where(a => a.Mandant_Nr == mandantnr);
    if (!string.IsNullOrEmpty(uid))
      l = l.Where(a => a.Uid == uid);
    if (status2.HasValue)
      l = l.Where(a => a.Status2 == status2.Value);
    return l.OrderBy(a => a.Autor).ThenBy(a => a.Uid).ToList();
  }

  /// <summary>
  /// Updates for column Status2.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="state2">Affected operator for column Status2.</param>
  /// <returns>Number of updates.</returns>
  public int UpdateStatus2(ServiceDaten daten, int state2)
  {
    var db = GetDb(daten);
    var l = db.SB_Quelle.Where(a => a.Mandant_Nr == daten.MandantNr && a.Status2 != state2);
    var anzahl = 0;
    foreach (var e in l.ToList())
    {
      e.Status2 = state2;
      //// Update(daten, e);
      anzahl++;
    }
    return anzahl;
  }

  /// <summary>
  /// Updates for column Status2.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="state2">Affected value of column Status2.</param>
  /// <returns>Number of updates.</returns>
  public int UpdatePersonStatus2(ServiceDaten daten, int state2)
  {
    var db = GetDb(daten);
    var l = db.SB_Quelle.Where(a => a.Mandant_Nr == daten.MandantNr && a.Status2 != state2)
      .GroupJoin(db.SB_Person.Where(a => a.Mandant_Nr == daten.MandantNr),
      a => new { a.Uid }, b => new { Uid = b.Quelle_Uid }, (a, b) => new { source = a, person = b })
      .SelectMany(ab => ab.person.DefaultIfEmpty(), (a, b) => new { a.source, person = b })
      .Where(a => a.person.Status2 == state2).Select(a => a.source);
    var anzahl = 0;
    foreach (var e in l.ToList())
    {
      e.Status2 = state2;
      //// Update(daten, e);
      anzahl++;
    }
    return anzahl;
  }

#pragma warning restore CA1822
}
