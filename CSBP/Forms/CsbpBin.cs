// <copyright file="CsbpBin.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms
{
  using System;
  using System.Collections.Generic;
  using Gtk;
  using static CSBP.Resources.Messages;
  using CSBP.Apis.Enums;
  using CSBP.Apis.Services;
  using CSBP.Resources;
  using CSBP.Base;
  using System.Drawing;
  using System.Reactive.Linq;
  using CSBP.Apis.Models;
  using System.Diagnostics;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  [System.ComponentModel.ToolboxItem(false)]
  public partial class CsbpBin : Bin
  {
    public static int TitleHeight { get; private set; } = 37;

    protected Builder Builder { get; private set; }

    protected Dialog dialog;

    protected DialogTypeEnum DialogType { get; private set; }

    protected CsbpBin CsbpParent { get; private set; }

    protected object Parameter1 { get; private set; }

    protected object Response { get; set; }

    protected ServiceDaten ServiceDaten => MainClass.ServiceDaten;

    protected bool EventsActive;

    protected static Builder GetBuilder(string name, out IntPtr handle)
    {
      var builder = new Builder($"CSBP.GtkGui.{name.Substring(0, 2)}.{name}.glade");
      handle = builder.GetObject(name).Handle;
      return builder;
    }

    /// <summary>Initialisierung der Events.</summary>
    virtual protected void SetupHandlers()
    {
    }

    /// <summary>Konstruktor für modalen Dialog.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="handle">Betroffenes Handle vom Builder.</param>
    /// <param name="d">Betroffener einbettender Dialog.</param>
    /// <param name="dt">Betroffener Dialogtyp.</param>
    /// <param name="parameter1">1. Parameter für Dialog.</param>
    /// <param name="csbpparent">Betroffener Eltern-Dialog.</param>
    /// <returns>Nicht-modalen Dialogs.</returns>
    public CsbpBin(Builder builder, IntPtr handle, Dialog d, DialogTypeEnum dt = DialogTypeEnum.Without, object parameter1 = null,
        CsbpBin csbpparent = null) : base(handle)
    {
      Builder = builder;
      dialog = d;
      DialogType = dt;
      CsbpParent = csbpparent;
      Parameter1 = parameter1;
      builder.Autoconnect(this);
      GetChildren().ForEach(c =>
      {
        if (!string.IsNullOrEmpty(c.TooltipText) && c.TooltipText.EndsWith(".tt", StringComparison.CurrentCulture))
        {
          c.TooltipText = Messages.Get(c.TooltipText);
        }
        var e = c as Entry;
        if (e != null && !string.IsNullOrEmpty(e.PlaceholderText) && e.PlaceholderText.EndsWith(".tt", StringComparison.CurrentCulture))
        {
          e.PlaceholderText = Messages.Get(e.PlaceholderText);
        }
        var lbl = c as Label;
        if (lbl != null && !string.IsNullOrEmpty(lbl.LabelProp) && lbl.LabelProp.Contains(".", StringComparison.CurrentCulture))
        {
          lbl.LabelProp = Messages.Get(lbl.LabelProp);
        }
        var btn = c as Button;
        if (btn != null)
        {
          if (!string.IsNullOrEmpty(btn.TooltipText) && btn.TooltipText.StartsWith("Action.", StringComparison.CurrentCulture))
            btn.TooltipText = Messages.Get(btn.TooltipText);
          if (!string.IsNullOrEmpty(btn.Label) && btn.Label.Contains(".", StringComparison.CurrentCulture))
            btn.Label = Messages.Get(btn.Label);
        }
      });
      SetupHandlers();
    }

    /// <summary>Starten eines modalen oder nicht-modalen Dialogs.</summary>
    /// <param name="type">Betroffener Dialog-Type.</param>
    /// <param name="title">Betroffener Titel.</param>
    /// <param name="dialogType">Betroffener Dialogtyp.</param>
    /// <param name="parameter1">1. Parameter für Dialog.</param>
    /// <param name="modal">Soll der Dialog modal geöffnet werden?</param>
    /// <param name="p">Betroffener Haupt-Dialog.</param>
    /// <param name="csbpparent">Betroffener direkter Eltern-Dialog.</param>
    /// <returns>Ergebnis ist nur bei modalem Dialog sinnvoll.</returns>
    public static object Start(Type type, string title = null,
      DialogTypeEnum dialogType = DialogTypeEnum.Without, object parameter1 = null,
      bool modal = false, Gtk.Window p = null, CsbpBin csbpparent = null)
    {
      p = p ?? MainClass.MainWindow;
      var f = DialogFlags.DestroyWithParent;
      if (modal)
        f |= DialogFlags.Modal;
      if (dialogType != DialogTypeEnum.Without)
      {
        var x = dialogType == DialogTypeEnum.New ? Enum_dialog_new
          : dialogType == DialogTypeEnum.Copy ? Enum_dialog_copy
          : dialogType == DialogTypeEnum.Copy2 ? Enum_dialog_copy2
          : dialogType == DialogTypeEnum.Edit ? Enum_dialog_edit
          : dialogType == DialogTypeEnum.Delete ? Enum_dialog_delete
          : dialogType == DialogTypeEnum.Reverse ? Enum_dialog_reverse
          : "";
        title = $"{(title ?? "")} - {x}";
      }

      //using (var dialog = new Dialog(title, p, f))
      //{
      var dialog = new Dialog(title, p, f);
      var builder = GetBuilder(type.Name, out var handle);
      var form = Activator.CreateInstance(type, builder, handle, dialog, dialogType, parameter1, csbpparent) as CsbpBin;
      //form.ShowAll();
      dialog.ContentArea.PackStart(form, true, true, 0);
      var size = Parameter.GetDialogSize(type);
      dialog.DefaultWidth = size.Width;
      dialog.DefaultHeight = size.Height;
      //dialog.SetSizeRequest(200, 100);
      if (size.X == -1 && size.Y == -1)
        dialog.SetPosition(WindowPosition.Center);
      else
        dialog.Move(size.X, size.Y);
      var ob = Observable.FromEvent<SizeAllocatedHandler, SizeAllocatedArgs>(
        h0 =>
        {
          SizeAllocatedHandler h = (sender, e) => { h0(e); };
          return h;
        },
        h => dialog.SizeAllocated += h, h => dialog.SizeAllocated -= h
      ).Throttle(TimeSpan.FromMilliseconds(1000));
      ob.Subscribe(e =>
      {
        Application.Invoke(delegate
        {
          if (dialog.Window != null)
          {
            dialog.Window.GetGeometry(out int x0, out int y0, out int w, out int h);
            dialog.Window.GetOrigin(out int x, out int y);
            // Höhe der Titelleiste abziehen
            Parameter.SetDialogSize(type, new Rectangle(x, y - TitleHeight, w, h));
            //Console.WriteLine($"{x} {y} {w} {h}");
            //Console.WriteLine($"{DateTime.Now}");
          }
        });
      });
      dialog.DefaultResponse = ResponseType.Cancel;
      var def = form.GetDefaultButton();
      if (def != null)
        dialog.Default = def;
      dialog.ShowAll();
      if (modal)
      {
        dialog.Run();
        //dialog.Hide();
        dialog.Dispose();
      }
      return form.Response;
      //}
    }

    /// <summary>Fokussieren oder Starten eines nicht-modalen Dialogs.</summary>
    /// <param name="title">Betroffener Titel.</param>
    /// <param name="dialogType">Betroffener Dialogtyp.</param>
    /// <param name="parameter1">1. Parameter für Dialog.</param>
    /// <param name="modal">Soll der Dialog modal geöffnet werden?</param>
    /// <param name="p">Betroffener Haupt-Dialog.</param>
    /// <param name="csbpparent">Betroffener direkter Eltern-Dialog.</param>
    /// <returns>Ergebnis ist nur bei modalem Dialog sinnvoll.</returns>
    public static T Focus<T>(string title = null,
      DialogTypeEnum dialogType = DialogTypeEnum.Without, object parameter1 = null,
      bool modal = false, Gtk.Window p = null, CsbpBin csbpparent = null) where T : CsbpBin
    {
      var dlg = MainClass.MainWindow.FocusPage<T>();
      if (dlg == null)
      {
        var create = typeof(T).GetMethod("Create");
        dlg = create.Invoke(null, new[] { parameter1, csbpparent }) as T;
        MainClass.MainWindow.AppendPage(dlg, title);
      }
      return dlg;
    }

    /// <summary>Model-Daten initialisieren.</summary>
    /// <param name="step">Betroffener Schritt: 0 erstmalig, 1 aktualisieren.</param>
    protected virtual void InitData(int step)
    {
    }

    /// <summary>Aktualisierung des Eltern-Dialogs.</summary>
    protected virtual void UpdateParent()
    {
      var p = CsbpParent;
      if (p != null)
        p.UpdateParent();
    }

    private List<Widget> GetChildren(Container con = null, List<Widget> l = null)
    {
      if (con == null)
        con = this;
      if (l == null)
        l = new List<Widget>();
      var array = con.Children;
      foreach (var c in array)
      {
        l.Add(c);
        if (c is Container)
          GetChildren(c as Container, l);
      }
      return l;
    }

    protected Widget GetDefaultButton(Container con = null)
    {
      if (con == null)
        con = this;
      var array = con.Children;
      foreach (var c in array)
      {
        if (c is Container)
        {
          var w = GetDefaultButton(c as Container);
          if (w != null)
            return w;
        }
        if (c is Button && c.CanDefault)
          return c;
      }
      return null;
    }

    /// <summary>
    /// Abfangen und Drosseln eines Events.
    /// </summary>
    /// <param name="w">Betroffenes Widget.</param>
    /// <param name="h">Neuer Eventhandler.</param>
    /// <param name="eventname">Optionaler Event-Name.</param>
    /// <param name="millis">Verzögerung in Millisekunden.</param>
    protected void ObservableEventThrottle(Widget w, EventHandler h, string eventname = "Clicked", int millis = 500)
    {
      var ob = Observable.FromEventPattern<EventArgs>(w, eventname)
        .Throttle(TimeSpan.FromMilliseconds(millis));
      ob.Subscribe(e => Application.Invoke(h));
    }

    public static bool ShowYesNoQuestion(string msg)
    {
      int r;
      using (var md = new MessageDialog(MainClass.MainWindow,
          DialogFlags.DestroyWithParent, MessageType.Question,
          ButtonsType.YesNo, msg))
      {
        r = md.Run();
        // md.Hide();
        md.Dispose();
      }
      return r == (int)ResponseType.Yes;
    }

    public virtual object Close()
    {
      return null;
    }

    public static T Get<T>(ServiceErgebnis<T> r, bool dialog = true)
    {
      return MainClass.Get(r, dialog);
    }

    public static bool Get(ServiceErgebnis r, bool dialog = true)
    {
      return MainClass.Get(r, dialog);
    }

    public static void ShowInfo(string s, bool dialog = true)
    {
      MainClass.ShowInfo(s, dialog);
    }

    public static void ShowError(string s, bool dialog = true)
    {
      MainClass.ShowError(s, dialog);
    }

    protected void RefreshTreeView(TreeView tv, int step)
    {
      var v = GetValue<string>(tv, false);
      var s = tv.Selection.GetSelectedRows();
      var si = -1;
      var so = SortType.Ascending;
      for (var i = 1; i < tv.NColumns; i++)
      {
        if (tv.GetColumn(i).SortIndicator)
        {
          so = tv.GetColumn(i).SortOrder;
          si = i;
          break;
        }
      }
      InitData(step);
      if (si > 0)
      {
        // Sortierung asynchron wiederherstellen
        Task.Run(() =>
        {
          var x = 0;
          try
          {
            // tv.GetColumn(si).SortOrder = SortType.Ascending;
            Debug.WriteLine($"1 Soll {so} Ist {tv.GetColumn(si).SortOrder}");
            Application.Invoke(delegate
            {
              tv.GetColumn(si).Button.Activate();
              x = 1;
            });
            while (x < 1)
              Thread.Sleep(100);
            // Debug.WriteLine($"2 Soll {so} Ist {tv.GetColumn(si).SortOrder}");
            if (so == SortType.Descending)
            {
              Thread.Sleep(300);
              Application.Invoke(delegate
              {
                tv.GetColumn(si).Button.Activate();
                x = 2;
              });
              while (x < 2)
                Thread.Sleep(100);
              // Debug.WriteLine($"3 Soll {so} Ist {tv.GetColumn(si).SortOrder}");
            }
            Thread.Sleep(200);
            Application.Invoke(delegate
            {
              if (string.IsNullOrEmpty(v))
              {
                foreach (var p in s)
                  tv.Selection.SelectPath(p);
              }
              else
                SetText(tv, v);
            });
          }
          catch (Exception ex)
          {
            Functions.MachNichts(ex);
          }
          return 0;
        });
      }
      else
      {
        if (string.IsNullOrEmpty(v))
        {
          foreach (var p in s)
            tv.Selection.SelectPath(p);
        }
        else
          SetText(tv, v);
      }
    }

    /// <summary>
    /// Gets the value of a TreeView.
    /// </summary>
    /// <returns>The value.</returns>
    /// <param name="tv">Specific TreeView.</param>
    /// <param name="mandatory">Is the value mandatory?</param>
    /// <param name="column">The column number.</param>
    protected T GetValue<T>(TreeView tv, bool mandatory = true, int column = 0) where T : class
    {
      T value = default(T);
      var s = tv.Selection.GetSelectedRows();
      if (s.Length > 0 && tv.Model.GetIter(out var iter, s[0]))
      {
        var v = new GLib.Value();
        tv.Model.GetValue(iter, column, ref v);
        value = v.Val as T;
      }
      else if (s.Length <= 0 && tv.Model != null && tv.Model.IterNChildren() == 1)
      {
        if (tv.Model.GetIterFirst(out var iter1))
        {
          var v = new GLib.Value();
          tv.Model.GetValue(iter1, column, ref v);
          value = v.Val as T;
        };
      }
      if (mandatory && value == null)
        throw new MessageException(M1013);
      return value;
    }

    /// <summary>
    /// Gets the value of a TreeView.
    /// </summary>
    /// <returns>The value.</returns>
    /// <param name="tv">Specific TreeView.</param>
    /// <param name="mandatory">Is the value mandatory?</param>
    /// <param name="column">The column number.</param>
    protected string GetText(TreeView tv, bool mandatory = false, int column = 0)
    {
      return GetValue<string>(tv, mandatory, column);
    }

    /// <summary>Label mit fetter Schrift.</summary>
    protected void SetBold(GLib.Object w)
    {
      var lbl = w as Label;
      if (lbl != null && !lbl.UseMarkup)
      {
        lbl.UseMarkup = true;
        lbl.LabelProp = $"<b>{lbl.LabelProp}</b>";
      }
    }

    //protected TreeStore AddStringColumns(ComboBox tv, string headers)
    //{
    //    var titles = headers.Split(';');
    //    var types = new Type[titles.Length];
    //    for (var i = 0; i < titles.Length; i++)
    //        types[i] = typeof(string);
    //    return AddColumns(tv, titles, types);
    //}

    [Obsolete("Use AddStringColumnsSort instead.")]
    protected TreeStore AddStringColumns(TreeView tv, string headers)
    {
      var titles = headers.Split(';');
      if (titles.Length <= 1)
        throw new ArgumentException("Too few column headers", nameof(headers));
      var types = new Type[titles.Length];
      for (var i = 0; i < titles.Length; i++)
        types[i] = typeof(string);
      return AddColumns(tv, titles, types);
    }

    /// <summary>TreeView mit editierbaren Daten füllen.
    /// Die Spaltenüberschriften werden wie in Excel gebildet: A, B, C, ... Z.</summary>
    protected void AddStringColumns(TreeView tv, int columns, bool editable = false, List<string[]> values = null)
    {
      var titles = new string[Math.Min(Math.Max(columns + 2, 1), 28)];
      var types = new Type[titles.Length];
      for (var i = 0; i < titles.Length; i++)
      {
        if (i < 2)
          titles[i] = "";
        else
          titles[i] = ((char)((i - 2) % 26 + 'A')).ToString();
        types[i] = typeof(string);
      }
      AddColumns(tv, titles, types, editable, values);
    }

    /// <summary>Liefert eine TreeView-Spalte.</summary>
    private TreeViewColumn GetColumn(string title, int i, bool editable, ITreeModel store)
    {
      var align = 0f; // left
      if (title.EndsWith("_r", StringComparison.InvariantCulture))
      {
        title = title.Substring(0, title.Length - 2);
        align = 1f; // right
      }
      else if (title.EndsWith("_e", StringComparison.InvariantCulture))
      {
        title = title.Substring(0, title.Length - 2);
        editable = true;
      }
      var cell = new CellRendererText
      // var cell = new CellRendererSpin
      {
        Xalign = align,
        Editable = editable,
      };
      if (editable)
      {
        cell.Data["store"] = store;
        cell.Data["cnr"] = i;
        cell.Edited += TableCell_Edited;
      }
      var col = new TreeViewColumn
      {
        SortColumnId = i,
        Resizable = true,
        Alignment = align,
      };
      if (editable)
      {
        col.Data["cnr"] = i;
      }
      var lbl = new Label
      {
        LabelProp = $"<b>{title}</b>",
        UseMarkup = true,
      };
      lbl.Show();
      col.Widget = lbl;
      col.PackStart(cell, true);
      col.AddAttribute(cell, "text", i);
      if (i == 0)
        col.MaxWidth = 13; // Nicht 0 wegen: Negative content width -12 (allocation 1, extents 6x7) while allocating gadget (node button, owner GtkButton)
      return col;
    }

    /// <summary>TreeView mit Spalten und evtl. Daten initialisieren.</summary>
    protected TreeStore AddColumns(TreeView tv, string[] titles, Type[] types, bool editable = false,
        List<string[]> values = null)
    {
      foreach (var c in tv.Columns)
        tv.RemoveColumn(c);
      var store = new TreeStore(types);
      for (var i = 0; i < titles.Length; i++)
      {
        var col = GetColumn(titles[i], i, editable, store);
        tv.AppendColumn(col);
      }
      tv.Model = store;
      if (editable)
      {
        tv.Selection.Mode = SelectionMode.Multiple;
        tv.Data["tv"] = tv;
        tv.Data["store"] = store;
        tv.ButtonReleaseEvent += OnTableButtonReleaseEvent;
        if (values != null)
          foreach (var arr in values)
            store.AppendValues(arr);
      }
      if (titles.Length == 2)
      {
        // tv.Selection.Mode = SelectionMode.Single;
        tv.EnableSearch = true;
        tv.SearchColumn = 1;
      }
      else
        tv.EnableSearch = false;
      return store;
    }

    protected void AddStringColumnsSort(TreeView tv, string headers, List<string[]> values = null)
    {
      var titles = headers.Split(';');
      if (titles.Length <= 1)
        throw new ArgumentException("Too few column headers", nameof(headers));
      var types = new Type[titles.Length];
      for (var i = 0; i < titles.Length; i++)
        types[i] = typeof(string);
      //types[i] = titles[i].EndsWith("_r", StringComparison.InvariantCulture) ? typeof(decimal) : typeof(string);
      AddColumnsSort(tv, titles, types, values: values);
    }

    protected void AddColumnsSort(TreeView tv, string[] titles, Type[] types, bool editable = false,
        List<string[]> values = null)
    {
      foreach (var c in tv.Columns)
        tv.RemoveColumn(c);
      var store = new TreeStore(types);
      for (var i = 0; i < titles.Length; i++)
      {
        var col = GetColumn(titles[i], i, editable, store);
        tv.AppendColumn(col);
      }
      if (values != null)
      {
        foreach (var v in values)
          store.AppendValues(v);
      }
      var sortable = new TreeModelSort(store);
      for (var i = 0; i < titles.Length; i++)
      {
        if (titles[i].EndsWith("_r", StringComparison.InvariantCulture))
        {
          var j = i;
          sortable.SetSortFunc(i, (ITreeModel model, TreeIter a, TreeIter b) =>
          {
            var s1 = Functions.ToDecimal((string)model.GetValue(a, j)) ?? 0;
            var s2 = Functions.ToDecimal((string)model.GetValue(b, j)) ?? 0;
            return Decimal.Compare(s1, s2);
          });
        }
      }
      tv.Model = sortable;
      if (titles.Length == 2)
      {
        // tv.Selection.Mode = SelectionMode.Single;
        tv.EnableSearch = true;
        tv.SearchColumn = 1;
      }
      else
        tv.EnableSearch = false;
    }

    private void TableCell_Edited(object o, EditedArgs args)
    {
      var cr = o as Gtk.CellRenderer;
      if (cr == null)
        return;
      var cnr = (int)cr.Data["cnr"];
      var store = cr.Data["store"] as TreeStore;
      store.GetIter(out var it, new TreePath(args.Path));
      store.SetValue(it, cnr, args.NewText);
    }

    private void OnTableMenuItemClick(object o, ButtonPressEventArgs args)
    {
      var mi = o as MenuItem;
      if (mi == null)
        return;
      var tv = mi.Data["tv"] as TreeView;
      var store = mi.Data["store"] as TreeStore;
      var spalten = store.NColumns;
      var zeilen = 0;
      if (store.GetIterFirst(out var it))
      {
        do
        {
          zeilen++;
        } while (store.IterNext(ref it));
      }
      var s = tv.Selection.GetSelectedRows();
      var znummern = false;
      // Menüpunkte verarbeiten
      var l = (mi.Child as Label)?.Text;
      if (l == Menu_table_addrow)
      {
        if (s.Length > 0 && store.GetIter(out var i, s[0]))
        {
          var v = new GLib.Value();
          store.GetValue(i, 0, ref v);
          var value = Math.Max(1, Functions.ToInt32(v.Val as string));
          store.InsertWithValues(value - 1, "0", "000");
          znummern = true;
        }
      }
      else if (l == Menu_table_addrow2)
      {
        foreach (var sel in s)
        {
          zeilen++;
          store.AppendValues($"{zeilen}", $"{zeilen:000}");
        }
      }
      else if (l == Menu_table_delrow)
      {
        var z = zeilen;
        for (var i = s.Length - 1; i >= 0 && z > 1; i--)
          if (store.GetIter(out var iter, s[i]))
          {
            // var childIter = treeModelSort.ConvertIterToChildIter(iter);
            store.Remove(ref iter);
            z--;
            znummern = true;
          }
      }
      else if (l == Menu_table_addcol || l == Menu_table_addcol2)
      {
        var pos = spalten;
        var anz = 1;
        if (l == Menu_table_addcol)
        {
          tv.GetCursor(out var p, out var c);
          if (c != null)
            pos = (int)c.Data["cnr"];
        }
        pos = Math.Max(2, pos);
        if (spalten + anz <= 26 + 2)
        {
          var list = new List<string[]>();
          if (store.GetIterFirst(out var i))
          {
            do
            {
              var arr = new string[spalten + anz];
              for (var j = 0; j < spalten; j++)
              {
                var v = new GLib.Value();
                store.GetValue(i, j, ref v);
                var val = v.Val as string;
                if (j < pos)
                  arr[j] = val;
                else
                  arr[j + anz] = val;
              }
              list.Add(arr);
              // Debug.Print(string.Join(" | ", arr));
            } while (store.IterNext(ref i));
          }
          AddStringColumns(tv, spalten + anz - 2, true, list);
        }
      }
      else if (l == Menu_table_delcol)
      {
        var pos = spalten;
        var anz = 1;
        tv.GetCursor(out var p, out var c);
        if (c != null)
          pos = (int)c.Data["cnr"];
        pos = Math.Max(2, pos);
        if (pos < spalten && spalten - anz > 2)
        {
          var list = new List<string[]>();
          // Debug.Print($"{pos} {anz}");
          if (store.GetIterFirst(out var i))
          {
            do
            {
              var arr = new string[spalten - anz];
              for (var j = 0; j < spalten; j++)
              {
                var v = new GLib.Value();
                store.GetValue(i, j, ref v);
                var val = v.Val as string;
                if (j <= pos)
                  arr[j] = val;
                else
                  arr[j - anz] = val;
              }
              list.Add(arr);
              // Debug.Print(string.Join(" | ", arr));
            } while (store.IterNext(ref i));
          }
          AddStringColumns(tv, spalten - anz - 2, true, list);
        }
      }
      if (znummern)
      {
        // Debug.Print("znummern");
        if (store.GetIterFirst(out var i))
        {
          var z = 0;
          do
          {
            z++;
            var v = new GLib.Value();
            store.GetValue(i, 0, ref v);
            var value = Functions.ToInt32(v.Val as string);
            if (z != value)
            {
              store.SetValue(i, 0, $"{z}");
              store.SetValue(i, 1, $"{z:000}");
            }
          } while (store.IterNext(ref i));
        }
      }
    }

    private void OnTableButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
      var cr = o as Gtk.Widget;
      if (cr == null)
        return;
      var tv = cr.Data["tv"] as TreeView;
      var store = cr.Data["store"] as TreeStore;
      if (args.Event.Button == 3)
      {
        var m = new Menu();
        var mi = new MenuItem(Menu_table_addrow);
        mi.Data["tv"] = tv;
        mi.Data["store"] = store;
        mi.ButtonPressEvent += OnTableMenuItemClick;
        m.Add(mi);
        mi = new MenuItem(Menu_table_addrow2);
        mi.Data["tv"] = tv;
        mi.Data["store"] = store;
        mi.ButtonPressEvent += OnTableMenuItemClick;
        m.Add(mi);
        mi = new MenuItem(Menu_table_delrow);
        mi.Data["tv"] = tv;
        mi.Data["store"] = store;
        mi.ButtonPressEvent += OnTableMenuItemClick;
        m.Add(mi);
        mi = new MenuItem(Menu_table_addcol);
        mi.Data["tv"] = tv;
        mi.Data["store"] = store;
        mi.ButtonPressEvent += OnTableMenuItemClick;
        m.Add(mi);
        mi = new MenuItem(Menu_table_addcol2);
        mi.Data["tv"] = tv;
        mi.Data["store"] = store;
        mi.ButtonPressEvent += OnTableMenuItemClick;
        m.Add(mi);
        mi = new MenuItem(Menu_table_delcol);
        mi.Data["tv"] = tv;
        mi.Data["store"] = store;
        mi.ButtonPressEvent += OnTableMenuItemClick;
        m.Add(mi);
        m.ShowAll();
        m.Popup();
      }
    }

    protected TreeStore AddColumns(ComboBox cb, List<MaParameter> list = null, bool emptyentry = false)
    {
      var types = new[] { typeof(string), typeof(string) };
      for (var i = 0; i < types.Length; i++)
      {
        var cell = new CellRendererText();
        cb.PackStart(cell, true);
      }
      var store = new TreeStore(types);
      var sortable = new TreeModelSort(store);
      cb.Model = sortable;
      if (list != null)
      {
        foreach (var p in list)
          store.AppendValues(p.Wert, p.Schluessel);
      }
      if (emptyentry)
        store.AppendValues("", "");
      if (!cb.Data.ContainsKey("KeyReleaseEvent"))
      {
        var sb = new StringBuilder();
        cb.Data.Add("KeyReleaseEvent", "");
        cb.KeyReleaseEvent += (object o, KeyReleaseEventArgs e) =>
        {
          var k = e.Event.Key;
          try
          {
            if (k == Gdk.Key.Tab || k == Gdk.Key.Escape)
            {
              sb.Length = 0;
              // Debug.WriteLine("Tab");
              return;
            }
            if (k == Gdk.Key.Down || k == Gdk.Key.Up
              || ((int)e.Event.State & ((int)Gdk.ModifierType.ShiftMask | (int)Gdk.ModifierType.ControlMask | (int)Gdk.ModifierType.Mod1Mask)) != 0)
              return;
            if (k == Gdk.Key.BackSpace || k == Gdk.Key.Delete || k == Gdk.Key.KP_Delete)
            {
              if (sb.Length > 0)
                sb.Length--;
              return;
            }
            if (e.Event.KeyValue <= 255)
            {
              sb.Append((char)e.Event.KeyValue);
            }
            if (sb.Length <= 0)
              return;
            var found = false;
            var s = sb.ToString().ToLower();
            if (cb.Model.GetIterFirst(out TreeIter iter))
            {
              var valid = true;
              while (valid)
              {
                if (matches(s, ((cb.Model.GetValue(iter, 0) as string) ?? "").ToLower()))
                {
                  cb.SetActiveIter(iter);
                  found = true;
                  break;
                }
                valid = cb.Model.IterNext(ref iter);
              }
            }
            if (!found && sb.Length > 0)
              sb.Length--;
          }
          finally
          {
            Debug.WriteLine(e.Event.Key + " " + e.Event.State + " " + e.Event.KeyValue + ": " + sb);
          }
        };
      }
      return store;
    }

    /// <summary>Aktualisieren des Status.</summary>
    protected void ShowStatus(StringBuilder Status, StringBuilder Cancel)
    {
      ShowError(null);
      Cancel.Clear();
      Status.Clear();
      Task.Run(() =>
      {
        try
        {
          while (true)
          {
            Application.Invoke(delegate
            {
              MainClass.MainWindow.SetError(Status.ToString());
            });
            if (Cancel.Length > 0)
              break;
            Thread.Sleep(200);
          }
        }
        catch (Exception ex)
        {
          Functions.MachNichts(ex);
        }
        return 0;
      });
    }

    // Erster Buchstabe muss übereinstimmen, die nächsten müssen nur in der Reihenfolge vorkommen.
    private bool matches(string s, string o)
    {
      // return o.startsWith(s);
      if (string.IsNullOrEmpty(o))
        return string.IsNullOrEmpty(s);
      int i = 0;
      foreach (char c in s.ToCharArray())
      {
        if (i == 0 && c != o[0])
          return false;
        i = o.IndexOf(c, i) + 1;
        if (i <= 0)
          return false;
      }
      return true;
    }


    protected bool SetText(TreeView tv, string v)
    {
      tv.Selection.UnselectAll();
      var store = tv.Model;
      if (!string.IsNullOrEmpty(v) && store.GetIterFirst(out var i))
      {
        do
        {
          var val = store.GetValue(i, 0) as string;
          if (val != null && v == val)
          {
            tv.Selection.SelectIter(i);
            var path = store.GetPath(i);
            if (path != null)
              tv.ScrollToCell(path, null, false, 0, 0);
            return true;
          }
        } while (store.IterNext(ref i));
      }
      return false;
    }

    protected bool SetText(ComboBox cb, string v)
    {
      cb.Active = -1;
      if (cb.Model.GetIterFirst(out TreeIter iter))
      {
        var valid = true;
        while (valid)
        {
          if (Functions.CompString(cb.Model.GetValue(iter, 1) as string, v) == 0)
          {
            cb.SetActiveIter(iter);
            return true;
          }
          valid = cb.Model.IterNext(ref iter);
        }
      }
      return false;
    }

    protected string GetText(ComboBox cb, bool id = true)
    {
      if (cb.GetActiveIter(out var iter))
      {
        return cb.Model.GetValue(iter, id ? 1 : 0) as string;
      }
      return null;
    }

    protected void SetUserData(RadioButton[] rb, string[] v)
    {
      if (rb == null || v == null || rb.Length != v.Length)
        throw new ArgumentException("SetUserData");
      for (var i = 0; i < rb.Length; i++)
      {
        if (!rb[i].Data.ContainsKey("v"))
          rb[i].Data.Add("v", v[i]);
      }
    }

    protected void SetText(RadioButton rb, string v)
    {
      if (rb == null)
        throw new ArgumentException("SetText");
      foreach (RadioButton r in rb.Group)
      {
        var val = r.Data["v"]?.ToString();
        if (v == val)
        {
          r.Active = true;
          return;
        }
      }
      rb.Active = true;
    }

    protected string GetText(RadioButton rb)
    {
      if (rb == null)
        return null;
      foreach (RadioButton r in rb.Group)
      {
        if (r.Active)
        {
          var val = r.Data["v"]?.ToString();
          return val;
        }
      }
      return null;
    }

    /// <summary>Select a file name.</summary>
    /// <returns>Selected file name or default file name.</returns>
    /// <param name="filename">Affected default file name.</param>
    /// <param name="ext">Affected file extension.</param>
    /// <param name="extension">Affected description of file extension.</param>
    protected string SelectFile(string filename, string ext = null, string extension = null)
    {
      string file = filename;
      string path = null;
      // Pfad bestimmen
      if (!string.IsNullOrEmpty(file))
      {
        path = System.IO.Path.GetDirectoryName(file);
      }
      if (string.IsNullOrEmpty(path))
      {
        path = Parameter.TempPath;
      }
      // Datei mit Pfad
      if (!string.IsNullOrEmpty(file))
      {
        if (string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(file)))
        {
          file = System.IO.Path.Combine(path, file);
        }
      }
      if (string.IsNullOrEmpty(ext) || string.IsNullOrEmpty(extension))
        return file;
      using (var dlg = new FileChooserDialog(Forms_selectfile, dialog,
           FileChooserAction.Open, Forms_select, ResponseType.Accept, Forms_cancel, ResponseType.Cancel))
      {
        if (!string.IsNullOrEmpty(ext) || !string.IsNullOrEmpty(extension))
        {
          var ff = new FileFilter { Name = extension };
          ff.AddPattern(ext);
          dlg.AddFilter(ff);
        }
        if (!string.IsNullOrEmpty(file))
          dlg.SetFilename(file);
        else if (!string.IsNullOrEmpty(path))
          dlg.SetCurrentFolder(path);
        if (dlg.Run() == (int)ResponseType.Accept)
        {
          file = dlg.Filename;
        }
        dlg.Dispose();
      }
      return file;
    }
  }
}
