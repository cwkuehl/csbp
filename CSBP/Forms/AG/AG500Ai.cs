// <copyright file="AG500Ai.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AG;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Forms.TB;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Controller for AG500Ai dialog.</summary>
public partial class AG500Ai : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Label date0.</summary>
  [Builder.Object]
  private readonly Label date0;

  /// <summary>Date date.</summary>
  //// [Builder.Object]
  private readonly Date date;

  /// <summary>Label prompt0.</summary>
  [Builder.Object]
  private readonly Label prompt0;

  /// <summary>TextView prompt.</summary>
  [Builder.Object]
  private readonly TextView prompt;

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

  /// <summary>Label after10.</summary>
  [Builder.Object]
  private readonly Label after10;

  /// <summary>TextView after1.</summary>
  [Builder.Object]
  private readonly TextView after1;

#pragma warning restore CS0649

  /// <summary>List of current positions.</summary>
  private List<TbEintragOrt> positionList = new();

  /// <summary>Initializes a new instance of the <see cref="AG500Ai"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AG500Ai(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(AG500Ai), dt, p1, p)
  {
    date = new Date(Builder.GetObject("date").Handle)
    {
      IsNullable = false,
      IsWithCalendar = true,
      IsCalendarOpen = false,
      YesterdayAccel = "m",
      TomorrowAccel = "p",
    };
    date.DateChanged += OnDateDateChanged;
    date.MonthChanged += OnDateMonthChanged;
    date.Show();
    SetBold(date0);
    SetBold(prompt0);
    InitData(0);
    prompt.GrabFocus();
  }

  /// <summary>Gets or sets the diary entry for copying.</summary>
  private string Copy { get; set; } = string.Empty;

  /// <summary>Gets or sets the origanal saved diary entry.</summary>
  private TbEintrag EntryOld { get; set; } = new TbEintrag { Positions = new List<TbEintragOrt>() };

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AG500Ai Create(object p1 = null, CsbpBin p = null)
  {
    return new AG500Ai(GetBuilder("AG500Ai", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      EventsActive = false;
      InitLists();
      EntryOld.Datum = DateTime.Today;
      date.Value = EntryOld.Datum;
      SetText(prompt, "What is your temperatur");
      after1.Editable = false;
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
    Copy = prompt.Buffer.Text;
  }

  /// <summary>Handles Paste.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnPasteClicked(object sender, EventArgs e)
  {
    SetText(prompt, Copy);
  }

  /// <summary>Handles Undo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUndoClicked(object sender, EventArgs e)
  {
    if (MainClass.Undo())
    {
      InitLists();
    }
  }

  /// <summary>Handles Redo.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnRedoClicked(object sender, EventArgs e)
  {
    if (MainClass.Redo())
    {
      InitLists();
    }
  }

  /// <summary>Handles Weather.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnWeatherClicked(object sender, EventArgs e)
  {
    var v = after1.Visible;
    after10.LabelProp = v ? TB100_after1w : TB100_after1;
    after1.Visible = !v;
    if (!v)
      return;
  }

  /// <summary>Handles Save.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSaveClicked(object sender, EventArgs e)
  {
    // Bericht erzeugen
    var pfad = Parameter.TempPath;
    var datei = Functions.GetDateiname(M0(TB005), true, true, "txt");
    UiTools.SaveFile(Get(FactoryService.DiaryService.GetDiaryReport(ServiceDaten, null,
      null, null, null)), pfad, datei);
  }

  /// <summary>Handles Date.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateDateChanged(object sender, DateChangedEventArgs e)
  {
    if (!EventsActive)
      return;
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
    var r = FactoryService.ClientService.AskChatGpt(ServiceDaten, prompt.Buffer.Text);
    Get(r);
    var data = r.Ergebnis;
    if (r.Ok && data != null)
    {
      if (data.Messages.Any())
        SetText(after1, r.Ergebnis.Messages.FirstOrDefault());
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

  /// <summary>Handles Clear.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnClearClicked(object sender, EventArgs e)
  {
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
}
