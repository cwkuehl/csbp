// <copyright file="FZ350Book.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for FZ350Book dialog.</summary>
public partial class FZ350Book : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label titel0.</summary>
  [Builder.Object]
  private readonly Label titel0;

  /// <summary>Entry titel.</summary>
  [Builder.Object]
  private readonly Entry titel;

  /// <summary>Entry untertitel.</summary>
  [Builder.Object]
  private readonly Entry untertitel;

  /// <summary>Label autor0.</summary>
  [Builder.Object]
  private readonly Label autor0;

  /// <summary>ComboBox autor.</summary>
  [Builder.Object]
  private readonly ComboBox autor;

  /// <summary>Label serie0.</summary>
  [Builder.Object]
  private readonly Label serie0;

  /// <summary>ComboBox serie.</summary>
  [Builder.Object]
  private readonly ComboBox serie;

  /// <summary>Entry seriennummer.</summary>
  [Builder.Object]
  private readonly Entry seriennummer;

  /// <summary>Entry seiten.</summary>
  [Builder.Object]
  private readonly Entry seiten;

  /// <summary>Label sprache0.</summary>
  [Builder.Object]
  private readonly Label sprache0;

  /// <summary>RadioButton sprache1.</summary>
  [Builder.Object]
  private readonly RadioButton sprache1;

  /// <summary>RadioButton sprache2.</summary>
  [Builder.Object]
  private readonly RadioButton sprache2;

  /// <summary>RadioButton sprache3.</summary>
  [Builder.Object]
  private readonly RadioButton sprache3;

  /// <summary>RadioButton sprache4.</summary>
  [Builder.Object]
  private readonly RadioButton sprache4;

  /// <summary>CheckButton besitz.</summary>
  [Builder.Object]
  private readonly CheckButton besitz;

  /// <summary>Date Lesedatum.</summary>
  //// [Builder.Object]
  private readonly Date lesedatum;

  /// <summary>Date Hoerdatum.</summary>
  //// [Builder.Object]
  private readonly Date hoerdatum;

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
  private FzBuch model;

  /// <summary>Initializes a new instance of the <see cref="FZ350Book"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ350Book(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    lesedatum = new Date(Builder.GetObject("lesedatum").Handle)
    {
      IsNullable = true,
      IsWithCalendar = true,
      IsCalendarOpen = false,
      YesterdayAccel = "m",
      TomorrowAccel = "p",
    };
    lesedatum.DateChanged += OnLesedatumDateChanged;
    lesedatum.Show();
    hoerdatum = new Date(Builder.GetObject("hoerdatum").Handle)
    {
      IsNullable = true,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    hoerdatum.DateChanged += OnHoerdatumDateChanged;
    hoerdatum.Show();
    SetBold(titel0);
    SetBold(autor0);
    SetBold(serie0);
    SetBold(sprache0);
    InitData(0);
    titel.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ350Book Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ350Book(GetBuilder("FZ350Book", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      SetUserData(new[] { sprache1, sprache2, sprache3, sprache4 }, new[] { "1", "2", "3", "0" });
      var al = Get(FactoryService.PrivateService.GetAuthorList(daten));
      var rs = AddColumns(autor, emptyentry: true);
      foreach (var p in al)
        rs.AppendValues(p.CompleteName, p.Uid);
      SetText(autor, "");
      var sl = Get(FactoryService.PrivateService.GetSeriesList(daten));
      rs = AddColumns(serie);
      foreach (var p in sl)
        rs.AppendValues(p.Name, p.Uid);
      if (sl.Any())
        SetText(serie, sl.First().Uid);
      sprache1.Active = true;
      besitz.Active = false;
      lesedatum.Value = daten.Heute;
      hoerdatum.Value = null; // daten.Heute;
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.PrivateService.GetBook(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { dialog.Hide(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(titel, k.Titel);
        SetText(untertitel, k.Untertitel);
        SetText(autor, k.Autor_Uid);
        SetText(serie, k.Serie_Uid);
        SetText(seriennummer, Functions.ToString(k.Seriennummer == 0 ? (int?)null : k.Seriennummer));
        SetText(seiten, Functions.ToString(k.Seiten == 0 ? (int?)null : k.Seiten));
        SetText(sprache1, Functions.ToString(k.Sprache_Nr));
        besitz.Active = k.StatePossession;
        lesedatum.Value = k.StateRead;
        hoerdatum.Value = k.StateHeard;
        SetText(notiz, k.Notiz);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      titel.IsEditable = !loeschen;
      untertitel.IsEditable = !loeschen;
      autor.Sensitive = !loeschen;
      serie.Sensitive = !loeschen;
      seriennummer.IsEditable = !loeschen;
      seiten.IsEditable = !loeschen;
      foreach (RadioButton a in sprache1.Group)
        a.Sensitive = !loeschen;
      besitz.Sensitive = !loeschen;
      lesedatum.Sensitive = !loeschen;
      hoerdatum.Sensitive = !loeschen;
      notiz.Editable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Autor.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAutorChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Autorneu.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAutorneuClicked(object sender, EventArgs e)
  {
    if (Start(typeof(FZ310Author), FZ310_title, DialogTypeEnum.New, modal: true, csbpparent: this) is FzBuchautor a)
    {
      var al = Get(FactoryService.PrivateService.GetAuthorList(ServiceDaten));
      var rs = AddColumns(autor, emptyentry: true);
      foreach (var p in al)
        rs.AppendValues(p.CompleteName, p.Uid);
      SetText(autor, a.Uid);
    }
  }

  /// <summary>Handles Serie.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSerieChanged(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Serieneu.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSerieneuClicked(object sender, EventArgs e)
  {
    if (Start(typeof(FZ330Series), FZ330_title, DialogTypeEnum.New, modal: true, csbpparent: this) is FzBuchserie a)
    {
      var al = Get(FactoryService.PrivateService.GetSeriesList(ServiceDaten));
      var rs = AddColumns(serie, emptyentry: true);
      foreach (var p in al)
        rs.AppendValues(p.Name, p.Uid);
      SetText(serie, a.Uid);
    }
  }

  /// <summary>Handles lesedatum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnLesedatumDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles hoerdatum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHoerdatumDateChanged(object sender, DateChangedEventArgs e)
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
      r = FactoryService.PrivateService.SaveBook(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(autor), GetText(serie),
        Functions.ToInt32(seriennummer.Text), titel.Text, untertitel.Text, Functions.ToInt32(seiten.Text),
        Functions.ToInt32(GetText(sprache1)), besitz.Active, lesedatum.Value,
        hoerdatum.Value, notiz.Buffer.Text);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.PrivateService.DeleteBook(ServiceDaten, model);
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
