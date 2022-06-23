// <copyright file="TableReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Reports;

using System.Collections.Generic;

/// <summary>Creates a html table report.</summary>
public partial class TableReport : ReportBase
{
  /// <summary>Initializes a new instance of the <see cref="TableReport"/> class.
  /// Sets styles.</summary>
  public TableReport()
  {
    Style = @"
* {
 font-family: Arial;
 font-size: 10px;
 margin: 0;
 padding 0;
}
table, th, td {
  border: 1px solid black;
  border-collapse: collapse;
}
";
  }

  /// <summary>Gets or sets the value of lines.</summary>
  public List<List<string>> Lines { get; set; }

  /// <summary>Internal generation of report.</summary>
  protected override void DoGenerate()
  {
    if (Lines == null)
      return;
    Xml.WriteStartElement("table");
    foreach (var line in Lines)
    {
      Xml.WriteStartElement("tr");
      foreach (var c in line)
      {
        Xml.WriteStartElement("td");
        Xml.WriteString(c ?? "");
        Xml.WriteEndElement(); // td
      }
      Xml.WriteEndElement(); // tr
    }
    Xml.WriteEndElement(); // table
  }
}
