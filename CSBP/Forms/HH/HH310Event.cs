// <copyright file="HH310Event.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für HH310Event Dialog.</summary>
  public partial class HH310Event : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private HhEreignis Model;

#pragma warning disable 169, 649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Label bezeichnung0.</summary>
    [Builder.Object]
    private Label bezeichnung0;

    /// <summary>Entry bezeichnung.</summary>
    [Builder.Object]
    private Entry bezeichnung;

    /// <summary>Label kennzeichen0.</summary>
    [Builder.Object]
    private Label kennzeichen0;

    /// <summary>Entry kennzeichen.</summary>
    [Builder.Object]
    private Entry kennzeichen;

    /// <summary>Label eText0.</summary>
    [Builder.Object]
    private Label eText0;

    /// <summary>Entry eText.</summary>
    [Builder.Object]
    private Entry eText;

    /// <summary>Label sollkonto0.</summary>
    [Builder.Object]
    private Label sollkonto0;

    /// <summary>TreeView sollkonto.</summary>
    [Builder.Object]
    private TreeView sollkonto;

    /// <summary>Label habenkonto0.</summary>
    [Builder.Object]
    private Label habenkonto0;

    /// <summary>TreeView habenkonto.</summary>
    [Builder.Object]
    private TreeView habenkonto;

    /// <summary>Label angelegt0.</summary>
    [Builder.Object]
    private Label angelegt0;

    /// <summary>Entry angelegt.</summary>
    [Builder.Object]
    private Entry angelegt;

    /// <summary>Label geaendert0.</summary>
    [Builder.Object]
    private Label geaendert0;

    /// <summary>Entry geaendert.</summary>
    [Builder.Object]
    private Entry geaendert;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button kontentausch.</summary>
    [Builder.Object]
    private Button kontentausch;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static HH310Event Create(object p1 = null, CsbpBin p = null)
    {
      return new HH310Event(GetBuilder("HH310Event", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public HH310Event(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(bezeichnung0);
      SetBold(eText0);
      SetBold(sollkonto0);
      SetBold(habenkonto0);
      InitData(0);
      bezeichnung.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        var kl = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, null, null));
        var values = new List<string[]>();
        foreach (var e in kl)
        {
          // Nr.;Bezeichnung
          values.Add(new string[] { e.Uid, e.Name });
        }
        AddStringColumnsSort(sollkonto, HH310_sollkonto_columns, values);
        AddStringColumnsSort(habenkonto, HH310_habenkonto_columns, values);
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var aendern = DialogType == DialogTypeEnum.Edit;
        var uid = Parameter1 as string;
        if (!neu && uid != null)
        {
          var k = Get(FactoryService.BudgetService.GetEvent(ServiceDaten, uid));
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
          bezeichnung.Text = k.Bezeichnung;
          kennzeichen.Text = k.Kz;
          eText.Text = k.EText;
          SetText(sollkonto, k.Soll_Konto_Uid);
          SetText(habenkonto, k.Haben_Konto_Uid);
          angelegt.Text = k.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = k.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        nr.IsEditable = false;
        bezeichnung.IsEditable = !loeschen;
        kennzeichen.IsEditable = !loeschen;
        eText.IsEditable = !loeschen;
        sollkonto.Sensitive = !loeschen;
        habenkonto.Sensitive = !loeschen;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        if (loeschen)
          ok.Label = Forms_delete;
      }
    }

    /// <summary>Behandlung von Sollkonto.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSollkontoRowActivated(object sender, RowActivatedArgs e)
    {
    }

    /// <summary>Behandlung von Habenkonto.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHabenkontoRowActivated(object sender, RowActivatedArgs e)
    {
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
        r = FactoryService.BudgetService.SaveEvent(ServiceDaten,
            DialogType == DialogTypeEnum.Edit ? nr.Text : null, kennzeichen.Text,
            GetText(sollkonto), GetText(habenkonto), bezeichnung.Text, eText.Text);
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.BudgetService.DeleteEvent(ServiceDaten, Model);
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

    /// <summary>Behandlung von Kontentausch.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKontentauschClicked(object sender, EventArgs e)
    {
      var s = GetText(sollkonto);
      var h = GetText(habenkonto);
      SetText(sollkonto, h);
      SetText(habenkonto, s);
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
