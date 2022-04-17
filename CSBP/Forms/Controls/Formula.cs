// <copyright file="Formula.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSBP.Base;
using Gtk;

/// <summary>
/// Data and parser for a formula.
/// </summary>
public class Formula
{
  /// <summary>Number of column the formula is in.</summary>
  public int column { get; set; }

  /// <summary>Number of row the formula is in.</summary>
  public int row { get; set; }

  /// <summary>Formula as string.</summary>
  public string formula { get; private set; }

  /// <summary>Affected function.</summary>
  public string function { get; private set; }

  /// <summary>Column number 1 of affected area.</summary>
  public int column1 { get; private set; } = -1;

  /// <summary>Row number 1 of affected area.</summary>
  public int row1 { get; private set; } = -1;

  /// <summary>Column number 2 of affected area.</summary>
  public int column2 { get; private set; } = -1;

  /// <summary>Row number 2 of affected area.</summary>
  public int row2 { get; private set; } = -1;

  /// <summary>Value as string.</summary>
  private string _value;

  /// <summary>Value as string.</summary>
  public string Value { get { return _value; } set { _value = Functions.MakeBold(value, !_bold); } }

  /// <summary>Is formula bold?</summary>
  private bool _bold;

  /// <summary>Is formula bold?</summary>
  public bool bold
  {
    get
    {
      return _bold;
    }
    set
    {
      Value = Functions.MakeBold(Value, value);
      _bold = value;
    }
  }

  /// <summary>Regex for formula sum.</summary>
  private static Regex RxSum = new Regex(@"^=(sum|summe)\(([a-z]+)(\d+):([a-z]+)(\d+)\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>Regex for formula count.</summary>
  private static Regex RxCount = new Regex(@"^=(count|anzahl)\(([a-z]+)(\d+):([a-z]+)(\d+)\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>Regex for formula count.</summary>
  private static Regex RxDays = new Regex(@"^=(days|tage)\(([a-z]+)(\d+);([a-z]+)(\d+)\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>Regex for formula today.</summary>
  private static Regex RxToday = new Regex(@"^=(today|heute)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>Regex for formula now.</summary>
  private static Regex RxNow = new Regex(@"^=(now|jetzt)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>
  /// Private constructor.
  /// </summary>
  /// <param name="formula">Affected formula.</param>
  /// <param name="c">Affected column.</param>
  /// <param name="r">Affected row.</param>
  /// <param name="function">Affected function.</param>
  /// <param name="bold">Is formula bold?</param>
  private Formula(string formula, int c, int r, string function, bool bold)
  {
    this.formula = formula;
    this.column = c;
    this.row = r;
    this.function = function;
    this.bold = bold;
    Debug.Print($"Formula {formula} {c} {r} {function} {bold}");
  }

  /// <summary>
  /// Create a Formula instance, if it is a formula.
  /// </summary>
  /// <param name="formula">Possible formula.</param>
  /// <param name="c">Affected column.</param>
  /// <param name="r">Affected row.</param>
  /// <returns>Formula instance or null.</returns>
  public static Formula Instance(string formula, int c, int r)
  {
    if (string.IsNullOrWhiteSpace(formula))
      return null;
    var bold = false;
    if (Functions.IsBold(formula))
    {
      bold = true;
      formula = Functions.MakeBold(formula, true);
    }
    var m = RxSum.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "sum", bold)
      {
        column1 = GetColumnIndex(m.Groups[2].Value),
        row1 = Functions.ToInt32(m.Groups[3].Value) - 1,
        column2 = GetColumnIndex(m.Groups[4].Value),
        row2 = Functions.ToInt32(m.Groups[5].Value) - 1,
      };
    }
    m = RxCount.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "count", bold)
      {
        column1 = GetColumnIndex(m.Groups[2].Value),
        row1 = Functions.ToInt32(m.Groups[3].Value) - 1,
        column2 = GetColumnIndex(m.Groups[4].Value),
        row2 = Functions.ToInt32(m.Groups[5].Value) - 1,
      };
    }
    m = RxDays.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "days", bold)
      {
        column1 = GetColumnIndex(m.Groups[2].Value),
        row1 = Functions.ToInt32(m.Groups[3].Value) - 1,
        column2 = GetColumnIndex(m.Groups[4].Value),
        row2 = Functions.ToInt32(m.Groups[5].Value) - 1,
      };
    }
    m = RxNow.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "now", bold);
    }
    m = RxToday.Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "today", bold);
    }
    return null;
  }

  /// <summary>
  /// Returns a default name for the column using spreadsheet conventions: A, B, C, ... Z, AA, AB, etc.
  /// If column cannot be found, returns an empty string.
  /// </summary>
  /// <param name="column">Affected column number.</param>
  /// <returns>Default name of column</returns>
  public static string GetColumnName(int column)
  {
    if (column < 0)
      return " ";
    var sb = new StringBuilder();
    for (; column >= 0; column = column / 26 - 1)
    {
      sb.Insert(0, (char)((char)(column % 26) + 'A'));
    }
    return sb.ToString();
  }

  /// <summary>
  /// Calculate the row number for a column name using spreadsheet conventions: A, B, C, ... Z, AA, AB, etc.
  /// </summary>
  /// <param name="name">Affected column name.</param>
  /// <returns>Column number.</returns>
  public static int GetColumnIndex(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return -1;
    var str = name.ToUpper();
    int column = 0;
    int p = 1;
    for (int i = str.Length - 1; i >= 0; i--)
    {
      column += ((str[i] - 'A') % 26 + 1) * p;
      p *= 26;
    }
    return column - 1;
  }
}

/// <summary>
/// List of formulas and status of editing.
/// </summary>
public class Formulas
{
  /// <summary>List of formulas.</summary>
  public List<Formula> List { get; } = new List<Formula>();

  /// <summary>List of formulas.</summary>
  public Tuple<int, int> Cell { get; private set; }

  /// <summary>
  /// Get formula of a cell.
  /// </summary>
  /// <param name="c">Affected column number.</param>
  /// <param name="r">Affected row number.</param>
  /// <returns>Formula or null.</returns>
  public Formula Get(int c, int r)
  {
    if (c < 2)
      return null;
    var f = List.FirstOrDefault(a => a.column == c - 2 && a.row == r);
    return f;
  }

  /// <summary>
  /// Begin the editing of a cell.
  /// </summary>
  /// <param name="cnr">Affected column number.</param>
  /// <param name="rnr">Affected row number.</param>
  /// <returns>Is editing OK?</returns>
  public bool BeginEdit(int cnr, int rnr)
  {
    var b = false;
    if (cnr >= 2)
    {
      var cell = Cell;
      if (cell == null)
      {
        Cell = new Tuple<int, int>(cnr, rnr);
        b = true;
      }
      else
        b = (cnr == cell.Item1 && rnr == cell.Item2);
    }
    // Debug.Print($"BeginEdit cnr {cnr} rnr {rnr}");
    return b;
  }

  /// <summary>
  /// Begin the editing of a cell.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="store">Affected TreeView store.</param>
  public void BeginEdit(TreeView tv, TreeStore store)
  {
    if (tv == null || store == null)
      return;
    tv.GetCursor(out var path, out var c);
    if (path == null || c.Data["cnr"] == null)
      return;
    var cnr = (int)c.Data["cnr"];
    var rnr = path.Indices[0];
    store.GetIter(out var it, path);
    var v = new GLib.Value();
    store.GetValue(it, cnr, ref v);
    var val = v.Val as string;
    Debug.Print($"BeginEdit cnr {cnr} rnr {rnr}");
    var cell = Cell;
    if (cell != null && !(cell.Item1 == cnr && cell.Item2 == rnr))
    {
      // EndEdit for previous Edit
      var f0 = Get(cell.Item1, cell.Item2);
      if (f0 != null)
      {
        var path0 = new TreePath(new[] { cell.Item2 });
        store.GetIter(out var it0, path0);
        var v0 = new GLib.Value();
        store.GetValue(it0, cell.Item1, ref v0);
        var newtext = v0.Val as string;
        EndEdit(store, cell.Item1, cell.Item2, newtext);
      }
      Cell = null;
    }
    BeginEdit(cnr, rnr);
    var f = Get(cnr, rnr);
    if (f != null && val != f.formula)
    {
      store.SetValue(it, cnr, f.formula);
    }
  }

  /// <summary>
  /// End the editing of a cell.
  /// </summary>
  /// <param name="store">Affected TreeView store.</param>
  /// <param name="cnr">Affected column number.</param>
  /// <param name="args">Affected event args.</param>
  public void EndEdit(TreeStore store, int cnr, EditedArgs args)
  {
    if (store == null || args == null)
      return;
    var path = new TreePath(args.Path);
    var rnr = path.Indices[0];
    EndEdit(store, cnr, rnr, args.NewText);
  }

  /// <summary>
  /// End the editing of a cell.
  /// </summary>
  /// <param name="store">Affected TreeView store.</param>
  /// <param name="cnr">Affected column number.</param>
  /// <param name="rnr">Affected row number.</param>
  /// <param name="newtext">Affected newtext.</param>
  private void EndEdit(TreeStore store, int cnr, int rnr, string newtext)
  {
    if (store == null)
      return;
    var path = new TreePath(new[] { rnr });
    store.GetIter(out var it, path);
    var v = new GLib.Value();
    store.GetValue(it, cnr, ref v);
    var val = v.Val as string;
    if (cnr == 1)
    {
      // Do not change the row number.
      if (val != newtext)
        store.SetValue(it, cnr, val);
      return;
    }
    var f = Get(cnr, rnr);
    if (Functions.IsBold(val))
      newtext = Functions.MakeBold(newtext); // Preserve boldness
    if (val == newtext)
    {
      if (f != null)
      {
        // Formula stays the same, show last value.
        newtext = f.Value;
        if (val != newtext)
        {
          store.SetValue(it, cnr, newtext);
        }
      }
    }
    else // if (val != newtext)
    {
      Debug.Print($"old {val} new {newtext}");
      if (f != null)
        List.Remove(f);
      var newf = Formula.Instance(newtext, cnr - 2, rnr);
      if (newf == null)
        store.SetValue(it, cnr, newtext);
      else
      {
        List.Add(newf);
        CalculateFormulas(store);
      }
    }
    var cell = Cell;
    if (cell != null && cell.Item1 == cnr && cell.Item2 == rnr)
      Cell = null;
    else
      Debug.Print($"No EndEdit cnr {cnr} rnr {rnr}");
  }

  /// <summary>
  /// Calculate all formulas.
  /// </summary>
  public void CalculateFormulas(TreeStore store)
  {
    if (store == null)
      return;
    foreach (var f in List)
    {
      string val = null;
      if (f.function == "sum")
      {
        if (f.column1 >= 0 && f.row1 >= 0 && f.column2 >= 0 && f.row2 >= 0)
        {
          var sum = 0m;
          var v = new GLib.Value();
          for (var r = f.row1; r <= f.row2; r++)
          {
            var path = new TreePath(new[] { r });
            store.GetIter(out var it, path);
            for (var c = f.column1; c <= f.column2; c++)
            {
              store.GetValue(it, c + 2, ref v);
              sum += Functions.ToDecimalCi(Functions.MakeBold(v.Val as string, true)) ?? 0;
            }
          }
          val = Functions.ToString(sum);
        }
      }
      else if (f.function == "count")
      {
        if (f.column1 >= 0 && f.row1 >= 0 && f.column2 >= 0 && f.row2 >= 0)
        {
          var count = 0;
          var v = new GLib.Value();
          for (var r = f.row1; r <= f.row2; r++)
          {
            var path = new TreePath(new[] { r });
            store.GetIter(out var it, path);
            for (var c = f.column1; c <= f.column2; c++)
            {
              store.GetValue(it, c + 2, ref v);
              if (!string.IsNullOrWhiteSpace(Functions.MakeBold(v.Val as string, true)))
                count++;
            }
          }
          val = Functions.ToString(count);
        }
      }
      else if (f.function == "days")
      {
        val = "";
        if (f.column1 >= 0 && f.row1 >= 0 && f.column2 >= 0 && f.row2 >= 0)
        {
          var v = new GLib.Value();
          var path = new TreePath(new[] { f.row1 });
          store.GetIter(out var it, path);
          store.GetValue(it, f.column1 + 2, ref v);
          var val1 = Functions.ToDateTime(Functions.MakeBold(v.Val as string, true));
          if (val1.HasValue)
          {
            if (f.row1 != f.row2)
            {
              path = new TreePath(new[] { f.row2 });
              store.GetIter(out it, path);
            }
            store.GetValue(it, f.column2 + 2, ref v);
            var val2 = Functions.ToDateTime(Functions.MakeBold(v.Val as string, true));
            if (val2.HasValue)
              val = Functions.ToString((long)(val1.Value - val2.Value).TotalDays, 0);
          }
        }
      }
      else if (f.function == "now")
      {
        val = Functions.ToString(DateTime.Now, true);
      }
      else if (f.function == "today")
      {
        val = Functions.ToString(DateTime.Today, false);
      }
      if (val != null)
      {
        f.Value = Functions.MakeBold(val, !f.bold);
        Debug.Print($"Calculate {f.formula} {f.Value}");
        var tp = new TreePath(new[] { f.row });
        store.GetIter(out var it, tp);
        store.SetValue(it, f.column + 2, f.Value);
      }
    }
  }

  /// <summary>
  /// Add a row.
  /// </summary>
  /// <param name="rnr">Affected row number.</param>
  public void AddRow(int rnr)
  {
    // TODO AddRow formula sum, count, days.
    Debug.Print($"AddRow rnr {rnr}");
    foreach (var f in List)
    {
      if (f.row >= rnr)
        f.row++;
    }
  }

  /// <summary>
  /// Delete a row.
  /// </summary>
  /// <param name="rnr">Affected row number.</param>
  public void DeleteRow(int rnr)
  {
    Debug.Print($"DeleteRow rnr {rnr}");
    var l = new List<Formula>();
    foreach (var f in List)
    {
      if (f.row == rnr)
        l.Add(f);
      if (f.row > rnr)
        f.row--;
    }
    foreach (var f in l)
    {
      List.Remove(f);
    }
  }

  /// <summary>
  /// Add a column.
  /// </summary>
  /// <param name="cnr">Affected column number.</param>
  public void AddColumn(int cnr)
  {
    Debug.Print($"AddColumn cnr {cnr}");
    foreach (var f in List)
    {
      if (f.column >= cnr)
        f.column++;
    }
  }

  /// <summary>
  /// Delete a column.
  /// </summary>
  /// <param name="cnr">Affected column number.</param>
  public void DeleteColumn(int cnr)
  {
    Debug.Print($"DeleteColumn cnr {cnr}");
    var l = new List<Formula>();
    foreach (var f in List)
    {
      if (f.column == cnr)
        l.Add(f);
      if (f.column > cnr)
        f.column--;
    }
    foreach (var f in l)
    {
      List.Remove(f);
    }
  }
}
