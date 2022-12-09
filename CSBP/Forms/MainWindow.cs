// <copyright file="MainWindow.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using System.Reflection;
using CSBP.Apis.Enums;
using CSBP.Apis.Services;
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
using CSBP.Resources;
using CSBP.Services.Base;
using CSBP.Services.Factory; // Muss für build bleiben.
using Gtk;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Main Window.</summary>
public class MainWindow : Window
{
#pragma warning disable CS0649, SA1306

  /// <summary>Notebook with all tab dialogs.</summary>
  [Builder.Object]
  private readonly Notebook Notebook;

  /// <summary>Statusbar at the bottom.</summary>
  [Builder.Object]
  private readonly Statusbar Statusbar;

  /// <summary>Menu Clients.</summary>
  [Builder.Object]
  private readonly MenuItem MenuClients;

  /// <summary>Menu Users.</summary>
  [Builder.Object]
  private readonly MenuItem MenuUsers;

  /// <summary>Menu Backups.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBackups;

  /// <summary>Menu Login.</summary>
  [Builder.Object]
  private readonly MenuItem MenuLogin;

  /// <summary>Menu Logout.</summary>
  [Builder.Object]
  private readonly MenuItem MenuLogout;

  /// <summary>Menu Password change.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPwchange;

  /// <summary>Menu Options.</summary>
  [Builder.Object]
  private readonly MenuItem MenuOptions;

  /// <summary>Menu Start dialogs.</summary>
  [Builder.Object]
  private readonly MenuItem MenuDialogs;

  /// <summary>Menu Reset.</summary>
  [Builder.Object]
  private readonly MenuItem MenuReset;

  /// <summary>Menu Diary.</summary>
  [Builder.Object]
  private readonly MenuItem MenuDiary;

  /// <summary>Menu Positions.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPositions;

  /// <summary>Menu Notes.</summary>
  [Builder.Object]
  private readonly MenuItem MenuNotes;

  /// <summary>Menu Persons/Addresses.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPersons;

  /// <summary>Menu Mileages.</summary>
  [Builder.Object]
  private readonly MenuItem MenuMileages;

  /// <summary>Menu Bikes.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBikes;

  /// <summary>Menu Books.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBooks;

  /// <summary>Menu Authors.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAuthors;

  /// <summary>Menu Series.</summary>
  [Builder.Object]
  private readonly MenuItem MenuSeries;

  /// <summary>Menu Statistics.</summary>
  [Builder.Object]
  private readonly MenuItem MenuStatistics;

  /// <summary>Menu Sudoku.</summary>
  [Builder.Object]
  private readonly MenuItem MenuSudoku;

  /// <summary>Menu Bookings.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBookings;

  /// <summary>Menu Events.</summary>
  [Builder.Object]
  private readonly MenuItem MenuEvents;

  /// <summary>Menu Accounts.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAccounts;

  /// <summary>Menu Periods.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPeriods;

  /// <summary>Menu Final balance.</summary>
  [Builder.Object]
  private readonly MenuItem MenuFinalbalance;

  /// <summary>Menu Profit loss balance.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPlbalance;

  /// <summary>Menu Opening balance.</summary>
  [Builder.Object]
  private readonly MenuItem MenuOpeningbalance;

  /// <summary>Menu Ancestors.</summary>
  [Builder.Object]
  private readonly MenuItem MenuAncestors;

  /// <summary>Menu Families.</summary>
  [Builder.Object]
  private readonly MenuItem MenuFamilies;

  /// <summary>Menu Sources.</summary>
  [Builder.Object]
  private readonly MenuItem MenuSources;

  /// <summary>Menu Stocks.</summary>
  [Builder.Object]
  private readonly MenuItem MenuStocks;

  /// <summary>Menu Konfigurations.</summary>
  [Builder.Object]
  private readonly MenuItem MenuConfigurations;

  /// <summary>Menu Chart.</summary>
  [Builder.Object]
  private readonly MenuItem MenuChart;

  /// <summary>Menu Investments.</summary>
  [Builder.Object]
  private readonly MenuItem MenuInvestments;

  /// <summary>Menu Investments booking.</summary>
  [Builder.Object]
  private readonly MenuItem MenuBookings3;

  /// <summary>Menu Prices.</summary>
  [Builder.Object]
  private readonly MenuItem MenuPrices;

#pragma warning restore CS0649, SA1306

  /// <summary>Initializes a new instance of the <see cref="MainWindow"/> class.</summary>
  /// <param name="builder">Affected builder.</param>
  /// <param name="handle">Affected handle.</param>
  protected MainWindow(Builder builder, IntPtr handle)
    : base(handle)
  {
    Settings.Default.ApplicationPreferDarkTheme = true;
    builder.Autoconnect(this);
    var l = GetChildren();
    l.ForEach(c =>
    {
      if (c is MenuItem mi && !string.IsNullOrEmpty(mi.Label) && mi.Label.Contains('.', StringComparison.CurrentCulture))
      {
        mi.Label = Messages.Get(mi.Label);
      }
      if (c is ImageMenuItem imi && !string.IsNullOrEmpty(imi.Label) && imi.Label.StartsWith("gtk-", StringComparison.CurrentCulture))
      {
        var m = Messages.Get(imi.Label.Replace("gtk-", "Menu."));
        if (!string.IsNullOrEmpty(m))
          imi.Label = m;
      }
    });
    Icon = Gdk.Pixbuf.LoadFromResource("CSBP.Resources.Icons.WKHH.gif");

    // Icon.Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icon/test.png", "png");
    // this.RenderIconPixbuf("gtk-refresh", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-refresh.png", "png");
    // this.RenderIconPixbuf("gtk-undo", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-undo.png", "png");
    // this.RenderIconPixbuf("gtk-redo", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-redo.png", "png");
    // this.RenderIconPixbuf("gtk-new", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-new.png", "png");
    // this.RenderIconPixbuf("gtk-copy", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-copy.png", "png");
    // this.RenderIconPixbuf("gtk-edit", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-edit.png", "png");
    // this.RenderIconPixbuf("gtk-delete", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-delete.png", "png");
    // this.RenderIconPixbuf("gtk-floppy", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-floppy.png", "png");
    // this.RenderIconPixbuf("gtk-print", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-print.png", "png");
    // this.RenderIconPixbuf("gtk-paste", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-paste.png", "png");
    // this.RenderIconPixbuf("gtk-clear", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-clear.png", "png");
    // this.RenderIconPixbuf("gtk-add", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-add.png", "png");
    // this.RenderIconPixbuf("gtk-remove", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-remove.png", "png");
    // this.RenderIconPixbuf("gtk-save", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-save.png", "png");
    // this.RenderIconPixbuf("gtk-goto-first", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-goto-first.png", "png");
    // this.RenderIconPixbuf("gtk-go-back", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-go-back.png", "png");
    // this.RenderIconPixbuf("gtk-go-forward", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-go-forward.png", "png");
    // this.RenderIconPixbuf("gtk-goto-last", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-goto-last.png", "png");
    // this.RenderIconPixbuf("gtk-justify-fill", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-justify-fill.png", "png");
    // this.RenderIconPixbuf("gtk-select-color", Gtk.IconSize.Button).Save("/home/wolfgang/cs/csbp/Asciidoc/de/assets/icons/gtk-select-color.png", "png");
    SetupHandlers();
    var size = Parameter.GetDialogSize(typeof(MainWindow));
    Console.WriteLine($"MainWindow Old size x {size.X} y {size.Y} w {size.Width} h {size.Height}");
    SetSizeRequest(size.Width, size.Height);
    DefaultWidth = size.Width;
    DefaultHeight = size.Height;
    Resizable = true;
    if (size.X == -1 && size.Y == -1)
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
          SetSizeRequest(0, 0);
          Window.GetGeometry(out int x0, out int y0, out int w, out int h);
          Window.GetOrigin(out int x, out int y);
          //// Höhe der Titelleiste abziehen
          Parameter.SetDialogSize(typeof(MainWindow), new Rectangle(x, y - CsbpBin.TitleHeight, w, h));
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
        if (!ServiceBase.IsUndoRedo() || CsbpBin.ShowYesNoQuestion(M0(AG003)))
          MainClass.Quit();
      }
    };
  }

  /// <summary>Creates main window.</summary>
  /// <returns>The MainWindow.</returns>
  public static MainWindow Create()
  {
    var builder = new Builder("CSBP.GtkGui.MainWindow.glade");
    return new MainWindow(builder, builder.GetObject("MainWindow").Handle);
  }

  /// <summary>Starten der Anmeldung und der Tabs.</summary>
  /// <param name="p">Betroffenes Main Window.</param>
  public void Start(Gtk.Window p)
  {
    Notebook.RemovePage(0);
    MainClass.InitDb(new ServiceDaten(0, "Admin"));
#if DEBUG
    var username = Environment.UserName;
    //// Automatic login with current user.
    MainClass.Login(new ServiceDaten(1, username.ToFirstUpper()));
    //// MainClass.Login(new ServiceDaten(3, "Wolfgang"));
#else
    var daten = new ServiceDaten(Functions.ToInt32(Parameter.LoginClient), Environment.UserName);
    var r = FactoryService.LoginService.IsWithoutPassword(daten);
    if (r?.Ergebnis ?? false)
      MainClass.Login(daten);
    else
      CsbpBin.Start(typeof(AM000Login), AM000_title, p: p, modal: true);
#endif

    // MenuClients.Activate();
    // MenuUsers.Activate();
    // MenuBackups.Activate();
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
    var client = daten.MandantNr == 0 ? M.AM005 : daten.MandantNr == 1 ? "" : $" ({M.AG110_title} {daten.MandantNr})";
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

    MenuClients.Visible = b && per >= (int)PermissionEnum.Admin;
    MenuUsers.Visible = b;
    MenuBackups.Visible = b;

    MenuLogin.Visible = !b;
    MenuLogout.Visible = b;
    MenuPwchange.Visible = b;
    MenuOptions.Visible = b;
    MenuDialogs.Visible = b;
    MenuReset.Visible = true;

    MenuDiary.Visible = b;
    MenuPositions.Visible = b;
    MenuNotes.Visible = b;
    MenuPersons.Visible = b;
    MenuMileages.Visible = b;
    MenuBikes.Visible = b;
    MenuBooks.Visible = b;
    MenuAuthors.Visible = b;
    MenuSeries.Visible = b;
    MenuStatistics.Visible = b;
    MenuSudoku.Visible = b;

    MenuBookings.Visible = b;
    MenuEvents.Visible = b;
    MenuAccounts.Visible = b;
    MenuPeriods.Visible = b;
    MenuFinalbalance.Visible = b;
    MenuPlbalance.Visible = b;
    MenuOpeningbalance.Visible = b;

    MenuAncestors.Visible = b;
    MenuFamilies.Visible = b;
    MenuSources.Visible = b;

    MenuStocks.Visible = b;
    MenuConfigurations.Visible = b;
    MenuChart.Visible = b;
    MenuInvestments.Visible = b;
    MenuBookings3.Visible = b;
    MenuPrices.Visible = b;
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
  /// <param name="widget">Betroffener Dialog.</param>
  /// <param name="label">Betroffene Bezeichnung.</param>
  public void AppendPage(CsbpBin widget, string label)
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

  /// <summary>Menu Rückgängig.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuUndo(object sender, EventArgs e)
  {
    MainClass.Undo();
  }

  /// <summary>Menu Wiederherstellen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuRedo(object sender, EventArgs e)
  {
    MainClass.Redo();
  }

  /// <summary>Menu Mandanten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuClients(object sender, EventArgs e)
  {
    AppendPage(AG100Clients.Create(), AG100_title);
  }

  /// <summary>Menu Benutzer.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuUsers(object sender, EventArgs e)
  {
    AppendPage(AG200Users.Create(), AG200_title);
  }

  /// <summary>Menu Sicherungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBackups(object sender, EventArgs e)
  {
    AppendPage(AG400Backups.Create(), AG400_title);
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
  }

  /// <summary>Menu Abmeldung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuLogout(object sender, EventArgs e)
  {
    ClosePages();
    MainClass.Logout();
  }

  /// <summary>Menu Kennwort-Änderung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPwchange(object sender, EventArgs e)
  {
    CsbpBin.Start(typeof(AM100Change), AM100_title, modal: true);
  }

  /// <summary>Menu Einstellungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuOptions(object sender, EventArgs e)
  {
    CsbpBin.Start(typeof(AM500Options), AM500_title, modal: true);
  }

  /// <summary>Menu Start-Formulare.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuDialogs(object sender, EventArgs e)
  {
    CsbpBin.Start(typeof(AM510Dialogs), AM510_title, modal: true);
  }

  /// <summary>Menu Dialoge zurücksetzen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuReset(object sender, EventArgs e)
  {
    Parameter.ResetDialogSizes();
  }

  /// <summary>Menu Tagebuch.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuDiary(object sender, EventArgs e)
  {
    AppendPage(TB100Diary.Create(), TB100_title);
  }

  /// <summary>Menu Positionen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPositions(object sender, EventArgs e)
  {
    AppendPage(TB200Positions.Create(), TB200_title);
  }

  /// <summary>Menu Notizen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuNotes(object sender, EventArgs e)
  {
    AppendPage(FZ700Memos.Create(), FZ700_title);
  }

  /// <summary>Menu Personen/Adressen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPersons(object sender, EventArgs e)
  {
    AppendPage(AD100Persons.Create(), AD100_title);
  }

  /// <summary>Menu Fahrradstände.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuMileages(object sender, EventArgs e)
  {
    AppendPage(FZ250Mileages.Create(), FZ250_title);
  }

  /// <summary>Menu Fahrraäder.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBikes(object sender, EventArgs e)
  {
    AppendPage(FZ200Bikes.Create(), FZ200_title);
  }

  /// <summary>Menu Bücher.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBooks(object sender, EventArgs e)
  {
    AppendPage(FZ340Books.Create(), FZ340_title);
  }

  /// <summary>Menu Autoren.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAuthors(object sender, EventArgs e)
  {
    AppendPage(FZ300Authors.Create(), FZ300_title);
  }

  /// <summary>Menu Serien.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuSeries(object sender, EventArgs e)
  {
    AppendPage(FZ320Series.Create(), FZ320_title);
  }

  /// <summary>Menu Statistik.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuStatistics(object sender, EventArgs e)
  {
    AppendPage(FZ100Statistics.Create(), FZ100_title);
  }

  /// <summary>Menu Sudoku.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuSudoku(object sender, EventArgs e)
  {
    AppendPage(SO100Sudoku.Create(), SO100_title);
  }

  /// <summary>Menu Buchungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBookings(object sender, EventArgs e)
  {
    AppendPage(HH400Bookings.Create(), HH400_title);
  }

  /// <summary>Menu Ereignisse.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuEvents(object sender, EventArgs e)
  {
    AppendPage(HH300Events.Create(), HH300_title);
  }

  /// <summary>Menu Konten.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAccounts(object sender, EventArgs e)
  {
    AppendPage(HH200Accounts.Create(), HH200_title);
  }

  /// <summary>Menu Perioden.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPeriods(object sender, EventArgs e)
  {
    AppendPage(HH100Periods.Create(), HH100_title);
  }

  /// <summary>Menu Schlussbilanz.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuFinalbalance(object sender, EventArgs e)
  {
    AppendPage(HH500Balance.Create(Constants.KZBI_SCHLUSS), HH500_title_SB);
  }

  /// <summary>Menu Gewinn+Verlust-Rechnung.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPlbalance(object sender, EventArgs e)
  {
    AppendPage(HH500Balance.Create(Constants.KZBI_GV), HH500_title_GV);
  }

  /// <summary>Menu Eröffnungsbilanz.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuOpeningbalance(object sender, EventArgs e)
  {
    AppendPage(HH500Balance.Create(Constants.KZBI_EROEFFNUNG), HH500_title_EB);
  }

  /// <summary>Menu Ahnen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAncestors(object sender, EventArgs e)
  {
    AppendPage(SB200Ancestors.Create(), SB200_title);
  }

  /// <summary>Menu Familien.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuFamilies(object sender, EventArgs e)
  {
    AppendPage(SB300Families.Create(), SB300_title);
  }

  /// <summary>Menu Quellen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuSources(object sender, EventArgs e)
  {
    AppendPage(SB400Sources.Create(), SB400_title);
  }

  /// <summary>Menu Wertpapiere.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuStocks(object sender, EventArgs e)
  {
    AppendPage(WP200Stocks.Create(), WP200_title);
  }

  /// <summary>Menu Konfigurationen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuConfigurations(object sender, EventArgs e)
  {
    AppendPage(WP300Configurations.Create(), WP300_title);
  }

  /// <summary>Menu Chart.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuChart(object sender, EventArgs e)
  {
    AppendPage(WP100Chart.Create(), WP100_title);
  }

  /// <summary>Menu Anlagen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuInvestments(object sender, EventArgs e)
  {
    AppendPage(WP250Investments.Create(), WP250_title);
  }

  /// <summary>Menu Buchungen.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuBookings3(object sender, EventArgs e)
  {
    AppendPage(WP400Bookings.Create(), WP400_title);
  }

  /// <summary>Menu Stände.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuPrices(object sender, EventArgs e)
  {
    AppendPage(WP500Prices.Create(), WP500_title);
  }

  /// <summary>Menu Über.</summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  protected void OnMenuAbout(object sender, EventArgs e)
  {
    var daten = MainClass.ServiceDaten;
    var ver = Assembly.GetEntryAssembly()?.GetName().Version.ToString() ?? "1.1";
    var db = Parameter.Connect;
    using var about = new AboutDialog
    {
      Title = "", // Titel geht nicht.
      ProgramName = "CSharp Budget Program",
      Version = ver + ", Runtime " + GetNetCoreVersion(),
      Copyright = "(c) 2019-2022 Wolfgang Kuehl",
      Comments = $@"CSBP is a simple budget program.
Database: {db}
Client: {daten.MandantNr} User: {daten.BenutzerId}",
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
