// <copyright file="FZ100Statistics.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Services.Apis.Enums;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for FZ100Statistics dialog.</summary>
public partial class FZ100Statistics : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button RefreshAction.</summary>
  [Builder.Object]
  private readonly Button refreshAction;

  /// <summary>Paned splitpane.</summary>
  [Builder.Object]
  private readonly Paned splitpane;

  /// <summary>Date Datum.</summary>
  ////[Builder.Object]
  private readonly Date datum;

  /// <summary>TextView bilanz.</summary>
  [Builder.Object]
  private readonly TextView bilanz;

  /// <summary>TextView buecher.</summary>
  [Builder.Object]
  private readonly TextView buecher;

  /// <summary>TextView fahrrad.</summary>
  [Builder.Object]
  private readonly TextView fahrrad;

#pragma warning restore CS0649

  /// <summary>Diagram model proprietary.</summary>
  private List<KeyValuePair<string, decimal>> pplList;

  /// <summary>Diagram model bike mileages.</summary>
  private List<KeyValuePair<string, decimal>> mileageList;

  /// <summary>Initializes a new instance of the <see cref="FZ100Statistics"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public FZ100Statistics(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(FZ100Statistics), dt, p1, p)
  {
    datum = new Date(Builder.GetObject("datum").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    datum.DateChanged += OnDatumDateChanged;
    datum.Show();
#pragma warning disable CS0612
    bilanz.OverrideFont(Pango.FontDescription.FromString("mono"));
    buecher.OverrideFont(Pango.FontDescription.FromString("mono"));
    fahrrad.OverrideFont(Pango.FontDescription.FromString("mono"));
#pragma warning restore CS0612
    //// SetBold(client0);
    InitData(0);
    bilanz.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static FZ100Statistics Create(object p1 = null, CsbpBin p = null)
  {
    return new FZ100Statistics(GetBuilder("FZ100Statistics", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      datum.Value = ServiceDaten.Heute;
      EventsActive = true;
    }
    if (step <= 1)
    {
      var str = Get(FactoryService.PrivateService.GetStatistics(daten, 1, datum.ValueNn));
      SetText(bilanz, str);
      str = Get(FactoryService.PrivateService.GetStatistics(daten, 2, datum.ValueNn));
      SetText(buecher, str);
      str = Get(FactoryService.PrivateService.GetStatistics(daten, 3, datum.ValueNn));
      SetText(fahrrad, str);

      var l = Get(FactoryService.BudgetService.GetProprietaryPlList(daten, datum.ValueNn));
      l.Reverse();
      pplList = l.Select(a => new KeyValuePair<string, decimal>(Functions.ToString(a.Geaendert_Am), a.EBetrag)).ToList();
      var l2 = Get(FactoryService.PrivateService.GetMileages(daten, datum.ValueNn));
      mileageList = l2.Select(a => new KeyValuePair<string, decimal>(Functions.ToString(a.Datum), a.Periode_km)).ToList();
    }
    if (step <= 0)
    {
      Application.Invoke((sender, e) =>
      {
        System.Threading.Thread.Sleep(1000);
        var s = MainClass.MainWindow.Allocation;
        var p = s.Width * 2 / 5; // 40 %
        splitpane.Position = Math.Max(p, 600);
      });
    }
  }

  /// <summary>Handles Refresh.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRefreshClicked(object sender, EventArgs e)
  {
    InitData(1);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
      refreshAction.Click();
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
      refreshAction.Click();
  }

  /// <summary>Handles datum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    refreshAction.Click();
  }

  /// <summary>Handles Diagramm.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagramDraw(object sender, DrawnArgs e)
  {
    var da = sender as DrawingArea;
    var c = e.Cr;
    var w = da.Window.Width;
    var h = da.Window.Height;
    //// System.Diagnostics.Debug.WriteLine($"{w} {h}");
    //// c.SetSourceRGBA(1, 0, 0, 1);
    //// c.LineWidth = 1;
    //// c.MoveTo(0, 0);
    //// c.LineTo(0, h);
    //// c.LineTo(w, h);
    //// c.LineTo(w, 0);
    //// c.LineTo(0, 0);
    //// c.Stroke();
    //// var d = Chart.ComputeDimension(w, h);
    Diagram.Draw(M0(HH001), pplList, c, 0, 0, w, h / 2);
    Diagram.Draw(M0(FZ044), mileageList, c, 0, h / 2, w, h / 2);
  }
}
