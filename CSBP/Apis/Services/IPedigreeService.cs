// <copyright file="IPedigreeService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Models;

  public interface IPedigreeService
  {
    /// <summary>
    /// Gets a list of ancestors.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="name">Affected last or birth name.</param>
    /// <param name="vorname">Affected first name.</param>
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
    /// <returns>Created or changed ancestor.</returns>
    ServiceErgebnis<SbPerson> SaveAncestor(ServiceDaten daten, string uid, string name, string vorname,
      string gebname, string geschlecht, string titel, string konfession, string bemerkung, string quid, int status1,
      int status2, int status3, string geburtsdatum, string geburtsort, string geburtsbem, string geburtsQuelle,
      string taufdatum, string taufort, string taufbem, String taufQuelle, string todesdatum, string todesort,
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
    /// Liefert den Ahnenbericht als PDF-Dokument in Bytes.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected Ahnancestor uid.</param>
    /// <param name="generations">Number of generations.</param>
    /// <param name="siblings">With siblings?</param>
    /// <param name="descendants">List of descendants?</param>
    /// <param name="forbears">List of forbears?</param>
    /// <returns>Ahnenbericht als PDF-Dokument in Bytes.</returns>
    ServiceErgebnis<byte[]> GetAncestorReport(ServiceDaten daten, string uid, int generations, bool siblings, bool descendants, bool forbears);

    /// <summary>
    /// Erstellen einer GEDCOM-Datei der Version 4.0 oder 5.5.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="skript">Name der Datei mit Pfad.</param>
    /// <param name="name">Name des Stammbaums in der Datei.</param>
    /// <param name="filter">Filter-Kriterium für Ahnen.</param>
    /// <param name="version">Version kann null sein, dann gilt Version "5.5".</param>
    /// <returns>Datei als String-Array.</returns>
    ServiceErgebnis<List<string>> ExportAncestorList(ServiceDaten daten, string skript, string name, string filter, string version = null);

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
}
