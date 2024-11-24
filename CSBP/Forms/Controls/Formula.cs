// <copyright file="Formula.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSBP.Services.Base;
using Gtk;

/// <summary>
/// Data and parser for a formula.
/// </summary>
public partial class Formula
{
  /// <summary>Internal value as string.</summary>
  private string intvalue;

  /// <summary>Is formula bold or not.</summary>
  private bool intbold;

  /// <summary>
  /// Initializes a new instance of the <see cref="Formula"/> class.
  /// </summary>
  /// <param name="formula">Affected formula.</param>
  /// <param name="c">Affected column.</param>
  /// <param name="r">Affected row.</param>
  /// <param name="function">Affected function.</param>
  /// <param name="bold">Is formula bold or not.</param>
  /// <param name="c1">Affected column number 1.</param>
  /// <param name="r1">Affected row number 1.</param>
  /// <param name="c2">Affected column number 2.</param>
  /// <param name="r2">Affected row number 2.</param>
  private Formula(string formula, int c, int r, string function, bool bold, int c1 = -1, int r1 = -1, int c2 = -1, int r2 = -1)
  {
    // this.formula = formula;
    Column = c;
    Row = r;
    Function = function;
    Bold = bold;
    Column1 = c1;
    Row1 = r1;
    Column2 = c2;
    Row2 = r2;
    Formula1 = ToString();
    Debug.Print($"Formula {formula} {c} {r} {function} {bold}");
  }

  /// <summary>Gets or sets the number of column the formula is in.</summary>
  public int Column { get; set; }

  /// <summary>Gets or sets the number of row the formula is in.</summary>
  public int Row { get; set; }

  /// <summary>Gets the formula as string.</summary>
  public string Formula1 { get; internal set; }

  /// <summary>Gets the function.</summary>
  public string Function { get; private set; }

  /// <summary>Gets column number 1 of affected area.</summary>
  public int Column1 { get; internal set; } = -1;

  /// <summary>Gets row number 1 of affected area.</summary>
  public int Row1 { get; internal set; } = -1;

  /// <summary>Gets column number 2 of affected area.</summary>
  public int Column2 { get; internal set; } = -1;

  /// <summary>Gets row number 2 of affected area.</summary>
  public int Row2 { get; internal set; } = -1;

  /// <summary>Gets or sets the value as string.</summary>
  public string Value
  {
    get { return intvalue; }
    set { intvalue = Functions.MakeBold(value, !intbold); }
  }

  /// <summary>Gets or sets a value indicating whether the formula is bold.</summary>
  public bool Bold
  {
    get
    {
      return intbold;
    }

    set
    {
      Value = Functions.MakeBold(Value, value);
      intbold = value;
    }
  }

  /// <summary>
  /// Creates a Formula instance, if it is a formula.
  /// </summary>
  /// <param name="formula">Possible formula.</param>
  /// <param name="c">Affected column number.</param>
  /// <param name="r">Affected row number.</param>
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
    var m = SumRegex().Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "sum", bold, GetColumnIndex(m.Groups[2].Value), Functions.ToInt32(m.Groups[3].Value) - 1, GetColumnIndex(m.Groups[4].Value), Functions.ToInt32(m.Groups[5].Value) - 1);
    }
    m = CountRegex().Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "count", bold, GetColumnIndex(m.Groups[2].Value), Functions.ToInt32(m.Groups[3].Value) - 1, GetColumnIndex(m.Groups[4].Value), Functions.ToInt32(m.Groups[5].Value) - 1);
    }
    m = DaysRegex().Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "days", bold, GetColumnIndex(m.Groups[2].Value), Functions.ToInt32(m.Groups[3].Value) - 1, GetColumnIndex(m.Groups[4].Value), Functions.ToInt32(m.Groups[5].Value) - 1);
    }
    m = NowRegex().Match(formula);
    if (m.Success)
    {
      return new Formula(formula, c, r, "now", bold);
    }
    m = TodayRegex().Match(formula);
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
  /// <returns>Default name of column.</returns>
  public static string GetColumnName(int column)
  {
    if (column < 0)
      return " ";
    var sb = new StringBuilder();
    for (; column >= 0; column = (column / 26) - 1)
    {
      sb.Insert(0, (char)((char)(column % 26) + 'A'));
    }
    return sb.ToString();
  }

  /// <summary>
  /// Calculates the row number for a column name using spreadsheet conventions: A, B, C, ... Z, AA, AB, etc.
  /// </summary>
  /// <param name="name">Affected column name.</param>
  /// <returns>Column number.</returns>
  public static int GetColumnIndex(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return -1;
    var str = name.ToUpper();
    var column = 0;
    var p = 1;
    for (var i = str.Length - 1; i >= 0; i--)
    {
      column += (((str[i] - 'A') % 26) + 1) * p;
      p *= 26;
    }
    return column - 1;
  }

  /// <summary>
  /// Gets string with formatted formula.
  /// </summary>
  /// <returns>Formatted formula.</returns>
  public override string ToString()
  {
    var en = !Functions.IsDe;
    var f = en ? Function
      : Function == "sum" ? "summe" : Function == "count" ? "anzahl" : Function == "days" ? "tage" : Function == "now" ? "jetzt" : "heute";
    var sb = new StringBuilder();
    sb.Append('=').Append(f).Append('(');
    if (Column1 >= 0 && Row1 >= 0)
      sb.Append($"{GetColumnName(Column1)}{Row1 + 1}".ToLower());
    if (Column2 >= 0 && Row2 >= 0)
    {
      sb.Append(Function == "days" ? ";" : ":");
      sb.Append($"{GetColumnName(Column2)}{Row2 + 1}".ToLower());
    }
    sb.Append(')');
    return sb.ToString();
  }

  /// <summary>Regex for formula sum.</summary>
  [GeneratedRegex("^=(sum|summe)\\(([a-z]+)(\\d+):([a-z]+)(\\d+)\\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex SumRegex();

  /// <summary>Regex for formula count.</summary>
  [GeneratedRegex("^=(count|anzahl)\\(([a-z]+)(\\d+):([a-z]+)(\\d+)\\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex CountRegex();

  /// <summary>Regex for formula count.</summary>
  [GeneratedRegex("^=(days|tage)\\(([a-z]+)(\\d+);([a-z]+)(\\d+)\\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex DaysRegex();

  /// <summary>Regex for formula today.</summary>
  [GeneratedRegex("^=(today|heute)(\\(\\))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex TodayRegex();

  /// <summary>Regex for formula now.</summary>
  [GeneratedRegex("^=(now|jetzt)(\\(\\))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled, "de-DE")]
  private static partial Regex NowRegex();
}

/// <summary>
/// List of formulas and status of editing.
/// </summary>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
public class Formulas
{
  /// <summary>Gets the list of formulas.</summary>
  public List<Formula> List { get; } = new List<Formula>();

  /// <summary>Gets the affected cell.</summary>
  public Tuple<int, int> Cell { get; private set; }

  /// <summary>
  /// Gets formula of a cell.
  /// </summary>
  /// <param name="c">Affected column number.</param>
  /// <param name="r">Affected row number.</param>
  /// <returns>Formula or null.</returns>
  public Formula Get(int c, int r)
  {
    if (c < 2)
      return null;
    var f = List.FirstOrDefault(a => a.Column == c - 2 && a.Row == r);
    return f;
  }

  /// <summary>
  /// Begins the editing of a cell.
  /// </summary>
  /// <param name="cnr">Affected column number.</param>
  /// <param name="rnr">Affected row number.</param>
  /// <returns>Is editing OK or not.</returns>
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
        b = cnr == cell.Item1 && rnr == cell.Item2;
    }
    //// Debug.Print($"BeginEdit cnr {cnr} rnr {rnr}");
    return b;
  }

  /// <summary>
  /// Begins the editing of a cell.
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
    var v = default(GLib.Value);
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
        var v0 = default(GLib.Value);
        store.GetValue(it0, cell.Item1, ref v0);
        var newtext = v0.Val as string;
        EndEdit(store, cell.Item1, cell.Item2, newtext);
      }
      Cell = null;
    }
    BeginEdit(cnr, rnr);
    var f = Get(cnr, rnr);
    if (f != null && val != f.Formula1)
    {
      store.SetValue(it, cnr, f.Formula1);
    }
  }

  /// <summary>
  /// Ends the editing of a cell.
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
  /// Calculates all formulas.
  /// </summary>
  /// <param name="store">Affected store.</param>
  public void CalculateFormulas(TreeStore store)
  {
    if (store == null)
      return;
    foreach (var f in List)
    {
      string val = null;
      if (f.Function == "sum")
      {
        if (f.Column1 >= 0 && f.Row1 >= 0 && f.Column2 >= 0 && f.Row2 >= 0)
        {
          var sum = 0m;
          var v = default(GLib.Value);
          for (var r = f.Row1; r <= f.Row2; r++)
          {
            var path = new TreePath(new[] { r });
            store.GetIter(out var it, path);
            for (var c = f.Column1; c <= f.Column2; c++)
            {
              store.GetValue(it, c + 2, ref v);
              sum += Functions.ToDecimalCi(Functions.MakeBold(v.Val as string, true)) ?? 0;
            }
          }
          val = Functions.ToString(sum);
        }
      }
      else if (f.Function == "count")
      {
        if (f.Column1 >= 0 && f.Row1 >= 0 && f.Column2 >= 0 && f.Row2 >= 0)
        {
          var count = 0;
          var v = default(GLib.Value);
          for (var r = f.Row1; r <= f.Row2; r++)
          {
            var path = new TreePath(new[] { r });
            store.GetIter(out var it, path);
            for (var c = f.Column1; c <= f.Column2; c++)
            {
              store.GetValue(it, c + 2, ref v);
              if (!string.IsNullOrWhiteSpace(Functions.MakeBold(v.Val as string, true)))
                count++;
            }
          }
          val = Functions.ToString(count);
        }
      }
      else if (f.Function == "days")
      {
        val = "";
        if (f.Column1 >= 0 && f.Row1 >= 0 && f.Column2 >= 0 && f.Row2 >= 0)
        {
          var v = default(GLib.Value);
          var path = new TreePath(new[] { f.Row1 });
          store.GetIter(out var it, path);
          store.GetValue(it, f.Column1 + 2, ref v);
          var val1 = Functions.ToDateTime(Functions.MakeBold(v.Val as string, true));
          if (val1.HasValue)
          {
            if (f.Row1 != f.Row2)
            {
              path = new TreePath(new[] { f.Row2 });
              store.GetIter(out it, path);
            }
            store.GetValue(it, f.Column2 + 2, ref v);
            var val2 = Functions.ToDateTime(Functions.MakeBold(v.Val as string, true));
            if (val2.HasValue)
              val = Functions.ToString((long)(val1.Value - val2.Value).TotalDays, 0);
          }
        }
      }
      else if (f.Function == "now")
      {
        val = Functions.ToString(DateTime.Now, true);
      }
      else if (f.Function == "today")
      {
        val = Functions.ToString(DateTime.Today, false);
      }
      if (val != null)
      {
        f.Value = Functions.MakeBold(val, !f.Bold);
        Debug.Print($"Calculate {f.Formula1} {f.Value}");
        var tp = new TreePath(new[] { f.Row });
        store.GetIter(out var it, tp);
        store.SetValue(it, f.Column + 2, f.Value);
      }
    }
  }

  /// <summary>
  /// Adds a row.
  /// </summary>
  /// <param name="rnr">Affected row number.</param>
  public void AddRow(int rnr)
  {
    Debug.Print($"AddRow rnr {rnr}");
    foreach (var f in List)
    {
      if (f.Row >= rnr)
        f.Row++;
      if (f.Row1 >= 0 && f.Row1 >= rnr)
        f.Row1++;
      if (f.Row2 >= 0 && f.Row2 >= rnr)
        f.Row2++;
      f.Formula1 = f.ToString();
    }
  }

  /// <summary>
  /// Deletes a row.
  /// </summary>
  /// <param name="rnr">Affected row number.</param>
  public void DeleteRow(int rnr)
  {
    Debug.Print($"DeleteRow rnr {rnr}");
    var l = new List<Formula>();
    foreach (var f in List)
    {
      if (f.Row == rnr)
        l.Add(f);
      else
      {
        if (f.Row > rnr)
          f.Row--;
        if (f.Row1 >= 0 && f.Row1 > rnr)
          f.Row1--;
        if (f.Row2 >= 0 && f.Row2 > rnr)
          f.Row2--;
        f.Formula1 = f.ToString();
      }
    }
    foreach (var f in l)
    {
      List.Remove(f);
    }
  }

  /// <summary>
  /// Adds a column.
  /// </summary>
  /// <param name="cnr">Affected column number.</param>
  public void AddColumn(int cnr)
  {
    Debug.Print($"AddColumn cnr {cnr}");
    foreach (var f in List)
    {
      if (f.Column >= cnr)
        f.Column++;
      if (f.Column1 >= 0 && f.Column1 >= cnr)
        f.Column1++;
      if (f.Column2 >= 0 && f.Column2 >= cnr)
        f.Column2++;
      f.Formula1 = f.ToString();
    }
  }

  /// <summary>
  /// Deletes a column.
  /// </summary>
  /// <param name="cnr">Affected column number.</param>
  public void DeleteColumn(int cnr)
  {
    Debug.Print($"DeleteColumn cnr {cnr}");
    var l = new List<Formula>();
    foreach (var f in List)
    {
      if (f.Column == cnr)
        l.Add(f);
      else
      {
        if (f.Column > cnr)
          f.Column--;
        if (f.Column1 >= 0 && f.Column1 > cnr)
          f.Column1--;
        if (f.Column2 >= 0 && f.Column2 > cnr)
          f.Column2--;
        f.Formula1 = f.ToString();
      }
    }
    foreach (var f in l)
    {
      List.Remove(f);
    }
  }

  /// <summary>
  /// Ends the editing of a cell.
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
    var v = default(GLib.Value);
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
    else
    {
      // if (val != newtext)
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
}
