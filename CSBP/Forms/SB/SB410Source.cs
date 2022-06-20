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

  /// <summary>Dialog model.</summary>
  private SbQuelle model;

  /// <summary>Initializes a new instance of the <see cref="SB410Source"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public SB410Source(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(autor0);
    SetBold(beschreibung0);
    InitData(0);
    autor.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static SB410Source Create(object p1 = null, CsbpBin p = null)
  {
    return new SB410Source(GetBuilder("SB410Source", out var handle), handle, p1: p1, p: p);
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
          Application.Invoke((sender, e) => { dialog.Hide(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(autor, k.Autor);
        SetText(beschreibung, k.Beschreibung);
        SetText(zitat, k.Zitat);
        SetText(bemerkung, k.Bemerkung);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
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
      r = FactoryService.PedigreeService.DeleteSource(ServiceDaten, model);
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
