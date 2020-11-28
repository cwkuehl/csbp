// <copyright file="Date.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls
{
  using System;
  using CSBP.Base;
  using Gtk;
  using static CSBP.Resources.M;
  using static CSBP.Resources.Messages;

  public partial class Date : Gtk.Grid
  {
#pragma warning disable 169, 649

    /// <summary>CheckButton unknown.</summary>
    [Builder.Object]
    private CheckButton unknown;

    /// <summary>Entry date.</summary>
    [Builder.Object]
    private Entry date;

    /// <summary>Button down.</summary>
    [Builder.Object]
    private Button down;

    /// <summary>Label daytext.</summary>
    [Builder.Object]
    private Label daytext;

    /// <summary>Button yesterday.</summary>
    [Builder.Object]
    private Button yesterday;

    /// <summary>Button today.</summary>
    [Builder.Object]
    private Button today;

    /// <summary>Button tomorrow.</summary>
    [Builder.Object]
    private Button tomorrow;

    /// <summary>Calendar calendar.</summary>
    [Builder.Object]
    private Calendar calendar;

#pragma warning restore 169, 649

    DateTime? _value;

    bool entrecursion; // Entry-Rekursion vermeiden.

    bool calrecursion; // Calendar-Rekursion vermeiden.

    public bool IsNullable { get; set; }

    public bool IsWithCalendar { get; set; }

    public bool IsCalendarOpen { get; set; }

    public string YesterdayAccel
    {
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        yesterday.Label = $"-_{value}";
      }
    }

    public string TomorrowAccel
    {
      set
      {
        if (string.IsNullOrEmpty(value))
          return;
        tomorrow.Label = $"+_{value}";
      }
    }

    public DateTime? Value
    {
      get
      {
        return unknown.Active ? null : _value;
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
        if (_value.HasValue != value.HasValue || (value.HasValue && _value.Value != value.Value))
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
          monthchanged = _value.HasValue != value.HasValue || !_value.HasValue
              || _value.Value.Month != value.Value.Month || _value.Value.Year != value.Value.Year;
          _value = value;
        }
        if (datechanged)
        {
          DateChanged?.Invoke(this, new DateChangedEventArgs { Date = value });
          if (monthchanged)
            MonthChanged?.Invoke(this, new DateChangedEventArgs { Date = value });
        }
      }
    }

    public DateTime ValueNn { get { return Value ?? DateTime.Today; } }

    public Date(IntPtr handle) : base(handle)
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
        WidthChars = 9
      };
      Attach(date, 1, 0, 1, 1);
      down = new Button
      {
        Label = "",
        Visible = true,
        CanFocus = true,
        ReceivesDefault = true,
        Image = new Image
        {
          Pixbuf = Gtk.IconTheme.Default.LoadIcon("gtk-go-down", 8, 0)
        },
        AlwaysShowImage = true,
        NoShowAll = true,
      };
      down.Clicked += OnDownClicked;
      Attach(down, 2, 0, 1, 1);
      daytext = new Label
      {
        Text = "",
        Visible = true,
        Hexpand = false, // true
        MarginStart = 5
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
        // DetailWidthChars = 3,
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
      //ShowAll();
      Hide();
    }

    public event EventHandler<DateChangedEventArgs> DateChanged;

    public event EventHandler<DateChangedEventArgs> MonthChanged;

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

    public void ActivateDateChanged()
    {
      DateChanged?.Invoke(this, new DateChangedEventArgs { Date = _value });
    }

    protected void OnYesterdayClicked(object sender, EventArgs e)
    {
      var d = Functions.ToDateTime(date.Text);
      if (d.HasValue)
      {
        Value = d.Value.AddDays(-1);
      }
    }

    protected void OnTodayClicked(object sender, EventArgs e)
    {
      Value = DateTime.Today;
    }

    protected void OnTomorrowClicked(object sender, EventArgs e)
    {
      var d = Functions.ToDateTime(date.Text);
      if (d.HasValue)
      {
        Value = d.Value.AddDays(1);
      }
    }

    protected void OnDateChanged(object sender, EventArgs e)
    {
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

    /// <summary>Anzeigen Event.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnShown(object sender, EventArgs e)
    {
      if (IsNullable)
        unknown.Show();
      else
        unknown.Hide();
      if (IsWithCalendar)
      {
        down.Show();
      }
      else
      {
        down.Hide();
        IsCalendarOpen = false;
      }
      if (IsCalendarOpen)
      {
        calendar.Show();
      }
      else
      {
        calendar.Hide();
      }
    }

    protected void OnDownClicked(object sender, EventArgs e)
    {
      if (IsWithCalendar)
      {
        IsCalendarOpen = !IsCalendarOpen;
        Hide();
        ShowAll();
      }
    }

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

    public new void GrabFocus()
    {
      calendar.GrabFocus();
    }
  }

  public class DateChangedEventArgs : EventArgs
  {
    public DateTime? Date { get; set; }
  }
}
