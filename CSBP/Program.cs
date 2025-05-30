﻿// <copyright file="Program.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP;

using System;
using System.IO;
using System.Text.RegularExpressions;
using CSBP.Base;
using CSBP.Forms;
using CSBP.Forms.AD;
using CSBP.Services.Apis.Enums;
using CSBP.Services.Base;
using CSBP.Services.Factory;
using CSBP.Services.Repositories.Base;
using CSBP.Services.Resources;
using Gtk;
using static CSBP.Services.Resources.Messages;

/// <summary>
/// Hauptklasse des Projekts CSBP.
/// </summary>
#pragma warning disable SA1649
public class MainClass
#pragma warning restore SA1649
{
  /// <summary>Initial service data.</summary>
  private static readonly ServiceDaten ServiceDaten0 = new(UiTools.SessionId, 0, Constants.USER_ID, null);

  /// <summary>Actual service data.</summary>
  private static ServiceDaten intServiceData = ServiceDaten0;

  /// <summary>Gets the main window.</summary>
  public static Application App { get; private set; }

  /// <summary>Gets the main window.</summary>
  public static MainWindow MainWindow { get; private set; }

  /// <summary>Gets new copy of service data.</summary>
  public static ServiceDaten ServiceDaten
  {
    get { return new ServiceDaten(intServiceData.Daten); }
    private set { intServiceData = value; }
  }

  /// <summary>
  /// Main entry function.
  /// </summary>
  /// <param name="args">Affected command line arguments.</param>
  [STAThread]
  public static void Main(string[] args)
  {
    // Exception-Behandlung
    AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
    GLib.ExceptionManager.UnhandledException += UnhandledExceptionGlib;
    //// Sprache initialisieren
    var ci = new System.Globalization.CultureInfo("de-DE");
    //// var ci = new System.Globalization.CultureInfo("en-US");
    Functions.SetCultureInfo(ci);
    try
    {
      // Init GUI-Parameters.
      ParameterGui.Init();
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex);
      Application.Init();
      ShowError(ex.ToString());
      return;
    }
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

    // App = new Application("cwkuehl.de", GLib.ApplicationFlags.None);
    MainWindow = MainWindow.Create(App);
    App?.AddWindow(MainWindow);
    MainWindow.Show();
    MainWindow.SetPermission();
    MainWindow.Start(MainWindow);
    if (Functions.MachNichts() != 0)
      TestCode();
    Application.Run();
  }

  /// <summary>Start help file in Browser.</summary>
  /// <param name="form">Affected form name.</param>
  public static void Help(string form = null)
  {
    var fn = ParameterGui.HelpFile;
    if (!string.IsNullOrEmpty(fn))
    {
      var d = MainWindow.GetActiveDialog();
      var f = form ?? d?.GetType().Name.Left(5);
      if (f != null && fn.EndsWith(".html", StringComparison.CurrentCultureIgnoreCase))
      {
        // Start help for a dialog.
        var fn0 = fn[..^5] + "0.html";
        var link = $"file://{fn}?#{f}";
        var html = $@"<!DOCTYPE html>
<html>
  <head>
    <meta http-equiv='refresh' content=""0; url='{link}'""/>
  </head>
  <body>
    <p>Please follow <a href='{link}'>{f}</a>.</p>
  </body>
</html>";
        File.WriteAllText(fn0, html);
        fn = fn0;
      }
      UiTools.StartFile(fn);
    }
  }

  /// <summary>Closes all tabs and the application and saves all parameters.</summary>
  public static void Quit()
  {
    if (MainWindow != null)
    {
      // MainWindow.Window.GetGeometry(out int _, out int _, out int w, out int h);
      MainWindow.GetSize(out int w, out int h); // better than MainWindow.Window.GetGeometry(out x, out y, out w, out h);
      //// MainWindow.Window.GetOrigin(out x, out y);
      //// Services.Base.ServiceBase.Log.Warn($"{typeof(MainWindow).Name} MainWindow.Window.GetOrigin x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
      //// MainWindow.Window.GetPosition(out x, out y);
      //// Services.Base.ServiceBase.Log.Warn($"{typeof(MainWindow).Name} MainWindow.Window.GetPosition x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
      MainWindow.GetPosition(out int x, out int y); // gets also values for x <= 37.
      //// Services.Base.ServiceBase.Log.Warn($"{typeof(MainWindow).Name} MainWindow.GetPosition x {x} y {y} {DateTime.Now:HH:mm:ss.fff}");
      ParameterGui.SetDialogSize(typeof(MainWindow), new System.Drawing.Rectangle(x, y, w, h), CsbpBin.TitleHeight);
    }
    MainWindow.ClosePages();
    ParameterGui.Save();
    Application.Quit();
  }

  /// <summary>
  /// Initialises the database.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  public static void InitDb(ServiceDaten daten)
  {
    if (daten == null)
      return;
    Get(FactoryService.ClientService.InitDb(daten));
  }

  /// <summary>
  /// Logs the user in.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  public static void Login(ServiceDaten daten)
  {
    if (daten != null)
    {
      ServiceDaten = daten;
      Get(FactoryService.ClientService.GetOptionList(daten, daten.MandantNr, Parameter.Params, null));
      var user = Get(FactoryService.ClientService.GetUser(daten, -1));
      var per = user == null ? (int)PermissionEnum.Without : user.Berechtigung;
      MainWindow.SetPermission(true, per);

      if (Functions.MachNichts() == 0)
      {
        // Start-Dialoge starten
#if DEBUG
        var sd = daten.MandantNr == 1 ? Parameter.GetValue(Parameter.AG_STARTDIALOGE) ?? "" : "";
        sd = "#FZ250"; // "#AG500|#WP500|#TB100";
        var focus = true;
#else
        var sd = Parameter.GetValue(Parameter.AG_STARTDIALOGE) ?? "";
        var focus = false;
#endif
        var arr = sd.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var d in arr)
        {
          if (StartDialog.Dialoge.TryGetValue(d[1..], out var sf))
          {
            var parr = d[1..].Split([";"], StringSplitOptions.RemoveEmptyEntries);
            var parm = parr.Length > 1 ? parr[1] : null;
            var create = sf.Type.GetMethod("Create");
            var dlg = create.Invoke(null, new object[] { parm, null }) as CsbpBin;
            MainWindow.AppendPage(dlg, sf.Title, focus);
          }
        }
        if (ParameterGui.AD120Start)
        {
          // Show birthday list.
          CsbpBin.Start(typeof(AD120Birthdays), AD120_title, parameter1: true);
        }
      }
    }
  }

  /// <summary>
  /// Logs the user out.
  /// </summary>
  public static void Logout()
  {
    Get(FactoryService.LoginService.Logout(ServiceDaten));
    ServiceDaten = ServiceDaten0;
    MainWindow.SetPermission();
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
    if (r == null)
      return default;
    var s = r.GetErrors();
    ShowError(s, dialog);
    return r.Ergebnis;
  }

  /// <summary>
  /// Extracts possible errors from service result.
  /// </summary>
  /// <param name="r">Affected service result.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
  /// <returns>Is it OK or not.</returns>
  public static bool Get(ServiceErgebnis r, bool dialog = true)
  {
    if (r == null)
      return false;
    var s = r.GetErrors();
    ShowError(s, dialog);
    return r.Ok;
  }

  /// <summary>
  /// Sets global error message.
  /// </summary>
  /// <param name="s">Affected error message or null.</param>
  public static void SetStatus(string s)
  {
    MainWindow.SetError(s);
  }

  /// <summary>
  /// Shows info message.
  /// </summary>
  /// <param name="s">Affected info message.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
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

  /// <summary>
  /// Shows error message.
  /// </summary>
  /// <param name="s">Affected error message.</param>
  /// <param name="dialog">Shows as message dialog or not.</param>
  public static void ShowError(string s, bool dialog = true)
  {
    MainWindow?.SetError(s);
    if (dialog && !string.IsNullOrWhiteSpace(s))
    {
      s = s.Replace("{", @"{{").Replace("}", @"}}");
      var md = new MessageDialog(MainWindow, DialogFlags.DestroyWithParent,
          MessageType.Error, ButtonsType.Close, false, s);
      md.Run();
      md.Dispose();
    }
  }

  /// <summary>GTK-Thema anwenden.</summary>
  public static void ApplyTheme()
  {
    var theme = ParameterGui.AppTheme;
    if (string.IsNullOrEmpty(theme) || theme.Contains("xxx") || theme == "default")
      return;
    try
    {
      // Load the Theme
      var css_provider = new Gtk.CssProvider();
      //// Download von https://www.gnome-look.org/p/1275087/
      css_provider.LoadFromPath(theme); // "/opt/Haushalt/Themes/Mojave-dark/gtk-3.0/gtk.css");
      //// css_provider.LoadFromResource("CSBP.GtkGui.Themes.MojaveDark.gtk3.gtk.css");
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
    // Console.WriteLine("Hello World!");
    var assembly = typeof(MainWindow).Assembly;
    string[] resourceNames = assembly.GetManifestResourceNames();
    foreach (string resourceName in resourceNames)
    {
      System.Diagnostics.Trace.WriteLine(resourceName);
      //// using (Stream stream = assembly.GetManifestResourceStream(resourceName))
      //// using (StreamReader reader = new StreamReader(stream))
      //// {
      ////   var result = reader.ReadToEnd();
      //// }
    }
    var rm = new System.Resources.ResourceManager("CSBP.Services.Resources.Messages", typeof(Messages).Assembly);
    _ = rm.GetString("Main.title");
    //// Task.Run(() =>
    //// {
    ////   Thread.Sleep(200);
    ////   GLib.ExceptionManager.RaiseUnhandledException(new Exception("Hallo."), false);
    ////   throw new Exception("Hallo!!!");
    //// });
    //// if (Functions.MachNichts() == 0)
    ////   throw new Exception("2");
  }

  /// <summary>
  /// Undo last transaction.
  /// </summary>
  /// <returns>Is anything changed or not.</returns>
  internal static bool Undo()
  {
    return Get(FactoryService.LoginService.Undo(MainClass.ServiceDaten));
  }

  /// <summary>
  /// Redo last transaction.
  /// </summary>
  /// <returns>Is anything changed or not.</returns>
  internal static bool Redo()
  {
    return Get(FactoryService.LoginService.Redo(MainClass.ServiceDaten));
  }

  /// <summary>
  /// Handles unhandled exceptions.
  /// </summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="e">Affected event.</param>
  private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
  {
    Console.WriteLine(e.ExceptionObject.ToString());
    MainWindow?.SetError(e.ExceptionObject.ToString());
    //// Environment.Exit(1);
  }

  /// <summary>
  /// Handles unhandled exceptions.
  /// </summary>
  /// <param name="e">Affected event.</param>
  private static void UnhandledExceptionGlib(GLib.UnhandledExceptionArgs e)
  {
    var ex = (e.ExceptionObject as Exception)?.InnerException;
    var s = ex is MessageException ? ex.Message : e.ExceptionObject.ToString();
    Application.Invoke((sender, e1) =>
    {
      MainWindow.SetError(s);
      var md = new MessageDialog(MainWindow, DialogFlags.DestroyWithParent,
          MessageType.Error, ButtonsType.Close, false, s);
      md.Run();
      md.Dispose();
    });
  }
}

/*

Create separate project for Services without UI:
dotnet new classlib -o CSBP.Services
dotnet add ./CSBP/CSBP.csproj reference ./CSBP.Services/CSBP.Services.csproj

*/
