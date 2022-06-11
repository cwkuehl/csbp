// <copyright file="IStockService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services;

using System;
using System.Collections.Generic;
using System.Text;
using CSBP.Apis.Models;

/// <summary>
/// Interface for stock service.
/// </summary>
public interface IStockService
{
  /// <summary>
  /// Gets a list of stocks.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inactive">Get also inactive investments or not.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="pattern">Affected Pattern.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of stocks.</returns>
  ServiceErgebnis<List<WpWertpapier>> GetStockList(ServiceDaten daten, bool inactive,
    string desc = null, string pattern = null, string uid = null, string search = null);

  /// <summary>
  /// Gets a stock.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Stock or null.</returns>
  ServiceErgebnis<WpWertpapier> GetStock(ServiceDaten daten, string uid);

  /// <summary>
  /// Saves a stock.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="abbreviation">Affected abbreviation.</param>
  /// <param name="signal1">Affected signal price 1.</param>
  /// <param name="sort">Affected sorting criteria.</param>
  /// <param name="source">Affected source.</param>
  /// <param name="state">Affected state.</param>
  /// <param name="relationuid">Affected relationuid.</param>
  /// <param name="memo">Affected memo.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="currency">Affected currency.</param>
  /// <param name="inv">Create also an investment or not.</param>
  /// <returns>Created or changed stock.</returns>
  ServiceErgebnis<WpWertpapier> SaveStock(ServiceDaten daten, string uid, string desc,
    string abbreviation, decimal? signal1, string sort, string source, string state,
    string relationuid, string memo, string type = null, string currency = null, bool inv = false);

  /// <summary>
  /// Deletes a stock.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteStock(ServiceDaten daten, WpWertpapier e);

  /// <summary>
  /// Calculates all stocks.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="pattern">Affected pattern.</param>
  /// <param name="uid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="kuid">Affected konfiguration ID.</param>
  /// <param name="status">Status of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  ServiceErgebnis CalculateStocks(ServiceDaten daten, string desc, string pattern, string uid,
    DateTime date, bool inactive, string search, string kuid, StringBuilder status, StringBuilder cancel);

  /// <summary>
  /// Gets a list of states.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of states.</returns>
  ServiceErgebnis<List<MaParameter>> GetStateList(ServiceDaten daten);

  /// <summary>
  /// Gets a list of providers.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of providers.</returns>
  ServiceErgebnis<List<MaParameter>> GetProviderList(ServiceDaten daten);

  /// <summary>
  /// Gets a list of configurations.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="extended">Get more data or not.</param>
  /// <param name="state">Affected configuration state.</param>
  /// <returns>List of configurations.</returns>
  ServiceErgebnis<List<WpKonfiguration>> GetConfigurationList(ServiceDaten daten, bool extended, string state);

  /// <summary>
  /// Gets a configuration.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="def">Returns defaults if empty.</param>
  /// <returns>Configurations or null.</returns>
  ServiceErgebnis<WpKonfiguration> GetConfiguration(ServiceDaten daten, string uid, bool def = false);

  /// <summary>
  /// Saves a configuration.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="box">Affected box size.</param>
  /// <param name="reversal">Affected reversal.</param>
  /// <param name="method">Affected method.</param>
  /// <param name="duration">Affected duration.</param>
  /// <param name="relative">Is it relative Calculation or not.</param>
  /// <param name="scale">Affected scale.</param>
  /// <param name="state">Affected state.</param>
  /// <param name="memo">Affected memo.</param>
  /// <returns>Created or changed configuration.</returns>
  ServiceErgebnis<WpKonfiguration> SaveConfiguration(ServiceDaten daten, string uid, string desc,
    decimal box, int reversal, int method, int duration, bool relative, int scale,
    string state, string memo);

  /// <summary>
  /// Deletes a configuration.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteConfiguration(ServiceDaten daten, WpKonfiguration e);

  /// <summary>
  /// Gets a list of scales.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of scales.</returns>
  ServiceErgebnis<List<MaParameter>> GetScaleList(ServiceDaten daten);

  /// <summary>
  /// Gets a list of PnF methods.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of PnF methods.</returns>
  ServiceErgebnis<List<MaParameter>> GetMethodList(ServiceDaten daten);

  /// <summary>
  /// Gets a list of prices or rates for a period.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Beginning of the period.</param>
  /// <param name="to">End of the period.</param>
  /// <param name="uid">Affected stock uid.</param>
  /// <param name="relative">Relative prices to relation or not.</param>
  /// <returns>List of prices.</returns>
  ServiceErgebnis<List<SoKurse>> GetPriceList(ServiceDaten daten, DateTime from, DateTime to,
    string uid, bool relative);

  /// <summary>
  /// Gets a list of investments.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inactive">Get also inactive investments or not.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of investments.</returns>
  ServiceErgebnis<List<WpAnlage>> GetInvestmentList(ServiceDaten daten, bool inactive,
    string desc = null, string uid = null, string stuid = null, string search = null);

  /// <summary>
  /// Gets an investment.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Investment or null.</returns>
  ServiceErgebnis<WpAnlage> GetInvestment(ServiceDaten daten, string uid);

  /// <summary>
  /// Saves an investment.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="memo">Affected memo.</param>
  /// <param name="state">Affected state.</param>
  /// <param name="pfuid">Affected portfolio account ID.</param>
  /// <param name="smuid">Affected settlement account ID.</param>
  /// <param name="icuid">Affected income account ID.</param>
  /// <param name="valuta">Affected value date.</param>
  /// <param name="value">Affected value.</param>
  /// <returns>Created or changed investment.</returns>
  ServiceErgebnis<WpAnlage> SaveInvestment(ServiceDaten daten, string uid, string stuid,
    string desc, string memo, int state, string pfuid, string smuid, string icuid,
    DateTime? valuta, decimal value);

  /// <summary>
  /// Deletes an investment.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteInvestment(ServiceDaten daten, WpAnlage e);

  /// <summary>
  /// Calculates all investments.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="status">Status of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  ServiceErgebnis CalculateInvestments(ServiceDaten daten, string desc, string uid, string stuid,
    DateTime date, bool inactive, string search, StringBuilder status, StringBuilder cancel);

  /// <summary>
  /// Gets a list of bookings.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="inuid">Affected investment ID.</param>
  /// <returns>List of bookings.</returns>
  ServiceErgebnis<List<WpBuchung>> GetBookingList(ServiceDaten daten,
    string desc = null, string uid = null, string stuid = null, string inuid = null);

  /// <summary>
  /// Gets a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Booking or null.</returns>
  ServiceErgebnis<WpBuchung> GetBooking(ServiceDaten daten, string uid);

  /// <summary>
  /// Gets list of events for an investment.
  /// </summary>
  /// <returns>List of events.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="iuid">Affected investment ID.</param>
  ServiceErgebnis<List<HhEreignis>> GetEventList(ServiceDaten daten, string iuid);

  /// <summary>
  /// Saves a booking with optional budget booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="inuid">Affected investment ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="payment">Affected payment.</param>
  /// <param name="discount">Affected discount.</param>
  /// <param name="shares">Affected shares.</param>
  /// <param name="interest">Affected interest.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="memo">Affected memo.</param>
  /// <param name="price">Affected price.</param>
  /// <param name="buid">Affected budget booking ID.</param>
  /// <param name="vd">Affected value date.</param>
  /// <param name="v">Affected value in EUR.</param>
  /// <param name="duid">Affected debit account ID.</param>
  /// <param name="cuid">Affected credit account ID.</param>
  /// <param name="text">Affected posting text.</param>
  /// <returns>Created or changed booking.</returns>
  ServiceErgebnis<WpBuchung> SaveBooking(ServiceDaten daten, string uid, string inuid,
    DateTime date, decimal payment, decimal discount, decimal shares, decimal interest,
    string desc, string memo, decimal price,
    string buid, DateTime vd, decimal v, string duid, string cuid, string text);

  /// <summary>
  /// Deletes a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeleteBooking(ServiceDaten daten, WpBuchung e);

  /// <summary>
  /// Gets a list of prices.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected stock ID.</param>
  /// <param name="from">Beginning of the period.</param>
  /// <param name="to">End of the period.</param>
  /// <returns>List of prices.</returns>
  ServiceErgebnis<List<WpStand>> GetPriceList(ServiceDaten daten, string uid = null,
    DateTime? from = null, DateTime? to = null);

  /// <summary>
  /// Gets a price.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <returns>Price or null.</returns>
  ServiceErgebnis<WpStand> GetPrice(ServiceDaten daten, string stuid, DateTime date);

  /// <summary>
  /// Saves a price.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="price">Affected price.</param>
  /// <returns>Created or changed price.</returns>
  ServiceErgebnis<WpStand> SavePrice(ServiceDaten daten, string stuid, DateTime date,
    decimal price);

  /// <summary>
  /// Deletes a price.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  ServiceErgebnis DeletePrice(ServiceDaten daten, WpStand e);

  /// <summary>
  /// Export stocks in csv file.
  /// </summary>
  /// <returns>Csv file as lines array or errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="pattern">Affected pattern.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="cuid">Affected configuration ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="days">Affected konfiguration ID.</param>
  /// <param name="status">Status of backup is always updated.</param>
  /// <param name="cancel">Cancel backup if not empty.</param>
  ServiceErgebnis<List<string>> ExportStocks(ServiceDaten daten, string search, string desc, string pattern, string stuid, bool inactive,
    string cuid, DateTime date, int days, StringBuilder status, StringBuilder cancel);
}
