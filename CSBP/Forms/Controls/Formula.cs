// <copyright file="Formula.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

  /// <summary>Regex for formula today.</summary>
  private static Regex RxToday = new Regex(@"^=(today)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  /// <summary>Regex for formula today.</summary>
  private static Regex RxNow = new Regex(@"^=(now)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
    var m = RxNow.Match(formula);
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
    Debug.Print($"BeginEdit cnr {cnr} rnr {rnr}");
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
    var cnr = (int)c.Data["cnr"];
    store.GetIter(out var it, path);
    var v = new GLib.Value();
    store.GetValue(it, cnr, ref v);
    var val = v.Val as string;
    var rnr = path.Indices[0];
    // Debug.Print($"Return cnr {cnr} rnr {rnr}");
    BeginEdit(cnr, rnr);
    var f = Get(cnr, rnr);
    if (f != null && val != f.formula)
    {
      store.SetValue(it, cnr, f.formula);
      Debug.Print($"Bold {f.bold}");
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
    store.GetIter(out var it, path);
    var v = new GLib.Value();
    store.GetValue(it, cnr, ref v);
    var val = v.Val as string;
    var rnr = path.Indices[0];
    var f = Get(cnr, rnr);
    var newtext = args.NewText;
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
      string v = null;
      if (f.function == "now")
      {
        v = Functions.ToString(DateTime.Now, true);
      }
      else if (f.function == "today")
      {
        v = Functions.ToString(DateTime.Today, false);
      }
      if (v != null)
      {
        f.Value = v;
        Debug.Print($"Calculate {f.formula} {f.Value}");
        var tp = new TreePath(new[] { f.row });
        store.GetIter(out var it, tp);
        store.SetValue(it, f.column + 2, v);
      }
    }
  }
}
