// <copyright file="AdAdresseRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories
{
  using System;
  using System.Linq;
  using CSBP.Apis.Services;

  /// <summary>
  /// Klasse für AD_Adresse-Repository.
  /// </summary>
  public partial class AdAdresseRep
  {
    /// <summary>
    /// Gets Uid of an address.
    /// </summary>
    /// <returns>Uid of an address or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="state">Affected state.</param>
    /// <param name="postalcode">Affected postal code.</param>
    /// <param name="town">Affected town.</param>
    /// <param name="street">Affected street.</param>
    /// <param name="no">Affected street number.</param>
    /// <param name="createdby">Affected creation user.</param>
    /// <param name="createdat">Affected creation time.</param>
    /// <param name="changedby">Affected changing user.</param>
    /// <param name="changedat">Affected changing time.</param>
    public string GetUid(ServiceDaten daten, string state, string postalcode, string town,
        string street, string no, string createdby, DateTime? createdat, string changedby, DateTime? changedat)
    {
      var db = GetDb(daten);
      var l = db.AD_Adresse.Where(a => a.Mandant_Nr == daten.MandantNr);
      if (string.IsNullOrEmpty(state))
        l = l.Where(a => string.IsNullOrEmpty(a.Staat));
      else
        l = l.Where(a => a.Staat == state);
      if (string.IsNullOrEmpty(postalcode))
        l = l.Where(a => string.IsNullOrEmpty(a.Plz));
      else
        l = l.Where(a => a.Plz == postalcode);
      if (string.IsNullOrEmpty(town))
        l = l.Where(a => string.IsNullOrEmpty(a.Ort));
      else
        l = l.Where(a => a.Ort == town);
      if (string.IsNullOrEmpty(street))
        l = l.Where(a => string.IsNullOrEmpty(a.Strasse));
      else
        l = l.Where(a => a.Strasse == street);
      if (string.IsNullOrEmpty(no))
        l = l.Where(a => string.IsNullOrEmpty(a.HausNr));
      else
        l = l.Where(a => a.HausNr == no);
      if (string.IsNullOrEmpty(createdby))
        l = l.Where(a => string.IsNullOrEmpty(a.Angelegt_Von));
      else
        l = l.Where(a => a.Angelegt_Von == createdby);
      if (createdat.HasValue)
        l = l.Where(a => a.Angelegt_Am == createdat.Value);
      else
        l = l.Where(a => a.Angelegt_Am == null);
      if (string.IsNullOrEmpty(changedby))
        l = l.Where(a => string.IsNullOrEmpty(a.Geaendert_Von));
      else
        l = l.Where(a => a.Geaendert_Von == changedby);
      if (changedat.HasValue)
        l = l.Where(a => a.Geaendert_Am == changedat.Value);
      else
        l = l.Where(a => a.Geaendert_Am == null);
      return l.FirstOrDefault()?.Uid;
    }
  }
}
