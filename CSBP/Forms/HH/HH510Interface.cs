// <copyright file="HH510Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH
{
  using System;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für HH510Interface Dialog.</summary>
  public partial class HH510Interface : CsbpBin
  {
#pragma warning disable 169, 649

    /// <summary>Label titel0.</summary>
    [Builder.Object]
    private Label titel0;

    /// <summary>Entry titel.</summary>
    [Builder.Object]
    private Entry titel;

    /// <summary>Label von0.</summary>
    [Builder.Object]
    private Label von0;

    /// <summary>Date Von.</summary>
    //[Builder.Object]
    private Date von;

    /// <summary>Label bis0.</summary>
    [Builder.Object]
    private Label bis0;

    /// <summary>Date Bis.</summary>
    //[Builder.Object]
    private Date bis;

    /// <summary>Label berichte0.</summary>
    [Builder.Object]
    private Label berichte0;

    /// <summary>CheckButton eb.</summary>
    [Builder.Object]
    private CheckButton eb;

    /// <summary>CheckButton gv.</summary>
    [Builder.Object]
    private CheckButton gv;

    /// <summary>CheckButton sb.</summary>
    [Builder.Object]
    private CheckButton sb;

    /// <summary>CheckButton kassenbericht.</summary>
    [Builder.Object]
    private CheckButton kassenbericht;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

    /// <summary>Label datei0.</summary>
    [Builder.Object]
    private Label datei0;

    /// <summary>Entry datei.</summary>
    [Builder.Object]
    private Entry datei;

    /// <summary>Button dateiAuswahl.</summary>
    [Builder.Object]
    private Button dateiAuswahl;

    /// <summary>CheckButton loeschen.</summary>
    [Builder.Object]
    private CheckButton loeschen;

    /// <summary>Button import1.</summary>
    [Builder.Object]
    private Button import1;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static HH510Interface Create(object p1 = null, CsbpBin p = null)
    {
      return new HH510Interface(GetBuilder("HH510Interface", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public HH510Interface(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      von = new Date(Builder.GetObject("von").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      von.DateChanged += OnVonDateChanged;
      von.Show();
      bis = new Date(Builder.GetObject("bis").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      bis.DateChanged += OnBisDateChanged;
      bis.Show();
      SetBold(titel0);
      SetBold(von0);
      SetBold(bis0);
      SetBold(berichte0);
      InitData(0);
      titel.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        titel.Text = Parameter.HH510Title;
        kassenbericht.Active = Parameter.HH510Cashreport;
        datei.Text = Parameter.HH510File;
        var p = Parameter1 as Tuple<string, DateTime, DateTime>;
        if (p != null)
        {
          var kz = p.Item1;
          var v = p.Item2;
          var b = p.Item3;
          if (kz == Constants.KZBI_EROEFFNUNG)
          {
            von.Value = v;
            bis.Value = new DateTime(v.Year, 12, 31);
            eb.Active = true;
          }
          else if (kz == Constants.KZBI_SCHLUSS)
          {
            bis.Value = v;
            von.Value = b.AddDays(1 - b.DayOfYear);
            sb.Active = true;
          }
          else
          {
            von.Value = v;
            bis.Value = b;
            gv.Active = true;
          }
        }
        if (ServiceDaten.BenutzerId.ToLower() != "wolfgang")
        {
          datei0.Visible = false;
          datei.Visible = false;
          dateiAuswahl.Visible = false;
          loeschen.Visible = false;
          import1.Visible = false;
        }
      }
    }

    /// <summary>Behandlung von von.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnVonDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von bis.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBisDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnOkClicked(object sender, EventArgs e)
    {
      Parameter.HH510Title = titel.Text;
      Parameter.HH510Cashreport = kassenbericht.Active;
      if (eb.Active || gv.Active || sb.Active)
      {
        var pdf = Get(
         FactoryService.BudgetService.GetAnnualReport(ServiceDaten, von.ValueNn, bis.ValueNn, titel.Text,
           eb.Active, gv.Active, sb.Active));
        UiTools.SaveFile(pdf, M0(HH048));
      }
      if (kassenbericht.Active)
      {
        var pdf = Get(FactoryService.BudgetService.GetCashReport(ServiceDaten, von.ValueNn, bis.ValueNn, titel.Text));
        UiTools.SaveFile(pdf, M0(HH049));
      }
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }

    /// <summary>Behandlung von Dateiauswahl.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDateiauswahlClicked(object sender, EventArgs e)
    {
      var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? HH510_select_file : datei.Text, "*.csv", HH510_select_ext);
      if (!string.IsNullOrEmpty(file))
      {
        datei.Text = file;
        Parameter.HH510File = file;
      }
    }

    /// <summary>Behandlung von Import1.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnImport1Clicked(object sender, EventArgs e)
    {
      if (string.IsNullOrEmpty(datei.Text))
        throw new MessageException(M1012);
      if (!ShowYesNoQuestion(M0(HH052)))
        return;
      var lines = UiTools.ReadFile(datei.Text);
      var message = Get(FactoryService.BudgetService.ImportBookingList(ServiceDaten, lines, loeschen.Active));
      if (!string.IsNullOrEmpty(message))
      {
        Application.Invoke(delegate
        {
          UpdateParent();
        });
        ShowInfo(message);
      }
    }
  }
}
