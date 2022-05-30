// <copyright file="WP250Investments.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
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

  /// <summary>Controller für WP250Investments Dialog.</summary>
  public partial class WP250Investments : CsbpBin
  {
    /// <summary>State of calculation.</summary>
    StringBuilder Status = new StringBuilder();

    /// <summary>Cancel of calculation.</summary>
    StringBuilder Cancel = new StringBuilder();

#pragma warning disable CS0649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private Button refreshAction;

    /// <summary>Button UndoAction.</summary>
    [Builder.Object]
    private Button undoAction;

    /// <summary>Button RedoAction.</summary>
    [Builder.Object]
    private Button redoAction;

    /// <summary>Button NewAction.</summary>
    [Builder.Object]
    private Button newAction;

    /// <summary>Button CopyAction.</summary>
    [Builder.Object]
    private Button copyAction;

    /// <summary>Button EditAction.</summary>
    [Builder.Object]
    private Button editAction;

    /// <summary>Button DeleteAction.</summary>
    [Builder.Object]
    private Button deleteAction;

    /// <summary>Label anlagen0.</summary>
    [Builder.Object]
    private Label anlagen0;

    /// <summary>TreeView anlagen.</summary>
    [Builder.Object]
    private TreeView anlagen;

    /// <summary>Label anlagenStatus.</summary>
    [Builder.Object]
    private Label anlagenStatus;

    /// <summary>Label bis0.</summary>
    [Builder.Object]
    private Label bis0;

    /// <summary>Date Bis.</summary>
    //[Builder.Object]
    private Date bis;

    /// <summary>CheckButton auchinaktiv.</summary>
    [Builder.Object]
    private CheckButton auchinaktiv;

    /// <summary>Label bezeichnung0.</summary>
    [Builder.Object]
    private Label bezeichnung0;

    /// <summary>Entry bezeichnung.</summary>
    [Builder.Object]
    private Entry bezeichnung;

    /// <summary>Label wertpapier0.</summary>
    [Builder.Object]
    private Label wertpapier0;

    /// <summary>ComboBox wertpapier.</summary>
    [Builder.Object]
    private ComboBox wertpapier;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP250Investments Create(object p1 = null, CsbpBin p = null)
    {
      return new WP250Investments(GetBuilder("WP250Investments", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP250Investments(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
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
        var uid = WP260Investment.lastcopyuid;
        WP260Investment.lastcopyuid = null;
        RefreshTreeView(anlagen, 1, uid);
      });
      // SetBold(client0);
      InitData(0);
      // anlagen.GrabFocus();
      bezeichnung.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
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
          // Nr.;Bezeichnung;Status;Provider;Kürzel;Betrag;Wert;Anteile;Gewinn;+/-;Valuta;Währung;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Bezeichnung, UiFunctions.GetStockState(e.State.ToString(), e.StockShortcut),
            e.StockProvider, e.StockShortcut, Functions.ToString(e.Payment, 2),
            Functions.ToString(e.Shares, 2), Functions.ToString(e.Value, 2), Functions.ToString(e.Profit, 2),
            Functions.ToString(e.Value2 == 0 ? 0 : e.Value - e.Value2, 2),
            Functions.ToString(e.PriceDate), e.Currency, Functions.ToString(e.Geaendert_Am, true),
            e.Geaendert_Von, Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
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

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    protected override void UpdateParent()
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      // RefreshTreeView(anlagen, 1);
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
      if (MainClass.Undo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von Redo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRedoClicked(object sender, EventArgs e)
    {
      if (MainClass.Redo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von New.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNewClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.New);
    }

    /// <summary>Behandlung von Copy.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnCopyClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Copy);
    }

    /// <summary>Behandlung von Edit.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnEditClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Edit);
    }

    /// <summary>Behandlung von Delete.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDeleteClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Delete);
    }

    /// <summary>Behandlung von Chart.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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

    /// <summary>Behandlung von Details.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDetailsClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(anlagen);
      var desc = GetValue<string>(anlagen, column: 1);
      MainClass.MainWindow.AppendPage(WP400Bookings.Create(uid), desc);
    }

    /// <summary>Behandlung von Anlagen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAnlagenRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Bis.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von Auchinaktiv.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAuchinaktivChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Bezeichnung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBezeichnungKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Wertpapier.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnWertpapierChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      Parameter.WP250Stock = GetText(wertpapier);
      refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      SetText(wertpapier, null);
      RefreshTreeView(anlagen, 0);
    }

    /// <summary>Behandlung von Berechnen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBerechnenClicked(object sender, EventArgs e)
    {
      CalculateInvestments();
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      Cancel.Append("Cancel");
    }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.
    private async void CalculateInvestments()
#pragma warning restore RECS0165
    {
      try
      {
        // ShowStatus(Status, Cancel);
        Status.Clear();
        Cancel.Clear();
        var r = await Task.Run(() =>
        {
          var r0 = FactoryService.StockService.CalculateInvestments(ServiceDaten, null,
            null, GetText(wertpapier), bis.ValueNn, auchinaktiv.Active, bezeichnung.Text, Status, Cancel);
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
      var uid = GetValue<string>(anlagen, dt != DialogTypeEnum.New);
      Start(typeof(WP260Investment), WP260_title, dt, uid, csbpparent: this);
    }
  }
}
