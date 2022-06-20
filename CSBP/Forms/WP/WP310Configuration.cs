// <copyright file="WP310Configuration.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for WP310Configuration dialog.</summary>
public partial class WP310Configuration : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label bezeichnung0.</summary>
  [Builder.Object]
  private readonly Label bezeichnung0;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>Label box0.</summary>
  [Builder.Object]
  private readonly Label box0;

  /// <summary>Entry box.</summary>
  [Builder.Object]
  private readonly Entry box;

  /// <summary>ComboBox skala.</summary>
  [Builder.Object]
  private readonly ComboBox skala;

  /// <summary>Label umkehr0.</summary>
  [Builder.Object]
  private readonly Label umkehr0;

  /// <summary>Entry umkehr.</summary>
  [Builder.Object]
  private readonly Entry umkehr;

  /// <summary>Label methode0.</summary>
  [Builder.Object]
  private readonly Label methode0;

  /// <summary>ComboBox methode.</summary>
  [Builder.Object]
  private readonly ComboBox methode;

  /// <summary>Label dauer0.</summary>
  [Builder.Object]
  private readonly Label dauer0;

  /// <summary>Entry dauer.</summary>
  [Builder.Object]
  private readonly Entry dauer;

  /// <summary>CheckButton relativ.</summary>
  [Builder.Object]
  private readonly CheckButton relativ;

  /// <summary>Label status0.</summary>
  [Builder.Object]
  private readonly Label status0;

  /// <summary>ComboBox status.</summary>
  [Builder.Object]
  private readonly ComboBox status;

  /// <summary>TextView notiz.</summary>
  [Builder.Object]
  private readonly TextView notiz;

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
  private WpKonfiguration model;

  /// <summary>Initializes a new instance of the <see cref="WP310Configuration"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP310Configuration(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(bezeichnung0);
    SetBold(box0);
    SetBold(umkehr0);
    SetBold(methode0);
    SetBold(dauer0);
    SetBold(status0);
    InitData(0);
    bezeichnung.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP310Configuration Create(object p1 = null, CsbpBin p = null)
  {
    return new WP310Configuration(GetBuilder("WP310Configuration", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      AddColumns(skala, Get(FactoryService.StockService.GetScaleList(daten)));
      AddColumns(methode, Get(FactoryService.StockService.GetMethodList(daten)));
      AddColumns(status, Get(FactoryService.StockService.GetStateList(daten)));
      SetText(skala, "1");
      SetText(methode, "1");
      SetText(status, "1");
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var copy = DialogType == DialogTypeEnum.Copy;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.StockService.GetConfiguration(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { dialog.Hide(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(bezeichnung, k.Bezeichnung);
        SetText(box, Functions.ToString(k.Box, 2));
        SetText(skala, Functions.ToString(k.Scale));
        SetText(umkehr, Functions.ToString(k.Reversal));
        SetText(methode, Functions.ToString(k.Method));
        SetText(dauer, Functions.ToString(k.Duration));
        relativ.Active = k.Relative;
        SetText(status, k.Status);
        SetText(notiz, k.Notiz);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      bezeichnung.IsEditable = !loeschen;
      box.IsEditable = !loeschen;
      skala.Sensitive = !loeschen;
      umkehr.IsEditable = !loeschen;
      methode.Sensitive = !loeschen;
      dauer.IsEditable = !loeschen;
      relativ.Sensitive = !loeschen;
      status.Sensitive = !loeschen;
      notiz.Editable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Skala.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSkalaChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Methode.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMethodeChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles state.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnStatusChanged(object sender, EventArgs e)
  {
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
      r = FactoryService.StockService.SaveConfiguration(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, bezeichnung.Text, Functions.ToDecimal(box.Text) ?? 0,
        Functions.ToInt32(umkehr.Text), Functions.ToInt32(GetText(methode)), Functions.ToInt32(dauer.Text),
        relativ.Active, Functions.ToInt32(GetText(skala)), GetText(status), notiz.Buffer.Text);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.StockService.DeleteConfiguration(ServiceDaten, model);
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
