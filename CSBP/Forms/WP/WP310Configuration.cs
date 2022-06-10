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
  /// <summary>Dialog Model.</summary>
  private WpKonfiguration Model;

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

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static WP310Configuration Create(object p1 = null, CsbpBin p = null)
  {
    return new WP310Configuration(GetBuilder("WP310Configuration", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
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

  /// <summary>Model-Daten initialisieren.</summary>
  /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
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
          Application.Invoke(delegate
          {
            dialog.Hide();
          });
          return;
        }
        Model = k;
        nr.Text = k.Uid;
        bezeichnung.Text = k.Bezeichnung;
        box.Text = Functions.ToString(k.Box, 2);
        SetText(skala, Functions.ToString(k.Scale));
        umkehr.Text = Functions.ToString(k.Reversal);
        SetText(methode, Functions.ToString(k.Method));
        dauer.Text = Functions.ToString(k.Duration);
        relativ.Active = k.Relative;
        SetText(status, k.Status);
        notiz.Buffer.Text = k.Notiz ?? "";
        angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
        geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
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

  /// <summary>Handle Skala.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSkalaChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handle Methode.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMethodeChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handle Status.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnStatusChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handle Ok.</summary>
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
      r = FactoryService.StockService.DeleteConfiguration(ServiceDaten, Model);
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

  /// <summary>Handle Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }
}
