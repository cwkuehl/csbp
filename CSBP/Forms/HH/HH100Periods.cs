// <copyright file="HH100Periods.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH100Periods dialog.</summary>
public partial class HH100Periods : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>TreeView perioden.</summary>
  [Builder.Object]
  private readonly TreeView perioden;

  /// <summary>Entry anfang.</summary>
  [Builder.Object]
  private readonly Entry anfang;

  /// <summary>Entry ende.</summary>
  [Builder.Object]
  private readonly Entry ende;

  /// <summary>RadioButton laenge1.</summary>
  [Builder.Object]
  private readonly RadioButton laenge1;

  /// <summary>RadioButton laenge2.</summary>
  [Builder.Object]
  private readonly RadioButton laenge2;

  /// <summary>RadioButton laenge3.</summary>
  [Builder.Object]
  private readonly RadioButton laenge3;

  /// <summary>RadioButton laenge4.</summary>
  [Builder.Object]
  private readonly RadioButton laenge4;

  /// <summary>RadioButton art1.</summary>
  [Builder.Object]
  private readonly RadioButton art1;

  /// <summary>RadioButton art2.</summary>
  [Builder.Object]
  private readonly RadioButton art2;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="HH100Periods"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH100Periods(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    perioden.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH100Periods Create(object p1 = null, CsbpBin p = null)
  {
    return new HH100Periods(GetBuilder("HH100Periods", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      SetUserData(new[] { laenge1, laenge2, laenge3, laenge4 }, new[] { "1", "3", "6", "12" });
      SetUserData(new[] { art1, art2 }, new[] { "0", "1" });
      anfang.IsEditable = false;
      ende.IsEditable = false;
      SetText(laenge1, Parameter.HH100Length);
      SetText(art2, Parameter.HH100When);
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.BudgetService.GetPeriodList(ServiceDaten)) ?? new List<HhPeriode>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;No._r;Time span;From;To;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          Functions.ToString(e.Nr), Functions.ToString(e.Nr), e.Period,
          Functions.ToString(e.Datum_Von), Functions.ToString(e.Datum_Bis),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      if (l.Count > 0)
      {
        anfang.Text = Functions.ToString(l.Last().Datum_Von);
        ende.Text = Functions.ToString(l.First().Datum_Bis);
      }
      AddStringColumnsSort(perioden, HH100_perioden_columns, values);
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
    RefreshTreeView(perioden, 1);
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
    if (Get(FactoryService.BudgetService.SavePeriod(ServiceDaten, Functions.ToInt32(GetText(laenge1)),
      Functions.ToInt32(GetText(art1)) != 0)))
      refreshAction.Click();
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    if (Get(FactoryService.BudgetService.DeletePeriod(ServiceDaten, Functions.ToInt32(GetText(art1)) != 0)))
      refreshAction.Click();
  }

  /// <summary>Handles Perioden.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPeriodenRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles length.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnLengthChanged(object sender, EventArgs e)
  {
    if (EventsActive)
      Parameter.HH100Length = GetText(laenge1);
  }

  /// <summary>Handles when.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWhenChanged(object sender, EventArgs e)
  {
    if (EventsActive)
      Parameter.HH100When = GetText(art1);
  }
}
