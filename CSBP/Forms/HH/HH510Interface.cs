// <copyright file="HH510Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH510Interface dialog.</summary>
public partial class HH510Interface : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label titel0.</summary>
  [Builder.Object]
  private readonly Label titel0;

  /// <summary>Entry titel.</summary>
  [Builder.Object]
  private readonly Entry titel;

  /// <summary>Label von0.</summary>
  [Builder.Object]
  private readonly Label von0;

  /// <summary>Date Von.</summary>
  //[Builder.Object]
  private readonly Date von;

  /// <summary>Label bis0.</summary>
  [Builder.Object]
  private readonly Label bis0;

  /// <summary>Date Bis.</summary>
  //[Builder.Object]
  private readonly Date bis;

  /// <summary>Label berichte0.</summary>
  [Builder.Object]
  private readonly Label berichte0;

  /// <summary>CheckButton eb.</summary>
  [Builder.Object]
  private readonly CheckButton eb;

  /// <summary>CheckButton gv.</summary>
  [Builder.Object]
  private readonly CheckButton gv;

  /// <summary>CheckButton sb.</summary>
  [Builder.Object]
  private readonly CheckButton sb;

  /// <summary>CheckButton kassenbericht.</summary>
  [Builder.Object]
  private readonly CheckButton kassenbericht;

  /// <summary>Label datei0.</summary>
  [Builder.Object]
  private readonly Label datei0;

  /// <summary>Entry datei.</summary>
  [Builder.Object]
  private readonly Entry datei;

  /// <summary>Button dateiAuswahl.</summary>
  [Builder.Object]
  private readonly Button dateiAuswahl;

  /// <summary>CheckButton loeschen.</summary>
  [Builder.Object]
  private readonly CheckButton loeschen;

  /// <summary>Button import1.</summary>
  [Builder.Object]
  private readonly Button import1;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static HH510Interface Create(object p1 = null, CsbpBin p = null)
  {
    return new HH510Interface(GetBuilder("HH510Interface", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public HH510Interface(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    von = new Date(Builder.GetObject("von").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false
    };
    von.DateChanged += OnVonDateChanged;
    von.Show();
    bis = new Date(Builder.GetObject("bis").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false
    };
    bis.DateChanged += OnBisDateChanged;
    bis.Show();
    SetBold(titel0);
    SetBold(von0);
    SetBold(bis0);
    SetBold(berichte0);
    InitData(0);
    titel.GrabFocus();
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      titel.Text = Parameter.HH510Title;
      kassenbericht.Active = Parameter.HH510Cashreport;
      datei.Text = Parameter.HH510File;
      if (Parameter1 is Tuple<string, DateTime, DateTime> p)
      {
        var kz = p.Item1;
        var v = p.Item2;
        var b = p.Item3;
        if (kz == Constants.KZBI_EROEFFNUNG)
        {
          von.Value = v;
          bis.Value = new DateTime(v.Year, 12, 31);
          eb.Active = true;
        }
        else if (kz == Constants.KZBI_SCHLUSS)
        {
          bis.Value = v;
          von.Value = b.AddDays(1 - b.DayOfYear);
          sb.Active = true;
        }
        else
        {
          von.Value = v;
          bis.Value = b;
          gv.Active = true;
        }
      }
      if (ServiceDaten.BenutzerId.ToLower() != "wolfgang")
      {
        datei0.Visible = false;
        datei.Visible = false;
        dateiAuswahl.Visible = false;
        loeschen.Visible = false;
        import1.Visible = false;
      }
    }
  }

  /// <summary>Handle von.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handle bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handle Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    Parameter.HH510Title = titel.Text;
    Parameter.HH510Cashreport = kassenbericht.Active;
    if (eb.Active || gv.Active || sb.Active)
    {
      var pdf = Get(
       FactoryService.BudgetService.GetAnnualReport(ServiceDaten, von.ValueNn, bis.ValueNn, titel.Text,
         eb.Active, gv.Active, sb.Active));
      UiTools.SaveFile(pdf, M0(HH048));
    }
    if (kassenbericht.Active)
    {
      var pdf = Get(FactoryService.BudgetService.GetCashReport(ServiceDaten, von.ValueNn, bis.ValueNn, titel.Text));
      UiTools.SaveFile(pdf, M0(HH049));
    }
  }

  /// <summary>Handle Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }

  /// <summary>Handle Dateiauswahl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateiauswahlClicked(object sender, EventArgs e)
  {
    var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? HH510_select_file : datei.Text, "*.csv", HH510_select_ext);
    if (!string.IsNullOrEmpty(file))
    {
      datei.Text = file;
      Parameter.HH510File = file;
    }
  }

  /// <summary>Handle Import1.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnImport1Clicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(datei.Text))
      throw new MessageException(M1012);
    if (!ShowYesNoQuestion(M0(HH052)))
      return;
    var lines = UiTools.ReadFile(datei.Text);
    var message = Get(FactoryService.BudgetService.ImportBookingList(ServiceDaten, lines, loeschen.Active));
    if (!string.IsNullOrEmpty(message))
    {
      Application.Invoke(delegate
      {
        UpdateParent();
      });
      ShowInfo(message);
    }
  }
}
