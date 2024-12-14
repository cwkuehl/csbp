// <copyright file="AgDialogRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

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
  /// <param name="uid">Affected uid.</param>
  /// <param name="search">Affected search string.</param>
  /// <param name="d">Affected date.</param>
  /// <param name="ordernr">Ordering descending by nr wished.</param>
  /// <returns>List of entities.</returns>
  public List<AgDialog> GetList(ServiceDaten daten, string api, string uid = null, string search = null,
    DateTime? d = null, bool ordernr = false)
  {
    var db = GetDb(daten);
    var l = db.AG_Dialog.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(uid))
      l = l.Where(a => a.Uid == uid);
    if (!string.IsNullOrEmpty(api))
      l = l.Where(a => a.Api == api);
    if (CsbpBase.IsLike(search))
      l = l.Where(a => EF.Functions.Like(a.Frage, search) || EF.Functions.Like(a.Antwort, search));
    if (d.HasValue)
      l = l.Where(a => a.Datum == d.Value);
    if (ordernr)
      return l.OrderByDescending(a => a.Nr).ToList();
    return l.OrderByDescending(a => a.Datum).ThenByDescending(a => a.Nr).ToList();
  }

#pragma warning restore CA1822
}
