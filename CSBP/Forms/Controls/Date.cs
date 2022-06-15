// <copyright file="Date.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System;
using System.Diagnostics.CodeAnalysis;
using CSBP.Base;
using Gtk;
using static CSBP.Resources.Messages;

/// <summary>
/// Date control.
/// </summary>
public partial class Date : Grid
{
  /// <summary>CheckButton unknown.</summary>
  [Builder.Object]
  private readonly CheckButton unknown;

  /// <summary>Entry date.</summary>
  [Builder.Object]
  private readonly Entry date;

  /// <summary>Button down.</summary>
  [Builder.Object]
  private readonly Button down;

  /// <summary>Label daytext.</summary>
  [Builder.Object]
  private readonly Label daytext;

  /// <summary>Button yesterday.</summary>
  [Builder.Object]
  private readonly Button yesterday;

  /// <summary>Button today.</summary>
  [Builder.Object]
  private readonly Button today;

  /// <summary>Button tomorrow.</summary>
  [Builder.Object]
  private readonly Button tomorrow;

  /// <summary>Calendar calendar.</summary>
  [Builder.Object]
  private readonly Calendar calendar;

  /// <summary>Internal value.</summary>
  private DateTime? intvalue;

  /// <summary>Avoids entry recursion value.</summary>
  private bool entrecursion;

  /// <summary>Avoids calendar recursion value.</summary>
  private bool calrecursion;

  /// <summary>
  /// Initializes a new instance of the <see cref="Date"/> class.
  /// </summary>
  /// <param name="handle">Handle for control.</param>
  public Date(IntPtr handle)
    : base(handle)
  {
    unknown = new CheckButton
    {
      Label = Date_unknown,
      Visible = true,
      CanFocus = true,
      TooltipText = Date_unknown_tt,
      NoShowAll = true,
    };
    Attach(unknown, 0, 0, 1, 1);
    date = new Entry
    {
      Visible = true,
      CanFocus = true,
      Hexpand = false,
      WidthChars = 10,
    };
    Attach(date, 1, 0, 1, 1);
    down = new Button
    {
      Label = "v",
      Visible = true,
      CanFocus = true,
      ReceivesDefault = true,
      //// Image = new Image
      //// {
      ////   Pixbuf = Gtk.IconTheme.Default.LoadIcon("gtk-go-down", 8, 0)
      //// },
      //// AlwaysShowImage = true,
      NoShowAll = true,
    };
    down.Clicked += OnDownClicked;
    Attach(down, 2, 0, 1, 1);
    daytext = new Label
    {
      Text = "",
      Visible = true,
      Hexpand = false, // true
      MarginStart = 5,
      WidthChars = 10,
    };
    Attach(daytext, 3, 0, 1, 1);
    yesterday = new Button
    {
      Label = Date_yesterday,
      Visible = true,
      CanFocus = true,
      Hexpand = true,
      FocusOnClick = false,
      TooltipText = Date_yesterday_tt,
      UseUnderline = true,
      MarginStart = 5,
    };
    yesterday.Clicked += OnYesterdayClicked;
    Attach(yesterday, 4, 0, 1, 1);
    today = new Button
    {
      Label = Date_today,
      Visible = true,
      CanFocus = true,
      Hexpand = true,
      FocusOnClick = false,
      TooltipText = Date_today_tt,
      UseUnderline = true,
    };
    today.Clicked += OnTodayClicked;
    Attach(today, 5, 0, 1, 1);
    tomorrow = new Button
    {
      Label = Date_tomorrow,
      Visible = true,
      CanFocus = true,
      Hexpand = true,
      FocusOnClick = false,
      TooltipText = Date_tomorrow_tt,
      UseUnderline = true,
    };
    tomorrow.Clicked += OnTomorrowClicked;
    Attach(tomorrow, 6, 0, 1, 1);
    calendar = new Calendar
    {
      Visible = true,
      CanFocus = true,
      Hexpand = true,
      Vexpand = false,
      //// DetailWidthChars = 3,
      NoShowAll = true,
    };
    calendar.DaySelected += OnCalendarDaySelected;
    Attach(calendar, 0, 1, 7, 1);
    Shown += OnShown;
    Value = null; // DateTime.Today;

    // var group = new AccelGroup();
    // yesterday.AddAccelerator("clicked", group, (int)Gdk.Key.m, Gdk.ModifierType.ControlMask, (Gtk.AccelFlags)0);
    // var g = new GLib.SimpleActionGroup();
    // var sa = new GLib.SimpleAction("nix", GLib.VariantType.Any);
    // sa.AddSignalHandler("clicked", OnYesterdayClicked);
    // g.AddAction(sa);
    // var p = Parent;
    // while (p != null && !(p is CsbpBin))
    //   p = p.Parent;
    // if (p != null && p is CsbpBin)
    //   p.InsertActionGroup(Name, g);
    // ShowAll();
    Hide();
  }

  /// <summary>Eventhandler for changed date.</summary>
  public event EventHandler<DateChangedEventArgs> DateChanged;

  /// <summary>Eventhandler for changed month.</summary>
  public event EventHandler<DateChangedEventArgs> MonthChanged;

  /// <summary>Gets or sets a value indicating whether the date can be null.</summary>
  public bool IsNullable { get; set; }

  /// <summary>Gets or sets a value indicating whether the calendar can be shown.</summary>
  public bool IsWithCalendar { get; set; }

  /// <summary>Gets or sets a value indicating whether the calendar is open.</summary>
  public bool IsCalendarOpen { get; set; }

  /// <summary>Gets or sets a value indicating whether there is a null label or not.</summary>
  public bool IsWithoutNullLabel { get; set; }

  /// <summary>Gets or sets a value indicating whether there is a day of week label or not.</summary>
  public bool IsWithoutDayOfWeek { get; set; }

  /// <summary>Sets the associated label for the calendar.</summary>
  public Label Label
  {
    set
    {
      if (value != null)
      {
        value.MnemonicWidget = calendar;
      }
    }
  }

  /// <summary>Sets the yesterday accelerator key.</summary>
  public string YesterdayAccel
  {
    set
    {
      if (string.IsNullOrEmpty(value))
        return;
      yesterday.Label = $"-_{value}";
    }
  }

  /// <summary>Sets the tomorrow accelerator key.</summary>
  public string TomorrowAccel
  {
    set
    {
      if (string.IsNullOrEmpty(value))
        return;
      tomorrow.Label = $"+_{value}";
    }
  }

  /// <summary>Gets or sets the DateTime value of the control.</summary>
  public DateTime? Value
  {
    get
    {
      return unknown.Active ? null : intvalue;
    }

    set
    {
      if (value.HasValue)
      {
        date.IsEditable = true;
        daytext.Text = Functions.ToStringWd(value.Value);
      }
      else
      {
        date.IsEditable = false;
        daytext.Text = string.Empty;
      }
      var datechanged = false;
      var monthchanged = false;
      if (!entrecursion)
      {
        try
        {
          entrecursion = true;
          unknown.Active = !value.HasValue;
          var d = Functions.ToString(value);
          date.Text = d;
        }
        finally
        {
          entrecursion = false;
        }
      }
      if (intvalue.HasValue != value.HasValue || (value.HasValue && intvalue.Value != value.Value))
      {
        datechanged = true;
        if (!calrecursion)
        {
          try
          {
            calrecursion = true;
            if (value.HasValue)
              calendar.Date = value.Value;
          }
          finally
          {
            calrecursion = false;
          }
        }
        monthchanged = intvalue.HasValue != value.HasValue || !intvalue.HasValue
            || intvalue.Value.Month != value.Value.Month || intvalue.Value.Year != value.Value.Year;
        intvalue = value;
      }
      if (datechanged)
      {
        DateChanged?.Invoke(this, new DateChangedEventArgs { Date = value });
        if (monthchanged)
          MonthChanged?.Invoke(this, new DateChangedEventArgs { Date = value });
      }
    }
  }

  /// <summary>Gets not null DateTime value.</summary>
  public DateTime ValueNn
  {
    get { return Value ?? DateTime.Today; }
  }

  /// <summary>
  /// Marks the days of the current month as bold.
  /// </summary>
  /// <param name="marks">List of bold days.</param>
  public void MarkMonth(bool[] marks)
  {
    for (var i = 0; i < 31; i++)
    {
      if (marks != null && marks.Length > i && marks[i])
        calendar.MarkDay((uint)i + 1);
      else
        calendar.UnmarkDay((uint)i + 1);
    }
  }

  /// <summary>Activates an date changed event.</summary>
  public void ActivateDateChanged()
  {
    DateChanged?.Invoke(this, new DateChangedEventArgs { Date = intvalue });
  }

  /// <summary>Grabs the calendar focus.</summary>
  public new void GrabFocus()
  {
    calendar.GrabFocus();
  }

  /// <summary>Handles yesterday.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnYesterdayClicked(object sender, EventArgs e)
  {
    var d = Functions.ToDateTime(date.Text);
    if (d.HasValue)
    {
      Value = d.Value.AddDays(-1);
    }
  }

  /// <summary>Handles today.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTodayClicked(object sender, EventArgs e)
  {
    Value = DateTime.Today;
  }

  /// <summary>Handles tomorrow.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnTomorrowClicked(object sender, EventArgs e)
  {
    var d = Functions.ToDateTime(date.Text);
    if (d.HasValue)
    {
      Value = d.Value.AddDays(1);
    }
  }

  /// <summary>Handles changed date.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDateChanged(object sender, EventArgs e)
  {
    // nicht verwendet: Problem bei einstelligen Tagen und Monaten.
    if (entrecursion || date.Text.Length != 10)
    {
      return;
    }
    try
    {
      entrecursion = true;
      var d = Functions.ToDateTime(date.Text);
      if (d.HasValue)
      {
        var c = date.Position;
        Value = d;
        date.Position = c;
      }
    }
    finally
    {
      entrecursion = false;
    }
  }

  /// <summary>Handles shown event.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnShown(object sender, EventArgs e)
  {
    if (IsNullable)
    {
      unknown.Label = IsWithoutNullLabel ? "" : Date_unknown;
      unknown.Show();
    }
    else
      unknown.Hide();
    if (IsWithCalendar)
      down.Show();
    else
    {
      down.Hide();
      IsCalendarOpen = false;
    }
    if (IsCalendarOpen)
      calendar.Show();
    else
      calendar.Hide();
    if (IsWithoutDayOfWeek)
      daytext.Hide();
  }

  /// <summary>Handles down.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDownClicked(object sender, EventArgs e)
  {
    if (IsWithCalendar)
    {
      IsCalendarOpen = !IsCalendarOpen;
      Hide();
      ShowAll();
    }
  }

  /// <summary>Handles calendar.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnCalendarDaySelected(object sender, EventArgs e)
  {
    if (calrecursion)
      return;
    try
    {
      calrecursion = true;
      Value = calendar.Date;
    }
    finally
    {
      calrecursion = false;
    }
  }
}

/// <summary>Event args for changed date.</summary>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed.")]
public class DateChangedEventArgs : EventArgs
{
  /// <summary>Gets or sets the affected date.</summary>
  public DateTime? Date { get; set; }
}
