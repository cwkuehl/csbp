// <copyright file="FzNotizRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table FZ_Notiz.
/// </summary>
public partial class FzNotizRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list of memos.
  /// </summary>
  /// <returns>List of memos.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="text">Affected posting text.</param>
  public List<FzNotiz> GetList(ServiceDaten daten, string text = null)
  {
    var db = GetDb(daten);
    var l = db.FZ_Notiz.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (Functions.IsLike(text))
      l = l.Where(a => EF.Functions.Like(a.Thema, text) || EF.Functions.Like(a.Notiz, text));
    return l.ToList();
  }

#pragma warning restore CA1822
}
