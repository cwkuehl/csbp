// <copyright file="WP400Bookings.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP400Bookings Dialog.</summary>
  public partial class WP400Bookings : CsbpBin
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

    /// <summary>Label buchungen0.</summary>
    [Builder.Object]
    private Label buchungen0;

    /// <summary>TreeView buchungen.</summary>
    [Builder.Object]
    private TreeView buchungen;

    /// <summary>Label bezeichnung0.</summary>
    [Builder.Object]
    private Label bezeichnung0;

    /// <summary>Entry bezeichnung.</summary>
    [Builder.Object]
    private Entry bezeichnung;

    /// <summary>Label anlage0.</summary>
    [Builder.Object]
    private Label anlage0;

    /// <summary>ComboBox anlage.</summary>
    [Builder.Object]
    private ComboBox anlage;

    /// <summary>Button alle.</summary>
    [Builder.Object]
    private Button alle;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP400Bookings Create(object p1 = null, CsbpBin p = null)
    {
      return new WP400Bookings(GetBuilder("WP400Bookings", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP400Bookings(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      // SetBold(client0);
      InitData(0);
      buchungen.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        EventsActive = false;
        bezeichnung.Text = "%%";
        var rl = Get(FactoryService.StockService.GetInvestmentList(daten, true)) ?? new List<WpAnlage>();
        var rs = AddColumns(anlage, emptyentry: true);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        var p1 = Parameter1 as string;
        var invuid = string.IsNullOrEmpty(p1) ? Parameter.WP400Investment : p1;
        SetText(anlage, invuid);
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.StockService.GetBookingList(daten, bezeichnung.Text,
            null, null, GetText(anlage))) ?? new List<WpBuchung>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Wertpapier;Bezeichnung;Datum;Buchungstext;Betrag;Rabatt;Anteile;Zinsen;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.StockDescription, e.InvestmentDescription, Functions.ToString(e.Datum),
            e.BText, Functions.ToString(e.Zahlungsbetrag, 2), Functions.ToString(e.Rabattbetrag, 2),
            Functions.ToString(e.Anteile, 5), Functions.ToString(e.Zinsen, 2),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(buchungen, WP400_buchungen_columns, values);
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
      var uid = WP410Booking.lastcopyuid;
      WP410Booking.lastcopyuid = null;
      RefreshTreeView(buchungen, 1, uid);
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

    /// <summary>Behandlung von Buchungen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBuchungenRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
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

    /// <summary>Behandlung von Anlage.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAnlageChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      Parameter.WP400Investment = GetText(anlage);
      refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      SetText(anlage, null);
      RefreshTreeView(buchungen, 0);
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(buchungen, dt != DialogTypeEnum.New);
      if (dt == DialogTypeEnum.New)
        uid = GetText(anlage);
      Start(typeof(WP410Booking), WP410_title, dt, uid, csbpparent: this);
    }
  }
}
