// <copyright file="EnergyService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CSBP.Services.Apis.Models;
using CSBP.Services.Apis.Services;
using CSBP.Services.Base;
using CSBP.Services.Base.Csv;
using CSBP.Services.NonService;
using CSBP.Services.Pnf;
using CSBP.Services.Resources;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>
/// Implementation of energy service.
/// </summary>
public class EnergyService : ServiceBase, IEnergyService
{
  /// <summary>
  /// Returns a CSV file with all data of a form.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="page">Affected page, e.g. "AG100".</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <returns>CSV file as string.</returns>
  public ServiceErgebnis<string> GetCsvString(ServiceDaten daten, string page, TableReadModel rm)
  {
    var r = new ServiceErgebnis<string>();
    if (!(page == "EN100") || rm == null)
      return r;
    rm.NoPaging = true;
    var cs = new CsvWriter();
    if (page == "ENN100")
    {
      // TODO EN100
      var l = WpWertpapierRep.GetList(daten, rm, daten.MandantNr, null);
      cs.AddCsvLine(["Mandant_Nr", "Nr", "Sortierung", "Bezeichnung",
        "Status", "Provider", "Kürzel", "Typ", "Akt. Kurs", "Muster", "Währung",
        "Angelegt_Am", "Angelegt_Von", "Geaendert_Am", "Geaendert_Von"]);
      foreach (var o in l)
      {
        cs.AddCsvLine([Functions.ToString(daten.MandantNr), o.Uid, o.Sorting, o.Bezeichnung,
          CsbpBase.GetStockState(o.Status, o.Kuerzel), o.Datenquelle, o.Kuerzel, o.Type,
          Functions.ToString(o.CurrentPrice), o.Pattern, o.Currency,
          Functions.ToString(o.Angelegt_Am), o.Angelegt_Von, Functions.ToString(o.Geaendert_Am), o.Geaendert_Von]);
      }
    }
    r.Ergebnis = cs.GetContent();
    return r;
  }

  /// <summary>
  /// Gets a list of queries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="rm">Affected read model for filtering and sorting.</param>
  /// <param name="inactive">Gets also inactive investments or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of queries.</returns>
  public ServiceErgebnis<List<EnAbfrage>> GetQueryList(ServiceDaten daten, TableReadModel rm = null, bool inactive = false, string search = null)
  {
    // var r = new ServiceErgebnis<List<EnAbfrage>>(EnAbfrageRep.GetList(daten, rm, daten.MandantNr, inactive, search));
    var r = new ServiceErgebnis<List<EnAbfrage>>(EnAbfrageRep.GetList(daten, daten.MandantNr));
    return r;
  }

  /// <summary>
  /// Gets a query.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Query or null.</returns>
  public ServiceErgebnis<EnAbfrage> GetQuery(ServiceDaten daten, string uid)
  {
    var r = new ServiceErgebnis<EnAbfrage>(EnAbfrageRep.Get(daten, daten.MandantNr, uid));
    return r;
  }

  /// <summary>
  /// Saves a query.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Value of column Uid.</param>
  /// <param name="sortierung">Value of column Sortierung.</param>
  /// <param name="art">Value of column Art.</param>
  /// <param name="bezeichnung">Value of column Bezeichnung.</param>
  /// <param name="hosturl">Value of column Host_Url.</param>
  /// <param name="datentyp">Value of column Datentyp.</param>
  /// <param name="schreibbarkeit">Value of column Schreibbarkeit.</param>
  /// <param name="einheit">Value of column Einheit.</param>
  /// <param name="param1">Value of column Param1.</param>
  /// <param name="param2">Value of column Param2.</param>
  /// <param name="param3">Value of column Param3.</param>
  /// <param name="param4">Value of column Param4.</param>
  /// <param name="param5">Value of column Param5.</param>
  /// <param name="status">Value of column Status.</param>
  /// <param name="notiz">Value of column Notiz.</param>
  /// <returns>Created or changed query.</returns>
  public ServiceErgebnis<EnAbfrage> SaveQuery(ServiceDaten daten, string uid, string sortierung, string art, string bezeichnung, string hosturl, string datentyp, string schreibbarkeit, string einheit, string param1, string param2, string param3, string param4, string param5, string status, string notiz)
  {
    sortierung = sortierung.TrimNull();
    art = art.TrimNull();
    bezeichnung = bezeichnung.TrimNull();
    hosturl = hosturl.TrimNull();
    datentyp = datentyp.TrimNull();
    schreibbarkeit = schreibbarkeit.TrimNull();
    einheit = einheit.TrimNull();
    param1 = param1.TrimNull();
    param2 = param2.TrimNull();
    param3 = param3.TrimNull();
    param4 = param4.TrimNull();
    param5 = param5.TrimNull();
    status = status.TrimNull();
    notiz = notiz.TrimNull();
    var r = new ServiceErgebnis<EnAbfrage>();
    if (string.IsNullOrWhiteSpace(sortierung))
      r.Errors.Add(Message.New(EN001)); // TODO EN003: Sortierung ist erforderlich.
    if (string.IsNullOrWhiteSpace(art))
      r.Errors.Add(Message.New(EN001));
    if (string.IsNullOrWhiteSpace(bezeichnung))
      r.Errors.Add(Message.New(EN001));
    if (string.IsNullOrWhiteSpace(hosturl))
      r.Errors.Add(Message.New(EN001));
    if (string.IsNullOrWhiteSpace(datentyp))
      r.Errors.Add(Message.New(EN001));
    if (string.IsNullOrWhiteSpace(status))
      r.Errors.Add(Message.New(EN002));
    if (!r.Ok)
      return r;
    var e = (string.IsNullOrEmpty(uid) ? null
      : EnAbfrageRep.Get(daten, daten.MandantNr, uid, true)) ?? new EnAbfrage();
    r.Ergebnis = EnAbfrageRep.Save(daten, daten.MandantNr, uid, sortierung, art, bezeichnung, hosturl, datentyp, schreibbarkeit, einheit, param1, param2, param3, param4, param5, status, notiz, e.Parameter);
    return r;
  }

  /// <summary>
  /// Deletes a query.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteQuery(ServiceDaten daten, EnAbfrage e)
  {
    var r = new ServiceErgebnis();
    EnAbfrageRep.Delete(daten, e);
    return r;
  }

  /// <summary>
  /// Queries all queries.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="state">State of calculation is always updated, cancelling is possible.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis QueryQueries(ServiceDaten daten, bool inactive, string search, StatusTask state)
  {
    return null;
  }

  /// <summary>
  /// Gets a list of states.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of states.</returns>
  public ServiceErgebnis<List<MaParameter>> GetStateList(ServiceDaten daten)
  {
    var l = new List<MaParameter>
    {
      new() { Schluessel = "1", Wert = Enum_state_active },
      new() { Schluessel = "0", Wert = Enum_state_inactive },
      new() { Schluessel = "2", Wert = Enum_state_nocalc },
    };
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }

  /// <summary>
  /// Gets a list of kinds.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of kinds.</returns>
  public ServiceErgebnis<List<MaParameter>> GetKindList(ServiceDaten daten)
  {
    var l = new List<MaParameter>
    {
      new() { Schluessel = "MODBUS-TCP", Wert = "Modbus-TCP" },
      new() { Schluessel = "JSON", Wert = "HTTP(S)-JSON" },
    };
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }

  /// <summary>
  /// Gets a list of datatypes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of datatypes.</returns>
  public ServiceErgebnis<List<MaParameter>> GetDatatypeList(ServiceDaten daten)
  {
    var l = new List<MaParameter>
    {
      new() { Schluessel = "int16", Wert = "int16, Integer 16" },
      new() { Schluessel = "int32", Wert = "int32, Integer 32" },
      new() { Schluessel = "uint16", Wert = "uint16, Unsigned Integer 16" },
    };
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }
}
