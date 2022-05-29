// <copyright file="AG110Client.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AG110Client Dialog.</summary>
  public partial class AG110Client : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private MaMandant Model;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private readonly Entry nr;

    /// <summary>Label beschreibung0.</summary>
    [Builder.Object]
    private readonly Label beschreibung0;

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

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AG110Client Create(object p1 = null, CsbpBin p = null)
    {
      return new AG110Client(GetBuilder("AG110Client", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AG110Client(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(beschreibung0);
      InitData(0);
      beschreibung.GrabFocus();
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
          var k = Get(FactoryService.ClientService.GetClient(ServiceDaten, Functions.ToInt32(uid)));
          Model = k;
          nr.Text = k.Nr.ToString();
          beschreibung.Text = k.Beschreibung;
          angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        nr.IsEditable = false;
        beschreibung.IsEditable = !loeschen;
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
        r = FactoryService.ClientService.SaveClient(ServiceDaten,
            DialogType == DialogTypeEnum.Edit ? Functions.ToInt32(nr.Text) : 0, beschreibung.Text);
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.ClientService.DeleteClient(ServiceDaten, Model);
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
