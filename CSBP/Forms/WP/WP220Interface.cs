// <copyright file="WP220Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Threading.Tasks;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP220Interface Dialog.</summary>
  public partial class WP220Interface : CsbpBin
  {
    /// <summary>State of calculation.</summary>
    StringBuilder Status = new StringBuilder();

    /// <summary>Cancel of calculation.</summary>
    StringBuilder Cancel = new StringBuilder();

#pragma warning disable 169, 649

    /// <summary>Label datum0.</summary>
    [Builder.Object]
    private Label datum0;

    /// <summary>Date Datum.</summary>
    //[Builder.Object]
    private Date datum;

    /// <summary>Label anzahl0.</summary>
    [Builder.Object]
    private Label anzahl0;

    /// <summary>Entry anzahl.</summary>
    [Builder.Object]
    private Entry anzahl;

    /// <summary>Label bezeichnung0.</summary>
    [Builder.Object]
    private Label bezeichnung0;

    /// <summary>Entry bezeichnung.</summary>
    [Builder.Object]
    private Entry bezeichnung;

    /// <summary>Label konfiguration0.</summary>
    [Builder.Object]
    private Label konfiguration0;

    /// <summary>TreeView konfiguration.</summary>
    [Builder.Object]
    private TreeView konfiguration;

    /// <summary>Label datei0.</summary>
    [Builder.Object]
    private Label datei0;

    /// <summary>Entry datei.</summary>
    [Builder.Object]
    private Entry datei;

    /// <summary>Button dateiAuswahl.</summary>
    [Builder.Object]
    private Button dateiAuswahl;

    /// <summary>Button export.</summary>
    [Builder.Object]
    private Button export;

    /// <summary>Label datum20.</summary>
    [Builder.Object]
    private Label datum20;

    /// <summary>Date Datum2.</summary>
    //[Builder.Object]
    private Date datum2;

    /// <summary>Date Datum3.</summary>
    //[Builder.Object]
    private Date datum3;

    /// <summary>Date Datum4.</summary>
    //[Builder.Object]
    private Date datum4;

    /// <summary>Label wertpapier0.</summary>
    [Builder.Object]
    private ScrolledWindow wertpapiersw;

    /// <summary>Label wertpapier0.</summary>
    [Builder.Object]
    private Label wertpapier0;

    /// <summary>TreeView wertpapier.</summary>
    [Builder.Object]
    private TreeView wertpapier;

    /// <summary>Label datei20.</summary>
    [Builder.Object]
    private Label datei20;

    /// <summary>Entry datei2.</summary>
    [Builder.Object]
    private Entry datei2;

    /// <summary>Button datei2Auswahl.</summary>
    [Builder.Object]
    private Button datei2Auswahl;

    /// <summary>Button export2.</summary>
    [Builder.Object]
    private Button export2;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

    /// <summary>Entry statustext.</summary>
    [Builder.Object]
    private Entry statustext;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP220Interface Create(object p1 = null, CsbpBin p = null)
    {
      return new WP220Interface(GetBuilder("WP220Interface", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP220Interface(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
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
      datum2 = new Date(Builder.GetObject("datum2").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      datum2.DateChanged += OnDatum2DateChanged;
      datum2.Show();
      datum3 = new Date(Builder.GetObject("datum3").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      datum3.DateChanged += OnDatum3DateChanged;
      datum3.Show();
      datum4 = new Date(Builder.GetObject("datum4").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      datum4.DateChanged += OnDatum4DateChanged;
      datum4.Show();
      // SetBold(client0);
      InitData(0);
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        var daten = ServiceDaten;
        var today = daten.Heute;
        datum.Value = Functions.Workday(today);
        anzahl.Text = "5";
        bezeichnung.Text = "%%";
        datum2.Value = Functions.Workday(new DateTime(today.Year - 1, 1, 1));
        datum3.Value = Functions.Workday(new DateTime(today.Year, 1, 1));
        datum4.Value = Functions.Workday(today);
        var clist = Get(FactoryService.StockService.GetConfigurationList(ServiceDaten, true, "1"));
        var cvalues = new List<string[]>();
        // Nr.;Bezeichnung;Geändert am;Geändert von;Angelegt am;Angelegt von
        cvalues.Add(new string[] { "", "", "", "", "" }); // Empty entry.
        foreach (var c in clist)
        {
          cvalues.Add(new string[] { c.Uid, c.Bezeichnung, Functions.ToString(c.Geaendert_Am, true), c.Geaendert_Von,
            Functions.ToString(c.Angelegt_Am, true), c.Angelegt_Von });
        }
        AddStringColumnsSort(konfiguration, WP220_konfiguration_columns, cvalues);
        SetText(konfiguration, Parameter.WP220Configuration);
        datei.Text = Parameter.WP220File ?? "";
        var slist = Get(FactoryService.StockService.GetStockList(ServiceDaten, true));
        var svalues = new List<string[]>();
        // Nr.;Bezeichnung;Geändert am;Geändert von;Angelegt am;Angelegt von
        svalues.Add(new string[] { "", "", "", "", "" }); // Empty entry.
        foreach (var s in slist)
        {
          svalues.Add(new string[] { s.Uid, s.Bezeichnung, Functions.ToString(s.Geaendert_Am, true), s.Geaendert_Von,
            Functions.ToString(s.Angelegt_Am, true), s.Angelegt_Von });
        }
        AddStringColumnsSort(wertpapier, WP220_wertpapier_columns, svalues);
        SetText(wertpapier, Parameter.WP220Stock);
        datum20.Visible = false;
        datum20.NoShowAll = true;
        datum2.Visible = false;
        datum2.NoShowAll = true;
        datum3.Visible = false;
        datum3.NoShowAll = true;
        datum4.Visible = false;
        datum4.NoShowAll = true;
        wertpapiersw.Visible = false;
        wertpapiersw.NoShowAll = true;
        wertpapier0.Visible = false;
        wertpapier0.NoShowAll = true;
        wertpapier.Visible = false;
        wertpapier.NoShowAll = true;
        datei2.Text = Parameter.WP220File2 ?? "";
        datei20.Visible = false;
        datei20.NoShowAll = true;
        datei2.Visible = false;
        datei2.NoShowAll = true;
        datei2Auswahl.Visible = false;
        datei2Auswahl.NoShowAll = true;
        export2.Visible = false;
        export2.NoShowAll = true;
        statustext.Visible = false;
        statustext.NoShowAll = true;
      }
    }

    /// <summary>Behandlung von datum.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von Konfiguration.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKonfigurationRowActivated(object sender, RowActivatedArgs e)
    {
    }

    /// <summary>Behandlung von Dateiauswahl.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDateiauswahlClicked(object sender, EventArgs e)
    {
      var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? WP220_select_file : datei.Text, "*.csv", WP220_select_ext);
      if (!string.IsNullOrEmpty(file))
      {
        datei.Text = file;
        Parameter.WP220File = file;
      }
    }

    /// <summary>Behandlung von Export.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnExportClicked(object sender, EventArgs e)
    {
      ExportStocks();
    }

    /// <summary>Behandlung von datum2.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatum2DateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von datum3.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatum3DateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von datum4.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatum4DateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von Wertpapier.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnWertpapierRowActivated(object sender, RowActivatedArgs e)
    {
    }

    /// <summary>Behandlung von Datei2auswahl.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatei2auswahlClicked(object sender, EventArgs e)
    {
      var file = SelectFile(string.IsNullOrEmpty(datei2.Text) ? WP220_select2_file : datei2.Text, "*.xls", WP220_select2_ext);
      if (!string.IsNullOrEmpty(file))
      {
        datei2.Text = file;
        Parameter.WP220File2 = file;
      }
    }

    /// <summary>Behandlung von Export2.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnExport2Clicked(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      if (Cancel.Length <= 0)
        Cancel.Append("Cancel");
      else
        dialog.Hide();
    }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.
    private async void ExportStocks()
#pragma warning restore RECS0165
    {
      if (string.IsNullOrWhiteSpace(datei.Text))
        throw new MessageException(Message.New(M1012));
      try
      {
        Status.Clear();
        Cancel.Clear();
        var r = await Task.Run(() =>
        {
          var r0 = FactoryService.StockService.ExportStocks(ServiceDaten, bezeichnung.Text, null,
            null, null, false, GetText(konfiguration), datum.ValueNn, Functions.ToInt32(anzahl.Text), Status, Cancel);
          return r0;
        });
        r.ThrowAllErrors();
        if (Cancel.Length <= 0)
          UiTools.SaveFile(r.Ergebnis, datei.Text);
        Application.Invoke(delegate
        {
          // refreshAction.Click();
        });
      }
      catch (Exception ex)
      {
        Application.Invoke(delegate
        {
          ShowError(ex.Message);
        });
      }
      finally
      {
        Cancel.Append("End");
      }
    }
  }
}
