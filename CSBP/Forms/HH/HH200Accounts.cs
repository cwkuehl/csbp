// <copyright file="HH200Accounts.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for HH200Accounts dialog.</summary>
public partial class HH200Accounts : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView konten.</summary>
  [Builder.Object]
  private readonly TreeView konten;

  /// <summary>Entry text.</summary>
  [Builder.Object]
  private readonly Entry text;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="HH200Accounts"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH200Accounts(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(HH200Accounts), dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(konten, 1); });
    //// SetBold(client0);
    InitData(0);
    konten.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH200Accounts Create(object p1 = null, CsbpBin p = null)
  {
    return new HH200Accounts(GetBuilder("HH200Accounts", out var handle), handle, p1: p1, p: p);
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
      var l = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, text.Text)) ?? new List<HhKonto>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Type;Attr.;Description;Valid from;Valid to;Value_r;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Art, e.Kz, e.Name,
          Functions.ToString(e.Gueltig_Von), Functions.ToString(e.Gueltig_Bis), Functions.ToString(e.EBetrag, 2),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(konten, HH200_konten_columns, values);
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
    // RefreshTreeView(konten, 1);
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

  /// <summary>Handles Konten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKontenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(konten, 0);
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
    var uid = GetValue<string>(konten, dt != DialogTypeEnum.New);
    Start(typeof(HH210Account), HH210_title, dt, uid, csbpparent: this);
  }
}
