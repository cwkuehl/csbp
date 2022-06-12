// <copyright file="Parameter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using CSBP.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Manager of parameter which are stored in setting file or database.
/// </summary>
public class Parameter
{
#pragma warning disable SA1310

  /// <summary>Constant key for database driver connection.</summary>
  public const string DB_DRIVER_CONNECT = "DB_DRIVER_CONNECT";

  /// <summary>Constant key for application theme.</summary>
  public const string APP_THEME = "APP_THEME";

  /// <summary>Constant key for application title.</summary>
  public const string AG_ANWENDUNGS_TITEL = "AG_ANWENDUNGS_TITEL";

  //// public const string AG_BACKUPS = "AG_BACKUPS";

  /// <summary>Constant key for help file.</summary>
  public const string AG_HILFE_DATEI = "AG_HILFE_DATEI";

  /// <summary>Constant key for start dialogs.</summary>
  public const string AG_STARTDIALOGE = "AG_STARTDIALOGE";

  /// <summary>Constant key for folder for temporary files.</summary>
  public const string AG_TEMP_PFAD = "AG_TEMP_PFAD";

  /// <summary>Constant key for test or production.</summary>
  public const string AG_TEST_PRODUKTION = "AG_TEST_PRODUKTION";

  /// <summary>Constant key for GEDCOM file submitter.</summary>
  public const string SB_SUBMITTER = "SB_SUBMITTER";

  /// <summary>Constant key for access key for fixer.io.</summary>
  public const string WP_FIXER_IO_ACCESS_KEY = "WP_FIXER_IO_ACCESS_KEY";

#pragma warning restore SA1310

  /// <summary>Setting file name with path, e.g. .csbp.json.</summary>
  private static readonly string AppConfig;

  /// <summary>
  /// Initializes static members of the <see cref="Parameter"/> class.
  /// </summary>
  static Parameter()
  {
    Params = new Dictionary<string, Parameter>
    {
      { DB_DRIVER_CONNECT, new Parameter(DB_DRIVER_CONNECT, "Data Source=csbp.db", setting: "ConnectionString") },
      { APP_THEME, new Parameter(APP_THEME, "default", setting: "AppTheme") },
      { AG_ANWENDUNGS_TITEL, new Parameter(AG_ANWENDUNGS_TITEL, setting: "Title", database: true, client: true) },
      //// { AG_BACKUPS, new Parameter(AG_BACKUPS) },
      { AG_HILFE_DATEI, new Parameter(AG_HILFE_DATEI, setting: "HelpFile") },
      { AG_STARTDIALOGE, new Parameter(AG_STARTDIALOGE, setting: "StartingDialogs", database: true, client: true) },
      { AG_TEMP_PFAD, new Parameter(AG_TEMP_PFAD, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "temp"), setting: "TempPath") },
      { AG_TEST_PRODUKTION, new Parameter(AG_TEST_PRODUKTION, setting: "TestProduktion", database: true) },
      { SB_SUBMITTER, new Parameter(SB_SUBMITTER, setting: "GedcomSubmitter", database: true) },
      { WP_FIXER_IO_ACCESS_KEY, new Parameter(WP_FIXER_IO_ACCESS_KEY, setting: "FixerIoAccessKey", database: true) },
    };
    Params2 = new Dictionary<string, string>();
    AppConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".csbp.json");
    if (File.Exists(AppConfig))
    {
      try
      {
        using var file = File.OpenText(AppConfig);
        using var reader = new JsonTextReader(file);
        var jo = (JObject)JToken.ReadFrom(reader);
        foreach (var jt in jo.Values())
        {
          if (jt.Type == JTokenType.String)
            Params2[jt.Path] = jt.Value<string>();
        }
      }
      catch (Exception)
      {
        Functions.MachNichts();
      }
    }
    foreach (var p in Params)
    {
      GetValue(p.Key);
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Parameter"/> class.
  /// </summary>
  /// <param name="key">Affected key.</param>
  /// <param name="default_">Affected default value.</param>
  /// <param name="trim">Affected trim value.</param>
  /// <param name="crypted">Affected crypted value.</param>
  /// <param name="setting">Affected setting value.</param>
  /// <param name="database">Affected database value.</param>
  /// <param name="client">Affected client value.</param>
  public Parameter(string key, string default_ = null, bool trim = true, bool crypted = false,
      string setting = null, bool database = false, bool client = false)
  {
    Key = key;
    Default = default_.TrimNull(trim);
    if (Default == null)
    {
      Default = typeof(Messages).GetProperty($"parm_{key}_value", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) as string;
    }
    Comment = typeof(Messages).GetProperty($"parm_{key}_text", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) as string;
    Trim = trim;
    Crypted = crypted;
    Setting = setting;
    Database = database;
    Client = client;
  }

  /// <summary>Gets or sets the connection string to database.</summary>
  public static string Connect
  {
    get { return GetValue(DB_DRIVER_CONNECT); }
    set { SetValue(DB_DRIVER_CONNECT, value); }
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

  /// <summary>Gets the list of all regulary parameters with constants.</summary>
  public static Dictionary<string, Parameter> Params { get; private set; }

  /// <summary>Gets the list of all parameters.</summary>
  public static Dictionary<string, string> Params2 { get; private set; }

  /// <summary>Gets or sets the key.</summary>
  public string Key { get; set; }

  /// <summary>Gets or sets the string value.</summary>
  public string Value { get; set; }

  /// <summary>Gets or sets the default value.</summary>
  public string Default { get; set; }

  /// <summary>Gets or sets the description.</summary>
  public string Comment { get; set; }

  /// <summary>Gets or sets a value indicating whether the parameter value is always trimmed.</summary>
  public bool Trim { get; set; }

  /// <summary>Gets or sets a value indicating whether the parameter value is always crypted.</summary>
  public bool Crypted { get; set; }

  /// <summary>Gets or sets a value indicating whether the parameter value is initially loaded.</summary>
  public bool Loaded { get; set; }

  /// <summary>Gets or sets the key in the setting file.</summary>
  public string Setting { get; set; }

  /// <summary>Gets or sets a value indicating whether the parameter value is stored in database.</summary>
  public bool Database { get; set; }

  /// <summary>Gets or sets a value indicating whether the parameter value is stored in database by client number.</summary>
  public bool Client { get; set; }

  /// <summary>
  /// Gets the parameter value.
  /// </summary>
  /// <param name="key">Affected parameter key.</param>
  /// <returns>Parameter value.</returns>
  public static string GetValue(string key)
  {
    if (Params.TryGetValue(key, out var p))
      return p.GetValue();
    if (Params2.TryGetValue(key, out var p2))
      return p2;
    return null;
  }

  /// <summary>
  /// Sets the parameter value.
  /// </summary>
  /// <param name="key">Affected parameter key.</param>
  /// <param name="value">New value.</param>
  public static void SetValue(string key, string value)
  {
    if (Params.TryGetValue(key, out var p))
      p.SetValue(value);
    else
      Params2[key] = value;
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
    if (l == null)
      l = new[] { -1, -1, 400, 300 };
    return new Rectangle(l[0], l[1], l[2], l[3]);
  }

  /// <summary>
  /// Sets the dialog size.
  /// </summary>
  /// <param name="type">Affected dialog type.</param>
  /// <param name="l">Affected dialog size as rectangle.</param>
  public static void SetDialogSize(Type type, Rectangle l)
  {
    var key = GetDialogKey(type);
    var bytes = Functions.Serialize(new[] { l.X, l.Y, l.Width, l.Height });
    var v = Convert.ToBase64String(bytes);
    SetValue(key, v);
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
      var jo = new JObject();
      foreach (var p in Params2)
      {
        if (!string.IsNullOrEmpty(p.Value))
          jo[p.Key] = p.Value;
      }
      using var file = File.CreateText(AppConfig);
      using var writer = new JsonTextWriter(file);
      jo.WriteTo(writer);
    }
  }

  /// <summary>
  /// Gets the parameter value.
  /// </summary>
  /// <returns>Parameter value.</returns>
  public string GetValue()
  {
    if (!string.IsNullOrEmpty(Setting) && !Loaded)
    {
      Value = Params2.TryGetValue(Setting, out var value) ? value.TrimNull(Trim) : null;
      Loaded = true;
    }
    var s = Value ?? Default ?? "";
    if (Trim)
      s = s.Trim();
    return s;
  }

  /// <summary>
  /// Sets the parameter value.
  /// </summary>
  /// <param name="value">New value.</param>
  public void SetValue(string value)
  {
    Value = value.TrimNull(Trim);
    if (!string.IsNullOrEmpty(Setting))
      Params2[Setting] = Value;
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
}
