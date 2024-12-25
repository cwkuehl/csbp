// <copyright file="ServiceTest.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.UnitTest;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CSBP.Services.Base;
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
    daten = new ServiceDaten(1, "Administrator", null);
    var r1 = FactoryService.ClientService.InitDb(daten);
    r1.ThrowAllErrors("InitDb");
    var r2 = FactoryService.ClientService.GetOptionList(daten, daten.MandantNr, Parameter.Params);
    r2.ThrowAllErrors("GetOptionList");
  }

  /// <summary>
  /// Call of all Tests.
  /// </summary>
  [Test]
  public void TestAll()
  {
    GetMileages();
  }

  /// <summary>
  /// Test of function PrivateService.GetMileages.
  /// </summary>
  [Test]
  public void GetMileages()
  {
    var dates = new List<DateTime> { new(2024, 12, 30), new(2024, 12, 31), new(2025, 1, 1) };
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
}
