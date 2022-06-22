// <copyright file="BenutzerRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;

/// <summary>
/// Klasse für Benutzer-Repository.
/// </summary>
public partial class BenutzerRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list or users.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected user number or 0.</param>
  /// <param name="id">Affected user id.</param>
  /// <param name="nrne">User id which has to be omitted.</param>
  /// <returns>List of users.</returns>
  public List<Benutzer> GetList(ServiceDaten daten, int nr, string id, int nrne = 0)
  {
    var db = GetDb(daten);
    var l = db.Benutzer.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (nr > 0)
      l = l.Where(a => a.Person_Nr == nr);
    if (nr < 0)
      l = l.Where(a => a.Benutzer_ID == daten.BenutzerId);
    if (nrne > 0)
      l = l.Where(a => a.Person_Nr != nrne);
    if (!string.IsNullOrEmpty(id))
      l = l.Where(a => a.Benutzer_ID == id);
    return l.OrderBy(a => a.Benutzer_ID).ToList();
  }
#pragma warning restore CA1822
}
