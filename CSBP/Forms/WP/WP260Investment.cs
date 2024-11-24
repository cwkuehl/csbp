// <copyright file="WP260Investment.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for WP260Investment dialog.</summary>
public partial class WP260Investment : CsbpBin
{
  /// <summary>Last copied ID.</summary>
  private static string lastcopyuid = null;

#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label wertpapier0.</summary>
  [Builder.Object]
  private readonly Label wertpapier0;

  /// <summary>ComboBox wertpapier.</summary>
  [Builder.Object]
  private readonly ComboBox wertpapier;

  /// <summary>Label bezeichnung0.</summary>
  [Builder.Object]
  private readonly Label bezeichnung0;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>Label status0.</summary>
  [Builder.Object]
  private readonly Label status0;

  /// <summary>ComboBox status.</summary>
  [Builder.Object]
  private readonly ComboBox status;

  /// <summary>ComboBox depot.</summary>
  [Builder.Object]
  private readonly ComboBox depot;

  /// <summary>ComboBox abrechnung.</summary>
  [Builder.Object]
  private readonly ComboBox abrechnung;

  /// <summary>ComboBox ertrag.</summary>
  [Builder.Object]
  private readonly ComboBox ertrag;

  /// <summary>TextView notiz.</summary>
  [Builder.Object]
  private readonly TextView notiz;

  /// <summary>TextView daten.</summary>
  [Builder.Object]
  private readonly TextView data;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Date Valuta.</summary>
  //// [Builder.Object]
  private readonly Date valuta;

  /// <summary>Entry stand.</summary>
  [Builder.Object]
  private readonly Entry stand;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private WpAnlage model;

  /// <summary>Initializes a new instance of the <see cref="WP260Investment"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP260Investment(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(WP260Investment), dt, p1, p)
  {
    valuta = new Date(Builder.GetObject("valuta").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    //// valuta.DateChanged += OnValutaDateChanged;
    valuta.Show();
    SetBold(wertpapier0);
    SetBold(bezeichnung0);
    SetBold(status0);
    InitData(0);
#pragma warning disable CS0612 // deprecated.
    data.OverrideFont(Pango.FontDescription.FromString("mono"));
#pragma warning restore CS0612
    bezeichnung.GrabFocus();
  }

  /// <summary>Gets or sets the last copied ID.</summary>
  public static string Lastcopyuid { get => lastcopyuid; set => lastcopyuid = value; }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP260Investment Create(object p1 = null, CsbpBin p = null)
  {
    return new WP260Investment(GetBuilder("WP260Investment", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      var rl = Get(FactoryService.StockService.GetStockList(daten, true)) ?? new List<WpWertpapier>();
      var rs = AddColumns(wertpapier);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      AddColumns(status, Get(FactoryService.StockService.GetStateList(ServiceDaten)));
      SetText(status, "1");
      var kl = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, null, daten.Heute, daten.Heute));
      var keind = !kl.Any(a => a.Kz == Constants.KZK_DEPOT);
      var rd = AddColumns(depot, emptyentry: true);
      var ra = AddColumns(abrechnung, emptyentry: true);
      var re = AddColumns(ertrag, emptyentry: true);
      foreach (var p in kl)
      {
        if (p.Art == Constants.ARTK_AKTIVKONTO && (keind || p.Kz == Constants.KZK_DEPOT))
          rd.AppendValues(p.Name, p.Uid);
        if (p.Art == Constants.ARTK_AKTIVKONTO && (keind || p.Kz == Constants.KZK_DEPOT))
          ra.AppendValues(p.Name, p.Uid);
        if (p.Art == Constants.ARTK_ERTRAGSKONTO && (keind || p.Kz == Constants.KZK_DEPOT))
          re.AppendValues(p.Name, p.Uid);
      }
      valuta.Value = daten.Heute.AddDays(-1); // Yesterday.
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.StockService.GetInvestment(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(wertpapier, k.Wertpapier_Uid);
        SetText(bezeichnung, k.Bezeichnung);
        SetText(status, Functions.ToString(k.State));
        SetText(depot, k.PortfolioAccountUid);
        SetText(abrechnung, k.SettlementAccountUid);
        SetText(ertrag, k.IncomeAccountUid);
        SetText(notiz, k.Notiz);
        SetText(data, k.Data);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      wertpapier.Sensitive = !loeschen;
      bezeichnung.IsEditable = !loeschen;
      status.Sensitive = !loeschen;
      depot.Sensitive = !loeschen;
      abrechnung.Sensitive = !loeschen;
      ertrag.Sensitive = !loeschen;
      notiz.Editable = !loeschen;
      data.Editable = false;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Wertpapier.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWertpapierChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Wpdetails.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWpdetailsClicked(object sender, EventArgs e)
  {
    var uid = GetText(wertpapier);
    Start(typeof(WP210Stock), WP210_title, DialogTypeEnum.Edit, uid, csbpparent: this);
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
    var daten = ServiceDaten;
    if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
      || DialogType == DialogTypeEnum.Edit)
    {
      var rb = FactoryService.StockService.SaveInvestment(daten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(wertpapier), bezeichnung.Text,
        notiz.Buffer.Text, Functions.ToInt32(GetText(status)), GetText(depot), GetText(abrechnung), GetText(ertrag),
        valuta.Value, Functions.ToDecimal(stand.Text, 2) ?? 0);
      r = rb;
      if (rb.Ok && !string.IsNullOrEmpty(stand.Text) && rb.Ergebnis != null)
      {
        Get(FactoryService.StockService.CalculateInvestments(daten, null, rb.Ergebnis.Uid, null, valuta.ValueNn, false, null, new System.Text.StringBuilder(), new System.Text.StringBuilder()));
      }
      if (rb.Ok && rb.Ergebnis != null && DialogType == DialogTypeEnum.Copy)
        Lastcopyuid = rb.Ergebnis.Uid;
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.StockService.DeleteInvestment(daten, model);
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
