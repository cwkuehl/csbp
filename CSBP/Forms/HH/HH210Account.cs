// <copyright file="HH210Account.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for HH210Account dialog.</summary>
public partial class HH210Account : CsbpBin
{
  /// <summary>Dialog Model.</summary>
  private HhKonto Model;

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

  /// <summary>RadioButton kennzeichen1.</summary>
  [Builder.Object]
  private readonly RadioButton kennzeichen1;

  /// <summary>RadioButton kennzeichen2.</summary>
  [Builder.Object]
  private readonly RadioButton kennzeichen2;

  /// <summary>RadioButton kennzeichen3.</summary>
  [Builder.Object]
  private readonly RadioButton kennzeichen3;

  /// <summary>RadioButton kennzeichen4.</summary>
  [Builder.Object]
  private readonly RadioButton kennzeichen4;

  /// <summary>Label kontoart0.</summary>
  [Builder.Object]
  private readonly Label kontoart0;

  /// <summary>RadioButton kontoart1.</summary>
  [Builder.Object]
  private readonly RadioButton kontoart1;

  /// <summary>RadioButton kontoart2.</summary>
  [Builder.Object]
  private readonly RadioButton kontoart2;

  /// <summary>RadioButton kontoart3.</summary>
  [Builder.Object]
  private readonly RadioButton kontoart3;

  /// <summary>RadioButton kontoart4.</summary>
  [Builder.Object]
  private readonly RadioButton kontoart4;

  /// <summary>Date Von.</summary>
  //[Builder.Object]
  private readonly Date von;

  /// <summary>Date Bis.</summary>
  //[Builder.Object]
  private readonly Date bis;

  /// <summary>Entry betrag.</summary>
  [Builder.Object]
  private readonly Entry betrag;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Label buchung.</summary>
  [Builder.Object]
  private readonly Label buchung;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

#pragma warning restore CS0649

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static HH210Account Create(object p1 = null, CsbpBin p = null)
  {
    return new HH210Account(GetBuilder("HH210Account", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public HH210Account(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    von = new Date(Builder.GetObject("von").Handle)
    {
      IsNullable = true,
      IsWithCalendar = true,
      IsCalendarOpen = false
    };
    von.DateChanged += OnVonDateChanged;
    von.Show();
    bis = new Date(Builder.GetObject("bis").Handle)
    {
      IsNullable = true,
      IsWithCalendar = true,
      IsCalendarOpen = false
    };
    bis.DateChanged += OnBisDateChanged;
    bis.Show();
    SetBold(bezeichnung0);
    // SetBold(kennzeichen0);
    SetBold(kontoart0);
    InitData(0);
    bezeichnung.GrabFocus();
  }

  /// <summary>Model-Daten initialisieren.</summary>
  /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      SetUserData(new[] { kennzeichen1, kennzeichen2, kennzeichen3, kennzeichen4 },
        new[] { "", Constants.KZK_EK, Constants.KZK_GV, Constants.KZK_DEPOT });
      SetUserData(new[] { kontoart1, kontoart2, kontoart3, kontoart4 },
        new[] { Constants.ARTK_AKTIVKONTO, Constants.ARTK_PASSIVKONTO, Constants.ARTK_AUFWANDSKONTO, Constants.ARTK_ERTRAGSKONTO });
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      var aendern = DialogType == DialogTypeEnum.Edit;
      if (!neu && Parameter1 is string uid)
      {
        var daten = ServiceDaten;
        var k = Get(FactoryService.BudgetService.GetAccount(daten, uid));
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
        bezeichnung.Text = k.Name;
        SetText(kennzeichen1, k.Kz);
        SetText(kontoart1, k.Art);
        von.Value = k.Gueltig_Von;
        bis.Value = k.Gueltig_Bis;
        betrag.Text = Functions.ToString(k.EBetrag, 2);
        angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
        geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        buchung.Text = Get(FactoryService.BudgetService.GetBookingSpan(daten, k.Uid));
      }
      nr.IsEditable = false;
      bezeichnung.IsEditable = !loeschen;
      foreach (RadioButton a in kennzeichen1.Group)
        a.Sensitive = !(loeschen || (aendern && Model != null && (Model.Kz == Constants.KZK_EK || Model.Kz == Constants.KZK_GV)));
      foreach (RadioButton a in kontoart1.Group)
        a.Sensitive = !(loeschen || aendern);
      von.Sensitive = !loeschen;
      bis.Sensitive = !loeschen;
      betrag.IsEditable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
    }
  }

  /// <summary>Handle von.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handle bis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
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
      r = FactoryService.BudgetService.SaveAccount(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(kontoart1),
        GetText(kennzeichen1), bezeichnung.Text, von.Value, bis.Value, Functions.ToDecimal(betrag.Text) ?? 0);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.BudgetService.DeleteAccount(ServiceDaten, Model);
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
