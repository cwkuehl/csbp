// <copyright file="Parameter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.IO;
  using System.Reflection;
  using CSBP.Resources;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class Parameter
  {
    public const string DB_DRIVER_CONNECT = "DB_DRIVER_CONNECT";
    public const string APP_THEME = "APP_THEME";
    public const string AG_ANWENDUNGS_TITEL = "AG_ANWENDUNGS_TITEL";
    // public const string AG_BACKUPS = "AG_BACKUPS";
    public const string AG_HILFE_DATEI = "AG_HILFE_DATEI";
    public const string AG_STARTDIALOGE = "AG_STARTDIALOGE";
    public const string AG_TEMP_PFAD = "AG_TEMP_PFAD";
    public const string AG_TEST_PRODUKTION = "AG_TEST_PRODUKTION";

    public const string SB_SUBMITTER = "SB_SUBMITTER";

    /// <summary>Wertpapier-Parameter: Access Key für Währungskurse von Fixer.io.</summary>
    public const string WP_FIXER_IO_ACCESS_KEY = "WP_FIXER_IO_ACCESS_KEY";

    /// <summary>Dateiname inkl.false Pfad vor .csbp.json.</summary>
    private static readonly string AppConfig;

    /// <summary>Sammlung von festen Parametern.</summary>
    public static Dictionary<string, Parameter> Params { get; private set; }

    /// <summary>Sammlung von allen Parametern.</summary>
    public static Dictionary<string, string> Params2 { get; private set; }

    /// <summary>Holt oder setzt den Schlüssel.</summary>
    public string Key { get; set; }

    /// <summary>Holt oder setzt den Wert als String.</summary>
    public string Value { get; set; }

    /// <summary>Holt oder setzt den Standardwert.</summary>
    public string Default { get; set; }

    /// <summary>Holt oder setzt die Beschreibung zum Parameter.</summary>
    public string Comment { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Wert getrimmt wird.</summary>
    public bool Trim { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Wert verschlüsselt ist.</summary>
    public bool Crypted { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Wert aus den Benutzer-Einstellungen gelesen wurde?</summary>
    public bool Loaded { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Wert in den Benutzer-Einstellungen gespeichert wird?</summary>
    public string Setting { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Wert in der Datenbank gespeichert wird?</summary>
    public bool Database { get; set; }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Wert in der Datenbank pro Mandant gespeichert wird?</summary>
    public bool Client { get; set; }

    static Parameter()
    {
      Params = new Dictionary<string, Parameter> {
        { DB_DRIVER_CONNECT, new Parameter(DB_DRIVER_CONNECT, "Data Source=csbp.db", setting: "ConnectionString") },
        { APP_THEME, new Parameter(APP_THEME, "default", setting: "AppTheme") },
        { AG_ANWENDUNGS_TITEL, new Parameter(AG_ANWENDUNGS_TITEL, setting: "Title", database: true, client: true) },
        //{ AG_BACKUPS, new Parameter(AG_BACKUPS) },
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

    public Parameter(string key, string _default = null, bool trim = true, bool crypted = false,
        string setting = null, bool database = false, bool client = false)
    {
      Key = key;
      Default = _default.TrimNull(trim);
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

    public void SetValue(string value)
    {
      Value = value.TrimNull(Trim);
      if (!string.IsNullOrEmpty(Setting))
        Params2[Setting] = Value;
    }

    /// <summary>
    /// Connection string to database.
    /// </summary>
    /// <value>Connection string.</value>
    public static string Connect
    {
      get { return GetValue(DB_DRIVER_CONNECT); }
      set { SetValue(DB_DRIVER_CONNECT, value); }
    }


    public static string LoginClient
    {
      get { return GetValue("LoginClient"); }
      set { SetValue("LoginClient", value); }
    }

    public static string LoginUser
    {
      get { return GetValue("LoginUser"); }
      set { SetValue("LoginUser", value); }
    }

    /// <summary>Holt oder setzt den Pfad für die temporären Dateien.</summary>
    public static string TempPath
    {
      get { return GetValue(AG_TEMP_PFAD); }
      set { SetValue(AG_TEMP_PFAD, value); }
    }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob die Geburtstagsliste nach dem Anmelden angezeigt wird.</summary>
    public static bool AD120Start
    {
      get { return Functions.ToBool(GetValue("AD120Start")) ?? false; }
      set { SetValue("AD120Start", Functions.ToString(value)); }
    }

    /// <summary>Holt oder setzt die Anzahl der Tage in der Geburtstagsliste.</summary>
    public static int AD120Days
    {
      get { return Functions.ToInt32(GetValue("AD120Days") ?? "12"); }
      set { SetValue("AD120Days", Functions.ToString(value)); }
    }

    /// <summary>Holt oder setzt den Namen der Adress-Datei.</summary>
    public static string AD200File
    {
      get { return GetValue("AD200File"); }
      set { SetValue("AD200File", value); }
    }

    /// <summary>Holt oder setzt die Länge der Periode.</summary>
    public static string HH100Length
    {
      get { return GetValue("HH100Length") ?? "1"; }
      set { SetValue("HH100Length", value); }
    }

    /// <summary>Holt oder setzt die Art des Einfügens einer neuen Periode.</summary>
    public static string HH100When
    {
      get { return GetValue("HH100When") ?? "1"; }
      set { SetValue("HH100When", value); }
    }

    /// <summary>Holt oder setzt den Titel für Bilanzen.</summary>
    public static string HH510Title
    {
      get { return GetValue("HH510Title") ?? "Bilanzen"; }
      set { SetValue("HH510Title", value); }
    }

    /// <summary>Holt oder setzt einen Wert, der angibt, ob der Kassenbericht ausgewählt ist.</summary>
    public static bool HH510Cashreport
    {
      get { return Functions.ToBool(GetValue("HH510Cashreport")) ?? false; }
      set { SetValue("HH510Cashreport", Functions.ToString(value)); }
    }

    /// <summary>Holt oder setzt den Namen der Buchungen-Datei.</summary>
    public static string HH510File
    {
      get { return GetValue("HH510File"); }
      set { SetValue("HH510File", value); }
    }

    /// <summary>Holt oder setzt den Ahnen.</summary>
    public static string SB220Ancestor
    {
      get { return GetValue("SB220Ancestor"); }
      set { SetValue("SB220Ancestor", value); }
    }

    /// <summary>Holt oder setzt die Anzahl der Generationen.</summary>
    public static string SB220Generation
    {
      get { return GetValue("SB220Generation") ?? "3"; }
      set { SetValue("SB220Generation", value); }
    }

    /// <summary>Holt oder setzt den Namen des Stammbaums.</summary>
    public static string SB500Name
    {
      get { return GetValue("SB500Name"); }
      set { SetValue("SB500Name", value); }
    }

    /// <summary>Holt oder setzt den Dateinamen.</summary>
    public static string SB500File
    {
      get { return GetValue("SB500File"); }
      set { SetValue("SB500File", value); }
    }

    /// <summary>Holt oder setzt den Filter.</summary>
    public static string SB500Filter
    {
      get { return GetValue("SB500Filter"); }
      set { SetValue("SB500Filter", value); }
    }

    /// <summary>Get or set the sudoku field.</summary>
    public static string SO100Sudoku
    {
      get { return GetValue("SO100Sudoku"); }
      set { SetValue("SO100Sudoku", value); }
    }

    /// <summary>Get or set the stock configuration.</summary>
    public static string WP200Configuration
    {
      get { return GetValue("WP200Configuration"); }
      set { SetValue("WP200Configuration", value); }
    }

    /// <summary>Get or set the stock configuration.</summary>
    public static string WP220Configuration
    {
      get { return GetValue("WP220Configuration"); }
      set { SetValue("WP220Configuration", value); }
    }

    /// <summary>Get or set the stock.</summary>
    public static string WP220Stock
    {
      get { return GetValue("WP220Stock"); }
      set { SetValue("WP220Stock", value); }
    }

    /// <summary>Get or set the file name.</summary>
    public static string WP220File
    {
      get { return GetValue("WP220File"); }
      set { SetValue("WP220File", value); }
    }

    /// <summary>Get or set the file name 2.</summary>
    public static string WP220File2
    {
      get { return GetValue("WP220File2"); }
      set { SetValue("WP220File2", value); }
    }

    /// <summary>Holt oder setzt die Wertpapier-Konfiguration.</summary>
    public static string WP250Stock
    {
      get { return GetValue("WP250Stock"); }
      set { SetValue("WP250Stock", value); }
    }

    /// <summary>Holt oder setzt die Anlage.</summary>
    public static string WP400Investment
    {
      get { return GetValue("WP400Investment"); }
      set { SetValue("WP400Investment", value); }
    }

    /// <summary>Holt oder setzt das Wertpapier.</summary>
    public static string WP500Stock
    {
      get { return GetValue("WP500Stock"); }
      set { SetValue("WP500Stock", value); }
    }

    //public static string Backups {
    //    get {
    //        var s = GetValue(AG_BACKUPS);
    //        return s;
    //    }
    //    set {
    //        SetValue(AG_BACKUPS, value);
    //    }
    //}

    public static string GetValue(string key)
    {
      if (Params.TryGetValue(key, out var p))
        return p.GetValue();
      if (Params2.TryGetValue(key, out var p2))
        return p2;
      return null;
    }

    public static void SetValue(string key, string value)
    {
      if (Params.TryGetValue(key, out var p))
        p.SetValue(value);
      else
        Params2[key] = value;
    }

    static string GetDialogKey(Type type)
    {
      var key = $"{type.Name}_Size";
      return key;
    }

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

    public static void SetDialogSize(Type type, Rectangle l)
    {
      var key = GetDialogKey(type);
      var bytes = Functions.Serialize(new[] { l.X, l.Y, l.Width, l.Height });
      var v = Convert.ToBase64String(bytes);
      SetValue(key, v);
    }

    public static void ResetDialogSizes()
    {
      var keys = new List<string>(Params2.Keys);
      foreach (var k in keys)
      {
        if (k.EndsWith("_Size"))
          Params2.Remove(k);
      }
    }

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
  }
}
