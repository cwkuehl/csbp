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
    var index = 0;
    while (index < header.Count)
    {
      ColumnIndex.Add(index, index);
      index++;
    }
    index = 0;
    foreach (var c in this.Columns)
    {
      var i = header.IndexOf(c);
      if (i < 0)
        throw new MessageException(Message.New("00003Spalte {0} fehlt.", c));
      var j = ColumnIndex[index];
      if (i != j)
      {
        // Spaltenindices tauschen.
        ColumnIndex[index] = i;
        ColumnIndex[i] = j;
      }
      index++;
    }
  }

  /// <summary>Gets or sets the list of columns.</summary>
  private List<string> Columns { get; set; }

  /// <summary>Gets or sets the list of columns.</summary>
  private Dictionary<int, int> ColumnIndex { get; set; } = new();

  /// <summary>
  /// Lesen der nächsten Zeile und Sortieren der Spalten.
  /// </summary>
  /// <returns>Werte der nächsten CSV-Zeile, sortiert nach Spalten.</returns>
  public List<string> GetLineInColums()
  {
    var line0 = GetLine();
    if (line0 == null)
      return null;
    var line = ParseCsvLine(line0);
    if (line.Length != ColumnIndex.Count)
      throw new MessageException(Message.New("00004Zeile zu kurz."));
    var list = new List<string>();
    foreach (var i in ColumnIndex.Values)
      list.Add(line[i]);
    return list;
  }
}
