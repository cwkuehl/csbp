// <copyright file="IBudgetService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services;

using System;
using System.Collections.Generic;
using System.Text;
using CSBP.Apis.Models;

/// <summary>
/// Interface for address service.
/// </summary>
public interface IBudgetService
{
  /// <summary>
  /// Gets list of periods.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of periods.</returns>
  ServiceErgebnis<List<HhPeriode>> GetPeriodList(ServiceDaten daten);

  /// <summary>
  /// Gets a period with first beginning and last end.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Period with first beginning and last end.</returns>
  ServiceErgebnis<HhPeriode> GetMinMaxPeriod(ServiceDaten daten);

  /// <summary>Saves a new period.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="months">Number of months.</param>
  /// <param name="end">At the end or at the beginning or not.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis SavePeriod(ServiceDaten daten, int months, bool end);

  /// <summary>Deletes a period.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="end">At the end or at the beginning or not.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeletePeriod(ServiceDaten daten, bool end);

  /// <summary>
  /// Gets list of accounts.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="from">Affected minimum date.</param>
  /// <param name="to">Affected maximum date.</param>
  /// <returns>List of accounts.</returns>
  ServiceErgebnis<List<HhKonto>> GetAccountList(ServiceDaten daten, string text = null, DateTime? from = null, DateTime? to = null);

  /// <summary>
  /// Gets an account.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <returns>An account or null.</returns>
  ServiceErgebnis<HhKonto> GetAccount(ServiceDaten daten, string auid);

  /// <summary>Saves an account.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="attr">Affected attribute.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="value">Affected value on date from.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis SaveAccount(ServiceDaten daten, string uid, string type, string attr,
      string desc, DateTime? from, DateTime? to, decimal value);

  /// <summary>
  /// Deletes an account.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteAccount(ServiceDaten daten, HhKonto e);

  /// <summary>
  /// Get the booking span of an account.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected uid.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis<string> GetBookingSpan(ServiceDaten daten, string uid);

  /// <summary>
  /// Gets list of events.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="from">Affected minimum date.</param>
  /// <param name="to">Affected maximum date.</param>
  /// <returns>List of events.</returns>
  ServiceErgebnis<List<HhEreignis>> GetEventList(ServiceDaten daten, string text = null, DateTime? from = null, DateTime? to = null);

  /// <summary>
  /// Gets an event.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="euid">Affected event ID.</param>
  /// <returns>An event or null.</returns>
  ServiceErgebnis<HhEreignis> GetEvent(ServiceDaten daten, string euid);

  /// <summary>Saves an event.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="attr">Affected attribute.</param>
  /// <param name="duid">Affected debit account ID.</param>
  /// <param name="cuid">Affected credit account ID.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="text">Affected posting text.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis SaveEvent(ServiceDaten daten, string uid, string attr, string duid,
      string cuid, string desc, string text);

  /// <summary>
  /// Deletes an event.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteEvent(ServiceDaten daten, HhEreignis e);

  /// <summary>
  /// Gets list of bookings.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="valuta">Search for value date.</param>
  /// <param name="from">Affected minimum date.</param>
  /// <param name="to">Affected maximum date.</param>
  /// <param name="text">Affected posting text.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <param name="value">Affected value.</param>
  /// <returns>List of bookings.</returns>
  ServiceErgebnis<List<HhBuchung>> GetBookingList(ServiceDaten daten, bool valuta,
      DateTime? from = null, DateTime? to = null, string text = null, string auid = null,
      string value = null);

  /// <summary>
  /// Gets list of bookings in csv strings.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="valuta">Search for value date.</param>
  /// <param name="from">Affected minimum date.</param>
  /// <param name="to">Affected maximum date.</param>
  /// <param name="text">Affected posting text.</param>
  /// <param name="auid">Affected account ID.</param>
  /// <param name="value">Affected value.</param>
  /// <returns>List of bookings in csv strings.</returns>
  ServiceErgebnis<List<string>> ExportBookingList(ServiceDaten daten, bool valuta,
    DateTime? from = null, DateTime? to = null, string text = null, string auid = null,
    string value = null);

  /// <summary>
  /// Imports list of bookings.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="list">List of addresses.</param>
  /// <param name="delete">Delete all bookings or not.</param>
  /// <returns>Message of import.</returns>
  ServiceErgebnis<string> ImportBookingList(ServiceDaten daten, List<string> list, bool delete);

  /// <summary>
  /// Gets a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="buid">Affected booking ID.</param>
  /// <returns>A booking or null.</returns>
  ServiceErgebnis<HhBuchung> GetBooking(ServiceDaten daten, string buid);

  /// <summary>
  /// Gets a new receipt number.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected receipt date.</param>
  /// <returns>A new receipt number.</returns>
  ServiceErgebnis<string> GetNewReceipt(ServiceDaten daten, DateTime date);

  /// <summary>Saves a booking.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="vd">Affected value date.</param>
  /// <param name="vdm">Affected value in DM.</param>
  /// <param name="v">Affected value in EUR.</param>
  /// <param name="duid">Affected debit account ID.</param>
  /// <param name="cuid">Affected credit account ID.</param>
  /// <param name="text">Affected posting text.</param>
  /// <param name="rn">Affected receipt number.</param>
  /// <param name="rd">Affected receipt date.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis<HhBuchung> SaveBooking(ServiceDaten daten, string uid, DateTime vd, decimal vdm, decimal v,
    string duid, string cuid, string text, string rn, DateTime rd);

  /// <summary>Saves a booking with revision.</summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="vd">Affected value date.</param>
  /// <param name="kz">Affected attribute.</param>
  /// <param name="vdm">Affected value in DM.</param>
  /// <param name="v">Affected value in EUR.</param>
  /// <param name="duid">Affected debit account ID.</param>
  /// <param name="cuid">Affected credit account ID.</param>
  /// <param name="text">Affected posting text.</param>
  /// <param name="rn">Affected receipt number.</param>
  /// <param name="rd">Affected receipt date.</param>
  /// <param name="angelegtVon">Affected creator.</param>
  /// <param name="angelegtAm">Affected creation date.</param>
  /// <param name="geaendertVon">Affected changer.</param>
  /// <param name="geaendertAm">Affected change date.</param>
  /// <returns>Saved entity.</returns>
  HhBuchung SaveBookingIntern(ServiceDaten daten, string uid, DateTime vd, string kz, decimal vdm, decimal v,
    string duid, string cuid, string text, string rn, DateTime rd, string angelegtVon, DateTime? angelegtAm,
    string geaendertVon, DateTime? geaendertAm);

  /// <summary>
  /// Reverses a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis ReverseBooking(ServiceDaten daten, HhBuchung e);

  /// <summary>
  /// Deletes a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteBooking(ServiceDaten daten, HhBuchung e);

  /// <summary>
  /// Gets list of balance rows.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="type">Affected balance type.</param>
  /// <param name="from">Affected start date.</param>
  /// <param name="to">Affected end date.</param>
  /// <returns>list of balance rows.</returns>
  ServiceErgebnis<List<HhBilanz>> GetBalanceList(ServiceDaten daten, string type, DateTime from, DateTime to);

  /// <summary>
  /// Calculates the balances in one or more periods.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="state">State of calculation is always updated.</param>
  /// <param name="cancel">Cancel calculation if not empty.</param>
  /// <param name="alles">All periods or only one.</param>
  /// <param name="von">Affected date in period.</param>
  /// <returns>0 alles aktuell; 1 eine Periode aktualisiert; 2 noch eine weitere kann Periode aktualisiert werden.</returns>
  ServiceErgebnis<string[]> CalculateBalances(ServiceDaten daten, StringBuilder state, StringBuilder cancel,
    bool alles = false, DateTime? von = null);

  /// <summary>
  /// Swaps the sorting of 2 accounts.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="auid">Betroffenes 1. Konto.</param>
  /// <param name="auid2">Betroffenes 2. Konto.</param>
  /// <returns>Evtl. Fehlermeldungen.</returns>
  ServiceErgebnis SwapAccountSort(ServiceDaten daten, string auid, string auid2);

  /// <summary>
  /// Gets the annual report as html document in bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="title">Affected title.</param>
  /// <param name="ob">With opening balance of not.</param>
  /// <param name="pl">With profit loss balance or not.</param>
  /// <param name="fb">With final balance or not.</param>
  /// <returns>Annual report as html document in bytes.</returns>
  ServiceErgebnis<byte[]> GetAnnualReport(ServiceDaten daten, DateTime from, DateTime to, string title, bool ob, bool pl, bool fb);

  /// <summary>
  /// Gets the cash report as html document in bytes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="title">Affected title.</param>
  /// <returns>Cash report as html document in bytes.</returns>
  ServiceErgebnis<byte[]> GetCashReport(ServiceDaten daten, DateTime from, DateTime to, string title);

  /// <summary>
  /// Gets list of proprietary in the last years.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Affected date.</param>
  /// <param name="years">Affected years.</param>
  /// <returns>List of proprietary.</returns>
  ServiceErgebnis<List<HhBilanz>> GetProprietaryList(ServiceDaten daten, DateTime from, int years = 10);

  /// <summary>
  /// Gets list of proprietary and profit and loss in the last years.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Affected date.</param>
  /// <param name="years">Affected years.</param>
  /// <returns>List of proprietary and profit and loss.</returns>
  ServiceErgebnis<List<HhBilanz>> GetProprietaryPlList(ServiceDaten daten, DateTime from, int years = 10);
}
