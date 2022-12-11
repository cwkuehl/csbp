// <copyright file="AG210User.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for AG210User dialog.</summary>
public partial class AG210User : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label benutzerId0.</summary>
  [Builder.Object]
  private readonly Label benutzerId0;

  /// <summary>Entry benutzerId.</summary>
  [Builder.Object]
  private readonly Entry benutzerId;

  /// <summary>Label kennwort0.</summary>
  [Builder.Object]
  private readonly Label kennwort0;

  /// <summary>Entry kennwort.</summary>
  [Builder.Object]
  private readonly Entry kennwort;

  /// <summary>Label berechtigung0.</summary>
  [Builder.Object]
  private readonly Label berechtigung0;

  /// <summary>RadioButton berechtigung1.</summary>
  [Builder.Object]
  private readonly RadioButton berechtigung1;

  /// <summary>RadioButton berechtigung2.</summary>
  [Builder.Object]
  private readonly RadioButton berechtigung2;

  /// <summary>RadioButton berechtigung3.</summary>
  [Builder.Object]
  private readonly RadioButton berechtigung3;

  /// <summary>Label geburt0.</summary>
  [Builder.Object]
  private readonly Label geburt0;

  /// <summary>Date Geburt.</summary>
  //// [Builder.Object]
  private readonly Date geburt;

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
  private Benutzer model;

  /// <summary>Initializes a new instance of the <see cref="AG210User"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AG210User(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AG210User), dt, p1, p)
  {
    geburt = new Date(Builder.GetObject("geburt").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = true,
      Label = geburt0,
    };
    geburt.Show();
    geburt.DateChanged += OnGeburtDateChanged;
    SetBold(benutzerId0);
    SetBold(kennwort0);
    SetBold(berechtigung0);
    SetBold(geburt0);
    InitData(0);
    benutzerId.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AG210User Create(object p1 = null, CsbpBin p = null)
  {
    return new AG210User(GetBuilder("AG210User", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      SetUserData(new[] { berechtigung1, berechtigung2, berechtigung3 }, new[] { "0", "1", "2" });
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.ClientService.GetUser(ServiceDaten, Functions.ToInt32(uid)));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, Functions.ToString(k.Person_Nr));
        SetText(benutzerId, k.Benutzer_ID);
        SetText(kennwort, k.Passwort);
        SetText(berechtigung1, k.Berechtigung.ToString());
        geburt.Value = k.Geburt;
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      nr.IsEditable = false;
      benutzerId.IsEditable = !loeschen;
      kennwort.IsEditable = !loeschen;
      foreach (RadioButton a in berechtigung1.Group)
        a.Sensitive = !loeschen;
      geburt.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handles Geburt.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnGeburtDateChanged(object sender, DateChangedEventArgs e)
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
      r = FactoryService.ClientService.SaveUser(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? Functions.ToInt32(nr.Text) : 0, benutzerId.Text, kennwort.Text,
        Functions.ToInt32(GetText(berechtigung1)), geburt.Value);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.ClientService.DeleteUser(ServiceDaten, model);
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
