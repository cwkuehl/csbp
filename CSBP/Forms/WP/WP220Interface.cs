// <copyright file="WP220Interface.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for WP220Interface dialog.</summary>
public partial class WP220Interface : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Date Datum.</summary>
  //// [Builder.Object]
  private readonly Date datum;

  /// <summary>Entry anzahl.</summary>
  [Builder.Object]
  private readonly Entry anzahl;

  /// <summary>Entry bezeichnung.</summary>
  [Builder.Object]
  private readonly Entry bezeichnung;

  /// <summary>TreeView konfiguration.</summary>
  [Builder.Object]
  private readonly TreeView konfiguration;

  /// <summary>Entry datei.</summary>
  [Builder.Object]
  private readonly Entry datei;

  /// <summary>Label datum20.</summary>
  [Builder.Object]
  private readonly Label datum20;

  /// <summary>Date Datum2.</summary>
  //// [Builder.Object]
  private readonly Date datum2;

  /// <summary>Date Datum3.</summary>
  //// [Builder.Object]
  private readonly Date datum3;

  /// <summary>Date Datum4.</summary>
  //// [Builder.Object]
  private readonly Date datum4;

  /// <summary>Label wertpapier0.</summary>
  [Builder.Object]
  private readonly ScrolledWindow wertpapiersw;

  /// <summary>Label wertpapier0.</summary>
  [Builder.Object]
  private readonly Label wertpapier0;

  /// <summary>TreeView wertpapier.</summary>
  [Builder.Object]
  private readonly TreeView wertpapier;

  /// <summary>Label datei20.</summary>
  [Builder.Object]
  private readonly Label datei20;

  /// <summary>Entry datei2.</summary>
  [Builder.Object]
  private readonly Entry datei2;

  /// <summary>Button datei2Auswahl.</summary>
  [Builder.Object]
  private readonly Button datei2Auswahl;

  /// <summary>Button export2.</summary>
  [Builder.Object]
  private readonly Button export2;

  /// <summary>Entry statustext.</summary>
  [Builder.Object]
  private readonly Entry statustext;

#pragma warning restore CS0649

  /// <summary>State of calculation.</summary>
  private readonly StringBuilder state = new();

  /// <summary>Cancel of calculation.</summary>
  private readonly StringBuilder cancel = new();

  /// <summary>Initializes a new instance of the <see cref="WP220Interface"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP220Interface(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(WP220Interface), dt, p1, p)
  {
    datum = new Date(Builder.GetObject("datum").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    datum.DateChanged += OnDatumDateChanged;
    datum.Show();
    datum2 = new Date(Builder.GetObject("datum2").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    datum2.DateChanged += OnDatum2DateChanged;
    datum2.Show();
    datum3 = new Date(Builder.GetObject("datum3").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    datum3.DateChanged += OnDatum3DateChanged;
    datum3.Show();
    datum4 = new Date(Builder.GetObject("datum4").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    datum4.DateChanged += OnDatum4DateChanged;
    datum4.Show();
    //// SetBold(client0);
    InitData(0);
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP220Interface Create(object p1 = null, CsbpBin p = null)
  {
    return new WP220Interface(GetBuilder("WP220Interface", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      var daten = ServiceDaten;
      var today = daten.Heute;
      datum.Value = Functions.Workday(today);
      SetText(anzahl, "5");
      SetText(bezeichnung, "%%");
      datum2.Value = Functions.Workday(new DateTime(today.Year - 1, 1, 1));
      datum3.Value = Functions.Workday(new DateTime(today.Year, 1, 1));
      datum4.Value = Functions.Workday(today);
      var clist = Get(FactoryService.StockService.GetConfigurationList(ServiceDaten, true, "1"));
      var cvalues = new List<string[]>
      {
        // No.;Description;St.;Changed at;Changed by;Created at;Created by
        new string[] { "", "", "", "", "" }, // Empty entry.
      };
      foreach (var c in clist)
      {
        cvalues.Add(new string[]
        {
          c.Uid, c.Bezeichnung, Functions.ToString(c.Geaendert_Am, true), c.Geaendert_Von,
          Functions.ToString(c.Angelegt_Am, true), c.Angelegt_Von,
        });
      }
      AddStringColumnsSort(konfiguration, WP220_konfiguration_columns, cvalues);
      SetText(konfiguration, Parameter.WP220Configuration);
      SetText(datei, Parameter.WP220File);
      var slist = Get(FactoryService.StockService.GetStockList(ServiceDaten, true));
      var svalues = new List<string[]>
        {
          // No.;Description;St.;Changed at;Changed by;Created at;Created by
          new string[] { "", "", "", "", "" }, // Empty entry.
        };
      foreach (var s in slist)
      {
        svalues.Add(new string[]
        {
          s.Uid, s.Bezeichnung, Functions.ToString(s.Geaendert_Am, true), s.Geaendert_Von,
          Functions.ToString(s.Angelegt_Am, true), s.Angelegt_Von,
        });
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
      SetText(datei2, Parameter.WP220File2);
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

  /// <summary>Handles datum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Konfiguration.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKonfigurationRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles Dateiauswahl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateiauswahlClicked(object sender, EventArgs e)
  {
    var file = SelectFile(string.IsNullOrEmpty(datei.Text) ? WP220_select_file : datei.Text, "*.csv", WP220_select_ext);
    if (!string.IsNullOrEmpty(file))
    {
      SetText(datei, file);
      Parameter.WP220File = file;
    }
  }

  /// <summary>Handles Export.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnExportClicked(object sender, EventArgs e)
  {
    ExportStocks();
  }

  /// <summary>Handles datum2.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatum2DateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles datum3.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatum3DateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles datum4.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatum4DateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Wertpapier.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWertpapierRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles Datei2auswahl.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatei2auswahlClicked(object sender, EventArgs e)
  {
    var file = SelectFile(string.IsNullOrEmpty(datei2.Text) ? WP220_select2_file : datei2.Text, "*.xls", WP220_select2_ext);
    if (!string.IsNullOrEmpty(file))
    {
      SetText(datei2, file);
      Parameter.WP220File2 = file;
    }
  }

  /// <summary>Handles Export2.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnExport2Clicked(object sender, EventArgs e)
  {
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    if (cancel.Length <= 0)
      cancel.Append("cancel");
    else
      CloseDialog();
  }

#pragma warning disable RECS0165 // Asynchrone Methoden sollten eine Aufgabe anstatt 'void' zurückgeben.
  private async void ExportStocks()
#pragma warning restore RECS0165
  {
    if (string.IsNullOrWhiteSpace(datei.Text))
      throw new MessageException(Message.New(M1012));
    try
    {
      state.Clear();
      cancel.Clear();
      var r = await Task.Run(() =>
      {
        var r0 = FactoryService.StockService.ExportStocks(ServiceDaten, bezeichnung.Text, null,
          null, null, false, GetText(konfiguration), datum.ValueNn, Functions.ToInt32(anzahl.Text), state, cancel);
        return r0;
      });
      r.ThrowAllErrors();
      if (cancel.Length <= 0)
        UiTools.SaveFile(r.Ergebnis, datei.Text);
      //// Application.Invoke((sender, e) => { refreshAction.Click(); });
    }
    catch (Exception ex)
    {
      Application.Invoke((sender, e) => { ShowError(ex.Message); });
    }
    finally
    {
      cancel.Append("End");
    }
  }
}
