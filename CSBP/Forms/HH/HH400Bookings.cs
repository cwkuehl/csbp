// <copyright file="HH400Bookings.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH
{
  using System;
  using System.Collections.Generic;
  using System.Reactive.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für HH400Bookings Dialog.</summary>
  public partial class HH400Bookings : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    string lastreverse;

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

    /// <summary>Label buchungen0.</summary>
    [Builder.Object]
    private Label buchungen0;

    /// <summary>TreeView buchungen.</summary>
    [Builder.Object]
    private TreeView buchungen;

    /// <summary>Label buchungenStatus.</summary>
    [Builder.Object]
    private Label buchungenStatus;

    /// <summary>Label kennzeichen0.</summary>
    [Builder.Object]
    private Label kennzeichen0;

    /// <summary>RadioButton kennzeichen1.</summary>
    [Builder.Object]
    private RadioButton kennzeichen1;

    /// <summary>RadioButton kennzeichen2.</summary>
    [Builder.Object]
    private RadioButton kennzeichen2;

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

    /// <summary>Label bText0.</summary>
    [Builder.Object]
    private Label bText0;

    /// <summary>Entry bText.</summary>
    [Builder.Object]
    private Entry bText;

    /// <summary>Label betrag0.</summary>
    [Builder.Object]
    private Label betrag0;

    /// <summary>Entry betrag.</summary>
    [Builder.Object]
    private Entry betrag;

    /// <summary>Label konto0.</summary>
    [Builder.Object]
    private Label konto0;

    /// <summary>ComboBox konto.</summary>
    [Builder.Object]
    private ComboBox konto;

    /// <summary>Button alle.</summary>
    [Builder.Object]
    private Button alle;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static HH400Bookings Create(object p1 = null, CsbpBin p = null)
    {
      return new HH400Bookings(GetBuilder("HH400Bookings", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public HH400Bookings(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
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
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(buchungen, 1); });
      // SetBold(client0);
      InitData(0);
      buchungen.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        EventsActive = false;
        SetUserData(new[] { kennzeichen1, kennzeichen2 }, new[] { "1", "0" });
        var kl = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, null, von.Value, bis.Value));
        var rs = AddColumns(konto, emptyentry: true);
        foreach (var p in kl)
          rs.AppendValues(p.Name, p.Uid);
        SetText(kennzeichen1, "1");
        var d = ServiceDaten.Heute;
        bis.Value = d.AddDays(1 - d.Day).AddMonths(1);
        von.Value = d.AddDays(1 - d.Day).AddMonths(-14);
        bText.Text = "%%";
        betrag.Text = "";
        SetText(konto, "");
        // Parameter
        var p1 = Parameter1 as Tuple<HhKonto, DateTime, DateTime>;
        if (p1 != null)
        {
          SetText(konto, p1.Item1.Uid);
          von.Value = p1.Item2;
          bis.Value = p1.Item3;
        }
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.BudgetService.GetBookingList(ServiceDaten, GetText(kennzeichen1) == "1",
            von.Value, bis.Value, bText.Text, GetText(konto), betrag.Text)) ?? new List<HhBuchung>();
        var anz = l.Count;
        var summe = 0m;
        // Nr.;Valuta;K.;Betrag;Buchungstext;Sollkonto;Habenkonto;Beleg;Geändert am;Geändert von;Angelegt am;Angelegt von
        var values = new List<string[]>();
        foreach (var e in l)
        {
          summe += e.EBetrag;
          values.Add(new string[] { e.Uid, Functions.ToString(e.Soll_Valuta), e.Kz,
            Functions.ToString(e.EBetrag, 2), e.BText, e.DebitName, e.CreditName, e.Beleg_Nr,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        buchungenStatus.Text = HH054(anz, summe);
        AddStringColumnsSort(buchungen, HH400_buchungen_columns, values);
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
      // RefreshTreeView(buchungen, 1);
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
      var file = SelectFile(HH400_select_file);
      var lines = Get(FactoryService.BudgetService.ExportBookingList(ServiceDaten,
          GetText(kennzeichen1) == "1", von.Value, bis.Value, bText.Text, GetText(konto), betrag.Text));
      UiTools.SaveFile(lines, file, open: true);
    }

    /// <summary>Behandlung von Buchungen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBuchungenRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
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

    /// <summary>Behandlung von Konto.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKontoChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Buchungstext.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBTextKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Betrag.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBetragKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(buchungen, 0);
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(buchungen, dt != DialogTypeEnum.New);
      if (dt == DialogTypeEnum.Delete)
      {
        if (lastreverse != uid)
          dt = DialogTypeEnum.Reverse;
        lastreverse = uid;
      }
      Start(typeof(HH410Booking), HH410_title, dt, uid, csbpparent: this);
    }
  }
}
