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
using AMWD.Protocols.Modbus.Common;
using AMWD.Protocols.Modbus.Tcp;
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
    var r = new ServiceErgebnis<List<EnAbfrage>>(EnAbfrageRep.GetList(daten, rm, !inactive, search));
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
    var r = new ServiceErgebnis();
    var l = EnAbfrageRep.GetList(daten, null, !inactive, search);
    var m1 = true;
    foreach (var q in l)
    {
      if (state.IstAbbruch())
        break;
      if (q.Art == "MODBUS-TCP")
      {
        var arr = q.Host_Url.Split(':');
        var host = arr[0];
        var port = arr.Length > 1 ? Functions.ToInt32(arr[1]) : 502;
        using var client = new ModbusTcpClient(host, port);
        byte unitId = q.Param1 != null ? (byte)Functions.ToInt32(q.Param1) : (byte)1;
        ushort address = q.Param2 != null ? (ushort)Functions.ToInt32(q.Param2) : (ushort)1;
        ushort count = q.Param3 != null ? (ushort)Functions.ToInt32(q.Param3) : (ushort)1;
        var registers = client.ReadHoldingRegistersAsync(unitId, address, count).GetAwaiter().GetResult(); // .Select(r => r.Value).ToArray();
        var wert = GetDatentypValue(q, registers);
        var einheit = string.IsNullOrWhiteSpace(q.Einheit) ? "" : $" {q.Einheit}";
        ////state.SetMeldung($"Modbus-TCP: {q.Bezeichnung}, {host}:{port}, UnitId={unitId}, Address={address}, Count={count}, Registers={string.Join(", ", registers)}");
        // var v = switch (q.Datentyp)
        // {
        //   default:
        //     return "TODO EN004: Datentyp {0} nicht unterstützt.".FormatWith(q.Datentyp);
        // }
        state.SetMeldung($"{Functions.Iif(m1, "", "  ")}{q.Bezeichnung}: {wert}{einheit}", true, "  ");
        m1 = false;
      }
    }
    return r;
  }

  private static string GetDatentypValue(EnAbfrage q, IReadOnlyList<HoldingRegister> registers)
  {
    var arr = q.Datentyp.Split('|');
    var dt = arr[0];
    var az = arr.Length > 1 ? arr[1] : null; // Enum, Aufzählung
    var faktor = Functions.ToDecimal(q.Param4) ?? 0m;
    var d = 0m;
    if (dt == "uint16")
    {
      d = registers[0].GetUInt16();
    }
    else if (dt == "int16")
    {
      d = registers[0].GetInt16();
    }
    else if (dt == "int32")
    {
      d = registers.GetInt32();
    }
    else if (dt == "decimal")
    {
      d = (decimal)registers.GetDouble();
    }
    else if (dt == "string")
    {
      // var l = registers.Count;
      // var s = registers.GetString(l, reverseByteOrderPerRegister: true);
      var sb = new StringBuilder();
      foreach (var r in registers)
        sb.Append((char)r.Value);
      var s2 = sb.ToString().TrimEnd('\0');
      return s2;
    }
    else
      return $"Datentyp {q.Datentyp} nicht unterstützt.";

    if (faktor != 0)
      d *= faktor;
    string sw;
    if (!string.IsNullOrWhiteSpace(q.Param5))
      sw = d.ToString(q.Param5);
    else
      sw = Functions.ToString(d, 0);
    if (az != null)
    {
      var wen = Functions.Between(az, $"{sw}=", ";");
      if (wen == null)
        wen = Functions.Between(az, "_=", ";");
      if (wen != null)
        sw = wen;
    }
    return sw;
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
      new() { Schluessel = "decimal", Wert = "decimal, Dezimalzahl" },
      new() { Schluessel = "string", Wert = "string, Zeichenkette" },
    };
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }
}
