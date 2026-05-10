// <copyright file="EN110Query.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.EN;

using System;
using System.Linq;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for EN110Query dialog.</summary>
public partial class EN110Query : CsbpBin
{
  /// <summary>Last copied ID.</summary>
  private static string lastcopyuid = null;

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

  /// <summary>Label sortierung0.</summary>
  [Builder.Object]
  private readonly Label sortierung0;

  /// <summary>Entry sortierung.</summary>
  [Builder.Object]
  private readonly Entry sortierung;

  /// <summary>Label art0.</summary>
  [Builder.Object]
  private readonly Label art0;

  /// <summary>ComboBox art.</summary>
  [Builder.Object]
  private readonly ComboBox art;

  /// <summary>Label status0.</summary>
  [Builder.Object]
  private readonly Label status0;

  /// <summary>ComboBox status.</summary>
  [Builder.Object]
  private readonly ComboBox status;

  /// <summary>Entry aktKurs.</summary>
  [Builder.Object]
  private readonly Entry aktKurs;

  /// <summary>Entry stopKurs.</summary>
  [Builder.Object]
  private readonly Entry stopKurs;

  /// <summary>Entry signalKurs1.</summary>
  [Builder.Object]
  private readonly Entry signalKurs1;

  /// <summary>Entry muster.</summary>
  [Builder.Object]
  private readonly Entry muster;

  /// <summary>Entry typ.</summary>
  [Builder.Object]
  private readonly Entry typ;

  /// <summary>Entry waehrung.</summary>
  [Builder.Object]
  private readonly Entry waehrung;

  /// <summary>Entry param1.</summary>
  [Builder.Object]
  private readonly Entry param1;

  /// <summary>ComboBox relation.</summary>
  [Builder.Object]
  private readonly ComboBox relation;

  /// <summary>TextView notiz.</summary>
  [Builder.Object]
  private readonly TextView notiz;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>CheckButton anlage.</summary>
  [Builder.Object]
  private readonly CheckButton anlage;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private EnAbfrage model;

  /// <summary>Initializes a new instance of the <see cref="EN110Query"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public EN110Query(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(EN110Query), dt, p1, p)
  {
    SetBold(bezeichnung0);
    SetBold(sortierung0);
    SetBold(art0);
    SetBold(status0);
    InitData(0);
    bezeichnung.GrabFocus();
  }

  /// <summary>Gets or sets last copied ID.</summary>
  public static string Lastcopyuid { get => lastcopyuid; set => lastcopyuid = value; }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static EN110Query Create(object p1 = null, CsbpBin p = null)
  {
    return new EN110Query(GetBuilder("EN110Query", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      var daten = ServiceDaten;
      var pl = Get(FactoryService.EnergyService.GetKindList(daten));
      AddColumns(art, pl);
      AddColumns(status, Get(FactoryService.EnergyService.GetStateList(daten)));
      if (pl != null && pl.Any())
        SetText(art, pl.First().Schluessel);
      SetText(status, "1");
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var copy = DialogType == DialogTypeEnum.Copy;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.EnergyService.GetQuery(daten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(bezeichnung, k.Bezeichnung);
        SetText(sortierung, k.Sortierung);
        SetText(art, k.Art);
        SetText(status, k.Status);
        //SetText(aktKurs, Functions.ToString(k.CurrentPrice));
        //SetText(stopKurs, Functions.ToString(k.StopPrice));
        //SetText(signalKurs1, Functions.ToString(k.SignalPrice1));
        //SetText(muster, k.Pattern);
        //SetText(typ, k.Type);
        //SetText(waehrung, k.Currency);
        SetText(param1, k.Param1);
        //SetText(relation, k.Relation_Uid);
        SetText(notiz, k.Notiz);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      if (neu || copy)
        anlage.Active = true;
      nr.IsEditable = false;
      bezeichnung.IsEditable = !loeschen;
      sortierung.IsEditable = !loeschen;
      art.Sensitive = !loeschen;
      status.Sensitive = !loeschen;
      aktKurs.IsEditable = false;
      stopKurs.IsEditable = false;
      signalKurs1.IsEditable = !loeschen;
      muster.IsEditable = false;
      typ.IsEditable = !loeschen;
      waehrung.IsEditable = !loeschen;
      sortierung.IsEditable = !loeschen;
      relation.Sensitive = !loeschen;
      notiz.Editable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      anlage.Sensitive = !loeschen;
      if (loeschen)
        ok.Label = Forms_delete;
      var rl = Get(FactoryService.StockService.GetStockList(ServiceDaten, null, true, null, null, copy ? null : model?.Uid));
      var rs = AddColumns(relation);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      //if (!neu && model != null)
      //  SetText(relation, model.Relation_Uid);
    }
  }

  /// <summary>Handles Provider.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnProviderChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles state.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnStatusChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Relation.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRelationChanged(object sender, EventArgs e)
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
      // var rb = FactoryService.EnergyService.SaveQuery(ServiceDaten,
      //   DialogType == DialogTypeEnum.Edit ? nr.Text : null, bezeichnung.Text, kuerzel.Text,
      //   Functions.ToDecimal(signalKurs1.Text, 4), sortierung.Text,
      //   GetText(provider), GetText(status), GetText(relation), notiz.Buffer.Text,
      //   typ.Text, waehrung.Text, anlage.Active);
      // r = rb;
      // if (rb.Ok && rb.Ergebnis != null && DialogType == DialogTypeEnum.Copy)
      //   Lastcopyuid = rb.Ergebnis.Uid;
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.EnergyService.DeleteQuery(ServiceDaten, model);
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

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }
}
