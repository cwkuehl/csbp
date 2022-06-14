// <copyright file="SB410Source.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SB;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for SB410Source dialog.</summary>
public partial class SB410Source : CsbpBin
{
  /// <summary>Dialog model.</summary>
  private SbQuelle Model;

#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label autor0.</summary>
  [Builder.Object]
  private readonly Label autor0;

  /// <summary>Entry autor.</summary>
  [Builder.Object]
  private readonly Entry autor;

  /// <summary>Label beschreibung0.</summary>
  [Builder.Object]
  private readonly Label beschreibung0;

  /// <summary>Entry beschreibung.</summary>
  [Builder.Object]
  private readonly Entry beschreibung;

  /// <summary>TextView zitat.</summary>
  [Builder.Object]
  private readonly TextView zitat;

  /// <summary>TextView bemerkung.</summary>
  [Builder.Object]
  private readonly TextView bemerkung;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static SB410Source Create(object p1 = null, CsbpBin p = null)
  {
    return new SB410Source(GetBuilder("SB410Source", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public SB410Source(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(autor0);
    SetBold(beschreibung0);
    InitData(0);
    autor.GrabFocus();
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
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.PedigreeService.GetSource(daten, uid));
        if (k == null)
        {
          Application.Invoke(delegate
          {
            dialog.Hide();
          });
          return;
        }
        Model = k;
        nr.Text = k.Uid ?? "";
        autor.Buffer.Text = k.Autor ?? "";
        beschreibung.Buffer.Text = k.Beschreibung ?? "";
        zitat.Buffer.Text = k.Zitat ?? "";
        bemerkung.Buffer.Text = k.Bemerkung ?? "";
        angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
        geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
      }
      nr.IsEditable = false;
      autor.Sensitive = !loeschen;
      beschreibung.Sensitive = !loeschen;
      zitat.Sensitive = !loeschen;
      bemerkung.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
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
    ServiceErgebnis r = null;
    if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
        || DialogType == DialogTypeEnum.Edit)
    {
      r = FactoryService.PedigreeService.SaveSource(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, autor.Text, beschreibung.Text,
        zitat.Buffer.Text, bemerkung.Buffer.Text);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PedigreeService.DeleteSource(ServiceDaten, Model);
    }
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        UpdateParent();
        dialog.Hide();
      }
    }
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }
}
