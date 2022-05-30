// <copyright file="TB110Date.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für TB110Date Dialog.</summary>
  public partial class TB110Date : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private TbOrt Model;

#pragma warning disable CS0649

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

    /// <summary>Label date0.</summary>
    [Builder.Object]
    private Label date0;

    /// <summary>Date date.</summary>
    //[Builder.Object]
    private Date date;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static TB110Date Create(object p1 = null, CsbpBin p = null)
    {
      return new TB110Date(GetBuilder("TB110Date", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public TB110Date(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      date = new Date(Builder.GetObject("datum").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = true,
        YesterdayAccel = "m",
        TomorrowAccel = "p",
        TooltipText = TB110_datum_tt,
      };
      date.Show();
      SetBold(date0);
      InitData(0);
      date.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        EventsActive = false;
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var aendern = DialogType == DialogTypeEnum.Edit;
        var p = Parameter1 as Tuple<string, DateTime>;
        if (!neu && p.Item1 != null)
        {
          var k = Get(FactoryService.DiaryService.GetPosition(daten, p.Item1));
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
          date.Value = p.Item2;
        }
        nr.IsEditable = false;
        bezeichnung.Sensitive = !loeschen;
        date.Sensitive = !loeschen;
        if (loeschen)
          ok.Label = Forms_delete;
        EventsActive = true;
      }
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      var daten = ServiceDaten;
      var p = Get(FactoryService.DiaryService.GetPosition(daten, nr.Text));
      if (p != null && p.Bezeichnung != bezeichnung.Text)
      {
        var r = FactoryService.DiaryService.SavePosition(daten, p.Uid, bezeichnung.Text, Functions.ToString(p.Breite, 5), Functions.ToString(p.Laenge, 5), Functions.ToString(p.Hoehe, 2), p.Notiz);
        Get(r);
        if (!r.Ok)
          return;
      }
      var d = date.ValueNn;
      Response = d;
      dialog.Hide();
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
