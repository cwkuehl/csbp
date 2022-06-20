// <copyright file="SB220Print.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for SB220Print dialog.</summary>
public partial class SB220Print : CsbpBin
{
  /// <summary>Dialog model.</summary>
#pragma warning disable CS0649

  /// <summary>Label person0.</summary>
  [Builder.Object]
  private readonly Label person0;

  /// <summary>ComboBox person.</summary>
  [Builder.Object]
  private readonly ComboBox person;

  /// <summary>Label generation0.</summary>
  [Builder.Object]
  private readonly Label generation0;

  /// <summary>Entry generation.</summary>
  [Builder.Object]
  private readonly Entry generation;

  /// <summary>CheckButton vorfahren.</summary>
  [Builder.Object]
  private readonly CheckButton vorfahren;

  /// <summary>CheckButton geschwister.</summary>
  [Builder.Object]
  private readonly CheckButton geschwister;

  /// <summary>CheckButton nachfahren.</summary>
  [Builder.Object]
  private readonly CheckButton nachfahren;

#pragma warning restore CS0649

  /// <summary>Initializes a new instance of the <see cref="SB220Print"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public SB220Print(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(person0);
    SetBold(generation0);
    InitData(0);
    person.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static SB220Print Create(object p1 = null, CsbpBin p = null)
  {
    return new SB220Print(GetBuilder("SB220Print", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      var l = Get(FactoryService.PedigreeService.GetAncestorList(daten)) ?? new List<SbPerson>();
      var fl = new List<MaParameter>();
      foreach (var p in l)
      {
        fl.Add(new MaParameter { Schluessel = p.Uid, Wert = p.AncestorName });
      }
      AddColumns(person, fl, true);
      SetText(person, Parameter.SB220Ancestor);
      SetText(generation, Parameter.SB220Generation ?? "3");
      EventsActive = true;
    }
  }

  /// <summary>Handles Person.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPersonChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    Parameter.SB220Ancestor = GetText(person);
  }

  /// <summary>Handles Generation.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnGenerationKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    if (!EventsActive || Functions.ToInt32(generation.Text) <= 0)
      return;
    Parameter.SB220Generation = generation.Text;
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    var pdf = Get(FactoryService.PedigreeService.GetAncestorReport(ServiceDaten, GetText(person), Functions.ToInt32(generation.Text),
      geschwister.Active, nachfahren.Active, vorfahren.Active));
    UiTools.SaveFile(pdf, M0(SB030));
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }
}
