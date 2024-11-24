// <copyright file="ByteDatenRep.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories;

using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;

/// <summary>
/// Repository class for table Byte_Daten.
/// </summary>
public partial class ByteDatenRep
{
#pragma warning disable CA1822

  /// <summary>
  /// Gets a list of bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="uid">Affected uid.</param>
  /// <returns>List of bytes.</returns>
  public List<ByteDaten> GetList(ServiceDaten daten, string type = null, string uid = null)
  {
    var db = GetDb(daten);
    var l = db.Byte_Daten.Where(a => a.Mandant_Nr == daten.MandantNr);
    if (!string.IsNullOrEmpty(type))
      l = l.Where(a => a.Typ == type);
    if (!string.IsNullOrEmpty(uid))
      l = l.Where(a => a.Uid == uid);
    return l.OrderBy(a => a.Typ).ThenBy(a => a.Uid).ToList();
  }

  /// <summary>
  /// Gets a list of bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="uid">Affected uid.</param>
  /// <param name="byteliste">Affected list of bytes.</param>
  public void SaveList(ServiceDaten daten, string type, string uid, List<ByteDaten> byteliste)
  {
    var liste = GetList(daten, type, uid);
    var anzahl = byteliste?.Count ?? 0;
    for (var nr = 1; nr <= anzahl; nr++)
    {
      var bdn = byteliste[nr - 1];
      var bds = liste.FirstOrDefault(a => a.Lfd_Nr == nr);
      if (bds == null)
      {
        var bd = new ByteDaten
        {
          Mandant_Nr = daten.MandantNr,
          Typ = type,
          Uid = uid,
          Lfd_Nr = nr,
          Metadaten = bdn.Metadaten,
          Bytes = bdn.Bytes,
        };
        Insert(daten, bd);
      }
      else
      {
        bds.Metadaten = bdn.Metadaten;
        bds.Bytes = bdn.Bytes;
        Update(daten, bds);
        liste.Remove(bds);
      }

      // restliche ByteDaten löschen
      foreach (var bd in liste)
      {
        Delete(daten, bd);
      }
    }
  }

#pragma warning disable CA1822
}
