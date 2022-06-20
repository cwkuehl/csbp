// <copyright file="HH400Bookings.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH400Bookings dialog.</summary>
public partial class HH400Bookings : CsbpBin
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

  /// <summary>Label buchungenStatus.</summary>
  [Builder.Object]
  private readonly Label buchungenStatus;

  /// <summary>RadioButton kennzeichen1.</summary>
  [Builder.Object]
  private readonly RadioButton kennzeichen1;

  /// <summary>RadioButton kennzeichen2.</summary>
  [Builder.Object]
  private readonly RadioButton kennzeichen2;

  /// <summary>Date Von.</summary>
  //// [Builder.Object]
  private readonly Date von;

  /// <summary>Date Bis.</summary>
  //// [Builder.Object]
  private readonly Date bis;

  /// <summary>Entry bText.</summary>
  [Builder.Object]
  private readonly Entry bText;

  /// <summary>Entry betrag.</summary>
  [Builder.Object]
  private readonly Entry betrag;

  /// <summary>ComboBox konto.</summary>
  [Builder.Object]
  private readonly ComboBox konto;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private string lastreverse;

  /// <summary>Initializes a new instance of the <see cref="HH400Bookings"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH400Bookings(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
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
    ObservableEventThrottle(refreshAction, (sender, e) =>
    {
      var uid = HH410Booking.Lastcopyuid;
      HH410Booking.Lastcopyuid = null;
      RefreshTreeView(buchungen, 1, uid);
    });
    //// SetBold(client0);
    InitData(0);
    buchungen.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH400Bookings Create(object p1 = null, CsbpBin p = null)
  {
    return new HH400Bookings(GetBuilder("HH400Bookings", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      SetUserData(new[] { kennzeichen1, kennzeichen2 }, new[] { "1", "0" });
      var kl = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, null, von.Value, bis.Value));
      var rs = AddColumns(konto, emptyentry: true);
      foreach (var p in kl)
        rs.AppendValues(p.Name, p.Uid);
      SetText(kennzeichen1, "1");
      var d = ServiceDaten.Heute;
      bis.Value = d.AddDays(1 - d.Day).AddMonths(1);
      von.Value = d.AddDays(1 - d.Day).AddMonths(-14);
      SetText(bText, "%%");
      SetText(betrag, "");
      SetText(konto, "");
      //// Parameter
      if (Parameter1 is Tuple<HhKonto, DateTime, DateTime> p1)
      {
        SetText(konto, p1.Item1.Uid);
        von.Value = p1.Item2;
        bis.Value = p1.Item3;
      }
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.BudgetService.GetBookingList(ServiceDaten, GetText(kennzeichen1) == "1",
          von.Value, bis.Value, bText.Text, GetText(konto), betrag.Text)) ?? new List<HhBuchung>();
      var anz = l.Count;
      var summe = 0m;
      //// No.;Value date;A.;Value_r;Posting text;Debit account;Credit account;Receipt;Changed at;Changed by;Created at;Created by
      var values = new List<string[]>();
      foreach (var e in l)
      {
        summe += e.EBetrag;
        values.Add(new string[]
        {
          e.Uid, Functions.ToString(e.Soll_Valuta), e.Kz,
          Functions.ToString(e.EBetrag, 2), e.BText, e.DebitName, e.CreditName, e.Beleg_Nr,
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      SetText(buchungenStatus, HH054(anz, summe));
      AddStringColumnsSort(buchungen, HH400_buchungen_columns, values);
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
    // RefreshTreeView(buchungen, 1);
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

  /// <summary>Handles Save.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSaveClicked(object sender, EventArgs e)
  {
    var file = SelectFile(HH400_select_file);
    var lines = Get(FactoryService.BudgetService.ExportBookingList(ServiceDaten,
      GetText(kennzeichen1) == "1", von.Value, bis.Value, bText.Text, GetText(konto), betrag.Text));
    UiTools.SaveFile(lines, file);
  }

  /// <summary>Handles Buchungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBuchungenRowActivated(object sender, RowActivatedArgs e)
  {
    editAction.Activate();
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

  /// <summary>Handles Konto.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKontoChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Buchungstext.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBTextKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Betrag.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBetragKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Alle.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAlleClicked(object sender, EventArgs e)
  {
    RefreshTreeView(buchungen, 0);
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = GetValue<string>(buchungen, dt != DialogTypeEnum.New);
    if (dt == DialogTypeEnum.Delete)
    {
      if (lastreverse != uid)
        dt = DialogTypeEnum.Reverse;
      lastreverse = uid;
    }
    Start(typeof(HH410Booking), HH410_title, dt, uid, csbpparent: this);
  }
}
