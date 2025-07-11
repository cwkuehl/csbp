// <copyright file="MainWindow.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using CSBP.Base;
using CSBP.Forms.AD;
using CSBP.Forms.AG;
using CSBP.Forms.AM;
using CSBP.Forms.FZ;
using CSBP.Forms.HH;
using CSBP.Forms.SB;
using CSBP.Forms.SO;
using CSBP.Forms.TB;
using CSBP.Forms.WP;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Base;
using CSBP.Services.Factory; // Muss für build bleiben.
using CSBP.Services.Resources;
using Gtk;
using static CSBP.Services.Resources.M;
using static CSBP.Services.Resources.Messages;

/// <summary>Main Window.</summary>
public class MainWindow : Window
{
#pragma warning disable CS0649, SA1306

  /// <summary>PopoverMenu popovermenu1.</summary>
  [Builder.Object]
  private readonly PopoverMenu popovermenu1;

  /// <summary>Notebook with all tab dialogs.</summary>
  [Builder.Object]
  private readonly Notebook Notebook;

  /// <summary>Statusbar at the bottom.</summary>
  [Builder.Object]
  private readonly Statusbar Statusbar;

  /// <summary>Menu bar.</summary>
  [Builder.Object]
  private readonly MenuBar menubar;

  /// <summary>Menu Clients.</summary>
  [Builder.Object]
  private readonly MenuItem MenuClients;

  /// <summary>Menu Clients.</summary>
  [Builder.Object]
  private readonly Button menuclients;

  /// <summary>Menu Users.</summary>
  [Builder.Object]
  private readonly MenuItem MenuUsers;

  /// <summary>Menu Users.</summary>
  [Builder.Object]
  private readonly Button menuusers;

  /// <summary>Menu Backups.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBackups;

  /// <summary>Menu Backups.</summary>
  [Builder.Object]
  private readonly Button menubackups;

  /// <summary>Menu AI.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAi;

  /// <summary>Menu AI.</summary>
  [Builder.Object]
  private readonly Button menuai;

  /// <summary>Menu Login.</summary>
  [Builder.Object]
  private readonly MenuItem MenuLogin;

  /// <summary>Menu Login.</summary>
  [Builder.Object]
  private readonly Button menulogin;

  /// <summary>Menu Logout.</summary>
  [Builder.Object]
  private readonly MenuItem MenuLogout;

  /// <summary>Menu Logout.</summary>
  [Builder.Object]
  private readonly Button menulogout;

  /// <summary>Menu Password change.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPwchange;

  /// <summary>Menu Pwchange.</summary>
  [Builder.Object]
  private readonly Button menupwchange;

  /// <summary>Menu Options.</summary>
  [Builder.Object]
  private readonly MenuItem MenuOptions;

  /// <summary>Menu Options.</summary>
  [Builder.Object]
  private readonly Button menuoptions;

  /// <summary>Menu Start dialogs.</summary>
  [Builder.Object]
  private readonly MenuItem MenuDialogs;

  /// <summary>Menu Options.</summary>
  [Builder.Object]
  private readonly Button menudialogs;

  /// <summary>Menu Reset.</summary>
  [Builder.Object]
  private readonly MenuItem MenuReset;

  /// <summary>Menu Reset.</summary>
  [Builder.Object]
  private readonly Button menureset;

  /// <summary>Menu Diary.</summary>
  [Builder.Object]
  private readonly MenuItem MenuDiary;

  /// <summary>Menu Diary.</summary>
  [Builder.Object]
  private readonly Button menudiary;

  /// <summary>Menu Positions.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPositions;

  /// <summary>Menu Positions.</summary>
  [Builder.Object]
  private readonly Button menupositions;

  /// <summary>Menu Notes.</summary>
  [Builder.Object]
  private readonly MenuItem MenuNotes;

  /// <summary>Menu Notes.</summary>
  [Builder.Object]
  private readonly Button menunotes;

  /// <summary>Menu Persons/Addresses.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPersons;

  /// <summary>Menu Persons.</summary>
  [Builder.Object]
  private readonly Button menupersons;

  /// <summary>Menu Mileages.</summary>
  [Builder.Object]
  private readonly MenuItem MenuMileages;

  /// <summary>Menu Mileages.</summary>
  [Builder.Object]
  private readonly Button menumileages;

  /// <summary>Menu Bikes.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBikes;

  /// <summary>Menu Bikes.</summary>
  [Builder.Object]
  private readonly Button menubikes;

  /// <summary>Menu Books.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBooks;

  /// <summary>Menu Books.</summary>
  [Builder.Object]
  private readonly Button menubooks;

  /// <summary>Menu Authors.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAuthors;

  /// <summary>Menu Authors.</summary>
  [Builder.Object]
  private readonly Button menuauthors;

  /// <summary>Menu Series.</summary>
  [Builder.Object]
  private readonly MenuItem MenuSeries;

  /// <summary>Menu Series.</summary>
  [Builder.Object]
  private readonly Button menuseries;

  /// <summary>Menu Statistics.</summary>
  [Builder.Object]
  private readonly MenuItem MenuStatistics;

  /// <summary>Menu Statistics.</summary>
  [Builder.Object]
  private readonly Button menustatistics;

  /// <summary>Menu Sudoku.</summary>
  [Builder.Object]
  private readonly MenuItem MenuSudoku;

  /// <summary>Menu Sudoku.</summary>
  [Builder.Object]
  private readonly Button menusudoku;

  /// <summary>Menu Bookings.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBookings;

  /// <summary>Menu Bookings.</summary>
  [Builder.Object]
  private readonly Button menubookings;

  /// <summary>Menu Events.</summary>
  [Builder.Object]
  private readonly MenuItem MenuEvents;

  /// <summary>Menu Events.</summary>
  [Builder.Object]
  private readonly Button menuevents;

  /// <summary>Menu Accounts.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAccounts;

  /// <summary>Menu Accounts.</summary>
  [Builder.Object]
  private readonly Button menuaccounts;

  /// <summary>Menu Periods.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPeriods;

  /// <summary>Menu Periods.</summary>
  [Builder.Object]
  private readonly Button menuperiods;

  /// <summary>Menu Final balance.</summary>
  [Builder.Object]
  private readonly MenuItem MenuFinalbalance;

  /// <summary>Menu Final balance.</summary>
  [Builder.Object]
  private readonly Button menufinalbalance;

  /// <summary>Menu Profit loss balance.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPlbalance;

  /// <summary>Menu Profit loss balance.</summary>
  [Builder.Object]
  private readonly Button menuplbalance;

  /// <summary>Menu Opening balance.</summary>
  [Builder.Object]
  private readonly MenuItem MenuOpeningbalance;

  /// <summary>Menu Opening balance.</summary>
  [Builder.Object]
  private readonly Button menuopeningbalance;

  /// <summary>Menu Ancestors.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAncestors;

  /// <summary>Menu Ancestors.</summary>
  [Builder.Object]
  private readonly Button menuancestors;

  /// <summary>Menu Families.</summary>
  [Builder.Object]
  private readonly MenuItem MenuFamilies;

  /// <summary>Menu Families.</summary>
  [Builder.Object]
  private readonly Button menufamilies;

  /// <summary>Menu Sources.</summary>
  [Builder.Object]
  private readonly MenuItem MenuSources;

  /// <summary>Menu Sources.</summary>
  [Builder.Object]
  private readonly Button menusources;

  /// <summary>Menu Stocks.</summary>
  [Builder.Object]
  private readonly MenuItem MenuStocks;

  /// <summary>Menu Stocks.</summary>
  [Builder.Object]
  private readonly Button menustocks;

  /// <summary>Menu Configurations.</summary>
  [Builder.Object]
  private readonly MenuItem MenuConfigurations;

  /// <summary>Menu Configurations.</summary>
  [Builder.Object]
  private readonly Button menuconfigurations;

  /// <summary>Menu Chart.</summary>
  [Builder.Object]
  private readonly MenuItem MenuChart;

  /// <summary>Menu Chart.</summary>
  [Builder.Object]
  private readonly Button menuchart;

  /// <summary>Menu Investments.</summary>
  [Builder.Object]
  private readonly MenuItem MenuInvestments;

  /// <summary>Menu Investments.</summary>
  [Builder.Object]
  private readonly Button menuinvestments;

  /// <summary>Menu Investment bookings.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBookings3;

  /// <summary>Menu Investment bookings.</summary>
  [Builder.Object]
  private readonly Button menubookings3;

  /// <summary>Menu Prices.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPrices;

  /// <summary>Menu Prices.</summary>
  [Builder.Object]
  private readonly Button menuprices;

#pragma warning restore CS0649, SA1306

  /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
  /// <param name="app">Affected application.</param>
  /// <param name="builder">Affected builder.</param>
  /// <param name="handle">Affected handle.</param>
  protected MainWindow(Application app, Builder builder, IntPtr handle)
    : base(handle)
  {
    if (app != null)
      Application = app;
    Settings.Default.ApplicationPreferDarkTheme = true;
    builder.Autoconnect(this);
    var l = GetChildren();
    l.ForEach(c =>
    {
      if (c is MenuItem mi && !string.IsNullOrEmpty(mi.Label) && mi.Label.Contains('.', StringComparison.CurrentCulture))
      {
        mi.Label = Messages.Get(mi.Label);
      }
      if (c is ImageMenuItem imi && !string.IsNullOrEmpty(imi.Label))
      {
        if (imi.Label.StartsWith("gtk-", StringComparison.CurrentCulture))
        {
          var m = Messages.Get(imi.Label.Replace("gtk-", "Menu."));
          if (!string.IsNullOrEmpty(m))
            imi.Label = m;
        }
      }
      if (!string.IsNullOrEmpty(c.TooltipText) && c.TooltipText.StartsWith("Action.", StringComparison.CurrentCulture))
      {
        c.TooltipText = Messages.Get(c.TooltipText);
      }
    });
    var lpo = GetChildren(popovermenu1);
    lpo.ForEach(c =>
    {
      if (c is ModelButton mb)
      {
        if (!string.IsNullOrEmpty(mb.Text) && mb.Text.Contains('.', StringComparison.CurrentCulture))
          mb.Text = Messages.Get(mb.Text);
      }
      if (c is Button b)
      {
        if (b.Label == "Menu.undo")
          b.Clicked += OnMenuUndo;
        else if (b.Label == "Menu.redo")
          b.Clicked += OnMenuRedo;
        else if (b.Label == "Menu.clients")
          b.Clicked += OnMenuClients;
        else if (b.Label == "Menu.users")
          b.Clicked += OnMenuUsers;
        else if (b.Label == "Menu.backups")
          b.Clicked += OnMenuBackups;
        else if (b.Label == "Menu.ai")
          b.Clicked += OnMenuAi;
        else if (b.Label == "Menu.quit")
          b.Clicked += OnMenuQuit;
        else if (b.Label == "Menu.login")
          b.Clicked += OnMenuLogin;
        else if (b.Label == "Menu.logout")
          b.Clicked += OnMenuLogout;
        else if (b.Label == "Menu.pwchange")
          b.Clicked += OnMenuPwchange;
        else if (b.Label == "Menu.options")
          b.Clicked += OnMenuOptions;
        else if (b.Label == "Menu.dialogs")
          b.Clicked += OnMenuDialogs;
        else if (b.Label == "Menu.reset")
          b.Clicked += OnMenuReset;
        else if (b.Label == "Menu.diary")
          b.Clicked += OnMenuDiary;
        else if (b.Label == "Menu.positions")
          b.Clicked += OnMenuPositions;
        else if (b.Label == "Menu.notes")
          b.Clicked += OnMenuNotes;
        else if (b.Label == "Menu.persons")
          b.Clicked += OnMenuPersons;
        else if (b.Label == "Menu.mileages")
          b.Clicked += OnMenuMileages;
        else if (b.Label == "Menu.bikes")
          b.Clicked += OnMenuBikes;
        else if (b.Label == "Menu.books")
          b.Clicked += OnMenuBooks;
        else if (b.Label == "Menu.authors")
          b.Clicked += OnMenuAuthors;
        else if (b.Label == "Menu.series")
          b.Clicked += OnMenuSeries;
        else if (b.Label == "Menu.statistics")
          b.Clicked += OnMenuStatistics;
        else if (b.Label == "Menu.sudoku")
          b.Clicked += OnMenuSudoku;
        else if (b.Label == "Menu.bookings")
          b.Clicked += OnMenuBookings;
        else if (b.Label == "Menu.events")
          b.Clicked += OnMenuEvents;
        else if (b.Label == "Menu.accounts")
          b.Clicked += OnMenuAccounts;
        else if (b.Label == "Menu.periods")
          b.Clicked += OnMenuPeriods;
        else if (b.Label == "Menu.finalbalance")
          b.Clicked += OnMenuFinalbalance;
        else if (b.Label == "Menu.plbalance")
          b.Clicked += OnMenuPlbalance;
        else if (b.Label == "Menu.openingbalance")
          b.Clicked += OnMenuOpeningbalance;
        else if (b.Label == "Menu.ancestors")
          b.Clicked += OnMenuAncestors;
        else if (b.Label == "Menu.families")
          b.Clicked += OnMenuFamilies;
        else if (b.Label == "Menu.sources")
          b.Clicked += OnMenuSources;
        else if (b.Label == "Menu.stocks")
          b.Clicked += OnMenuStocks;
        else if (b.Label == "Menu.configurations")
          b.Clicked += OnMenuConfigurations;
        else if (b.Label == "Menu.chart")
          b.Clicked += OnMenuChart;
        else if (b.Label == "Menu.investments")
          b.Clicked += OnMenuInvestments;
        else if (b.Label == "Menu.bookings3")
          b.Clicked += OnMenuBookings3;
        else if (b.Label == "Menu.prices")
          b.Clicked += OnMenuPrices;
        else if (b.Label == "Menu.about")
          b.Clicked += OnMenuAbout;
        else if (b.Label == "Menu.help2")
          b.Clicked += OnMenuHelp2;
        if (!string.IsNullOrEmpty(b.Label) && b.Label.Contains('.', StringComparison.CurrentCulture))
          b.Label = Messages.Get(b.Label);
      }
    });
    ////var actionGroup = new GLib.SimpleActionGroup();
    ////var action = new GLib.SimpleAction("undo", null);
    ////action.Activate(new GLib.Variant("Hallo"));
    ////action.Activated += OnMenuUndo;
    ////actionGroup.AddAction(action);
    ////app.AddAction(action);
    Icon = Gdk.Pixbuf.LoadFromResource("CSBP.Resources.Icons.WKHH.gif");

    // Icon.Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icon/test.png", "png");
    // find /usr/share/icons -type f -name '*.png'
    if (Functions.MachNichts() != 0)
    {
      var path = "/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons";
      var theme = IconTheme.Default;
      //// theme.CustomTheme = "Yaru-olive-dark";
      //// theme.RescanIfNeeded();
      var names = new List<string>
      {
        "applications-graphics", "dialog-cancel",
        "document-new", "document-edit", "document-print", "document-save",
        "edit-clear", "edit-copy", "edit-delete", "edit-find", "edit-paste", "edit-redo", "edit-undo",
        "format-justify-fill", "go-first", "go-last", "go-next", "go-previous", "list-add", "list-remove",
        "media-floppy", "view-refresh", "weather-few-clouds",
      };
      foreach (var n in names)
      {
        var icon = theme.LoadIcon(n, 48, 0);
        icon.Save(System.IO.Path.Combine(path, n + ".png"), "png");
      }
    }

    SetupHandlers();
    var size = ParameterGui.GetDialogSize(typeof(MainWindow));
    SetSizeRequest(size.Width, size.Height);
    DefaultWidth = size.Width;
    DefaultHeight = size.Height;
    Resizable = true;
    var architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
    if (architecture.ToString() == "Arm64")
      SetPosition(WindowPosition.None);
    else if (size.X == -1 && size.Y == -1)
      SetPosition(WindowPosition.Center);
    else
      Window.Move(size.X, size.Y);
    var ob = Observable.FromEvent<SizeAllocatedHandler, SizeAllocatedArgs>(
      h0 =>
      {
        void H(object sender, SizeAllocatedArgs e)
        {
          h0(e);
        }
        return H;
      },
      h => SizeAllocated += h, h => SizeAllocated -= h
    ).Throttle(TimeSpan.FromMilliseconds(1000));
    ob.Subscribe(e =>
    {
      Application.Invoke((sender, e1) =>
      {
        if (Window != null && Visible)
        {
          SetSizeRequest(0, 0); // Enables MainWindow to get smaller
          //// Window.GetGeometry(out int _, out int _, out int w, out int h);
          //// Window.GetOrigin(out int x, out int y);
          //// Parameter.SetDialogSize(typeof(MainWindow), new Rectangle(x, y, w, h));
        }
      });
    });
    KeyPressEvent += (object sender, KeyPressEventArgs e) =>
    {
      if (e.Event.Key == Gdk.Key.F1)
      {
        MainClass.Help();
      }
      else if (e.Event.Key == Gdk.Key.Escape)
      {
        if (!ServiceBase.IsUndoRedo(UiTools.SessionId) || CsbpBin.ShowYesNoQuestion(M0(AG003)))
          MainClass.Quit();
      }
    };
  }

  /// <summary>Creates main window.</summary>
  /// <returns>The MainWindow.</returns>
  /// <param name="app">Affected application.</param>
  public static MainWindow Create(Application app)
  {
    var builder = new Builder("CSBP.GtkGui.MainWindow.glade");
    return new MainWindow(app, builder, builder.GetObject("MainWindow").Handle);
  }

  /// <summary>Starten der Anmeldung und der Tabs.</summary>
  /// <param name="p">Affected MainWindow.</param>
  public void Start(Gtk.Window p)
  {
    Notebook.RemovePage(0);
    MainClass.InitDb(new ServiceDaten(UiTools.SessionId, 0, "Admin", null));
#if DEBUG
    var username = Environment.UserName;
    //// Automatic login with current user.
    MainClass.Login(new ServiceDaten(UiTools.SessionId, 1, username.ToLower(), [UserDaten.RoleAdmin] )); // Lower for Replication.
    //// MainClass.Login(new ServiceDaten(3, "Wolfgang"));
#else
    var daten = new ServiceDaten(UiTools.SessionId, Functions.ToInt32(ParameterGui.LoginClient), Environment.UserName, null);
    var r = FactoryService.LoginService.IsWithoutPassword(daten);
    if (r?.Ergebnis == null)
      CsbpBin.Start(typeof(AM000Login), AM000_title, p: p, modal: true);
    else
      MainClass.Login(new ServiceDaten(r.Ergebnis));
#endif

    // MenuClients.Activate();
    // MenuUsers.Activate();
    // MenuBackups.Activate();
    // MenuAi.Activate();
    // MenuPwchange.Activate();
    // MenuOptions.Activate();
    // MenuDialogs.Activate();
    // MenuDiary.Activate();
    // MenuPositions.Activate();
    // MenuNotes.Activate();
    // MenuPersons.Activate();
    // MenuMileages.Activate();
    // MenuBikes.Activate();
    // MenuBooks.Activate();
    // MenuAuthors.Activate();
    // MenuSeries.Activate();
    // MenuSudoku.Activate();
    // MenuPeriods.Activate();
    // MenuAccounts.Activate();
    // MenuEvents.Activate();
    // MenuBookings.Activate();
    // MenuOpeningbalance.Activate();
    // MenuPlbalance.Activate();
    // MenuFinalbalance.Activate();
    // MenuAncestors.Activate();
    // MenuFamilies.Activate();
    // MenuSources.Activate();
    // MenuStocks.Activate();
    // MenuChart.Activate();
    // MenuConfigurations.Activate();
    // MenuInvestments.Activate();
    // MenuBookings3.Activate();
    // MenuPrices.Activate();
    if (Functions.MachNichts() != 0)
    {
      var alist = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.GetName().Name).ToList();
      var sb = new StringBuilder();
      foreach (var assembly in alist)
      {
        var name = assembly.GetName();
        var loc = "not available";
        try
        {
          loc = assembly.Location;
        }
        catch (Exception)
        {
          // Ignore.
        }
        sb.Append($"Name={name.Name} Version={name.Version} Location={loc}").Append(Constants.CRLF);
      }
      ServiceBase.Log.Warn(sb.ToString());
    }
  }

  /// <summary>Resets the main window position and size.</summary>
  public void Reset()
  {
    Application.Invoke((sender, e) =>
    {
      if (Window != null)
      {
        Hide();
        SetPosition(WindowPosition.Center);
        SetSizeRequest(400, 300);
        ShowAll();
      }
    });
  }

  /// <summary>
  /// Aktualisieren des Anwendungstitels.
  /// </summary>
  public void RefreshTitle()
  {
    var daten = MainClass.ServiceDaten;
    var test = Parameter.GetValue(Parameter.AG_TEST_PRODUKTION) == "TEST" ? "Test-" : "";
#if DEBUG
    var client = "";
#else
    var client = daten.MandantNr == 0 ? M0(AM005) : daten.MandantNr == 1 ? "" : $" ({AG110_title} {daten.MandantNr})";
#endif
    Title = $"{test}CSBP {Parameter.GetValue(Parameter.AG_ANWENDUNGS_TITEL)} W. Kuehl{client}";
  }

  /// <summary>
  /// Set permissions for application.
  /// </summary>
  /// <param name="b">Is user logged in or not.</param>
  /// <param name="per">Affected optional permission.</param>
  public void SetPermission(bool b = false, int per = (int)PermissionEnum.Without)
  {
    RefreshTitle();

    menubar.Visible = false;
    MenuClients.Visible = menuclients.Visible = b && per >= (int)PermissionEnum.Admin;
    MenuUsers.Visible = menuusers.Visible = b;
    MenuBackups.Visible = menubackups.Visible = b;
    MenuAi.Visible = menuai.Visible = b;

    MenuLogin.Visible = menulogin.Visible = !b;
    MenuLogout.Visible = menulogout.Visible = b;
    MenuPwchange.Visible = menupwchange.Visible = b;
    MenuOptions.Visible = menuoptions.Visible = b;
    MenuDialogs.Visible = menudialogs.Visible = b;
    MenuReset.Visible = menureset.Visible = true;

    MenuDiary.Visible = menudiary.Visible = b;
    MenuPositions.Visible = menupositions.Visible = b;
    MenuNotes.Visible = menunotes.Visible = b;
    MenuPersons.Visible = menupersons.Visible = b;
    MenuMileages.Visible = menumileages.Visible = b;
    MenuBikes.Visible = menubikes.Visible = b;
    MenuBooks.Visible = menubooks.Visible = b;
    MenuAuthors.Visible = menuauthors.Visible = b;
    MenuSeries.Visible = menuseries.Visible = b;
    MenuStatistics.Visible = menustatistics.Visible = b;
    MenuSudoku.Visible = menusudoku.Visible = b;

    MenuBookings.Visible = menubookings.Visible = b;
    MenuEvents.Visible = menuevents.Visible = b;
    MenuAccounts.Visible = menuaccounts.Visible = b;
    MenuPeriods.Visible = menuperiods.Visible = b;
    MenuFinalbalance.Visible = menufinalbalance.Visible = b;
    MenuPlbalance.Visible = menuplbalance.Visible = b;
    MenuOpeningbalance.Visible = menuopeningbalance.Visible = b;

    MenuAncestors.Visible = menuancestors.Visible = b;
    MenuFamilies.Visible = menufamilies.Visible = b;
    MenuSources.Visible = menusources.Visible = b;

    MenuStocks.Visible = menustocks.Visible = b;
    MenuConfigurations.Visible = menuconfigurations.Visible = b;
    MenuChart.Visible = menuchart.Visible = b;
    MenuInvestments.Visible = menuinvestments.Visible = b;
    MenuBookings3.Visible = menubookings3.Visible = b;
    MenuPrices.Visible = menuprices.Visible = b;
  }

  /// <summary>
  /// Anzeigen einer Meldung in der Statusleiste.
  /// </summary>
  /// <param name="e">Betroffener Text.</param>
  public void SetError(string e)
  {
    var contextid = Statusbar.GetContextId("Error");
    if (string.IsNullOrWhiteSpace(e))
    {
      Statusbar.Pop(contextid);
      //// Statusbar.Remove(contextid, 2);
    }
    else
    {
      Statusbar.Pop(contextid);
      Statusbar.Push(contextid, e);
    }
  }

  /// <summary>
  /// Hinzufügen einer Seite zum Notebook.
  /// </summary>
  /// <param name="widget">Affected dialog.</param>
  /// <param name="label">Affected notebook title.</param>
  /// <param name="focus">The focus can be grabbed.</param>
  public void AppendPage(CsbpBin widget, string label, bool focus = false)
  {
    var title = label.Replace("_", ""); // .Replace("&", "&amp;");
    if (Functions.MachNichts() == 0)
    {
      var closeImage = Image.NewFromIconName("window-close", IconSize.Button);
      var button = new Button();
      var lbl = new Label(title);
      if (title.Length > 18)
      {
        var sh = title.Left(18) + "...";
        lbl.Text = sh ?? "";
        lbl.TooltipText = title;
      }
      var tab = new Box(Orientation.Horizontal, 0);
      button.Relief = ReliefStyle.None;
      button.Add(closeImage);
      tab.PackStart(lbl, false, false, 0);
      tab.PackStart(button, false, false, 0);
      tab.ShowAll();
      var p = Notebook.AppendPage(widget, tab);
      Notebook.SetTabReorderable(widget, true);
      button.Clicked += (object sender, EventArgs e) =>
      {
        Notebook.Remove(widget);
        widget.Close();
        widget.Dispose();
      };
      Notebook.Page = p;
    }
    else
    {
      var l = new Label
      {
        UseMarkup = true,
        Markup = $"<span size='small'>{title}</span> <span color='red' size='large'>x</span>",
      };
      var eb = new EventBox
        {
          l,
        };
      eb.Events = Gdk.EventMask.ButtonPressMask;
      eb.ButtonPressEvent += (object sender, ButtonPressEventArgs e) =>
      {
        if (e.Event.Button == 1 && e.Event.X >= e.Event.Window.Width - (e.Event.Window.Height / 2))
        {
          // Click im linken Bereich beim x.
          Notebook.Remove(widget);
          widget.Close();
          widget.Dispose();
        }
      };
      eb.ShowAll();
      var p = Notebook.AppendPage(widget, eb);
      Notebook.SetTabReorderable(widget, true);
      Notebook.ShowAll();
      Notebook.Page = p;
    }
    if (focus)
    {
      Notebook.GrabFocus();
      widget.GrabFocus();
    }
  }

  /// <summary>Schließen aller Tabs.</summary>
  public void ClosePages()
  {
    while (Notebook.NPages > 0)
    {
      if (Notebook.GetNthPage(0) is not CsbpBin p)
        Notebook.RemovePage(0);
      else
      {
        Notebook.Remove(p);
        p.Close();
        p.Dispose();
      }
    }
  }

  /// <summary>
  /// Get active dialog of notebook.
  /// </summary>
  /// <returns>Active dialog of notebook.</returns>
  public CsbpBin GetActiveDialog()
  {
    if (Notebook.NPages > 0)
    {
      var p = Notebook.GetNthPage(Notebook.Page) as CsbpBin;
      return p;
    }
    return null;
  }

  /// <summary>
  /// Fokussieren bzw.async Wechseln zu einer Seite.
  /// </summary>
  /// <typeparam name="T">Betroffener Dialog-Type.</typeparam>
  /// <returns>Betroffene Dialog-Instanz.</returns>
  public T FocusPage<T>()
    where T : Widget
  {
    for (var i = 0; i < Notebook.NPages; i++)
    {
      var p = Notebook.GetNthPage(i);
      if (p is T)
      {
        Notebook.CurrentPage = i;
        return p as T;
      }
    }
    return null;
  }

  /// <summary>
  /// Get the .NET version.
  /// </summary>
  /// <returns>.NET version as string.</returns>
  protected static string GetNetCoreVersion()
  {
    var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
    var assemblyPath = assembly.Location.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
    int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
    if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
    {
      // e.g. 6.0.5
      return assemblyPath[netCoreAppIndex + 1];
      //// return null;
    }
    var ver = Assembly.GetEntryAssembly()?.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName;
    //// e.g. .NETCoreApp,Version=v6.0
    return ver;
  }

  /// <summary>Initialisierung der Events.</summary>
  protected void SetupHandlers()
  {
    DeleteEvent += OnDeleteEvent;
  }

  /// <summary>Menu open or close.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuClicked(object sender, EventArgs e)
  {
    // Functions.MachNichts();
    popovermenu1.Visible = !popovermenu1.Visible;
  }

  /// <summary>Menu Rückgängig.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuUndo(object sender, EventArgs e)
  {
    MainClass.Undo();
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Wiederherstellen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuRedo(object sender, EventArgs e)
  {
    MainClass.Redo();
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Mandanten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuClients(object sender, EventArgs e)
  {
    AppendPage(AG100Clients.Create(), AG100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Benutzer.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuUsers(object sender, EventArgs e)
  {
    AppendPage(AG200Users.Create(), AG200_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Sicherungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBackups(object sender, EventArgs e)
  {
    AppendPage(AG400Backups.Create(), AG400_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Künstliche Intelligenz.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAi(object sender, EventArgs e)
  {
    AppendPage(AG500Ai.Create(), AG500_title, true);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Schließen der Anwendung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuQuit(object sender, EventArgs e)
  {
    MainClass.Quit();
  }

  /// <summary>Menu Anmeldung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuLogin(object sender, EventArgs e)
  {
    //// AppendPage(AM000Login.Create(), AM000_title);
    CsbpBin.Start(typeof(AM000Login), AM000_title, modal: true);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Abmeldung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuLogout(object sender, EventArgs e)
  {
    ClosePages();
    MainClass.Logout();
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Kennwort-Änderung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPwchange(object sender, EventArgs e)
  {
    CsbpBin.Start(typeof(AM100Change), AM100_title, modal: true);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Einstellungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuOptions(object sender, EventArgs e)
  {
    CsbpBin.Start(typeof(AM500Options), AM500_title, modal: true);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Start-Formulare.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuDialogs(object sender, EventArgs e)
  {
    CsbpBin.Start(typeof(AM510Dialogs), AM510_title, modal: true);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Dialoge zurücksetzen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuReset(object sender, EventArgs e)
  {
    ParameterGui.ResetDialogSizes();
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Tagebuch.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuDiary(object sender, EventArgs e)
  {
    AppendPage(TB100Diary.Create(), TB100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Positionen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPositions(object sender, EventArgs e)
  {
    AppendPage(TB200Positions.Create(), TB200_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Notizen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuNotes(object sender, EventArgs e)
  {
    AppendPage(FZ700Memos.Create(), FZ700_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Personen/Adressen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPersons(object sender, EventArgs e)
  {
    AppendPage(AD100Persons.Create(), AD100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Fahrradstände.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuMileages(object sender, EventArgs e)
  {
    AppendPage(FZ250Mileages.Create(), FZ250_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Fahrraäder.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBikes(object sender, EventArgs e)
  {
    AppendPage(FZ200Bikes.Create(), FZ200_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Bücher.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBooks(object sender, EventArgs e)
  {
    AppendPage(FZ340Books.Create(), FZ340_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Autoren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAuthors(object sender, EventArgs e)
  {
    AppendPage(FZ300Authors.Create(), FZ300_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Serien.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuSeries(object sender, EventArgs e)
  {
    AppendPage(FZ320Series.Create(), FZ320_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Statistik.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuStatistics(object sender, EventArgs e)
  {
    AppendPage(FZ100Statistics.Create(), FZ100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Sudoku.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuSudoku(object sender, EventArgs e)
  {
    AppendPage(SO100Sudoku.Create(), SO100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Buchungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBookings(object sender, EventArgs e)
  {
    AppendPage(HH400Bookings.Create(), HH400_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Ereignisse.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuEvents(object sender, EventArgs e)
  {
    AppendPage(HH300Events.Create(), HH300_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Konten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAccounts(object sender, EventArgs e)
  {
    AppendPage(HH200Accounts.Create(), HH200_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Perioden.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPeriods(object sender, EventArgs e)
  {
    AppendPage(HH100Periods.Create(), HH100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Schlussbilanz.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuFinalbalance(object sender, EventArgs e)
  {
    AppendPage(HH500Balance.Create(Constants.KZBI_SCHLUSS), HH500_title_SB);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Gewinn+Verlust-Rechnung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPlbalance(object sender, EventArgs e)
  {
    AppendPage(HH500Balance.Create(Constants.KZBI_GV), HH500_title_GV);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Eröffnungsbilanz.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuOpeningbalance(object sender, EventArgs e)
  {
    AppendPage(HH500Balance.Create(Constants.KZBI_EROEFFNUNG), HH500_title_EB);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Ahnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAncestors(object sender, EventArgs e)
  {
    AppendPage(SB200Ancestors.Create(), SB200_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Familien.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuFamilies(object sender, EventArgs e)
  {
    AppendPage(SB300Families.Create(), SB300_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Quellen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuSources(object sender, EventArgs e)
  {
    AppendPage(SB400Sources.Create(), SB400_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Wertpapiere.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuStocks(object sender, EventArgs e)
  {
    AppendPage(WP200Stocks.Create(), WP200_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Konfigurationen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuConfigurations(object sender, EventArgs e)
  {
    AppendPage(WP300Configurations.Create(), WP300_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Chart.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuChart(object sender, EventArgs e)
  {
    AppendPage(WP100Chart.Create(), WP100_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Anlagen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuInvestments(object sender, EventArgs e)
  {
    AppendPage(WP250Investments.Create(), WP250_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Buchungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBookings3(object sender, EventArgs e)
  {
    AppendPage(WP400Bookings.Create(), WP400_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Stände.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPrices(object sender, EventArgs e)
  {
    AppendPage(WP500Prices.Create(), WP500_title);
    popovermenu1.Visible = false;
  }

  /// <summary>Menu Über.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAbout(object sender, EventArgs e)
  {
    popovermenu1.Visible = false;
    var daten = MainClass.ServiceDaten;
    var assembly = Assembly.GetEntryAssembly();
    var ver = assembly?.GetName().Version.ToString() ?? "1.1";
    var loc = assembly?.Location;
    var date = string.IsNullOrEmpty(loc) ? "" : Functions.ToString(System.IO.File.GetCreationTime(loc), true);
    var db = Parameter.Connect;
    var roles = string.Join(", ", daten.Daten.Rollen);
    using var about = new AboutDialog
    {
      Title = "", // Titel geht nicht.
      ProgramName = "CSharp Budget Program",
      Version = $"{ver} ({date}), Runtime {GetNetCoreVersion()}",
      Copyright = "(c) 2019-2025 Wolfgang Kuehl",
      Comments = $"""
      CSBP is a simple budget program.
      Database: {db}
      Client: {daten.MandantNr} User: {daten.BenutzerId} Roles: {roles}
      """,
      Website = "https://cwkuehl.de",
      Logo = Gdk.Pixbuf.LoadFromResource("CSBP.Resources.Icons.WKHH.gif"),
    };
    about.Run();
    about.Hide();
  }

  /// <summary>Menu Help.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuHelp2(object sender, EventArgs e)
  {
    MainClass.Help();
    popovermenu1.Visible = false;
  }

  /// <summary>Schließen des Fensters und der Anwendung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnDeleteEvent(object sender, DeleteEventArgs e)
  {
    MainClass.Quit();
    e.RetVal = true;
  }

  /// <summary>
  /// Liefert alle Kindelemente eines Containers als Liste.
  /// </summary>
  /// <param name="con">Betroffener Container.</param>
  /// <param name="l">Falls nicht null, wird diese Liste ergänzt.</param>
  /// <returns>Liste mit Kindelementen.</returns>
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
      if (c is MenuItem && (c as MenuItem).Submenu is Menu)
        GetChildren((c as MenuItem).Submenu as Container, l);
    }
    return l;
  }
}
