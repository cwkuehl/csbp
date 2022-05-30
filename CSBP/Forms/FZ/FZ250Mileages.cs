// <copyright file="FZ250Mileages.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für FZ250Mileages Dialog.</summary>
  public partial class FZ250Mileages : CsbpBin
  {
#pragma warning disable CS0649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private readonly Button refreshAction;

    /// <summary>Button EditAction.</summary>
    [Builder.Object]
    private readonly Button editAction;

    /// <summary>TreeView fahrradstaende.</summary>
    [Builder.Object]
    private readonly TreeView fahrradstaende;

    /// <summary>ComboBox fahrrad.</summary>
    [Builder.Object]
    private readonly ComboBox fahrrad;

    /// <summary>Entry text.</summary>
    [Builder.Object]
    private readonly Entry text;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static FZ250Mileages Create(object p1 = null, CsbpBin p = null)
    {
      return new FZ250Mileages(GetBuilder("FZ250Mileages", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public FZ250Mileages(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(fahrradstaende, 1); });
      // SetBold(client0);
      InitData(0);
      fahrradstaende.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        // Get(FactoryService.PrivateService.RepairMileages(daten));
        var rl = Get(FactoryService.PrivateService.GetBikeList(daten));
        var rs = AddColumns(fahrrad, emptyentry: true);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        SetText(fahrrad, null);
        text.Text = "%%";
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.PrivateService.GetMileageList(ServiceDaten, GetText(fahrrad), text.Text))
          ?? new List<FzFahrradstand>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Fahrrad;Datum;Nr.;Zähler_r;Km_r;Schnitt_r;Beschreibung;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Fahrrad_Uid, e.BikeDescription, Functions.ToString(e.Datum),
            Functions.ToString(e.Nr), Functions.ToString(e.Zaehler_km, 0),
            Functions.ToString(e.Periode_km, 0), Functions.ToString(e.Periode_Schnitt, 2),
            e.Beschreibung, Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(fahrradstaende, FZ250_fahrradstaende_columns, values);
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
      // RefreshTreeView(fahrradstaende, 1);
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

    /// <summary>Behandlung von Fahrradstaende.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFahrradstaendeRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(fahrradstaende, 0);
    }

    /// <summary>Behandlung von Fahrrad.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFahrradChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Behandlung von Text.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnTextKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(fahrradstaende, dt != DialogTypeEnum.New);
      var date = Functions.ToDateTime(GetValue<string>(fahrradstaende, dt != DialogTypeEnum.New, 2));
      var nr = Functions.ToInt32(GetValue<string>(fahrradstaende, dt != DialogTypeEnum.New, 3));
      var key = new Tuple<string, DateTime?, int>(uid, date, nr);
      Start(typeof(FZ260Mileage), FZ260_title, dt, key, csbpparent: this);
    }

  }
}
