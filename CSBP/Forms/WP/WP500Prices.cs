// <copyright file="WP500Prices.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for WP500Prices dialog.</summary>
public partial class WP500Prices : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView staende.</summary>
  [Builder.Object]
  private readonly TreeView staende;

  /// <summary>ComboBox wertpapier.</summary>
  [Builder.Object]
  private readonly ComboBox wertpapier;

  /// <summary>Date Von.</summary>
  //// [Builder.Object]
  private readonly Date von;

  /// <summary>Date Bis.</summary>
  //// [Builder.Object]
  private readonly Date bis;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="WP500Prices"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP500Prices(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    von = new Date(Builder.GetObject("von").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    von.DateChanged += OnVonDateChanged;
    von.Show();
    bis = new Date(Builder.GetObject("bis").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    bis.DateChanged += OnBisDateChanged;
    bis.Show();
    //// SetBold(client0);
    InitData(0);
    staende.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP500Prices Create(object p1 = null, CsbpBin p = null)
  {
    return new WP500Prices(GetBuilder("WP500Prices", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      von.Value = null;
      bis.Value = null;
      var rl = Get(FactoryService.StockService.GetStockList(daten, true)) ?? new List<WpWertpapier>();
      var rs = AddColumns(wertpapier, emptyentry: true);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      SetText(wertpapier, Parameter.WP500Stock);
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.StockService.GetPriceList(ServiceDaten, GetText(wertpapier),
        von.Value, bis.Value)) ?? new List<WpStand>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Stock;Date;Price_r;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Wertpapier_Uid, e.StockDescription, Functions.ToString(e.Datum),
          Functions.ToString(e.Stueckpreis, 4), Functions.ToString(e.Geaendert_Am, true),
          e.Geaendert_Von, Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(staende, WP500_staende_columns, values);
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
    RefreshTreeView(staende, 1);
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

  /// <summary>Handles Staende.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnStaendeRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(staende, 0);
    SetText(wertpapier, null);
  }

  /// <summary>Handles Wertpapier.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWertpapierChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    Parameter.WP500Stock = GetText(wertpapier);
    refreshAction.Click();
  }

  /// <summary>Handles von.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(staende, dt != DialogTypeEnum.New);
    var date = Functions.ToDateTime(GetValue<string>(staende, dt != DialogTypeEnum.New, 2));
    var key = new Tuple<string, DateTime?>(uid, date);
    Start(typeof(WP510Price), WP510_title, dt, key, csbpparent: this);
  }
}
