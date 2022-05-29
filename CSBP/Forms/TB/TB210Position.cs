// <copyright file="TB210Position.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für TB210Position Dialog.</summary>
  public partial class TB210Position : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private TbOrt Model;

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

    /// <summary>Label breite0.</summary>
    [Builder.Object]
    private Label breite0;

    /// <summary>Entry breite.</summary>
    [Builder.Object]
    private Entry breite;

    /// <summary>Label laenge0.</summary>
    [Builder.Object]
    private Label laenge0;

    /// <summary>Entry laenge.</summary>
    [Builder.Object]
    private Entry laenge;

    /// <summary>Label hoehe0.</summary>
    [Builder.Object]
    private Label hoehe0;

    /// <summary>Entry hoehe.</summary>
    [Builder.Object]
    private Entry hoehe;

    /// <summary>Label notiz0.</summary>
    [Builder.Object]
    private Label notiz0;

    /// <summary>TextView notiz.</summary>
    [Builder.Object]
    private TextView notiz;

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

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static TB210Position Create(object p1 = null, CsbpBin p = null)
    {
      return new TB210Position(GetBuilder("TB210Position", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public TB210Position(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      SetBold(bezeichnung0);
      SetBold(breite0);
      SetBold(laenge0);
      InitData(0);
      bezeichnung.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        EventsActive = false;
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var aendern = DialogType == DialogTypeEnum.Edit;
        var uid = Parameter1 as string;
        if (!neu && uid != null)
        {
          var k = Get(FactoryService.DiaryService.GetPosition(daten, uid));
          if (k == null)
          {
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
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

    /// <summary>Behandlung von Breite.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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
        r = FactoryService.DiaryService.DeletePosition(ServiceDaten, Model);
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
