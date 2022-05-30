// <copyright file="FZ350Book.cs" company="cwkuehl.de">
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

  /// <summary>Controller für FZ350Book Dialog.</summary>
  public partial class FZ350Book : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    private FzBuch Model;

#pragma warning disable CS0649

    /// <summary>Label nr0.</summary>
    [Builder.Object]
    private Label nr0;

    /// <summary>Entry nr.</summary>
    [Builder.Object]
    private Entry nr;

    /// <summary>Label titel0.</summary>
    [Builder.Object]
    private Label titel0;

    /// <summary>Entry titel.</summary>
    [Builder.Object]
    private Entry titel;

    /// <summary>Entry untertitel.</summary>
    [Builder.Object]
    private Entry untertitel;

    /// <summary>Label autor0.</summary>
    [Builder.Object]
    private Label autor0;

    /// <summary>ComboBox autor.</summary>
    [Builder.Object]
    private ComboBox autor;

    /// <summary>Button autorneu.</summary>
    [Builder.Object]
    private Button autorneu;

    /// <summary>Label serie0.</summary>
    [Builder.Object]
    private Label serie0;

    /// <summary>ComboBox serie.</summary>
    [Builder.Object]
    private ComboBox serie;

    /// <summary>Button serieneu.</summary>
    [Builder.Object]
    private Button serieneu;

    /// <summary>Label seriennummer0.</summary>
    [Builder.Object]
    private Label seriennummer0;

    /// <summary>Entry seriennummer.</summary>
    [Builder.Object]
    private Entry seriennummer;

    /// <summary>Label seiten0.</summary>
    [Builder.Object]
    private Label seiten0;

    /// <summary>Entry seiten.</summary>
    [Builder.Object]
    private Entry seiten;

    /// <summary>Label sprache0.</summary>
    [Builder.Object]
    private Label sprache0;

    /// <summary>RadioButton sprache1.</summary>
    [Builder.Object]
    private RadioButton sprache1;

    /// <summary>RadioButton sprache2.</summary>
    [Builder.Object]
    private RadioButton sprache2;

    /// <summary>RadioButton sprache3.</summary>
    [Builder.Object]
    private RadioButton sprache3;

    /// <summary>RadioButton sprache4.</summary>
    [Builder.Object]
    private RadioButton sprache4;

    /// <summary>Label besitz0.</summary>
    [Builder.Object]
    private Label besitz0;

    /// <summary>CheckButton besitz.</summary>
    [Builder.Object]
    private CheckButton besitz;

    /// <summary>Label lesedatum0.</summary>
    [Builder.Object]
    private Label lesedatum0;

    /// <summary>Date Lesedatum.</summary>
    //[Builder.Object]
    private Date lesedatum;

    /// <summary>Label hoerdatum0.</summary>
    [Builder.Object]
    private Label hoerdatum0;

    /// <summary>Date Hoerdatum.</summary>
    //[Builder.Object]
    private Date hoerdatum;

    /// <summary>TextView notiz.</summary>
    [Builder.Object]
    private TextView notiz;

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

#pragma warning restore CS0649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static FZ350Book Create(object p1 = null, CsbpBin p = null)
    {
      return new FZ350Book(GetBuilder("FZ350Book", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public FZ350Book(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      lesedatum = new Date(Builder.GetObject("lesedatum").Handle)
      {
        IsNullable = true,
        IsWithCalendar = true,
        IsCalendarOpen = false,
        YesterdayAccel = "m",
        TomorrowAccel = "p",
      };
      lesedatum.DateChanged += OnLesedatumDateChanged;
      lesedatum.Show();
      hoerdatum = new Date(Builder.GetObject("hoerdatum").Handle)
      {
        IsNullable = true,
        IsWithCalendar = true,
        IsCalendarOpen = false
      };
      hoerdatum.DateChanged += OnHoerdatumDateChanged;
      hoerdatum.Show();
      SetBold(titel0);
      SetBold(autor0);
      SetBold(serie0);
      SetBold(sprache0);
      InitData(0);
      titel.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected override void InitData(int step)
    {
      var daten = ServiceDaten;
      if (step <= 0)
      {
        SetUserData(new[] { sprache1, sprache2, sprache3, sprache4 }, new[] { "1", "2", "3", "0" });
        var al = Get(FactoryService.PrivateService.GetAuthorList(daten));
        var rs = AddColumns(autor, emptyentry: true);
        foreach (var p in al)
          rs.AppendValues(p.CompleteName, p.Uid);
        SetText(autor, "");
        var sl = Get(FactoryService.PrivateService.GetSeriesList(daten));
        rs = AddColumns(serie);
        foreach (var p in sl)
          rs.AppendValues(p.Name, p.Uid);
        if (sl.Any())
          SetText(serie, sl.First().Uid);
        sprache1.Active = true;
        besitz.Active = false;
        lesedatum.Value = daten.Heute;
        hoerdatum.Value = null; // daten.Heute;
        var neu = DialogType == DialogTypeEnum.New;
        var loeschen = DialogType == DialogTypeEnum.Delete;
        var uid = Parameter1 as string;
        if (!neu && uid != null)
        {
          var k = Get(FactoryService.PrivateService.GetBook(ServiceDaten, uid));
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
          titel.Text = k.Titel ?? "";
          untertitel.Text = k.Untertitel ?? "";
          SetText(autor, k.Autor_Uid);
          SetText(serie, k.Serie_Uid);
          seriennummer.Text = Functions.ToString(k.Seriennummer == 0 ? (int?)null : k.Seriennummer);
          seiten.Text = Functions.ToString(k.Seiten == 0 ? (int?)null : k.Seiten);
          SetText(sprache1, Functions.ToString(k.Sprache_Nr));
          besitz.Active = k.StatePossession;
          lesedatum.Value = k.StateRead;
          hoerdatum.Value = k.StateHeard;
          notiz.Buffer.Text = k.Notiz ?? "";
          angelegt.Text = ModelBase.FormatDateOf(k.Angelegt_Am, k.Angelegt_Von);
          geaendert.Text = ModelBase.FormatDateOf(k.Geaendert_Am, k.Geaendert_Von);
        }
        nr.IsEditable = false;
        titel.IsEditable = !loeschen;
        untertitel.IsEditable = !loeschen;
        autor.Sensitive = !loeschen;
        serie.Sensitive = !loeschen;
        seriennummer.IsEditable = !loeschen;
        seiten.IsEditable = !loeschen;
        foreach (RadioButton a in sprache1.Group)
          a.Sensitive = !loeschen;
        besitz.Sensitive = !loeschen;
        lesedatum.Sensitive = !loeschen;
        hoerdatum.Sensitive = !loeschen;
        notiz.Editable = !loeschen;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        if (loeschen)
          ok.Label = Forms_delete;
      }
    }

    /// <summary>Behandlung von Autor.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAutorChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Autorneu.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAutorneuClicked(object sender, EventArgs e)
    {
      var a = Start(typeof(FZ310Author), FZ310_title, DialogTypeEnum.New, modal: true, csbpparent: this) as FzBuchautor;
      if (a != null)
      {
        var al = Get(FactoryService.PrivateService.GetAuthorList(ServiceDaten));
        var rs = AddColumns(autor, emptyentry: true);
        foreach (var p in al)
          rs.AppendValues(p.CompleteName, p.Uid);
        SetText(autor, a.Uid);
      }
    }

    /// <summary>Behandlung von Serie.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSerieChanged(object sender, EventArgs e)
    {
    }

    /// <summary>Behandlung von Serieneu.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSerieneuClicked(object sender, EventArgs e)
    {
      var a = Start(typeof(FZ330Series), FZ330_title, DialogTypeEnum.New, modal: true, csbpparent: this) as FzBuchserie;
      if (a != null)
      {
        var al = Get(FactoryService.PrivateService.GetSeriesList(ServiceDaten));
        var rs = AddColumns(serie, emptyentry: true);
        foreach (var p in al)
          rs.AppendValues(p.Name, p.Uid);
        SetText(serie, a.Uid);
      }
    }

    /// <summary>Behandlung von lesedatum.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnLesedatumDateChanged(object sender, DateChangedEventArgs e)
    {
    }

    /// <summary>Behandlung von hoerdatum.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnHoerdatumDateChanged(object sender, DateChangedEventArgs e)
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
        r = FactoryService.PrivateService.SaveBook(ServiceDaten,
          DialogType == DialogTypeEnum.Edit ? nr.Text : null, GetText(autor), GetText(serie),
            Functions.ToInt32(seriennummer.Text), titel.Text, untertitel.Text, Functions.ToInt32(seiten.Text),
            Functions.ToInt32(GetText(sprache1)), besitz.Active, lesedatum.Value,
            hoerdatum.Value, notiz.Buffer.Text);
      }
      else if (DialogType == DialogTypeEnum.Delete)
      {
        r = FactoryService.PrivateService.DeleteBook(ServiceDaten, Model);
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
