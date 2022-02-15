// <copyright file="FZ260Mileage.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.FZ
{
  using System;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für FZ260Mileage Dialog.</summary>
  public partial class FZ260Mileage : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private FzFahrradstand Model;

#pragma warning disable 169, 649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Label fahrrad0.</summary>
    [Builder.Object]
    private Label fahrrad0;

    /// <summary>ComboBox fahrrad.</summary>
    [Builder.Object]
    private ComboBox fahrrad;

    /// <summary>Label datum0.</summary>
    [Builder.Object]
    private Label datum0;

    /// <summary>Date Datum.</summary>
    //[Builder.Object]
    private Date datum;

    /// <summary>Label zaehler0.</summary>
    [Builder.Object]
    private Label zaehler0;

    /// <summary>Entry zaehler.</summary>
    [Builder.Object]
    private Entry zaehler;

    /// <summary>Label km0.</summary>
    [Builder.Object]
    private Label km0;

    /// <summary>Entry km.</summary>
    [Builder.Object]
    private Entry km;

    /// <summary>Label schnitt0.</summary>
    [Builder.Object]
    private Label schnitt0;

    /// <summary>Entry schnitt.</summary>
    [Builder.Object]
    private Entry schnitt;

    /// <summary>Label beschreibung0.</summary>
    [Builder.Object]
    private Label beschreibung0;

    /// <summary>Entry beschreibung.</summary>
    [Builder.Object]
    private Entry beschreibung;

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
    public static FZ260Mileage Create(object p1 = null, CsbpBin p = null)
    {
      return new FZ260Mileage(GetBuilder("FZ260Mileage", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public FZ260Mileage(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      datum = new Date(Builder.GetObject("datum").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = false,
        YesterdayAccel = "m",
        TomorrowAccel = "p",
      };
      datum.DateChanged += OnDatumDateChanged;
      datum.Show();
      SetBold(fahrrad0);
      SetBold(datum0);
      SetBold(km0);
      InitData(0);
      km.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        var rl = Get(FactoryService.PrivateService.GetBikeList(daten));
        var rs = AddColumns(fahrrad, emptyentry: true);
        foreach (var p in rl)
          rs.AppendValues(p.Bezeichnung, p.Uid);
        if (rl.Any())
          SetText(fahrrad, rl.First().Uid);
        else
          SetText(fahrrad, "");
        datum.Value = daten.Heute;
        var neu = DialogType == DialogTypeEnum.New;
        var copy = DialogType == DialogTypeEnum.Copy;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var key = Parameter1 as Tuple<string, DateTime?, int>;
        if (!neu && key != null && key.Item2.HasValue)
        {
          var k = Get(FactoryService.PrivateService.GetMileage(daten, key.Item1, key.Item2.Value, key.Item3));
          if (k == null)
          {
            Application.Invoke(delegate
            {
              dialog.Hide();
            });
            return;
          }
          Model = k;
          nr.Text = k.Nr.ToString();
          SetText(fahrrad, k.Fahrrad_Uid);
          datum.Value = k.Datum;
          zaehler.Text = Functions.ToString(k.Zaehler_km, 0);
          km.Text = Functions.ToString(k.Periode_km, 0);
          schnitt.Text = Functions.ToString(k.Periode_Schnitt, 2);
          beschreibung.Text = k.Beschreibung;
          angelegt.Text = k.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = k.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        nr.IsEditable = false;
        fahrrad.Sensitive = neu;
        datum.Sensitive = neu || copy;
        zaehler.IsEditable = !loeschen;
        km.IsEditable = !loeschen;
        schnitt.IsEditable = !loeschen;
        beschreibung.IsEditable = !loeschen;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        if (loeschen)
          ok.Label = Forms_delete;
        EventsActive = true;
      }
    }

    /// <summary>Behandlung von Fahrrad.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFahrradChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von datum.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDatumDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von Zaehler.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnZaehlerKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      km.Text = "";
    }

    /// <summary>Behandlung von Km.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnKmKeyReleaseEvent(object o, KeyReleaseEventArgs e)
    {
      zaehler.Text = "";
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
        r = FactoryService.PrivateService.SaveMileage(ServiceDaten, GetText(fahrrad),
            datum.ValueNn, DialogType == DialogTypeEnum.Edit ? Functions.ToInt32(nr.Text) : -1,
            Functions.ToDecimal(zaehler.Text, 0) ?? 0, Functions.ToDecimal(km.Text, 0) ?? 0,
            Functions.ToDecimal(schnitt.Text, 2) ?? 0, beschreibung.Text);
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.PrivateService.DeleteMileage(ServiceDaten, Model);
      }
      if (r != null)
      {
        Get(r);
        if (r.Ok)
        {
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
