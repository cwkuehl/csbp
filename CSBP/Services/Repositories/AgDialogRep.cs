// <copyright file="AgDialogRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Apis.Services;

/// <summary>
/// Repository class for table AG_Dialog.
/// </summary>
public partial class AgDialogRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list of entities.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="api">Affected api name.</param>
  /// <param name="d">Affected date.</param>
  /// <param name="ordernr">Ordering descending by nr wished.</param>
  /// <returns>List of entities.</returns>
  public List<AgDialog> GetList(ServiceDaten daten, string api, DateTime? d = null, bool ordernr = false)
  {
    var db = GetDb(daten);
    var l = db.AG_Dialog.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(api))
      l = l.Where(a => a.Api == api);
    if (d.HasValue)
      l = l.Where(a => a.Datum == d.Value);
    if (ordernr)
      return l.OrderBy(a => a.Nr).ToList();
    return l.ToList();
  }

#pragma warning restore CA1822
}
