// <copyright file="SbEreignisRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;

/// <summary>
/// Klasse für SB_Ereignis-Repository.
/// </summary>
public partial class SbEreignisRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of events.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ancestor uid.</param>
  /// <param name="fuid">Affected family uid.</param>
  /// <param name="type">Affected event type.</param>
  /// <param name="suid">Affected source uid.</param>
  /// <returns>List of events.</returns>
  public List<SbEreignis> GetList(ServiceDaten daten, string uid = null, string fuid = null, string type = null,
    string suid = null)
  {
    var mandantnr = daten.MandantNr;
    var db = GetDb(daten);
    var l = db.SB_Ereignis.Where(a => a.Mandant_Nr == mandantnr);
    if (!string.IsNullOrEmpty(uid))
      l = l.Where(a => a.Person_Uid == uid);
    if (!string.IsNullOrEmpty(fuid))
      l = l.Where(a => a.Familie_Uid == fuid);
    if (!string.IsNullOrEmpty(type))
      l = l.Where(a => a.Typ == type);
    if (!string.IsNullOrEmpty(suid))
      l = l.Where(a => a.Quelle_Uid == suid);
    return l.OrderBy(a => a.Person_Uid).ThenBy(a => a.Familie_Uid).ThenBy(a => a.Typ).ToList();
  }

#pragma warning restore CA1822
}
