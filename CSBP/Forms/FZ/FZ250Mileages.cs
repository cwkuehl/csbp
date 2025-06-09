// <copyright file="FZ250Mileages.cs" company="cwkuehl.de">
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

/// <summary>Controller for FZ250Mileages dialog.</summary>
public partial class FZ250Mileages : CsbpBin
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

  /// <summary>Initializes a new instance of the <see cref="FZ250Mileages"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ250Mileages(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(FZ250Mileages), dt, p1, p)
  {
    ObservableEventThrottle(refreshAction, (sender, e) => { RefreshTreeView(fahrradstaende, 1); });
    //// SetBold(client0);
    InitData(0);
    undoAction.EnterNotifyEvent += OnUndoRedoEnter;
    redoAction.EnterNotifyEvent += OnUndoRedoEnter;
    fahrradstaende.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ250Mileages Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ250Mileages(GetBuilder("FZ250Mileages", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
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
      SetText(text, "%%");
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.PrivateService.GetMileageList(ServiceDaten, null, GetText(fahrrad), text.Text))
        ?? new List<FzFahrradstand>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Bike;Date;No.;Odometer_r;Km_r;Average_r;Description;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Fahrrad_Uid, e.BikeDescription, Functions.ToString(e.Datum),
          Functions.ToString(e.Nr), Functions.ToString(e.Zaehler_km, 0),
          Functions.ToString(e.Periode_km, 0), Functions.ToString(e.Periode_Schnitt, 2),
          e.Beschreibung, Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(fahrradstaende, FZ250_fahrradstaende_columns, values);
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
    // RefreshTreeView(fahrradstaende, 1);
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

  /// <summary>Handles Fahrradstaende.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFahrradstaendeRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(fahrradstaende, 0);
    text.GrabFocus();
  }

  /// <summary>Handles Fahrrad.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFahrradChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Text.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTextKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    refreshAction.Click();
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(fahrradstaende, dt != DialogTypeEnum.New);
    var date = Functions.ToDateTime(GetValue<string>(fahrradstaende, dt != DialogTypeEnum.New, 2));
    var nr = Functions.ToInt32(GetValue<string>(fahrradstaende, dt != DialogTypeEnum.New, 3));
    var key = new Tuple<string, DateTime?, int>(uid, date, nr);
    Start(typeof(FZ260Mileage), FZ260_title, dt, key, csbpparent: this);
  }
}
