// <copyright file="IPedigreeService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Apis.Services;

using System.Collections.Generic;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;

/// <summary>
/// Interface for ancestor service.
/// </summary>
public interface IPedigreeService
{
  /// <summary>
  /// Gets a list of ancestors.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="name">Affected last or birth name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>List of ancestors.</returns>
  ServiceErgebnis<List<SbPerson>> GetAncestorList(ServiceDaten daten, string name = null, string firstname = null, string uid = null);

  /// <summary>
  /// Gets an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>Ancestor or null.</returns>
  ServiceErgebnis<SbPerson> GetAncestor(ServiceDaten daten, string uid);

  /// <summary>
  /// Gets a list of bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>list of bytes.</returns>
  ServiceErgebnis<List<ByteDaten>> GetBytes(ServiceDaten daten, string uid);

  /// <summary>
  /// Saves an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="vorname">Affected first names.</param>
  /// <param name="gebname">Affected birth name.</param>
  /// <param name="geschlecht">Affected gender.</param>
  /// <param name="titel">Affected title.</param>
  /// <param name="konfession">Affected confession.</param>
  /// <param name="bemerkung">Affected memo.</param>
  /// <param name="quid">Affected source id.</param>
  /// <param name="status1">Affected state 1.</param>
  /// <param name="status2">Affected state 2.</param>
  /// <param name="status3">Affected state 3.</param>
  /// <param name="geburtsdatum">Affected birth date.</param>
  /// <param name="geburtsort">Affected birth place.</param>
  /// <param name="geburtsbem">Affected birth memo.</param>
  /// <param name="geburtsQuelle">Affected birth source.</param>
  /// <param name="taufdatum">Affected baptist date.</param>
  /// <param name="taufort">Affected baptist place.</param>
  /// <param name="taufbem">Affected baptist memo.</param>
  /// <param name="taufQuelle">Affected baptist soure.</param>
  /// <param name="todesdatum">Affected death date.</param>
  /// <param name="todesort">Affected deatch place.</param>
  /// <param name="todesbem">Affected death memo.</param>
  /// <param name="todesQuelle">Affected death source.</param>
  /// <param name="begraebnisdatum">Affected funeral date.</param>
  /// <param name="begraebnisort">Affected funeral place.</param>
  /// <param name="begraebnisbem">Affected funeral memo.</param>
  /// <param name="begraebnisQuelle">Affected funeral source.</param>
  /// <param name="gatteNeu">New spouce id.</param>
  /// <param name="vaterUidNeu">New father id.</param>
  /// <param name="mutterUidNeu">New mother id.</param>
  /// <param name="byteliste">Affected byte data list.</param>
  /// <returns>Created or changed ancestor.</returns>
  ServiceErgebnis<SbPerson> SaveAncestor(ServiceDaten daten, string uid, string name, string vorname,
    string gebname, string geschlecht, string titel, string konfession, string bemerkung, string quid, int status1,
    int status2, int status3, string geburtsdatum, string geburtsort, string geburtsbem, string geburtsQuelle,
    string taufdatum, string taufort, string taufbem, string taufQuelle, string todesdatum, string todesort,
    string todesbem, string todesQuelle, string begraebnisdatum, string begraebnisort, string begraebnisbem,
    string begraebnisQuelle, string gatteNeu, string vaterUidNeu, string mutterUidNeu, List<ByteDaten> byteliste);

  /// <summary>
  /// Deletes an ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteAncestor(ServiceDaten daten, SbPerson e);

  /// <summary>
  /// Gets the next ancestor by name.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="name">Affected name.</param>
  /// <param name="firstname">Affected first name.</param>
  /// <returns>Ancestor uid or null.</returns>
  ServiceErgebnis<string> GetNextAncestorName(ServiceDaten daten, string uid, string name, string firstname);

  /// <summary>
  /// Gets the first child of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <param name="fuid">Affected family uid.</param>
  /// <returns>Ancestor uid or null.</returns>
  ServiceErgebnis<string> GetFirstChild(ServiceDaten daten, string uid, string fuid = null);

  /// <summary>
  /// Gets the next spouse of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>Ancestor uid or null.</returns>
  ServiceErgebnis<SbPerson> GetNextSpouse(ServiceDaten daten, string uid);

  /// <summary>
  /// Gets all spouses of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>List of Ancestors.</returns>
  ServiceErgebnis<List<SbPerson>> GetSpouseList(ServiceDaten daten, string uid);

  /// <summary>
  /// Gets the next sibling of the ancestor.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of ancestor.</param>
  /// <returns>Ancestor uid or null.</returns>
  ServiceErgebnis<SbPerson> GetNextSibling(ServiceDaten daten, string uid);

  /// <summary>
  /// Gets a list of families.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of families.</returns>
  ServiceErgebnis<List<SbFamilie>> GetFamilyList(ServiceDaten daten);

  /// <summary>
  /// Gets a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of family.</param>
  /// <param name="puid">Affected father or mother uid.</param>
  /// <param name="cuid">Affected child uid.</param>
  /// <returns>Familiy or null.</returns>
  ServiceErgebnis<SbFamilie> GetFamily(ServiceDaten daten, string uid, string puid = null, string cuid = null);

  /// <summary>
  /// Gets a list of children.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of family.</param>
  /// <returns>list of children.</returns>
  ServiceErgebnis<List<SbPerson>> GetChildList(ServiceDaten daten, string uid);

  /// <summary>
  /// Saves a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of family.</param>
  /// <param name="fuid">Affected father uid.</param>
  /// <param name="muid">Affected mother uid.</param>
  /// <param name="mdate">Affected marriage date.</param>
  /// <param name="mplace">Affected marriage place.</param>
  /// <param name="mmemo">Affected marriage memo.</param>
  /// <param name="suid">Affected source id.</param>
  /// <param name="children">Affected list of children.</param>
  /// <returns>Created or changed family.</returns>
  ServiceErgebnis<SbFamilie> SaveFamily(ServiceDaten daten, string uid, string fuid, string muid,
    string mdate, string mplace, string mmemo, string suid, List<string> children);

  /// <summary>
  /// Deletes a family.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteFamily(ServiceDaten daten, SbFamilie e);

  /// <summary>
  /// Gets a list of sources.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of sources.</returns>
  ServiceErgebnis<List<SbQuelle>> GetSourceList(ServiceDaten daten);

  /// <summary>
  /// Gets a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of source.</param>
  /// <returns>Source or null.</returns>
  ServiceErgebnis<SbQuelle> GetSource(ServiceDaten daten, string uid);

  /// <summary>
  /// Saves a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">ID of source.</param>
  /// <param name="author">Affected author.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="quotation">Affected quotation.</param>
  /// <param name="memo">Affected memo.</param>
  /// <returns>Created or changed source.</returns>
  ServiceErgebnis<SbQuelle> SaveSource(ServiceDaten daten, string uid, string author, string desc,
    string quotation, string memo);

  /// <summary>
  /// Deletes a source.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteSource(ServiceDaten daten, SbQuelle e);

  /// <summary>
  /// Gets the ancestor report as html document in bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected Ahnancestor uid.</param>
  /// <param name="generations">Number of generations.</param>
  /// <param name="siblings">With siblings or not.</param>
  /// <param name="descendants">List of descendants or not.</param>
  /// <param name="forebears">List of forebears or not.</param>
  /// <returns>Ahnenbericht als PDF-Dokument in Bytes.</returns>
  ServiceErgebnis<byte[]> GetAncestorReport(ServiceDaten daten, string uid, int generations, bool siblings, bool descendants, bool forebears);

  /// <summary>
  /// Exports the ancestor report as GEDCOM file in version 4.0 or 5.5.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="skript">Affected file name with path.</param>
  /// <param name="name">Name of pedigree or family in the file.</param>
  /// <param name="filter">Filter criteria for ancestors.</param>
  /// <param name="version">Version can be null.</param>
  /// <returns>File as list of lines.</returns>
  ServiceErgebnis<List<string>> ExportAncestorList(ServiceDaten daten, string skript, string name, string filter, string version = "5.5");

  /// <summary>
  /// Imports list of ancestors from GEDCOM file.
  /// </summary>
  /// <returns>Message of import.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="list">GEDCOM file as list of lines.</param>
  ServiceErgebnis<string> ImportAncestorList(ServiceDaten daten, List<string> list);

  /// <summary>
  /// Calculate the death year of all ancestors (Status1).
  /// </summary>
  /// <returns>Message of import.</returns>
  /// <param name="daten">Service data for database access.</param>
  ServiceErgebnis CalculateDeathYear(ServiceDaten daten);
}
