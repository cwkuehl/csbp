﻿// <copyright file="IClientService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using CSBP.Apis.Models;
using CSBP.Apis.Models.Extension;
using CSBP.Base;

namespace CSBP.Apis.Services
{
  public interface IClientService
  {
    /// <summary>
    /// Initializes the database.
    /// </summary>
    /// <returns>Possibly errors.</returns>
    /// <param name="daten">Service data for database access.</param>
    ServiceErgebnis InitDb(ServiceDaten daten);

    /// <summary>
    /// Gets list with clients.
    /// </summary>
    /// <returns>List with clients.</returns>
    /// <param name="daten">Service data for database access.</param>
    ServiceErgebnis<List<MaMandant>> GetClientList(ServiceDaten daten);

    /// <summary>
    /// Gets client by number.
    /// </summary>
    /// <returns>The client.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected client number.</param>
    ServiceErgebnis<MaMandant> GetClient(ServiceDaten daten, int nr);

    /// <summary>
    /// Saves a client.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected client number.</param>
    /// <param name="desc">Affected description.</param>
    /// <returns>Created or changed entity.</returns>
    ServiceErgebnis<MaMandant> SaveClient(ServiceDaten daten, int nr, string desc);

    /// <summary>
    /// Deletes a client.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteClient(ServiceDaten daten, MaMandant e);

    /// <summary>
    /// Gets a list of users.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>List of users.</returns>
    ServiceErgebnis<List<Benutzer>> GetUserList(ServiceDaten daten);

    /// <summary>
    /// Gets user by number.
    /// </summary>
    /// <returns>The user.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected user number.</param>
    ServiceErgebnis<Benutzer> GetUser(ServiceDaten daten, int nr);

    /// <summary>
    /// Saves an user.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected user number.</param>
    /// <param name="id">Affected user ID.</param>
    /// <param name="password">Affected passwordD.</param>
    /// <param name="permission">Affected permission.</param>
    /// <param name="birthday">Affected birthday.</param>
    /// <returns>Created or changed entity.</returns>
    ServiceErgebnis<Benutzer> SaveUser(ServiceDaten daten, int nr, string id, string password,
        int permission, DateTime? birthday);

    /// <summary>
    /// Deletes an user.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteUser(ServiceDaten daten, Benutzer e);

    /// <summary>
    /// Gets a list of options.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="client">Affected client number.</param>
    /// <param name="plist">Affected parameter list.</param>
    /// <returns>List of options.</returns>
    ServiceErgebnis<List<MaParameter>> GetOptionList(ServiceDaten daten, int client, Dictionary<string, Parameter> plist);

    /// <summary>
    /// Saves an option.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="client">Affected client number.</param>
    /// <param name="p">Affected parameter.</param>
    /// <param name="value">New value.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis SaveOption(ServiceDaten daten, int client, Parameter p, string value);

    /// <summary>
    /// Saves a list of options.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="client">Affected client number.</param>
    /// <param name="list">Affected option list.</param>
    /// <param name="p">Affected parameter list.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis SaveOptionList(ServiceDaten daten, int client, List<MaParameter> list, Dictionary<string, Parameter> p);

    /// <summary>
    /// Gets list with backup entries.
    /// </summary>
    /// <returns>List with backup entries.</returns>
    /// <param name="daten">Service data for database access.</param>
    ServiceErgebnis<List<BackupEntry>> GetBackupEntryList(ServiceDaten daten);

    /// <summary>
    /// Gets a backup entry.
    /// </summary>
    /// <returns>Backup entry or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    ServiceErgebnis<BackupEntry> GetBackupEntry(ServiceDaten daten, string uid);

    /// <summary>
    /// Saves backup entry.
    /// </summary>
    /// <returns>List with backup entries.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="target">Affected target.</param>
    /// <param name="sources">Affected sources.</param>
    /// <param name="encrypted">Should the target copy be encrypted?</param>
    /// <param name="zipped">Should the target copy be zipped?</param>
    ServiceErgebnis<BackupEntry> SaveBackupEntry(ServiceDaten daten, string uid,
        string target, string[] sources, bool encrypted, bool zipped);

    /// <summary>
    /// Deletes a backup entry.
    /// </summary>
    /// <returns>Possibly errors.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    ServiceErgebnis DeleteBackupEntry(ServiceDaten daten, BackupEntry e);

    /// <summary>
    /// Makes a backup.
    /// </summary>
    /// <returns>Possibly errors.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID of backup entry.</param>
    /// <param name="restore">Reverse direction of copying?</param>
    /// <param name="password">Password for encryption.</param>
    /// <param name="status">Status of backup is always updated.</param>
    /// <param name="cancel">Cancel backup if not empty.</param>
    ServiceErgebnis MakeBackup(ServiceDaten daten, string uid, bool restore,
        string password, StringBuilder status, StringBuilder cancel);

    /// <summary>
    /// Replicates a table.
    /// </summary>
    /// <returns>Possibly errors.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="table">Affected table.</param>
    /// <param name="mode">Affected mode.</param>
    /// <param name="json">Affected data as json string.</param>
    ServiceErgebnis<string> ReplicateTable(ServiceDaten daten, string table, string mode, string json);
  }
}
