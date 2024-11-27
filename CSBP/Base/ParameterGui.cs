// <copyright file="ParameterGui.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using CSBP.Services.Base;

/// <summary>
/// Manager of parameters which are stored in setting file or database.
/// </summary>
public class ParameterGui : Parameter
{
#pragma warning disable SA1310

  /// <summary>Constant key for application theme.</summary>
  private const string APP_THEME = "APP_THEME";

  /// <summary>Constant key for help file.</summary>
  private const string AG_HILFE_DATEI = "AG_HILFE_DATEI";

  /// <summary>Constant key for folder for temporary files.</summary>
  private const string AG_TEMP_PFAD = "AG_TEMP_PFAD";

#pragma warning restore SA1310

  /// <summary>Setting file name with path, e.g. .csbp.json.</summary>
  private static readonly string AppConfig;

  /// <summary>
  /// Initializes static members of the <see cref="ParameterGui"/> class.
  /// </summary>
  static ParameterGui()
  {
    AppConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".csbp.json");
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ParameterGui"/> class.
  /// </summary>
  /// <param name="key">Affected key.</param>
  /// <param name="default_">Affected default value.</param>
  /// <param name="trim">Affected trim value.</param>
  /// <param name="crypted">Affected crypted value.</param>
  /// <param name="setting">Affected setting value.</param>
  /// <param name="database">Affected database value.</param>
  /// <param name="client">Affected client value.</param>
  public ParameterGui(string key, string default_ = null, bool trim = true, bool crypted = false,
    string setting = null, bool database = false, bool client = false)
    : base(key, default_, trim, crypted, setting, database, client)
  {
    Functions.MachNichts();
  }

  /// <summary>Gets or sets the last login client.</summary>
  public static string LoginClient
  {
    get { return GetValue("LoginClient"); }
    set { SetValue("LoginClient", value); }
  }

  /// <summary>Gets or sets the last login user id.</summary>
  public static string LoginUser
  {
    get { return GetValue("LoginUser"); }
    set { SetValue("LoginUser", value); }
  }

  /// <summary>Gets or sets the Application theme.</summary>
  public static string AppTheme
  {
    get { return GetValue(APP_THEME); }
    set { SetValue(APP_THEME, value); }
  }

  /// <summary>Gets or sets the path to the help file.</summary>
  public static string HelpFile
  {
    get { return GetValue(AG_HILFE_DATEI); }
    set { SetValue(AG_HILFE_DATEI, value); }
  }

  /// <summary>Gets or sets the path to the temporary files.</summary>
  public static string TempPath
  {
    get { return GetValue(AG_TEMP_PFAD); }
    set { SetValue(AG_TEMP_PFAD, value); }
  }

  /// <summary>Gets or sets a value indicating whether the birthday list is opened after the login.</summary>
  public static bool AD120Start
  {
    get { return Functions.ToBool(GetValue("AD120Start")) ?? false; }
    set { SetValue("AD120Start", Functions.ToString(value)); }
  }

  /// <summary>Gets or sets the amount of days in the birthday list.</summary>
  public static int AD120Days
  {
    get { return Functions.ToInt32(GetValue("AD120Days") ?? "12"); }
    set { SetValue("AD120Days", Functions.ToString(value)); }
  }

  /// <summary>Gets or sets the address file name.</summary>
  public static string AD200File
  {
    get { return GetValue("AD200File"); }
    set { SetValue("AD200File", value); }
  }

  /// <summary>Gets or sets the period length.</summary>
  public static string HH100Length
  {
    get { return GetValue("HH100Length") ?? "1"; }
    set { SetValue("HH100Length", value); }
  }

  /// <summary>Gets or sets the position of a new period.</summary>
  public static string HH100When
  {
    get { return GetValue("HH100When") ?? "1"; }
    set { SetValue("HH100When", value); }
  }

  /// <summary>Gets or sets the title of the balances.</summary>
  public static string HH510Title
  {
    get { return GetValue("HH510Title") ?? "Bilanzen"; }
    set { SetValue("HH510Title", value); }
  }

  /// <summary>Gets or sets a value indicating whether the cash report is selected.</summary>
  public static bool HH510Cashreport
  {
    get { return Functions.ToBool(GetValue("HH510Cashreport")) ?? false; }
    set { SetValue("HH510Cashreport", Functions.ToString(value)); }
  }

  /// <summary>Gets or sets the booking file name.</summary>
  public static string HH510File
  {
    get { return GetValue("HH510File"); }
    set { SetValue("HH510File", value); }
  }

  /// <summary>Gets or sets the ancestor.</summary>
  public static string SB220Ancestor
  {
    get { return GetValue("SB220Ancestor"); }
    set { SetValue("SB220Ancestor", value); }
  }

  /// <summary>Gets or sets the number generations.</summary>
  public static string SB220Generation
  {
    get { return GetValue("SB220Generation") ?? "3"; }
    set { SetValue("SB220Generation", value); }
  }

  /// <summary>Gets or sets the pedigree name.</summary>
  public static string SB500Name
  {
    get { return GetValue("SB500Name"); }
    set { SetValue("SB500Name", value); }
  }

  /// <summary>Gets or sets the file name.</summary>
  public static string SB500File
  {
    get { return GetValue("SB500File"); }
    set { SetValue("SB500File", value); }
  }

  /// <summary>Gets or sets the filter criteria.</summary>
  public static string SB500Filter
  {
    get { return GetValue("SB500Filter"); }
    set { SetValue("SB500Filter", value); }
  }

  /// <summary>Gets or sets the sudoku field.</summary>
  public static string SO100Sudoku
  {
    get { return GetValue("SO100Sudoku"); }
    set { SetValue("SO100Sudoku", value); }
  }

  /// <summary>Gets or sets the stock configuration.</summary>
  public static string WP200Configuration
  {
    get { return GetValue("WP200Configuration"); }
    set { SetValue("WP200Configuration", value); }
  }

  /// <summary>Gets or sets the stock configuration.</summary>
  public static string WP220Configuration
  {
    get { return GetValue("WP220Configuration"); }
    set { SetValue("WP220Configuration", value); }
  }

  /// <summary>Gets or sets the stock.</summary>
  public static string WP220Stock
  {
    get { return GetValue("WP220Stock"); }
    set { SetValue("WP220Stock", value); }
  }

  /// <summary>Gets or sets the file name.</summary>
  public static string WP220File
  {
    get { return GetValue("WP220File"); }
    set { SetValue("WP220File", value); }
  }

  /// <summary>Gets or sets the file name 2.</summary>
  public static string WP220File2
  {
    get { return GetValue("WP220File2"); }
    set { SetValue("WP220File2", value); }
  }

  /// <summary>Gets or sets the stock configuration.</summary>
  public static string WP250Stock
  {
    get { return GetValue("WP250Stock"); }
    set { SetValue("WP250Stock", value); }
  }

  /// <summary>Gets or sets the investment.</summary>
  public static string WP400Investment
  {
    get { return GetValue("WP400Investment"); }
    set { SetValue("WP400Investment", value); }
  }

  /// <summary>Gets or sets the stock.</summary>
  public static string WP500Stock
  {
    get { return GetValue("WP500Stock"); }
    set { SetValue("WP500Stock", value); }
  }

  ////public static string Backups {
  ////    get {
  ////        var s = GetValue(AG_BACKUPS);
  ////        return s;
  ////    }
  ////    set {
  ////        SetValue(AG_BACKUPS, value);
  ////    }
  ////}

  /// <summary>
  /// Initializes static members of the <see cref="ParameterGui"/> class.
  /// </summary>
  public static void Init()
  {
    // Storage in setting file
    Params.Add(APP_THEME, new Parameter(APP_THEME, "default", setting: "AppTheme"));
    Params.Add(AG_HILFE_DATEI, new Parameter(AG_HILFE_DATEI, setting: "HelpFile"));
    Params.Add(AG_TEMP_PFAD, new Parameter(AG_TEMP_PFAD, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "temp"), setting: "TempPath"));
    ReadSettings();
  }

  /// <summary>
  /// Gets the stored dialog size.
  /// </summary>
  /// <param name="type">Affected dialog type.</param>
  /// <returns>Dialog size.</returns>
  public static Rectangle GetDialogSize(Type type)
  {
    var key = GetDialogKey(type);
    var v = GetValue(key) ?? "";
    var bytes = Convert.FromBase64String(v);
    int[] l = null;
    if (bytes.Length > 0)
    {
      try
      {
        l = Functions.Deserialize<int[]>(bytes);
      }
      catch (System.Exception)
      {
        Functions.MachNichts();
      }
    }
    l ??= new[] { -1, -1, 400, 300 };
    //// var s = $"{type.Name} Old size x {l[0]} y {l[1]} w {l[2]} h {l[3]} {DateTime.Now:HH:mm:ss.fff}";
    //// if (type.Name == "MainWindow")
    ////   Services.Base.ServiceBase.Log.Warn(s);
    ////   Console.WriteLine(s);
    //// else
    ////   Gtk.Application.Invoke((sender, e) =>
    ////   {
    ////     MainClass.MainWindow.SetError(s);
    ////   });
    return new Rectangle(l[0], l[1], l[2], l[3]);
  }

  /// <summary>
  /// Sets the dialog size.
  /// </summary>
  /// <param name="type">Affected dialog type.</param>
  /// <param name="l">Affected dialog size as rectangle.</param>
  /// <param name="h">Affected CsbpBin.TitleHeight.</param>
  public static void SetDialogSize(Type type, Rectangle l, int h)
  {
    var th0 = h; // CsbpBin.TitleHeight; // Functions.IsLinux() ? 37 : 0;
    var th = l.X < 1920 ? h : 0; // CsbpBin.TitleHeight : 0; // Title height only on the left screen.
    var y = Math.Max(0, l.Y - th);
    var l0 = GetDialogSize(type);
    if (th0 > 0 && l.X == l0.X && l.Width == l0.Width && l.Height == l0.Height)
    {
      if (y - th0 == l0.Y || y + th0 == l0.Y)
        y = l0.Y;
    }

    // if (l.X == l0.X && (y == l0.Y || y - th0 == l0.Y) && l.Width == l0.Width && l.Height == l0.Height)
    // {
    //   Console.WriteLine($"{type.Name} New size x {l.X} y {y} w {l.Width} h {l.Height} {DateTime.Now:HH:mm:ss.fff} th {th} th0 {th0} ignored");
    //   return;
    // }
    var key = GetDialogKey(type);
    var bytes = Functions.Serialize(new[] { l.X, y, l.Width, l.Height });
    var v = Convert.ToBase64String(bytes);
    SetValue(key, v);
    //// var s = $"{type.Name} New size x {l.X} y {y} w {l.Width} h {l.Height} {DateTime.Now:HH:mm:ss.fff} th {th} th0 {th0}";
    //// if (type.Name == "MainWindow")
    ////   Services.Base.ServiceBase.Log.Warn(s);
    ////   Console.WriteLine(s);
    //// else
    ////   Gtk.Application.Invoke((sender, e) =>
    ////   {
    ////     MainClass.MainWindow.SetError(s);
    ////   });
  }

  /// <summary>
  /// Resets all dialog sizes.
  /// </summary>
  public static void ResetDialogSizes()
  {
    var keys = new List<string>(Params2.Keys);
    foreach (var k in keys)
    {
      if (k.EndsWith("_Size"))
        Params2.Remove(k);
    }
  }

  /// <summary>
  /// Saves all parameters.
  /// </summary>
  public static void Save()
  {
    lock (AppConfig)
    {
      foreach (var p in Params)
      {
        if (!string.IsNullOrEmpty(p.Value.Setting))
          Params2[p.Value.Setting] = GetValue(p.Key);
      }
      if (File.Exists(AppConfig))
        File.Delete(AppConfig);
      using var file = File.OpenWrite(AppConfig);
      using var writer = new Utf8JsonWriter(file, new JsonWriterOptions { Indented = true });
      writer.WriteStartObject();
      foreach (var p in Params2)
      {
        if (!string.IsNullOrEmpty(p.Value))
        {
          writer.WriteString(p.Key, p.Value);
          //// writer.Flush();
        }
      }
      writer.WriteEndObject();
      writer.Flush();
    }
  }

  /// <summary>
  /// Gets the dialog key.
  /// </summary>
  /// <param name="type">Affected dialog type.</param>
  /// <returns>Dialog key.</returns>
  private static string GetDialogKey(Type type)
  {
    var key = $"{type.Name}_Size";
    return key;
  }

  /// <summary>
  /// Reads the settings.
  /// </summary>
  private static void ReadSettings()
  {
    var appconfig0 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".csbp0.json");
    if (File.Exists(AppConfig))
    {
      var cont = true;
      while (cont)
      try
      {
        cont = false;
        using var file = File.OpenRead(AppConfig);
        using var doc = JsonDocument.Parse(file);
        var root = doc.RootElement;
        var values = root.EnumerateObject();
        while (values.MoveNext())
        {
          var v = values.Current;
          if (v.Value.ValueKind == JsonValueKind.String)
            Params2[v.Name] = v.Value.GetString();
        }
      }
      catch (Exception ex)
      {
        if (ex is JsonException)
        {
          if (File.Exists(appconfig0))
          {
            // Restore backup
            File.Copy(appconfig0, AppConfig, true);
            cont = true;
          }
        }
        if (!cont)
          throw;
      }

      // Save backup
      File.Copy(AppConfig, appconfig0, true);
    }
    foreach (var p in Params)
    {
      GetValue(p.Key);
    }
  }
}
