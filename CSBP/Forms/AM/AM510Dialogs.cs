// <copyright file="AM510Dialogs.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.AM
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Enums;
  using CSBP.Base;
  using CSBP.Services.Factory;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>Controller für AM510Dialogs Dialog.</summary>
  public partial class AM510Dialogs : CsbpBin
  {
    /// <summary>Dialog Model.</summary>
    List<string> Model = new List<string>();

#pragma warning disable 169, 649

    /// <summary>Label dialoge0.</summary>
    [Builder.Object]
    private Label dialoge0;

    /// <summary>TreeView dialoge.</summary>
    [Builder.Object]
    private TreeView dialoge;

    /// <summary>Button zuordnen.</summary>
    [Builder.Object]
    private Button zuordnen;

    /// <summary>Button entfernen.</summary>
    [Builder.Object]
    private Button entfernen;

    /// <summary>Label zudialoge0.</summary>
    [Builder.Object]
    private Label zudialoge0;

    /// <summary>TreeView zudialoge.</summary>
    [Builder.Object]
    private TreeView zudialoge;

    /// <summary>Button oben.</summary>
    [Builder.Object]
    private Button oben;

    /// <summary>Button unten.</summary>
    [Builder.Object]
    private Button unten;

    /// <summary>Button ok.</summary>
    [Builder.Object]
    private Button ok;

    /// <summary>Button abbrechen.</summary>
    [Builder.Object]
    private Button abbrechen;

#pragma warning restore 169, 649

    /// <summary>Erstellen des nicht-modalen Dialogs.</summary>
    /// <param name="p1">1. Parameter für Dialog.</param>
    /// <param name="p">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public static AM510Dialogs Create(object p1 = null, CsbpBin p = null)
    {
      return new AM510Dialogs(GetBuilder("AM510Dialogs", out var handle), handle, p1: p1, p: p);
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
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

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    override protected void InitData(int step)
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

    /// <summary>Behandlung von Dialoge.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDialogeRowActivated(object sender, RowActivatedArgs e)
    {
      zuordnen.Click();
    }

    /// <summary>Behandlung von Zuordnen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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

    /// <summary>Behandlung von Entfernen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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

    /// <summary>Behandlung von Zudialoge.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnZudialogeRowActivated(object sender, RowActivatedArgs e)
    {
      entfernen.Click();
    }

    /// <summary>Behandlung von Oben.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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
          var value = tv.Model.GetValue(iter, 0) as string;
          if (value != null)
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

    /// <summary>Behandlung von Unten.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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
          var value = tv.Model.GetValue(iter, 0) as string;
          if (value != null)
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

    /// <summary>Behandlung von Ok.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
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

    /// <summary>Behandlung von Abbrechen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnAbbrechenClicked(object sender, EventArgs e)
    {
      dialog.Hide();
    }

    private void FillLists()
    {
      var sel = GetSelected(dialoge);
      var sel2 = GetSelected(zudialoge);

      var list = StartDialog.Dialoge;
#pragma warning disable 618
      // Nr.;Kürzel;Titel
      var store = AddStringColumns(dialoge, AM510_dialoge_columns);
      var store2 = AddStringColumns(zudialoge, AM510_zudialoge_columns);
#pragma warning restore 618
      foreach (var mp in list)
      {
        var key = $"#{mp.Key}";
        if (!Model.Contains(key))
          store.AppendValues(key, mp.Key, mp.Value.Title);
      }
      foreach (var key in Model)
      {
        var schluessel = key.Substring(1);
        if (list.TryGetValue(schluessel, out var mp))
          store2.AppendValues(key, schluessel, mp.Title);
      }
      if (sel.Count > 0 && store.GetIterFirst(out var i))
      {
        do
        {
          var val = store.GetValue(i, 0) as string;
          if (val != null && sel.Contains(val))
            dialoge.Selection.SelectIter(i);
        } while (store.IterNext(ref i));
      }
      if (sel2.Count > 0 && store2.GetIterFirst(out var i2))
      {
        do
        {
          var val = store2.GetValue(i2, 0) as string;
          if (val != null && sel2.Contains(val))
            zudialoge.Selection.SelectIter(i2);
        } while (store2.IterNext(ref i2));
      }
    }

    private List<string> GetSelected(TreeView tv)
    {
      var list = new List<string>();
      var s = tv.Selection.GetSelectedRows();
      if (s.Length <= 0)
        return list;
      foreach (var sel in s)
      {
        if (tv.Model.GetIter(out var iter, sel))
        {
          var value = tv.Model.GetValue(iter, 0) as string;
          if (value != null)
          {
            list.Add(value);
          }
        }
      }
      return list;
    }
  }
}
