// <copyright file="AM100Change.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM;

using System;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;

/// <summary>Controller for AM100Change dialog.</summary>
public partial class AM100Change : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry mandant.</summary>
  [Builder.Object]
  private readonly Entry mandant;

  /// <summary>Entry benutzer.</summary>
  [Builder.Object]
  private readonly Entry benutzer;

  /// <summary>Label kennwortAlt0.</summary>
  [Builder.Object]
  private readonly Label kennwortAlt0;

  /// <summary>Entry kennwortAlt.</summary>
  [Builder.Object]
  private readonly Entry kennwortAlt;

  /// <summary>Label kennwortNeu0.</summary>
  [Builder.Object]
  private readonly Label kennwortNeu0;

  /// <summary>Entry kennwortNeu.</summary>
  [Builder.Object]
  private readonly Entry kennwortNeu;

  /// <summary>Label kennwortNeu20.</summary>
  [Builder.Object]
  private readonly Label kennwortNeu20;

  /// <summary>Entry kennwortNeu2.</summary>
  [Builder.Object]
  private readonly Entry kennwortNeu2;

  /// <summary>CheckButton speichern.</summary>
  [Builder.Object]
  private readonly CheckButton speichern;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static AM100Change Create(object p1 = null, CsbpBin p = null)
  {
    return new AM100Change(GetBuilder("AM100Change", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public AM100Change(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(kennwortAlt0);
    SetBold(kennwortNeu0);
    SetBold(kennwortNeu20);
    InitData(0);
    ok.Sensitive = false;
    kennwortAlt.GrabFocus();
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      var daten = ServiceDaten;
      mandant.Text = Functions.ToString(daten.MandantNr);
      benutzer.Text = daten.BenutzerId;
      kennwortAlt.Text = "";
      kennwortNeu.Text = "";
      kennwortNeu2.Text = "";
      //// mandant.IsEditable = false;
      //// benutzer.IsEditable = false;
    }
  }

  /// <summary>Handle KennwortNeu.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKennwortNeuKeyRelease(object sender, KeyReleaseEventArgs e)
  {
    ok.Sensitive = kennwortNeu.Text == kennwortNeu2.Text;
  }

  /// <summary>Handle Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    var daten = ServiceDaten;
    var r = FactoryService.LoginService.ChangePassword(daten, daten.MandantNr, daten.BenutzerId,
        kennwortAlt.Text, kennwortNeu.Text, speichern.Active);
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        dialog.Hide();
      }
    }
  }

  /// <summary>Handle Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }
}
