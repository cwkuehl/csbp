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
  /// <summary>State of calculation.</summary>
  readonly StringBuilder Status = new();

  /// <summary>Cancel of calculation.</summary>
  readonly StringBuilder Cancel = new();

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
  //[Builder.Object]
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

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static WP200Stocks Create(object p1 = null, CsbpBin p = null)
  {
    return new WP200Stocks(GetBuilder("WP200Stocks", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public WP200Stocks(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    bis = new Date(Builder.GetObject("bis").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false
    };
    bis.DateChanged += OnBisDateChanged;
    bis.Show();
    ObservableEventThrottle(refreshAction, delegate
    {
      var uid = WP210Stock.Lastcopyuid;
      WP210Stock.Lastcopyuid = null;
      RefreshTreeView(wertpapiere, 1, uid);
    });
    // SetBold(client0);
    InitData(0);
    wertpapiere.GrabFocus();
  }

  /// <summary>Model-Daten initialisieren.</summary>
  /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      bis.Value = DateTime.Today;
      bezeichnung.Text = "%%";
      muster.Text = "%%";
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
      // Nr.;Sort.;Name;Status;Provider;Kürzel;Relation;Bewertung;Trend;Box 0.5;T;1;T;2;T;3;T;5;T;XO;Bew.;Datum;Signal;200;Geändert am;Geändert von;Angelegt am;Angelegt von
      var anz = l.Count;
      var values = new List<string[]>();
      foreach (var e in l)
      {
        values.Add(new string[] { e.Uid, e.Sorting, e.Bezeichnung, UiFunctions.GetStockState(e.Status, e.Kuerzel),
            e.Datenquelle, e.Kuerzel, e.RelationDescription, e.Assessment, e.Trend,
            e.Assessment1, e.Trend1, e.Assessment2, e.Trend2, e.Assessment3, e.Trend3,
            e.Assessment4, e.Trend4, e.Assessment5, e.Trend5, e.Xo, e.SignalAssessment,
            Functions.ToString(e.SignalDate), e.SignalDescription, e.Average200,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
      }
      status.Text = WP056(anz);
      AddStringColumnsSort(wertpapiere, WP200_wertpapiere_columns, values);
    }
  }

  /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handle Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // RefreshTreeView(wertpapiere, 1);
  }

  /// <summary>Handle Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handle Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      refreshAction.Click();
  }

  /// <summary>Handle New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.New);
  }

  /// <summary>Handle Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Copy);
  }

  /// <summary>Handle Edit.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEditClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handle Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Delete);
  }

  /// <summary>Handle Floppy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFloppyClicked(object sender, EventArgs e)
  {
    Start(typeof(WP220Interface), WP220_title, csbpparent: this);
  }

  /// <summary>Handle Chart.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnChartClicked(object sender, EventArgs e)
  {
    var uid = GetValue<string>(wertpapiere);
    var desc = GetValue<string>(wertpapiere, column: 2);
    var kuid = GetText(konfiguration);
    var t = new Tuple<DateTime?, string, string>(bis.Value, uid, kuid);
    //Start(typeof(WP100Chart), Messages.WP100_title, DialogTypeEnum.Without, t, mbpparent: this);
    MainClass.MainWindow.AppendPage(WP100Chart.Create(t), desc);
  }

  /// <summary>Handle Wertpapiere.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWertpapiereRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handle bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    // refreshAction.Click();
  }

  /// <summary>Handle Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(wertpapiere, 0);
  }

  /// <summary>Handle Auchinaktiv.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAuchinaktivChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handle Berechnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBerechnenClicked(object sender, EventArgs e)
  {
    CalculateStocks();
  }

  /// <summary>Handle Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    Cancel.Append("Cancel");
  }

  /// <summary>Handle Bezeichnung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBezeichnungKeyReleaseEvent(object o, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handle Muster.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMusterKeyReleaseEvent(object o, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handle Konfiguration.</summary>
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
      Status.Clear();
      Cancel.Clear();
      var r = await Task.Run(() =>
      {
        var r0 = FactoryService.StockService.CalculateStocks(ServiceDaten, null, muster.Text,
          null, bis.ValueNn, auchinaktiv.Active, bezeichnung.Text, GetText(konfiguration), Status, Cancel);
        return r0;
      });
      r.ThrowAllErrors();
      Application.Invoke(delegate
      {
        refreshAction.Click();
      });
    }
    catch (Exception ex)
    {
      Application.Invoke(delegate
      {
        ShowError(ex.Message);
      });
    }
    finally
    {
      Cancel.Append("End");
    }
  }

  /// <summary>Starten des Details-Dialogs.</summary>
  /// <param name="dt">Betroffener Dialog-Typ.</param>
  void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(wertpapiere, dt != DialogTypeEnum.New);
    Start(typeof(WP210Stock), WP210_title, dt, uid, csbpparent: this);
  }
}
