// <copyright file="HH300Events.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH300Events dialog.</summary>
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

  /// <summary>Initializes a new instance of the <see cref="HH300Events"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH300Events(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(HH300Events), dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(ereignisse, 1); });
    //// SetBold(client0);
    InitData(0);
    ereignisse.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH300Events Create(object p1 = null, CsbpBin p = null)
  {
    return new HH300Events(GetBuilder("HH300Events", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      SetText(text, "%%");
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.BudgetService.GetEventList(ServiceDaten, text.Text)) ?? new List<HhEreignis>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Description;Posting text;Debit account;Credit account;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Bezeichnung, e.EText, e.DebitName,
          e.CreditName, Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(ereignisse, HH300_ereignisse_columns, values);
    }
  }

  /// <summary>Updates parent dialog.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handles Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // RefreshTreeView(ereignisse, 1);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      refreshAction.Click();
  }

  /// <summary>Handles New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.New);
  }

  /// <summary>Handles Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Copy);
  }

  /// <summary>Handles Edit.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEditClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Delete);
  }

  /// <summary>Handles Ereignisse.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEreignisseRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(ereignisse, 0);
    text.GrabFocus();
  }

  /// <summary>Handles Text.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTextKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(ereignisse, dt != DialogTypeEnum.New);
    Start(typeof(HH310Event), HH310_title, dt, uid, csbpparent: this);
  }
}
