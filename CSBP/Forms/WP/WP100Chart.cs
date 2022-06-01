// <copyright file="WP100Chart.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using CSBP.Services.Pnf;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP100Chart Dialog.</summary>
  public partial class WP100Chart : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private Tuple<DateTime?, string, string> Model;

    /// <summary>Chart Model.</summary>
    private PnfChart Chart;

#pragma warning disable CS0649

    /// <summary>Button RefreshAction.</summary>
    [Builder.Object]
    private readonly Button refreshAction;

    /// <summary>TreeView data.</summary>
    [Builder.Object]
    private readonly TreeView data;

    /// <summary>Date Von.</summary>
    //[Builder.Object]
    private readonly Date von;

    /// <summary>Date Bis.</summary>
    //[Builder.Object]
    private readonly Date bis;

    /// <summary>Label wertpapier0.</summary>
    [Builder.Object]
    private readonly Label wertpapier0;

    /// <summary>ComboBox wertpapier.</summary>
    [Builder.Object]
    private readonly ComboBox wertpapier;

    /// <summary>Entry box.</summary>
    [Builder.Object]
    private readonly Entry box;

    /// <summary>ComboBox skala.</summary>
    [Builder.Object]
    private readonly ComboBox skala;

    /// <summary>Entry umkehr.</summary>
    [Builder.Object]
    private readonly Entry umkehr;

    /// <summary>Label methode0.</summary>
    [Builder.Object]
    private readonly Label methode0;

    /// <summary>ComboBox methode.</summary>
    [Builder.Object]
    private readonly ComboBox methode;

    /// <summary>CheckButton relativ.</summary>
    [Builder.Object]
    private readonly CheckButton relativ;

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP100Chart Create(object p1 = null, CsbpBin p = null)
    {
      return new WP100Chart(GetBuilder("WP100Chart", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP100Chart(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
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
      //chartpane.Draw += ChartpaneDraw;
      SetBold(wertpapier0);
      SetBold(methode0);
      InitData(0);
      data.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        if (Model == null)
          Model = Parameter1 as Tuple<DateTime?, string, string>;
        var rl = Get(FactoryService.StockService.GetStockList(daten, true)) ?? new List<WpWertpapier>();
        var rs = AddColumns(wertpapier);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        AddColumns(skala, Get(FactoryService.StockService.GetScaleList(daten)));
        AddColumns(methode, Get(FactoryService.StockService.GetMethodList(daten)));
        var d = Model != null && Model.Item1.HasValue ? Model.Item1.Value : daten.Heute;
        bis.Value = d;
        var k = Get(FactoryService.StockService.GetConfiguration(daten, Model?.Item3, true));
        von.Value = d.AddDays(-k.Duration);
        if (Model != null)
        {
          SetText(wertpapier, Model.Item2);
        }
        box.Text = Functions.ToString(k.Box);
        SetText(skala, k.Scale.ToString());
        umkehr.Text = k.Reversal.ToString();
        SetText(methode, k.Method.ToString());
        relativ.Active = k.Relative;
      }
      if (step <= 1)
      {
        var wpuid = GetText(wertpapier);
        var l = Get(FactoryService.StockService.GetPriceList(ServiceDaten, von.Value ?? daten.Heute,
          bis.Value ?? daten.Heute, wpuid, relativ.Active));
        // Datum;Open;High;Low;Close
        var values = new List<string[]>();
        if (l != null)
        {
          foreach (var e in l)
          {
            values.Add(new string[] { Functions.ToString(e.Datum), Functions.ToString(e.Datum), Functions.ToString(e.Open),
              Functions.ToString(e.High), Functions.ToString(e.Low), Functions.ToString(e.Close) });
          }
        }
        AddStringColumnsSort(data, WP100_daten_columns, values);
        var wp = Get(FactoryService.StockService.GetStock(ServiceDaten, wpuid));
        if (wp != null)
        {
          Chart = new PnfChart(Functions.ToInt32(GetText(methode)), Functions.ToDecimal(box.Text) ?? 0,
            Functions.ToInt32(GetText(skala)), Functions.ToInt32(umkehr.Text), wp.Bezeichnung);
          Chart.AddKurse(l);
        }
        // chartpane.QueueDraw();
      }
    }

    /// <summary>Behandlung von Refresh.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRefreshClicked(object sender, EventArgs e)
    {
      RefreshTreeView(data, 1);
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
      if (MainClass.Undo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von Redo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRedoClicked(object sender, EventArgs e)
    {
      if (MainClass.Redo())
        refreshAction.Click();
    }

    /// <summary>Behandlung von Daten.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatenRowActivated(object sender, RowActivatedArgs e)
    {
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

    /// <summary>Behandlung von Wertpapier.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnWertpapierChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Skala.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSkalaChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Methode.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMethodeChanged(object sender, EventArgs e)
    {
    }

    // protected void OnChartpaneExposeEvent(object o, ExposeEventArgs args)
    // {
    //   var win = args.Event.Window; // chartpane.GdkWindow;
    //   var area = args.Event.Area; // new Rectangle(0, 0, 100, 200);
    //   area.Width -= 1;
    //   area.Height -= 1;
    //   var d = Chart.computeDimension(area.Width, area.Height);
    //   ChartPane.paintChart(Chart, win, PangoContext, Style);
    // }

    // private Pango.Layout GetLayout(string text)
    // {
    //   Pango.Layout layout = new Pango.Layout(PangoContext);
    //   layout.FontDescription = Pango.FontDescription.FromString("monospace 8");
    //   layout.SetMarkup($"<span color='black'>{text}</span>");
    //   return layout;
    // }

    /// <summary>Behandlung von Methode.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnChartpaneDraw(object sender, DrawnArgs e)
    {
      var da = sender as DrawingArea;
      var c = e.Cr;
      var w = da.Window.Width;
      var h = da.Window.Height;
      // System.Diagnostics.Debug.WriteLine($"{w} {h}");
      // c.SetSourceRGBA(1, 0, 0, 1);
      // c.LineWidth = 1;
      // c.MoveTo(0, 0);
      // c.LineTo(0, h);
      // c.LineTo(w, h);
      // c.LineTo(w, 0);
      // c.LineTo(0, 0);
      // c.Stroke();
      // var d = Chart.ComputeDimension(w, h);
      ChartPane.DrawChart(Chart, c, w, h);
    }
  }
}
