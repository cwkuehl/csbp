// <copyright file="TB100Diary.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.TB;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>Controller for TB100Diary dialog.</summary>
public partial class TB100Diary : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Button UndoAction.</summary>
  [Builder.Object]
  private readonly Button undoAction;

  /// <summary>Button RedoAction.</summary>
  [Builder.Object]
  private readonly Button redoAction;

  /// <summary>Label before10.</summary>
  [Builder.Object]
  private readonly Label before10;

  /// <summary>TextView before1.</summary>
  [Builder.Object]
  private readonly TextView before1;

  /// <summary>DrawingArea diagramb1.</summary>
  [Builder.Object]
  private readonly DrawingArea diagramb1;

  /// <summary>Label before20.</summary>
  [Builder.Object]
  private readonly Label before20;

  /// <summary>TextView before2.</summary>
  [Builder.Object]
  private readonly TextView before2;

  /// <summary>DrawingArea diagramb2.</summary>
  [Builder.Object]
  private readonly DrawingArea diagramb2;

  /// <summary>Label before30.</summary>
  [Builder.Object]
  private readonly Label before30;

  /// <summary>TextView before3.</summary>
  [Builder.Object]
  private readonly TextView before3;

  /// <summary>DrawingArea diagramb3.</summary>
  [Builder.Object]
  private readonly DrawingArea diagramb3;

  /// <summary>Label date0.</summary>
  [Builder.Object]
  private readonly Label date0;

  /// <summary>Date date.</summary>
  //// [Builder.Object]
  private readonly Date date;

  /// <summary>Label entry0.</summary>
  [Builder.Object]
  private readonly Label entry0;

  /// <summary>TextView entry.</summary>
  [Builder.Object]
  private readonly TextView entry;

  /// <summary>TreeView positions.</summary>
  [Builder.Object]
  private readonly TreeView positions;

  /// <summary>ComboBox position.</summary>
  [Builder.Object]
  private readonly ComboBox position;

  /// <summary>Entry angelegt.</summary>
  [Builder.Object]
  private readonly Entry angelegt;

  /// <summary>Entry geaendert.</summary>
  [Builder.Object]
  private readonly Entry geaendert;

  /// <summary>Entry search1.</summary>
  [Builder.Object]
  private readonly Grid searchgrid;

  /// <summary>Entry search1.</summary>
  [Builder.Object]
  private readonly Entry search1;

  /// <summary>Entry search2.</summary>
  [Builder.Object]
  private readonly Entry search2;

  /// <summary>Entry search3.</summary>
  [Builder.Object]
  private readonly Entry search3;

  /// <summary>Entry search4.</summary>
  [Builder.Object]
  private readonly Entry search4;

  /// <summary>Entry search5.</summary>
  [Builder.Object]
  private readonly Entry search5;

  /// <summary>Entry search6.</summary>
  [Builder.Object]
  private readonly Entry search6;

  /// <summary>Entry search7.</summary>
  [Builder.Object]
  private readonly Entry search7;

  /// <summary>Entry search8.</summary>
  [Builder.Object]
  private readonly Entry search8;

  /// <summary>Entry search9.</summary>
  [Builder.Object]
  private readonly Entry search9;

  /// <summary>ComboBox position2.</summary>
  [Builder.Object]
  private readonly ComboBox position2;

  /// <summary>From date.</summary>
  //// [Builder.Object]
  private readonly Date from;

  /// <summary>To date.</summary>
  //// [Builder.Object]
  private readonly Date to;

  /// <summary>Button last.</summary>
  [Builder.Object]
  private readonly Button last;

  /// <summary>Label after10.</summary>
  [Builder.Object]
  private readonly Label after10;

  /// <summary>TextView after1.</summary>
  [Builder.Object]
  private readonly TextView after1;

  /// <summary>DrawingArea diagrama1.</summary>
  [Builder.Object]
  private readonly DrawingArea diagrama1;

  /// <summary>Label after20.</summary>
  [Builder.Object]
  private readonly Label after20;

  /// <summary>TextView after2.</summary>
  [Builder.Object]
  private readonly TextView after2;

  /// <summary>DrawingArea diagrama2.</summary>
  [Builder.Object]
  private readonly DrawingArea diagrama2;

  /// <summary>Label after30.</summary>
  [Builder.Object]
  private readonly Label after30;

  /// <summary>TextView after3.</summary>
  [Builder.Object]
  private readonly TextView after3;

  /// <summary>DrawingArea diagrama3.</summary>
  [Builder.Object]
  private readonly DrawingArea diagrama3;

#pragma warning restore CS0649

  /// <summary>List of current positions.</summary>
  private List<TbEintragOrt> positionList = new();

  /// <summary>Diagram model temperature.</summary>
  private List<KeyValuePair<string, decimal>> templist;

  /// <summary>Diagram model sea-level air pressure in hPa.</summary>
  private List<KeyValuePair<string, decimal>> preslist;

  /// <summary>Diagram model relative humidity in percent (%).</summary>
  private List<KeyValuePair<string, decimal>> rhumlist;

  /// <summary>Diagram model one hour precipitation total in mm.</summary>
  private List<KeyValuePair<string, decimal>> prcplist;

  /// <summary>Diagram model average wind speed in km/h.</summary>
  private List<KeyValuePair<string, decimal>> wspdlist;

  /// <summary>Diagram model wind direction in degrees (°).</summary>
  private List<KeyValuePair<string, decimal>> wdirlist;

  /// <summary>Initializes a new instance of the <see cref="TB100Diary"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public TB100Diary(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(TB100Diary), dt, p1, p)
  {
    date = new Date(Builder.GetObject("date").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = Functions.IsLinux(),
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
    undoAction.EnterNotifyEvent += OnUndoRedoEnter;
    redoAction.EnterNotifyEvent += OnUndoRedoEnter;
    entry.GrabFocus();
  }

  /// <summary>Gets or sets the diary entry for copying.</summary>
  private string Copy { get; set; } = string.Empty;

  /// <summary>Gets or sets a value indicating whether the diary entry is loaded or not.</summary>
  private bool Loaded { get; set; }

  /// <summary>Gets or sets the origanal saved diary entry.</summary>
  private TbEintrag EntryOld { get; set; } = new TbEintrag { Positions = new List<TbEintragOrt>() };

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static TB100Diary Create(object p1 = null, CsbpBin p = null)
  {
    return new TB100Diary(GetBuilder("TB100Diary", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      InitLists();
      ClearSearch();
      searchgrid.Visible = false;
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

  /// <summary>Updates parent dialog.</summary>
  protected override void UpdateParent()
  {
    InitLists();
  }

  /// <summary>Handles Copy.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCopyClicked(object sender, EventArgs e)
  {
    Copy = entry.Buffer.Text;
  }

  /// <summary>Handles Paste.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPasteClicked(object sender, EventArgs e)
  {
    SetText(entry, Copy);
    BearbeiteEintraege(true, false);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
    {
      InitLists();
      BearbeiteEintraege(false);
    }
  }

  /// <summary>Handles Undo Redo Enter.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoRedoEnter(object sender, EnterNotifyEventArgs e)
  {
    UiTools.UpdateUndoRedoSize(undoAction, redoAction);
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
    {
      InitLists();
      BearbeiteEintraege(false);
    }
  }

  /// <summary>Handles Weather.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWeatherClicked(object sender, EventArgs e)
  {
    var v = before1.Visible;
    before10.LabelProp = v ? TB100_before1w : TB100_before1;
    before20.LabelProp = v ? TB100_before2w : TB100_before2;
    before30.LabelProp = v ? TB100_before3w : TB100_before3;
    after10.LabelProp = v ? TB100_after1w : TB100_after1;
    after20.LabelProp = v ? TB100_after2w : TB100_after2;
    after30.LabelProp = v ? TB100_after3w : TB100_after3;
    before1.Visible = before2.Visible = before3.Visible = after1.Visible = after2.Visible = after3.Visible = !v;
    diagramb1.Visible = diagramb2.Visible = diagramb3.Visible = diagrama1.Visible = diagrama2.Visible = diagrama3.Visible = v;
    if (!v)
      return;
    Application.Invoke((sender, e) =>
    {
      ShowWeather(true);
    });
  }

  /// <summary>Handles Download.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDownloadClicked(object sender, EventArgs e)
  {
    var d = date.ValueNn;
    var t = Get(FactoryService.DiaryService.GetApiDiaryList(ServiceDaten, d));
    if (t == null)
      return;
    if (t.Item2.Count() <= 0)
    {
      // TODO Translate messages.
      ShowInfo("Keine Rabp-Daten gefunden.");
      return;
    }
    var sb = new StringBuilder();
    sb.AppendLine(@$"""Rabp-Version {t.Item1}  {t.Item2.Count()} {Functions.Iif(t.Item2.Count() == 1, "Eintrag", "Einträge")} vor {d:yyyy-MM-dd}""");
    foreach (var item in t.Item2)
    {
      sb.AppendLine($"{item.Datum:yyyy-MM-dd} {item.Eintrag}");
    }
    sb.AppendLine("Importieren der Rabp-Daten ins Tagebuch?");
    if (ShowYesNoQuestion(sb.ToString()))
    {
      BearbeiteEintraege(); // Save
      if (!Get(FactoryService.DiaryService.ReplicateDiaryList(ServiceDaten, t.Item2)))
        return;
      BearbeiteEintraege(false); // Reload
    }
    var s = @$"Löschen der Rabp-Daten?";
    if (ShowYesNoQuestion(s))
      Get(FactoryService.DiaryService.GetApiDiaryList(ServiceDaten, d, true));
  }

  /// <summary>Handles Save.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSaveClicked(object sender, EventArgs e)
  {
    // Bericht erzeugen
    BearbeiteEintraege(true, false);
    var puid = GetText(position2);
    var pfad = ParameterGui.TempPath;
    var datei = Functions.GetDateiname(M0(TB005), true, true, "txt");
    var daten = ServiceDaten;
    UiTools.SaveFile(daten, Get(FactoryService.DiaryService.GetDiaryReport(daten, GetSearchArray(),
      puid, from.Value, to.Value)), pfad, datei);
  }

  /// <summary>Handles Date.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    BearbeiteEintraege();
  }

  /// <summary>Handles Date.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateMonthChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
    LoadMonth(e.Date);
  }

  /// <summary>Handles Positions.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPositionsCursorChanged(object sender, EventArgs e)
  {
    if (!before1.Visible)
    {
      ShowWeather();
    }
  }

  /// <summary>Handles Positions.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPositionsRowActivated(object sender, RowActivatedArgs e)
  {
    var uid = Functions.ToString(GetText(positions));
    var p = positionList.FirstOrDefault(a => a.Ort_Uid == uid);
    if (p != null)
    {
      UiTools.StartFile($"https://www.openstreetmap.org/#map=19/{Functions.ToString(p.Latitude, 5, Functions.CultureInfoEn)}/{Functions.ToString(p.Longitude, 5, Functions.CultureInfoEn)}");
    }
  }

  /// <summary>Handles Position.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPositionChanged(object sender, EventArgs e)
  {
    // refreshAction.Click();
  }

  /// <summary>Handles New.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnNewClicked(object sender, EventArgs e)
  {
    Start(typeof(TB210Position), TB210_title, DialogTypeEnum.New, null, csbpparent: this);
  }

  /// <summary>Handles Add.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAddClicked(object sender, EventArgs e)
  {
    var uid = GetText(position);
    if (string.IsNullOrEmpty(uid))
      return;
    var o = positionList.FirstOrDefault(a => a.Ort_Uid == uid);
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
        Description = k.Bezeichnung,
        Latitude = k.Breite,
        Longitude = k.Laenge,
        Height = k.Hoehe,
        Memo = k.Notiz,
      };
      positionList.Add(p);
      InitPositions();
    }
  }

  /// <summary>Handles Posbefore.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPosbeforeClicked(object sender, EventArgs e)
  {
    var yd = date.ValueNn.AddDays(-1);
    var r = FactoryService.DiaryService.GetEntry(ServiceDaten, yd, true);
    if (r.Ok && r.Ergebnis != null)
    {
      foreach (var p in r.Ergebnis.Positions ?? new List<TbEintragOrt>())
      {
        if (positionList.FirstOrDefault(a => a.Ort_Uid == p.Ort_Uid) == null)
        {
          if (p.Datum_Bis == yd)
            p.Datum_Bis = p.Datum_Bis.AddDays(1);
          positionList.Add(p);
        }
      }
      InitPositions();
    }
  }

  /// <summary>Handles Remove.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRemoveClicked(object sender, EventArgs e)
  {
    var uid = GetText(positions);
    if (string.IsNullOrEmpty(uid) || !positionList.Any(a => a.Ort_Uid == uid))
      return;
    positionList = positionList.Where(a => a.Ort_Uid != uid).ToList();
    InitPositions();
  }

  /// <summary>Handles Search.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSearchClicked(object sender, EventArgs e)
  {
    searchgrid.Visible = !searchgrid.Visible;
  }

  /// <summary>Handles First.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnFirstClicked(object sender, EventArgs e)
  {
    SearchEntry(SearchDirectionEnum.First);
  }

  /// <summary>Handles Back.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnBackClicked(object sender, EventArgs e)
  {
    SearchEntry(SearchDirectionEnum.Back);
  }

  /// <summary>Handles Forward.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnForwardClicked(object sender, EventArgs e)
  {
    SearchEntry(SearchDirectionEnum.Forward);
  }

  /// <summary>Handles Ende.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnLastClicked(object sender, EventArgs e)
  {
    SearchEntry(SearchDirectionEnum.Last);
  }

  /// <summary>Handles Clear.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnClearClicked(object sender, EventArgs e)
  {
    ClearSearch();
    BearbeiteEintraege(true, false);
  }

  /// <summary>Handles Diagramb1.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagramb1Draw(object sender, DrawnArgs e)
  {
    var avg = templist?.Where(a => Functions.ToInt32(a.Key) >= 6 && Functions.ToInt32(a.Key) <= 22).Average(a => a.Value) ?? 0m;
    OnDiagramaDraw(sender, e, TB100_before1w, templist, $"Ø {avg:0.000} °C (6-22 Uhr)");
  }

  /// <summary>Handles Diagramb2.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagramb2Draw(object sender, DrawnArgs e)
  {
    OnDiagramaDraw(sender, e, TB100_before2w, preslist);
  }

  /// <summary>Handles Diagramb3.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagramb3Draw(object sender, DrawnArgs e)
  {
    OnDiagramaDraw(sender, e, TB100_before3w, rhumlist);
  }

  /// <summary>Handles Diagrama1.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagrama1Draw(object sender, DrawnArgs e)
  {
    OnDiagramaDraw(sender, e, TB100_after1w, prcplist);
  }

  /// <summary>Handles Diagrama2.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagrama2Draw(object sender, DrawnArgs e)
  {
    OnDiagramaDraw(sender, e, TB100_after2w, wspdlist);
  }

  /// <summary>Handles Diagrama3.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDiagrama3Draw(object sender, DrawnArgs e)
  {
    OnDiagramaDraw(sender, e, TB100_after3w, wdirlist);
  }

  /// <summary>Handles Diagrama3.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  /// <param name="label">Affected label.</param>
  /// <param name="list">Affected list.</param>
  /// <param name="label2">Affected second label.</param>
  private void OnDiagramaDraw(object sender, DrawnArgs e, string label, List<KeyValuePair<string, decimal>> list, string label2 = null)
  {
    if (before1.Visible && list == null)
      return;
    var da = sender as DrawingArea;
    var c = e.Cr;
    var w = da.Window.Width;
    var h = da.Window.Height;
    Diagram.Draw(label, list, c, 0, 0, w, h, label2);
  }

  /// <summary>Initialises the lists.</summary>
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

  /// <summary>
  /// Loads the diary entries from date.
  /// </summary>
  /// <param name="d">Affected date.</param>
  /// <returns>Service result and possibly errors.</returns>
  private ServiceErgebnis LadeEintraege(DateTime? d)
  {
    var daten = ServiceDaten;
    var r = new ServiceErgebnis();
    if (!d.HasValue)
      return r;
    var tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddDays(-1)));
    SetText(before1, tb?.Eintrag);
    tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddMonths(-1)));
    SetText(before2, tb?.Eintrag);
    tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddYears(-1)));
    SetText(before3, tb?.Eintrag);
    tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value, true));
    EntryOld.Positions.Clear();
    if (tb == null)
    {
      EntryOld.Eintrag = "";
      SetText(angelegt, "");
      SetText(geaendert, "");
    }
    else
    {
      EntryOld.Eintrag = tb.Eintrag;
      EntryOld.Positions.AddRange(tb.Positions);
      SetText(angelegt, ModelBase.FormatDateOf(tb.Angelegt_Am, tb.Angelegt_Von));
      SetText(geaendert, ModelBase.FormatDateOf(tb.Geaendert_Am, tb.Geaendert_Von));
    }
    EntryOld.Datum = d.Value;
    SetText(entry, EntryOld.Eintrag);
    tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddDays(1)));
    SetText(after1, tb?.Eintrag);
    tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddMonths(1)));
    SetText(after2, tb?.Eintrag);
    tb = r.Get(FactoryService.DiaryService.GetEntry(daten, d.Value.AddYears(1)));
    SetText(after3, tb?.Eintrag);
    InitPositions(EntryOld.Positions);
    if (!before1.Visible)
      ShowWeather();
    return r;
  }

  /// <summary>
  /// Initialises the position list.
  /// </summary>
  /// <param name="list">New list.</param>
  private void InitPositions(List<TbEintragOrt> list = null)
  {
    if (list != null)
    {
      positionList.Clear();
      foreach (var p in list)
        positionList.Add(ServiceBase.Clone(p));
    }
    var values = new List<string[]>();
    foreach (var e in positionList)
    {
      // No.;Description;Latitude_r;Longitude_r;From;To;Changed at;Changed by;Created at;Created by
      values.Add(new string[]
      {
        e.Ort_Uid, e.Description, Functions.ToString(e.Latitude, 5), Functions.ToString(e.Longitude, 5),
        Functions.ToString(e.Datum_Von), Functions.ToString(e.Datum_Bis),
        Functions.ToString(e.Geaendert_Am, true), e.Geaendert_Von,
        Functions.ToString(e.Angelegt_Am, true), e.Angelegt_Von,
      });
    }
    AddStringColumnsSort(positions, TB100_positions_columns, values);
  }

  /// <summary>
  /// Loads the diary entry from date. Optionally the current entry is saved before.
  /// </summary>
  /// <param name="speichern">Saves before or not.</param>
  /// <param name="laden">Loads the entry or not.</param>
  private void BearbeiteEintraege(bool speichern = true, bool laden = true)
  {
    var daten = ServiceDaten;
    var r = new ServiceErgebnis();
    //// Prohibits rekursion.
    if (speichern && Loaded)
    {
      // Alten Eintrag von vorher merken.
      var str = EntryOld.Eintrag;
      var p0 = EntryOld.Positions.OrderBy(a => a.Ort_Uid).Select(a => a.Hash()).Aggregate("", (c, n) => c + n);
      var p = positionList.OrderBy(a => a.Ort_Uid).Select(a => a.Hash()).Aggregate("", (c, n) => c + n);
      //// Nur speichern, wenn etwas geändert ist.
      if (str == null || Functions.CompString(str, entry.Buffer.Text) != 0 || Functions.CompString(p0, p) != 0)
      {
        var pos = positionList.Select(a => new Tuple<string, DateTime, DateTime>(a.Ort_Uid, a.Datum_Von, a.Datum_Bis)).ToList();
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

  /// <summary>
  /// Loads the month data.
  /// </summary>
  /// <param name="d">Affected date.</param>
  private void LoadMonth(DateTime? d)
  {
    bool[] m = null;
    if (d.HasValue)
      m = Get(FactoryService.DiaryService.GetMonth(ServiceDaten, d.Value));
    date.MarkMonth(m);
  }

  /// <summary>
  /// Clears the search data.
  /// </summary>
  private void ClearSearch()
  {
    SetText(search1, "%%");
    SetText(search2, "%%");
    SetText(search3, "%%");
    SetText(search4, "%%");
    SetText(search5, "%%");
    SetText(search6, "%%");
    SetText(search7, "%%");
    SetText(search8, "%%");
    SetText(search9, "%%");
    SetText(position2, null);
    from.Value = Functions.IsLinux() ? DateTime.Today.AddYears(-1) : null;
    to.Value = DateTime.Today;
    //// to.Value = null;
  }

  /// <summary>
  /// Gets the search array.
  /// </summary>
  /// <returns>Search array.</returns>
  private string[] GetSearchArray()
  {
    var search = new[] { search1.Text, search2.Text, search3.Text, search4.Text, search5.Text, search6.Text, search7.Text, search8.Text, search9.Text };
    return search;
  }

  /// <summary>
  /// Searches for next fitting entry in search direction.
  /// </summary>
  /// <param name="stelle">Affected search direction.</param>
  private void SearchEntry(SearchDirectionEnum stelle)
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

  /// <summary>Show weather data.</summary>
  /// <param name="init">Initial call or not.</param>
  private void ShowWeather(bool init = false)
  {
    var puid = GetText(positions, false, 0, true);
    templist = [];
    preslist = [];
    rhumlist = [];
    prcplist = [];
    wspdlist = [];
    wdirlist = [];
    if (string.IsNullOrEmpty(puid))
    {
      if (init)
        ShowInfo("Für Wetterdaten muss eine Position zugeordnet werden."); // TODO Translate messages.
      return;
    }
    var r = Get(FactoryService.DiaryService.GetWeatherList(ServiceDaten, date.ValueNn, puid));
    foreach (var w in r ?? [])
    {
      templist.Add(new KeyValuePair<string, decimal>(w.Time.ToString("HH"), w.Temp));
      preslist.Add(new KeyValuePair<string, decimal>(w.Time.ToString("HH"), w.Pres));
      rhumlist.Add(new KeyValuePair<string, decimal>(w.Time.ToString("HH"), w.Rhum));
      prcplist.Add(new KeyValuePair<string, decimal>(w.Time.ToString("HH"), w.Prcp));
      wspdlist.Add(new KeyValuePair<string, decimal>(w.Time.ToString("HH"), w.Wspd));
      wdirlist.Add(new KeyValuePair<string, decimal>(w.Time.ToString("HH"), w.Wdir));
    }
    //// diagramb1.Window.InvalidateRect(new Gdk.Rectangle(0, 0, diagramb1.Window.Width, diagramb1.Window.Height), true);
  }
}
