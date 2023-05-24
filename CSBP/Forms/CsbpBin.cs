// <copyright file="CsbpBin.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CSBP.Apis.Enums;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Forms.Controls;
using CSBP.Resources;
using CSBP.Services.Factory;
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>
/// Base class for dialogs.
/// </summary>
[System.ComponentModel.ToolboxItem(false)]
public partial class CsbpBin : Bin
{
#pragma warning disable SA1401 // Field should be private
  /// <summary>Internal GTK dialog.</summary>
  protected Dialog dialog;
#pragma warning restore SA1401

  /// <summary>Value indicating whether events are active or not.</summary>
  private bool eventsActive;

  /// <summary>Initializes a new instance of the <see cref="CsbpBin"/> class.</summary>
  /// <param name="builder">Affected Builder.</param>
  /// <param name="handle">Affected handle from Builder.</param>
  /// <param name="d">Affected embedded dialog.</param>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="dt">Affected dialog type.</param>
  /// <param name="parameter1">1. parameter for dialog.</param>
  /// <param name="csbpparent">Affected parent dialog.</param>
  public CsbpBin(Builder builder, IntPtr handle, Dialog d, Type type = null, DialogTypeEnum dt = DialogTypeEnum.Without, object parameter1 = null, CsbpBin csbpparent = null)
    : base(handle)
  {
    Builder = builder;
    dialog = d;
    ClassType = type;
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
      if (c is Entry e && !string.IsNullOrEmpty(e.PlaceholderText) && e.PlaceholderText.EndsWith(".tt", StringComparison.CurrentCulture))
      {
        e.PlaceholderText = Messages.Get(e.PlaceholderText);
      }
      if (c is Label lbl && !string.IsNullOrEmpty(lbl.LabelProp) && lbl.LabelProp.Contains('.', StringComparison.CurrentCulture))
      {
        lbl.LabelProp = Messages.Get(lbl.LabelProp);
      }
      if (c is Button btn)
      {
        if (!string.IsNullOrEmpty(btn.TooltipText) && btn.TooltipText.StartsWith("Action.", StringComparison.CurrentCulture))
          btn.TooltipText = Messages.Get(btn.TooltipText);
        if (!string.IsNullOrEmpty(btn.Label) && btn.Label.Contains('.', StringComparison.CurrentCulture))
          btn.Label = Messages.Get(btn.Label);
      }
    });
    SetupHandlers();
  }

  /// <summary>Gets height of the title bar.</summary>
  public static int TitleHeight { get; private set; } = Functions.IsLinux() ? 37 : 10;

  /// <summary>Gets the current service data.</summary>
  protected static ServiceDaten ServiceDaten => MainClass.ServiceDaten;

  /// <summary>Gets the GTK build.</summary>
  protected Builder Builder { get; private set; }

  /// <summary>Gets the dialog type.</summary>
  protected DialogTypeEnum DialogType { get; private set; }

  /// <summary>Gets the parent dialog.</summary>
  protected CsbpBin CsbpParent { get; private set; }

  /// <summary>Gets the first parameter.</summary>
  protected object Parameter1 { get; private set; }

  /// <summary>Gets or sets the dialog response for modal dialogs.</summary>
  protected object Response { get; set; }

  /// <summary>Gets or sets a value indicating whether events are active or not.</summary>
  protected bool EventsActive { get => eventsActive; set => eventsActive = value; }

  /// <summary>Gets the dialog class type.</summary>
  protected Type ClassType { get; private set; }

  /// <summary>Starts a modal or non modal dialog.</summary>
  /// <param name="type">Affected dialog class type.</param>
  /// <param name="title">Affected title.</param>
  /// <param name="dialogType">Affected dialog type.</param>
  /// <param name="parameter1">Affected 1. parameter for dialog.</param>
  /// <param name="modal">Modal dialog or not.</param>
  /// <param name="p">Affected mail dialog.</param>
  /// <param name="csbpparent">Affected direct parent dialog.</param>
  /// <returns>Modal dialog result.</returns>
  public static object Start(Type type, string title = null,
    DialogTypeEnum dialogType = DialogTypeEnum.Without, object parameter1 = null,
    bool modal = false, Gtk.Window p = null, CsbpBin csbpparent = null)
  {
    p ??= MainClass.MainWindow;
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
      title = $"{title ?? ""} - {x}";
    }

    var dialog = new Dialog(title, p, f);
    var builder = GetBuilder(type.Name, out var handle);
    var form = Activator.CreateInstance(type, builder, handle, dialog, type, dialogType, parameter1, csbpparent) as CsbpBin;
    ////form.ShowAll();
    dialog.ContentArea.PackStart(form, true, true, 0);
    var size = Parameter.GetDialogSize(type);
    //// dialog.SetSizeRequest(size.Width, size.Height); // Window doesn't get smaller than that.
    dialog.SetDefaultSize(size.Width, size.Height);
    if (size.X == -1 && size.Y == -1)
      dialog.SetPosition(WindowPosition.Center);
    else
      dialog.Move(size.X, size.Y);

    // var ob = Observable.FromEvent<SizeAllocatedHandler, SizeAllocatedArgs>(
    //   h0 =>
    //   {
    //     void H(object sender, SizeAllocatedArgs e)
    //     {
    //       h0(e);
    //     }
    //     return H;
    //   },
    //   h => dialog.SizeAllocated += h, h => dialog.SizeAllocated -= h
    // ).Throttle(TimeSpan.FromMilliseconds(1000));
    // ob.Subscribe(e =>
    // {
    //   // Possibly for closing by x.
    //   Application.Invoke((sender, e1) =>
    //   {
    //     if (dialog.Window != null)
    //     {
    //       dialog.Window.GetGeometry(out int x0, out int y0, out int w, out int h);
    //       dialog.Window.GetOrigin(out int x, out int y);
    //       Parameter.SetDialogSize(type, new Rectangle(x, y, w, h));
    //     }
    //   });
    // });
    dialog.DeleteEvent += (sender, e) =>
    {
      if (dialog?.Window != null)
      {
        // dialog.SetSizeRequest(0, 0);
        // dialog.Window.GetPosition(out x, out y);
        // Services.Base.ServiceBase.Log.Warn($"{type.Name} Window.GetPosition x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
        // dialog.Window.GetGeometry(out int _, out int _, out int w, out int h);
        // Services.Base.ServiceBase.Log.Warn($"{type.Name} Window.GetGeometry x {x} y {y} w {w} h {h} {DateTime.Now:HH:mm:ss.fff}");
        dialog.Window.GetOrigin(out int x, out int y);
        //// Services.Base.ServiceBase.Log.Warn($"{type.Name} Window.GetOrigin x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
        dialog.GetSize(out int w, out int h); // better than dialog.Window.GetGeometry(out x, out y, out w, out h);
        //// Services.Base.ServiceBase.Log.Warn($"{type.Name} Gtk.GetSize w {w} h {h} {DateTime.Now:HH:mm:ss.fff}");
        //// dialog.GetPosition(out x, out y);
        //// Services.Base.ServiceBase.Log.Warn($"{type.Name} Gtk.GetPosition x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
        //// dialog.GetOrigin(out x, out y);
        //// Services.Base.ServiceBase.Log.Warn($"{type.Name} GetOrigin x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
        Parameter.SetDialogSize(type, new Rectangle(x, y, w, h));
      }
    };
    dialog.DefaultResponse = ResponseType.Cancel;
    var def = form.GetDefaultButton();
    if (def != null)
      dialog.Default = def;
    dialog.ShowAll();
    if (modal)
    {
      dialog.Run();
      //// dialog.Hide();
      dialog.Dispose();
    }
    return form.Response;
  }

  /// <summary>Focus or starts a non modal dialog.</summary>
  /// <param name="title">Affected title.</param>
  /// <param name="dialogType">Affected dialog type.</param>
  /// <param name="parameter1">Affected 1. parameter for dialog.</param>
  /// <param name="modal">Modal dialog or not.</param>
  /// <param name="p">Affected mail dialog.</param>
  /// <param name="csbpparent">Affected direct parent dialog.</param>
  /// <returns>Modal dialog result.</returns>
  /// <typeparam name="T">Affected dialog class type.</typeparam>
  public static T Focus<T>(string title = null, DialogTypeEnum dialogType = DialogTypeEnum.Without, object parameter1 = null,
    bool modal = false, Gtk.Window p = null, CsbpBin csbpparent = null)
    where T : CsbpBin
  {
    var dlg = MainClass.MainWindow.FocusPage<T>();
    if (dlg == null)
    {
      Functions.MachNichts(dialogType);
      Functions.MachNichts(modal);
      Functions.MachNichts(p);
      var create = typeof(T).GetMethod("Create");
      dlg = create.Invoke(null, new[] { parameter1, csbpparent }) as T;
      MainClass.MainWindow.AppendPage(dlg, title);
    }
    return dlg;
  }

  /// <summary>
  /// Shows a yes no question.
  /// </summary>
  /// <param name="msg">Affected message.</param>
  /// <returns>True if user chose yes.</returns>
  public static bool ShowYesNoQuestion(string msg)
  {
    int r;
    using (var md = new MessageDialog(MainClass.MainWindow,
        DialogFlags.DestroyWithParent, MessageType.Question,
        ButtonsType.YesNo, msg))
    {
      r = md.Run();
      //// md.Hide();
      md.Dispose();
    }
    return r == (int)ResponseType.Yes;
  }

  /// <summary>
  /// Extracts possible errors from service result.
  /// </summary>
  /// <param name="r">Affected service result.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
  /// <typeparam name="T">Affected result type.</typeparam>
  /// <returns>Result of service result.</returns>
  public static T Get<T>(ServiceErgebnis<T> r, bool dialog = true)
  {
    return MainClass.Get(r, dialog);
  }

  /// <summary>
  /// Extracts possible errors from service result.
  /// </summary>
  /// <param name="r">Affected service result.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
  /// <returns>Are there errors or not.</returns>
  public static bool Get(ServiceErgebnis r, bool dialog = true)
  {
    return MainClass.Get(r, dialog);
  }

  /// <summary>
  /// Shows info message.
  /// </summary>
  /// <param name="s">Affected info message.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
  public static void ShowInfo(string s, bool dialog = true)
  {
    MainClass.ShowInfo(s, dialog);
  }

  /// <summary>
  /// Shows error message.
  /// </summary>
  /// <param name="s">Affected error message.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
  public static void ShowError(string s, bool dialog = true)
  {
    MainClass.ShowError(s, dialog);
  }

  /// <summary>
  /// Close event.
  /// </summary>
  /// <returns>Result of the dialog.</returns>
  public virtual object Close()
  {
    return null;
  }

  /// <summary>
  /// Create a build for as dialog name.
  /// </summary>
  /// <param name="name">Affected dialog name.</param>
  /// <param name="handle">Returns affencted handle.</param>
  /// <returns>Created builder.</returns>
  protected static Builder GetBuilder(string name, out IntPtr handle)
  {
    var builder = new Builder($"CSBP.GtkGui.{name[..2]}.{name}.glade");
    handle = builder.GetObject(name).Handle;
    return builder;
  }

  /// <summary>
  /// Throttles events.
  /// </summary>
  /// <param name="w">Affected Widget.</param>
  /// <param name="h">New Eventhandler.</param>
  /// <param name="eventname">Optional event name.</param>
  /// <param name="millis">Throttle time span.</param>
  protected static void ObservableEventThrottle(Widget w, EventHandler h, string eventname = "Clicked", int millis = 500)
  {
    var ob = Observable.FromEventPattern<EventArgs>(w, eventname)
      .Throttle(TimeSpan.FromMilliseconds(millis));
    ob.Subscribe(e => Application.Invoke(h));
  }

  /// <summary>
  /// Gets the selected value of a TreeView.
  /// </summary>
  /// <param name="tv">Specific TreeView.</param>
  /// <param name="mandatory">Is the value mandatory or not.</param>
  /// <param name="column">Affected column number for value.</param>
  /// <param name="first">Take first entry, if nothing is selected.</param>
  /// <typeparam name="T">Affected return value type.</typeparam>
  /// <returns>Selected value.</returns>
  protected static T GetValue<T>(TreeView tv, bool mandatory = true, int column = 0, bool first = false)
    where T : class
  {
    T value = default;
    var s = tv.Selection.GetSelectedRows();
    if (s.Length > 0 && tv.Model.GetIter(out var iter, s[0]))
    {
      var v = default(GLib.Value);
      tv.Model.GetValue(iter, column, ref v);
      value = v.Val as T;
    }
    else if (s.Length <= 0 && tv.Model != null && (tv.Model.IterNChildren() == 1 || (tv.Model.IterNChildren() > 1 && first)))
    {
      if (tv.Model.GetIterFirst(out var iter1))
      {
        var v = default(GLib.Value);
        tv.Model.GetValue(iter1, column, ref v);
        value = v.Val as T;
      }
    }
    if (mandatory && value == null)
      throw new MessageException(M1013);
    return value;
  }

  /// <summary>
  /// Gets the selected value of a TreeView.
  /// </summary>
  /// <param name="tv">Specific TreeView.</param>
  /// <param name="mandatory">Is the value mandatory or not.</param>
  /// <param name="column">The column number.</param>
  /// <param name="first">Take first entry, if nothing is selected.</param>
  /// <returns>Selected value.</returns>
  protected static string GetText(TreeView tv, bool mandatory = false, int column = 0, bool first = false)
  {
    return GetValue<string>(tv, mandatory, column, first);
  }

  /// <summary>Sets bold fond in label.</summary>
  /// <param name="w">Affected Label.</param>
  protected static void SetBold(GLib.Object w)
  {
    if (w is Label lbl && !lbl.UseMarkup)
    {
      lbl.UseMarkup = true;
      lbl.LabelProp = $"<b>{lbl.LabelProp}</b>";
    }
  }

  /// <summary>
  /// Adds two columns (descripton and id) to a ComboBox.
  /// </summary>
  /// <param name="cb">Affected ComboBox.</param>
  /// <param name="list">List of values.</param>
  /// <param name="emptyentry">Adds an empty entry or not.</param>
  /// <returns>Tree store.</returns>
  protected static TreeStore AddColumns(ComboBox cb, List<MaParameter> list = null, bool emptyentry = false)
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
      cb.Data.Add("KeyReleaseEvent", "");
      var sb = new StringBuilder();
      var completion = new Gtk.EntryCompletion
      {
        Model = cb.Model,
        TextColumn = cb.EntryTextColumn,
      };
      completion.MatchFunc = (EntryCompletion completion, string key, TreeIter iter) =>
      {
        if (!string.IsNullOrEmpty(key) && Matches(key.ToLower(), ((completion.Model.GetValue(iter, 0) as string) ?? "").ToLower()))
          return true;
        return false;
      };
      if (cb.Child is Entry entry)
        entry.Completion = completion;
    }
    return store;
  }

  /// <summary>Updates the state.</summary>
  /// <param name="state">State of calculation is always updated.</param>
  /// <param name="cancel">Cancel calculation if not empty.</param>
  protected static void ShowStatus(StringBuilder state, StringBuilder cancel)
  {
    ShowError(null);
    cancel.Clear();
    state.Clear();
    Task.Run(() =>
    {
      try
      {
        while (true)
        {
          Application.Invoke((sender, e) =>
          {
            MainClass.MainWindow.SetError(state.ToString());
          });
          if (cancel.Length > 0)
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

  /// <summary>
  /// Sets the value of a Label.
  /// </summary>
  /// <param name="e">Affected Label.</param>
  /// <param name="v">Affected value.</param>
  protected static void SetText(Label e, string v)
  {
    e.Text = v ?? "";
  }

  /// <summary>
  /// Sets the value of a Entry.
  /// </summary>
  /// <param name="e">Affected Entry.</param>
  /// <param name="v">Affected value.</param>
  protected static void SetText(Entry e, string v)
  {
    e.Text = v ?? "";
  }

  /// <summary>
  /// Sets the value of a TextView.
  /// </summary>
  /// <param name="e">Affected TextView.</param>
  /// <param name="v">Affected value.</param>
  protected static void SetText(TextView e, string v)
  {
    e.Buffer.Text = v ?? "";
  }

  /// <summary>
  /// Sets the value of a TreeView.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="v">Affected value.</param>
  /// <returns>Is the value set or not.</returns>
  protected static bool SetText(TreeView tv, string v)
  {
    tv.Selection.UnselectAll();
    var store = tv.Model;
    if (!string.IsNullOrEmpty(v) && store.GetIterFirst(out var i))
    {
      do
      {
        if (store.GetValue(i, 0) is string val && v == val)
        {
          tv.Selection.SelectIter(i);
          var path = store.GetPath(i);
          if (path != null)
            tv.ScrollToCell(path, null, false, 0, 0);
          return true;
        }
      }
      while (store.IterNext(ref i));
    }
    return false;
  }

  /// <summary>
  /// Sets the value of a ComboBox.
  /// </summary>
  /// <param name="cb">Affected ComboBox.</param>
  /// <param name="v">Affected value.</param>
  /// <returns>Is the value set or not.</returns>
  protected static bool SetText(ComboBox cb, string v)
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

  /// <summary>
  /// Gets the selected value of a ComboBox.
  /// </summary>
  /// <param name="cb">Affected ComboBox.</param>
  /// <param name="id">Gets the id or description.</param>
  /// <returns>Value or null.</returns>
  protected static string GetText(ComboBox cb, bool id = true)
  {
    if (cb.GetActiveIter(out var iter))
    {
      return cb.Model.GetValue(iter, id ? 1 : 0) as string;
    }
    else
    {
      // EntryCompletion does not select active iter.
      var s = (cb as ComboBoxText)?.ActiveText;
      if (!string.IsNullOrEmpty(s))
      {
        if (cb.Model.GetIterFirst(out var it))
        {
          do
          {
            if (cb.Model.GetValue(it, 0) as string == s)
              return cb.Model.GetValue(it, id ? 1 : 0) as string;
          }
          while (cb.Model.IterNext(ref it));
        }
      }
    }
    return null;
  }

  /// <summary>
  /// Sets the values of a RadioButton list.
  /// </summary>
  /// <param name="rb">Affected list of RadioButtons.</param>
  /// <param name="v">Affected values.</param>
  protected static void SetUserData(RadioButton[] rb, string[] v)
  {
    if (rb == null || v == null || rb.Length != v.Length)
      throw new ArgumentException("SetUserData");
    for (var i = 0; i < rb.Length; i++)
    {
      if (!rb[i].Data.ContainsKey("v"))
        rb[i].Data.Add("v", v[i]);
    }
  }

  /// <summary>
  /// Sets the selected value of a RadioButton list.
  /// </summary>
  /// <param name="rb">One of the RadioButton list.</param>
  /// <param name="v">Affected value.</param>
  protected static void SetText(RadioButton rb, string v)
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

  /// <summary>
  /// Gets the selected value of a RadioButton list.
  /// </summary>
  /// <param name="rb">One of the RadioButton list.</param>
  /// <returns>Selected value.</returns>
  protected static string GetText(RadioButton rb)
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

  /// <summary>Initialisierung der Events.</summary>
  protected virtual void SetupHandlers()
  {
  }

  /// <summary>Initialises model data.</summary>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  protected virtual void InitData(int step)
  {
  }

  /// <summary>Updates parent dialog.</summary>
  protected virtual void UpdateParent()
  {
    var p = CsbpParent;
    p?.UpdateParent();
  }

  /// <summary>
  /// Gets the default button in the container.
  /// </summary>
  /// <param name="con">Affected container.</param>
  /// <returns>Default button as Widget or null.</returns>
  protected Widget GetDefaultButton(Container con = null)
  {
    con ??= this;
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
  /// Refreshes a TreeView and sets or maintaines the selection.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="step">Affected step: 0 initially, 1 update.</param>
  /// <param name="value">New value for selection.</param>
  protected void RefreshTreeView(TreeView tv, int step, string value = null)
  {
    var v = value ?? GetValue<string>(tv, false);
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
      if (Functions.MachNichts() != 0)
      {
        // Sortierung wiederherstellen
        tv.GetColumn(si).Button.Activate();
        if (so == SortType.Descending)
          tv.GetColumn(si).Button.Activate();
        if (string.IsNullOrEmpty(v))
        {
          foreach (var p in s)
            tv.Selection.SelectPath(p);
        }
        else
          SetText(tv, v);
      }
      else
      {
        Task.Run(() =>
        {
          // Sortierung asynchron wiederherstellen
          var x = 0;
          try
          {
            // tv.GetColumn(si).SortOrder = SortType.Ascending;
            Debug.WriteLine($"1 Soll {so} Ist {tv.GetColumn(si).SortOrder}");
            Application.Invoke((sender, e) =>
            {
              tv.GetColumn(si).Button.Activate();
              x = 1;
            });
            while (x < 1)
              Thread.Sleep(100);
            //// Debug.WriteLine($"2 Soll {so} Ist {tv.GetColumn(si).SortOrder}");
            if (so == SortType.Descending)
            {
              Thread.Sleep(300);
              Application.Invoke((sender, e) =>
              {
                tv.GetColumn(si).Button.Activate();
                x = 2;
              });
              while (x < 2)
                Thread.Sleep(100);
              //// Debug.WriteLine($"3 Soll {so} Ist {tv.GetColumn(si).SortOrder}");
            }
            Thread.Sleep(200);
            Application.Invoke((sender, e) =>
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
  /// Adds string columns to a TreeView.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="headers">Column headers separated by semicolon.</param>
  /// <returns>TreeView store.</returns>
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

  /// <summary>
  /// Adds editable string columns to a TreeView. The column headers are built like Excel: A, B, C, ... Z, AA, ....
  /// Possibly fills up with values.</summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="columns">Number of columns.</param>
  /// <param name="editable">Are the columns editable or not.</param>
  /// <param name="values">List of all row values.</param>
  /// <param name="flist">Affected formulas.</param>
  protected void AddStringColumns(TreeView tv, int columns, bool editable = false, List<string[]> values = null,
    Formulas flist = null)
  {
    var titles = new string[Math.Min(Math.Max(columns + 2, 1), 28)];
    var types = new Type[titles.Length];
    for (var i = 0; i < titles.Length; i++)
    {
      if (i < 2)
        titles[i] = "";
      else
        titles[i] = Formula.GetColumnName(i - 2); // ((char)((i - 2) % 26 + 'A')).ToString();
      types[i] = typeof(string);
    }
    if (editable)
    {
      tv.KeyReleaseEvent += (o, e) =>
      {
        if (e.Event.Key == Gdk.Key.Return)
        {
          var store = tv.Data["store"] as TreeStore;
          var flist = tv.Data["flist"] as Formulas;
          flist?.BeginEdit(tv, store);
        }
      };
    }
    AddColumns(tv, titles, types, editable, values, flist);
  }

  /// <summary>
  /// Adds columns and possibly values to a TreeView.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="titles">Affected column header titles.</param>
  /// <param name="types">Affected column value types.</param>
  /// <param name="editable">Are the columns editable or not.</param>
  /// <param name="values">List of all row values.</param>
  /// <param name="flist">Affected formulas.</param>
  /// <returns>TreeView store.</returns>
  protected TreeStore AddColumns(TreeView tv, string[] titles, Type[] types, bool editable = false,
      List<string[]> values = null, Formulas flist = null)
  {
    foreach (var c in tv.Columns)
      tv.RemoveColumn(c);
    var store = new TreeStore(types);
    for (var i = 0; i < titles.Length; i++)
    {
      var col = GetColumn(titles[i], i, editable && i >= 2, !editable, store, flist);
      tv.AppendColumn(col);
    }
    tv.Model = store;
    if (editable)
    {
      tv.Selection.Mode = SelectionMode.Multiple;
      tv.Data["tv"] = tv;
      tv.Data["store"] = store;
      tv.Data["flist"] = flist;
      tv.ButtonReleaseEvent += OnTableButtonReleaseEvent;
      if (values != null)
        foreach (var arr in values)
          store.AppendValues(arr);
      flist?.CalculateFormulas(store);
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

  /// <summary>
  /// Adds sortable string columns to a TreeView.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="headers">Affected header titles.</param>
  /// <param name="values">List of row values.</param>
  /// <returns>TreeStore with values.<returns>
  protected TreeStore AddStringColumnsSort(TreeView tv, string headers, List<string[]> values = null)
  {
    var titles = headers.Split(';');
    if (titles.Length <= 1)
      throw new ArgumentException("Too few column headers", nameof(headers));
    var types = new Type[titles.Length];
    for (var i = 0; i < titles.Length; i++)
      types[i] = typeof(string);
    //// types[i] = titles[i].EndsWith("_r", StringComparison.InvariantCulture) ? typeof(decimal) : typeof(string);
    return AddColumnsSort(tv, titles, types, values: values);
  }

  /// <summary>
  /// Adds sortable columns to a TreeView.
  /// </summary>
  /// <param name="tv">Affected TreeView.</param>
  /// <param name="titles">Affected column header titles.</param>
  /// <param name="types">Affected column value types.</param>
  /// <param name="editable">Are the columns editable or not.</param>
  /// <param name="values">List of row values.</param>
  /// <returns>TreeStore with values.<returns>
  protected TreeStore AddColumnsSort(TreeView tv, string[] titles, Type[] types, bool editable = false,
      List<string[]> values = null)
  {
    foreach (var c in tv.Columns)
      tv.RemoveColumn(c);
    var store = new TreeStore(types);
    for (var i = 0; i < titles.Length; i++)
    {
      var col = GetColumn(titles[i], i, editable && i >= 2, true, store, null);
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
          return decimal.Compare(s1, s2);
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
    return store;
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
    //// Pfad bestimmen
    if (!string.IsNullOrEmpty(file))
    {
      path = System.IO.Path.GetDirectoryName(file);
    }
    if (string.IsNullOrEmpty(path))
    {
      path = Parameter.TempPath;
    }
    //// file name incl. path
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

  /// <summary>
  /// Close non modal dialog.
  /// </summary>
  protected void CloseDialog()
  {
    if (dialog.Window != null)
    {
      // dialog.SetSizeRequest(0, 0);
      dialog.Window.GetGeometry(out _, out _, out int w, out int h);
      dialog.Window.GetOrigin(out int x, out int y);
      //// dialog.Window.GetPosition(out int x, out int y);
      Parameter.SetDialogSize(ClassType, new Rectangle(x, y, w, h));
    }
    dialog.Dispose();
  }

  /// <summary>
  /// Matches two strings: first letter must match, the following must appear in sequence.
  /// </summary>
  /// <param name="s">First string.</param>
  /// <param name="o">Second string.</param>
  /// <returns>Match or not.</returns>
  private static bool Matches(string s, string o)
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

  /// <summary>
  /// Gets children of a container.
  /// </summary>
  /// <param name="con">Affected container.</param>
  /// <param name="l">Affected list to fill up.</param>
  /// <returns>List of child controls.</returns>
  private List<Widget> GetChildren(Container con = null, List<Widget> l = null)
  {
    con ??= this;
    l ??= new List<Widget>();
    var array = con.Children;
    foreach (var c in array)
    {
      l.Add(c);
      if (c is Container)
        GetChildren(c as Container, l);
    }
    return l;
  }

  /// <summary>Creates and gets a TreeView column.</summary>
  /// <param name="title">Affected title.</param>
  /// <param name="i">Affected index.</param>
  /// <param name="editable">Is the column editable or not.</param>
  /// <param name="sortable">Is the column sortable or not.</param>
  /// <param name="store">Affected TreeModel store.</param>
  /// <param name="flist">Affected formulas.</param>
  /// <returns>Created column.</returns>
  private TreeViewColumn GetColumn(string title, int i, bool editable, bool sortable, ITreeModel store, Formulas flist)
  {
    var align = 0f; // left
    if (title.EndsWith("_r", StringComparison.InvariantCulture))
    {
      title = title[..^2];
      align = 1f; // right
    }
    else if (title.EndsWith("_e", StringComparison.InvariantCulture))
    {
      title = title[..^2];
      editable = true;
    }
    var cell = new CellRendererText
    //// var cell = new CellRendererSpin
    {
      Xalign = align,
      Editable = editable,
    };
    if (editable)
    {
      cell.Data["store"] = store;
      if (flist != null)
        cell.Data["flist"] = flist;
      cell.Data["cnr"] = i;
      cell.Edited += TableCell_Edited;
    }
    var col = new TreeViewColumn
    {
      SortColumnId = sortable ? i : -1,
      Resizable = true,
      Alignment = align,
    };
    if (editable)
    {
      col.Data["cnr"] = i;
      //// col.Clicked += (o, e) =>
      //// {
      ////   Functions.MachNichts();
      //// };
    }
    var lbl = new Label
    {
      LabelProp = $"<b>{title}</b>",
      UseMarkup = true,
    };
    lbl.Show();
    col.Widget = lbl;
    col.PackStart(cell, true);
    col.AddAttribute(cell, editable ? "markup" : "text", i);
    if (i == 0)
      col.MaxWidth = 13; // Nicht 0 wegen: Negative content width -12 (allocation 1, extents 6x7) while allocating gadget (node button, owner GtkButton)
    return col;
  }

  /// <summary>
  /// Event handler for a editable table cell.
  /// </summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="args">Affected event arguments.</param>
  private void TableCell_Edited(object sender, EditedArgs args)
  {
    if (sender is not Gtk.CellRenderer cr)
      return;
    var cnr = (int)cr.Data["cnr"];
    var store = cr.Data["store"] as TreeStore;
    var flist = cr.Data["flist"] as Formulas;
    //// Debug.Print($"TableCell_Edited cnr {cnr}");
    if (flist == null)
    {
      var path = new TreePath(args.Path);
      store.GetIter(out var it, path);
      var v = default(GLib.Value);
      store.GetValue(it, cnr, ref v);
      var val = v.Val as string;
      var newtext = args.NewText;
      if (val != newtext)
        store.SetValue(it, cnr, newtext);
    }
    flist?.EndEdit(store, cnr, args);
  }

  /// <summary>
  /// Perform context menu commands.
  /// </summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="args">Affected event args.</param>
  private void OnTableMenuItemClick(object sender, ButtonPressEventArgs args)
  {
    if (sender is not MenuItem mi)
      return;
    var tv = mi.Data["tv"] as TreeView;
    var store = mi.Data["store"] as TreeStore;
    var flist = tv.Data["flist"] as Formulas;
    var spalten = store.NColumns;
    var zeilen = 0;
    if (store.GetIterFirst(out var it))
    {
      do
      {
        zeilen++;
      }
      while (store.IterNext(ref it));
    }
    var s = tv.Selection.GetSelectedRows();
    var znummern = false;
    //// Handles menu items
    var l = (mi.Child as Label)?.Text;
    if (l == Menu_table_addrow)
    {
      if (s.Length > 0 && store.GetIter(out var i, s[0]))
      {
        var v = default(GLib.Value);
        store.GetValue(i, 0, ref v);
        var value = Math.Max(1, Functions.ToInt32(v.Val as string));
        flist?.AddRow(value - 1);
        store.InsertWithValues(value - 1, "0", "000");
        znummern = true;
      }
    }
    else if (l == Menu_table_addrow2)
    {
      flist?.AddRow(zeilen);
      foreach (var sel in s)
      {
        zeilen++;
        store.AppendValues($"{zeilen}", $"{zeilen:000}");
      }
    }
    else if (l == Menu_table_delrow)
    {
      if (s.Length > 0 && store.GetIter(out var it0, s[0]))
      {
        var v = default(GLib.Value);
        store.GetValue(it0, 0, ref v);
        var value = Math.Max(1, Functions.ToInt32(v.Val as string));
        flist?.DeleteRow(value - 1);
      }
      var z = zeilen;
      for (var i = s.Length - 1; i >= 0 && z > 1; i--)
        if (store.GetIter(out var iter, s[i]))
        {
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
        tv.GetCursor(out _, out var c);
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
              var v = default(GLib.Value);
              store.GetValue(i, j, ref v);
              var val = v.Val as string;
              if (j < pos)
                arr[j] = val;
              else
                arr[j + anz] = val;
            }
            list.Add(arr);
            //// Debug.Print(string.Join(" | ", arr));
          }
          while (store.IterNext(ref i));
        }
        flist?.AddColumn(pos + anz - 2 - 1);
        AddStringColumns(tv, spalten + anz - 2, true, list, flist);
      }
    }
    else if (l == Menu_table_delcol)
    {
      var pos = spalten;
      var anz = 1;
      tv.GetCursor(out _, out var c);
      if (c != null)
        pos = (int)c.Data["cnr"];
      pos = Math.Max(2, pos);
      if (pos < spalten && spalten - anz > 2)
      {
        var list = new List<string[]>();
        //// Debug.Print($"{pos} {anz}");
        if (store.GetIterFirst(out var i))
        {
          do
          {
            var arr = new string[spalten - anz];
            for (var j = 0; j < spalten; j++)
            {
              var v = default(GLib.Value);
              store.GetValue(i, j, ref v);
              var val = v.Val as string;
              if (j <= pos)
                arr[j] = val;
              else
                arr[j - anz] = val;
            }
            list.Add(arr);
            //// Debug.Print(string.Join(" | ", arr));
          }
          while (store.IterNext(ref i));
        }
        flist?.DeleteColumn(pos + anz - 2 - 1);
        AddStringColumns(tv, spalten - anz - 2, true, list, flist);
      }
    }
    else if (l == Menu_table_bold || l == Menu_table_normal)
    {
      // Make cells bold or normal.
      var unbold = l == Menu_table_normal;
      var cnr = (int)tv.Data["menucnr"]; // Affected column.
      var rnr = (int)tv.Data["menurnr"]; // Affected row.
      Debug.Print($"Bold cnr {cnr} rnry {rnr}");
      var r = 0;
      var cmin = cnr <= 1 ? 0 : cnr;
      var cmax = cnr <= 1 ? (int)tv.NColumns - 1 : cnr;
      if (store.GetIterFirst(out var i))
      {
        do
        {
          if (rnr < 0 || r == rnr)
          {
            var v = default(GLib.Value);
            for (var c = cmin; c <= cmax; c++)
            {
              store.GetValue(i, c, ref v);
              var val = v.Val as string;
              if (c >= 2)
              {
                var f = flist?.Get(c, r);
                if (f != null)
                {
                  Debug.Print($"Formula bold cnr {c - 2} rnry {r}");
                  f.Bold = !unbold;
                }
              }
              store.SetValue(i, c, Functions.MakeBold(val, unbold));
            }
          }
          r++;
        }
        while ((rnr < 0 || r <= rnr) && store.IterNext(ref i));
      }
    }
    else if (l == Menu_table_copy)
    {
      // Copy with formulas to CSV format.
      var lines = new List<string>();
      var r = 0;
      if (store.GetIterFirst(out var i))
      {
        var columns = store.NColumns;
        do
        {
          var cells = new List<string>();
          for (var c = 2; c < columns; c++)
          {
            var f = flist?.Get(c, r);
            var val = f?.Formula1;
            if (val == null)
            {
              var v = default(GLib.Value);
              store.GetValue(i, c, ref v);
              val = v.Val as string;
            }
            cells.Add(Functions.MakeBold(val ?? "", true));
          }
          lines.Add(Functions.EncodeCSV(cells));
          r++;
        }
        while (store.IterNext(ref i));
      }
      //// var csv = string.Join(Constants.CRLF, lines);
      UiTools.SaveFile(lines, Parameter.TempPath, M0(M1000), true, "csv");
    }
    else if (l == Menu_table_print)
    {
      // Copy with values to html format.
      var lines = new List<List<string>>();
      if (store.GetIterFirst(out var i))
      {
        var columns = store.NColumns;
        do
        {
          var cells = new List<string>();
          for (var c = 1; c < columns; c++)
          {
            var v = default(GLib.Value);
            store.GetValue(i, c, ref v);
            var val = v.Val as string;
            cells.Add(Functions.MakeBold(val ?? "", true));
          }
          lines.Add(cells);
        }
        while (store.IterNext(ref i));
        var r = Get(FactoryService.ClientService.GetTableReport(ServiceDaten, "", lines));
        UiTools.SaveFile(r, M0(M1000));
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
          var v = default(GLib.Value);
          store.GetValue(i, 0, ref v);
          var value = Functions.ToInt32(v.Val as string);
          if (z != value)
          {
            store.SetValue(i, 0, $"{z}");
            store.SetValue(i, 1, $"{z:000}");
          }
        }
        while (store.IterNext(ref i));
      }
    }
  }

  /// <summary>
  /// ButtonReleaseEvent for TreeView.
  /// </summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="args">Affected event args.</param>
  private void OnTableButtonReleaseEvent(object sender, ButtonReleaseEventArgs args)
  {
    if (sender is not Gtk.Widget cr)
      return;
    if (cr.Data["tv"] is not TreeView tv || cr.Data["store"] is not TreeStore store)
      return;
    if (args.Event.Button == 1)
    {
      var flist = tv.Data["flist"] as Formulas;
      flist?.BeginEdit(tv, store);
    }
    else if (args.Event.Button == 3)
    {
      tv.GetPathAtPos((int)args.Event.X, (int)args.Event.Y, out var path, out var column, out var cx, out var cy);
      var cnr = (int)column.Data["cnr"];
      var rnr = path.Indices[0];
      var r = tv.GetCellArea(path, column);
      var rtv = tv.Window.GetOrigin(out var xtv, out var ytv);
      if (r.Height > args.Event.YRoot - ytv)
      {
        // Header geklickt
        rnr = -1;
        cnr = 0;
        var cmax = tv.Columns.GetLength(0);
        var colx = xtv;
        while (cnr < cmax && colx + tv.GetColumn(cnr).Width < args.Event.XRoot)
        {
          colx += tv.GetColumn(cnr).Width;
          cnr++;
        }
      }
      tv.Data["menucnr"] = cnr; // Affected column.
      tv.Data["menurnr"] = rnr; // Affected row.
      //// Debug.Print($"Menu cnr {cnr} rnr {rnr} r {r} xtv {xtv} ytv {ytv} x {args.Event.X} y {args.Event.Y} xr {args.Event.XRoot} yr {args.Event.YRoot}");
      //// if (Functions.MachNichts() == 0)
      ////   return;
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
      mi = new MenuItem(Menu_table_bold);
      mi.Data["tv"] = tv;
      mi.Data["store"] = store;
      mi.ButtonPressEvent += OnTableMenuItemClick;
      m.Add(mi);
      mi = new MenuItem(Menu_table_normal);
      mi.Data["tv"] = tv;
      mi.Data["store"] = store;
      mi.ButtonPressEvent += OnTableMenuItemClick;
      m.Add(mi);
      mi = new MenuItem(Menu_table_copy);
      mi.Data["tv"] = tv;
      mi.Data["store"] = store;
      mi.ButtonPressEvent += OnTableMenuItemClick;
      m.Add(mi);
      mi = new MenuItem(Menu_table_print);
      mi.Data["tv"] = tv;
      mi.Data["store"] = store;
      mi.ButtonPressEvent += OnTableMenuItemClick;
      m.Add(mi);
      m.ShowAll();
      m.Popup();
    }
  }
}
