// <copyright file="WP510Price.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP510Price Dialog.</summary>
  public partial class WP510Price : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private WpStand Model;

    /// <summary>Letztes Valuta merken.</summary>
    private static DateTime LastValuta = DateTime.Today;

#pragma warning disable 169, 649

    /// <summary>Label wertpapier0.</summary>
    [Builder.Object]
    private Label wertpapier0;

    /// <summary>ComboBox wertpapier.</summary>
    [Builder.Object]
    private ComboBox wertpapier;

    /// <summary>Label valuta0.</summary>
    [Builder.Object]
    private Label valuta0;

    /// <summary>Date Valuta.</summary>
    //[Builder.Object]
    private Date valuta;

    /// <summary>Label betrag0.</summary>
    [Builder.Object]
    private Label betrag0;

    /// <summary>Entry betrag.</summary>
    [Builder.Object]
    private Entry betrag;

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
    public static WP510Price Create(object p1 = null, CsbpBin p = null)
    {
      return new WP510Price(GetBuilder("WP510Price", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP510Price(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      valuta = new Date(Builder.GetObject("valuta").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      valuta.DateChanged += OnValutaDateChanged;
      valuta.Show();
      SetBold(wertpapier0);
      SetBold(valuta0);
      SetBold(betrag0);
      InitData(0);
      betrag.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        valuta.Value = LastValuta;
        var rl = Get(FactoryService.StockService.GetStockList(daten, true)) ?? new List<WpWertpapier>();
        var rs = AddColumns(wertpapier);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var copy = DialogType == DialogTypeEnum.Copy;
        var key = Parameter1 as Tuple<string, DateTime?>;
        if (!neu && key != null && key.Item2.HasValue)
        {
          var k = Get(FactoryService.StockService.GetPrice(ServiceDaten, key.Item1, key.Item2.Value));
          if (k == null)
          {
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
          SetText(wertpapier, k.Wertpapier_Uid);
          valuta.Value = k.Datum;
          betrag.Text = Functions.ToString(k.Stueckpreis, 4);
          angelegt.Text = k.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = k.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        wertpapier.Sensitive = neu || copy;
        valuta.Sensitive = neu || copy;
        betrag.IsEditable = !loeschen;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        if (loeschen)
          ok.Label = Forms_delete;
      }
    }

    /// <summary>Behandlung von Wertpapier.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnWertpapierChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von valuta.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnValutaDateChanged(object sender, DateChangedEventArgs e)
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
        r = FactoryService.StockService.SavePrice(ServiceDaten,
            GetText(wertpapier), valuta.Value ?? LastValuta,
            Functions.ToDecimal(betrag.Text, 4) ?? 0);
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.StockService.DeletePrice(ServiceDaten, Model);
      }
      if (r != null)
      {
        Get(r);
        if (r.Ok)
        {
          // letztes Datum merken
          LastValuta = valuta.Value.HasValue ? valuta.Value.Value : LastValuta;
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
