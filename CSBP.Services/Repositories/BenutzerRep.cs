// <copyright file="BenutzerRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Repository class for table Benutzer.
/// </summary>
public partial class BenutzerRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets list or users.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="nr">Affected user number or 0.</param>
  /// <param name="id">Affected user id.</param>
  /// <param name="nrne">User id which has to be omitted.</param>
  /// <returns>List of users.</returns>
  public List<Benutzer> GetList(ServiceDaten daten, TableReadModel rm, int nr, string id, int nrne = 0)
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
    if (rm != null && !string.IsNullOrEmpty(rm.SortColumn))
    {
      if (CsbpBase.IsLike(rm.Search))
      {
        l = l.Where(a => EF.Functions.Like(a.Benutzer_ID, rm.Search));
      }
      if (!rm.NoPaging)
      {
        rm.PageCount = rm.RowsPerPage == 0 ? 1 : (int)Math.Ceiling(l.Count() / (decimal)(rm.RowsPerPage ?? 0));
        var l1 = SortList(l, rm.SortColumn);
        var page = Math.Max(1, rm.SelectedPage ?? 1) - 1;
        var rowsPerPage = Math.Max(1, rm.RowsPerPage ?? 1);
        var l2 = l1.Skip(page * rowsPerPage).Take(rowsPerPage).ToList();
        return l2;
      }
    }
    return l.OrderBy(a => a.Benutzer_ID).ToList();
  }
#pragma warning restore CA1822
}
