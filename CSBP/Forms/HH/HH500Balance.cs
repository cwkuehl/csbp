// <copyright file="HH500Balance.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH500Balance dialog.</summary>
public partial class HH500Balance : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label soll0.</summary>
  [Builder.Object]
  private readonly Label soll0;

  /// <summary>TreeView soll.</summary>
  [Builder.Object]
  private readonly TreeView soll;

  /// <summary>Label haben0.</summary>
  [Builder.Object]
  private readonly Label haben0;

  /// <summary>TreeView haben.</summary>
  [Builder.Object]
  private readonly TreeView haben;

  /// <summary>Label sollBetrag0.</summary>
  [Builder.Object]
  private readonly Label sollBetrag0;

  /// <summary>Label habenBetrag0.</summary>
  [Builder.Object]
  private readonly Label habenBetrag0;

  /// <summary>Label von0.</summary>
  [Builder.Object]
  private readonly Label von0;

  /// <summary>Date Von.</summary>
  //// [Builder.Object]
  private readonly Date von;

  /// <summary>Label bis0.</summary>
  [Builder.Object]
  private readonly Label bis0;

  /// <summary>Date Bis.</summary>
  //// [Builder.Object]
  private readonly Date bis;

#pragma warning restore CS0649

  /// <summary>State for task.</summary>
  private readonly StringBuilder state = new();

  /// <summary>Cancel for task.</summary>
  private readonly StringBuilder cancel = new();

  /// <summary>Counter for not calculating.</summary>
  private int calculate = 0;

  /// <summary>Left or right side.</summary>
  private bool sollTabelle;

  /// <summary>List of left account side.</summary>
  private List<HhBilanz> sollListe;

  /// <summary>List of right account side.</summary>
  private List<HhBilanz> habenListe;

  /// <summary>Initializes a new instance of the <see cref="HH500Balance"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH500Balance(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(HH500Balance), dt, p1, p)
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
    soll.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH500Balance Create(object p1 = null, CsbpBin p = null)
  {
    return new HH500Balance(GetBuilder("HH500Balance", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    var parameter = Parameter1 as string;
    if (step <= 0)
    {
      EventsActive = false;
      var max = Get(FactoryService.BudgetService.GetMinMaxPeriod(daten));
      var dB = daten.Heute;
      dB = new DateTime(dB.Year, dB.Month, DateTime.DaysInMonth(dB.Year, dB.Month));
      if (max != null && max.Datum_Bis < dB)
        dB = max.Datum_Bis;
      var dV = new DateTime(dB.Year, 1, 1);
      if (max != null && max.Datum_Von > dV)
        dV = max.Datum_Von;
      if (parameter == Constants.KZBI_GV)
      {
        von.Value = dV;
        bis.Value = dB;
      }
      else if (parameter == Constants.KZBI_EROEFFNUNG)
      {
        von.Value = dV;
        bis.Value = dV;
      }
      else
      {
        von.Value = dB;
        bis.Value = dB;
      }
      if (parameter == Constants.KZBI_GV)
      {
        SetText(soll0, HH500_soll_GV);
        SetText(haben0, HH500_haben_GV);
      }
      else
      {
        SetText(soll0, HH500_soll_EB);
        SetText(haben0, HH500_haben_EB);
        SetText(von0, HH500_von_EB);
        bis0.Visible = false;
        bis0.NoShowAll = true;
        bis.Visible = false;
        bis.NoShowAll = true;
      }
      soll0.UseUnderline = true;
      haben0.UseUnderline = true;
      von0.UseUnderline = true;
      EventsActive = true;
    }
    if (step <= 1)
    {
      DateTime v = von.ValueNn;
      DateTime b = parameter == Constants.KZBI_GV ? bis.ValueNn : von.ValueNn;
      var l = Get(FactoryService.BudgetService.GetBalanceList(daten, parameter, v, b)) ?? new List<HhBilanz>();
      sollListe = l.Where(a => a.AccountType > 0).ToList();
      habenListe = l.Where(a => a.AccountType <= 0).ToList();
      //// No.;Description;Value_r;Changed at;Changed by;Created at;Created by
      var svalues = new List<string[]>();
      var hvalues = new List<string[]>();
      foreach (var e in sollListe)
      {
        svalues.Add(new string[]
        {
          e.Konto_Uid, e.AccountName, Functions.ToString(e.AccountEsum, 2),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      foreach (var e in habenListe)
      {
        hvalues.Add(new string[]
        {
          e.Konto_Uid, e.AccountName, Functions.ToString(e.AccountEsum, 2),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
        });
      }
      SetText(sollBetrag0, Functions.ToString(sollListe.Sum(a => a.AccountEsum), 2));
      SetText(habenBetrag0, Functions.ToString(habenListe.Sum(a => a.AccountEsum), 2));
      AddStringColumnsSort(soll, HH500_soll_columns, svalues);
      AddStringColumnsSort(haben, HH500_haben_columns, hvalues);
    }
  }

  /// <summary>Handles Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    // Calculates balances.
    ShowStatus(state, cancel);
    Task.Run(() =>
    {
      var keine = true;
      var weiter = 2;
      DateTime? v = null;
      ServiceErgebnis<string[]> r = null;
      try
      {
        if (calculate > 0)
        {
          v = von.ValueNn;
          calculate = 0;
        }
        while (weiter >= 2 && (r == null || r.Ok))
        {
          r = FactoryService.BudgetService.CalculateBalances(ServiceDaten, state, cancel, true, v);
          v = null;
          if (r.Ok)
          {
            var l = r.Ergebnis;
            if (l != null && l.Length >= 1)
            {
              weiter = Functions.ToInt32(l[0]);
              if (weiter > 0)
                keine = false;
            }
          }
          else
            Application.Invoke((sender1, e1) => { Get(r); });
        }
      }
      catch (Exception ex)
      {
        Functions.MachNichts(ex);
      }
      finally
      {
        if (keine)
          calculate++;
        else
          Application.Invoke((sender1, e1) => { OnRefresh(); });
      }
      return 0;
    });
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      OnRefresh();
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      OnRefresh();
  }

  /// <summary>Handles Print.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPrintClicked(object sender, EventArgs e)
  {
    var d = new Tuple<string, DateTime, DateTime>(Parameter1?.ToString(), von.ValueNn, bis.ValueNn);
    Start(typeof(HH510Interface), HH510_title, DialogTypeEnum.Without, d, csbpparent: this);
  }

  /// <summary>Handles Soll.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSollCursorChanged(object sender, EventArgs e)
  {
    if (EventsActive)
      sollTabelle = true;
  }

  /// <summary>Handles Soll.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSollRowActivated(object sender, RowActivatedArgs e)
  {
    var uid = GetValue<string>(soll, true);
    StartBookings(uid);
  }

  /// <summary>Handles Haben.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHabenCursorChanged(object sender, EventArgs e)
  {
    if (EventsActive)
      sollTabelle = false;
  }

  /// <summary>Handles Haben.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHabenRowActivated(object sender, RowActivatedArgs e)
  {
    var uid = GetValue<string>(haben, true);
    StartBookings(uid);
  }

  /// <summary>Handles von.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
  {
    OnRefresh();
  }

  /// <summary>Handles bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
    if (bis.ValueNn < von.ValueNn)
      bis.Value = von.ValueNn;
    OnRefresh();
  }

  /// <summary>Handles Oben.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnObenClicked(object sender, EventArgs e)
  {
    SwapSortings(true);
  }

  /// <summary>Handles Unten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUntenClicked(object sender, EventArgs e)
  {
    SwapSortings(false);
  }

  /// <summary>Handles Refresh.</summary>
  private void OnRefresh()
  {
    if (!EventsActive)
      return;
    try
    {
      EventsActive = false;
      var s = haben.Selection.GetSelectedRows();
      RefreshTreeView(soll, 1);
      //// RefreshTreeView(haben, 1);
      foreach (var p in s)
        haben.Selection.SelectPath(p);
    }
    finally
    {
      EventsActive = true;
    }
  }

  /// <summary>Starts bookings dialog.</summary>
  /// <param name="uid">Affected account id.</param>
  private void StartBookings(string uid)
  {
    var a = Get(FactoryService.BudgetService.GetAccount(ServiceDaten, uid));
    if (a != null)
    {
      var v = von.ValueNn;
      var b = bis.ValueNn;
      if (Parameter1?.ToString() == Constants.KZBI_EROEFFNUNG)
        b = v.AddYears(1).AddDays(-1);
      else if (Parameter1?.ToString() == Constants.KZBI_SCHLUSS)
      {
        b = v;
        v = b.AddDays(1 - b.DayOfYear);
      }
      var p1 = new Tuple<HhKonto, DateTime, DateTime>(a, v, b);
      MainClass.MainWindow.AppendPage(HH400Bookings.Create(p1), a.Name);
    }
  }

  /// <summary>Swap accouts sorting.</summary>
  /// <param name="up">Move up or down.</param>
  private void SwapSortings(bool up)
  {
    var uid = sollTabelle ? GetValue<string>(soll, false) : GetValue<string>(haben, false);
    if (string.IsNullOrEmpty(uid))
      return;
    var l = sollTabelle ? sollListe : habenListe;
    var acc = l.FirstOrDefault(a => a.Konto_Uid == uid);
    if (acc == null)
      return;
    var i = l.IndexOf(acc);
    var d = up ? -1 : 1;
    if (i >= 0 && i < l.Count && i + d >= 0 && i + d < l.Count)
    {
      var uid2 = l[i + d].Konto_Uid;
      var r = FactoryService.BudgetService.SwapAccountSort(ServiceDaten, uid, uid2);
      Get(r);
      if (r.Ok)
      {
        OnRefresh();
        if (sollTabelle)
          SetText(soll, uid);
        else
          SetText(haben, uid);
      }
    }
  }
}
