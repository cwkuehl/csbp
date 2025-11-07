// <copyright file="ServiceTest.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.UnitTest;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CSBP.Services.Base;
using CSBP.Services.Base.Csv;
using CSBP.Services.Factory;
using NUnit.Framework;

/// <summary>Test class for services.</summary>
public class ServiceTest
{
  private static ServiceDaten daten; // = new ServiceDaten(1, "Test", null);

  /// <summary>Test setup.</summary>
  [SetUp]
  public void Setup()
  {
    var connect = "Data Source=/home/wolfgang/hsqldb/csbp.db";
    Parameter.Connect = connect;
    daten = new ServiceDaten("0", 1, "Administrator", null);
    var r1 = FactoryService.ClientService.InitDb(daten);
    r1.ThrowAllErrors("InitDb");
    var r2 = FactoryService.ClientService.GetOptionList(daten, daten.MandantNr, Parameter.Params, null);
    r2.ThrowAllErrors("GetOptionList");
  }

  /// <summary>
  /// Call of all Tests.
  /// </summary>
  [Test]
  public void TestAll()
  {
    // GetMileages();
    // CalculateInvestments();
    // GetWeatherList();
    GetAverageTemperatureList();
  }

  /// <summary>
  /// Test of function PrivateService.GetMileages.
  /// </summary>
  [Test]
  public void GetMileages()
  {
    // var dates = new List<DateTime> { new(2024, 12, 30), new(2024, 12, 31), new(2025, 1, 1) };
    var dates = new List<DateTime> { new(2024, 6, 26) };
    foreach (var date in dates)
    {
      var r = FactoryService.PrivateService.GetMileages(daten, date, 100);
      r.ThrowAllErrors("GetMileages");
      Debug.Write($"GetMileages {date:yyyy-MM-dd}");
      foreach (var a in r.Ergebnis)
      {
        Debug.Write($" {a.Datum:yyyy-MM-dd} {a.Periode_km:0.00} {a.Zaehler_km:0.00}");
      }
      Debug.WriteLine("");
    }
  }

  /// <summary>
  /// Test of function StockService.CalculateInvestments.
  /// </summary>
  [Test]
  public void CalculateInvestments()
  {
    var date = new DateTime(2024, 12, 27);
    //// var search = "pmp amundi e%";
    //// var search = "DB Allianz Global Water Funds";
    var search = "PMP Broadcom Inc";
    var r = FactoryService.StockService.CalculateInvestments(daten, null, null, null, date, true, search, new StringBuilder(), new StringBuilder());
    r.ThrowAllErrors("CalculateInvestments");
  }

  /// <summary>
  /// Test of function DiaryService.GetWeatherList.
  /// </summary>
  [Test]
  public void GetWeatherList()
  {
    // var date = new DateTime(2024, 12, 30);
    var date = DateTime.Today;
    var r0 = FactoryService.DiaryService.GetPositionList(daten, null, "%IM4%");
    r0.ThrowAllErrors("GetPositionList");
    var puid = r0?.Ergebnis.FirstOrDefault()?.Uid;
    var r = FactoryService.DiaryService.GetWeatherList(daten, date, puid);
    r.ThrowAllErrors("GetWeatherList");
  }

  /// <summary>
  /// Calculate average temperatures.
  /// </summary>
  [Test]
  public void GetAverageTemperatureList()
  {
    var date = new DateTime(2025, 11, 8);
    var today = DateTime.Today;
    var r0 = FactoryService.DiaryService.GetPositionList(daten, null, "%IM4%");
    r0.ThrowAllErrors("GetPositionList");
    var puid = r0?.Ergebnis.FirstOrDefault()?.Uid;
    var cs = new CsvWriter();
    cs.AddCsvLine(["Datum", "Temperatur 6-22 Uhr (°C)"]);
    while (date <= today)
    {
      var r = FactoryService.DiaryService.GetWeatherList(daten, date, puid);
      if (r.Ok && r.Ergebnis != null)
      {
        var avg = r?.Ergebnis.Where(a => a.Time.Hour >= 6 && a.Time.Hour <= 22).Average(a => a.Temp);
        //// Debug.WriteLine($"Average temperature for {date:yyyy-MM-dd} is {avg: 0.00} °C");
        cs.AddCsvLine([Functions.ToString(date), Functions.ToString(avg)]);
        date = date.AddDays(1);
      }
      r.ThrowAllErrors("GetAverageTemperatureList");
    }
    var filename = $"/home/wolfgang/Downloads/AverageTemperature.csv";
    cs.WriteFile(filename);
  }
}
