// <copyright file="HH510Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

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
  //// [Builder.Object]
  private readonly Date von;

  /// <summary>Label bis0.</summary>
  [Builder.Object]
  private readonly Label bis0;

  /// <summary>Date Bis.</summary>
  //// [Builder.Object]
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

  /// <summary>Initializes a new instance of the <see cref="HH510Interface"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH510Interface(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(HH510Interface), dt, p1, p)
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
    SetBold(titel0);
    SetBold(von0);
    SetBold(bis0);
    SetBold(berichte0);
    InitData(0);
    titel.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH510Interface Create(object p1 = null, CsbpBin p = null)
  {
    return new HH510Interface(GetBuilder("HH510Interface", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      SetText(titel, ParameterGui.HH510Title);
      kassenbericht.Active = ParameterGui.HH510Cashreport;
      SetText(datei, ParameterGui.HH510File);
      if (Parameter1 is Tuple<string, DateTime, DateTime> p)
      {
        var kz = p.Item1;
        var v = p.Item2;
        var b = p.Item3;
        if (kz == Constants.KZBI_EROEFFNUNG)
        {
          von.Value = v;
          bis.Value = new DateTime(v.Year, 12, 31);
          eb.Active = !kassenbericht.Active;
        }
        else if (kz == Constants.KZBI_SCHLUSS)
        {
          bis.Value = v;
          von.Value = b <= v ? b.AddDays(1 - b.DayOfYear) : new DateTime(v.Year, 1, 1);
          sb.Active = !kassenbericht.Active;
        }
        else
        {
          von.Value = v;
          bis.Value = b;
          gv.Active = !kassenbericht.Active;
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

  /// <summary>Handles von.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    ParameterGui.HH510Title = titel.Text;
    ParameterGui.HH510Cashreport = kassenbericht.Active;
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

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }

  /// <summary>Handles Dateiauswahl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateiauswahlClicked(object sender, EventArgs e)
  {
    var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? HH510_select_file : datei.Text, "*.csv", HH510_select_ext);
    if (!string.IsNullOrEmpty(file))
    {
      SetText(datei, file);
      ParameterGui.HH510File = file;
    }
  }

  /// <summary>Handles Import1.</summary>
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
      Application.Invoke((sender1, e1) => { UpdateParent(); });
      ShowInfo(message);
    }
  }
}
