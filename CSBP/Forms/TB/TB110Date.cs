// <copyright file="TB110Date.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB;

using System;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for TB110Date dialog.</summary>
public partial class TB110Date : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>Label date0.</summary>
  [Builder.Object]
  private readonly Label date0;

  /// <summary>Date date.</summary>
  //// [Builder.Object]
  private readonly Date date;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="TB110Date"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public TB110Date(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(TB110Date), dt, p1, p)
  {
    date = new Date(Builder.GetObject("datum").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = true,
      YesterdayAccel = "m",
      TomorrowAccel = "p",
      TooltipText = TB110_datum_tt,
    };
    date.Show();
    SetBold(date0);
    InitData(0);
    date.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static TB110Date Create(object p1 = null, CsbpBin p = null)
  {
    return new TB110Date(GetBuilder("TB110Date", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var aendern = DialogType == DialogTypeEnum.Edit;
      var p = Parameter1 as Tuple<string, DateTime>;
      if (!neu && p.Item1 != null)
      {
        var k = Get(FactoryService.DiaryService.GetPosition(daten, p.Item1));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        SetText(nr, k.Uid);
        SetText(bezeichnung, k.Bezeichnung);
        date.Value = p.Item2;
      }
      nr.IsEditable = false;
      bezeichnung.Sensitive = !loeschen;
      date.Sensitive = !loeschen;
      if (loeschen)
        ok.Label = Forms_delete;
      EventsActive = true;
    }
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    var daten = ServiceDaten;
    var p = Get(FactoryService.DiaryService.GetPosition(daten, nr.Text));
    if (p != null && p.Bezeichnung != bezeichnung.Text)
    {
      var r = FactoryService.DiaryService.SavePosition(daten, p.Uid, bezeichnung.Text, Functions.ToString(p.Breite, 5), Functions.ToString(p.Laenge, 5), Functions.ToString(p.Hoehe, 2), p.Zeitzone, p.Notiz);
      Get(r);
      if (!r.Ok)
        return;
    }
    var d = date.ValueNn;
    Response = d;
    CloseDialog();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }
}
