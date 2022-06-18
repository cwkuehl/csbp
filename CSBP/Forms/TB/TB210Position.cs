// <copyright file="TB210Position.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for TB210Position dialog.</summary>
public partial class TB210Position : CsbpBin
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

  /// <summary>Label breite0.</summary>
  [Builder.Object]
  private readonly Label breite0;

  /// <summary>Entry breite.</summary>
  [Builder.Object]
  private readonly Entry breite;

  /// <summary>Label laenge0.</summary>
  [Builder.Object]
  private readonly Label laenge0;

  /// <summary>Entry laenge.</summary>
  [Builder.Object]
  private readonly Entry laenge;

  /// <summary>Entry hoehe.</summary>
  [Builder.Object]
  private readonly Entry hoehe;

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
  private TbOrt model;

  /// <summary>Initializes a new instance of the <see cref="TB210Position"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public TB210Position(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(bezeichnung0);
    SetBold(breite0);
    SetBold(laenge0);
    InitData(0);
    bezeichnung.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static TB210Position Create(object p1 = null, CsbpBin p = null)
  {
    return new TB210Position(GetBuilder("TB210Position", out var handle), handle, p1: p1, p: p);
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
        var k = Get(FactoryService.DiaryService.GetPosition(daten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { dialog.Hide(); });
          return;
        }
        model = k;
        nr.Text = k.Uid ?? "";
        bezeichnung.Buffer.Text = k.Bezeichnung ?? "";
        breite.Buffer.Text = Functions.ToString(k.Breite, 5);
        laenge.Buffer.Text = Functions.ToString(k.Laenge, 5);
        hoehe.Buffer.Text = Functions.ToString(k.Hoehe, 2);
        notiz.Buffer.Text = k.Notiz ?? "";
        angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
        geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
      }
      nr.IsEditable = false;
      bezeichnung.Sensitive = !loeschen;
      breite.Sensitive = !loeschen;
      laenge.Sensitive = !loeschen;
      hoehe.Sensitive = !loeschen;
      notiz.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
      EventsActive = true;
    }
  }

  /// <summary>Handles Breite.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBreiteFocusOutEvent(object sender, Gtk.FocusOutEventArgs e)
  {
    var c = Functions.ToCoordinates(breite.Text);
    if (c != null)
    {
      breite.Text = Functions.ToString(c.Item1, 5);
      laenge.Text = Functions.ToString(c.Item2, 5);
      hoehe.Text = Functions.ToString(c.Item3, 2);
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
      r = FactoryService.DiaryService.SavePosition(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, bezeichnung.Text, breite.Text,
        laenge.Text, hoehe.Text, notiz.Buffer.Text);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.DiaryService.DeletePosition(ServiceDaten, model);
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
