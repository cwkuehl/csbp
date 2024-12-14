// <copyright file="FzBuchautorRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table FZ_Buchautor.
/// </summary>
public partial class FzBuchautorRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of authors.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected name.</param>
  /// <returns>List of authors.</returns>
  public List<FzBuchautor> GetList(ServiceDaten daten, string name)
  {
    var db = GetDb(daten);
    var l = db.FZ_Buchautor.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (CsbpBase.IsLike(name))
      l = l.Where(a => EF.Functions.Like(a.Name, name) || EF.Functions.Like(a.Vorname, name));
    return l.OrderBy(a => a.Name).ThenBy(a => a.Vorname).ToList();
  }

#pragma warning restore CA1822
}
