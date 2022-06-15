// <copyright file="AM510Dialogs.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM;

using System;
using System.Collections.Generic;
using CSBP.Apis.Enums;
using CSBP.Base;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>Controller for AM510Dialogs dialog.</summary>
public partial class AM510Dialogs : CsbpBin
{
#pragma warning disable CS0649

  /// <summary>TreeView dialoge.</summary>
  [Builder.Object]
  private readonly TreeView dialoge;

  /// <summary>Button zuordnen.</summary>
  [Builder.Object]
  private readonly Button zuordnen;

  /// <summary>Button entfernen.</summary>
  [Builder.Object]
  private readonly Button entfernen;

  /// <summary>TreeView zudialoge.</summary>
  [Builder.Object]
  private readonly TreeView zudialoge;

#pragma warning restore CS0649

  /// <summary>Dialog model.</summary>
  private readonly List<string> model = new();

  /// <summary>Initializes a new instance of the <see cref="AM510Dialogs"/> class.</summary>
  /// <param name="b">Affected Builder.</param>
  /// <param name="h">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  public AM510Dialogs(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    dialoge.GrabFocus();
  }

  /// <summary>Creates non modal dialog.</summary>
  /// <param name="p1">1. parameter for dialog.</param>
  /// <param name="p">Affected parent dialog.</param>
  /// <returns>Created dialog.</returns>
  public static AM510Dialogs Create(object p1 = null, CsbpBin p = null)
  {
    return new AM510Dialogs(GetBuilder("AM510Dialogs", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected override void InitData(int step)
  {
    if (step <= 0)
    {
      dialoge.Selection.Mode = SelectionMode.Multiple;
      zudialoge.Selection.Mode = SelectionMode.Multiple;

      var sd = Parameter.GetValue(Parameter.AG_STARTDIALOGE) ?? "";
      var arr = sd.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
      model.AddRange(arr);
      FillLists();
    }
  }

  /// <summary>Handles Dialoge.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDialogeRowActivated(object sender, RowActivatedArgs e)
  {
    zuordnen.Click();
  }

  /// <summary>Handles Zuordnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZuordnenClicked(object sender, EventArgs e)
  {
    var tv = dialoge;
    var sel = GetSelected(tv);
    foreach (var s in sel)
    {
      model.Add(s);
    }
    tv.Selection.UnselectAll();
    FillLists();
  }

  /// <summary>Handles Entfernen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEntfernenClicked(object sender, EventArgs e)
  {
    var tv = zudialoge;
    var sel = GetSelected(tv);
    foreach (var s in sel)
    {
      model.Remove(s);
    }
    tv.Selection.UnselectAll();
    FillLists();
  }

  /// <summary>Handles Zudialoge.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZudialogeRowActivated(object sender, RowActivatedArgs e)
  {
    entfernen.Click();
  }

  /// <summary>Handles Oben.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnObenClicked(object sender, EventArgs e)
  {
    var tv = zudialoge;
    var s = tv.Selection.GetSelectedRows();
    if (s.Length != 1)
      return;
    foreach (var sel in s)
    {
      if (tv.Model.GetIter(out var iter, sel))
      {
        if (tv.Model.GetValue(iter, 0) is string value)
        {
          var i = model.IndexOf(value);
          if (i > 0)
          {
            model.Remove(value);
            model.Insert(i - 1, value);
          }
        }
      }
    }
    FillLists();
  }

  /// <summary>Handles Unten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnUntenClicked(object sender, EventArgs e)
  {
    var tv = zudialoge;
    var s = tv.Selection.GetSelectedRows();
    if (s.Length != 1)
      return;
    foreach (var sel in s)
    {
      if (tv.Model.GetIter(out var iter, sel))
      {
        if (tv.Model.GetValue(iter, 0) is string value)
        {
          var i = model.IndexOf(value);
          if (i >= 0 && i < model.Count - 1)
          {
            model.Remove(value);
            model.Insert(i + 1, value);
          }
        }
      }
    }
    FillLists();
  }

  /// <summary>Handles Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    var daten = ServiceDaten;
    var sd = string.Join("|", model.ToArray());
    if (Get(FactoryService.ClientService.SaveOption(daten, daten.MandantNr,
        Parameter.Params[Parameter.AG_STARTDIALOGE], sd)))
    {
      dialog.Hide();
    }
  }

  /// <summary>Handles Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }

  /// <summary>
  /// Gets selected ids.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <returns>List of selected ids.</returns>
  private static List<string> GetSelected(TreeView tv)
  {
    var list = new List<string>();
    var s = tv.Selection.GetSelectedRows();
    if (s.Length <= 0)
      return list;
    foreach (var sel in s)
    {
      if (tv.Model.GetIter(out var iter, sel))
      {
        if (tv.Model.GetValue(iter, 0) is string value)
        {
          list.Add(value);
        }
      }
    }
    return list;
  }

  /// <summary>
  /// Fill the lists.
  /// </summary>
  private void FillLists()
  {
    var sel = GetSelected(dialoge);
    var sel2 = GetSelected(zudialoge);
    var list = StartDialog.Dialoge;
#pragma warning disable CS0618
    //// No.;Abbreviation;Title
    var store = AddStringColumns(dialoge, AM510_dialoge_columns);
    var store2 = AddStringColumns(zudialoge, AM510_zudialoge_columns);
#pragma warning restore CS0618
    foreach (var mp in list)
    {
      var key = $"#{mp.Key}";
      if (!model.Contains(key))
        store.AppendValues(key, mp.Key, mp.Value.Title);
    }
    foreach (var key in model)
    {
      var schluessel = key[1..];
      if (list.TryGetValue(schluessel, out var mp))
        store2.AppendValues(key, schluessel, mp.Title);
    }
    if (sel.Count > 0 && store.GetIterFirst(out var i))
    {
      do
      {
        if (store.GetValue(i, 0) is string val && sel.Contains(val))
          dialoge.Selection.SelectIter(i);
      }
      while (store.IterNext(ref i));
    }
    if (sel2.Count > 0 && store2.GetIterFirst(out var i2))
    {
      do
      {
        if (store2.GetValue(i2, 0) is string val && sel2.Contains(val))
          zudialoge.Selection.SelectIter(i2);
      }
      while (store2.IterNext(ref i2));
    }
  }
}
