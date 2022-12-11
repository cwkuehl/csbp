// <copyright file="HH310Event.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH310Event dialog.</summary>
public partial class HH310Event : CsbpBin
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

  /// <summary>Entry kennzeichen.</summary>
  [Builder.Object]
  private readonly Entry kennzeichen;

  /// <summary>Label eText0.</summary>
  [Builder.Object]
  private readonly Label eText0;

  /// <summary>Entry eText.</summary>
  [Builder.Object]
  private readonly Entry eText;

  /// <summary>Label sollkonto0.</summary>
  [Builder.Object]
  private readonly Label sollkonto0;

  /// <summary>TreeView sollkonto.</summary>
  [Builder.Object]
  private readonly TreeView sollkonto;

  /// <summary>Label habenkonto0.</summary>
  [Builder.Object]
  private readonly Label habenkonto0;

  /// <summary>TreeView habenkonto.</summary>
  [Builder.Object]
  private readonly TreeView habenkonto;

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
  private HhEreignis model;

  /// <summary>Initializes a new instance of the <see cref="HH310Event"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH310Event(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(HH310Event), dt, p1, p)
  {
    SetBold(bezeichnung0);
    SetBold(eText0);
    SetBold(sollkonto0);
    SetBold(habenkonto0);
    InitData(0);
    bezeichnung.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH310Event Create(object p1 = null, CsbpBin p = null)
  {
    return new HH310Event(GetBuilder("HH310Event", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      var kl = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, null, null));
      var values = new List<string[]>();
      foreach (var e in kl)
      {
        // Nr.;Bezeichnung
        values.Add(new string[] { e.Uid, e.Name });
      }
      AddStringColumnsSort(sollkonto, HH310_sollkonto_columns, values);
      AddStringColumnsSort(habenkonto, HH310_habenkonto_columns, values);
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var aendern = DialogType == DialogTypeEnum.Edit;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.BudgetService.GetEvent(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) =>
          {
            CloseDialog();
          });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(bezeichnung, k.Bezeichnung);
        SetText(kennzeichen, k.Kz);
        SetText(eText, k.EText);
        SetText(sollkonto, k.Soll_Konto_Uid);
        SetText(habenkonto, k.Haben_Konto_Uid);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      bezeichnung.IsEditable = !loeschen;
      kennzeichen.IsEditable = !loeschen;
      eText.IsEditable = !loeschen;
      sollkonto.Sensitive = !loeschen;
      habenkonto.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Sollkonto.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSollkontoRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles Habenkonto.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHabenkontoRowActivated(object sender, RowActivatedArgs e)
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
      r = FactoryService.BudgetService.SaveEvent(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, kennzeichen.Text,
          GetText(sollkonto), GetText(habenkonto), bezeichnung.Text, eText.Text);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.BudgetService.DeleteEvent(ServiceDaten, model);
    }
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        UpdateParent();
        CloseDialog();
      }
    }
  }

  /// <summary>Handles Kontentausch.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKontentauschClicked(object sender, EventArgs e)
  {
    var s = GetText(sollkonto);
    var h = GetText(habenkonto);
    SetText(sollkonto, h);
    SetText(habenkonto, s);
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }
}
