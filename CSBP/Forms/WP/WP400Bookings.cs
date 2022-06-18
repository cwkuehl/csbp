// <copyright file="WP400Bookings.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for WP400Bookings dialog.</summary>
public partial class WP400Bookings : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Button EditAction.</summary>
  [Builder.Object]
  private readonly Button editAction;

  /// <summary>TreeView buchungen.</summary>
  [Builder.Object]
  private readonly TreeView buchungen;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>ComboBox anlage.</summary>
  [Builder.Object]
  private readonly ComboBox anlage;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="WP400Bookings"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP400Bookings(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    buchungen.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP400Bookings Create(object p1 = null, CsbpBin p = null)
  {
    return new WP400Bookings(GetBuilder("WP400Bookings", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      bezeichnung.Text = "%%";
      var rl = Get(FactoryService.StockService.GetInvestmentList(daten, true)) ?? new List<WpAnlage>();
      var rs = AddColumns(anlage, emptyentry: true);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      var p1 = Parameter1 as string;
      var invuid = string.IsNullOrEmpty(p1) ? Parameter.WP400Investment : p1;
      SetText(anlage, invuid);
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.StockService.GetBookingList(daten, bezeichnung.Text,
          null, null, GetText(anlage))) ?? new List<WpBuchung>();
      var values = new List<string[]>();
      foreach (var e in l)
      {
        // No.;Stock;Description;Date;Posting text;Payment_r;Discount_r;Shares_r;Interest_r;Changed at;Changed by;Created at;Created by
        values.Add(new string[]
        {
          e.Uid, e.StockDescription, e.InvestmentDescription, Functions.ToString(e.Datum),
          e.BText, Functions.ToString(e.Zahlungsbetrag, 2), Functions.ToString(e.Rabattbetrag, 2),
          Functions.ToString(e.Anteile, 5), Functions.ToString(e.Zinsen, 2),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      AddStringColumnsSort(buchungen, WP400_buchungen_columns, values);
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
    var uid = WP410Booking.Lastcopyuid;
    WP410Booking.Lastcopyuid = null;
    RefreshTreeView(buchungen, 1, uid);
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

  /// <summary>Handles Buchungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBuchungenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
  }

  /// <summary>Handles Bezeichnung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBezeichnungKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Anlage.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAnlageChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    Parameter.WP400Investment = GetText(anlage);
    refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    SetText(anlage, null);
    RefreshTreeView(buchungen, 0);
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(buchungen, dt != DialogTypeEnum.New);
    if (dt == DialogTypeEnum.New)
      uid = GetText(anlage);
    Start(typeof(WP410Booking), WP410_title, dt, uid, csbpparent: this);
  }
}
