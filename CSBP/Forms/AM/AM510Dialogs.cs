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
  /// <summary>Dialog Model.</summary>
  private readonly List<string> Model = new();

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

  /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public static AM510Dialogs Create(object p1 = null, CsbpBin p = null)
  {
    return new AM510Dialogs(GetBuilder("AM510Dialogs", out var handle), handle, p1: p1, p: p);
  }

  /// <summary>Konstruktor für modalen Dialog.</summary>
  /// <param name="b">Betroffener Builder.</param>
  /// <param name="h">Betroffenes Handle vom Builder.</param>
  /// <param name="d">Betroffener einbettender Dialog.</param>
  /// <param name="dt">Betroffener Dialogtyp.</param>
  /// <param name="p1">1. Parameter für Dialog.</param>
  /// <param name="p">Betroffener Eltern-Dialog.</param>
  /// <returns>Nicht-modalen Dialogs.</returns>
  public AM510Dialogs(Builder b, IntPtr h, Dialog d = null, DialogTypeEnum dt = DialogTypeEnum.Without, object p1 = null, CsbpBin p = null)
      : base(b, h, d, dt, p1, p)
  {
    // SetBold(client0);
    InitData(0);
    dialoge.GrabFocus();
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
      Model.AddRange(arr);
      FillLists();
    }
  }

  /// <summary>Handle Dialoge.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDialogeRowActivated(object sender, RowActivatedArgs e)
  {
    zuordnen.Click();
  }

  /// <summary>Handle Zuordnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZuordnenClicked(object sender, EventArgs e)
  {
    var tv = dialoge;
    var sel = GetSelected(tv);
    foreach (var s in sel)
    {
      Model.Add(s);
    }
    tv.Selection.UnselectAll();
    FillLists();
  }

  /// <summary>Handle Entfernen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnEntfernenClicked(object sender, EventArgs e)
  {
    var tv = zudialoge;
    var sel = GetSelected(tv);
    foreach (var s in sel)
    {
      Model.Remove(s);
    }
    tv.Selection.UnselectAll();
    FillLists();
  }

  /// <summary>Handle Zudialoge.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnZudialogeRowActivated(object sender, RowActivatedArgs e)
  {
    entfernen.Click();
  }

  /// <summary>Handle Oben.</summary>
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
          var i = Model.IndexOf(value);
          if (i > 0)
          {
            Model.Remove(value);
            Model.Insert(i - 1, value);
          }
        }
      }
    }
    FillLists();
  }

  /// <summary>Handle Unten.</summary>
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
          var i = Model.IndexOf(value);
          if (i >= 0 && i < Model.Count - 1)
          {
            Model.Remove(value);
            Model.Insert(i + 1, value);
          }
        }
      }
    }
    FillLists();
  }

  /// <summary>Handle Ok.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnOkClicked(object sender, EventArgs e)
  {
    var daten = ServiceDaten;
    var sd = string.Join("|", Model.ToArray());
    if (Get(FactoryService.ClientService.SaveOption(daten, daten.MandantNr,
        Parameter.Params[Parameter.AG_STARTDIALOGE], sd)))
    {
      dialog.Hide();
    }
  }

  /// <summary>Handle Abbrechen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnAbbrechenClicked(object sender, EventArgs e)
  {
    dialog.Hide();
  }

  private void FillLists()
  {
    var sel = GetSelected(dialoge);
    var sel2 = GetSelected(zudialoge);

    var list = StartDialog.Dialoge;
#pragma warning disable CS0618
    // Nr.;Kürzel;Titel
    var store = AddStringColumns(dialoge, AM510_dialoge_columns);
    var store2 = AddStringColumns(zudialoge, AM510_zudialoge_columns);
#pragma warning restore CS0618
    foreach (var mp in list)
    {
      var key = $"#{mp.Key}";
      if (!Model.Contains(key))
        store.AppendValues(key, mp.Key, mp.Value.Title);
    }
    foreach (var key in Model)
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
      } while (store.IterNext(ref i));
    }
    if (sel2.Count > 0 && store2.GetIterFirst(out var i2))
    {
      do
      {
        if (store2.GetValue(i2, 0) is string val && sel2.Contains(val))
          zudialoge.Selection.SelectIter(i2);
      } while (store2.IterNext(ref i2));
    }
  }

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
}
