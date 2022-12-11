// <copyright file="SO100Sudoku.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.SO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Resources;
using CSBP.Services.NonService;
using Gtk;

/// <summary>Controller for SO100Sudoku dialog.</summary>
public partial class SO100Sudoku : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>Grid field.</summary>
  [Builder.Object]
  private readonly Grid field;

  /// <summary>Label anzahl.</summary>
  [Builder.Object]
  private readonly Label anzahl;

  /// <summary>CheckButton diagonal.</summary>
  [Builder.Object]
  private readonly CheckButton diagonal;

#pragma warning restore CS0649

  /// <summary>Sudoku context.</summary>
  private SudokuContext context;

  /// <summary>Undo list.</summary>
  private Stack<SudokuContext> undolist;

  /// <summary>Array of fields.</summary>
  private Entry[] fields;

  /// <summary>Initializes a new instance of the <see cref="SO100Sudoku"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public SO100Sudoku(Builder b, IntPtr h, Dialog d = null, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, type ?? typeof(SO100Sudoku), dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    fields[0].GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static SO100Sudoku Create(object p1 = null, CsbpBin p = null)
  {
    return new SO100Sudoku(GetBuilder("SO100Sudoku", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      undolist = new Stack<SudokuContext>();
      var json = Parameter.SO100Sudoku;
      try
      {
        context = JsonSerializer.Deserialize<SudokuContext>(json)!;
      }
      catch (Exception)
      {
        Functions.MachNichts();
      }
      if (context == null)
      {
        var su = new[]
        {
          "____8____", // Zeit 12/08
          "_4_1_7_8_",
          "__65324__",
          "_71___62_",
          "6_9___3_7",
          "_34___95_",
          "__53298__",
          "_1_8_5_9_",
          "____1____",
        };
        var arr = string.Join("", su).ToCharArray().Select(a => Functions.ToInt32(a.ToString())).ToArray();
        context = new SudokuContext(arr, false);
      }
      foreach (var f in field.Children)
      {
        field.Remove(f);
      }
      fields = new Entry[context.Max];
      var x = context.Maxx;
      var y = context.Maxy;
      for (var i = 0; i < x; i++)
      {
        for (var j = 0; j < y; j++)
        {
          var ff = new Entry
          {
            Visible = true,
            CanFocus = true,
            Hexpand = false,
            WidthChars = 1,
          };
          ff.KeyReleaseEvent += OnFieldKeyRelease;
          ff.KeyPressEvent += OnFieldKeyPress;
          field.Attach(ff, i, j, 1, 1);
          fields[(j * x) + i] = ff;
        }
      }
    }
    if (step <= 1)
    {
      var arr = context.Numbers;
      var l = Math.Min(arr.Length, fields.Length);
      for (var i = 0; i < l; i++)
      {
        SetText(fields[i], arr[i] <= 0 ? "" : arr[i].ToString());
      }
      diagonal.Active = context.Diagonal;
      anzahl.LabelProp = M.SO002(context.Count());
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
    if (undolist.Count > 0)
    {
      var c = undolist.Pop();
      if (c != null)
      {
        context = c;
        InitData(1);
      }
    }
  }

  /// <summary>Handles Save.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnSaveClicked(object sender, EventArgs e)
  {
    OnDiagonal(null, null);
    var json = JsonSerializer.Serialize<SudokuContext>(context);
    Parameter.SO100Sudoku = json;
  }

  /// <summary>Handles Delete.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteClicked(object sender, EventArgs e)
  {
    if (context.Count() > 0)
    {
      var c = new SudokuContext(context, null);
      c.Clear();
      undolist.Push(context);
      context = c;
      InitData(1);
    }
  }

  /// <summary>Handles Zug.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZugClicked(object sender, EventArgs e)
  {
    Solve(true);
  }

  /// <summary>Handles Loesen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnLoesenClicked(object sender, EventArgs e)
  {
    Solve(false);
  }

  /// <summary>Handles Test.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTestClicked(object sender, EventArgs e)
  {
    Solve(false, true);
  }

  /// <summary>Handles field.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  private void OnFieldKeyRelease(object sender, KeyReleaseEventArgs e)
  {
    var key = e.Event.Key switch
    {
      Gdk.Key.Key_1 => "1",
      Gdk.Key.Key_2 => "2",
      Gdk.Key.Key_3 => "3",
      Gdk.Key.Key_4 => "4",
      Gdk.Key.Key_5 => "5",
      Gdk.Key.Key_6 => "6",
      Gdk.Key.Key_7 => "7",
      Gdk.Key.Key_8 => "8",
      Gdk.Key.Key_9 => "9",
      _ => "",
    };
    SetText((Entry)sender, key);
    for (var i = 0; i < fields.Length; i++)
    {
      if (fields[i] == sender)
      {
        // Set focus.
        var next = i < fields.Length - 1 ? fields[i + 1] : fields[0];
        next.GrabFocus();
        break;
      }
    }
  }

  /// <summary>Handles field.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  private void OnFieldKeyPress(object sender, KeyPressEventArgs e)
  {
    // if (e.Event.Key == Gdk.Key.a)
    // {
    //   SetText((Entry)sender, "X");
    //   Functions.MachNichts();
    // }
  }

  /// <summary>Handles diagonal.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  private void OnDiagonal(object sender, EventArgs e)
  {
    var arr = context.Numbers;
    var l = Math.Min(arr.Length, fields.Length);
    for (var i = 0; i < l; i++)
    {
      arr[i] = Functions.ToInt32(fields[i].Text);
    }
    var c = new SudokuContext(context, arr, diagonal.Active);
    SudokuContext.Add(undolist, context);
    context = c;
  }

  /// <summary>Solve or test.</summary>
  /// <param name="move1">Only 1 move or more.</param>
  /// <param name="test">Test for discrepancy.</param>
  private void Solve(bool move1, bool test = false)
  {
    var arr = context.Numbers;
    var l = Math.Min(arr.Length, fields.Length);
    for (var i = 0; i < l; i++)
    {
      arr[i] = Functions.ToInt32(fields[i].Text);
    }
    try
    {
      Application.Invoke((sender, e) => { MainClass.MainWindow.SetError(null); });
      var c = new SudokuContext(context, arr);
      if (test)
        SudokuContext.Test(context, true);
      else
        SudokuContext.Solve(c, move1);
      SudokuContext.Add(undolist, context);
      context = c;
      InitData(1);
    }
    catch (Exception ex)
    {
      Application.Invoke((sender, e) => { MainClass.MainWindow.SetError(ex.Message); });
    }
  }
}
