// <copyright file="AncestorReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Reports;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CSBP.Services.Apis.Models;

/// <summary>Create a html ancestor report.</summary>
public partial class AncestorReport : ReportBase
{
  /// <summary>Initializes a new instance of the <see cref="AncestorReport"/> class.</summary>
  public AncestorReport()
  {
    Style = @"
* {
 font-family: Arial;
 margin: 0;
 padding 0;
}
.bold {
  font-weight: bold;
}
";
  }

  /// <summary>Gets or sets the subtitle.</summary>
  public string Subtitle { get; set; }

  /// <summary>Gets or sets the subtitle for forebears.</summary>
  public string SubtitleForebears { get; set; }

  /// <summary>Gets or sets list of descendants.</summary>
  public List<SbPerson> Descendants { get; set; }

  /// <summary>Gets or sets list of forebears.</summary>
  public List<SbPerson> Forebears { get; set; }

  /// <summary>Gets a value indicating whether all values for the report are present.</summary>
  protected override bool DataOk
  {
    get { return Descendants != null || Forebears != null; }
  }

  /// <summary>Internal generation of report.</summary>
  protected override void DoGenerate()
  {
    if (WriteList(Subtitle, Descendants))
      AddNewLine(2);
    WriteList(SubtitleForebears, Forebears);
  }

  /// <summary>Parser for ancestor formatting.</summary>
  [GeneratedRegex("^( *)([0-9\\+]+ )<b>(.*)</b> (.*)$", RegexOptions.Compiled)]
  private static partial Regex AncestorRegex();

  /// <summary>
  /// Write a list of ancestors.
  /// </summary>
  /// <param name="subtitle">Affected subtitle.</param>
  /// <param name="list">Affected list of ancestors.</param>
  /// <returns>Was anything written or not.</returns>
  private bool WriteList(string subtitle, List<SbPerson> list)
  {
    if (list == null || !list.Any())
      return false;
    if (!string.IsNullOrEmpty(subtitle))
    {
      Xml.WriteStartElement("h3");
      Xml.WriteString(subtitle);
      Xml.WriteEndElement();
      AddNewLine();
    }
    foreach (var p in list)
    {
      var ind = 1;
      var normal1 = p.Bemerkung;
      var bold = "";
      var normal2 = "";
      var m = AncestorRegex().Match(p.Bemerkung);
      if (m.Success)
      {
        ind = m.Groups[1].Length + 1;
        normal1 = m.Groups[2].Value;
        bold = m.Groups[3].Value;
        normal2 = m.Groups[4].Value;
      }
      Xml.WriteStartElement("div");
      Xml.WriteAttributeString("style", $"padding-left: {ind * 1}em; text-indent: {1 * -1}em;");
      Xml.WriteString(normal1);
      Xml.WriteStartElement("span");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteString(bold);
      Xml.WriteEndElement();
      Xml.WriteString(normal2);
      AddNewLine();
      Xml.WriteEndElement();
    }
    return true;
  }
}
