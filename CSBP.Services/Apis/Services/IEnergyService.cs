// <copyright file="IEnergyService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Services;

using System.Collections.Generic;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;

/// <summary>
/// Interface for energy service.
/// </summary>
public interface IEnergyService
{
  /// <summary>
  /// Returns a CSV file with all data of a form.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="page">Affected page, e.g. "AG100".</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>CSV file as string.</returns>
  ServiceErgebnis<string> GetCsvString(ServiceDaten daten, string page, TableReadModel rm);

  /// <summary>
  /// Gets a list of queries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="inactive">Gets also inactive investments or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of queries.</returns>
  ServiceErgebnis<List<EnAbfrage>> GetQueryList(ServiceDaten daten, TableReadModel rm = null, bool inactive = false, string search = null);

  /// <summary>
  /// Gets a query.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Query or null.</returns>
  ServiceErgebnis<EnAbfrage> GetQuery(ServiceDaten daten, string uid);

  /// <summary>
  /// Saves a query.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="sortierung">Value of column Sortierung.</param>
  /// <param name="art">Value of column Art.</param>
  /// <param name="bezeichnung">Value of column Bezeichnung.</param>
  /// <param name="hosturl">Value of column Host_Url.</param>
  /// <param name="datentyp">Value of column Datentyp.</param>
  /// <param name="schreibbarkeit">Value of column Schreibbarkeit.</param>
  /// <param name="einheit">Value of column Einheit.</param>
  /// <param name="param1">Value of column Param1.</param>
  /// <param name="param2">Value of column Param2.</param>
  /// <param name="param3">Value of column Param3.</param>
  /// <param name="param4">Value of column Param4.</param>
  /// <param name="param5">Value of column Param5.</param>
  /// <param name="status">Value of column Status.</param>
  /// <param name="notiz">Value of column Notiz.</param>
  /// <returns>Created or changed query.</returns>
  ServiceErgebnis<EnAbfrage> SaveQuery(ServiceDaten daten, string uid, string sortierung, string art, string bezeichnung, string hosturl, string datentyp, string schreibbarkeit, string einheit, string param1, string param2, string param3, string param4, string param5, string status, string notiz);

  /// <summary>
  /// Deletes a query.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteQuery(ServiceDaten daten, EnAbfrage e);

  /// <summary>
  /// Queries all queries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="state">State of calculation is always updated, cancelling is possible.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis QueryQueries(ServiceDaten daten, bool inactive, string search, StatusTask state);

  /// <summary>
  /// Gets a list of states.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of states.</returns>
  ServiceErgebnis<List<MaParameter>> GetStateList(ServiceDaten daten);

  /// <summary>
  /// Gets a list of kinds.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of kinds.</returns>
  ServiceErgebnis<List<MaParameter>> GetKindList(ServiceDaten daten);

  /// <summary>
  /// Gets a list of datatypes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of datatypes.</returns>
  ServiceErgebnis<List<MaParameter>> GetDatatypeList(ServiceDaten daten);
}
