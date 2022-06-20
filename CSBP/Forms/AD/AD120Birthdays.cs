// <copyright file="AD120Birthdays.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;

/// <summary>Controller for AD120Birthdays dialog.</summary>
public partial class AD120Birthdays : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label datum0.</summary>
  [Builder.Object]
  private readonly Label datum0;

  /// <summary>Date Datum.</summary>
  private readonly Date datum;

  /// <summary>Label tage0.</summary>
  [Builder.Object]
  private readonly Label tage0;

  /// <summary>Entry tage.</summary>
  [Builder.Object]
  private readonly Entry tage;

  /// <summary>TextView geburtstage.</summary>
  [Builder.Object]
  private readonly TextView geburtstage;

  /// <summary>CheckButton starten.</summary>
  [Builder.Object]
  private readonly CheckButton starten;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="AD120Birthdays"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AD120Birthdays(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    datum = new Date(Builder.GetObject("datum").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    datum.DateChanged += OnDatumDateChanged;
    datum.Show();
    SetBold(datum0);
    SetBold(tage0);
    InitData(0);
    ok.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AD120Birthdays Create(object p1 = null, CsbpBin p = null)
  {
    return new AD120Birthdays(GetBuilder("AD120Birthdays", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      datum.Value = daten.Heute;
      SetText(tage, Functions.ToString(Parameter.AD120Days));
      starten.Active = Parameter.AD120Start;
      EventsActive = true;
    }
    if (step <= 1)
    {
      var l = Get(FactoryService.AddressService.GetBirthdayList(daten, datum.ValueNn,
          Functions.ToInt32(tage.Text))) ?? new List<string>();
      if (l.Count <= 1)
      {
        Application.Invoke((sender, e) => { dialog.Hide(); });
        return;
      }
      SetText(geburtstage, string.Join("\n", l.ToArray()));
    }
  }

  /// <summary>Handles Datum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
  {
    if (EventsActive)
      InitData(1);
  }

  /// <summary>Handles Tage.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTageKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (EventsActive)
      InitData(1);
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    Parameter.AD120Days = Functions.ToInt32(tage.Text);
    Parameter.AD120Start = starten.Active;
    dialog.Hide();
  }
}
