// <copyright file="StatusTask.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

#nullable enable

namespace CSBP.Services.Base;

using System.Text;

/// <summary>
/// This class manages the status of a running task.
/// </summary>
public class StatusTask
{
  /// <summary>
  /// Internal data.
  /// </summary>
  private readonly Dictionary<string, string?> daten = new();

  /// <summary>
  /// Initializes a new instance of the <see cref="StatusTask"/> class.
  /// </summary>
  /// <param name="server">Affected server.</param>
  /// <param name="mandant">Affected tenant.</param>
  /// <param name="funktion">Affected function.</param>
  /// <param name="dateiname">Affected filename.</param>
  public StatusTask(string server, string mandant, string funktion, string dateiname)
  {
    Server = server;
    Mandant = mandant;
    Mandant2 = null;
    Funktion = funktion;
    Dateiname = dateiname;
    Startzeit = DateTime.Now;
    LetzteAenderung = Startzeit;
  }

  /// <summary>
  /// Gets the server.
  /// </summary>
  public string Server { get; private set; }

  /// <summary>
  /// Gets the tenant.
  /// </summary>
  public string Mandant { get; private set; }

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
  public string Dateiname { get; private set; }

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
  /// <returns>Status as a string.</returns>
  public string GetStatus()
  {
    var sb = new StringBuilder();
    sb.Append("Status");
    var m = Mandant2 ?? Mandant;
    if (!string.IsNullOrEmpty(m))
      sb.Append(" of tenant ").Append(m);
    sb.Append(" on server ").Append(Functions.Right(Server, 4)); // Server name incomplete.
    if (daten.TryGetValue("Datenbank", out var v) && v != null)
      sb.Append(" on database ").Append(Functions.ToUpper(v));
    sb.Append(": ");
    sb.Append(Constants.CrLf).Append("Function: ").Append(Funktion);
    if (daten.TryGetValue("Name", out v) && v != null)
      sb.Append(" ").Append(v);
    sb.Append(Constants.CrLf).Append("Processing started: ").Append(Functions.ToString(Startzeit));
    sb.Append(Constants.CrLf).Append("Last change: ").Append(Functions.ToString(LetzteAenderung));
    if (daten.TryGetValue("Benutzer", out v) && v != null)
      sb.Append(Constants.CrLf).Append("User: ").Append(v);
    if (daten.TryGetValue("Meldung", out v) && v != null)
      sb.Append(Constants.CrLf).Append(v);
    if (daten.TryGetValue("Meldung1", out v) && v != null)
      sb.Append(Constants.CrLf).Append(v);
    if (daten.TryGetValue("Meldung2", out v) && v != null)
      sb.Append(Constants.CrLf).Append(v);
    if (daten.TryGetValue("Meldung3", out v) && v != null)
      sb.Append(Constants.CrLf).Append(v);
    if (daten.TryGetValue("Meldung4", out v) && v != null)
      sb.Append(Constants.CrLf).Append(v);
    if (daten.TryGetValue("Bust", out v) && v != null)
      sb.Append(Constants.CrLf).Append("Current posting position: ").Append(v);
    if (daten.TryGetValue("Tabelle", out v) && v != null)
      sb.Append(Constants.CrLf).Append("Current table: ").Append(v);
    if (daten.TryGetValue("Nr", out v) && v != null)
    {
      if (!daten.TryGetValue("Bust", out _) && !daten.TryGetValue("Tabelle", out _))
        sb.Append(Constants.CrLf);
      else
        sb.Append(" ");
      sb.Append("Record: ").Append(v);
    }
    if (daten.TryGetValue("Anzahl", out v) && v != null)
      sb.Append(" of ").Append(v);
    if (daten.TryGetValue("AnzahlGesamt", out v) && v != null)
      sb.Append(Constants.CrLf).Append("Records so far: ").Append(v);
    if (daten.TryGetValue("Fehler", out v) && v != null)
      sb.Append(Constants.CrLf).Append("Error: ").Append(v);
    if (daten.TryGetValue("Ergebnis", out v) & v != null)
      sb.Append(Constants.CrLf).Append("Result: ").Append(v);
    if (daten.TryGetValue("Abbruch", out v) & v != null)
      sb.Append(Constants.CrLf).Append("Abort: requested");
    if (Endzeit.HasValue)
      sb.Append(Constants.CrLf).Append("Processing finished: ").Append(Functions.ToString(Endzeit.Value));
    return sb.ToString();
  }

  /// <summary>
  /// Writes the function to a file, at most every 10 seconds.
  /// </summary>
  /// <param name="ja">Write in any case.</param>
  public void Schreiben(bool ja = false)
  {
    var j = DateTime.Now;
    if (Dateiname == null || !(ja || !Schreibzeit.HasValue || Schreibzeit.Value < j.AddSeconds(-10)))
      return;
    try
    {
      File.WriteAllText(Dateiname, GetStatus());
    }
    catch (Exception)
    {
      // ServiceBase.Log.Error("WebServiceStatusFunktion write", ex);
      // Try again after waiting a while.
      Thread.Sleep(Functions.NextRandom(1000, 2000));
      File.WriteAllText(Dateiname, GetStatus());
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
      sbe.Append("No result."); // for safety
    daten["Fehler"] = sbf.Length <= 0 ? null : sbf.ToString();
    var s = sbe.ToString();
    daten["Ergebnis"] = s;
    Endzeit = DateTime.Now;
    LetzteAenderung = Endzeit.Value;
    Schreiben(true);
    return s;
  }
}
