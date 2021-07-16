// <copyright file="MainWindow.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms
{
  using System;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Resources;
  using CSBP.Services.Factory;
  using static CSBP.Resources.Messages;
  using Gtk;
  using System.Collections.Generic;
  using CSBP.Forms.AM;
  using CSBP.Forms.AG;
  using System.Reactive.Linq;
  using System.Drawing;
  using CSBP.Forms.TB;
  using CSBP.Forms.FZ;
  using CSBP.Forms.AD;
  using CSBP.Forms.HH;
  using CSBP.Forms.WP;
  using CSBP.Forms.SB;
  using System.Reflection;

  /// <summary>Main Window.</summary>
  public class MainWindow : Window
  {
#pragma warning disable 649

    /// <summary>Status-Leiste.</summary>
    [Builder.Object]
    private Notebook Notebook;

    /// <summary>Status-Leiste.</summary>
    [Builder.Object]
    private Statusbar Statusbar;

    /// <summary>Menüpunkt Mandanten.</summary>
    [Builder.Object]
    private MenuItem MenuClients;

    /// <summary>Menüpunkt Benutzer.</summary>
    [Builder.Object]
    private MenuItem MenuUsers;

    /// <summary>Menüpunkt Sicherungen.</summary>
    [Builder.Object]
    private MenuItem MenuBackups;

    /// <summary>Menüpunkt Anmelden.</summary>
    [Builder.Object]
    private MenuItem MenuLogin;

    /// <summary>Menüpunkt Abmelden.</summary>
    [Builder.Object]
    private MenuItem MenuLogout;

    /// <summary>Menüpunkt Kennwort ändern.</summary>
    [Builder.Object]
    private MenuItem MenuPwchange;

    /// <summary>Menüpunkt Einstellungen.</summary>
    [Builder.Object]
    private MenuItem MenuOptions;

    /// <summary>Menüpunkt Start-Formulare.</summary>
    [Builder.Object]
    private MenuItem MenuDialogs;

    /// <summary>Menüpunkt Zurücksetzen.</summary>
    [Builder.Object]
    private MenuItem MenuReset;

    /// <summary>Menüpunkt Tagebuch.</summary>
    [Builder.Object]
    private MenuItem MenuDiary;

    /// <summary>Menüpunkt Notizen.</summary>
    [Builder.Object]
    private MenuItem MenuNotes;

    /// <summary>Menüpunkt Personen/Adressen.</summary>
    [Builder.Object]
    private MenuItem MenuPersons;

    /// <summary>Menüpunkt Fahrradstände.</summary>
    [Builder.Object]
    private MenuItem MenuMileages;

    /// <summary>Menüpunkt Fahrräder.</summary>
    [Builder.Object]
    private MenuItem MenuBikes;

    /// <summary>Menüpunkt Bücher.</summary>
    [Builder.Object]
    private MenuItem MenuBooks;

    /// <summary>Menüpunkt Autoren.</summary>
    [Builder.Object]
    private MenuItem MenuAuthors;

    /// <summary>Menüpunkt Serien.</summary>
    [Builder.Object]
    private MenuItem MenuSeries;

    /// <summary>Menüpunkt Statistik.</summary>
    [Builder.Object]
    private MenuItem MenuStatistics;

    /// <summary>Menüpunkt Sudoku.</summary>
    [Builder.Object]
    private MenuItem MenuSudoku;

    /// <summary>Menüpunkt Detektiv.</summary>
    [Builder.Object]
    private MenuItem MenuDetective;

    /// <summary>Menüpunkt Buchungen.</summary>
    [Builder.Object]
    private MenuItem MenuBookings;

    /// <summary>Menüpunkt Ereignisse.</summary>
    [Builder.Object]
    private MenuItem MenuEvents;

    /// <summary>Menüpunkt Konten.</summary>
    [Builder.Object]
    private MenuItem MenuAccounts;

    /// <summary>Menüpunkt Perioden.</summary>
    [Builder.Object]
    private MenuItem MenuPeriods;

    /// <summary>Menüpunkt Schlussbilanz.</summary>
    [Builder.Object]
    private MenuItem MenuFinalbalance;

    /// <summary>Menüpunkt Gewinn+Verlust-Rechnung.</summary>
    [Builder.Object]
    private MenuItem MenuPlbalance;

    /// <summary>Menüpunkt Eröffnungsbilanz.</summary>
    [Builder.Object]
    private MenuItem MenuOpeningbalance;

    /// <summary>Menüpunkt Ahnen.</summary>
    [Builder.Object]
    private MenuItem MenuAncestors;

    /// <summary>Menüpunkt Familien.</summary>
    [Builder.Object]
    private MenuItem MenuFamilies;

    /// <summary>Menüpunkt Quellen.</summary>
    [Builder.Object]
    private MenuItem MenuSources;

    /// <summary>Menüpunkt Wertpapiere.</summary>
    [Builder.Object]
    private MenuItem MenuStocks;

    /// <summary>Menüpunkt Konfigurationen.</summary>
    [Builder.Object]
    private MenuItem MenuConfigurations;

    /// <summary>Menüpunkt Chart.</summary>
    [Builder.Object]
    private MenuItem MenuChart;

    /// <summary>Menüpunkt Anlagen.</summary>
    [Builder.Object]
    private MenuItem MenuInvestments;

    /// <summary>Menüpunkt Buchungen.</summary>
    [Builder.Object]
    private MenuItem MenuBookings3;

    /// <summary>Menüpunkt Stände.</summary>
    [Builder.Object]
    private MenuItem MenuPrices;

#pragma warning restore 649

    /// <summary>Default Shared Constructor.</summary>
    /// <returns>A MainWindow.</returns>
    public static MainWindow Create()
    {
      var builder = new Builder("CSBP.GtkGui.MainWindow.glade");
      return new MainWindow(builder, builder.GetObject("MainWindow").Handle);
    }

    /// <summary>Konstruktor für Hauptfenster.</summary>
    /// <param name="builder">Betroffener Builder.</param>
    /// <param name="handle">Betroffenes Handle.</param>
    protected MainWindow(Builder builder, IntPtr handle) : base(handle)
    {
      builder.Autoconnect(this);
      var l = GetChildren();
      l.ForEach(c =>
      {
        var mi = c as MenuItem;
        if (mi != null && !string.IsNullOrEmpty(mi.Label) && mi.Label.Contains(".", StringComparison.CurrentCulture))
        {
          mi.Label = Messages.Get(mi.Label);
        }
        var imi = c as ImageMenuItem;
        if (imi != null && !string.IsNullOrEmpty(imi.Label) && imi.Label.StartsWith("gtk-", StringComparison.CurrentCulture))
        {
          var m = Messages.Get(imi.Label.Replace("gtk-", "Menu."));
          if (!string.IsNullOrEmpty(m))
            imi.Label = m;
        }
      });
      SetIconFromFile("Resources/Icons/WKHH.gif");
      SetupHandlers();
      var size = Parameter.GetDialogSize(typeof(MainWindow));
      // WidthRequest = size.Width;
      // HeightRequest = size.Height;
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
          SizeAllocatedHandler h = (sender, e) => { h0(e); };
          return h;
        },
        h => SizeAllocated += h, h => SizeAllocated -= h
      ).Throttle(TimeSpan.FromMilliseconds(1000));
      ob.Subscribe(e =>
      {
        Application.Invoke(delegate
        {
          if (Window != null && Visible)
          {
            SetSizeRequest(0, 0);
            Window.GetGeometry(out int x0, out int y0, out int w, out int h);
            Window.GetOrigin(out int x, out int y);
            // Höhe der Titelleiste abziehen
            Parameter.SetDialogSize(typeof(MainWindow), new Rectangle(x, y - CsbpBin.TitleHeight, w, h));
            //Console.WriteLine($"{x} {y} {w} {h}");
            //Console.WriteLine($"{DateTime.Now}");
          }
        });
      });
    }

    /// <summary>Hauptfenster wieder zurücksetzen.</summary>
    public void Reset()
    {
      Application.Invoke(delegate
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
    /// Liefert alle Kindelemente eines Containers als Liste.
    /// </summary>
    /// <param name="con">Betroffener Container.</param>
    /// <param name="l">Falls nicht null, wird diese Liste ergänzt.</param>
    /// <returns>Liste mit Kindelementen.</returns>
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
        if (c is MenuItem && (c as MenuItem).Submenu is Menu)
          GetChildren((c as MenuItem).Submenu as Container, l);
      }
      return l;
    }

    /// <summary>Starten der Anmeldung und der Tabs.</summary>
    /// <param name="p">Betroffenes Main Window.</param>
    public void Start(Gtk.Window p)
    {
      Notebook.RemovePage(0);
      MainClass.InitDb(new ServiceDaten(0, "Admin"));
#if DEBUG
      var username = Environment.UserName;
      MainClass.Login(new ServiceDaten(1, username.ToFirstUpper())); // Automatische Anmeldung mit aktuellem Benutzer.
      // MainClass.Login(new ServiceDaten(3, "Wolfgang"));
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
      // MenuNotes.Activate();
      // MenuPersons.Activate();
      // MenuMileages.Activate();
      // MenuBikes.Activate();
      // MenuBooks.Activate();
      // MenuAuthors.Activate();
      // MenuSeries.Activate();
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

    /// <summary>
    /// Aktualisieren des Anwendungstitels.
    /// </summary>
    public void RefreshTitle()
    {
      var test = Parameter.GetValue(Parameter.AG_TEST_PRODUKTION) == "TEST" ? "Test-" : "";
      Title = $"{test}CSBP {Parameter.GetValue(Parameter.AG_ANWENDUNGS_TITEL)} W. Kuehl";
    }

    /// <summary>
    /// Setzen der Berechtigung für die Anwendung.
    /// </summary>
    /// <param name="b">Ist die Anmeldung erfolgt?</param>
    public void SetPermission(bool b = false)
    {
      RefreshTitle();

      MenuClients.Visible = b;
      MenuUsers.Visible = b;
      MenuBackups.Visible = b;

      MenuLogin.Visible = !b;
      MenuLogout.Visible = b;
      MenuPwchange.Visible = b;
      MenuOptions.Visible = b;
      MenuDialogs.Visible = b;
      MenuReset.Visible = true;

      MenuDiary.Visible = b;
      MenuNotes.Visible = b;
      MenuPersons.Visible = b;
      MenuMileages.Visible = b;
      MenuBikes.Visible = b;
      MenuBooks.Visible = b;
      MenuAuthors.Visible = b;
      MenuSeries.Visible = b;
      MenuStatistics.Visible = b;
      MenuSudoku.Visible = b;
      MenuDetective.Visible = b;

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
        // Statusbar.Remove(contextid, 2);
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
      var title = label.Replace("_", "").Replace("&", "&amp;");
      if (Functions.MachNichts() == 0)
      {
        var closeImage = Image.NewFromIconName("window-close", IconSize.Button);
        var button = new Button();
        var lbl = new Label(title);
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
          Markup = $"<span size='small'>{title}</span> <span color='red' size='large'>x</span>"
        };
        var eb = new EventBox();
        eb.Add(l);
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
        var p = Notebook.GetNthPage(0) as CsbpBin;
        if (p == null)
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
    /// Fokussieren bzw.async Wechseln zu einer Seite.
    /// </summary>
    /// <typeparam name="T">Betroffener Dialog-Type.</typeparam>
    /// <returns>Betroffene Dialog-Instanz.</returns>
    public T FocusPage<T>() where T : Widget
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

    #region Handlers

    /// <summary>Initialisierung der Events.</summary>
    protected void SetupHandlers()
    {
      DeleteEvent += OnDeleteEvent;
    }

    /// <summary>Menüpunkt Rückgängig.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuUndo(object sender, EventArgs e)
    {
      MainClass.Undo();
    }

    /// <summary>Menüpunkt Wiederherstellen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuRedo(object sender, EventArgs e)
    {
      MainClass.Redo();
    }

    /// <summary>Menüpunkt Mandanten.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuClients(object sender, EventArgs e)
    {
      AppendPage(AG100Clients.Create(), AG100_title);
    }

    /// <summary>Menüpunkt Benutzer.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuUsers(object sender, EventArgs e)
    {
      AppendPage(AG200Users.Create(), AG200_title);
    }

    /// <summary>Menüpunkt Sicherungen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuBackups(object sender, EventArgs e)
    {
      AppendPage(AG400Backups.Create(), AG400_title);
    }

    /// <summary>Menüpunkt Schließen der Anwendung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuQuit(object sender, EventArgs e)
    {
      MainClass.Quit();
    }

    /// <summary>Menüpunkt Anmeldung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuLogin(object sender, EventArgs e)
    {
      //AppendPage(AM000Login.Create(), AM000_title);
      CsbpBin.Start(typeof(AM000Login), AM000_title, modal: true);
    }

    /// <summary>Menüpunkt Abmeldung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuLogout(object sender, EventArgs e)
    {
      ClosePages();
      MainClass.Logout();
    }

    /// <summary>Menüpunkt Kennwort-Änderung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuPwchange(object sender, EventArgs e)
    {
      CsbpBin.Start(typeof(AM100Change), AM100_title, modal: true);
    }

    /// <summary>Menüpunkt Einstellungen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuOptions(object sender, EventArgs e)
    {
      CsbpBin.Start(typeof(AM500Options), AM500_title, modal: true);
    }

    /// <summary>Menüpunkt Start-Formulare.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuDialogs(object sender, EventArgs e)
    {
      CsbpBin.Start(typeof(AM510Dialogs), AM510_title, modal: true);
    }

    /// <summary>Menüpunkt Dialoge zurücksetzen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuReset(object sender, EventArgs e)
    {
      Parameter.ResetDialogSizes();
    }

    /// <summary>Menüpunkt Tagebuch.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuDiary(object sender, EventArgs e)
    {
      AppendPage(TB100Diary.Create(), TB100_title);
    }

    /// <summary>Menüpunkt Notizen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuNotes(object sender, EventArgs e)
    {
      AppendPage(FZ700Memos.Create(), FZ700_title);
    }

    /// <summary>Menüpunkt Personen/Adressen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuPersons(object sender, EventArgs e)
    {
      AppendPage(AD100Persons.Create(), AD100_title);
    }

    /// <summary>Menüpunkt Fahrradstände.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuMileages(object sender, EventArgs e)
    {
      AppendPage(FZ250Mileages.Create(), FZ250_title);
    }

    /// <summary>Menüpunkt Fahrraäder.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuBikes(object sender, EventArgs e)
    {
      AppendPage(FZ200Bikes.Create(), FZ200_title);
    }

    /// <summary>Menüpunkt Bücher.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuBooks(object sender, EventArgs e)
    {
      AppendPage(FZ340Books.Create(), FZ340_title);
    }

    /// <summary>Menüpunkt Autoren.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuAuthors(object sender, EventArgs e)
    {
      AppendPage(FZ300Authors.Create(), FZ300_title);
    }

    /// <summary>Menüpunkt Serien.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuSeries(object sender, EventArgs e)
    {
      AppendPage(FZ320Series.Create(), FZ320_title);
    }

    /// <summary>Menüpunkt Statistik.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuStatistics(object sender, EventArgs e)
    {
      AppendPage(FZ100Statistics.Create(), FZ100_title);
    }

    /// <summary>Menüpunkt Sudoku.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuSudoku(object sender, EventArgs e)
    {
      // TODO AppendPage(SO100Sudoku.Create(), SO100_title);
    }

    /// <summary>Menüpunkt Detektiv.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuDetective(object sender, EventArgs e)
    {
      // TODO AppendPage(SO200Detektiv.Create(), SO200_title);
    }

    /// <summary>Menüpunkt Buchungen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuBookings(object sender, EventArgs e)
    {
      AppendPage(HH400Bookings.Create(), HH400_title);
    }

    /// <summary>Menüpunkt Ereignisse.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuEvents(object sender, EventArgs e)
    {
      AppendPage(HH300Events.Create(), HH300_title);
    }

    /// <summary>Menüpunkt Konten.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuAccounts(object sender, EventArgs e)
    {
      AppendPage(HH200Accounts.Create(), HH200_title);
    }

    /// <summary>Menüpunkt Perioden.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuPeriods(object sender, EventArgs e)
    {
      AppendPage(HH100Periods.Create(), HH100_title);
    }

    /// <summary>Menüpunkt Schlussbilanz.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuFinalbalance(object sender, EventArgs e)
    {
      AppendPage(HH500Balance.Create(Constants.KZBI_SCHLUSS), HH500_title_SB);
    }

    /// <summary>Menüpunkt Gewinn+Verlust-Rechnung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuPlbalance(object sender, EventArgs e)
    {
      AppendPage(HH500Balance.Create(Constants.KZBI_GV), HH500_title_GV);
    }

    /// <summary>Menüpunkt Eröffnungsbilanz.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuOpeningbalance(object sender, EventArgs e)
    {
      AppendPage(HH500Balance.Create(Constants.KZBI_EROEFFNUNG), HH500_title_EB);
    }

    /// <summary>Menüpunkt Ahnen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuAncestors(object sender, EventArgs e)
    {
      AppendPage(SB200Ancestors.Create(), SB200_title);
    }

    /// <summary>Menüpunkt Familien.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuFamilies(object sender, EventArgs e)
    {
      AppendPage(SB300Families.Create(), SB300_title);
    }

    /// <summary>Menüpunkt Quellen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuSources(object sender, EventArgs e)
    {
      AppendPage(SB400Sources.Create(), SB400_title);
    }

    /// <summary>Menüpunkt Wertpapiere.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuStocks(object sender, EventArgs e)
    {
      AppendPage(WP200Stocks.Create(), WP200_title);
    }

    /// <summary>Menüpunkt Konfigurationen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuConfigurations(object sender, EventArgs e)
    {
      AppendPage(WP300Configurations.Create(), WP300_title);
    }

    /// <summary>Menüpunkt Chart.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuChart(object sender, EventArgs e)
    {
      AppendPage(WP100Chart.Create(), WP100_title);
    }

    /// <summary>Menüpunkt Anlagen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuInvestments(object sender, EventArgs e)
    {
      AppendPage(WP250Investments.Create(), WP250_title);
    }

    /// <summary>Menüpunkt Buchungen.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuBookings3(object sender, EventArgs e)
    {
      AppendPage(WP400Bookings.Create(), WP400_title);
    }

    /// <summary>Menüpunkt Stände.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuPrices(object sender, EventArgs e)
    {
      AppendPage(WP500Prices.Create(), WP500_title);
    }

    /// <summary>
    /// Liefert die .Net Version.
    /// </summary>
    /// <returns>.Net Version als String.</returns>
    public static string GetNetCoreVersion()
    {
      var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
      var assemblyPath = assembly.Location.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
      int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
      if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
        return assemblyPath[netCoreAppIndex + 1];
      return null;
    }

    /// <summary>Menüpunkt Über.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuAbout(object sender, EventArgs e)
    {
      var daten = MainClass.ServiceDaten;
      using (var about = new AboutDialog
      {
        Title = "", // Titel geht nicht.
        ProgramName = "CSharp Budget Program",
        Version = "1.0, Runtime " + GetNetCoreVersion(),
        Copyright = "(c) 2019-2021 Wolfgang Kuehl",
        Comments = $@"CSBP is a simple budget program.
Client: {daten.MandantNr}
User: {daten.BenutzerId}",
        Website = "https://cwkuehl.de",
        Logo = new Gdk.Pixbuf("Resources/Icons/WKHH.gif")
      })
      {
        about.Run();
        about.Hide();
      }
    }

    /// <summary>Menüpunkt Hilfe.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnMenuHelp2(object sender, EventArgs e)
    {
      var fn = Parameter.GetValue(Parameter.AG_HILFE_DATEI);
      if (!string.IsNullOrEmpty(fn))
        UiTools.StartFile(fn);
    }

    /// <summary>Schließen des Fensters und der Anwendung.</summary>
    /// <param name="sender">Betroffener Sender.</param>
    /// <param name="e">Betroffenes Ereignis.</param>
    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
      MainClass.Quit();
      a.RetVal = true;
    }

    #endregion
  }
}