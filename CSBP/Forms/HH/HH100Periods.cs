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

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static HH100Periods Create(object p1 = null, CsbpBin p = null)
  {
    return new HH100Periods(GetBuilder("HH100Periods", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public HH100Periods(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    perioden.GrabFocus();
  }

  /// <summary>Model-Daten initialisieren.</summary>
  /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
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
        // Nr.;Nr.;Zeitraum;Von;Bis;Geändert am;Geändert von;Angelegt am;Angelegt von
        values.Add(new string[] { Functions.ToString(e.Nr), Functions.ToString(e.Nr), e.Period,
            Functions.ToString(e.Datum_Von), Functions.ToString(e.Datum_Bis),
            Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
            Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
      }
      if (l.Count > 0)
      {
        anfang.Text = Functions.ToString(l.Last().Datum_Von);
        ende.Text = Functions.ToString(l.First().Datum_Bis);
      }
      AddStringColumnsSort(perioden, HH100_perioden_columns, values);
    }
  }

  /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
  protected override void UpdateParent()
  {
    refreshAction.Click();
  }

  /// <summary>Handle Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    RefreshTreeView(perioden, 1);
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
    if (Get(FactoryService.BudgetService.SavePeriod(ServiceDaten, Functions.ToInt32(GetText(laenge1)),
      Functions.ToInt32(GetText(art1)) != 0)))
      refreshAction.Click();
  }

  /// <summary>Handle Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    if (Get(FactoryService.BudgetService.DeletePeriod(ServiceDaten, Functions.ToInt32(GetText(art1)) != 0)))
      refreshAction.Click();
  }

  /// <summary>Handle Perioden.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPeriodenRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handle Länge.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnLengthChanged(object sender, EventArgs e)
  {
    if (EventsActive)
      Parameter.HH100Length = GetText(laenge1);
  }

  /// <summary>Handle Art.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWhenChanged(object sender, EventArgs e)
  {
    if (EventsActive)
      Parameter.HH100When = GetText(art1);
  }
}
