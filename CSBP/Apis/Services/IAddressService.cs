// <copyright file="IAddressService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Models;

  public interface IAddressService
  {
    /// <summary>
    /// Gets list of birthdays.
    /// </summary>
    /// <returns>List of birthdays.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="days">Affected number of days.</param>
    ServiceErgebnis<List<string>> GetBirthdayList(ServiceDaten daten, DateTime date, int days);

    /// <summary>
    /// Gets list of sites with persons and addresses.
    /// </summary>
    /// <returns>List of sites.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="actual">Only actual persons?</param>
    /// <param name="search">Affected search string.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <param name="puid">Affected person ID.</param>
    /// <param name="suid">Affected site ID.</param>
    ServiceErgebnis<List<AdSitz>> GetPersonList(ServiceDaten daten, bool actual, string search = null,
      string name = null, string firstname = null, string puid = null, string suid = null);

    /// <summary>
    /// Gets a site with person and address.
    /// </summary>
    /// <returns>A site or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="suid">Affected site ID.</param>
    ServiceErgebnis<AdSitz> GetSite(ServiceDaten daten, string suid);

    /// <summary>
    /// Saves a site.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of person.</param>
    /// <param name="gender">Affected gender.</param>
    /// <param name="geburt">Affected birthday.</param>
    /// <param name="name1">Affected last name.</param>
    /// <param name="name2">Affected name part two.</param>
    /// <param name="predicate">Affected predicate.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <param name="title">Affected title.</param>
    /// <param name="personstate">Affected state of person.</param>
    /// <param name="suid">Affected site ID.</param>
    /// <param name="name">Affected site name.</param>
    /// <param name="phone">Affected phone number.</param>
    /// <param name="fax">Affected fax number.</param>
    /// <param name="mobile">Affected mobile number.</param>
    /// <param name="email">Affected email address.</param>
    /// <param name="homepage">Affected homepage.</param>
    /// <param name="pobox">Affected PO box.</param>
    /// <param name="memo">Affected memo.</param>
    /// <param name="sitestate">Affected site state.</param>
    /// <param name="auid">Affected address ID.</param>
    /// <param name="state">Affected state.</param>
    /// <param name="postalcode">Affected postal code.</param>
    /// <param name="town">Affected town.</param>
    /// <param name="street">Affected street.</param>
    /// <param name="no">Affected street number.</param>
    /// <returns>Created or changed site.</returns>
    ServiceErgebnis<AdSitz> SaveSite(ServiceDaten daten, string uid, string gender,
        DateTime? geburt, string name1, string name2, string predicate, string firstname,
        string title, int personstate, string suid, string name, string phone,
        string fax, string mobile, string email, string homepage, string pobox, string memo,
        int sitestate, string auid, string state, string postalcode, string town, string street,
        string no);

    /// <summary>
    /// Deletes a site, an address and a person.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteSite(ServiceDaten daten, AdSitz e);

    /// <summary>
    /// Gets the number of usages of an address.
    /// </summary>
    /// <returns>Number of usages of an address.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auid">Affected address ID.</param>
    ServiceErgebnis<int> GetAddressCount(ServiceDaten daten, string auid);

    /// <summary>
    /// Makes the site first.
    /// </summary>
    /// <returns>Possible errors.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="siteuid">Affected site ID.</param>
    ServiceErgebnis MakeSiteFirst(ServiceDaten daten, string siteuid);

    /// <summary>
    /// Gets list of addresses.
    /// </summary>
    /// <returns>List of addresses.</returns>
    /// <param name="daten">Service data for database access.</param>
    ServiceErgebnis<List<AdAdresse>> GetAddressList(ServiceDaten daten);

    /// <summary>
    /// Gets an address.
    /// </summary>
    /// <returns>An address or null.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auid">Affected Address ID.</param>
    ServiceErgebnis<AdAdresse> GetAddress(ServiceDaten daten, string auid);

    /// <summary>
    /// Exports list of addresses.
    /// </summary>
    /// <returns>List of addresses.</returns>
    /// <param name="daten">Service data for database access.</param>
    ServiceErgebnis<List<string>> ExportAddressList(ServiceDaten daten);

    /// <summary>
    /// Imports list of addresses.
    /// </summary>
    /// <returns>Message of import.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="list">List of addresses.</param>
    /// <param name="delete">Delete all persons, sites and addresses?</param>
    ServiceErgebnis<string> ImportAddressList(ServiceDaten daten, List<string> list, bool delete);

    /// <summary>
    /// Gets an address report.
    /// </summary>
    /// <returns>An address report.</returns>
    /// <param name="daten">Service data for database access.</param>
    ServiceErgebnis<byte[]> GetAddressReport(ServiceDaten daten);
  }
}
