// <copyright file="Parameter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System.Collections.Generic;
using System.Reflection;
using CSBP.Services.Resources;

/// <summary>
/// Base class for managing parameters which are stored in setting file or database.
/// Notes for usage of this class:
/// - extend this class for application specific parameters, e.g. ParameterGui.
/// - write function Init to initialize and read parameters from setting file, especially for parameter DB_DRIVER_CONNECT.
/// - call function Init at start of application.
/// - write function GetOptionList to fill all database parameters from database.
/// - call function GetOptionList after function Init.
/// - write function Save to store parameters in setting file.
/// - call function Save at end of application.
/// </summary>
public class Parameter
{
#pragma warning disable SA1310

  /// <summary>Constant key for database driver connection.</summary>
  public const string DB_DRIVER_CONNECT = "DB_DRIVER_CONNECT";

  /// <summary>Constant key for application title.</summary>
  public const string AG_ANWENDUNGS_TITEL = "AG_ANWENDUNGS_TITEL";

  //// public const string AG_BACKUPS = "AG_BACKUPS";

  /// <summary>Constant key for start dialogs.</summary>
  public const string AG_STARTDIALOGE = "AG_STARTDIALOGE";

  /// <summary>Constant key for test or production.</summary>
  public const string AG_TEST_PRODUKTION = "AG_TEST_PRODUKTION";

  /// <summary>Constant key for GEDCOM file submitter.</summary>
  public const string SB_SUBMITTER = "SB_SUBMITTER";

  /// <summary>Constant key for access key for fixer.io.</summary>
  public const string WP_FIXER_IO_ACCESS_KEY = "WP_FIXER_IO_ACCESS_KEY";

  /// <summary>Constant key for access key for meteostat.com.</summary>
  public const string TB_METEOSTAT_COM_ACCESS_KEY = "TB_METEOSTAT_COM_ACCESS_KEY";

  /// <summary>Constant key for access key for openai.com.</summary>
  public const string AG_OPENAI_COM_ACCESS_KEY = "AG_OPENAI_COM_ACCESS_KEY";

#pragma warning restore SA1310

  /// <summary>
  /// Initializes static members of the <see cref="Parameter"/> class.
  /// </summary>
  static Parameter()
  {
    // var userpath = Functions.IsLinux() ? $"/home/vscodeuser/hsqldb/" : ""; // Für Docker-Versuche
    var userpath = Functions.IsLinux() ? $"/home/{Environment.UserName}/hsqldb/" : "";

    // Values dependant from client number are directly read by function MaParameterRep.GetValue.
    Params = new Dictionary<string, Parameter>
    {
      // Storage in setting file
      { DB_DRIVER_CONNECT, new Parameter(DB_DRIVER_CONNECT, $"Data Source={userpath}csbp.db", setting: "ConnectionString") },

      // Storage in database
      { AG_ANWENDUNGS_TITEL, new Parameter(AG_ANWENDUNGS_TITEL, setting: "Title", database: true, client: true) },
      { AG_STARTDIALOGE, new Parameter(AG_STARTDIALOGE, setting: "StartingDialogs", database: true, client: true) },
      { AG_TEST_PRODUKTION, new Parameter(AG_TEST_PRODUKTION, setting: "TestProduktion", database: true) },
      { SB_SUBMITTER, new Parameter(SB_SUBMITTER, setting: "GedcomSubmitter", database: true) },
      { WP_FIXER_IO_ACCESS_KEY, new Parameter(WP_FIXER_IO_ACCESS_KEY, setting: "FixerIoAccessKey", database: true) },
      { TB_METEOSTAT_COM_ACCESS_KEY, new Parameter(TB_METEOSTAT_COM_ACCESS_KEY, setting: "MeteostatComAccessKey", database: true) },
      { AG_OPENAI_COM_ACCESS_KEY, new Parameter(AG_OPENAI_COM_ACCESS_KEY, setting: "OpenaiComAccessKey", database: true) },
    };
    Params2 = new Dictionary<string, string>(); // Storage in setting file
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
    Default = default_.TrimNull(trim)
      ?? typeof(Messages).GetProperty($"parm_{key}_value", BindingFlags.Static | BindingFlags.Public)?.GetValue(null) as string;
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

  //// TODO public static string Backups {
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
}
