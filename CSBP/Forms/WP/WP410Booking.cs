// <copyright file="WP410Booking.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Forms.Controls;
using CSBP.Forms.HH;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for WP410Booking dialog.</summary>
public partial class WP410Booking : CsbpBin
{
  /// <summary>Last valuta.</summary>
  private static DateTime lastvaluta = DateTime.Today;

  /// <summary>Last copied ID.</summary>
  private static string lastcopyuid = null;

#pragma warning disable CS0649

  /// <summary>Entry nr.</summary>
  [Builder.Object]
  private readonly Entry nr;

  /// <summary>Label anlage0.</summary>
  [Builder.Object]
  private readonly Label anlage0;

  /// <summary>ComboBox anlage.</summary>
  [Builder.Object]
  private readonly ComboBox anlage;

  /// <summary>Label valuta0.</summary>
  [Builder.Object]
  private readonly Label valuta0;

  /// <summary>Date Valuta.</summary>
  //// [Builder.Object]
  private readonly Date valuta;

  /// <summary>Entry preis.</summary>
  [Builder.Object]
  private readonly Entry preis;

  /// <summary>Entry betrag.</summary>
  [Builder.Object]
  private readonly Entry betrag;

  /// <summary>Entry rabatt.</summary>
  [Builder.Object]
  private readonly Entry rabatt;

  /// <summary>Entry anteile.</summary>
  [Builder.Object]
  private readonly Entry anteile;

  /// <summary>Entry preis2.</summary>
  [Builder.Object]
  private readonly Entry preis2;

  /// <summary>Entry zinsen.</summary>
  [Builder.Object]
  private readonly Entry zinsen;

  /// <summary>Label bText0.</summary>
  [Builder.Object]
  private readonly Label bText0;

  /// <summary>Entry bText.</summary>
  [Builder.Object]
  private readonly Entry bText;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Entry buchung.</summary>
  [Builder.Object]
  private readonly Entry buchung;

  /// <summary>Entry hhbuchung.</summary>
  [Builder.Object]
  private readonly Entry hhbuchung;

  /// <summary>Button hhaendern.</summary>
  [Builder.Object]
  private readonly Button hhaendern;

  /// <summary>Button hhstorno.</summary>
  [Builder.Object]
  private readonly Button hhstorno;

  /// <summary>Date hhvaluta.</summary>
  //// [Builder.Object]
  private readonly Date hhvaluta;

  /// <summary>Entry hhbetrag.</summary>
  [Builder.Object]
  private readonly Entry hhbetrag;

  /// <summary>TreeView hhereignis.</summary>
  [Builder.Object]
  private readonly TreeView hhereignis;

  /// <summary>Button ok.</summary>
  [Builder.Object]
  private readonly Button ok;

  /// <summary>Letzte Anlagen-ID.</summary>
  private string invuid;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private WpBuchung model;

  /// <summary>Aktuelle Ereignisse zur Anlage.</summary>
  private List<HhEreignis> events;

  /// <summary>Initializes a new instance of the <see cref="WP410Booking"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public WP410Booking(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
    : base(b, h, d, type ?? typeof(WP410Booking), dt, p1, p)
  {
    valuta = new Date(Builder.GetObject("valuta").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    valuta.DateChanged += OnValutaDateChanged;
    valuta.Show();
    SetBold(anlage0);
    SetBold(valuta0);
    SetBold(bText0);
    hhvaluta = new Date(Builder.GetObject("hhvaluta").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
    };
    //// hhvaluta.DateChanged += OnValutaDateChanged;
    hhvaluta.Show();
    InitData(0);
    betrag.GrabFocus();
  }

  /// <summary>Gets or sets last copied ID.</summary>
  public static string Lastcopyuid { get => lastcopyuid; set => lastcopyuid = value; }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static WP410Booking Create(object p1 = null, CsbpBin p = null)
  {
    return new WP410Booking(GetBuilder("WP410Booking", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    var daten = ServiceDaten;
    if (step <= 0)
    {
      EventsActive = false;
      valuta.Value = lastvaluta;
      hhvaluta.Value = lastvaluta;
      var rl = Get(FactoryService.StockService.GetInvestmentList(daten, true)) ?? new List<WpAnlage>();
      var rs = AddColumns(anlage);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      var neu = DialogType == DialogTypeEnum.New;
      var loeschen = DialogType == DialogTypeEnum.Delete || DialogType == DialogTypeEnum.Reverse;
      var kopieren = DialogType == DialogTypeEnum.Copy;
      var uid = Parameter1 as string;
      if (neu && uid != null)
      {
        SetText(anlage, uid);
        EventList(uid);
      }
      if (!neu && uid != null)
      {
        var k = Get(FactoryService.StockService.GetBooking(ServiceDaten, uid));
        if (k == null)
        {
          Application.Invoke((sender, e) => { CloseDialog(); });
          return;
        }
        model = k;
        SetText(nr, k.Uid);
        SetText(anlage, k.Anlage_Uid);
        valuta.Value = k.Datum;
        SetText(preis, Functions.ToString(k.Price, 4));
        SetText(betrag, Functions.ToString(k.Zahlungsbetrag, 2));
        SetText(rabatt, Functions.ToString(k.Rabattbetrag, 2));
        SetText(anteile, Functions.ToString(k.Anteile, 5));
        SetText(zinsen, Functions.ToString(k.Zinsen, 2));
        SetText(bText, k.BText);
        SetText(angelegt, ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von));
        SetText(geaendert, ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von));
        //// k.BookingUid = "6acdd0a1:16fd83aa41e:-7fed";
        if (kopieren)
          k.BookingUid = null;
        SetText(hhbuchung, k.BookingUid);
        hhvaluta.Value = k.Datum;
        EventList(k.Anlage_Uid);
        if (!string.IsNullOrEmpty(k.BookingUid))
        {
          var b = Get(FactoryService.BudgetService.GetBooking(ServiceDaten, k.BookingUid));
          if (b != null)
          {
            hhvaluta.Value = b.Soll_Valuta;
            SetText(hhbetrag, Functions.ToString(b.EBetrag, 2));
            if (events != null)
            {
              var ev = events.FirstOrDefault(a => a.Soll_Konto_Uid == b.Soll_Konto_Uid && a.Haben_Konto_Uid == b.Haben_Konto_Uid);
              if (ev != null)
                SetText(hhereignis, ev.Uid);
            }
          }
        }
      }
      nr.IsEditable = false;
      anlage.Sensitive = !loeschen;
      valuta.Sensitive = !loeschen;
      preis.IsEditable = !loeschen;
      betrag.IsEditable = !loeschen;
      rabatt.IsEditable = !loeschen;
      anteile.IsEditable = !loeschen;
      preis2.IsEditable = false;
      zinsen.IsEditable = !loeschen;
      bText.IsEditable = !loeschen;
      angelegt.IsEditable = false;
      geaendert.IsEditable = false;
      buchung.IsEditable = false;
      hhbuchung.IsEditable = false;
      hhvaluta.Sensitive = !loeschen && string.IsNullOrEmpty(hhbuchung.Text);
      hhbetrag.IsEditable = !loeschen && string.IsNullOrEmpty(hhbuchung.Text);
      hhereignis.Sensitive = !loeschen && string.IsNullOrEmpty(hhbuchung.Text);
      hhaendern.Sensitive = !loeschen && !string.IsNullOrEmpty(hhbuchung.Text);
      hhstorno.Sensitive = !loeschen && !string.IsNullOrEmpty(hhbuchung.Text);
      if (neu)
        ok.Label = Forms_oknew;
      else if (loeschen)
        ok.Label = Forms_delete;
      EventsActive = true;
      OnShares();
    }
  }

  /// <summary>Handles Anlage.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAnlageChanged(object sender, EventArgs e)
  {
    OnValuta();
  }

  /// <summary>Handles valuta.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnValutaDateChanged(object sender, DateChangedEventArgs e)
  {
    OnValuta();
  }

  /// <summary>Handles Betrag.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBetragKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    OnShares();
  }

  /// <summary>Handles Rabatt.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRabattKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    OnShares();
  }

  /// <summary>Handles Anteile.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAnteileKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    OnShares();
  }

  /// <summary>Handles Zinsen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZinsenKeyReleaseEvent(object sender, KeyReleaseEventArgs e)
  {
    OnShares();
  }

  /// <summary>Handles Buchungstext.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBtextFocusInEvent(object sender, FocusInEventArgs e)
  {
    // Debug.WriteLine("OnBtextFocusInEvent");
  }

  /// <summary>Handles Ereignis.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHhEreignisCursorChanged(object sender, EventArgs e)
  {
    if (!EventsActive)
      return;
    if (string.IsNullOrEmpty(hhbuchung.Text))
    {
      var euid = GetText(hhereignis);
      var ev = events.FirstOrDefault(a => a.Uid == euid);
      var text = ev?.Bezeichnung;
      if (!string.IsNullOrEmpty(text))
      {
        SetText(bText, text);
        if (ev?.EText == "1")
        {
          // Shares without interests.
          SetText(zinsen, "");
        }
        else if (ev?.EText == "2")
        {
          // Interests without changes of shares.
          SetText(betrag, "");
          //// SetText(rabatt, "");
          SetText(anteile, "");
        }
      }
    }
  }

  /// <summary>Handles HhAendern.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHhAendernClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Edit);
  }

  /// <summary>Handles HhStorno.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnHhStornoClicked(object sender, EventArgs e)
  {
    StartDialog(DialogTypeEnum.Reverse);
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    ServiceErgebnis r = null;
    if (DialogType == DialogTypeEnum.New || DialogType == DialogTypeEnum.Copy
      || DialogType == DialogTypeEnum.Edit)
    {
      var euid = GetText(hhereignis);
      var ev = events.FirstOrDefault(a => a.Uid == euid);
      var booktext = bText.Text ?? ev?.Bezeichnung;
      var rb = FactoryService.StockService.SaveBooking(ServiceDaten,
        DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(anlage), valuta.ValueNn,
        Functions.ToDecimal(betrag.Text, 2) ?? 0, Functions.ToDecimal(rabatt.Text, 2) ?? 0,
        Functions.ToDecimal(anteile.Text, 5) ?? 0, Functions.ToDecimal(zinsen.Text, 2) ?? 0,
        bText.Text, null, Functions.ToDecimal(preis.Text, 4) ?? 0, hhbuchung.Text, hhvaluta.ValueNn,
        Functions.ToDecimal(hhbetrag.Text, 2) ?? 0, ev?.Soll_Konto_Uid, ev?.Haben_Konto_Uid, booktext);
      r = rb;
      if (rb.Ok && rb.Ergebnis != null && DialogType == DialogTypeEnum.Copy)
        Lastcopyuid = rb.Ergebnis.Uid;
    }
    else if (DialogType == DialogTypeEnum.Delete)
    {
      r = FactoryService.StockService.DeleteBooking(ServiceDaten, model);
    }
    if (r != null)
    {
      Get(r);
      if (r.Ok)
      {
        lastvaluta = valuta.ValueNn;
        UpdateParent();
        if (DialogType == DialogTypeEnum.New)
        {
          var desc = GetText(anlage, false);
          SetText(buchung, WP030(valuta.ValueNn, betrag.Text, rabatt.Text, anteile.Text, zinsen.Text, desc, bText.Text));
          SetText(betrag, "");
          SetText(rabatt, "");
          betrag.GrabFocus();
        }
        else
          CloseDialog();
      }
    }
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    CloseDialog();
  }

  /// <summary>Handles valuta.</summary>
  private void OnValuta()
  {
    if (!EventsActive)
      return;
    var daten = ServiceDaten;
    var inuid = GetText(anlage);
    var inv = string.IsNullOrWhiteSpace(inuid) ? null :
      Get(FactoryService.StockService.GetInvestment(daten, inuid));
    var p = inv == null || !valuta.Value.HasValue ? null :
      Get(FactoryService.StockService.GetPrice(daten, inv.Wertpapier_Uid, valuta.Value.Value));
    SetText(preis, Functions.ToString(p?.Stueckpreis, 4));
    if (string.IsNullOrEmpty(hhbuchung.Text))
      hhvaluta.Value = valuta.ValueNn;
    EventList(inuid);
  }

  /// <summary>Handles shares.</summary>
  private void OnShares()
  {
    if (!EventsActive)
      return;
    var p = Functions.ToDecimal(betrag.Text);
    var s = Functions.ToDecimal(anteile.Text);
    var z = Functions.ToDecimal(zinsen.Text) ?? 0;
    SetText(preis2, Functions.ToString(Functions.CompDouble4(p, 0) == 0 || Functions.CompDouble4(s, 0) == 0 ? null : p / s, 6));
    if (string.IsNullOrEmpty(hhbuchung.Text))
    {
      if (z == 0)
        SetText(hhbetrag, Functions.ToString(Math.Abs((p ?? 0) - (Functions.ToDecimal(rabatt.Text) ?? 0)), 2));
      else
        SetText(hhbetrag, Functions.ToString(Math.Abs(z + (Functions.ToDecimal(rabatt.Text) ?? 0)), 2));
    }
  }

  /// <summary>Starts the details dialog.</summary>
  /// <param name="dt">Affected dialog type.</param>
  private void StartDialog(DialogTypeEnum dt)
  {
    var uid = hhbuchung.Text;
    if (!string.IsNullOrEmpty(uid))
      Start(typeof(HH410Booking), HH410_title, dt, uid, csbpparent: this);
  }

  /// <summary>Handles event list.</summary>
  /// <param name="inuid">Affected investment uid.</param>
  private void EventList(string inuid)
  {
    if (invuid != inuid)
    {
      events = Get(FactoryService.StockService.GetEventList(ServiceDaten, inuid)) ?? new List<HhEreignis>();
      var values = new List<string[]>
        {
          new string[] { "", "" }, // Empty entry for initially no event.
        };
      foreach (var e in events)
      {
        // No.;Description
        values.Add(new string[] { e.Uid, e.Bezeichnung });
      }
      AddStringColumnsSort(hhereignis, WP410_hhereignis_columns, values);
      invuid = inuid;
    }
  }
}
