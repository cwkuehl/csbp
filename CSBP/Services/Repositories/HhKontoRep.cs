// <copyright file="HhKontoRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Klasse für HH_Konto-Repository.
  /// </summary>
  public partial class HhKontoRep
  {
    /// <summary>
    /// Gets list of accounts.
    /// </summary>
    /// <returns>List of accounts.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nrle">Affected minimum period number.</param>
    /// <param name="nrge">Affected maximum period number.</param>
    /// <param name="art1">Affected first account type.</param>
    /// <param name="art2">Affected second account type.</param>
    /// <param name="dle">Affected minimum period date.</param>
    /// <param name="dge">Affected maximum period date.</param>
    /// <param name="text">Affected text.</param>
    public List<HhKonto> GetList(ServiceDaten daten, int nrle, int nrge, string art1 = null, string art2 = null,
        DateTime? dle = null, DateTime? dge = null, string text = null)
    {
      var db = GetDb(daten);
      var l = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (nrle >= 0)
        l = l.Where(a => a.Periode_Von <= nrle);
      if (nrge >= 0)
        l = l.Where(a => a.Periode_Bis >= nrge);
      if (!string.IsNullOrEmpty(art1) && !string.IsNullOrEmpty(art2))
        l = l.Where(a => a.Art == art1 || a.Art == art2);
      else if (!string.IsNullOrEmpty(art1))
        l = l.Where(a => a.Art == art1);
      if (dle.HasValue)
        l = l.Where(a => a.Gueltig_Von == null || a.Gueltig_Von <= dle.Value);
      if (dge.HasValue)
        l = l.Where(a => a.Gueltig_Bis == null || a.Gueltig_Bis >= dge.Value);
      if (Functions.IsLike(text))
        l = l.Where(a => EF.Functions.Like(a.Uid, text) || EF.Functions.Like(a.Name, text));
      return l.OrderBy(a => a.Mandant_Nr).ThenBy(a => a.Name).ThenBy(a => a.Uid).ToList();
    }

    /// <summary>
    /// Gets an account.
    /// </summary>
    /// <returns>Account or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auidne">Affected ID which is not equal.</param>
    /// <param name="attr">Affected attribute.</param>
    /// <param name="sort">Affected sotring.</param>
    /// <param name="desc">Affected description.</param>
    public HhKonto GetMin(ServiceDaten daten, string auidne, string attr = null, string sort = null, string desc = null)
    {
      var db = GetDb(daten);
      var l = db.HH_Konto.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (!string.IsNullOrEmpty(auidne))
        l = l.Where(a => a.Uid != auidne);
      if (!string.IsNullOrEmpty(attr))
        l = l.Where(a => a.Kz == attr);
      if (!string.IsNullOrEmpty(sort))
        l = l.Where(a => a.Sortierung == sort);
      if (!string.IsNullOrEmpty(desc))
        l = l.Where(a => a.Name == desc);
      return l.OrderBy(a => a.Mandant_Nr).ThenBy(a => a.Uid).FirstOrDefault();
    }
  }
}
