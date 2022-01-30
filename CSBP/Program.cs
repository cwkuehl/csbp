// <copyright file="Program.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP
{
  using System;
  using System.Text.RegularExpressions;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Forms;
  using CSBP.Forms.AD;
  using CSBP.Services.Factory;
  using CSBP.Services.Repositories.Base;
  using Gtk;
  using static CSBP.Resources.Messages;

  /// <summary>
  /// Hauptklasse des Projekts CSBP.
  /// </summary>
  class MainClass
  {
    public static MainWindow MainWindow { get; private set; }
    public static ServiceDaten ServiceDaten0 = new ServiceDaten(0, Constants.USER_ID);
    public static ServiceDaten ServiceDaten
    {
      get { return new ServiceDaten(serviceDaten.MandantNr, serviceDaten.BenutzerId); }
      private set
      {
        serviceDaten = value;
      }
    }

    static ServiceDaten serviceDaten = ServiceDaten0;

    [STAThread]
    static void Main(string[] args)
    {
      // Exception-Behandlung
      AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
      GLib.ExceptionManager.UnhandledException += UnhandledExceptionGlib;
      // Sprache initialisieren
      var ci = new System.Globalization.CultureInfo("de-DE");
      ////var ci = new System.Globalization.CultureInfo("en-US");
      Functions.SetCultureInfo(ci);
      if (args != null && args.Length > 0)
      {
        var re = new Regex("^" + Regex.Escape(Parameter.DB_DRIVER_CONNECT) + "=(.+)$", RegexOptions.Compiled);
        foreach (var arg in args)
        {
          var m = re.Match(arg);
          if (m.Success)
          {
            CsbpContext.DbDriverConnect = m.Groups[1].Value;
            Parameter.Connect = m.Groups[1].Value;
          }
        }
      }
      Application.Init();
      ApplyTheme();

      MainWindow = MainWindow.Create();
      MainWindow.Show();
      MainWindow.SetPermission();
      MainWindow.Start(MainWindow);
      if (Functions.MachNichts() != 0)
        TestCode();
      Application.Run();
    }

    static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
      Console.WriteLine(e.ExceptionObject.ToString());
      if (MainWindow != null)
        MainWindow.SetError(e.ExceptionObject.ToString());
      //Environment.Exit(1);
    }

    static void UnhandledExceptionGlib(GLib.UnhandledExceptionArgs e)
    {
      var ex = (e.ExceptionObject as Exception)?.InnerException;
      var s = ex is MessageException ? ex.Message : e.ExceptionObject.ToString();
      Application.Invoke(delegate
      {
        MainWindow.SetError(s);
        var md = new MessageDialog(MainWindow, DialogFlags.DestroyWithParent,
            MessageType.Error, ButtonsType.Close, false, s);
        md.Run();
        md.Dispose();
      });
    }

    /// <summary>Schließen der Anwendung mit Schließen aller Tabs und Speichern der Parameter.</summary>
    public static void Quit()
    {
      MainWindow.ClosePages();
      Parameter.Save();
      Application.Quit();
    }

    public static void InitDb(ServiceDaten daten)
    {
      if (daten == null)
        return;
      Get(FactoryService.ClientService.InitDb(daten));
    }

    public static void Login(ServiceDaten daten)
    {
      if (daten != null)
      {
        ServiceDaten = daten;
        Get(FactoryService.ClientService.GetOptionList(daten, daten.MandantNr, Parameter.Params));
        MainWindow.SetPermission(true);

        if (Functions.MachNichts() == 0)
        {
          // Start-Dialoge starten
          var sd = Parameter.GetValue(Parameter.AG_STARTDIALOGE) ?? "";
          var arr = sd.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
          foreach (var d in arr)
          {
            if (StartDialog.Dialoge.TryGetValue(d.Substring(1), out var sf))
            {
              var parr = d.Substring(1).Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
              var parm = parr.Length > 1 ? parr[1] : null;
              var create = sf.Type.GetMethod("Create");
              var dlg = create.Invoke(null, new object[] { parm, null }) as CsbpBin;
              MainWindow.AppendPage(dlg, sf.Title);
            }
          }
          if (Parameter.AD120Start)
          {
            // Geburtstagsliste starten
            CsbpBin.Start(typeof(AD120Birthdays), AD120_title);
          }
        }
      }
    }

    public static void Logout()
    {
      Get(FactoryService.LoginService.Logout(ServiceDaten));
      ServiceDaten = ServiceDaten0;
      MainWindow.SetPermission();
    }

    public static T Get<T>(ServiceErgebnis<T> r, bool dialog = true)
    {
      if (r == null)
        return default(T);
      var s = r.GetErrors();
      ShowError(s, dialog);
      return r.Ergebnis;
    }

    public static bool Get(ServiceErgebnis r, bool dialog = true)
    {
      if (r == null)
        return false;
      var s = r.GetErrors();
      ShowError(s, dialog);
      return r.Ok;
    }

    public static void SetStatus(string s)
    {
      MainWindow.SetError(s);
    }

    public static void ShowInfo(string s, bool dialog = true)
    {
      MainWindow.SetError(s);
      if (dialog && !string.IsNullOrWhiteSpace(s))
      {
        s = s.Replace("{", @"{{").Replace("}", @"}}");
        var md = new MessageDialog(MainWindow, DialogFlags.DestroyWithParent,
            MessageType.Info, ButtonsType.Close, false, s);
        md.Run();
        md.Dispose();
      }
    }

    public static void ShowError(string s, bool dialog = true)
    {
      MainWindow.SetError(s);
      if (dialog && !string.IsNullOrWhiteSpace(s))
      {
        s = s.Replace("{", @"{{").Replace("}", @"}}");
        var md = new MessageDialog(MainWindow, DialogFlags.DestroyWithParent,
            MessageType.Error, ButtonsType.Close, false, s);
        md.Run();
        md.Dispose();
      }
    }

    /// <summary>
    /// Undo last transaction.
    /// </summary>
    /// <returns>Is anything changed?</returns>
    internal static bool Undo()
    {
      return Get(FactoryService.LoginService.Undo(MainClass.ServiceDaten));
    }

    /// <summary>
    /// Redo last transaction.
    /// </summary>
    /// <returns>Is anything changed?</returns>
    internal static bool Redo()
    {
      return Get(FactoryService.LoginService.Redo(MainClass.ServiceDaten));
    }

    /// <summary>GTK-Thema anwenden.</summary>
    public static void ApplyTheme()
    {
      var theme = Parameter.GetValue(Parameter.APP_THEME);
      if (string.IsNullOrEmpty(theme) || theme.Contains("xxx") || theme == "default")
        return;
      try
      {
        // Load the Theme
        var css_provider = new Gtk.CssProvider();
        // Download von https://www.gnome-look.org/p/1275087/
        css_provider.LoadFromPath(theme); // "/opt/Haushalt/Themes/Mojave-dark/gtk-3.0/gtk.css");
        //css_provider.LoadFromResource("CSBP.GtkGui.Themes.MojaveDark.gtk3.gtk.css");
        Gtk.StyleContext.AddProviderForScreen(Gdk.Screen.Default, css_provider, 800);
      }
      catch (Exception)
      {
        Functions.MachNichts();
      }
    }

    /// <summary>Funktion mit Test-Code.</summary>
    public static void TestCode()
    {
      //Console.WriteLine("Hello World!");
      var assembly = typeof(MainWindow).Assembly;
      string[] resourceNames = assembly.GetManifestResourceNames();
      foreach (string resourceName in resourceNames)
      {
        System.Diagnostics.Trace.WriteLine(resourceName);
        // using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        // using (StreamReader reader = new StreamReader(stream))
        // {
        //   var result = reader.ReadToEnd();
        // }
      }
      var rm = new System.Resources.ResourceManager("CSBP.Resources.Messages", assembly);
      var title = rm.GetString("Main.title");
      // Task.Run(() =>
      // {
      //   Thread.Sleep(200);
      //   GLib.ExceptionManager.RaiseUnhandledException(new Exception("Hallo."), false);
      //   throw new Exception("Hallo!!!");
      // });
      // if (Functions.MachNichts() == 0)
      //   throw new Exception("2");
    }
  }
}
