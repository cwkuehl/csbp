// <copyright file="IPrivateService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Models;

  public interface IPrivateService
  {
    /// <summary>
    /// Gets a list of bikes.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>List of bikes.</returns>
    ServiceErgebnis<List<FzFahrrad>> GetBikeList(ServiceDaten daten);

    /// <summary>
    /// Gets a bike.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of bike.</param>
    /// <returns>Bike or null.</returns>
    ServiceErgebnis<FzFahrrad> GetBike(ServiceDaten daten, string uid);

    /// <summary>
    /// Saves a bike.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of bike.</param>
    /// <param name="desc">Description of bike.</param>
    /// <param name="type">Type of bike.</param>
    /// <returns>Created or changed bike.</returns>
    ServiceErgebnis<FzFahrrad> SaveBike(ServiceDaten daten, string uid, string desc, int type);

    /// <summary>
    /// Deletes a bike.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteBike(ServiceDaten daten, FzFahrrad e);

    /// <summary>
    /// Gets a list of mileages.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="buid">Affected bike ID.</param>
    /// <param name="text">Affected text.</param>
    /// <returns>List of mileages.</returns>
    ServiceErgebnis<List<FzFahrradstand>> GetMileageList(ServiceDaten daten, string buid, string text);

    /// <summary>
    /// Gets a list of mileages for statistics.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="buid">Affected date.</param>
    /// <param name="years">Affected years.</param>
    /// <returns>List of mileages.</returns>
    ServiceErgebnis<List<FzFahrradstand>> GetMileages(ServiceDaten daten, DateTime date, int years = 10);

    /// <summary>
    /// Gets a mileage.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="buid">Affected bike ID.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="nr">Affected number.</param>
    /// <returns>Mileage or null.</returns>
    ServiceErgebnis<FzFahrradstand> GetMileage(ServiceDaten daten, string buid, DateTime date, int nr);

    /// <summary>
    /// Saves a mileage.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="buid">Affected bike ID.</param>
    /// <param name="date">Affected date.</param>
    /// <param name="nr">Affected number.</param>
    /// <param name="odometer">Affected odometer.</param>
    /// <param name="km">Affected km.</param>
    /// <param name="average">Affected average.</param>
    /// <param name="text">Affected text.</param>
    /// <param name="angelegtVon">Affected creator.</param>
    /// <param name="angelegtAm">Affected creation date.</param>
    /// <param name="geaendertVon">Affected changer.</param>
    /// <param name="geaendertAm">Affected change date.</param>
    /// <returns>Created or changed mileage.</returns>
    ServiceErgebnis<FzFahrradstand> SaveMileage(ServiceDaten daten, string buid, DateTime date, int nr,
      decimal odometer, decimal km, decimal average, string text, string angelegtVon = null, DateTime? angelegtAm = null,
      string geaendertVon = null, DateTime? geaendertAm = null);

    /// <summary>
    /// Repairs all mileages per bike, so that all odometer values are growing with time.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis RepairMileages(ServiceDaten daten);

    /// <summary>
    /// Deletes a mileage.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteMileage(ServiceDaten daten, FzFahrradstand e);

    /// <summary>
    /// Gets a list of authors.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="name">Affected name.</param>
    /// <returns>List of authors.</returns>
    ServiceErgebnis<List<FzBuchautor>> GetAuthorList(ServiceDaten daten, string name = null);

    /// <summary>
    /// Gets a author.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <returns>Author or null.</returns>
    ServiceErgebnis<FzBuchautor> GetAuthor(ServiceDaten daten, string uid);

    /// <summary>
    /// Saves an author.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <returns>Created or changed author.</returns>
    ServiceErgebnis<FzBuchautor> SaveAuthor(ServiceDaten daten, string uid, string name, string firstname);

    /// <summary>
    /// Deletes an author.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteAuthor(ServiceDaten daten, FzBuchautor e);

    /// <summary>
    /// Gets a list of series.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="name">Affected name.</param>
    /// <returns>List of series.</returns>
    ServiceErgebnis<List<FzBuchserie>> GetSeriesList(ServiceDaten daten, string name = null);

    /// <summary>
    /// Gets a series.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <returns>Series or null.</returns>
    ServiceErgebnis<FzBuchserie> GetSeries(ServiceDaten daten, string uid);

    /// <summary>
    /// Saves a series.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="name">Affected name.</param>
    /// <returns>Created or changed series.</returns>
    ServiceErgebnis<FzBuchserie> SaveSeries(ServiceDaten daten, string uid, string name);

    /// <summary>
    /// Deletes a series.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteSeries(ServiceDaten daten, FzBuchserie e);

    /// <summary>
    /// Gets a list of books.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="auuid">Affected author ID.</param>
    /// <param name="seuid">Affected series ID.</param>
    /// <param name="bouid">Affected book ID.</param>
    /// <param name="name">Affected name.</param>
    /// <returns>List of books.</returns>
    ServiceErgebnis<List<FzBuch>> GetBookList(ServiceDaten daten, string auuid,
        string seuid, string bouid, string name);

    /// <summary>
    /// Gets a book.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <returns>Book or null.</returns>
    ServiceErgebnis<FzBuch> GetBook(ServiceDaten daten, string uid);

    /// <summary>
    /// Saves a book.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="auuid">Affected author ID.</param>
    /// <param name="seuid">Affected series ID.</param>
    /// <param name="serialnr">Affected serial number.</param>
    /// <param name="title">Affected book title.</param>
    /// <param name="pages">Affected number of pages.</param>
    /// <param name="lang">Affected language.</param>
    /// <param name="poss">Affected possession.</param>
    /// <param name="read">Affected reading date.</param>
    /// <param name="heard">Affected heard date.</param>
    /// <returns>Created or changed book.</returns>
    ServiceErgebnis<FzBuch> SaveBook(ServiceDaten daten, string uid, string auuid, string seuid,
        int serialnr, string title, int pages, int lang, bool poss, DateTime? read, DateTime? heard);

    /// <summary>
    /// Deletes a book.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteBook(ServiceDaten daten, FzBuch e);

    /// <summary>
    /// Gets a list of memos.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="text">Affected text.</param>
    /// <returns>List of memos.</returns>
    ServiceErgebnis<List<FzNotiz>> GetMemoList(ServiceDaten daten, string text = null);

    /// <summary>
    /// Gets a memo.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <returns>Memo or null.</returns>
    ServiceErgebnis<FzNotiz> GetMemo(ServiceDaten daten, string uid);

    /// <summary>
    /// Saves a memo.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected ID.</param>
    /// <param name="topic">Affected topic.</param>
    /// <param name="notes">Affected notes.</param>
    /// <returns>Created or changed memo.</returns>
    ServiceErgebnis<FzNotiz> SaveMemo(ServiceDaten daten, string uid, string topic, string notes);

    /// <summary>
    /// Deletes a memo.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    ServiceErgebnis DeleteMemo(ServiceDaten daten, FzNotiz e);

    /// <summary>
    /// Gets statistics as string.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="nr">Affected statistics.</param>
    /// <param name="date">Affected date.</param>
    /// <returns>Statistics as string.</returns>
    ServiceErgebnis<string> GetStatistics(ServiceDaten daten, int nr, DateTime date);
  }
}
