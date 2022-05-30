// <copyright file="HH300Events.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für HH300Events Dialog.</summary>
  public partial class HH300Events : CsbpBin
  {
#pragma warning disable CS0649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private readonly Button refreshAction;

    /// <summary>Button EditAction.</summary>
    [Builder.Object]
    private readonly Button editAction;

    /// <summary>TreeView ereignisse.</summary>
    [Builder.Object]
    private readonly TreeView ereignisse;

    /// <summary>Entry text.</summary>
    [Builder.Object]
    private readonly Entry text;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static HH300Events Create(object p1 = null, CsbpBin p = null)
    {
      return new HH300Events(GetBuilder("HH300Events", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public HH300Events(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(ereignisse, 1); });
      // SetBold(client0);
      InitData(0);
      ereignisse.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        EventsActive = false;
        text.Text = "%%";
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.BudgetService.GetEventList(ServiceDaten, text.Text)) ?? new List<HhEreignis>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Bezeichnung;Buchungstext;Sollkonto;Habenkonto;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Bezeichnung, e.EText, e.DebitName,
            e.CreditName, Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(ereignisse, HH300_ereignisse_columns, values);
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
      // RefreshTreeView(ereignisse, 1);
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

    /// <summary>Behandlung von Ereignisse.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnEreignisseRowActivated(object sender, RowActivatedArgs e)
    {
      editAction.Activate();
    }

    /// <summary>Behandlung von Alle.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAlleClicked(object sender, EventArgs e)
    {
      RefreshTreeView(ereignisse, 0);
    }

    /// <summary>Behandlung von Text.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnTextKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive)
        return;
      refreshAction.Click();
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(ereignisse, dt != DialogTypeEnum.New);
      Start(typeof(HH310Event), HH310_title, dt, uid, csbpparent: this);
    }

  }
}
