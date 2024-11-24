// <copyright file="HH410Booking.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.HH;

using System;
using System.Collections.Generic;
using System.Text;
using CSBP.Forms.Controls;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for HH410Booking dialog.</summary>
public partial class HH410Booking : CsbpBin
{
  /// <summary>Last valuta.</summary>
  private static DateTime lastvaluta = DateTime.Today;

  /// <summary>Last receipt date.</summary>
  private static DateTime lastreceiptdate = DateTime.Today;

#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label valuta0.</summary>
  [Builder.Object]
  private readonly Label valuta0;

  /// <summary>Date Valuta.</summary>
  //// [Builder.Object]
  private readonly Date valuta;

  /// <summary>Label betrag0.</summary>
  [Builder.Object]
  private readonly Label betrag0;

  /// <summary>Entry betrag.</summary>
  [Builder.Object]
  private readonly Entry betrag;

  /// <summary>Entry summe.</summary>
  [Builder.Object]
  private readonly Entry summe;

  /// <summary>TreeView ereignis.</summary>
  [Builder.Object]
  private readonly TreeView ereignis;

  /// <summary>Label sollkonto0.</summary>
  [Builder.Object]
  private readonly Label sollkonto0;

  /// <summary>TreeView sollkonto.</summary>
  [Builder.Object]
  private readonly TreeView sollkonto;

  /// <summary>Label habenkonto0.</summary>
  [Builder.Object]
  private readonly Label habenkonto0;

  /// <summary>TreeView habenkonto.</summary>
  [Builder.Object]
  private readonly TreeView habenkonto;

  /// <summary>Label bText0.</summary>
  [Builder.Object]
  private readonly Label bText0;

  /// <summary>Entry bText.</summary>
  [Builder.Object]
  private readonly Entry bText;

  /// <summary>Entry belegNr.</summary>
  [Builder.Object]
  private readonly Entry belegNr;

  /// <summary>Button neueNr.</summary>
  [Builder.Object]
  private readonly Button neueNr;

  /// <summary>Date Belegdatum.</summary>
  //// [Builder.Object]
  private readonly Date belegDatum;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Entry buchung.</summary>
  [Builder.Object]
  private readonly Entry buchung;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

  /// <summary>Button kontentausch.</summary>
  [Builder.Object]
  private readonly Button kontentausch;

  /// <summary>Button addition.</summary>
  [Builder.Object]
  private readonly Button addition;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private HhBuchung model;

  /// <summary>Initializes a new instance of the <see cref="HH410Booking"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public HH410Booking(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(HH410Booking), dt, p1, p)
  {
    valuta = new Date(Builder.GetObject("valuta").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = true,
      YesterdayAccel = "m",
      TomorrowAccel = "p",
    };
    valuta.DateChanged += OnValutaDateChanged;
    valuta.Show();
    belegDatum = new Date(Builder.GetObject("belegDatum").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    belegDatum.DateChanged += OnBelegdatumDateChanged;
    belegDatum.Show();
    SetBold(valuta0);
    SetBold(betrag0);
    SetBold(sollkonto0);
    SetBold(habenkonto0);
    SetBold(bText0);
    InitData(0);
    betrag.GrabFocus();
    betrag0.SelectRegion(0, -1);
  }

  /// <summary>Gets or sets last copied uid.</summary>
  public static string Lastcopyuid { get; set; }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static HH410Booking Create(object p1 = null, CsbpBin p = null)
  {
    return new HH410Booking(GetBuilder("HH410Booking", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      valuta.Value = lastvaluta;
      belegDatum.Value = lastreceiptdate;
      var init = false;
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete || DialogType == DialogTypeEnum.Reverse;
      var aendern = DialogType == DialogTypeEnum.Edit;
      if (!neu && Parameter1 is string uid)
      {
        var k = Get(FactoryService.BudgetService.GetBooking(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) =>
          {
            CloseDialog();
          });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        valuta.Value = k.Soll_Valuta;
        InitLists();
        init = true;
        SetText(betrag, Functions.ToString(k.EBetrag, 2));
        SetText(summe, "");
        SetText(sollkonto, k.Soll_Konto_Uid);
        SetText(habenkonto, k.Haben_Konto_Uid);
        SetText(bText, k.BText);
        SetText(belegNr, k.Beleg_Nr);
        belegDatum.Value = k.Beleg_Datum;
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
      }
      if (!init)
        InitLists();
      ereignis.Selection.UnselectAll();
      var ee = GetText(ereignis);
      nr.IsEditable = false;
      valuta.Sensitive = !loeschen;
      betrag.IsEditable = !loeschen;
      summe.IsEditable = false;
      ereignis.Sensitive = !loeschen;
      sollkonto.Sensitive = !loeschen;
      habenkonto.Sensitive = !loeschen;
      bText.IsEditable = !loeschen;
      belegNr.IsEditable = !loeschen;
      belegDatum.Sensitive = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      buchung.IsEditable = false;
      if (neu)
        ok.Label = Forms_oknew;
      else if (loeschen)
      {
        if (DialogType == DialogTypeEnum.Delete)
          ok.Label = Forms_delete;
        else if (model != null && model.Kz == Constants.KZB_STORNO)
          ok.Label = Forms_unreverse;
        else
          ok.Label = Forms_reverse;
        neueNr.Sensitive = false;
        kontentausch.Sensitive = false;
        addition.Sensitive = false;
      }
      EventsActive = true;
    }
  }

  /// <summary>Handles valuta.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnValutaDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    try
    {
      EventsActive = false;
      belegDatum.Value = valuta.Value;
      InitLists();
    }
    finally
    {
      EventsActive = true;
    }
  }

  /// <summary>Handles Ereignis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEreignisCursorChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    var euid = GetText(ereignis);
    var k = Get(FactoryService.BudgetService.GetEvent(ServiceDaten, euid));
    if (k != null)
    {
      SetText(sollkonto, k.Soll_Konto_Uid);
      SetText(habenkonto, k.Haben_Konto_Uid);
      SetText(bText, k.EText);
    }
  }

  /// <summary>Handles Sollkonto.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSollkontoRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles Habenkonto.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHabenkontoRowActivated(object sender, RowActivatedArgs e)
  {
  }

  /// <summary>Handles Neuenr.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNeuenrClicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(belegNr.Text))
    {
      var no = Get(FactoryService.BudgetService.GetNewReceipt(ServiceDaten, belegDatum.ValueNn));
      SetText(belegNr, no);
    }
  }

  /// <summary>Handles belegDatum.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBelegdatumDateChanged(object sender, DateChangedEventArgs e)
  {
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    ServiceErgebnis r = null;
    var v = CalculateValue();
    var vdm = Functions.KonvDM(v);
    if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
      || DialogType == DialogTypeEnum.Edit)
    {
      var rb = FactoryService.BudgetService.SaveBooking(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, valuta.ValueNn, vdm, v,
        GetText(sollkonto), GetText(habenkonto), bText.Text, belegNr.Text, belegDatum.ValueNn);
      r = rb;
      if (rb.Ok && rb.Ergebnis != null && DialogType == DialogTypeEnum.Copy)
        Lastcopyuid = rb.Ergebnis.Uid;
    }
    else if (DialogType == DialogTypeEnum.Reverse)
      r = FactoryService.BudgetService.ReverseBooking(ServiceDaten, model);
    else if (DialogType == DialogTypeEnum.Delete)
      r = FactoryService.BudgetService.DeleteBooking(ServiceDaten, model);
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        lastvaluta = valuta.ValueNn;
        lastreceiptdate = belegDatum.ValueNn;
        UpdateParent();
        if (DialogType == DialogTypeEnum.New)
        {
          var sb = new StringBuilder();
          sb.Append(HH057(valuta.ValueNn, v,
            GetText(sollkonto, false, 1), GetText(habenkonto, false, 1), bText.Text));
          if (!string.IsNullOrEmpty(belegNr.Text))
            sb.Append(HH058(belegNr.Text));
          if (valuta.ValueNn != belegDatum.ValueNn)
            sb.Append(HH059(belegDatum.ValueNn));
          SetText(buchung, sb.ToString());
          SetText(betrag, "");
          SetText(summe, "");
          if (!string.IsNullOrEmpty(belegNr.Text))
            SetText(belegNr, Functions.ToString(Functions.ToInt64(belegNr.Text) + 1));
          betrag.GrabFocus();
        }
        else
          CloseDialog();
      }
    }
  }

  /// <summary>Handles Kontentausch.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnKontentauschClicked(object sender, EventArgs e)
  {
    var s = GetText(sollkonto);
    var h = GetText(habenkonto);
    SetText(sollkonto, h);
    SetText(habenkonto, s);
  }

  /// <summary>Handles Addition.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAdditionClicked(object sender, EventArgs e)
  {
    SetText(summe, Functions.ToString(CalculateValue(), 2));
    SetText(betrag, "");
    betrag.GrabFocus();
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }

  /// <summary>Initialisierung der Listen.</summary>
  private void InitLists()
  {
    var ev = GetText(ereignis);
    var s = GetText(sollkonto);
    var h = GetText(habenkonto);
    var el = Get(FactoryService.BudgetService.GetEventList(ServiceDaten, null, valuta.Value, valuta.Value));
    var evalues = new List<string[]>
    {
      new string[] { "", "" }, // Empty entry for initially no event.
    };
    foreach (var e in el)
    {
      evalues.Add(new string[] { e.Uid, e.Bezeichnung });
    }
    AddStringColumnsSort(ereignis, HH410_ereignis_columns, evalues);
    var kl = Get(FactoryService.BudgetService.GetAccountList(ServiceDaten, null, valuta.Value, valuta.Value));
    //// No.;Description
    var avalues = new List<string[]>();
    foreach (var e in kl)
    {
      avalues.Add(new string[] { e.Uid, e.Name });
    }
    AddStringColumnsSort(sollkonto, HH410_sollkonto_columns, avalues);
    AddStringColumnsSort(habenkonto, HH410_habenkonto_columns, avalues);
    SetText(ereignis, ev);
    SetText(sollkonto, s);
    SetText(habenkonto, h);
  }

  /// <summary>Berechnet den Betrag aus Einzelbetrag, Summe und Operator.</summary>
  /// <returns>Berechneter Betrag.</returns>
  private decimal CalculateValue()
  {
    var op = GetOperator();
    var b = Functions.ToDecimal(betrag.Text) ?? 0;
    var d = Functions.ToDecimal(summe.Text) ?? 0;
    if (string.IsNullOrEmpty(op))
      d += b;
    else if (op == "*")
      d *= b;
    else if (b != 0)
      d /= b; // operator.equals("/")
    d = Functions.Round(d) ?? 0;
    return d;
  }

  /// <summary>Liefert den Rechen-Operator und entfernt ihn aus dem Betrag.</summary>
  /// <returns>Bestimmter Rechen-Operator.</returns>
  private string GetOperator()
  {
    var op = "";
    var strBetrag = Functions.ToString(betrag.Text).Trim();
    if (strBetrag.StartsWith("+", StringComparison.InvariantCulture))
    {
      strBetrag = strBetrag[1..];
    }
    else if (strBetrag.StartsWith("*", StringComparison.InvariantCulture))
    {
      op = "*";
      strBetrag = strBetrag[1..];
    }
    else if (strBetrag.StartsWith("/", StringComparison.InvariantCulture))
    {
      op = "/";
      strBetrag = strBetrag[1..];
    }
    SetText(betrag, strBetrag);
    return op;
  }
}
