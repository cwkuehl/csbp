// <copyright file="WP250Investments.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for WP250Investments dialog.</summary>
public partial class WP250Investments : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView anlagen.</summary>
  [Builder.Object]
  private readonly TreeView anlagen;

  /// <summary>Label anlagenStatus.</summary>
  [Builder.Object]
  private readonly Label anlagenStatus;

  /// <summary>Date Bis.</summary>
  //// [Builder.Object]
  private readonly Date bis;

  /// <summary>CheckButton auchinaktiv.</summary>
  [Builder.Object]
  private readonly CheckButton auchinaktiv;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>ComboBox wertpapier.</summary>
  [Builder.Object]
  private readonly ComboBox wertpapier;

#pragma warning restore CS0649

  /// <summary>State of calculation.</summary>
  private readonly StringBuilder state = new();

  /// <summary>Cancel of calculation.</summary>
  private readonly StringBuilder cancel = new();

  /// <summary>Initializes a new instance of the <see cref="WP250Investments"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP250Investments(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    bis = new Date(Builder.GetObject("bis").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    bis.DateChanged += OnBisDateChanged;
    bis.Show();
    ObservableEventThrottle(refreshAction, (sender, e) =>
    {
      var uid = WP260Investment.Lastcopyuid;
      WP260Investment.Lastcopyuid = null;
      RefreshTreeView(anlagen, 1, uid);
    });
    //// SetBold(client0);
    InitData(0);
    //// anlagen.GrabFocus();
    bezeichnung.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP250Investments Create(object p1 = null, CsbpBin p = null)
  {
    return new WP250Investments(GetBuilder("WP250Investments", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      bis.Value = daten.Heute;
      bezeichnung.Text = "%%";
      var rl = Get(FactoryService.StockService.GetStockList(daten, true));
      var rs = AddColumns(wertpapier, emptyentry: true);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      SetText(wertpapier, Parameter.WP250Stock);
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.StockService.GetInvestmentList(ServiceDaten, auchinaktiv.Active,
        null, null, GetText(wertpapier), bezeichnung.Text)) ?? new List<WpAnlage>();
      var values = new List<string[]>();
      var anz = l.Count;
      var summe = 0m;
      var wert = 0m;
      var gewinn = 0m;
      var diff = 0m;
      foreach (var e in l)
      {
        // No.;Description;St.;Provider;Shortcut;Payment_r;Shares_r;Value_r;Gain_r;+/-_r;Date;Currency;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Bezeichnung, UiFunctions.GetStockState(e.State.ToString(), e.StockShortcut),
          e.StockProvider, e.StockShortcut, Functions.ToString(e.Payment, 2),
          Functions.ToString(e.Shares, 2), Functions.ToString(e.Value, 2), Functions.ToString(e.Profit, 2),
          Functions.ToString(e.Value2 == 0 ? 0 : e.Value - e.Value2, 2),
          Functions.ToString(e.PriceDate), e.Currency, Functions.ToString(e.Geaendert_Am, true),
          e.Geaendert_Von, Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
        summe += e.Payment;
        wert += e.Value;
        gewinn += e.Profit;
        diff += e.Value2 == 0 ? 0 : e.Value - e.Value2;
      }
      var pgewinn = (wert == 0 || summe == 0) ? 0 : (gewinn < 0) ? gewinn / wert * 100 : gewinn / summe * 100;
      anlagenStatus.Text = WP029(anz, summe, wert, gewinn, pgewinn, diff);
      AddStringColumnsSort(anlagen, WP250_anlagen_columns, values);
    }
  }

  /// <summary>Updates parent dialog.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handles Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // RefreshTreeView(anlagen, 1);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      refreshAction.Click();
  }

  /// <summary>Handles New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.New);
  }

  /// <summary>Handles Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Copy);
  }

  /// <summary>Handles Edit.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEditClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Delete);
  }

  /// <summary>Handles Chart.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnChartClicked(object sender, EventArgs e)
  {
    var inv = Get(FactoryService.StockService.GetInvestment(ServiceDaten, GetValue<string>(anlagen)));
    if (inv == null)
      return;
    var uid = inv.Wertpapier_Uid;
    var desc = inv.StockDescription;
    var kuid = Parameter.WP200Configuration;
    var t = new Tuple<DateTime?, string, string>(bis.Value, uid, kuid);
    MainClass.MainWindow.AppendPage(WP100Chart.Create(t), desc);
  }

  /// <summary>Handles Details.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDetailsClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(anlagen);
    var desc = GetValue<string>(anlagen, column: 1);
    MainClass.MainWindow.AppendPage(WP400Bookings.Create(uid), desc);
  }

  /// <summary>Handles Anlagen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAnlagenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Auchinaktiv.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAuchinaktivChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Bezeichnung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBezeichnungKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Wertpapier.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWertpapierChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    Parameter.WP250Stock = GetText(wertpapier);
    refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    SetText(wertpapier, null);
    RefreshTreeView(anlagen, 0);
  }

  /// <summary>Handles Berechnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBerechnenClicked(object sender, EventArgs e)
  {
    CalculateInvestments();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    cancel.Append("cancel");
  }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.
  private async void CalculateInvestments()
#pragma warning restore RECS0165
  {
    try
    {
      // ShowStatus(state, cancel);
      state.Clear();
      cancel.Clear();
      var r = await Task.Run(() =>
      {
        var r0 = FactoryService.StockService.CalculateInvestments(ServiceDaten, null,
          null, GetText(wertpapier), bis.ValueNn, auchinaktiv.Active, bezeichnung.Text, state, cancel);
        return r0;
      });
      r.ThrowAllErrors();
      Application.Invoke((sender, e) =>
      {
        refreshAction.Click();
      });
    }
    catch (Exception ex)
    {
      Application.Invoke((sender, e) =>
      {
        ShowError(ex.Message);
      });
    }
    finally
    {
      cancel.Append("End");
    }
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(anlagen, dt != DialogTypeEnum.New);
    Start(typeof(WP260Investment), WP260_title, dt, uid, csbpparent: this);
  }
}
