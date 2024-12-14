// <copyright file="TableReadModel.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

/// <summary>
/// Basis-Klasse für alle Lesen-Details für Tabellen.
/// </summary>
[Serializable]
public class TableReadModel
{
  /// <summary>Gets or sets die Nummer der aktuell angezeigten Seite.</summary>
  public int? SelectedPage { get; set; } = default;

  /// <summary>Gets or sets die Anzahl der Seiten.</summary>
  public int? PageCount { get; set; } = default;

  /// <summary>Gets or sets die Anzahl der Zeilen pro Seite.</summary>
  public int? RowsPerPage { get; set; } = default;

  /// <summary>Gets or sets die Spalte, die zum Sortieren der Daten verwendet wird.</summary>
  public string SortColumn { get; set; } = default;

  /// <summary>Gets or sets die Sucheingabe.</summary>
  public string Search { get; set; } = default;
}
