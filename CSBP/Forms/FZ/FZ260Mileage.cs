// <copyright file="FZ260Mileage.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Linq;
using CSBP.Forms.Controls;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for FZ260Mileage dialog.</summary>
public partial class FZ260Mileage : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label fahrrad0.</summary>
  [Builder.Object]
  private readonly Label fahrrad0;

  /// <summary>ComboBox fahrrad.</summary>
  [Builder.Object]
  private readonly ComboBox fahrrad;

  /// <summary>Label datum0.</summary>
  [Builder.Object]
  private readonly Label datum0;

  /// <summary>Date Datum.</summary>
  //// [Builder.Object]
  private readonly Date datum;

  /// <summary>Entry zaehler.</summary>
  [Builder.Object]
  private readonly Entry zaehler;

  /// <summary>Label km0.</summary>
  [Builder.Object]
  private readonly Label km0;

  /// <summary>Entry km.</summary>
  [Builder.Object]
  private readonly Entry km;

  /// <summary>Entry schnitt.</summary>
  [Builder.Object]
  private readonly Entry schnitt;

  /// <summary>Entry beschreibung.</summary>
  [Builder.Object]
  private readonly Entry beschreibung;

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
  private FzFahrradstand model;

  /// <summary>Initializes a new instance of the <see cref="FZ260Mileage"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ260Mileage(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(FZ260Mileage), dt, p1, p)
  {
    datum = new Date(Builder.GetObject("datum").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
      YesterdayAccel = "m",
      TomorrowAccel = "p",
    };
    datum.DateChanged += OnDatumDateChanged;
    datum.Show();
    SetBold(fahrrad0);
    SetBold(datum0);
    SetBold(km0);
    InitData(0);
    km.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ260Mileage Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ260Mileage(GetBuilder("FZ260Mileage", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      var rl = Get(FactoryService.PrivateService.GetBikeList(daten));
      var rs = AddColumns(fahrrad, emptyentry: true);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      if (rl.Any())
        SetText(fahrrad, rl.First().Uid);
      else
        SetText(fahrrad, "");
      datum.Value = daten.Heute;
      var neu = DialogType == DialogTypeEnum.New;
      var copy = DialogType == DialogTypeEnum.Copy;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is Tuple<string, DateTime?, int> key && key.Item2.HasValue)
      {
        var k = Get(FactoryService.PrivateService.GetMileage(daten, key.Item1, key.Item2.Value, key.Item3));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, k.Nr.ToString());
        SetText(fahrrad, k.Fahrrad_Uid);
        datum.Value = k.Datum;
        SetText(zaehler, Functions.ToString(k.Zaehler_km, 0));
        SetText(km, Functions.ToString(k.Periode_km, 0));
        SetText(schnitt, Functions.ToString(k.Periode_Schnitt, 2));
        SetText(beschreibung, Functions.ToString(k.Beschreibung));
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      fahrrad.Sensitive = neu;
      datum.Sensitive = neu || copy;
      zaehler.IsEditable = !loeschen;
      km.IsEditable = !loeschen;
      schnitt.IsEditable = !loeschen;
      beschreibung.IsEditable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
      EventsActive = true;
    }
  }

  /// <summary>Handles Fahrrad.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFahrradChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles datum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Zaehler.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZaehlerKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    // System.Diagnostics.Debug.WriteLine($"OnZaehlerKeyReleaseEvent {e.Event.Key}");
    var k = e.Event.Key;
    if ((k >= Gdk.Key.Key_0 && k <= Gdk.Key.Key_9) || (k >= Gdk.Key.KP_0 && k <= Gdk.Key.KP_9))
      SetText(km, "");
  }

  /// <summary>Handles Km.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKmKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    // System.Diagnostics.Debug.WriteLine($"OnKmKeyReleaseEvent {e.Event.Key}");
    var k = e.Event.Key;
    if ((k >= Gdk.Key.Key_0 && k <= Gdk.Key.Key_9) || (k >= Gdk.Key.KP_0 && k <= Gdk.Key.KP_9))
      SetText(zaehler, "");
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
      r = FactoryService.PrivateService.SaveMileage(ServiceDaten, GetText(fahrrad),
          datum.ValueNn, DialogType == DialogTypeEnum.Edit ? Functions.ToInt32(nr.Text) : -1,
          Functions.ToDecimal(zaehler.Text, 0) ?? 0, Functions.ToDecimal(km.Text, 0) ?? 0,
          Functions.ToDecimal(schnitt.Text, 2) ?? 0, beschreibung.Text);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PrivateService.DeleteMileage(ServiceDaten, model);
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
