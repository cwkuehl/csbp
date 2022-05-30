// <copyright file="AG410Backup.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models.Extension;
  using CSBP.Apis.Services;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AG410Backup Dialog.</summary>
  public partial class AG410Backup : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private BackupEntry Model;

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

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AG410Backup Create(object p1 = null, CsbpBin p = null)
    {
      return new AG410Backup(GetBuilder("AG410Backup", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AG410Backup(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(quelle0);
      SetBold(ziel0);
      InitData(0);
      ziel.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      if (step <= 0)
      {
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        if (!neu && Parameter1 is string uid)
        {
          var k = Get(FactoryService.ClientService.GetBackupEntry(ServiceDaten, uid));
          Model = k;
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

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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
        r = FactoryService.ClientService.DeleteBackupEntry(ServiceDaten, Model);
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

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }
  }
}
