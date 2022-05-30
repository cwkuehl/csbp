// <copyright file="SB220Print.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB
{
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

  /// <summary>Controller für SB220Print Dialog.</summary>
  public partial class SB220Print : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
#pragma warning disable CS0649

    /// <summary>Label person0.</summary>
    [Builder.Object]
    private Label person0;

    /// <summary>ComboBox person.</summary>
    [Builder.Object]
    private ComboBox person;

    /// <summary>Label generation0.</summary>
    [Builder.Object]
    private Label generation0;

    /// <summary>Entry generation.</summary>
    [Builder.Object]
    private Entry generation;

    /// <summary>CheckButton vorfahren.</summary>
    [Builder.Object]
    private CheckButton vorfahren;

    /// <summary>CheckButton geschwister.</summary>
    [Builder.Object]
    private CheckButton geschwister;

    /// <summary>CheckButton nachfahren.</summary>
    [Builder.Object]
    private CheckButton nachfahren;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static SB220Print Create(object p1 = null, CsbpBin p = null)
    {
      return new SB220Print(GetBuilder("SB220Print", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public SB220Print(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(person0);
      SetBold(generation0);
      InitData(0);
      person.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
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
        generation.Text = Parameter.SB220Generation ?? "3";
        EventsActive = true;
      }
    }

    /// <summary>Behandlung von Person.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPersonChanged(object sender, EventArgs e)
    {
      if (!EventsActive)
        return;
      Parameter.SB220Ancestor = GetText(person);
    }

    /// <summary>Behandlung von Generation.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnGenerationKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (!EventsActive || Functions.ToInt32(generation.Text) <= 0)
        return;
      Parameter.SB220Generation = generation.Text;
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      var pdf = Get(FactoryService.PedigreeService.GetAncestorReport(ServiceDaten, GetText(person), Functions.ToInt32(generation.Text),
        geschwister.Active, nachfahren.Active, vorfahren.Active));
      UiTools.SaveFile(pdf, M0(SB030));
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }
  }
}
