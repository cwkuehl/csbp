// <copyright file="StatusTask.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

#nullable enable

namespace CSBP.Services.Base;

using System.Net;
using System.Text;

/// <summary>
/// This class manages the status of a running task.
/// </summary>
public class StatusTask
{
  /// <summary>
  /// Interne Daten.
  /// </summary>
  private static readonly List<StatusTask> Daten = new();

  /// <summary>
  /// Betroffener Server-Name.
  /// </summary>
  private static readonly string Servername = CsbpBase.Servername;

  /// <summary>
  /// Pfad für Dateien.
  /// TODO Verzeichnis anlegen, das für alle Server-Instanzen gleich ist, damit die Statusseite auch von anderen Servern gelesen werden kann.
  /// </summary>
  private static readonly string Pfad = CsbpBase.SharedPath;

  /// <summary>
  /// Internal data.
  /// </summary>
  private readonly Dictionary<string, string?> daten = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="StatusTask"/> class.
  /// </summary>
  /// <param name="mandant">Affected tenant.</param>
  /// <param name="funktion">Affected function.</param>
  /// <param name="dateiname">Affected filename.</param>
  /// <param name="kurz">A value indicating whether the message should be short.</param>
  public StatusTask(int mandant, string funktion, string? dateiname, bool kurz = false)
  {
    Mandant = mandant < 0 ? null : Functions.ToString(mandant);
    Mandant2 = null;
    Funktion = funktion;
    Dateiname = dateiname;
    Kurz = kurz;
    StartTask();
  }

  /// <summary>
  /// Gets the tenant.
  /// </summary>
  public string? Mandant { get; private set; }

  /// <summary>
  /// Gets the tenant that can be set later.
  /// </summary>
  public string? Mandant2 { get; private set; }

  /// <summary>
  /// Gets the function name.
  /// </summary>
  public string Funktion { get; private set; }

  /// <summary>
  /// Gets the filename.
  /// </summary>
  public string? Dateiname { get; private set; }

  /// <summary>
  /// Gets the start time.
  /// </summary>
  public DateTime Startzeit { get; private set; }

  /// <summary>
  /// Gets the time of the last change.
  /// </summary>
  public DateTime LetzteAenderung { get; private set; }

  /// <summary>
  /// Gets the time of the last write.
  /// </summary>
  public DateTime? Schreibzeit { get; private set; }

  /// <summary>
  /// Gets the end time.
  /// </summary>
  public DateTime? Endzeit { get; private set; }

  /// <summary>
  /// Gets a value indicating whether the message should be short.
  /// </summary>
  public bool Kurz { get; private set; }

  /// <summary>
  /// Liefert des Status aller gewünschten laufenden Funktionen auf allen Application Servern.
  /// </summary>
  /// <param name="mandant">Betroffener Mandant.</param>
  /// <param name="funktionen">Betroffene Funktionen können null sein.</param>
  /// <param name="kurz">True, wenn nur der aktuelle Taskstatus zurückgegeben werden soll.</param>
  /// <returns>Status als String.</returns>
  public static string GetStatus(int mandant, string[]? funktionen = null, bool kurz = false)
  {
      var suche = $"ST_{mandant}_*.txt";
      var fliste = funktionen?.Select(a => $"_{a}_").ToArray();
      var files = new DirectoryInfo(Pfad).GetFiles(suche).OrderByDescending(f => f.LastWriteTime).Select(f => f.FullName).ToList();
      if (kurz)
        files = files.Take(1).ToList();
      var sb = new StringBuilder();
      foreach (var file in files)
      {
          if (!file.Contains("ST_Abbruch_") && (fliste == null || Array.Exists(fliste, a => file.Contains(a))))
          {
              if (sb.Length > 0)
                  sb.Append(Constants.CrLf).Append(Constants.CrLf);
              sb.Append(File.ReadAllText(file));
          }
      }
      return sb.ToString();
  }

  /// <summary>
  /// Liefert zurück, ob eine Funktion noch am Laufen ist.
  /// </summary>
  /// <param name="mandant">Betroffener Mandant kann null.</param>
  /// <param name="funktionen">Betroffene Funktionen können null sein.</param>
  /// <returns>True, wenn Funktion noch läuft.</returns>
  public static bool IsTAmLaufen(int mandant = -1, string[]? funktionen = null)
  {
    foreach (var f in Daten)
    {
      if ((mandant == -1 || f.Mandant == Functions.ToString(mandant))
        && (funktionen == null || Array.Exists(funktionen, a => a == f.Funktion)))
        if (f.IsTAmLaufen())
          return true;
    }
    return false;
  }

  /// <summary>
  /// Abbruch-Kennzeichen in allen lokalen laufenden Funktionen setzen.
  /// Abbruch-Dateien für die anderen Application Server schreiben.
  /// </summary>
  /// <param name="mandant">Betroffener Mandant kann null.</param>
  /// <param name="funktionen">Betroffene Funktionen können null sein.</param>
  public static void Abbrechen(string? mandant = null, string[]? funktionen = null)
  {
    foreach (var f in Daten)
    {
      if ((mandant == null || f.Mandant == mandant) && (funktionen == null || Array.Exists(funktionen, a => a == f.Funktion)))
        if (f.IsTAmLaufen())
          f.SetAbbruch();
    }
    var m = string.IsNullOrEmpty(mandant) ? "M###" : mandant;
    var fliste = funktionen == null ? new List<string>() : funktionen.Where(a => !string.IsNullOrEmpty(a)).ToList();
    if (!fliste.Any())
      fliste.Add("F###");
    foreach (var f in fliste)
    {
      var dateiname = Path.Combine(Pfad, Functions.GetDateiname($"ST_Abbruch_{m}_{f}_{Servername}", true, true, true, "txt"));
      try
      {
        File.WriteAllText(dateiname, Servername);
      }
      catch (Exception)
      {
        Functions.MachNichts();
      }
    }
  }

  /// <summary>
  /// Räumt die Funktionen auf, die zuletzt vor 3 Stunden geändert wurden.
  /// </summary>
  public static void Aufraeumen()
  {
    lock (Daten)
    {
      var aktDate = DateTime.Now;
      var deleteDate = aktDate.AddMinutes(-180);
      var dliste = new List<StatusTask>();
      foreach (var f in Daten)
      {
        if (deleteDate > f.LetzteAenderung)
        {
          dliste.Add(f);
        }
      }
      foreach (var f in dliste)
      {
        // Löschen durchführen.
        Daten.Remove(f);
      }

      // Auch alle älteren Dateien löschen.
      var files = Directory.GetFiles(Pfad, "ST_*.txt");
      foreach (var file in files)
      {
        var fi = new FileInfo(file);
        if (fi.LastWriteTime < deleteDate)
        {
          try
          {
              File.Delete(file);
          }
          catch (Exception)
          {
            Functions.MachNichts();
          }
        }
      }

      // Abbrüche von anderen Application Servern anwenden.
      files = Directory.GetFiles(Pfad, "ST_Abbruch_*.txt");
      foreach (var file in files)
      {
        var inhalt = File.ReadAllText(file) ?? "";
        if (!inhalt.Contains(Servername))
        {
          // Betroffene laufende Funktionen abbrechen.
          var arr = file.Split(["_"], StringSplitOptions.RemoveEmptyEntries);
          var m = (arr.Length < 2 ? null : arr[2]) ?? "###";
          var fkt = (arr.Length < 3 ? null : arr[3]) ?? "###";
          if (m == "M###")
            m = null;
          if (fkt == "F###")
            fkt = null;
          foreach (var f in Daten)
          {
            if ((m == null || f.Mandant == m) && (fkt == null || f.Funktion == fkt))
              f.SetAbbruch();
          }
          try
          {
            // Eigenen Server eintragen, damit die nicht mehr verarbeitet wird.
            File.WriteAllText(file, $"{inhalt} {Servername}");
          }
          catch (Exception)
          {
            Functions.MachNichts();
          }
        }
      }
    }
  }

  /// <summary>
  /// Erzeugt einen neuen StatusTask.
  /// </summary>
  /// <param name="mandant">Betroffener Mandant oder -1.</param>
  /// <param name="funktion">Betroffene Funktion.</param>
  /// <param name="temporaer">True, wenn die betroffene Funktion nicht gemerkt und der Status nicht in eine Datei geschrieben werden soll.</param>
  /// <param name="funktionen">Für die betroffenen Prüffunktionen dürfen keinen Funktionen laufen.</param>
  /// <returns>Neue StatusTask oder null, wenn andere Funktion schon läuft.</returns>
  public static ServiceErgebnis<StatusTask?> HinzufuegenFunktion(int mandant, string funktion, bool temporaer = false, string[]? funktionen = null)
  {
    var r = new ServiceErgebnis<StatusTask?>();
    var dateiname = temporaer ? null
      : Path.Combine(Pfad, Functions.GetDateiname($"ST_{mandant}_{funktion}_{Servername}", true, true, true, "txt"));
    var f = new StatusTask(mandant, funktion, dateiname, true);
    r.Ergebnis = f;
    if (!temporaer)
    {
      lock (Daten)
      {
        if (IsTAmLaufen(mandant, funktionen ?? [funktion]))
        {
          r.Errors.Add(new Message($"Während der Verarbeitung von {funktion} wurde versucht, die Verarbeitung noch einmal"
            + " (für den gleichen Mandanten) zu starten. Nur die Daten der ursprünglichen Anforderung werden verarbeitet.", true));
          f.Beenden(null, "Sofortige Beendigung", r);
          return r;
        }
        Daten.Add(f);
      }
    }
    return r;
  }

  /// <summary>Sets the start time.</summary>
  public void StartTask()
  {
    Startzeit = DateTime.Now;
    LetzteAenderung = Startzeit;
    Aendern();
  }

  /// <summary>Sets the visible tenant.</summary>
  /// <param name="s">Affected string.</param>
  public void SetMandant(string s)
  {
    Mandant2 = Functions.TrimNull(s);
    Aendern();
  }

  /// <summary>Sets an error.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="nichtueberschreiben">An existing error is not overwritten.</param>
  public void SetFehler(string s, bool nichtueberschreiben = false)
  {
    var f = Functions.TrimNull(s);
    if (nichtueberschreiben && daten["Fehler"] != null)
      return;
    //// if (f != null)
    daten["Fehler"] = f;
    Aendern();
  }

  /// <summary>Sets the result.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="anhaengen">Affected string to be appended.</param>
  public void SetErgebnis(string s, bool anhaengen = false)
  {
    s = Functions.TrimNull(s);
    if (anhaengen && daten.TryGetValue("Ergebnis", out var v) && !string.IsNullOrEmpty(v))
    {
      if (s == null)
        s = v;
      else
        s = $"{s}{Constants.CrLf}{v}";
    }
    daten["Ergebnis"] = s;
    Aendern();
  }

  /// <summary>Sets the database.</summary>
  /// <param name="s">Affected string.</param>
  public void SetDatenbank(string s)
  {
    daten["Datenbank"] = Functions.TrimNull(s);
    Aendern();
  }

  /// <summary>Sets the name.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="trenner">If a separator is specified, the string will be appended. If a value exists, the separator is added in between.</param>
  /// <returns>True if a value was present.</returns>
  public bool SetName(string s, string? trenner = null)
  {
    var da = false;
    daten["Name"] = Functions.TrimNull(s);
    if (trenner != null && daten.TryGetValue("Ergebnis", out var v) && !string.IsNullOrEmpty(v))
    {
      da = true;
      if (s == null)
        s = v;
      else
        s = $"{s}{trenner}{v}";
    }
    daten["Name"] = s;
    Aendern();
    return da;
  }

  /// <summary>Sets the table.</summary>
  /// <param name="s">Affected string.</param>
  public void SetTabelle(string s)
  {
    daten["Tabelle"] = Functions.TrimNull(s);
    daten["Nr"] = Functions.ToString(0);
    daten["Anzahl"] = Functions.ToString(0);
    Aendern();
  }

  /// <summary>Sets the number.</summary>
  /// <param name="i">Affected number.</param>
  public void SetNr(int i)
  {
    daten["Nr"] = Functions.ToString(i);
    Aendern();
  }

  /// <summary>Sets the count.</summary>
  /// <param name="i">Affected number.</param>
  public void SetAnzahl(int i)
  {
    daten["Anzahl"] = Functions.ToString(i);
    Aendern();
  }

  /// <summary>Sets the total count.</summary>
  /// <param name="i">Affected number.</param>
  public void SetAnzahlGesamt(int i)
  {
    daten["AnzahlGesamt"] = Functions.ToString(i);
    Aendern();
  }

  /// <summary>Sets the posting position.</summary>
  /// <param name="s">Affected string.</param>
  public void SetBust(string s)
  {
    daten["Bust"] = Functions.TrimNull(s);
    //// daten["Nr"] = Functions.ToString(0);
    //// daten["Anzahl"] = Functions.ToString(0);
    Aendern();
  }

  /// <summary>Sets a message.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="anhaengen">Affected string to be appended.</param>
  public void SetMeldung(string s, bool anhaengen = false)
  {
    s = Functions.TrimNull(s);
    if (anhaengen && daten.TryGetValue("Meldung", out var v) && !string.IsNullOrEmpty(v))
    {
      if (s == null)
        s = v;
      else
        s = $"{s}{Constants.CrLf}{v}";
    }
    daten["Meldung"] = s;
    Aendern();
  }

  /// <summary>Sets a second message.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="anhaengen">Affected string to be appended.</param>
  public void SetMeldung1(string s, bool anhaengen = false)
  {
    s = Functions.TrimNull(s);
    if (anhaengen && daten.TryGetValue("Meldung1", out var v) && !string.IsNullOrEmpty(v))
    {
      if (s == null)
        s = v;
      else
        s = $"{s}{Constants.CrLf}{v}";
    }
    daten["Meldung1"] = s;
    Aendern();
  }

  /// <summary>Sets a third message.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="anhaengen">Affected string to be appended.</param>
  public void SetMeldung2(string s, bool anhaengen = false)
  {
    s = Functions.TrimNull(s);
    if (anhaengen && daten.TryGetValue("Meldung2", out var v) && !string.IsNullOrEmpty(v))
    {
      if (s == null)
        s = v;
      else
        s = $"{s}{Constants.CrLf}{v}";
    }
    daten["Meldung2"] = s;
    Aendern();
  }

  /// <summary>Sets a fourth message.</summary>
  /// <param name="s">Affected string.</param>
  /// <param name="anhaengen">Affected string to be appended.</param>
  public void SetMeldung3(string s, bool anhaengen = false)
  {
    s = Functions.TrimNull(s);
    if (anhaengen && daten.TryGetValue("Meldung3", out var v) && !string.IsNullOrEmpty(v))
    {
      if (s == null)
        s = v;
      else
        s = $"{s}{Constants.CrLf}{v}";
    }
    daten["Meldung3"] = s;
    Aendern();
  }

  /// <summary>Sets a fifth message.</summary>
  /// <param name="s">Affected string.</param>
  public void SetMeldung4(string s)
  {
    daten["Meldung4"] = Functions.TrimNull(s);
    Aendern();
  }

  /// <summary>Sets the abort flag.</summary>
  public void SetAbbruch()
  {
    daten["Abbruch"] = "true";
    Aendern();
  }

  /// <summary>Checks the abort flag.</summary>
  /// <returns>True if the function was aborted.</returns>
  public bool IstAbbruch()
  {
    return daten.TryGetValue("Abbruch", out var v) && v != null;
  }

  /// <summary>Sets the lock flag.</summary>
  public void SetSperre()
  {
    daten["SPERRE"] = "Data transfer";
    //// Aendern();
  }

  /// <summary>Checks the lock flag.</summary>
  /// <returns>True if the lock was set.</returns>
  public bool IstSperre()
  {
    return daten.TryGetValue("SPERRE", out var v) && v != null;
  }

  /// <summary>Sets the XML data (most likely obsolete).</summary>
  /// <param name="s">Affected string.</param>
  public void SetXmlDaten(string s)
  {
    daten["XmlDaten"] = Functions.TrimNull(s);
    Aendern();
  }

  /// <summary>
  /// Returns whether the function is still running.
  /// </summary>
  /// <returns>True if the function is still running.</returns>
  public bool IsTAmLaufen()
  {
    // return ((!daten.TryGetValue("Ergebnis", out var v) || v == null)
    //     && (!daten.TryGetValue("Fehler", out v) || v == null)) || !Endzeit.HasValue;
    return !Endzeit.HasValue;
  }

  /// <summary>
  /// Writes the function to a file and updates LastChange.
  /// </summary>
  /// <param name="ja">Write in any case.</param>
  public void Aendern(bool ja = false)
  {
    LetzteAenderung = DateTime.Now;
    Schreiben(ja);
  }

  /// <summary>
  /// Returns the status as a string.
  /// </summary>
  /// <param name="kurz">Indicates whether to return a short version.</param>
  /// <returns>Status as a string.</returns>
  public string GetStatus(bool kurz = false)
  {
    var sb = new StringBuilder();
    var tr = Constants.CrLf;
    string? v;
    if (kurz)
    {
      tr = " ";
      sb.Append(Funktion).Append(": ");
    }
    else
    {
      sb.Append("Status");
      var m = Mandant2 ?? Functions.ToString(Mandant);
      if (!string.IsNullOrEmpty(m))
        sb.Append(" von Mandant ").Append(m);
      sb.Append(" auf Server ").Append(Functions.Right(Servername, 4)); // Server name incomplete.
      if (daten.TryGetValue("Datenbank", out v) && v != null)
        sb.Append(" auf Datenbank ").Append(Functions.ToUpper(v));
      sb.Append(": ");
      sb.Append(tr).Append("Funktion: ").Append(Funktion);
      if (daten.TryGetValue("Name", out v) && v != null)
        sb.Append(" ").Append(v);
      sb.Append(tr).Append("Verarbeitung gestartet: ").Append(Functions.ToString(Startzeit, true));
      sb.Append(tr).Append("Letzte Änderung: ").Append(Functions.ToString(LetzteAenderung, true));
      if (daten.TryGetValue("Benutzer", out v) && v != null)
        sb.Append(tr).Append("User: ").Append(v);
    }
    if (!Endzeit.HasValue || !kurz)
    {
      if (daten.TryGetValue("Meldung", out v) && v != null)
        sb.Append(tr).Append(v);
      if (daten.TryGetValue("Meldung1", out v) && v != null)
        sb.Append(tr).Append(v);
      if (daten.TryGetValue("Meldung2", out v) && v != null)
        sb.Append(tr).Append(v);
      if (daten.TryGetValue("Meldung3", out v) && v != null)
        sb.Append(tr).Append(v);
      if (daten.TryGetValue("Meldung4", out v) && v != null)
        sb.Append(tr).Append(v);
    }
    if (kurz)
    {
      if (Endzeit.HasValue)
      {
        sb.Append(tr).Append("Beendet (").Append(Endzeit.Value.Subtract(Startzeit).TotalMilliseconds.ToString("0")).Append(" ms)");
      }
    }
    else
    {
      if (daten.TryGetValue("Bust", out v) && v != null)
        sb.Append(tr).Append("Aktuelle Buchungsstelle: ").Append(v);
      if (daten.TryGetValue("Tabelle", out v) && v != null)
        sb.Append(tr).Append("Aktuelle Tabelle: ").Append(v);
      if (daten.TryGetValue("Nr", out v) && v != null)
      {
        if (!daten.TryGetValue("Bust", out _) && !daten.TryGetValue("Tabelle", out _))
          sb.Append(tr);
        else
          sb.Append(" ");
        sb.Append("Datensatz: ").Append(v);
      }
      if (daten.TryGetValue("Anzahl", out v) && v != null)
        sb.Append(" von ").Append(v);
      if (daten.TryGetValue("AnzahlGesamt", out v) && v != null)
        sb.Append(tr).Append("Datensätze bisher: ").Append(v);
      if (daten.TryGetValue("Fehler", out v) && v != null)
        sb.Append(tr).Append("Fehler: ").Append(v);
      if (daten.TryGetValue("Ergebnis", out v) & v != null)
        sb.Append(tr).Append("Ergebnis: ").Append(v);
      if (daten.TryGetValue("Abbruch", out v) & v != null)
        sb.Append(tr).Append("Abbruch: angefordert");
      if (Endzeit.HasValue)
        sb.Append(tr).Append("Verarbeitung beendet: ").Append(Functions.ToString(Endzeit.Value));
    }
    return sb.ToString();
  }

  /// <summary>
  /// Writes the function to a file, at most every 10 seconds.
  /// </summary>
  /// <param name="ja">Write in any case.</param>
  public void Schreiben(bool ja = false)
  {
    var j = DateTime.Now;
    if (Dateiname == null || !(ja || !Schreibzeit.HasValue || Schreibzeit.Value < j.AddMilliseconds(Kurz ? -500 : -10000)))
      return;
    try
    {
      File.WriteAllText(Dateiname, GetStatus(Kurz));
    }
    catch (Exception)
    {
      // ServiceBase.Log.Error("WebServiceStatusFunktion write", ex);
      // Try again after waiting a while.
      Thread.Sleep(Functions.NextRandom(1000, 2000));
      File.WriteAllText(Dateiname, GetStatus(Kurz));
    }
    Schreibzeit = j;
  }

  /// <summary>
  /// Deletes the associated file.
  /// </summary>
  public void Loeschen()
  {
    try
    {
      if (Dateiname != null && File.Exists(Dateiname))
        File.Delete(Dateiname);
    }
    catch (Exception)
    {
      Functions.MachNichts();
    }
  }

  /// <summary>
  /// Ends the function.
  /// </summary>
  /// <typeparam name="T">Type of the result.</typeparam>
  /// <param name="fehler">Affected error can be null, is also noted as a result if result is null.</param>
  /// <param name="ergebnis">Affected result can be null.</param>
  /// <param name="r">Affected ServiceResult can be null. Messages and error messages are transferred to the result.</param>
  /// <returns>Result as string.</returns>
  public string Beenden<T>(string? fehler = null, string? ergebnis = null, ServiceErgebnis<T>? r = null)
  {
    var sbf = new StringBuilder();
    var sbe = new StringBuilder();
    var f = Functions.TrimNull(fehler);
    var e = Functions.TrimNull(ergebnis);
    if (daten.TryGetValue("Fehler", out var v) && v != null)
    {
      if (r == null)
        sbf.Append(v);
      else
        r.Errors.Add(new Message(v, true));
    }
    if (daten.TryGetValue("Ergebnis", out v) && v != null)
    {
      if (r == null)
        sbe.Append(v);
      else
        r.Messages.Add(new Message(v, true));
    }
    if (!string.IsNullOrEmpty(f))
    {
      if (sbf.Length > 0)
        sbf.Append(Constants.CrLf);
      sbf.Append(f);
    }
    if (!string.IsNullOrEmpty(e))
    {
        if (sbe.Length > 0)
          sbe.Append(Constants.CrLf);
        sbe.Append(e);
    }
    if (r != null)
    {
      foreach (var m in r.Messages)
      {
        if (sbe.Length > 0)
          sbe.Append(Constants.CrLf);
        sbe.Append(m.MessageText);
      }
      foreach (var m in r.Errors)
      {
        if (sbf.Length > 0)
          sbf.Append(Constants.CrLf);
        sbf.Append(m.MessageText);
      }
    }
    if (sbe.Length <= 0 && !string.IsNullOrEmpty(f))
      sbe.Append(f);
    if (sbe.Length <= 0)
      sbe.Append("Ohne Ergebnis."); // zur Sicherheit
    daten["Fehler"] = sbf.Length <= 0 ? null : sbf.ToString();
    var s = sbe.ToString();
    daten["Ergebnis"] = s;
    Endzeit = DateTime.Now;
    LetzteAenderung = Endzeit.Value;
    Schreiben(true);
    //// SetAbbruch();
    return s;
  }

  /// <summary>
  /// Ends the function.
  /// </summary>
  /// <param name="fehler">Affected error can be null, is also noted as a result if result is null.</param>
  /// <param name="ergebnis">Affected result can be null.</param>
  /// <param name="r">Affected ServiceResult can be null. Messages and error messages are transferred to the result.</param>
  /// <returns>True if the function was successful, false otherwise.</returns>
  public bool Beenden(string? fehler = null, string? ergebnis = null, ServiceErgebnis? r = null)
  {
    var sbf = new StringBuilder();
    var sbe = new StringBuilder();
    var f = Functions.TrimNull(fehler);
    var e = Functions.TrimNull(ergebnis);
    if (daten.TryGetValue("Fehler", out var v) && v != null)
    {
      if (r == null)
        sbf.Append(v);
      else
        r.Errors.Add(new Message(v, true));
    }
    if (daten.TryGetValue("Ergebnis", out v) && v != null)
    {
      if (r == null)
        sbe.Append(v);
      else
        r.Messages.Add(new Message(v, true));
    }
    if (!string.IsNullOrEmpty(f))
    {
      if (sbf.Length > 0)
        sbf.Append(Constants.CrLf);
      sbf.Append(f);
    }
    if (!string.IsNullOrEmpty(e))
    {
        if (sbe.Length > 0)
          sbe.Append(Constants.CrLf);
        sbe.Append(e);
    }
    if (r != null)
    {
      foreach (var m in r.Messages)
      {
        if (sbe.Length > 0)
          sbe.Append(Constants.CrLf);
        sbe.Append(m.MessageText);
      }
      foreach (var m in r.Errors)
      {
        if (sbf.Length > 0)
          sbf.Append(Constants.CrLf);
        sbf.Append(m.MessageText);
      }
    }
    if (sbe.Length <= 0 && !string.IsNullOrEmpty(f))
      sbe.Append(f);
    if (sbe.Length <= 0)
      sbe.Append("Ohne Ergebnis."); // zur Sicherheit
    daten["Fehler"] = sbf.Length <= 0 ? null : sbf.ToString();
    var s = sbe.ToString();
    daten["Ergebnis"] = s;
    Endzeit = DateTime.Now;
    LetzteAenderung = Endzeit.Value;
    Schreiben(true);
    return sbf.Length <= 0;
  }
}
