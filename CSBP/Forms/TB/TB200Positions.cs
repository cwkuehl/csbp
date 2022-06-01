// <copyright file="TB200Positions.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für TB200Positions Dialog.</summary>
  public partial class TB200Positions : CsbpBin
  {
#pragma warning disable CS0649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private readonly Button refreshAction;

    /// <summary>TreeView positions.</summary>
    [Builder.Object]
    private readonly TreeView positions;

    /// <summary>Entry search.</summary>
    [Builder.Object]
    private readonly Entry search;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static TB200Positions Create(object p1 = null, CsbpBin p = null)
    {
      return new TB200Positions(GetBuilder("TB200Positions", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public TB200Positions(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(positions, 1); });
      // SetBold(client0);
      InitData(0);
      positions.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        EventsActive = false;
        search.Text = "%%";
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.DiaryService.GetPositionList(ServiceDaten, null, search.Text))
          ?? new List<TbOrt>();
        var values = new List<string[]>();
        foreach (var e in l)
        {
          // Nr.;Bezeichnung;Breite;Länge;Geändert am;Geändert von;Angelegt am;Angelegt von
          values.Add(new string[] { e.Uid, e.Bezeichnung, Functions.ToString(e.Breite, 5), Functions.ToString(e.Laenge, 5),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
        }
        AddStringColumnsSort(positions, TB200_positions_columns, values);
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
      // RefreshTreeView(positions, 1);
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

    /// <summary>Handle Map.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnChartClicked(object sender, EventArgs e)
    {
      var uid = GetValue<string>(positions);
      var p = Get(FactoryService.DiaryService.GetPosition(ServiceDaten, uid));
      if (p != null)
      {
        UiTools.StartFile($"https://www.openstreetmap.org/#map=19/{Functions.ToString(p.Breite, 5, Functions.CultureInfoEn)}/{Functions.ToString(p.Laenge, 5, Functions.CultureInfoEn)}");
      }
    }

    /// <summary>Behandlung von Positions.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPositionsRowActivated(object sender, RowActivatedArgs e)
    {
      StartDialog(DialogTypeEnum.Edit);
    }

    /// <summary>Behandlung von Search.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSearchKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
    {
      refreshAction.Click();
    }

    /// <summary>Behandlung von All.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAllClicked(object sender, EventArgs e)
    {
      RefreshTreeView(positions, 0);
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = GetValue<string>(positions, dt != DialogTypeEnum.New);
      Start(typeof(TB210Position), TB210_title, dt, uid, csbpparent: this);
    }
  }
}
