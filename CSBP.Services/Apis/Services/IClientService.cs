﻿// <copyright file="IClientService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Services;

using System;
using System.Collections.Generic;
using System.Text;
using CSBP.Services.Apis.Models;
using CSBP.Services.Apis.Models.Extension;
using CSBP.Services.Base;
using CSBP.Services.NonService;

/// <summary>
/// Interface for client service.
/// </summary>
public interface IClientService
{
  /// <summary>
  /// Initializes the database.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis InitDb(ServiceDaten daten);

  /// <summary>
  /// Returns a CSV file with all data of a form.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="page">Affected page, e.g. "AG100".</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>CSV file as string.</returns>
  ServiceErgebnis<string> GetCsvString(ServiceDaten daten, string page, TableReadModel rm);

  /// <summary>
  /// Gets list with clients.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>List with clients.</returns>
  ServiceErgebnis<List<MaMandant>> GetClientList(ServiceDaten daten, TableReadModel rm);

  /// <summary>
  /// Gets client by number.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="nr">Affected client number.</param>
  /// <returns>Affected client or null.</returns>
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
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>List of users.</returns>
  ServiceErgebnis<List<Benutzer>> GetUserList(ServiceDaten daten, TableReadModel rm = null);

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
  /// Gets option value by primary key.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Value of column Mandant_Nr.</param>
  /// <param name="key">Value of column Schluessel.</param>
  /// <returns>Value or null.</returns>
  ServiceErgebnis<string> GetValue(ServiceDaten daten, int client, string key);

  /// <summary>
  /// Gets a list of options.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="client">Affected client number.</param>
  /// <param name="plist">Affected parameter list.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>List of options.</returns>
  ServiceErgebnis<List<MaParameter>> GetOptionList(ServiceDaten daten, int client, Dictionary<string, Parameter> plist, TableReadModel rm);

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
  /// <param name="encrypted">Should the target copy be encrypted or not.</param>
  /// <param name="zipped">Should the target copy be zipped or not.</param>
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
  /// <param name="restore">Reverse direction of copying or not.</param>
  /// <param name="password">Password for encryption.</param>
  /// <param name="state">State of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  ServiceErgebnis MakeBackup(ServiceDaten daten, string uid, bool restore,
      string password, StringBuilder state, StringBuilder cancel);

  /// <summary>
  /// Replicates a table.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="table">Affected table.</param>
  /// <param name="mode">Affected mode.</param>
  /// <param name="json">Affected data as json string.</param>
  ServiceErgebnis<string> ReplicateTable(ServiceDaten daten, string table, string mode, string json);

  /// <summary>
  /// Get an table report.
  /// </summary>
  /// <returns>An table report.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected table name.</param>
  /// <param name="lines">Affected table data.</param>
  ServiceErgebnis<byte[]> GetTableReport(ServiceDaten daten, string name, List<List<string>> lines);

  /// <summary>
  /// Commit a new file to the undo stack.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected file name with path.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis CommitFile(ServiceDaten daten, string name);

  /// <summary>
  /// Gets a list of AI models.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of AI models.</returns>
  ServiceErgebnis<List<MaParameter>> GetAiModelList(ServiceDaten daten);

  /// <summary>
  /// Gets response from ChatGPT.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="systemprompt">Affected input system prompt string.</param>
  /// <param name="prompt">Affected input prompt string.</param>
  /// <param name="model">Affected AI model.</param>
  /// <param name="maxtokens">Affected maximal number of tokens.</param>
  /// <param name="temperature">Affected temperature between 0 and 1.</param>
  /// <param name="dialog">Affected dialog to be continued.</param>
  /// <returns>AI data with response from ChatGPT.</returns>
  ServiceErgebnis<AiData> AskChatGpt(ServiceDaten daten, string systemprompt, string prompt, string model = AiData.Gpt35, int maxtokens = 50, decimal temperature = 0.7M, AgDialog dialog = null);

  /// <summary>
  /// Gets list with dialog entries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="api">Affected api string.</param>
  /// <param name="uid">Affected uid.</param>
  /// <param name="search">Affected search string.</param>
  /// <returns>List with dialog entries.</returns>
  ServiceErgebnis<List<AgDialog>> GetDialogList(ServiceDaten daten, string api = null, string uid = null, string search = null);

  /// <summary>
  /// Deletes an dialog.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected uid.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteDialog(ServiceDaten daten, string uid);

  /// <summary>
  /// Continues a dialog.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="dialog">Affected dialog entry or null.</param>
  /// <param name="continues">True if dialog continues.</param>
  /// <param name="systemprompt">Affected input system prompt string.</param>
  /// <param name="prompt">Affected input prompt string.</param>
  /// <param name="response">Affected AI response.</param>
  /// <returns>AI data with response as dialog.</returns>
  ServiceErgebnis<AiData> ContinueDialog(ServiceDaten daten, AgDialog dialog, bool continues, string systemprompt, string prompt, string response);
}
