// <copyright file="TB100Diary.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms.Controls;
  using CSBP.Services.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für TB100Diary Dialog.</summary>
  public partial class TB100Diary : CsbpBin
  {
    /// <summary>Holt oder setzt den zu kopierenden Tagebuch-Eintrag.</summary>
    string Copy { get; set; } = string.Empty;

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Tagebuch-Eintrag gelesen wurde.</summary>
    bool Loaded { get; set; }

    /// <summary>Holt oder setzt den bisherigen Tagebuch-Eintrag.</summary>
    TbEintrag EntryOld { get; set; } = new TbEintrag { Positions = new List<TbEintragOrt>() };

#pragma warning disable 169, 649

    /// <summary>Button CopyAction.</summary>
    [Builder.Object]
    private Button copyAction;

    /// <summary>Button UndoAction.</summary>
    [Builder.Object]
    private Button undoAction;

    /// <summary>Button RedoAction.</summary>
    [Builder.Object]
    private Button redoAction;

    /// <summary>Label before10.</summary>
    [Builder.Object]
    private Label before10;

    /// <summary>TextView before1.</summary>
    [Builder.Object]
    private TextView before1;

    /// <summary>Label before20.</summary>
    [Builder.Object]
    private Label before20;

    /// <summary>TextView before2.</summary>
    [Builder.Object]
    private TextView before2;

    /// <summary>Label before30.</summary>
    [Builder.Object]
    private Label before30;

    /// <summary>TextView before3.</summary>
    [Builder.Object]
    private TextView before3;

    /// <summary>Label date0.</summary>
    [Builder.Object]
    private Label date0;

    /// <summary>Date date.</summary>
    //[Builder.Object]
    private Date date;

    /// <summary>Label entry0.</summary>
    [Builder.Object]
    private Label entry0;

    /// <summary>TextView entry.</summary>
    [Builder.Object]
    private TextView entry;

    /// <summary>TreeView positions.</summary>
    [Builder.Object]
    private TreeView positions;

    /// <summary>ComboBox position.</summary>
    [Builder.Object]
    private ComboBox position;

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

    /// <summary>Label search00.</summary>
    [Builder.Object]
    private Label search00;

    /// <summary>Button clear.</summary>
    [Builder.Object]
    private Button clear;

    /// <summary>Label search10.</summary>
    [Builder.Object]
    private Label search10;

    /// <summary>Entry search1.</summary>
    [Builder.Object]
    private Entry search1;

    /// <summary>Label search20.</summary>
    [Builder.Object]
    private Label search20;

    /// <summary>Entry search2.</summary>
    [Builder.Object]
    private Entry search2;

    /// <summary>Label search30.</summary>
    [Builder.Object]
    private Label search30;

    /// <summary>Entry search3.</summary>
    [Builder.Object]
    private Entry search3;

    /// <summary>Label search40.</summary>
    [Builder.Object]
    private Label search40;

    /// <summary>Label search50.</summary>
    [Builder.Object]
    private Label search50;

    /// <summary>Entry search4.</summary>
    [Builder.Object]
    private Entry search4;

    /// <summary>Label search60.</summary>
    [Builder.Object]
    private Label search60;

    /// <summary>Entry search5.</summary>
    [Builder.Object]
    private Entry search5;

    /// <summary>Label search70.</summary>
    [Builder.Object]
    private Label search70;

    /// <summary>Entry search6.</summary>
    [Builder.Object]
    private Entry search6;

    /// <summary>Label search80.</summary>
    [Builder.Object]
    private Label search80;

    /// <summary>Label search90.</summary>
    [Builder.Object]
    private Label search90;

    /// <summary>Entry search7.</summary>
    [Builder.Object]
    private Entry search7;

    /// <summary>Label search100.</summary>
    [Builder.Object]
    private Label search100;

    /// <summary>Entry search8.</summary>
    [Builder.Object]
    private Entry search8;

    /// <summary>Label search110.</summary>
    [Builder.Object]
    private Label search110;

    /// <summary>Entry search9.</summary>
    [Builder.Object]
    private Entry search9;

    /// <summary>Label search120.</summary>
    [Builder.Object]
    private Label search120;

    /// <summary>ComboBox position2.</summary>
    [Builder.Object]
    private ComboBox position2;

    /// <summary>From date.</summary>
    //[Builder.Object]
    private Date from;

    /// <summary>To date.</summary>
    //[Builder.Object]
    private Date to;

    /// <summary>Button first.</summary>
    [Builder.Object]
    private Button first;

    /// <summary>Button back.</summary>
    [Builder.Object]
    private Button back;

    /// <summary>Button forward.</summary>
    [Builder.Object]
    private Button forward;

    /// <summary>Button last.</summary>
    [Builder.Object]
    private Button last;

    /// <summary>Label after10.</summary>
    [Builder.Object]
    private Label after10;

    /// <summary>TextView after1.</summary>
    [Builder.Object]
    private TextView after1;

    /// <summary>Label after20.</summary>
    [Builder.Object]
    private Label after20;

    /// <summary>TextView after2.</summary>
    [Builder.Object]
    private TextView after2;

    /// <summary>Label after30.</summary>
    [Builder.Object]
    private Label after30;

    /// <summary>TextView after3.</summary>
    [Builder.Object]
    private TextView after3;

    /// <summary>Liste der zugeordneten Positionen.</summary>
    private List<TbEintragOrt> PositionList = new List<TbEintragOrt>();

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static TB100Diary Create(object p1 = null, CsbpBin p = null)
    {
      return new TB100Diary(GetBuilder("TB100Diary", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="h">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public TB100Diary(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
        : base(b, h, d, dt, p1, p)
    {
      date = new Date(Builder.GetObject("date").Handle)
      {
        IsNullable = false,
        IsWithCalendar = true,
        IsCalendarOpen = Functions.IsLinux() ? true : false,
        YesterdayAccel = "m",
        TomorrowAccel = "p",
      };
      date.DateChanged += OnDateDateChanged;
      date.MonthChanged += OnDateMonthChanged;
      date.Show();
      from = new Date(Builder.GetObject("from").Handle)
      {
        IsNullable = true,
        IsWithCalendar = true,
        IsCalendarOpen = false,
        IsWithoutNullLabel = true,
        IsWithoutDayOfWeek = true,
      };
      from.Show();
      to = new Date(Builder.GetObject("to").Handle)
      {
        IsNullable = true,
        IsWithCalendar = true,
        IsCalendarOpen = false,
        IsWithoutNullLabel = true,
        IsWithoutDayOfWeek = true,
      };
      to.Show();
      SetBold(date0);
      SetBold(entry0);
      InitData(0);
      entry.GrabFocus();
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
    {
      if (step <= 0)
      {
        EventsActive = false;
        InitLists();
        ClearSearch();
        EntryOld.Datum = DateTime.Today;
        date.Value = EntryOld.Datum;
        BearbeiteEintraege(false);
        last.Click();
        before1.Editable = false;
        before2.Editable = false;
        before3.Editable = false;
        after1.Editable = false;
        after2.Editable = false;
        after3.Editable = false;
        angelegt.IsEditable = false;
        geaendert.IsEditable = false;
        EventsActive = true;
      }
    }

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    override protected void UpdateParent()
    {
      InitLists();
    }

    /// <summary>Initialisierung der Listen.</summary>
    private void InitLists()
    {
      var daten = ServiceDaten;
      var rl = Get(FactoryService.DiaryService.GetPositionList(daten));
      var uid = GetText(position);
      var rs = AddColumns(position, emptyentry: true);
      foreach (var p in rl)
        rs.AppendValues(p.Bezeichnung, p.Uid);
      SetText(position, uid);
      var uid2 = GetText(position2);
      var rs2 = AddColumns(position2, emptyentry: true);
      rl.Insert(0, new TbOrt { Uid = "0", Bezeichnung = M0(TB012) });
      foreach (var p in rl)
        rs2.AppendValues(p.Bezeichnung, p.Uid);
      SetText(position2, uid2);
    }

    /// <summary>Behandlung von Copy.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnCopyClicked(object sender, EventArgs e)
    {
      Copy = entry.Buffer.Text;
    }

    /// <summary>Behandlung von Paste.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPasteClicked(object sender, EventArgs e)
    {
      entry.Buffer.Text = Copy ?? "";
      BearbeiteEintraege(true, false);
    }

    /// <summary>Behandlung von Undo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnUndoClicked(object sender, EventArgs e)
    {
      if (MainClass.Undo())
      {
        InitLists();
        BearbeiteEintraege(false);
      }
    }

    /// <summary>Behandlung von Redo.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRedoClicked(object sender, EventArgs e)
    {
      if (MainClass.Redo())
      {
        InitLists();
        BearbeiteEintraege(false);
      }
    }

    /// <summary>Behandlung von Save.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnSaveClicked(object sender, EventArgs e)
    {
      // Bericht erzeugen
      BearbeiteEintraege(true, false);
      var puid = GetText(position2);
      var pfad = Parameter.TempPath;
      var datei = Functions.GetDateiname(M0(TB005), true, true, "txt");
      UiTools.SaveFile(Get(FactoryService.DiaryService.GetFile(ServiceDaten, GetSearchArray(),
        puid, from.Value, to.Value)), pfad, datei);
    }

    /// <summary>Behandlung von Date.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDateDateChanged(object sender, DateChangedEventArgs e)
    {
      if (!EventsActive)
        return;
      BearbeiteEintraege();
    }

    /// <summary>Behandlung von Date.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDateMonthChanged(object sender, DateChangedEventArgs e)
    {
      if (!EventsActive)
        return;
      LoadMonth(e.Date);
    }

    /// <summary>Behandlung von Positions.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPositionsRowActivated(object sender, RowActivatedArgs e)
    {
      var uid = GetText(positions) ?? "";
      var p = PositionList.FirstOrDefault(a => a.Ort_Uid == uid);
      if (p != null)
      {
        // UiTools.StartFile($"https://www.google.com/maps/@{Functions.ToString(p.Breite, 5, Functions.CultureInfoEn)},{Functions.ToString(p.Laenge, 5, Functions.CultureInfoEn)}"); // ,15z
        UiTools.StartFile($"https://www.openstreetmap.org/#map=19/{Functions.ToString(p.Breite, 5, Functions.CultureInfoEn)}/{Functions.ToString(p.Laenge, 5, Functions.CultureInfoEn)}");
      }
    }

    /// <summary>Behandlung von Position.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPositionChanged(object sender, EventArgs e)
    {
      // refreshAction.Click();
    }

    /// <summary>Behandlung von New.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnNewClicked(object sender, EventArgs e)
    {
      Start(typeof(TB210Position), TB210_title, DialogTypeEnum.New, null, csbpparent: this);
    }

    /// <summary>Behandlung von Add.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAddClicked(object sender, EventArgs e)
    {
      var uid = GetText(position);
      if (string.IsNullOrEmpty(uid))
        return;
      var o = PositionList.FirstOrDefault(a => a.Ort_Uid == uid);
      if (o != null)
      {
        var p = new Tuple<string, DateTime>(o.Ort_Uid, o.Datum_Bis);
        var to = Start(typeof(TB110Date), TB110_title, DialogTypeEnum.Edit, p, modal: true, csbpparent: this) as DateTime?;
        if (to.HasValue)
        {
          if (to.Value >= date.ValueNn)
            o.Datum_Bis = to.Value;
          else
            o.Datum_Von = to.Value;
        }
        InitPositions();
        return;
      }
      var k = Get(FactoryService.DiaryService.GetPosition(ServiceDaten, uid));
      if (k != null)
      {
        var p = new TbEintragOrt
        {
          Mandant_Nr = k.Mandant_Nr,
          Ort_Uid = k.Uid,
          Datum_Von = date.ValueNn,
          Datum_Bis = date.ValueNn,
          Bezeichnung = k.Bezeichnung,
          Breite = k.Breite,
          Laenge = k.Laenge,
          Hoehe = k.Hoehe,
        };
        PositionList.Add(p);
        InitPositions();
      }
    }

    /// <summary>Behandlung von Posbefore.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnPosbeforeClicked(object sender, EventArgs e)
    {
      var yd = date.ValueNn.AddDays(-1);
      var r = FactoryService.DiaryService.GetEntry(ServiceDaten, yd, true);
      if (r.Ok && r.Ergebnis != null)
      {
        foreach (var p in r.Ergebnis.Positions ?? new List<TbEintragOrt>())
        {
          if (PositionList.FirstOrDefault(a => a.Ort_Uid == p.Ort_Uid) == null)
          {
            if (p.Datum_Bis == yd)
              p.Datum_Bis = p.Datum_Bis.AddDays(1);
            PositionList.Add(p);
          }
        }
        InitPositions();
      }
    }

    /// <summary>Behandlung von Remove.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnRemoveClicked(object sender, EventArgs e)
    {
      var uid = GetText(positions);
      if (string.IsNullOrEmpty(uid) || !PositionList.Any(a => a.Ort_Uid == uid))
        return;
      PositionList = PositionList.Where(a => a.Ort_Uid != uid).ToList();
      InitPositions();
    }

    /// <summary>Behandlung von First.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnFirstClicked(object sender, EventArgs e)
    {
      SearchEntry(SearchDirectionEnum.First);
    }

    /// <summary>Behandlung von Back.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnBackClicked(object sender, EventArgs e)
    {
      SearchEntry(SearchDirectionEnum.Back);
    }

    /// <summary>Behandlung von Forward.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnForwardClicked(object sender, EventArgs e)
    {
      SearchEntry(SearchDirectionEnum.Forward);
    }

    /// <summary>Behandlung von Ende.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnLastClicked(object sender, EventArgs e)
    {
      SearchEntry(SearchDirectionEnum.Last);
    }

    /// <summary>Behandlung von Clear.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnClearClicked(object sender, EventArgs e)
    {
      ClearSearch();
      BearbeiteEintraege(true, false);
    }

    /// <summary>
    /// Lesen der Einträge ausgehend vom übergebenen Datum.
    /// </summary>
    /// <param name="d">Betroffenes Datum.</param>
    /// <returns>Service-Ergebnis mit evtl. Fehlermeldungen.</returns>
    ServiceErgebnis LadeEintraege(DateTime? d)
    {
      var daten = ServiceDaten;
      var r = new ServiceErgebnis();
      if (!d.HasValue)
        return r;
      var tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddDays(-1)));
      before1.Buffer.Text = tb?.Eintrag ?? "";
      tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddMonths(-1)));
      before2.Buffer.Text = tb?.Eintrag ?? "";
      tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddYears(-1)));
      before3.Buffer.Text = tb?.Eintrag ?? "";
      tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value, true));
      EntryOld.Positions.Clear();
      if (tb == null)
      {
        EntryOld.Eintrag = string.Empty;
        angelegt.Text = string.Empty;
        geaendert.Text = string.Empty;
      }
      else
      {
        EntryOld.Eintrag = tb.Eintrag;
        EntryOld.Positions.AddRange(tb.Positions);
        angelegt.Text = tb.FormatDateOf(tb.Angelegt_Am, tb.Angelegt_Von);
        geaendert.Text = tb.FormatDateOf(tb.Geaendert_Am, tb.Geaendert_Von);
      }
      EntryOld.Datum = d.Value;
      entry.Buffer.Text = EntryOld.Eintrag ?? "";
      tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddDays(1)));
      after1.Buffer.Text = tb?.Eintrag ?? "";
      tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddMonths(1)));
      after2.Buffer.Text = tb?.Eintrag ?? "";
      tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddYears(1)));
      after3.Buffer.Text = tb?.Eintrag ?? "";
      InitPositions(EntryOld.Positions);
      return r;
    }

    /// <summary>
    /// Initialisierung der Positionen.
    /// </summary>
    /// <param name="list">Neue Liste.</param>
    void InitPositions(List<TbEintragOrt> list = null)
    {
      if (list != null)
      {
        PositionList.Clear();
        foreach (var p in list)
          PositionList.Add(ServiceBase.Clone(p));
      }
      var values = new List<string[]>();
      foreach (var e in PositionList)
      {
        // Nr.;Bezeichnung;Breite;Länge;Von;Bis;Geändert am;Geändert von;Angelegt am;Angelegt von
        values.Add(new string[] { e.Ort_Uid, e.Bezeichnung, Functions.ToString(e.Breite, 5), Functions.ToString(e.Laenge, 5),
          Functions.ToString(e.Datum_Von), Functions.ToString(e.Datum_Bis),
          Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
          Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von });
      }
      AddStringColumnsSort(positions, TB100_positions_columns, values);
    }

    /// <summary>
    /// Lesen der Einträge ausgehend vom aktuellen Datum.
    /// Evtl. wird vorher der aktuelle Eintrag gespeichert.
    /// </summary>
    /// <param name="speichern">Soll vorher gespeichert werden soll?</param>
    /// <param name="laden">Sollen Einträge geladen werden?</param>
    void BearbeiteEintraege(bool speichern = true, bool laden = true)
    {
      var daten = ServiceDaten;
      var r = new ServiceErgebnis();
      // Rekursion vermeiden
      if (speichern && Loaded)
      {
        // Alten Eintrag von vorher merken.
        var str = EntryOld.Eintrag;
        var p0 = EntryOld.Positions.OrderBy(a => a.Ort_Uid).Select(a => a.Hash()).Aggregate("", (c, n) => c + n);
        var p = PositionList.OrderBy(a => a.Ort_Uid).Select(a => a.Hash()).Aggregate("", (c, n) => c + n);
        // Nur speichern, wenn etwas geändert ist.
        if (str == null || Functions.CompString(str, entry.Buffer.Text) != 0 || Functions.CompString(p0, p) != 0)
        {
          var pos = PositionList.Select(a => new Tuple<string, DateTime, DateTime>(a.Ort_Uid, a.Datum_Von, a.Datum_Bis)).ToList();
          r.Get(FactoryService.DiaryService.SaveEntry(daten, EntryOld.Datum, entry.Buffer.Text, pos));
        }
      }
      if (laden)
      {
        var d = date.Value;
        r.Get(LadeEintraege(d));
        LoadMonth(d);
        Loaded = true;
      }
      Get(r);
    }

    void LoadMonth(DateTime? d)
    {
      bool[] m = null;
      if (d.HasValue)
        m = Get(FactoryService.DiaryService.GetMonth(ServiceDaten, d.Value));
      date.MarkMonth(m);
    }

    void ClearSearch()
    {
      search1.Text = "%%";
      search2.Text = "%%";
      search3.Text = "%%";
      search4.Text = "%%";
      search5.Text = "%%";
      search6.Text = "%%";
      search7.Text = "%%";
      search8.Text = "%%";
      search9.Text = "%%";
      SetText(position2, null);
      from.Value = Functions.IsLinux() ? DateTime.Today.AddYears(-1) : null;
      to.Value = DateTime.Today;
      // to.Value = null;
    }

    string[] GetSearchArray()
    {
      var search = new[] { search1.Text, search2.Text, search3.Text, search4.Text, search5.Text, search6.Text, search7.Text, search8.Text, search9.Text };
      return search;
    }

    /// <summary>
    /// Suche des nächsten passenden Eintrags in der Suchrichtung.
    /// </summary>
    /// <param name="stelle">Gewünschte Such-Richtung.</param>
    void SearchEntry(SearchDirectionEnum stelle)
    {
      BearbeiteEintraege(true, false);
      var puid = GetText(position2);
      var d = Get(FactoryService.DiaryService.SearchDate(ServiceDaten, stelle, date.Value, GetSearchArray(),
        puid, from.Value, to.Value));
      if (d.HasValue)
      {
        date.Value = d;
        BearbeiteEintraege(false);
      }
    }
  }
}
