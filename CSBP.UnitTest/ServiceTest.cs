// <copyright file="ServiceTest.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.UnitTest;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BlazorSpa.Base;
using BlazorSpa.Base.Csv;
using BlazorSpa.Base.Services;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Xunit;
using Xunit.Abstractions;

/// <summary>Test class for services.</summary>
public class ServiceTest : IDisposable
{
  private static ServiceDaten daten; // = new ServiceDaten(1, "Test", null);

  private readonly ITestOutputHelper output;

  /// <summary>
  /// Initializes a new instance of the <see cref="ServiceTest"/> class.
  /// </summary>
  /// <param name="output">The test output helper.</param>
  public ServiceTest(ITestOutputHelper output)
  {
    // 1. Setup: Wird vor jedem Test ausgeführt
    this.output = output;
    var connect = "Data Source=/home/wolfgang/hsqldb/csbp.db";
    Parameter.Connect = connect;
    daten = new ServiceDaten("0", 1, "Administrator", null);
    var r1 = FactoryService.ClientService.InitDb(daten);
    r1.ThrowAllErrors("InitDb");
    var r2 = FactoryService.ClientService.GetOptionList(daten, daten.MandantNr, Parameter.Params, null);
    r2.ThrowAllErrors("GetOptionList");
  }

  /// <summary>
  /// Disposes the instance of the <see cref="ServiceTest"/> class.
  /// </summary>
  public void Dispose()
  {
    // 3. TearDown: Wird nach jedem Test ausgeführt
  }

  /// <summary>
  /// Call of all Tests.
  /// </summary>
  [Fact]
  public void TestAll()
  {
    // GetMileages();
    // CalculateInvestments();
    // GetWeatherList();
    GetAverageTemperatureList();
  }

  /// <summary>
  /// Call of all Tests.
  /// </summary>
  [Fact]
  public void BalanceTest()
  {
    var rs = StatusTask.HinzufuegenFunktion(daten.MandantNr, "AnalyzeBalances", "0");
    rs.ThrowAllErrors("HinzufuegenFunktion");
    var state = rs.Ergebnis;
    var r = FactoryService.BudgetService.AnalyzeBalances(daten, -1, -1, state);
    r.ThrowAllErrors("AnalyzeBalances");
    output.WriteLine($"AnalyzeBalances: {r.Ergebnis}");
  }

  /// <summary>
  /// Test of function PrivateService.GetMileages.
  /// </summary>
  [Fact]
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
  [Fact]
  public void CalculateInvestments()
  {
    var date = new DateTime(2024, 12, 27);
    //// var search = "pmp amundi e%";
    //// var search = "DB Allianz Global Water Funds";
    var search = "PMP Broadcom Inc";
    var rs = StatusTask.HinzufuegenFunktion(daten.MandantNr, "CalculateInvestments", "0");
    rs.ThrowAllErrors("HinzufuegenFunktion");
    var state = rs.Ergebnis;
    var r = FactoryService.StockService.CalculateInvestments(daten, null, null, null, date, true, search, state);
    r.ThrowAllErrors("CalculateInvestments");
  }

  /// <summary>
  /// Test of function DiaryService.GetWeatherList.
  /// </summary>
  [Fact]
  public void GetWeatherList()
  {
    // var date = new DateTime(2024, 12, 30);
    var date = DateTime.Today;
    var r0 = FactoryService.DiaryService.GetPositionList(daten, text: "%IM4%");
    r0.ThrowAllErrors("GetPositionList");
    var puid = r0?.Ergebnis.FirstOrDefault()?.Uid;
    var r = FactoryService.DiaryService.GetWeatherList(daten, date, puid);
    r.ThrowAllErrors("GetWeatherList");
  }

  /// <summary>
  /// Calculate average temperatures.
  /// </summary>
  [Fact]
  public void GetAverageTemperatureList()
  {
    var date = new DateTime(2026, 10, 30);
    var today = DateTime.Today;
    var r0 = FactoryService.DiaryService.GetPositionList(daten, text: "%IM4%");
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
        cs.AddCsvLine([Funktionen.ToString(date), Funktionen.ToString(avg)]);
        date = date.AddDays(1);
      }
      r.ThrowAllErrors("GetAverageTemperatureList");
    }
    var filename = $"/home/wolfgang/Downloads/AverageTemperature.csv";
    cs.WriteFile(filename);
  }
}
