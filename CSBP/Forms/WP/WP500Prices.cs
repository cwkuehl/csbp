// <copyright file="WP500Prices.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP500Prices Dialog.</summary>
  public partial class WP500Prices : CsbpBin
  {
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

    /// <summary>Label staende0.</summary>
    [Builder.Object]
    private Label staende0;

    /// <summary>TreeView staende.</summary>
    [Builder.Object]
    private TreeView staende;

    /// <summary>Label wertpapier0.</summary>
    [Builder.Object]
    private Label wertpapier0;

    /// <summary>ComboBox wertpapier.</summary>
    [Builder.Object]
    private ComboBox wertpapier;

    /// <summary>Label von0.</summary>
    [Builder.Object]
    private Label von0;

    /// <summary>Date Von.</summary>
    //[Builder.Object]
    private Date von;

    /// <summary>Label bis0.</summary>
    [Builder.Object]
    private Label bis0;

    /// <summary>Date Bis.</summary>
    //[Builder.Object]
    private Date bis;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP500Prices Create(object p1 = null, CsbpBin p = null)
    {
      return new WP500Prices(GetBuilder("WP500Prices", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP500Prices(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      von = new Date(Builder.GetObject("von").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      von.DateChanged += OnVonDateChanged;
      von.Show();
      bis = new Date(Builder.GetObject("bis").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      bis.DateChanged += OnBisDateChanged;
      bis.Show();
      // SetBold(client0);
      InitData(0);
      staende.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        EventsActive = false;
        von.Value = null;
        bis.Value = null;
        var rl = Get(FactoryService.StockService.GetStockList(daten, true)) ?? new List<WpWertpapier>();
        var rs = AddColumns(wertpapier, emptyentry: true);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        SetText(wertpapier, Parameter.WP500Stock);
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.StockService.GetPriceList(ServiceDaten, GetText(wertpapier),
          von.Value, bis.Value)) ?? new List<WpStand>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Wertpapier;Datum;Kurs;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Wertpapier_Uid, e.StockDescription, Functions.ToString(e.Datum),
            Functions.ToString(e.Stueckpreis, 4), Functions.ToString(e.Geaendert_Am, true),
            e.Geaendert_Von, Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(staende, WP500_staende_columns, values);
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
      RefreshTreeView(staende, 1);
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

    /// <summary>Behandlung von Staende.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnStaendeRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(staende, 0);
      SetText(wertpapier, null);
    }

    /// <summary>Behandlung von Wertpapier.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnWertpapierChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      Parameter.WP500Stock = GetText(wertpapier);
      refreshAction.Click();
    }

    /// <summary>Behandlung von von.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von bis.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(staende, dt != DialogTypeEnum.New);
      var date = Functions.ToDateTime(GetValue<string>(staende, dt != DialogTypeEnum.New, 2));
      var key = new Tuple<string, DateTime?>(uid, date);
      Start(typeof(WP510Price), WP510_title, dt, key, csbpparent: this);
    }

  }
}
