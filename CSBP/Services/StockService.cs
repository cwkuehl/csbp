// <copyright file="StockService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Base;
using CSBP.Services.Pnf;
using Newtonsoft.Json.Linq;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>
/// Implementation of stock service.
/// </summary>
public class StockService : ServiceBase, IStockService
{
  /// <summary>Dictionary for exchange rates.</summary>
  private static readonly Dictionary<string, SoKurse> Wkurse = new();

  /// <summary>Sets budget service.</summary>
  public IBudgetService BudgetService { private get; set; }

  /// <summary>
  /// Gets a list of stocks.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inactive">Gets also inactive investments or not.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="pattern">Affected Pattern.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of stocks.</returns>
  public ServiceErgebnis<List<WpWertpapier>> GetStockList(ServiceDaten daten, bool inactive,
      string desc = null, string pattern = null, string uid = null, string search = null)
  {
    var l = WpWertpapierRep.GetList(daten, daten.MandantNr, desc, pattern, null, uid, !inactive, search);
    foreach (var w in l)
    {
      w.Description = w.Bezeichnung;
    }
    return new ServiceErgebnis<List<WpWertpapier>>(l);
  }

  /// <summary>
  /// Gets a stock.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Stock or null.</returns>
  public ServiceErgebnis<WpWertpapier> GetStock(ServiceDaten daten, string uid)
  {
    var l = WpWertpapierRep.GetList(daten, daten.MandantNr, null, null, uid);
    var e = l.FirstOrDefault();
    return new ServiceErgebnis<WpWertpapier>(e);
  }

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
  /// <param name="relationuid">Affected relation uid.</param>
  /// <param name="notice">Affected memo.</param>
  /// <param name="type">Affected type.</param>
  /// <param name="currency">Affected currency.</param>
  /// <param name="inv">Create also an investment or not.</param>
  /// <returns>Created or changed stock.</returns>
  public ServiceErgebnis<WpWertpapier> SaveStock(ServiceDaten daten, string uid, string desc,
    string abbreviation, decimal? signal1, string sort, string source, string state,
    string relationuid, string notice, string type = null, string currency = null, bool inv = false)
  {
    desc = desc.TrimNull();
    abbreviation = abbreviation.TrimNull();
    sort = sort.TrimNull();
    source = source.TrimNull();
    state = state.TrimNull();
    relationuid = relationuid.TrimNull();
    notice = notice.TrimNull();
    type = type.TrimNull();
    currency = currency.TrimNull();
    var r = new ServiceErgebnis<WpWertpapier>();
    if (string.IsNullOrWhiteSpace(desc))
      r.Errors.Add(Message.New(WP001));
    if (string.IsNullOrWhiteSpace(state))
      r.Errors.Add(Message.New(WP002));
    if (string.IsNullOrWhiteSpace(source))
    {
      source = "";
      abbreviation = "";
    }
    else if (string.IsNullOrWhiteSpace(abbreviation))
      r.Errors.Add(Message.New(WP014));
    if (!r.Ok)
      return r;
    var wp = (string.IsNullOrEmpty(uid) ? null
        : WpWertpapierRep.Get(daten, daten.MandantNr, uid, true)) ?? new WpWertpapier();
    wp.SignalPrice1 = signal1;
    wp.Sorting = sort;
    wp.Type = type;
    wp.Currency = currency;
    r.Ergebnis = WpWertpapierRep.Save(daten, daten.MandantNr, uid, desc, abbreviation, wp.Parameter,
      source, state, relationuid, notice);
    if (inv && r.Ergebnis != null)
    {
      // Creates an investment.
      SaveChanges(daten);
      var st = r.Ergebnis;
      string parameter = null;
      if (!string.IsNullOrEmpty(st.Sorting))
      {
        var ilist = WpAnlageRep.GetList(daten, daten.MandantNr, st.Sorting.ToUpper() + " %");
        var i = ilist.FirstOrDefault(a => !string.IsNullOrEmpty(a.PortfolioAccountUid) && a.State == 1);
        if (i != null)
        {
          var i0 = new WpAnlage
          {
            State = 1,
            PortfolioAccountUid = i.PortfolioAccountUid,
            SettlementAccountUid = i.SettlementAccountUid,
            IncomeAccountUid = i.IncomeAccountUid,
          };
          parameter = i0.Parameter;
        }
      }
      var bez = (string.IsNullOrEmpty(st.Sorting) ? "" : st.Sorting.ToUpper() + " ") + st.Bezeichnung;
      WpAnlageRep.Save(daten, st.Mandant_Nr, null, st.Uid, bez, parameter, null);
    }
    return r;
  }

  /// <summary>
  /// Deletes a stock.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteStock(ServiceDaten daten, WpWertpapier e)
  {
    var r = new ServiceErgebnis();
    var slist = WpWertpapierRep.GetList(daten, daten.MandantNr, null, reuid: e.Uid);
    if (slist.Any())
      r.Errors.Add(Message.New(WP015));
    var ilist = WpAnlageRep.GetList(daten, daten.MandantNr, null, stuid: e.Uid);
    if (ilist.Any())
      r.Errors.Add(Message.New(WP016));
    if (!r.Ok)
      return r;
    var plist = WpStandRep.GetList(daten, daten.MandantNr, null, null, e.Uid);
    //// Deletes prices.
    foreach (var p in plist)
      WpStandRep.Delete(daten, p);
    WpWertpapierRep.Delete(daten, e);
    return r;
  }

  /// <summary>
  /// Calculates all stocks.
  /// </summary>
  /// <returns>Possibly errors.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="pattern">Affected pattern.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="cuid">Affected configuration ID.</param>
  /// <param name="state">State of calculation is always updated.</param>
  /// <param name="cancel">Cancel calculation if not empty.</param>
  public ServiceErgebnis CalculateStocks(ServiceDaten daten, string desc, string pattern, string stuid,
    DateTime date, bool inactive, string search, string cuid, StringBuilder state, StringBuilder cancel)
  {
    if (state == null || cancel == null)
      throw new ArgumentException(null, nameof(state));
    CalculateStocksIntern(daten, desc, pattern, stuid, date, inactive, search, cuid, state, cancel);
    var r = new ServiceErgebnis();
    return r;
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
      new MaParameter { Schluessel = "1", Wert = Enum_state_active },
      new MaParameter { Schluessel = "0", Wert = Enum_state_inactive },
      new MaParameter { Schluessel = "2", Wert = Enum_state_nocalc },
    };
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }

  /// <summary>
  /// Gets a list of scales.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of scales.</returns>
  public ServiceErgebnis<List<MaParameter>> GetScaleList(ServiceDaten daten)
  {
    var list = new List<MaParameter>
    {
      new MaParameter { Schluessel = "0", Wert = Enum_scale_fix },
      new MaParameter { Schluessel = "1", Wert = Enum_scale_pc },
      new MaParameter { Schluessel = "2", Wert = Enum_scale_dyn },
    };
    return new ServiceErgebnis<List<MaParameter>>(list);
  }

  /// <summary>
  /// Gets a list of PnF methods.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of PnF methods.</returns>
  public ServiceErgebnis<List<MaParameter>> GetMethodList(ServiceDaten daten)
  {
    var list = new List<MaParameter>
    {
      new MaParameter { Schluessel = "1", Wert = Enum_method_c },
      new MaParameter { Schluessel = "2", Wert = Enum_method_hl },
      new MaParameter { Schluessel = "3", Wert = Enum_method_hlr },
      new MaParameter { Schluessel = "4", Wert = Enum_method_ohlc },
      new MaParameter { Schluessel = "5", Wert = Enum_method_tp },
    };
    return new ServiceErgebnis<List<MaParameter>>(list);
  }

  /// <summary>
  /// Gets a list of providers.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>List of providers.</returns>
  public ServiceErgebnis<List<MaParameter>> GetProviderList(ServiceDaten daten)
  {
    var l = new List<MaParameter>
    {
      new MaParameter { Schluessel = "onvista", Wert = "Onvista Fonds, www.onvista.de" },
      new MaParameter { Schluessel = "yahoo", Wert = "Yahoo, finance.yahoo.com" },
      new MaParameter { Schluessel = "ariva", Wert = "Ariva, www.ariva.de" },
    };
    var r = new ServiceErgebnis<List<MaParameter>>(l);
    return r;
  }

  /// <summary>
  /// Gets a list of configurations.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="extended">Gets more data or not.</param>
  /// <param name="state">Affected configuration state.</param>
  /// <returns>List of configurations.</returns>
  public ServiceErgebnis<List<WpKonfiguration>> GetConfigurationList(ServiceDaten daten, bool extended, string state)
  {
    var l = WpKonfigurationRep.GetList(daten, daten.MandantNr);
    return new ServiceErgebnis<List<WpKonfiguration>>(l);
  }

  /// <summary>
  /// Gets a configuration.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="def">Returns defaults if empty.</param>
  /// <returns>Configurations or null.</returns>
  public ServiceErgebnis<WpKonfiguration> GetConfiguration(ServiceDaten daten, string uid, bool def = false)
  {
    WpKonfiguration e = null;
    if (!string.IsNullOrEmpty(uid))
      e = WpKonfigurationRep.Get(daten, daten.MandantNr, uid);
    if (e == null && def)
      e = new WpKonfiguration
      {
        Box = 1,
        Reversal = 3,
        Method = 1, // Schlusskurse
        Duration = Constants.STOCK_DAYS,
        Relative = false,
        Scale = 2, // dynamisch
        Status = "1",
      };
    return new ServiceErgebnis<WpKonfiguration>(e);
  }

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
  /// <param name="relative">Is it relative calculation or not.</param>
  /// <param name="scale">Affected scale.</param>
  /// <param name="state">Affected state.</param>
  /// <param name="notice">Affected notice.</param>
  /// <returns>Created or changed configuration.</returns>
  public ServiceErgebnis<WpKonfiguration> SaveConfiguration(ServiceDaten daten, string uid, string desc,
      decimal box, int reversal, int method, int duration, bool relative, int scale,
      string state, string notice)
  {
    var r = new ServiceErgebnis<WpKonfiguration>();
    desc = desc.TrimNull();
    notice = notice.TrimNull();
    if (string.IsNullOrWhiteSpace(desc))
      r.Errors.Add(Message.New(WP001));
    if (string.IsNullOrWhiteSpace(state))
      r.Errors.Add(Message.New(WP002));
    if (Functions.CompDouble4(box, 0) <= 0)
      r.Errors.Add(Message.New(WP003));
    if (reversal <= 0)
      r.Errors.Add(Message.New(WP004));
    if (method < 1 || method > 5)
      r.Errors.Add(Message.New(WP005));
    if (duration <= 10)
      r.Errors.Add(Message.New(WP006));
    if (scale < 0 || scale > 2)
      r.Errors.Add(Message.New(WP007));
    if (!r.Ok)
      return r;
    var k = (string.IsNullOrEmpty(uid) ? null
      : WpKonfigurationRep.Get(daten, daten.MandantNr, uid, true)) ?? new WpKonfiguration();
    k.Box = box;
    k.Scale = scale;
    k.Reversal = reversal;
    k.Method = method;
    k.Duration = duration;
    k.Relative = relative;
    r.Ergebnis = WpKonfigurationRep.Save(daten, daten.MandantNr, uid, desc, k.Parameter, state, notice);
    return r;
  }

  /// <summary>
  /// Deletes a configuration.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteConfiguration(ServiceDaten daten, WpKonfiguration e)
  {
    WpKonfigurationRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of prices or rates for a period.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Beginning of the period.</param>
  /// <param name="to">End of the period.</param>
  /// <param name="uid">Affected stock uid.</param>
  /// <param name="relative">Relative prices to relation or not.</param>
  /// <returns>List of prices.</returns>
  public ServiceErgebnis<List<SoKurse>> GetPriceList(ServiceDaten daten, DateTime from, DateTime to,
      string uid, bool relative)
  {
    var l = new List<SoKurse>();
    var r = new ServiceErgebnis<List<SoKurse>>(l);
    WpWertpapier st = null;
    WpWertpapier str = null;
    if (!string.IsNullOrEmpty(uid))
      st = WpWertpapierRep.Get(daten, daten.MandantNr, uid);
    if (st == null)
      return r;
    if (relative && !string.IsNullOrEmpty(st.Relation_Uid))
      _ = WpWertpapierRep.Get(daten, daten.MandantNr, str.Relation_Uid);
    r.Ergebnis = GetPriceListIntern(daten, from, to, st.Datenquelle, st.Kuerzel, st.Type, st.Currency, 0);
    return r;
  }

  /// <summary>
  /// Gets a list of investments.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inactive">Gets also inactive investments or not.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="search">Affected text search.</param>
  /// <returns>List of investments.</returns>
  public ServiceErgebnis<List<WpAnlage>> GetInvestmentList(ServiceDaten daten, bool inactive,
      string desc = null, string uid = null, string stuid = null, string search = null)
  {
    var l = WpAnlageRep.GetList(daten, daten.MandantNr, desc, uid, stuid, search);
    if (!inactive)
      l = l.Where(a => a.State != 0).ToList();
    var r = new ServiceErgebnis<List<WpAnlage>>(l);
    return r;
  }

  /// <summary>
  /// Gets a investment.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Investment or null.</returns>
  public ServiceErgebnis<WpAnlage> GetInvestment(ServiceDaten daten, string uid)
  {
    var l = WpAnlageRep.GetList(daten, daten.MandantNr, null, uid);
    var e = l.FirstOrDefault();
    if (e != null)
    {
      var p0 = e.PriceDate.HasValue ? WP026(e.PriceDate.Value) : null;
      var pm = e.MinDate.HasValue ? WP051(e.MinDate.Value) : null;
      var p1 = string.IsNullOrEmpty(e.Currency) ? p0 : WP025(e.Currency, e.CurrencyPrice, p0);
      var p2 = WP024(e.Price, p1, e.Value, e.Profit, e.ProfitPercent);
      var p3 = e.PriceDate2.HasValue ? WP028(e.Value2, e.Value - e.Value2, e.PriceDate2.Value) : null;
      e.Data = WP023(e.Payment, pm, e.Shares, e.ShareValue, e.Interest, p2, p3);
    }
    return new ServiceErgebnis<WpAnlage>(e);
  }

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
  public ServiceErgebnis<WpAnlage> SaveInvestment(ServiceDaten daten, string uid, string stuid,
    string desc, string memo, int state, string pfuid, string smuid, string icuid,
    DateTime? valuta, decimal value)
  {
    var r = new ServiceErgebnis<WpAnlage>();
    desc = desc.TrimNull();
    memo = memo.TrimNull();
    if (string.IsNullOrWhiteSpace(desc))
      r.Errors.Add(Message.New(WP001));
    //// if (!Global.nes(bez) && bez.length > WpAnlage.BEZEICHNUNG_LAENGE)
    ////    throw new MeldungException(Meldungen::WP050);
    if (string.IsNullOrEmpty(stuid) || WpWertpapierRep.Get(daten, daten.MandantNr, stuid) == null)
      r.Errors.Add(Message.New(WP017));
    if (string.IsNullOrEmpty(pfuid) != string.IsNullOrEmpty(smuid) || string.IsNullOrEmpty(smuid) != string.IsNullOrEmpty(icuid)
      || (!string.IsNullOrEmpty(pfuid) && (pfuid == smuid || smuid == icuid)))
      r.Errors.Add(Message.New(WP055));
    if (!string.IsNullOrEmpty(pfuid) && HhKontoRep.Get(daten, daten.MandantNr, pfuid) == null)
      r.Errors.Add(Message.New(HH019(pfuid)));
    if (!string.IsNullOrEmpty(smuid) && HhKontoRep.Get(daten, daten.MandantNr, smuid) == null)
      r.Errors.Add(Message.New(HH019(smuid)));
    if (!string.IsNullOrEmpty(icuid) && HhKontoRep.Get(daten, daten.MandantNr, icuid) == null)
      r.Errors.Add(Message.New(HH019(icuid)));
    if (!r.Ok)
      return r;
    var inv = (string.IsNullOrEmpty(uid) ? null
      : WpAnlageRep.Get(daten, daten.MandantNr, uid, true)) ?? new WpAnlage();
    inv.State = state;
    inv.PortfolioAccountUid = pfuid;
    inv.SettlementAccountUid = smuid;
    inv.IncomeAccountUid = icuid;
    r.Ergebnis = WpAnlageRep.Save(daten, daten.MandantNr, uid, stuid, desc, inv.Parameter, memo);
    if (valuta.HasValue && value > 0 && inv.Shares > 0)
    {
      SaveChanges(daten);
      var i = r.Ergebnis;
      var sv = Functions.Round4(value / i.Shares) ?? 0;
      WpStandRep.Save(daten, daten.MandantNr, i.Wertpapier_Uid, valuta.Value, sv);
    }
    return r;
  }

  /// <summary>
  /// Deletes an investment.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteInvestment(ServiceDaten daten, WpAnlage e)
  {
    var r = new ServiceErgebnis();
    var l = WpBuchungRep.GetList(daten, daten.MandantNr, null, inuid: e.Uid);
    if (l.Any())
      r.Errors.Add(Message.New(WP018));
    if (r.Ok)
      WpAnlageRep.Delete(daten, e);
    return r;
  }

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
  /// <param name="state">State of calculation is always updated.</param>
  /// <param name="cancel">Cancel calculation if not empty.</param>
  public ServiceErgebnis CalculateInvestments(ServiceDaten daten, string desc, string uid, string stuid,
      DateTime date, bool inactive, string search, StringBuilder state, StringBuilder cancel)
  {
    if (state == null || cancel == null)
      throw new ArgumentException("status, cancel");
    var from = date.AddDays(-7);
    var dictlist = new Dictionary<string, List<SoKurse>>();
    var dictresponse = new Dictionary<string, StockUrl>();
    var list = WpAnlageRep.GetList(daten, daten.MandantNr, desc, uid, stuid, search);
    if (!inactive && list.Count != 1)
    {
      // Calculate only active investments. No filtering with && !UiFunctions.IgnoreShortcut(a.StockShortcut) to use existing prices.
      list = list.Where(a => a.State == 1).ToList();
    }
    var l = list.Count;
    state.Clear().Append(M0(WP053));
    Gtk.Application.Invoke((sender, e) =>
    {
      MainClass.MainWindow.SetError(state.ToString());
    });
    for (var i = 0; i < l && cancel.Length <= 0; i++)
    {
      var inv = list[i];
      var ulist = GetPriceUrlsIntern(from, date, inv.StockProvider, inv.StockShortcut, inv.StockType);
      foreach (var (udate, url) in ulist)
      {
        var su = new StockUrl
        {
          Uid = inv.Wertpapier_Uid,
          Description = inv.Bezeichnung,
          Date = udate,
          Url = url,
        };
        if (!dictresponse.TryGetValue(su.Key, out var du))
        {
          dictresponse.Add(su.Key, su);
        }
      }
    }
    Debug.Print($"{DateTime.Now} Start.");
    var l1 = dictresponse.Count;
    var i1 = 1;
    foreach (var su in dictresponse.Values)
    {
      if (cancel.Length > 0)
        break;
      su.Task = ExecuteHttpsClient(su.Url);
      state.Clear().Append(WP008(i1, l1, su.Description, su.Date, null));
      Gtk.Application.Invoke((sender, e) =>
      {
        MainClass.MainWindow.SetError(state.ToString());
      });
      if (i1 < l1)
      {
        // Verzögerung wegen onvista.de notwendig. 500 OK.
        Thread.Sleep(HttpDelay);
      }
      i1++;
    }
    if (cancel.Length <= 0)
    {
      var tasks = dictresponse.Values.Select(a => a.Task).ToArray();
      Task.WaitAll(tasks, HttpTimeout);
      foreach (var su in dictresponse.Values)
      {
        su.Response = su.Task.Result;
        su.Task.Dispose();
        su.Task = null;
      }
    }
    Debug.Print($"{DateTime.Now} End.");

    for (var i = 0; i < l && cancel.Length <= 0; i++)
    {
      // Kurse berechnen.
      var inv = list[i];
      state.Clear().Append(WP009(i + 1, l, inv.Bezeichnung, date, null));
      Gtk.Application.Invoke((sender, e) =>
      {
        MainClass.MainWindow.SetError(state.ToString());
      });
      var blist = WpBuchungRep.GetList(daten, inv.Mandant_Nr, null, inuid: inv.Uid, to: date);
      inv.MinDate = blist.FirstOrDefault()?.Datum;
      if (!dictlist.TryGetValue(inv.Wertpapier_Uid, out var klist))
      {
        string response = null;
        if (dictresponse.TryGetValue(StockUrl.GetKey(inv.Wertpapier_Uid, date), out var resp))
          response = resp.Response;
        var pl = GetPriceListIntern(daten, from, date, inv.StockProvider, inv.StockShortcut, inv.StockType,
          inv.StockCurrency, inv.Price, inv.Wertpapier_Uid, dictresponse);
        klist = pl.Skip(Math.Max(0, pl.Count - 2)).ToList(); // Max. die letzten beiden Kurse.
        dictlist.Add(inv.Wertpapier_Uid, klist);
      }
      SoKurse k = null;
      SoKurse k1 = null;
      if (klist != null)
      {
        if (klist.Count >= 2)
        {
          k = klist[^1];
          k1 = klist[^2];
        }
        else if (klist.Count >= 1)
          k = klist[^1];
      }
      if (k == null)
      {
        var s = WpStandRep.GetLatest(daten, daten.MandantNr, inv.Wertpapier_Uid, date);
        if (s != null)
        {
          k = new SoKurse
          {
            Close = s.Stueckpreis,
            Datum = s.Datum,
            Bewertung = "EUR",
            Price = 1,
          };
        }
      }
      if (k == null)
      {
        k = new SoKurse
        {
          Close = inv.ShareValue,
          Datum = inv.MinDate ?? daten.Heute,
          Bewertung = "EUR",
          Price = 1,
        };
      }
      CalculateInvestment(daten, inv, blist, k);
      if (k1 != null)
      {
        var inv2 = Clone(inv);
        CalculateInvestment(daten, inv2, blist, k1);
        inv.PriceDate2 = inv2.PriceDate;
        inv.Value2 = inv2.Value;
      }
      WpAnlageRep.Update(daten, inv);
      if (inv.PriceDate.HasValue && Functions.CompDouble4(inv.Price, 0) > 0)
      {
        // Stand speichern
        WpStandRep.Save(daten, daten.MandantNr, inv.Wertpapier_Uid, inv.PriceDate.Value, inv.Price);
        SaveChanges(daten);
      }
    }
    state.Clear();
    var r = new ServiceErgebnis();
    return r;
  }

  /// <summary>
  /// Gets a list of bookings.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected Description.</param>
  /// <param name="uid">Affected ID.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="inuid">Affected investment ID.</param>
  /// <returns>List of bookings.</returns>
  public ServiceErgebnis<List<WpBuchung>> GetBookingList(ServiceDaten daten,
      string desc = null, string uid = null, string stuid = null, string inuid = null)
  {
    var r = new ServiceErgebnis<List<WpBuchung>>(WpBuchungRep.GetList(daten, daten.MandantNr,
        desc, uid, stuid, inuid, desc: true));
    return r;
  }

  /// <summary>
  /// Gets a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected ID.</param>
  /// <returns>Booking or null.</returns>
  public ServiceErgebnis<WpBuchung> GetBooking(ServiceDaten daten, string uid)
  {
    var l = WpBuchungRep.GetList(daten, daten.MandantNr, null, uid).FirstOrDefault();
    var r = new ServiceErgebnis<WpBuchung>(l);
    return r;
  }

  /// <summary>
  /// Gets list of events for an investment.
  /// </summary>
  /// <returns>List of events.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="iuid">Affected investment ID.</param>
  public ServiceErgebnis<List<HhEreignis>> GetEventList(ServiceDaten daten, string iuid)
  {
    var r = new ServiceErgebnis<List<HhEreignis>>(new List<HhEreignis>());
    var inv = WpAnlageRep.Get(daten, daten.MandantNr, iuid);
    if (inv != null && !string.IsNullOrEmpty(inv.PortfolioAccountUid)
      && !string.IsNullOrEmpty(inv.SettlementAccountUid) && !string.IsNullOrEmpty(inv.IncomeAccountUid))
    {
      var st = WpWertpapierRep.Get(daten, daten.MandantNr, inv.Wertpapier_Uid);
      var anleihe = st != null && !string.IsNullOrEmpty(st.Type) && st.Type.StartsWith("B");
      if (!anleihe)
      {
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = "Kauf aus Sparplan",
          EText = "1",
          Soll_Konto_Uid = inv.PortfolioAccountUid,
          Haben_Konto_Uid = inv.SettlementAccountUid,
        });
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = "Kauf aus Ertrag",
          EText = "1",
          Soll_Konto_Uid = inv.PortfolioAccountUid,
          Haben_Konto_Uid = inv.SettlementAccountUid,
        });
      }
      r.Ergebnis.Add(new HhEreignis
      {
        Uid = Functions.GetUid(),
        Bezeichnung = anleihe ? "Anleihekauf" : "Wertpapierkauf",
        EText = "1",
        Soll_Konto_Uid = inv.PortfolioAccountUid,
        Haben_Konto_Uid = inv.SettlementAccountUid,
      });
      r.Ergebnis.Add(new HhEreignis
      {
        Uid = Functions.GetUid(),
        Bezeichnung = anleihe ? "Kupongutschrift" : "Dividendengutschrift",
        EText = "2",
        Soll_Konto_Uid = inv.SettlementAccountUid,
        Haben_Konto_Uid = inv.IncomeAccountUid,
      });
      if (!anleihe)
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = "Ertragsgutschrift",
          EText = "2",
          Soll_Konto_Uid = inv.SettlementAccountUid,
          Haben_Konto_Uid = inv.IncomeAccountUid,
        });
      r.Ergebnis.Add(new HhEreignis
      {
        Uid = Functions.GetUid(),
        Bezeichnung = "Depotgebühren",
        EText = "2",
        Soll_Konto_Uid = inv.IncomeAccountUid,
        Haben_Konto_Uid = inv.SettlementAccountUid,
      });
      r.Ergebnis.Add(new HhEreignis
      {
        Uid = Functions.GetUid(),
        Bezeichnung = anleihe ? "Anleiheverkauf" : "Wertpapierverkauf",
        EText = "1",
        Soll_Konto_Uid = inv.SettlementAccountUid,
        Haben_Konto_Uid = inv.PortfolioAccountUid,
      });
    }
    return r;
  }

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
  public ServiceErgebnis<WpBuchung> SaveBooking(ServiceDaten daten, string uid, string inuid,
    DateTime date, decimal payment, decimal discount, decimal shares, decimal interest,
    string desc, string memo, decimal price,
    string buid, DateTime vd, decimal v, string duid, string cuid, string text)
  {
    var r = new ServiceErgebnis<WpBuchung>();
    desc = desc.TrimNull();
    //// memo = memo.TrimNull();
    var inv = string.IsNullOrWhiteSpace(inuid) ? null : WpAnlageRep.Get(daten, daten.MandantNr, inuid);
    if (inv == null)
      r.Errors.Add(Message.New(WP019));
    //// if (datum === null) {
    ////    throw new MeldungException(Meldungen::WP020);
    ////}
    if (string.IsNullOrWhiteSpace(desc))
      r.Errors.Add(Message.New(WP021));
    if (Functions.CompDouble(payment, 0) == 0 && Functions.CompDouble(discount, 0) == 0 &&
        Functions.CompDouble4(shares, 0) == 0 && Functions.CompDouble(interest, 0) == 0)
      r.Errors.Add(Message.New(WP022));
    if (!r.Ok)
      return r;

    if (Functions.CompDouble(v, 0) != 0 && string.IsNullOrEmpty(buid)
        && !string.IsNullOrEmpty(duid) && !string.IsNullOrEmpty(cuid) && !string.IsNullOrEmpty(text))
    {
      var vdm = Functions.KonvDM(v);
      var btext = $"{text} {inv?.Bezeichnung}";
      var r1 = BudgetService.SaveBooking(daten, buid, vd, vdm, v, duid, cuid, btext, null, vd);
      r.Get(r1);
      buid = r1?.Ergebnis?.Uid;
      if (!r.Ok)
        return r;
    }
    var balt = string.IsNullOrWhiteSpace(uid) ? null : WpBuchungRep.Get(daten, daten.MandantNr, uid);
    var bold = balt ?? new WpBuchung();
    bold.BookingUid = buid;
    memo = bold.Notiz;
    r.Ergebnis = WpBuchungRep.Save(daten, daten.MandantNr, uid, inv.Wertpapier_Uid, inuid, date, payment,
        discount, shares, interest, desc, memo);

    // Stand korrigieren
    var stand2 = price;
    if (balt != null && balt.Datum != date)
    {
      var st = WpStandRep.Get(daten, daten.MandantNr, balt.Wertpapier_Uid, balt.Datum);
      if (st != null)
      {
        if (balt.Wertpapier_Uid == inv.Wertpapier_Uid && price == 0)
        {
          // Datum-Änderung nimmt den Stand mit
          stand2 = st.Stueckpreis;
          WpStandRep.Delete(daten, st);
        }
        if (balt.Wertpapier_Uid != inv.Wertpapier_Uid)
          WpStandRep.Delete(daten, st);
      }
    }
    if (Functions.CompDouble(stand2, 0) <= 0)
    {
      var st = WpStandRep.Get(daten, daten.MandantNr, inv.Wertpapier_Uid, date);
      if (st != null)
        WpStandRep.Delete(daten, st);
    }
    else
      WpStandRep.Save(daten, daten.MandantNr, inv.Wertpapier_Uid, date, stand2, null, null, null, null);
    return r;
  }

  /// <summary>
  /// Deletes a booking.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeleteBooking(ServiceDaten daten, WpBuchung e)
  {
    WpBuchungRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

  /// <summary>
  /// Gets a list of prices.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="uid">Affected stock ID.</param>
  /// <param name="from">Beginning of the period.</param>
  /// <param name="to">End of the period.</param>
  /// <returns>List of prices.</returns>
  public ServiceErgebnis<List<WpStand>> GetPriceList(ServiceDaten daten, string uid = null,
      DateTime? from = null, DateTime? to = null)
  {
    var r = new ServiceErgebnis<List<WpStand>>();
    var max = string.IsNullOrWhiteSpace(uid) && !from.HasValue && !to.HasValue ? 100 : 0;
    r.Ergebnis = WpStandRep.GetList(daten, daten.MandantNr, from, to, uid, max);
    return r;
  }

  /// <summary>
  /// Gets a price.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <returns>Price or null.</returns>
  public ServiceErgebnis<WpStand> GetPrice(ServiceDaten daten, string stuid, DateTime date)
  {
    var r = new ServiceErgebnis<WpStand>(WpStandRep.Get(daten, daten.MandantNr, stuid, date));
    return r;
  }

  /// <summary>
  /// Saves a price.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="stuid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="price">Affected price.</param>
  /// <returns>Created or changed price.</returns>
  public ServiceErgebnis<WpStand> SavePrice(ServiceDaten daten, string stuid, DateTime date,
      decimal price)
  {
    var r = new ServiceErgebnis<WpStand>();
    if (string.IsNullOrWhiteSpace(stuid) || WpWertpapierRep.Get(daten, daten.MandantNr, stuid) == null)
      r.Errors.Add(Message.New(WP017));
    if (price <= 0)
      r.Errors.Add(Message.New(WP052));
    if (!r.Ok)
      return r;
    r.Ergebnis = WpStandRep.Save(daten, daten.MandantNr, stuid, date, price);
    return r;
  }

  /// <summary>
  /// Deletes a price.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Affected entity.</param>
  /// <returns>Possibly errors.</returns>
  public ServiceErgebnis DeletePrice(ServiceDaten daten, WpStand e)
  {
    WpStandRep.Delete(daten, e);
    return new ServiceErgebnis();
  }

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
  /// <param name="cuid">Affected configuration IDs, separated by semikolon.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="days">Affected konfiguration ID.</param>
  /// <param name="state">State of calculation is always updated.</param>
  /// <param name="cancel">Cancel calculation if not empty.</param>
  public ServiceErgebnis<List<string>> ExportStocks(ServiceDaten daten, string search, string desc, string pattern, string stuid, bool inactive,
    string cuid, DateTime date, int days, StringBuilder state, StringBuilder cancel)
  {
    if (state == null || cancel == null)
      throw new ArgumentException(null, nameof(state));
    var r = new ServiceErgebnis<List<string>>();
    var clist = (cuid ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries);
    if (clist.Length <= 0)
      r.Errors.Add(Message.New(M2095));
    if (!r.Ok)
      return r;
    var list = new List<string>();
    var columns = new List<string>
    {
      "Kursdatum", "Konfiguration", "Uid", "Bezeichnung", "RelationBezeichnung", "Bewertung",
      "Trend", "Bewertung1", "Trend1", "Bewertung2", "Trend2", "Bewertung3", "Trend3", "Bewertung4", "Trend4",
      "Bewertung5", "Trend5", "Aktuellerkurs", "Signalkurs1", "Stopkurs", "Signalkurs2", "Muster",
      "Sortierung", "Kuerzel", "Xo", "Signalbew", "Signaldatum", "Signalbez", "Index1", "Index2", "Index3",
      "Index4", "Schnitt200", "Typ", "Waehrung", "GeaendertAm", "GeaendertVon", "AngelegtAm", "AngelegtVon",
    };
    list.Add(Functions.EncodeCSV(columns));
    r.Ergebnis = list;
    var anzahl = Math.Max(1, days);
    var d = Functions.Workday(date);

    while (anzahl > 0)
    {
      foreach (var c in clist)
      {
        var slist = CalculateStocksIntern(daten, desc, pattern, stuid, d, inactive, search, c, state, cancel);
        foreach (var s in slist)
        {
          var l = new List<string>
          {
            ToStr(s.PriceDate), ToStr(s.Configuration), ToStr(s.Uid), ToStr(s.Description),
            ToStr(s.RelationDescription), ToStr(s.Assessment),
            ToStr(s.Trend), ToStr(s.Assessment1), ToStr(s.Trend1), ToStr(s.Assessment2), ToStr(s.Trend2), ToStr(s.Assessment3), ToStr(s.Trend3),
            ToStr(s.Assessment4), ToStr(s.Trend4), ToStr(s.Assessment5), ToStr(s.Trend5), ToStr(s.CurrentPrice), ToStr(s.SignalPrice1),
            ToStr(s.StopPrice), ToStr(s.SignalPrice2), ToStr(s.Pattern), ToStr(s.Sorting), ToStr(s.Kuerzel), ToStr(s.Xo),
            ToStr(s.SignalAssessment), ToStr(s.SignalDate), ToStr(s.SignalDescription), ToStr(s.Index1), ToStr(s.Index2),
            ToStr(s.Index3), ToStr(s.Index4), ToStr(s.Average200), ToStr(s.Type), ToStr(s.Currency),
            ToStr(s.Geaendert_Am), ToStr(s.Geaendert_Von), ToStr(s.Angelegt_Am), ToStr(s.Angelegt_Von),
          };
          list.Add(Functions.EncodeCSV(l));
        }
      }
      d = Functions.Workday(d.AddDays(-1));
      anzahl--;
      if (anzahl > 0)
        Thread.Sleep(HttpDelay);
    }
    return r;
  }

  /// <summary>
  /// Get cluster index.
  /// </summary>
  /// <param name="diff">Affected difference.</param>
  /// <param name="mini">Affected minimum.</param>
  /// <param name="maxi">Affected maximum.</param>
  /// <returns>Calculated index.</returns>
  private static decimal ClIndex(decimal diff, decimal mini, decimal maxi)
  {
    if (Functions.CompDouble(mini, maxi) == 0)
      return 0;
    else
      return (diff - mini) / (maxi - mini);
  }

  /// <summary>
  /// Calculates a investment.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="inv">Affected investment.</param>
  /// <param name="blist">Affected booking list.</param>
  /// <param name="k">Affected exchange rate or null.</param>
  private static void CalculateInvestment(ServiceDaten daten, WpAnlage inv, List<WpBuchung> blist, SoKurse k)
  {
    Functions.MachNichts(daten);
    //// No Payment for interests.
    inv.Payment = blist.Sum(a => a.Zahlungsbetrag - (a.Zahlungsbetrag == 0 ? 0 : a.Rabattbetrag));
    inv.Shares = blist.Sum(a => a.Anteile);
    inv.ShareValue = inv.Shares == 0 ? 0 : inv.Payment / inv.Shares;
    //// Interests without taxes.
    inv.Interest = blist.Sum(a => a.Zinsen + (a.Zinsen == 0 ? 0 : a.Rabattbetrag));
    inv.Price = k == null ? 0 : k.Close;
    inv.PriceDate = k?.Datum;
    if (inv.Shares <= 0 && blist.Any())
    {
      var maxdate = blist.Max(a => a.Datum);
      inv.PriceDate = maxdate;
    }
    inv.Currency = k?.Bewertung;
    inv.CurrencyPrice = k == null ? 1 : k.Price;
    inv.Value = inv.Shares * inv.Price;
    inv.Profit = inv.Value + inv.Interest - inv.Payment;
    inv.ProfitPercent = inv.Value == 0 || inv.Payment == 0 ? 0
      : inv.Profit < 0 ? inv.Profit / inv.Value * 100m : inv.Profit / inv.Payment * 100m;
    inv.PriceDate2 = null;
    inv.Value2 = 0;
  }

  /// <summary>HTTPS-Abfrage synchron mit HttpClient ausführen.</summary>
  private static List<string> ExecuteHttps(string url, bool lines)
  {
    var task = ExecuteHttpsClient(url);
    task.Wait();
    var v = Functions.SplitLines(task.Result, lines);
    return v;
  }

  /// <summary>HTTPS-Abfrage mit HttpClient ausführen.</summary>
  private static Task<string> ExecuteHttpsClient(string url)
  {
    return Httpsclient.GetStringAsync(url);
  }

  /// <summary>
  /// Gets a list of URLs for price request.
  /// </summary>
  /// <param name="from">Beginning of the period.</param>
  /// <param name="to">End of the period.</param>
  /// <param name="source">Affected provider for prices.</param>
  /// <param name="shortcut">Affected shortcut for source.</param>
  /// <param name="type">Affected type (stock or bond).</param>
  /// <returns>List of URLs for price request.</returns>
  private static List<(DateTime Date, string Url)> GetPriceUrlsIntern(DateTime from, DateTime to,
      string source, string shortcut, string type)
  {
    var urls = new List<(DateTime, string)>();
    if (string.IsNullOrEmpty(source) || UiFunctions.IgnoreShortcut(shortcut))
      return urls;
    if (source == "yahoo")
    {
      var p1 = (int)(from - new DateTime(1970, 1, 1)).TotalSeconds;
      var p2 = (int)(to - new DateTime(1970, 1, 1)).TotalSeconds;
      var url = $"https://query1.finance.yahoo.com/v7/finance/chart/{shortcut}?period1={p1}&period2={p2}&interval=1d&indicators=quote&includeTimestamps=true";
      urls.Add((to, url));
    }
    else if (source == "ariva")
    {
#pragma warning disable IDE0071
      var url = $"https://www.ariva.de/quote/historic/historic.csv?secu={shortcut}&boerse_id=6&clean_split=1&clean_payout=0&clean_bezug=1&min_time={from.ToString("dd.MM.yyyy")}&max_time={to.ToString("dd.MM.yyyy")}&trenner=%3B&go=Download";
#pragma warning restore IDE0071
      urls.Add((to, url));
    }
    else if (source == "onvista")
    {
      if (!string.IsNullOrWhiteSpace(type) && (type.StartsWith("B") || type.StartsWith("C") || type.StartsWith("D") || type.StartsWith("F") || type.StartsWith("P") || type.StartsWith("S") || type.StartsWith("Y")))
      {
        // type B... BOND, C... COMMODITY, D... DERIVATIVE, F... FUND, P... PRECIOUS_METAL, S... STOCK, Y... CURRENCY
        var type0 = type.StartsWith("B") ? "BOND" : type.StartsWith("C") ? "COMMODITY" : type.StartsWith("D") ? "DERIVATIVE" : type.StartsWith("F") ? "FUND" : type.StartsWith("P") ? "PRECIOUS_METAL" : type.StartsWith("S") ? "STOCK" : "CURRENCY";
        var type1 = type[1..];
        //// https://api.onvista.de/api/v1/instruments/BOND/177301996/simple_chart_history?chartType=PRICE&endDate=2022-07-20&idNotation=297412910&startDate=2022-01-01&withEarnings=true
        var url = $"https://api.onvista.de/api/v1/instruments/{type0}/{type1}/simple_chart_history?chartType=PRICE&endDate={Functions.ToString(to.AddDays(1))}&idNotation={shortcut}&startDate={Functions.ToString(from)}&withEarnings=true";
        urls.Add((to, url));
      }
    }
    return urls;
  }

  /// <summary>
  /// Gets a list of prices or rates for a period.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="from">Beginning of the period.</param>
  /// <param name="to">End of the period.</param>
  /// <param name="source">Affected provider for prices.</param>
  /// <param name="shortcut">Affected shortcut for source.</param>
  /// <param name="type">Affected type (bond, fund or stock with id).</param>
  /// <param name="currency">Affected currency for bond.</param>
  /// <param name="price">Actual price.</param>
  /// <param name="uid">Affected stock uid.</param>
  /// <param name="dictresponse">Affected preemptively read responses.</param>
  /// <returns>List of prices.</returns>
  private List<SoKurse> GetPriceListIntern(ServiceDaten daten, DateTime from, DateTime to,
    string source, string shortcut, string type, string currency, decimal price,
    string uid = null, Dictionary<string, StockUrl> dictresponse = null)
  {
    var l = new List<SoKurse>();
    if (string.IsNullOrEmpty(source) || UiFunctions.IgnoreShortcut(shortcut))
      return l;
    if (dictresponse == null)
    {
      dictresponse = new Dictionary<string, StockUrl>();
      var ulist = GetPriceUrlsIntern(from, to, source, shortcut, type);
      foreach (var (udate, url) in ulist)
      {
        var su = new StockUrl
        {
          Uid = "",
          Description = "",
          Date = udate,
          Url = url,
        };
        if (!dictresponse.TryGetValue(su.Key, out var du))
        {
          dictresponse.Add(su.Key, su);
        }
      }
      var l1 = dictresponse.Count;
      var i1 = 1;
      foreach (var su in dictresponse.Values)
      {
        su.Task = ExecuteHttpsClient(su.Url);
        if (i1 < l1)
        {
          // Verzögerung wegen onvista.de notwendig. 500 OK.
          Thread.Sleep(HttpDelay);
        }
        i1++;
      }
      var tasks = dictresponse.Values.Select(a => a.Task).ToArray();
      Task.WaitAll(tasks, HttpTimeout);
      foreach (var su in dictresponse.Values)
      {
        su.Response = su.Task.Result;
        su.Task.Dispose();
        su.Task = null;
      }
    }
    if (source == "yahoo")
    {
      var p1 = (int)(from - new DateTime(1970, 1, 1)).TotalSeconds;
      var p2 = (int)(to - new DateTime(1970, 1, 1)).TotalSeconds;
      var url = $"https://query1.finance.yahoo.com/v7/finance/chart/{shortcut}?period1={p1}&period2={p2}&interval=1d&indicators=quote&includeTimestamps=true";
      string response = null;
      if (dictresponse != null && dictresponse.TryGetValue(StockUrl.GetKey(uid, to), out var resp))
        response = resp.Response;
      var v = response == null ? ExecuteHttps(url, false) : Functions.SplitLines(response, false);
      if (v != null && v.Count > 0)
      {
        var jr = JObject.Parse(v[0]);
        var jc = jr["chart"];
        var error = jc["error"];
        if (error != null && error.HasValues)
          throw new Exception(error.ToString());
        var jresult = jc["result"][0];
        var jmeta = jresult["meta"];
        var jts = jresult["timestamp"]?.ToArray();
        var jquote = jresult["indicators"]["quote"][0];
        if (jquote.HasValues)
        {
          var jopen = jquote["open"];
          var jclose = jquote["close"];
          var jlow = jquote["low"];
          var jhigh = jquote["high"];
          for (var i = 0; jts != null && i < jts.Length; i++)
          {
            var k = new SoKurse
            {
              Datum = Functions.ToDateTime(Functions.ToInt64(jts[i].ToString())).Date,
              Open = Functions.ToDecimal(jopen[i].ToString(), 4) ?? 0,
              High = Functions.ToDecimal(jhigh[i].ToString(), 4) ?? 0,
              Low = Functions.ToDecimal(jlow[i].ToString(), 4) ?? 0,
              Close = Functions.ToDecimal(jclose[i].ToString(), 4) ?? 0,
              Bewertung = jmeta["currency"].ToString().ToUpper(),
            };
            if (k.Close != 0)
            {
              k.Open = k.Open == 0 ? k.Close : k.Open;
              k.High = k.Open == 0 ? k.Close : k.High;
              k.Low = k.Open == 0 ? k.Close : k.Low;
              l.Add(k);
            }
          }
        }
        else
        {
          var tl = jmeta["currentTradingPeriod"]["pre"]["end"].ToString();
          var k = new SoKurse
          {
            Datum = Functions.ToDateTime(Functions.ToInt64(tl)),
            Close = Functions.ToDecimal(jmeta["chartPreviousClose"].ToString(), 4) ?? 0,
            Bewertung = jmeta["currency"].ToString().ToUpper(),
          };
          if (k.Close != 0)
          {
            k.Open = k.Close;
            k.High = k.Close;
            k.Low = k.Close;
            l.Add(k);
          }
        }
      }
    }
    else if (source == "ariva")
    {
#pragma warning disable IDE0071
      var url = $"https://www.ariva.de/quote/historic/historic.csv?secu={shortcut}&boerse_id=6&clean_split=1&clean_payout=0&clean_bezug=1&min_time={from.ToString("dd.MM.yyyy")}&max_time={to.ToString("dd.MM.yyyy")}&trenner=%3B&go=Download";
#pragma warning restore IDE0071
      string response = null;
      if (dictresponse != null && dictresponse.TryGetValue(StockUrl.GetKey(uid, to), out var resp))
        response = resp.Response;
      var v = response == null ? ExecuteHttps(url, true) : Functions.SplitLines(response, true);
      var f = "Datum;Erster;Hoch;Tief;Schlusskurs;Stuecke;Volumen";
      if (v[0] != f)
        throw new MessageException(WP050(v[0], f));
      for (var i = 1; i < v.Count; i++)
      {
        var c = Functions.DecodeCSV(v[i], ';', ';');
        if (c != null && c.Count >= 5)
        {
          var k = new SoKurse
          {
            Datum = Functions.ToDateTime(c[0]) ?? daten.Heute,
            Open = Functions.ToDecimalDe(c[1]) ?? 0,
            High = Functions.ToDecimalDe(c[2]) ?? 0,
            Low = Functions.ToDecimalDe(c[3]) ?? 0,
            Close = Functions.ToDecimalDe(c[4]) ?? 0,
            Bewertung = "EUR",
            Price = 1,
          };
          l.Add(k);
        }
      }
    }
    else if (source == "onvista")
    {
      if (!string.IsNullOrWhiteSpace(type))
      {
        // Json result "{\"expires\":1658347274322,\"isoCurrency\":\"EUR\",\"unitType\":\"PCT\",\"displayUnit\":\"PCT\",\"datetimeTick\":[1657735289000,1657780419000,1657809305000,1657821684000,1657830330000,1657889989000,1657908083000,1657916730000,1658167302000,1658213095000,1658220109000,1658223076000,1658227261000,1658238488000,1658253681000,1658262331000],\"tick\":[92.345,92.449,91.783,92.004,92.004,92.48,92.468,92.468,92.188,92.314,92.059,92.065,91.964,91.909,92.141,92.141]}"
        string response = null;
        if (dictresponse != null && dictresponse.TryGetValue(StockUrl.GetKey(uid, to), out var resp))
          response = resp.Response;
        if (string.IsNullOrEmpty(response))
          throw new MessageException(WP050("", "JSON"));
        var jr = JObject.Parse(response);
        var error = jr["displayErrorMessage"];
        if (error != null)
          throw new Exception(error.ToString());
        var isoCurrency = jr["isoCurrency"]?.ToString()?.ToUpper();
        var unitType = jr["unitType"]?.ToString()?.ToUpper();
        var dates = jr["datetimeTick"]?.ToArray();
        if (dates == null || dates.Length <= 0)
          throw new MessageException(WP050("", "datetimeTick"));
        var ticks = jr["tick"]?.ToArray();
        if (ticks == null || ticks.Length <= 0)
          throw new MessageException(WP050("", "tick"));
        SoKurse k = null;
        var date0 = Functions.ToDateTime(0L).Date;
        for (var i = 0; dates != null && i < dates.Length; i++)
        {
          // Assumes ascending dates.
          SoKurse k1 = null;
          var tick = ticks.Length <= i ? 0 : Functions.ToDecimal(ticks[i].ToString()) ?? 0;
          if (unitType == "PCT")
            tick /= 100; // percentage
          var date = Functions.ToDateTime(Functions.ToInt64(dates[i].ToString()) / 1000L);
          if (tick > 0)
          {
            if (date0 != date.Date)
            {
              k1 = k;
              k = new SoKurse
              {
                Datum = date.Date,
                Open = tick,
                High = tick,
                Low = tick,
                Close = tick,
                Bewertung = isoCurrency ?? "EUR",
              };
            }
            else if (k != null)
            {
              k.High = Math.Max(k.High, tick);
              k.Low = Math.Min(k.Low, tick);
              k.Close = tick;
            }
          }
          if (i == dates.Length - 1)
            k1 = k;
          if (k1 != null)
            l.Add(k1);
          date0 = date.Date;
        }
      }
    }
    if (currency != "Y")
    {
      var cur = l.Count <= 0 ? null : l[0].Bewertung;
      SoKurse curPrice = null;
      if (l.Count > 0)
      {
        try
        {
          curPrice = GetCurrencyPrice(daten, l.Last().Datum, cur);
        }
        catch (Exception ex)
        {
          throw new Exception(WP054(cur, ex.Message));
        }
      }
      foreach (var k in l)
      {
        if (price == 0)
          price = k.Close;
        if (curPrice != null)
        {
          k.Open = Functions.Round4(k.Open * curPrice.Close) ?? 0;
          k.High = Functions.Round4(k.High * curPrice.Close) ?? 0;
          k.Low = Functions.Round4(k.Low * curPrice.Close) ?? 0;
          k.Close = Functions.Round4(k.Close * curPrice.Close) ?? 0;
          k.Price = curPrice.Close;
        }
        while (price != 0 && k.Close / price > 5)
        {
          // Skales right: WATL.L with factor 100.
          k.Open /= 10;
          k.High /= 10;
          k.Low /= 10;
          k.Close /= 10;
        }
      }
    }
    l = l.OrderBy(a => a.Datum).ToList();
    return l;
  }

  /// <summary>
  /// Calculates all stocks internally.
  /// </summary>
  /// <returns>List of affected stock.</returns>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="desc">Affected description.</param>
  /// <param name="pattern">Affected pattern.</param>
  /// <param name="uid">Affected stock ID.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="inactive">Also inactive investmenst or not.</param>
  /// <param name="search">Affected text search.</param>
  /// <param name="kuid">Affected konfiguration ID.</param>
  /// <param name="state">State of calculation is always updated.</param>
  /// <param name="cancel">Cancel calculation if not empty.</param>
  private List<WpWertpapier> CalculateStocksIntern(ServiceDaten daten, string desc, string pattern, string uid,
    DateTime date, bool inactive, string search, string kuid, StringBuilder state, StringBuilder cancel)
  {
    if (state == null || cancel == null)
      throw new ArgumentException(null, nameof(state));
    WpKonfiguration k = null;
    if (!string.IsNullOrEmpty(kuid))
    {
      k = WpKonfigurationRep.Get(daten, daten.MandantNr, kuid, true);
      if (k != null)
        k.Bezeichnung = PnfChart.GetBezeichnung(k.Bezeichnung, 0, k.Scale, k.Reversal, k.Method, k.Relative, k.Duration);
    }
    if (k == null)
      k = new WpKonfiguration
      {
        Bezeichnung = "",
        Duration = 182, // half year
        Box = 1,
        Scale = 2, // dynamic
        Reversal = 3,
        Method = 4, // Open high low close
        Relative = false,
      };
    var from = date.AddDays(-k.Duration);
    var dictlist = new Dictionary<string, List<SoKurse>>();
    var dictresponse = new Dictionary<string, StockUrl>();
    var list = WpWertpapierRep.GetList(daten, daten.MandantNr, desc, pattern, uid, null, !inactive, search);
    if (!inactive && list.Count != 1)
      list = list.Where(a => a.Status == "1").ToList(); // Calculate only active stock.
    list = list.Where(a => !UiFunctions.IgnoreShortcut(a.Kuerzel) && string.IsNullOrWhiteSpace(a.Type)).ToList();
    var l = list.Count;
    state.Clear().Append(M0(WP053));
    Gtk.Application.Invoke((sender, e) => { MainClass.MainWindow.SetError(state.ToString()); });
    for (var i = 0; i < l && cancel.Length <= 0; i++)
    {
      var st = list[i];
      var ulist = GetPriceUrlsIntern(from, date, st.Datenquelle, st.Kuerzel, st.Type);
      foreach (var (udate, url) in ulist)
      {
        var su = new StockUrl
        {
          Uid = st.Uid,
          Description = st.Bezeichnung,
          Date = udate,
          Url = url,
        };
        if (!dictresponse.TryGetValue(su.Key, out var du))
        {
          dictresponse.Add(su.Key, su);
        }
      }
    }
    Debug.Print($"{DateTime.Now} Start.");
    var l1 = dictresponse.Count;
    var i1 = 1;
    foreach (var su in dictresponse.Values)
    {
      if (cancel.Length > 0)
        break;
      su.Task = ExecuteHttpsClient(su.Url);
      state.Clear().Append(WP008(i1, l1, su.Description, su.Date, null));
      Gtk.Application.Invoke((sender, e) =>
      {
        MainClass.MainWindow.SetError(state.ToString());
      });
      if (i1 < l1)
      {
        // Verzögerung wegen onvista.de notwendig. 500 OK.
        Thread.Sleep(HttpDelay);
      }
      i1++;
    }
    if (cancel.Length <= 0)
    {
      var tasks = dictresponse.Values.Select(a => a.Task).ToArray();
      Task.WaitAll(tasks, HttpTimeout);
      foreach (var su in dictresponse.Values)
      {
        su.Response = su.Task.Result;
        su.Task.Dispose();
        su.Task = null;
      }
    }
    Debug.Print($"{DateTime.Now} End.");

    for (var i0 = 0; i0 < l && cancel.Length <= 0; i0++)
    {
      // Calculate stock.
      var st = list[i0];
      state.Clear().Append(WP009(i0 + 1, l, st.Bezeichnung, date, k?.Bezeichnung));
      Gtk.Application.Invoke((sender, e) =>
      {
        MainClass.MainWindow.SetError(state.ToString());
      });
      try
      {
        var liste = GetPriceListIntern(daten, from, date, st.Datenquelle, st.Kuerzel, st.Type, st.Currency, st.CurrentPrice ?? 0, st.Uid, dictresponse);

        st.Assessment = $"00 {M0(WP010)}";
        st.Assessment1 = "";
        st.Assessment2 = "";
        st.Assessment3 = "";
        st.Assessment4 = "";
        st.Assessment5 = "";
        st.Pattern = "";
        st.StopPrice = null;
        //// wp.signalkurs1 Zielkurs (Signalkur1) wird manuell erfasst.
        st.SignalPrice2 = null;
        st.Trend1 = "";
        st.Trend2 = "";
        st.Trend3 = "";
        st.Trend4 = "";
        st.Trend5 = "";
        st.Trend = "";
        st.PriceDate = null;
        st.Xo = "";
        st.SignalAssessment = "";
        st.SignalDate = null;
        st.SignalDescription = "";
        st.Index1 = "";
        st.Index2 = "";
        st.Index3 = "";
        st.Index4 = "";
        st.Average200 = "";
        //// st.Configuration = (k == null) ? M0(WP010) : k.Bezeichnung
        //// st.typ
        //// st.waehrung

        var kursdatum = liste.Count > 0 ? liste.Last().Datum : date;
        var signalbew = 0;
        var signaldatum = from;
        string signalbez = null;
        var a = new decimal[] { 0.5m, 1, 2, 3, 5 };
        var t = new string[] { null, null, null, null, null };
        var bew = new int[] { 0, 0, 0, 0, 0 };
        for (var i = 0; i < a.Length; i++)
        {
          var c = new PnfChart(k.Method, a[i], k.Scale, k.Reversal, st.Bezeichnung)
          {
            Ziel = st.SignalPrice1 ?? 0,
            Stop = st.StopPrice ?? 0,
            Relativ = k.Relative,
          };
          c.AddKurse(liste);
          var p = c.Pattern.LastOrDefault();
          if (p != null)
          {
            // letztes Signal nur bei letzter Säule, Signal am gleichen Tag
            if (p.Xpos >= c.Saeulen.Count - 1 && p.Datum == kursdatum)
            {
              bew[i] = p.Signal;
              if (string.IsNullOrEmpty(st.Pattern))
              {
                st.Pattern = PnfPattern.GetBezeichnung(p.Muster);
              }
            }
            if (p.Datum > signaldatum ||
              (p.Datum == signaldatum && Math.Abs(p.Signal) > Math.Abs(signalbew)))
            {
              signalbew = p.Signal;
              signaldatum = p.Datum;
              signalbez = PnfPattern.GetBezeichnung(p.Muster);
            }
          }
          if (c.Saeulen.Count > 0 && c.Saeulen.Last().Date == kursdatum)
          {
            st.Xo = WP048(c.Saeulen.Last().IsO ? "xo" : "ox", c.Box);
          }
          if (i == 0)
          {
            st.CurrentPrice = c.Kurs;
            if (Functions.CompDouble4(c.Stop, 0) > 0)
            {
              st.StopPrice = Functions.Round(c.Stop);
              st.SignalPrice2 = Functions.Round4(c.Kurs / c.Stop);
            }
          }
          var tr = c.Trend;
          t[i] = tr == 2 ? "+2" : tr == 1 ? "+1" : tr == 0.5m ? "+0,5" : tr == -2 ? "-2" : tr == -1 ? "-1" :
            tr == -0.5m ? "-0,5" : tr == 0 ? "0" : Functions.ToString(tr, 1);
        }
        st.Assessment1 = Functions.ToString(bew[0]);
        st.Assessment2 = Functions.ToString(bew[1]);
        st.Assessment3 = Functions.ToString(bew[2]);
        st.Assessment4 = Functions.ToString(bew[3]);
        st.Assessment5 = Functions.ToString(bew[4]);
        st.Assessment = string.Format("{0:00}", bew[0] + bew[1] + bew[2] + bew[3] + bew[4]);
        st.Trend1 = t[0];
        st.Trend2 = t[1];
        st.Trend3 = t[2];
        st.Trend4 = t[3];
        st.Trend5 = t[4];
        st.Trend = string.Format("{0:0}", Functions.ToInt32(t[0]) + Functions.ToInt32(t[1]) + Functions.ToInt32(t[2]) + Functions.ToInt32(t[3]) + Functions.ToInt32(t[4]));
        st.PriceDate = kursdatum;
        st.SignalAssessment = Functions.ToString(signalbew);
        st.SignalDate = signaldatum == from ? null : signaldatum;
        st.SignalDescription = signalbez;
        var ia = new int[] { 182, 730, 1460, 3650 };
        var mina = new decimal[] { 0, 0, 0, 0 };
        var maxa = new decimal[] { 0, 0, 0, 0 };
        var difa = new decimal[] { 0, 0, 0, 0 };
        var bis = liste.Last().Datum;
        var min0 = liste.Last().Close;
        for (var i = 0; i < ia.Length; i++)
        {
          var datumi = bis.AddDays(-ia[i]);
          var mini = min0;
          var maxi = 0m;
          var diff = 0m;
          foreach (var ku in liste)
          {
            if (datumi < ku.Datum)
            {
              // && datumi.hour == 0 && datumi.minute == 0 && datumi.second == 0) {
              // evtl. aktuellen Kurs ignorieren
              diff = diff + ku.Open - ku.Close;
              if (Functions.CompDouble(diff, maxi) > 0)
                maxi = diff;
              if (Functions.CompDouble(diff, mini) < 0)
                mini = diff;
            }
          }
          mina[i] = mini;
          maxa[i] = maxi;
          difa[i] = diff;
        }
        st.Index1 = Functions.ToString(ClIndex(difa[0], mina[0], maxa[0]));
        st.Index2 = Functions.ToString(ClIndex(difa[1], mina[1], maxa[1]));
        st.Index3 = Functions.ToString(ClIndex(difa[2], mina[2], maxa[2]));
        st.Index4 = Functions.ToString(ClIndex(difa[3], mina[3], maxa[3]));
        var datum14 = bis.AddDays(-14);
        var datum200 = bis.AddDays(200);
        var datum214 = bis.AddDays(214);
        var summe14 = 0m;
        var summe200 = 0m;
        var summe214 = 0m;
        var anzahl14 = 0;
        var anzahl200 = 0;
        var anzahl214 = 0;
        foreach (var ku in liste)
        {
          if (datum14 < ku.Datum)
          {
            summe14 += ku.Close;
            anzahl14++;
          }
          if (datum200 < ku.Datum)
          {
            summe200 += ku.Close;
            anzahl200++;
          }
          if (datum214 < ku.Datum)
          {
            summe214 += ku.Close;
            anzahl214++;
          }
        }
        summe214 -= summe14;
        anzahl214 -= anzahl14;
        if (anzahl200 > 0 && anzahl214 > 0)
        {
          var schnitt200 = summe200 / anzahl200;
          var schnitt214 = summe214 / anzahl214;
          st.Average200 = Functions.ToString(Functions.CompDouble(schnitt200, schnitt214));
        }
        WpWertpapierRep.Update(daten, st);
        //// Debug.WriteLine($"6 {st.Parameter}");
      }
      catch (Exception ex)
      {
        st.Assessment = $"00 {M1033(ex.Message)}";
        //// throw new RuntimeException(ex)
      }
    }
    SaveChanges(daten);
    state.Clear();
    return list;
  }

  /// <summary>
  /// Gets currency price in relation to EUR.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="shortcut">Affected currency shortcut.</param>
  /// <returns>List of prices.</returns>
  private SoKurse GetCurrencyPrice(ServiceDaten daten, DateTime date, string shortcut)
  {
    if (string.IsNullOrEmpty(shortcut))
      return null;
    shortcut = shortcut.ToUpper();
    if (shortcut == "EUR")
      return new SoKurse
      {
        Datum = date,
        Close = 1,
        Bewertung = shortcut,
      };
    var key = $"{date} {shortcut}";
    Wkurse.TryGetValue(key, out var wert);
    if (wert != null)
      return wert;
    var wp = WpWertpapierRep.GetList(daten, daten.MandantNr, $"EUR-{shortcut}").FirstOrDefault();
    if (wp != null)
    {
      var to = date.AddDays(1);
      var from = to.AddDays(-7);
      var l = GetPriceListIntern(daten, from, to, wp.Datenquelle, wp.Kuerzel, wp.Type, "Y", 0);
      foreach (var p1 in l ?? new List<SoKurse>())
      {
        var k = new SoKurse
        {
          Datum = p1.Datum,
          Close = p1.Close,
          Bewertung = shortcut,
        };
        WpStandRep.Save(daten, daten.MandantNr, wp.Uid, k.Datum, k.Close);
        SaveChanges(daten);
        if (k.Close != 0)
          k.Close = Functions.Round4(1 / k.Close) ?? 0; // Invert for multiplication.
        var key1 = $"{k.Datum} {shortcut}";
        if (Wkurse.ContainsKey(key1))
          Wkurse.Remove(key1);
        Wkurse.Add(key1, k);
      }
      Wkurse.TryGetValue(key, out wert);
      if (wert != null)
        return wert;
    }
    List<string> v;
    try
    {
      var accesskey = Parameter.GetValue(Parameter.WP_FIXER_IO_ACCESS_KEY);
      if (string.IsNullOrEmpty(accesskey))
        throw new MessageException(WP049);
      var url = $"http://data.fixer.io/api/{Functions.ToString(date)}?symbols={shortcut}&access_key={accesskey}";
      v = ExecuteHttps(url, false);
    }
    catch (Exception)
    {
      var k = GetCurrencyPriceDb(daten, date, shortcut);
      if (k == null)
        throw;
      Wkurse.Add(key, k);
      return k;
    }
    if (v != null && v.Count > 0)
    {
      var jr = JObject.Parse(v[0]);
      var success = Functions.ToBool(jr["success"].ToString()) ?? false;
      if (!success)
      {
        var error = jr["error"]["info"].ToString();
        if (error.StartsWith("Your monthly usage limit has been reached."))
        {
          // Your monthly usage limit has been reached. Please upgrade your Subscription Plan.
          // var wp = WpWertpapierRep.GetList(daten, daten.MandantNr, $"EUR-{shortcut}").FirstOrDefault();
          if (wp != null)
          {
            var s = WpStandRep.GetLatest(daten, daten.MandantNr, wp.Uid, date);
            if (s != null)
            {
              var k = new SoKurse
              {
                Datum = s.Datum,
                Close = s.Stueckpreis,
                Bewertung = shortcut,
              };
              if (k.Close != 0)
                k.Close = Functions.Round4(1 / k.Close) ?? 0;
              Wkurse.Add(key, k);
              return k;
            }
          }
        }
        throw new Exception(error);
      }
      var jresult = jr["rates"];
      if (jresult[shortcut] != null)
      {
        var k = new SoKurse
        {
          Datum = Functions.ToDateTime(jr["date"].ToString()) ?? date,
          Close = Functions.ToDecimal(jresult[shortcut].ToString()) ?? 0,
          Bewertung = shortcut,
        };
        //// var wp = WpWertpapierRep.GetList(daten, daten.MandantNr, $"EUR-{shortcut}").FirstOrDefault();
        if (wp != null)
        {
          WpStandRep.Save(daten, daten.MandantNr, wp.Uid, k.Datum, k.Close);
          SaveChanges(daten);
        }
        if (k.Close != 0)
          k.Close = Functions.Round4(1 / k.Close) ?? 0;
        Wkurse.Add(key, k);
        return k;
      }
    }
    return null;
  }

  /// <summary>
  /// Gets currency price in relation to EUR from Database.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="date">Affected date.</param>
  /// <param name="shortcut">Affected currency shortcut.</param>
  /// <returns>List of prices.</returns>
  private SoKurse GetCurrencyPriceDb(ServiceDaten daten, DateTime date, string shortcut)
  {
    if (string.IsNullOrEmpty(shortcut))
      return null;
    shortcut = shortcut.ToUpper();
    if (shortcut == "EUR")
      return new SoKurse
      {
        Datum = date,
        Close = 1,
        Bewertung = shortcut,
      };
    var desc = $"EUR-{shortcut}";
    var l = WpWertpapierRep.GetList(daten, daten.MandantNr, desc);
    var wp = l.FirstOrDefault();
    if (wp == null)
    {
      var r = SaveStock(daten, null, desc, "0", null, "cur", "onvista", "1", null, null, null, shortcut, false);
      SaveChanges(daten);
      wp = r.Ergebnis;
      decimal? kurs = null;
      //// Default exchange rate from 06.05.2022
      if (shortcut == "USD")
        kurs = 1.0548m;
      else if (shortcut == "CHF")
        kurs = 1.0431m;
      if (kurs.HasValue)
      {
        var k = new SoKurse
        {
          Datum = date,
          Close = Functions.Round4(1 / kurs.Value) ?? 0,
          Bewertung = shortcut,
        };
        return k;
      }
    }
    if (wp != null)
    {
      var st = WpStandRep.GetLatest(daten, daten.MandantNr, wp.Uid, date);
      if (st != null)
      {
        var k = new SoKurse
        {
          Datum = st.Datum,
          Close = st.Stueckpreis == 0 ? 0 : (Functions.Round4(1 / st.Stueckpreis) ?? 0),
          Bewertung = shortcut,
        };
        return k;
      }
    }
    return null;
  }

  /// <summary>
  /// Class for reading stock prices.
  /// </summary>
  private class StockUrl
  {
    /// <summary>Gets key for dictionary.</summary>
    public string Key
    {
      get { return GetKey(Uid, Date); }
    }

    /// <summary>Gets or sets uid.</summary>
    public string Uid { get; set; }

    /// <summary>Gets or sets description.</summary>
    public string Description { get; set; }

    /// <summary>Gets or sets date.</summary>
    public DateTime Date { get; set; }

    /// <summary>Gets or sets URL.</summary>
    public string Url { get; set; }

    /// <summary>Gets or sets task for https request.</summary>
    public Task<string> Task { get; set; }

    /// <summary>Gets or sets https response after reading.</summary>
    public string Response { get; set; }

    /// <summary>
    /// Gets key from uid and date.
    /// </summary>
    /// <param name="uid">Affected uid.</param>
    /// <param name="date">Affected date.</param>
    /// <returns>Composed key.</returns>
    public static string GetKey(string uid, DateTime date)
    {
      return $"{uid}#{date}";
    }
  }
}
