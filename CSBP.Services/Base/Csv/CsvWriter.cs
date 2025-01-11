// <copyright file="CsvWriter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base.Csv;

using System.Text;
using CSBP.Services.Base;

/// <summary>
/// Writer writes CSV files with Windows-1252 or UTF-8 characters.
/// </summary>
public class CsvWriter
{
  /// <summary>Interner StringBuilder für Dateiinhalt.</summary>
  private readonly StringBuilder inhalt = new();

  /// <summary>Interne Anführungszeichen.</summary>
  private readonly string af = "\"";

  /// <summary>Interner Spaltentrenner.</summary>
  private readonly string trenner = ";";

  /// <summary>Interner Zeilenumbruch.</summary>
  private readonly string crlf = Constants.CrLf;

  /// <summary>Initializes a new instance of the <see cref="CsvWriter"/> class.</summary>
  public CsvWriter()
  {
  }

  /// <summary>
  /// Anhängen einer CSV-Zeile.
  /// </summary>
  /// <param name="s">Betroffene CSV-Zeile getrennt nach Spalten.</param>
  public void AddCsvLine(string[] s)
  {
    var sb = new StringBuilder();
    if (s != null)
    {
      for (var i = 0; i < s.Length; i++)
      {
        var f = s[i];
        if (f != null)
        {
          // Spalten mit null werden ignoriert.
          if (f.Contains(trenner) || f.Contains(af) || f.Contains("\r") || f.Contains("\n"))
          {
            sb.Append(af).Append(f.Replace(af, "\"\"")).Append(af);
          }
          else
            sb.Append(f);
          if (i < s.Length - 1)
            sb.Append(trenner);
        }
      }
    }
    inhalt.Append(sb).Append(crlf);
  }

  /// <summary>
  /// Schreiben der CSV-Zeilen als Datei.
  /// </summary>
  /// <param name="datei">Betroffener Dateiname inkl. Pfad.</param>
  public void WriteFile(string datei)
  {
    File.WriteAllText(datei, inhalt.ToString(), Encoding.UTF8);
  }

  /// <summary>
  /// Schreiben der CSV-Zeilen als Datei.
  /// </summary>
  /// <returns>CSV-Datei als String.</returns>
  public string GetContent()
  {
    return inhalt.ToString();
  }
}
