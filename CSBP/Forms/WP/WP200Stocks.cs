// <copyright file="WP200Stocks.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using CSBP.Apis.Enums;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP200Stocks Dialog.</summary>
  public partial class WP200Stocks : CsbpBin
  {
    /// <summary>Status für die Berechnung.</summary>
    StringBuilder Status = new StringBuilder();

    /// <summary>Abbruch der Berechnung.</summary>
    StringBuilder Cancel = new StringBuilder();

#pragma warning disable 169, 649

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

    /// <summary>Button SaveAction.</summary>
    [Builder.Object]
    private Button saveAction;

    /// <summary>Label wertpapiere0.</summary>
    [Builder.Object]
    private Label wertpapiere0;

    /// <summary>TreeView wertpapiere.</summary>
    [Builder.Object]
    private TreeView wertpapiere;

    /// <summary>Label bis0.</summary>
    [Builder.Object]
    private Label bis0;

    /// <summary>Date Bis.</summary>
    //[Builder.Object]
    private Date bis;

    /// <summary>Button alle.</summary>
    [Builder.Object]
    private Button alle;

    /// <summary>Label bezeichnung0.</summary>
    [Builder.Object]
    private Label bezeichnung0;

    /// <summary>Entry bezeichnung.</summary>
    [Builder.Object]
    private Entry bezeichnung;

    /// <summary>Label muster0.</summary>
    [Builder.Object]
    private Label muster0;

    /// <summary>Entry muster.</summary>
    [Builder.Object]
    private Entry muster;

    /// <summary>CheckButton auchinaktiv.</summary>
    [Builder.Object]
    private CheckButton auchinaktiv;

    /// <summary>Label konfiguration0.</summary>
    [Builder.Object]
    private Label konfiguration0;

    /// <summary>ComboBox konfiguration.</summary>
    [Builder.Object]
    private ComboBox konfiguration;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP200Stocks Create(object p1 = null, CsbpBin p = null)
    {
      return new WP200Stocks(GetBuilder("WP200Stocks", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
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
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(wertpapiere, 1); });
      // SetBold(client0);
      InitData(0);
      wertpapiere.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
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
        // Nr.;Sort.;Name;Provider;Kürzel;Relation;Bewertung;Trend;Box 0.5;T;1;T;2;T;3;T;5;T;XO;Bew.;Datum;Signal;200;Geändert am;Geändert von;Angelegt am;Angelegt von
        var values = new List<string[]>();
        foreach (var e in l)
        {
          values.Add(new string[] { e.Uid, e.Sorting, e.Bezeichnung, e.Datenquelle, e.Kuerzel,
            e.RelationDescription, e.Assessment, e.Trend,
            e.Assessment1, e.Trend1, e.Assessment2, e.Trend2, e.Assessment3, e.Trend3,
            e.Assessment4, e.Trend4, e.Assessment5, e.Trend5, e.Xo, e.SignalAssessment,
            Functions.ToString(e.SignalDate), e.SignalDescription, e.Average200,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(wertpapiere, WP200_wertpapiere_columns, values);
      }
    }

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    override protected void UpdateParent()
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      // RefreshTreeView(wertpapiere, 1);
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

    /// <summary>Behandlung von Save.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSaveClicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Chart.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnChartClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(wertpapiere);
      var desc = GetValue<string>(wertpapiere, column: 2);
      var kuid = GetText(konfiguration);
      var t = new Tuple<DateTime?, string, string>(bis.Value, uid, kuid);
      //Start(typeof(WP100Chart), Messages.WP100_title, DialogTypeEnum.Without, t, mbpparent: this);
      MainClass.MainWindow.AppendPage(WP100Chart.Create(t), desc);
    }

    /// <summary>Behandlung von Wertpapiere.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnWertpapiereRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von bis.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
    {
      if (!EventsActive)
        return;
      // refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(wertpapiere, 0);
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

    /// <summary>Behandlung von Berechnen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBerechnenClicked(object sender, EventArgs e)
    {
      try
      {
        // var password = "";
        // var uid = GetValue<string>(verzeichnisse);
        // if (restore && !ShowYesNoQuestion(M0(AG001)))
        //   return;
        // var be = Get(FactoryService.ClientService.GetBackupEntry(ServiceDaten, uid));
        // if (be != null && be.Encrypted)
        // {
        //   password = (string)Start(typeof(AG420Encryption), AG420_title, parameter1: uid, modal: true, csbpparent: this);
        //   if (string.IsNullOrEmpty(password))
        //     return;
        // }
        ShowStatus(Status, Cancel);
        // var r = await Task.Run(() =>
        // {
        //   var r0 = FactoryService.StockService.CalculateInvestments(ServiceDaten, uid, restore, password, Status, Cancel);
        //   return r0;
        // });
        // r.ThrowAllErrors();
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

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      Cancel.Append("Cancel");
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

    /// <summary>Behandlung von Muster.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMusterKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Konfiguration.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKonfigurationChanged(object sender, EventArgs e)
    {
      Parameter.WP200Configuration = GetText(konfiguration);
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(wertpapiere, dt != DialogTypeEnum.New);
      Start(typeof(WP210Stock), WP210_title, dt, uid, csbpparent: this);
    }
  }
}
