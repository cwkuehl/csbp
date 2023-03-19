// <copyright file="IDiaryService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;

/// <summary>
/// Interface for diary service.
/// </summary>
public interface IDiaryService
{
  /// <summary>
  /// Gets the diary entry of a date.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="pos">With Positions or not.</param>
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
  /// Gets bool array of a month if there is an entry.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected month.</param>
  /// <returns>Bool array of a month.</returns>
  ServiceErgebnis<bool[]> GetMonth(ServiceDaten daten, DateTime date);

  /// <summary>
  /// Searches the next fitting entry in search direction.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stelle">Affected search direction.</param>
  /// <param name="aktDatum">Starting point of search.</param>
  /// <param name="search">Affected search string, possibly with placeholders, e.g. %B_den% finds Baden and Boden.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>Next fitting date.</returns>
  ServiceErgebnis<DateTime?> SearchDate(ServiceDaten daten, SearchDirectionEnum stelle,
    DateTime? aktDatum, string[] search, string puid, DateTime? from, DateTime? to);

  /// <summary>
  /// Create list of lines which contains all diary entries fitting the search string.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="search">Affected search string, possibly with placeholders, e.g. %B_den% finds Baden and Boden.
  /// To check counters use ####, e.g. %####. BGS: %.</param>
  /// <param name="puid">Affected position uid.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <returns>String array which can be stored in a text file.</returns>
  ServiceErgebnis<List<string>> GetDiaryReport(ServiceDaten daten, string[] search, string puid, DateTime? from, DateTime? to);

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
  /// <param name="tz">Affected timezone.</param>
  /// <param name="memo">Affected memos.</param>
  /// <returns>Saved entity.</returns>
  ServiceErgebnis<TbOrt> SavePosition(ServiceDaten daten, string uid, string desc, string lat, string lon, string alt, string tz, string memo);

  /// <summary>
  /// Deletes a position.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeletePosition(ServiceDaten daten, TbOrt e);

  /// <summary>
  /// Gets a list of timezones.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of timezones.</returns>
  ServiceErgebnis<List<MaParameter>> GetTimezoneList(ServiceDaten daten);
}
