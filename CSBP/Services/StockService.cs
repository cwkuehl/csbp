// <copyright file="StockService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using Newtonsoft.Json.Linq;
  using static CSBP.Resources.Messages;
  using static CSBP.Resources.M;
  using System.Threading.Tasks;
  using System.Diagnostics;
  using System.Threading;

  public class StockService : ServiceBase, IStockService
  {
    /// <summary>Holt oder setzt den Haushalt-Service.</summary>
    public IBudgetService BudgetService { private get; set; }

    /// <summary>
    /// Gets a list of stocks.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="inactive">Get also inactive investments?</param>
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
    /// <param name="relationuid">Affected relationuid.</param>
    /// <param name="notice">Affected abbreviation.</param>
    /// <param name="type">Affected type.</param>
    /// <param name="currency">Affected currency.</param>
    /// <param name="inv">Create also an investment?</param>
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
        // Create an investment.
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
      WpWertpapierRep.Delete(daten, e);
      return new ServiceErgebnis();
    }

    /// <summary>
    /// Gets a list of states.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>List of states.</returns>
    public ServiceErgebnis<List<MaParameter>> GetStateList(ServiceDaten daten)
    {
      var l = new List<MaParameter> {
        new MaParameter { Schluessel = "1", Wert = Enum_state_active },
        new MaParameter { Schluessel = "0", Wert = Enum_state_inactive },
        new MaParameter { Schluessel = "2", Wert = Enum_state_nocalc }
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
      var list = new List<MaParameter> {
        new MaParameter { Schluessel = "0", Wert = Enum_scale_fix },
        new MaParameter { Schluessel = "1", Wert = Enum_scale_pc },
        new MaParameter { Schluessel = "2", Wert = Enum_scale_dyn }
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
      var list = new List<MaParameter> {
        new MaParameter { Schluessel = "1", Wert = Enum_method_c },
        new MaParameter { Schluessel = "2", Wert = Enum_method_hl },
        new MaParameter { Schluessel = "3", Wert = Enum_method_hlr },
        new MaParameter { Schluessel = "4", Wert = Enum_method_ohlc },
        new MaParameter { Schluessel = "5", Wert = Enum_method_tp }
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
      var l = new List<MaParameter> {
        new MaParameter { Schluessel = "onvista", Wert = "Onvista Fonds, www.onvista.de" },
        new MaParameter { Schluessel = "yahoo", Wert = "Yahoo, finance.yahoo.com" },
        new MaParameter { Schluessel = "ariva", Wert = "Ariva, www.ariva.de" }
      };
      var r = new ServiceErgebnis<List<MaParameter>>(l);
      return r;
    }

    /// <summary>
    /// Gets a list of configurations.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="extended">Get more data?</param>
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
          Status = "1"
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
    /// <param name="relative">Is it relative Calculation?</param>
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
      if (Functions.compDouble4(box, 0) <= 0)
        r.Errors.Add(Message.New(WP003));
      if (reversal <= 0)
        r.Errors.Add(Message.New(WP004));
      if (1 > method || method > 5)
        r.Errors.Add(Message.New(WP005));
      if (duration <= 10)
        r.Errors.Add(Message.New(WP006));
      if (0 > scale || scale > 2)
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
    /// <param name="relative">Relative prices to relation?</param>
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
        str = WpWertpapierRep.Get(daten, daten.MandantNr, str.Relation_Uid);
      r.Ergebnis = GetPriceListIntern(daten, from, to, st.Datenquelle, st.Kuerzel, st.Type, st.Currency, 0);
      return r;
    }

    /// <summary>
    /// Gets a list of investments.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="inactive">Get also inactive investments?</param>
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
      //if (!Global.nes(bez) && bez.length > WpAnlage.BEZEICHNUNG_LAENGE)
      //    throw new MeldungException(Meldungen::WP050);
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

    private class StockUrl
    {
      public string Key { get { return GetKey(Uid, Date); } }
      public string Uid { get; set; }
      public string Description { get; set; }
      public DateTime Date { get; set; }
      public string Url { get; set; }
      public Task<string> Task { get; set; }
      public string Response { get; set; }
      public static string GetKey(string uid, DateTime date)
      {
        return $"{uid}#{date}";
      }
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
    /// <param name="inactive">Also inactive investmenst?</param>
    /// <param name="search">Affected text search.</param>
    /// <param name="status">Status of backup is always updated.</param>
    /// <param name="cancel">Cancel backup if not empty.</param>
    public ServiceErgebnis CalculateInvestments(ServiceDaten daten, string desc, string uid, string stuid,
        DateTime date, bool inactive, string search, StringBuilder status, StringBuilder cancel)
    {
      if (status == null || cancel == null)
        throw new ArgumentException();
      var from = date.AddDays(-7);
      var dictlist = new Dictionary<string, List<SoKurse>>();
      var dictresponse = new Dictionary<string, StockUrl>();
      var list = WpAnlageRep.GetList(daten, daten.MandantNr, desc, uid, stuid, search);
      if (!inactive)
        list = list.Where(a => a.State == 1).ToList(); // Nur aktive Anlagen berechnen.
      var l = list.Count;
      status.Clear().Append(M0(WP053));
      Gtk.Application.Invoke(delegate
      {
        MainClass.MainWindow.SetError(status.ToString());
      });
      for (var i = 0; i < l && cancel.Length <= 0; i++)
      {
        var inv = list[i];
        var ulist = GetPriceUrlsIntern(from, date, inv.StockProvider, inv.StockShortcut, inv.StockType);
        foreach (var url in ulist)
        {
          var su = new StockUrl
          {
            Uid = inv.Wertpapier_Uid,
            Description = inv.Bezeichnung,
            Date = url.Item1,
            Url = url.Item2,
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
        status.Clear().Append(WP008(i1, l1, su.Description, su.Date, null));
        Gtk.Application.Invoke(delegate
        {
          MainClass.MainWindow.SetError(status.ToString());
        });
        if (i1 < l1)
        {
          // Verzögerung wegen onvista.de notwendig. 500 OK.
          Thread.Sleep(500);
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
      Debug.Print($"{DateTime.Now} Ende.");

      for (var i = 0; i < l && cancel.Length <= 0; i++)
      {
        // Kurse berechnen.
        var inv = list[i];
        status.Clear().Append(WP009(i + 1, l, inv.Bezeichnung, date, null));
        Gtk.Application.Invoke(delegate
        {
          MainClass.MainWindow.SetError(status.ToString());
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
            k = klist[klist.Count - 1];
            k1 = klist[klist.Count - 2];
          }
          else if (klist.Count >= 1)
            k = klist[klist.Count - 1];
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
              Price = 1
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
            Price = 1
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
        if (inv.PriceDate.HasValue && Functions.compDouble4(inv.Price, 0) > 0)
        {
          // Stand speichern
          WpStandRep.Save(daten, daten.MandantNr, inv.Wertpapier_Uid, inv.PriceDate.Value, inv.Price);
          SaveChanges(daten);
        }
      }
      status.Clear();
      var r = new ServiceErgebnis();
      return r;
    }

    private void CalculateInvestment(ServiceDaten daten, WpAnlage inv, List<WpBuchung> blist, SoKurse k)
    {
      inv.Payment = blist.Sum(a => a.Zahlungsbetrag - a.Rabattbetrag);
      inv.Shares = blist.Sum(a => a.Anteile);
      inv.ShareValue = inv.Shares == 0 ? 0 : inv.Payment / inv.Shares;
      inv.Interest = blist.Sum(a => a.Zinsen);
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
        var anleihe = st != null && !string.IsNullOrEmpty(st.Type);
        if (!anleihe)
          r.Ergebnis.Add(new HhEreignis
          {
            Uid = Functions.GetUid(),
            Bezeichnung = "Kauf aus Sparplan",
            EText = "1",
            Soll_Konto_Uid = inv.PortfolioAccountUid,
            Haben_Konto_Uid = inv.SettlementAccountUid
          });
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = anleihe ? "Anleihekauf" : "Wertpapierkauf",
          EText = "1",
          Soll_Konto_Uid = inv.PortfolioAccountUid,
          Haben_Konto_Uid = inv.SettlementAccountUid
        });
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = anleihe ? "Kupongutschrift" : "Dividendengutschrift",
          EText = "2",
          Soll_Konto_Uid = inv.SettlementAccountUid,
          Haben_Konto_Uid = inv.IncomeAccountUid
        });
        if (!anleihe)
          r.Ergebnis.Add(new HhEreignis
          {
            Uid = Functions.GetUid(),
            Bezeichnung = "Ertragsgutschrift",
            EText = "2",
            Soll_Konto_Uid = inv.SettlementAccountUid,
            Haben_Konto_Uid = inv.IncomeAccountUid
          });
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = "Depotgebühren",
          EText = "2",
          Soll_Konto_Uid = inv.IncomeAccountUid,
          Haben_Konto_Uid = inv.SettlementAccountUid
        });
        r.Ergebnis.Add(new HhEreignis
        {
          Uid = Functions.GetUid(),
          Bezeichnung = anleihe ? "Anleiheverkauf" : "Wertpapierverkauf",
          EText = "1",
          Soll_Konto_Uid = inv.SettlementAccountUid,
          Haben_Konto_Uid = inv.PortfolioAccountUid
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
      memo = memo.TrimNull();
      var inv = string.IsNullOrWhiteSpace(inuid) ? null : WpAnlageRep.Get(daten, daten.MandantNr, inuid);
      if (inv == null)
        r.Errors.Add(Message.New(WP019));
      //if (datum === null) {
      //    throw new MeldungException(Meldungen::WP020);
      //}
      if (string.IsNullOrWhiteSpace(desc))
        r.Errors.Add(Message.New(WP021));
      if (Functions.compDouble(payment, 0) == 0 && Functions.compDouble(discount, 0) == 0 &&
          Functions.compDouble4(shares, 0) == 0 && Functions.compDouble(interest, 0) == 0)
        r.Errors.Add(Message.New(WP022));
      if (!r.Ok)
        return r;

      if (string.IsNullOrEmpty(buid) && !string.IsNullOrEmpty(duid) && !string.IsNullOrEmpty(cuid) && !string.IsNullOrEmpty(text))
      {
        var vdm = Functions.konvDM(v);
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
      if (Functions.compDouble(stand2, 0) <= 0)
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

    private static readonly Dictionary<string, SoKurse> Wkurse = new Dictionary<string, SoKurse>();

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
          Bewertung = shortcut
        };
      var key = $"{date} {shortcut}";
      Wkurse.TryGetValue(key, out var wert);
      if (wert != null)
        return wert;
      var accesskey = Parameter.GetValue(Parameter.WP_FIXER_IO_ACCESS_KEY);
      if (string.IsNullOrEmpty(accesskey))
        throw new MessageException(WP049);
      var url = $"http://data.fixer.io/api/{Functions.ToString(date)}?symbols={shortcut}&access_key={accesskey}";
      List<string> v = null;
      try
      {
        v = ExecuteHttps(url, false);
      }
      catch (Exception)
      {
        decimal? kurs = null;
        // TODO Standard-Devisen-Kurse auslagern
        // Kurse vom 02.04.2021
        if (shortcut == "USD")
          kurs = 1.1779m;
        else if (shortcut == "CHF")
          kurs = 1.1079m;
        if (kurs.HasValue)
        {
          var k = new SoKurse
          {
            Datum = date,
            Close = 1 / kurs.Value,
            Bewertung = shortcut,
          };
          Wkurse.Add(key, k);
          return k;
        }
        else
          throw;
      }
      if (v != null && v.Count > 0)
      {
        var jr = JObject.Parse(v[0]);
        var success = Functions.ToBool(jr["success"].ToString()) ?? false;
        if (!success)
        {
          var error = jr["error"]["info"].ToString();
          throw new Exception(error);
        }
        var jresult = jr["rates"];
        if (jresult[shortcut] != null)
        {
          var k = new SoKurse
          {
            Datum = Functions.ToDateTime(jr["date"].ToString()) ?? date,
            Close = Functions.ToDecimal(jresult[shortcut].ToString()) ?? 0,
            Bewertung = shortcut
          };
          if (k.Close != 0)
            k.Close = Functions.Round4(1 / k.Close) ?? 0;
          k.Open = k.Close;
          k.High = k.Close;
          k.Low = k.Close;
          Wkurse.Add(key, k);
          return k;
        }
      }
      return null;
    }

    /// <summary>
    /// Gets a list of URLs for price request.
    /// </summary>
    /// <param name="from">Beginning of the period.</param>
    /// <param name="to">End of the period.</param>
    /// <param name="source">Affected provider for prices.</param>
    /// <param name="shortcut">Affected shortcut for source</param>
    /// <param name="type">Affected type (stock or bond)</param>
    /// <returns>List of URLs for price request.</returns>
    private List<(DateTime, string)> GetPriceUrlsIntern(DateTime from, DateTime to,
        string source, string shortcut, string type)
    {
      var urls = new List<(DateTime, string)>();
      if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(shortcut))
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
        var url = $"https://www.ariva.de/quote/historic/historic.csv?secu={shortcut}&boerse_id=6&clean_split=1&clean_payout=0&clean_bezug=1&min_time={from.ToString("dd.MM.yyyy")}&max_time={to.ToString("dd.MM.yyyy")}&trenner=%3B&go=Download";
        urls.Add((to, url));
      }
      else if (source == "onvista")
      {
        if (string.IsNullOrWhiteSpace(type))
        {
          //var d = to.Year * 365 + to.DayOfYear - (from.Year * 365 + from.DayOfYear);
          //var span = $"{d}D";
          //var url = $"https://www.onvista.de/fonds/snapshotHistoryCSV?idNotation={shortcut}&datetimeTzStartRange={Functions.ToStringDe(from)}&timeSpan={span}&codeResolution=1D";
          var fr = from.ToString("dd.MM.yyyy");
          var d = to.Year * 365 + to.DayOfYear - (from.Year * 365 + from.DayOfYear);
          var span = d <= 31 ? "M1" : "Y1";
          var url = $"https://www.onvista.de/onvista/boxes/historicalquote/export.csv?notationId={shortcut}&dateStart={fr}&interval={span}";
          urls.Add((to, url));
        }
        else
        {
          var date = Functions.Workday(to);
          while (date >= from)
          {
            var d = Functions.ToEpochSecond(date) + 39944;
            var url = $"https://www.onvista.de/component/timesAndSalesCsv?codeMarket=_STU&idInstrument={shortcut}&idTypeCategory=2&day={d}";
            urls.Add((date, url));
            // if (l.Any())
            date = Functions.Workday(date.AddDays(-1));
            // else
            //   date = from.AddDays(-1); // Schleife beenden
          }
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
    /// <param name="shortcut">Affected shortcut for source</param>
    /// <param name="type">Affected type (stock or bond)</param>
    /// <param name="currency">Affected currency for bond.</param>
    /// <param name="price">Actual price.</param>
    /// <param name="currency">Affected stock uid.</param>
    /// <param name="dictresponse">Affected preemptively read responses.</param>
    /// <returns>List of prices.</returns>
    private List<SoKurse> GetPriceListIntern(ServiceDaten daten, DateTime from, DateTime to,
      string source, string shortcut, string type, string currency, decimal price,
      string uid = null, Dictionary<string, StockUrl> dictresponse = null)
    {
      var l = new List<SoKurse>();
      if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(shortcut))
        return l;
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
            for (var i = 0; jts != null && i < jts.Count(); i++)
            {
              var k = new SoKurse
              {
                Datum = Functions.ToDateTime(Functions.ToInt64(jts[i].ToString())).Date,
                Open = Functions.ToDecimal(jopen[i].ToString(), 4) ?? 0,
                High = Functions.ToDecimal(jhigh[i].ToString(), 4) ?? 0,
                Low = Functions.ToDecimal(jlow[i].ToString(), 4) ?? 0,
                Close = Functions.ToDecimal(jclose[i].ToString(), 4) ?? 0,
                Bewertung = jmeta["currency"].ToString().ToUpper()
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
              Bewertung = jmeta["currency"].ToString().ToUpper()
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
        var url = $"https://www.ariva.de/quote/historic/historic.csv?secu={shortcut}&boerse_id=6&clean_split=1&clean_payout=0&clean_bezug=1&min_time={from.ToString("dd.MM.yyyy")}&max_time={to.ToString("dd.MM.yyyy")}&trenner=%3B&go=Download";
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
              Price = 1
            };
            l.Add(k);
          }
        }
      }
      else if (source == "onvista")
      {
        if (string.IsNullOrWhiteSpace(type))
        {
          //var d = to.Year * 365 + to.DayOfYear - (from.Year * 365 + from.DayOfYear);
          //var span = $"{d}D";
          //var url = $"https://www.onvista.de/fonds/snapshotHistoryCSV?idNotation={shortcut}&datetimeTzStartRange={Functions.ToStringDe(from)}&timeSpan={span}&codeResolution=1D";
          var fr = from.ToString("dd.MM.yyyy");
          var d = to.Year * 365 + to.DayOfYear - (from.Year * 365 + from.DayOfYear);
          var span = d <= 31 ? "M1" : "Y1";
          var url = $"https://www.onvista.de/onvista/boxes/historicalquote/export.csv?notationId={shortcut}&dateStart={fr}&interval={span}";
          string response = null;
          if (dictresponse != null && dictresponse.TryGetValue(StockUrl.GetKey(uid, to), out var resp))
            response = resp.Response;
          var v = response == null ? ExecuteHttps(url, true) : Functions.SplitLines(response, true);
          var f = "Datum;Eroeffnung;Hoch;Tief;Schluss;Volumen";
          if (v[0] != f)
            throw new MessageException(WP050(v[0], f));
          for (var i = 1; i < v.Count; i++)
          {
            var c = Functions.DecodeCSV(v[i], ';', ';');
            if (c != null && c.Count >= 5)
            {
              var k = new SoKurse
              {
                Datum = Functions.ToDateTimeDe(Functions.TrimNull(c[0])) ?? daten.Heute,
                Open = Functions.ToDecimalDe(c[1]) ?? 0,
                High = Functions.ToDecimalDe(c[2]) ?? 0,
                Low = Functions.ToDecimalDe(c[3]) ?? 0,
                Close = Functions.ToDecimalDe(c[4]) ?? 0,
                Bewertung = string.IsNullOrWhiteSpace(currency) ? "EUR" : currency,
                Price = 1
              };
              if (k.Datum >= from && k.Datum <= to)
                l.Add(k);
            }
          }
        }
        else
        {
          var date = Functions.Workday(to);
          while (date >= from)
          {
            var d = Functions.ToEpochSecond(date) + 39944;
            var url = $"https://www.onvista.de/component/timesAndSalesCsv?codeMarket=_STU&idInstrument={shortcut}&idTypeCategory=2&day={d}";
            string response = null;
            if (dictresponse != null && dictresponse.TryGetValue(StockUrl.GetKey(uid, date), out var resp))
              response = resp.Response;
            var v = response == null ? ExecuteHttps(url, true) : Functions.SplitLines(response, true);
            var f = "Zeit;Kurs;Stück;Kumuliert";
            if (v[0] != f)
              throw new MessageException(WP050(v[0], f));
            var k = new SoKurse();
            for (var i = 1; i < v.Count; i++)
            {
              // absteigende Uhrzeit
              var c = Functions.DecodeCSV(v[i], ';', ';');
              if (c != null && c.Count >= 4)
              {
                // Prozent
                k.Open = (Functions.ToDecimal(c[1], english: true) ?? 0) / 100;
                if (i == 1)
                {
                  k.High = k.Open;
                  k.Low = k.Open;
                  k.Close = k.Open;
                }
                else
                {
                  k.High = Math.Max(k.High, k.Open);
                  k.Low = Math.Min(k.Low, k.Open);
                }
              }
            }
            if (Functions.compDouble4(k.Open, 0) != 0)
            {
              k.Datum = date;
              k.Bewertung = string.IsNullOrWhiteSpace(currency) ? "EUR" : currency;
              k.Price = 1;
              l.Add(k);
            }
            // if (l.Any())
            date = Functions.Workday(date.AddDays(-1));
            // else
            //   date = from.AddDays(-1); // Schleife beenden
          }
        }
      }
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
          // richtig skalieren: bei WATL.L Faktor 100.
          k.Open /= 10;
          k.High /= 10;
          k.Low /= 10;
          k.Close /= 10;
        }
      }
      l = l.OrderBy(a => a.Datum).ToList();
      return l;
    }

    /// <summary>HTTPS-Abfrage synchron mit HttpClient ausführen.</summary>
    private List<string> ExecuteHttps(string url, bool lines)
    {
      var task = ExecuteHttpsClient(url);
      task.Wait();
      var v = Functions.SplitLines(task.Result, lines);
      return v;
      // // ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
      // var l = new List<string>();
      // try
      // {
      //   var request = WebRequest.Create(url) as HttpWebRequest;
      //   using (var response = request.GetResponse() as HttpWebResponse)
      //   {
      //     if (response.StatusCode != HttpStatusCode.OK)
      //       throw new MessageException(WP012((int)response.StatusCode));
      //     using (var s = response.GetResponseStream())
      //     {
      //       var enc = string.IsNullOrEmpty(response.CharacterSet) ? null : Encoding.GetEncoding(response.CharacterSet);
      //       using (var r = new StreamReader(s, enc))
      //       {
      //         if (lines)
      //         {
      //           string line;
      //           while ((line = r.ReadLine()) != null)
      //             l.Add(line);
      //         }
      //         else
      //           l.Add(r.ReadToEnd());
      //       }
      //     }
      //   }
      // }
      // catch (Exception ex)
      // {
      //   throw new Exception($"{url}: {ex.Message}", ex);
      // }
      // return l;
    }

    /// <summary>HTTPS-Abfrage mit HttpClient ausführen.</summary>
    private Task<string> ExecuteHttpsClient(string url)
    {
      return httpsclient.GetStringAsync(url);
    }
  }
}
