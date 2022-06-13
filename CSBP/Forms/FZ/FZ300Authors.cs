// <copyright file="FZ300Authors.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for FZ300Authors dialog.</summary>
public partial class FZ300Authors : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>TreeView autoren.</summary>
  [Builder.Object]
  private readonly TreeView autoren;

  /// <summary>Entry name.</summary>
  [Builder.Object]
  private readonly Entry name;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static FZ300Authors Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ300Authors(GetBuilder("FZ300Authors", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public FZ300Authors(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, delegate { RefreshTreeView(autoren, 1); });
    // SetBold(client0);
    InitData(0);
    autoren.GrabFocus();
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      name.Text = "%%";
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.PrivateService.GetAuthorList(ServiceDaten, name.Text))
        ?? new List<FzBuchautor>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // Nr.;Name;Vorname;Geändert am;Geändert von;Angelegt am;Angelegt von
        values.Add(new string[] { e.Uid, e.Name, e.Vorname,
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
      }
      AddStringColumnsSort(autoren, FZ300_autoren_columns, values);
    }
  }

  /// <summary>Updates parent dialog.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handle Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // RefreshTreeView(autoren, 1);
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

  /// <summary>Handle Autoren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAutorenRowActivated(object sender, RowActivatedArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handle Name.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNameKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Handle Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(autoren, 0);
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(autoren, dt != DialogTypeEnum.New);
    Start(typeof(FZ310Author), FZ310_title, dt, uid, csbpparent: this);
  }
}
