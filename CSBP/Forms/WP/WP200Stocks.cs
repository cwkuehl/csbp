// <copyright file="WP200Stocks.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for WP200Stocks dialog.</summary>
public partial class WP200Stocks : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView wertpapiere.</summary>
  [Builder.Object]
  private readonly TreeView wertpapiere;

  /// <summary>Label status.</summary>
  [Builder.Object]
  private readonly Label status;

  /// <summary>Date Bis.</summary>
  //// [Builder.Object]
  private readonly Date bis;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>Entry muster.</summary>
  [Builder.Object]
  private readonly Entry muster;

  /// <summary>CheckButton auchinaktiv.</summary>
  [Builder.Object]
  private readonly CheckButton auchinaktiv;

  /// <summary>ComboBox konfiguration.</summary>
  [Builder.Object]
  private readonly ComboBox konfiguration;

#pragma warning restore CS0649

  /// <summary>State of calculation.</summary>
  private readonly StringBuilder state = new();

  /// <summary>Cancel of calculation.</summary>
  private readonly StringBuilder cancel = new();

  /// <summary>Initializes a new instance of the <see cref="WP200Stocks"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP200Stocks(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(WP200Stocks), dt, p1, p)
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
      var uid = WP210Stock.Lastcopyuid;
      WP210Stock.Lastcopyuid = null;
      RefreshTreeView(wertpapiere, 1, uid);
    });
    //// SetBold(client0);
    InitData(0);
    wertpapiere.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP200Stocks Create(object p1 = null, CsbpBin p = null)
  {
    return new WP200Stocks(GetBuilder("WP200Stocks", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      bis.Value = DateTime.Today;
      SetText(bezeichnung, "%%");
      SetText(muster, "%%");
      var kliste = Get(FactoryService.StockService.GetConfigurationList(ServiceDaten, true, "1"));
      var rs = AddColumns(konfiguration, emptyentry: true);
      foreach (var p in kliste)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      SetText(konfiguration, Parameter.WP200Configuration);
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.StockService.GetStockList(ServiceDaten, auchinaktiv.Active, null, muster.Text, null, bezeichnung.Text));
      //// No.;Description;St.;Provider;Shortcut;Relation;Rating;Trend;Box 0.5;T;1;T;2;T;3;T;5;T;XO;Bew.;Date;Signal;200;Changed at;Changed by;Created at;Created by
      var anz = l.Count;
      var values = new List<string[]>();
      foreach (var e in l)
      {
        values.Add(new string[]
        {
          e.Uid, e.Sorting, e.Bezeichnung, UiFunctions.GetStockState(e.Status, e.Kuerzel),
          e.Datenquelle, e.Kuerzel, e.RelationDescription, e.Assessment, e.Trend,
          e.Assessment1, e.Trend1, e.Assessment2, e.Trend2, e.Assessment3, e.Trend3,
          e.Assessment4, e.Trend4, e.Assessment5, e.Trend5, e.Xo, e.SignalAssessment,
          Functions.ToString(e.SignalDate), e.SignalDescription, e.Average200,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      SetText(status, WP056(anz));
      AddStringColumnsSort(wertpapiere, WP200_wertpapiere_columns, values);
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
    // RefreshTreeView(wertpapiere, 1);
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

  /// <summary>Handles Floppy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFloppyClicked(object sender, EventArgs e)
  {
    Start(typeof(WP220Interface), WP220_title, csbpparent: this);
  }

  /// <summary>Handles Chart.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnChartClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(wertpapiere);
    var desc = GetValue<string>(wertpapiere, column: 2);
    var kuid = GetText(konfiguration);
    var t = new Tuple<DateTime?, string, string>(bis.Value, uid, kuid);
    //// Start(typeof(WP100Chart), Messages.WP100_title, DialogTypeEnum.Without, t, mbpparent: this);
    MainClass.MainWindow.AppendPage(WP100Chart.Create(t), desc);
  }

  /// <summary>Handles Wertpapiere.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWertpapiereRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    //// refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(wertpapiere, 0);
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

  /// <summary>Handles Berechnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBerechnenClicked(object sender, EventArgs e)
  {
    CalculateStocks();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    cancel.Append("cancel");
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

  /// <summary>Handles Muster.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMusterKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Konfiguration.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKonfigurationChanged(object sender, EventArgs e)
  {
    Parameter.WP200Configuration = GetText(konfiguration);
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.
  private async void CalculateStocks()
#pragma warning restore RECS0165
  {
    try
    {
      state.Clear();
      cancel.Clear();
      var r = await Task.Run(() =>
      {
        var r0 = FactoryService.StockService.CalculateStocks(ServiceDaten, null, muster.Text,
          null, bis.ValueNn, auchinaktiv.Active, bezeichnung.Text, GetText(konfiguration), state, cancel);
        return r0;
      });
      r.ThrowAllErrors();
      Application.Invoke((sender, e) => { refreshAction.Click(); });
    }
    catch (Exception ex)
    {
      Application.Invoke((sender, e) => { ShowError(ex.Message); });
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
    var uid = GetValue<string>(wertpapiere, dt != DialogTypeEnum.New);
    Start(typeof(WP210Stock), WP210_title, dt, uid, csbpparent: this);
  }
}
