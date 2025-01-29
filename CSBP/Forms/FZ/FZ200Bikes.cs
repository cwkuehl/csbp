// <copyright file="FZ200Bikes.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Collections.Generic;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for FZ200Bikes dialog.</summary>
public partial class FZ200Bikes : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button UndoAction.</summary>
  [Builder.Object]
  private readonly Button undoAction;

  /// <summary>Button RedoAction.</summary>
  [Builder.Object]
  private readonly Button redoAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView fahrraeder.</summary>
  [Builder.Object]
  private readonly TreeView fahrraeder;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="FZ200Bikes"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ200Bikes(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(FZ200Bikes), dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    undoAction.EnterNotifyEvent += OnUndoRedoEnter;
    redoAction.EnterNotifyEvent += OnUndoRedoEnter;
    fahrraeder.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ200Bikes Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ200Bikes(GetBuilder("FZ200Bikes", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 1)
    {
      var l = Get(FactoryService.PrivateService.GetBikeList(ServiceDaten)) ?? new List<FzFahrrad>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Description;Type;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.Bezeichnung, e.TypBezeichnung,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(fahrraeder, FZ200_bikes_columns, values);
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
    RefreshTreeView(fahrraeder, 1);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handles Undo Redo Enter.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoRedoEnter(object sender, EnterNotifyEventArgs e)
  {
    UiTools.UpdateUndoRedoSize(undoAction, redoAction);
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

  /// <summary>Handles Fahrraeder.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFahrraederRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(fahrraeder, dt != DialogTypeEnum.New);
    Start(typeof(FZ210Bike), FZ210_title, dt, uid, csbpparent: this);
  }
}
