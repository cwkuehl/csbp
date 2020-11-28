// <copyright file="AD120Birthdays.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AD
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;

  /// <summary>Controller für AD120Birthdays Dialog.</summary>
  public partial class AD120Birthdays : CsbpBin
  {
#pragma warning disable 169, 649

    /// <summary>Label datum0.</summary>
    [Builder.Object]
    private Label datum0;

    /// <summary>Date Datum.</summary>
    //[Builder.Object]
    private Date datum;

    /// <summary>Label tage0.</summary>
    [Builder.Object]
    private Label tage0;

    /// <summary>Entry tage.</summary>
    [Builder.Object]
    private Entry tage;

    /// <summary>Label geburtstage0.</summary>
    [Builder.Object]
    private Label geburtstage0;

    /// <summary>TextView geburtstage.</summary>
    [Builder.Object]
    private TextView geburtstage;

    /// <summary>CheckButton starten.</summary>
    [Builder.Object]
    private CheckButton starten;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AD120Birthdays Create(object p1 = null, CsbpBin p = null)
    {
      return new AD120Birthdays(GetBuilder("AD120Birthdays", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public AD120Birthdays(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      datum = new Date(Builder.GetObject("datum").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      datum.DateChanged += OnDatumDateChanged;
      datum.Show();
      SetBold(datum0);
      SetBold(tage0);
      InitData(0);
      ok.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        datum.Value = daten.Heute;
        tage.Text = Functions.ToString(Parameter.AD120Days);
        starten.Active = Parameter.AD120Start;
        EventsActive = true;
      }
      if (step <= 1)
      {
        var l = Get(FactoryService.AddressService.GetBirthdayList(daten, datum.ValueNn,
            Functions.ToInt32(tage.Text))) ?? new List<string>();
        if (l.Count <= 1)
        {
          Application.Invoke(delegate
          {
            dialog.Hide();
          });
          return;
        }
        geburtstage.Buffer.Text = string.Join("\n", l.ToArray());
      }
    }

    /// <summary>Behandlung von Datum.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
    {
      if (EventsActive)
        InitData(1);
    }

    /// <summary>Behandlung von Tage.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnTageKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      if (EventsActive)
        InitData(1);
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      Parameter.AD120Days = Functions.ToInt32(tage.Text);
      Parameter.AD120Start = starten.Active;
      dialog.Hide();
    }
  }
}
