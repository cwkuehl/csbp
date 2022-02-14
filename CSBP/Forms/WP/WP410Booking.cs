// <copyright file="WP410Booking.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.WP
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Forms.HH;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für WP410Booking Dialog.</summary>
  public partial class WP410Booking : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private WpBuchung Model;

    /// <summary>Letztes Valuta merken.</summary>
    private static DateTime LastValuta = DateTime.Today;

#pragma warning disable 169, 649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Label anlage0.</summary>
    [Builder.Object]
    private Label anlage0;

    /// <summary>ComboBox anlage.</summary>
    [Builder.Object]
    private ComboBox anlage;

    /// <summary>Label valuta0.</summary>
    [Builder.Object]
    private Label valuta0;

    /// <summary>Date Valuta.</summary>
    //[Builder.Object]
    private Date valuta;

    /// <summary>Label preis0.</summary>
    [Builder.Object]
    private Label preis0;

    /// <summary>Entry preis.</summary>
    [Builder.Object]
    private Entry preis;

    /// <summary>Label betrag0.</summary>
    [Builder.Object]
    private Label betrag0;

    /// <summary>Entry betrag.</summary>
    [Builder.Object]
    private Entry betrag;

    /// <summary>Label rabatt0.</summary>
    [Builder.Object]
    private Label rabatt0;

    /// <summary>Entry rabatt.</summary>
    [Builder.Object]
    private Entry rabatt;

    /// <summary>Label anteile0.</summary>
    [Builder.Object]
    private Label anteile0;

    /// <summary>Entry anteile.</summary>
    [Builder.Object]
    private Entry anteile;

    /// <summary>Label preis20.</summary>
    [Builder.Object]
    private Label preis20;

    /// <summary>Entry preis2.</summary>
    [Builder.Object]
    private Entry preis2;

    /// <summary>Label zinsen0.</summary>
    [Builder.Object]
    private Label zinsen0;

    /// <summary>Entry zinsen.</summary>
    [Builder.Object]
    private Entry zinsen;

    /// <summary>Label bText0.</summary>
    [Builder.Object]
    private Label bText0;

    /// <summary>Entry bText.</summary>
    [Builder.Object]
    private Entry bText;

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

    /// <summary>Label buchung0.</summary>
    [Builder.Object]
    private Label buchung0;

    /// <summary>Entry buchung.</summary>
    [Builder.Object]
    private Entry buchung;

    /// <summary>Entry hhbuchung.</summary>
    [Builder.Object]
    private Entry hhbuchung;

    /// <summary>Button hhaendern.</summary>
    [Builder.Object]
    private Button hhaendern;

    /// <summary>Button hhstorno.</summary>
    [Builder.Object]
    private Button hhstorno;

    /// <summary>Date hhvaluta.</summary>
    //[Builder.Object]
    private Date hhvaluta;

    /// <summary>Entry hhbetrag.</summary>
    [Builder.Object]
    private Entry hhbetrag;

    /// <summary>TreeView hhereignis.</summary>
    [Builder.Object]
    private TreeView hhereignis;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

    /// <summary>Letzte Anlagen-ID.</summary>
    private string invuid;

    /// <summary>Aktuelle Ereignisse zur Anlage.</summary>
    private List<HhEreignis> events;

    /// <summary>Zuletzt kopiert ID.</summary>
    public static string lastcopyuid = null;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static WP410Booking Create(object p1 = null, CsbpBin p = null)
    {
      return new WP410Booking(GetBuilder("WP410Booking", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public WP410Booking(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
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
      SetBold(anlage0);
      SetBold(valuta0);
      SetBold(bText0);
      hhvaluta = new Date(Builder.GetObject("hhvaluta").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      // hhvaluta.DateChanged += OnValutaDateChanged;
      hhvaluta.Show();
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
        EventsActive = false;
        valuta.Value = LastValuta;
        hhvaluta.Value = LastValuta;
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
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
          nr.Text = k.Uid;
          SetText(anlage, k.Anlage_Uid);
          valuta.Value = k.Datum;
          preis.Text = Functions.ToString(k.Price, 4);
          betrag.Text = Functions.ToString(k.Zahlungsbetrag, 2);
          rabatt.Text = Functions.ToString(k.Rabattbetrag, 2);
          anteile.Text = Functions.ToString(k.Anteile, 5);
          zinsen.Text = Functions.ToString(k.Zinsen, 2);
          bText.Text = k.BText;
          angelegt.Text = k.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = k.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
          // k.BookingUid = "6acdd0a1:16fd83aa41e:-7fed";
          if (kopieren)
            k.BookingUid = null;
          hhbuchung.Text = k.BookingUid ?? "";
          hhvaluta.Value = k.Datum;
          EventList(k.Anlage_Uid);
          if (!string.IsNullOrEmpty(k.BookingUid))
          {
            var b = Get(FactoryService.BudgetService.GetBooking(ServiceDaten, k.BookingUid));
            if (b != null)
            {
              hhvaluta.Value = b.Soll_Valuta;
              hhbetrag.Text = Functions.ToString(b.EBetrag, 2);
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

    /// <summary>Behandlung von Anlage.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAnlageChanged(object sender, EventArgs e)
    {
      OnValuta();
    }

    /// <summary>Behandlung von valuta.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnValutaDateChanged(object sender, DateChangedEventArgs e)
    {
      OnValuta();
    }

    /// <summary>Behandlung von Betrag.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBetragKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      OnShares();
    }

    /// <summary>Behandlung von Rabatt.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRabattKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      OnShares();
    }

    /// <summary>Behandlung von Anteile.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAnteileKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      OnShares();
    }

    /// <summary>Behandlung von Zinsen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnZinsenKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      OnShares();
    }

    /// <summary>Behandlung von Buchungstext.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBtextFocusInEvent(object o, FocusInEventArgs e)
    {
      // Debug.WriteLine("OnBtextFocusInEvent");
    }

    /// <summary>Behandlung von Ereignis.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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
          bText.Text = text;
          if (ev?.EText == "1")
          {
            // Bestandsänderungen ohne Zinsen
            zinsen.Text = "";
          }
          else if (ev?.EText == "2")
          {
            // Zinsen ohne Bestandsänderungen
            betrag.Text = "";
            // rabatt.Text = "";
            anteile.Text = "";
          }
        }
      }
    }

    /// <summary>Behandlung von HhAendern.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHhAendernClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Edit);
    }

    /// <summary>Behandlung von HhStorno.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHhStornoClicked(object sender, EventArgs e)
    {
      StartDialog(DialogTypeEnum.Reverse);
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
          lastcopyuid = rb.Ergebnis.Uid;
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.StockService.DeleteBooking(ServiceDaten, Model);
      }
      if (r != null)
      {
        Get(r);
        if (r.Ok)
        {
          LastValuta = valuta.ValueNn;
          UpdateParent();
          if (DialogType == DialogTypeEnum.New)
          {
            var desc = GetText(anlage, false);
            buchung.Text = WP030(valuta.ValueNn, betrag.Text, rabatt.Text, anteile.Text,
              zinsen.Text, desc, bText.Text);
            betrag.Text = "";
            rabatt.Text = "";
            betrag.GrabFocus();
          }
          else
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
      preis.Text = Functions.ToString(p?.Stueckpreis, 4);
      if (string.IsNullOrEmpty(hhbuchung.Text))
        hhvaluta.Value = valuta.ValueNn;
      EventList(inuid);
    }

    private void OnShares()
    {
      if (!EventsActive)
        return;
      var p = Functions.ToDecimal(betrag.Text);
      var s = Functions.ToDecimal(anteile.Text);
      var z = Functions.ToDecimal(zinsen.Text) ?? 0;
      preis2.Text = Functions.ToString(Functions.compDouble4(p, 0) == 0
        || Functions.compDouble4(s, 0) == 0 ? null : p / s, 6);
      if (string.IsNullOrEmpty(hhbuchung.Text))
      {
        if (z == 0)
          hhbetrag.Text = Functions.ToString(Math.Abs((p ?? 0) - (Functions.ToDecimal(rabatt.Text) ?? 0)), 2);
        else
          hhbetrag.Text = Functions.ToString(Math.Abs(z + (Functions.ToDecimal(rabatt.Text) ?? 0)), 2);
      }
    }

    /// <summary>Starten des Details-Dialogs.</summary>
    /// <param name="dt">Betroffener Dialog-Typ.</param>
    void StartDialog(DialogTypeEnum dt)
    {
      var uid = hhbuchung.Text;
      if (!string.IsNullOrEmpty(uid))
        Start(typeof(HH410Booking), HH410_title, dt, uid, csbpparent: this);
    }

    private void EventList(string inuid)
    {
      if (invuid != inuid)
      {
        events = Get(FactoryService.StockService.GetEventList(ServiceDaten, inuid)) ?? new List<HhEreignis>();
        var values = new List<string[]>();
        values.Add(new string[] { "", "" }); // Leerer Eintrag, damit zunächst kein Ereignis ausgewählt ist.
        foreach (var e in events)
        {
          // Nr.;Bezeichnung
          values.Add(new string[] { e.Uid, e.Bezeichnung });
        }
        AddStringColumnsSort(hhereignis, WP410_hhereignis_columns, values);
        invuid = inuid;
      }
    }

  }
}
