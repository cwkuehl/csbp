// <copyright file="CsvColumnReader.cs" company="LDI">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System.Collections.Generic;

/// <summary>
/// Zum Lesen Csv-Datei mit vorgegebenen Spalten.
/// </summary>
public class CsvColumnReader : CsvReader
{
  /// <summary>
  /// Initializes a new instance of the <see cref="CsvColumnReader" /> class.
  /// </summary>
  /// <param name="filename">Betroffene Datei.</param>
  /// <param name="columns">Betroffene Spaltennamen, die vorhanden sein müssen. Diese Reihenfolge wird bei den Zeilen zurueckgegeben.</param>
  public CsvColumnReader(string filename, List<string> columns)
    : base(filename ?? "")
  {
    if (columns == null || columns.Count <= 0)
      throw new MessageException(Message.New("00001Keine Spalten angegeben."));
    this.Columns = columns.Select(a => a.ToLowerInvariant()).ToList();
    var header = ParseCsvLine(GetLine()).Select(a => a.ToLowerInvariant()).ToList();
    if (header.Count <= 0)
      throw new MessageException(Message.New("00002Datei ist leer."));
    ColumnIndex = new int[header.Count];
    for (var i = 0; i < header.Count; i++)
      ColumnIndex[i] = i;
    var index = 0;
    foreach (var c in this.Columns)
    {
      var i = header.IndexOf(c);
      if (i < 0)
        throw new MessageException(Message.New("00003Spalte {0} fehlt.", c));
      var j = ColumnIndex[index];
      if (i != j)
      {
        // Spaltenindizes tauschen.
        ColumnIndex[index] = i;
        ColumnIndex[i] = j;
      }
      index++;
    }
  }

  /// <summary>Gets or sets the list of columns.</summary>
  private List<string> Columns { get; set; }

  /// <summary>Gets or sets the list of columns.</summary>
  private int[] ColumnIndex { get; set; } = [];

  /// <summary>
  /// Lesen der nächsten Zeile und Sortieren der Spalten.
  /// </summary>
  /// <returns>Werte der nächsten CSV-Zeile, sortiert nach Spalten.</returns>
  public string[] GetLineInColums()
  {
    var line0 = GetLine();
    if (line0 == null)
      return null;
    var line = ParseCsvLine(line0);
    if (line.Length != ColumnIndex.Length)
      throw new MessageException(Message.New("00004Zeile zu kurz."));
    for (var i = 0; i < ColumnIndex.Length - 1; i++)
    {
      var j = ColumnIndex[i];
      if (i != j)
      {
        // Vertauschen der Spalteninhalte.
        var t = line[i];
        line[i] = line[j];
        line[j] = t;
      }
    }
    return line;
  }
}
