// <copyright file="IDiaryService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;

  public interface IDiaryService
  {
    /// <summary>
    /// Gets the diary entry of a date.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="pos">With Positions?</param>
    /// <returns>The entry.</returns>
    ServiceErgebnis<TbEintrag> GetEntry(ServiceDaten daten, DateTime date, bool pos = false);

    /// <summary>
    /// Saves the diary entry.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="entry">Affected Entry.</param>
    /// <param name="pos">Affected list of positions.</param>
    /// <returns>Possible errors.</returns>
    ServiceErgebnis SaveEntry(ServiceDaten daten, DateTime date, string entry, List<Tuple<string, DateTime, DateTime>> pos);

    /// <summary>
    /// Gets Bool-Array of a month if there is an entry.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected month.</param>
    /// <returns>Bool-Array of a month.</returns>
    ServiceErgebnis<bool[]> GetMonth(ServiceDaten daten, DateTime date);

    /// <summary>
    /// Suche des nächsten passenden Eintrags in der Suchrichtung.
    /// </summary>
    /// <returns>Datum des passenden Eintrags.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="stelle">Gewünschte Such-Richtung.</param>
    /// <param name="aktDatum">Aufsetzpunkt der Suche.</param>
    /// <param name="suche">Such-Strings, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.</param>
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected from date.</param>
    ServiceErgebnis<DateTime?> SearchDate(ServiceDaten daten, SearchDirectionEnum stelle,
      DateTime? aktDatum, string[] suche, string puid, DateTime? from, DateTime? to);

    /// <summary>
    /// Erzeugung einer Datei, die alle Tagebuch-Einträge enthält, die dem Such-String entsprechen.
    /// </summary>
    /// <returns>String-Array, das in einer Datei gespeichert werden kann.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="suche">Such-String, evtl. mit Platzhalter, z.B. %B_den% findet Baden und Boden.
    /// Bei der Suche kann auch ein Zähler geprüft werden, z.B. %####. BGS: %</param>
    /// <param name="puid">Affected position uid.</param>
    /// <param name="from">Affected from date.</param>
    /// <param name="to">Affected from date.</param>
    ServiceErgebnis<List<string>> GetDiaryReport(ServiceDaten daten, string[] suche, string puid, DateTime? from, DateTime? to);

    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected position uid.</param>
    /// <returns>The position.</returns>
    ServiceErgebnis<TbOrt> GetPosition(ServiceDaten daten, string uid);

    /// <summary>
    /// Gets a list of positions.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="puid">Affected position ID.</param>
    /// <param name="text">Affected text.</param>
    /// <returns>List of positions.</returns>
    ServiceErgebnis<List<TbOrt>> GetPositionList(ServiceDaten daten, string puid = null, string text = null);

    /// <summary>
    /// Saves the position.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected position uid.</param>
    /// <param name="desc">Affected description.</param>
    /// <param name="lat">Affected latitude.</param>
    /// <param name="lon">Affected longitude.</param>
    /// <param name="alt">Affected altitude.</param>
    /// <param name="memo">Affected memos.</param>
    /// <returns>Saved entity.</returns>
    ServiceErgebnis<TbOrt> SavePosition(ServiceDaten daten, string uid, string desc, string lat, string lon, string alt, string memo);

    /// <summary>
    /// Deletes a position.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeletePosition(ServiceDaten daten, TbOrt e);
  }
}
