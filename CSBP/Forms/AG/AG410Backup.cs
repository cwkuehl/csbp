// <copyright file="AG410Backup.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using CSBP.Apis.Enums;
using CSBP.Apis.Models.Extension;
using CSBP.Apis.Services;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for AG410Backup dialog.</summary>
public partial class AG410Backup : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label ziel0.</summary>
  [Builder.Object]
  private readonly Label ziel0;

  /// <summary>Entry ziel.</summary>
  [Builder.Object]
  private readonly Entry ziel;

  /// <summary>CheckButton encrypted.</summary>
  [Builder.Object]
  private readonly CheckButton encrypted;

  /// <summary>CheckButton zipped.</summary>
  [Builder.Object]
  private readonly CheckButton zipped;

  /// <summary>Label quelle0.</summary>
  [Builder.Object]
  private readonly Label quelle0;

  /// <summary>Entry quelle.</summary>
  [Builder.Object]
  private readonly Entry quelle;

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
  private BackupEntry model;

  /// <summary>Initializes a new instance of the <see cref="AG410Backup"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AG410Backup(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    SetBold(quelle0);
    SetBold(ziel0);
    InitData(0);
    ziel.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AG410Backup Create(object p1 = null, CsbpBin p = null)
  {
    return new AG410Backup(GetBuilder("AG410Backup", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.ClientService.GetBackupEntry(ServiceDaten, uid));
        model = k;
        nr.Text = k.Uid;
        ziel.Text = k.Target;
        encrypted.Active = k.Encrypted;
        zipped.Active = k.Zipped;
        quelle.Text = k.SourcesText;
        angelegt.Text = Base.ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
        geaendert.Text = Base.ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
      }
      nr.IsEditable = false;
      ziel.IsEditable = !loeschen;
      encrypted.Sensitive = !loeschen;
      zipped.Sensitive = !loeschen;
      quelle.IsEditable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      if (loeschen)
        ok.Label = Forms_delete;
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
      r = FactoryService.ClientService.SaveBackupEntry(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, ziel.Text,
          BackupEntry.SplitSources(quelle.Text), encrypted.Active, zipped.Active);
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.ClientService.DeleteBackupEntry(ServiceDaten, model);
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
