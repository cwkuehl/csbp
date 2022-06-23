// <copyright file="FzBuchserieRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table FZ_Buchserie.
/// </summary>
public partial class FzBuchserieRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of series.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected name.</param>
  /// <returns>List of series.</returns>
  public List<FzBuchserie> GetList(ServiceDaten daten, string name)
  {
    var db = GetDb(daten);
    var l = db.FZ_Buchserie.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (Functions.IsLike(name))
      l = l.Where(a => EF.Functions.Like(a.Name, name));
    return l.OrderBy(a => a.Name).ToList();
  }

#pragma warning restore CA1822
}
