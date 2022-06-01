// <copyright file="AncestorReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CSBP.Apis.Models;
using CSBP.Base;
using static CSBP.Base.Functions;

namespace CSBP.Services.Reports
{
  /// <summary>Klasse für die Erstellung eines Ahnen-Berichts.</summary>
  public partial class AncestorReport : ReportBase
  {
    /// <summary>Holt oder setzt den Untertitel.</summary>
    public string Undertitle { get; set; }

    /// <summary>Holt oder setzt den Untertitel für Vorfahren.</summary>
    public string UndertitleForbears { get; set; }

    /// <summary>Holt oder setzt die Liste von Nachfahren.</summary>
    public List<SbPerson> Descendants { get; set; }

    /// <summary>Holt oder setzt die Liste von Vorfahren.</summary>
    public List<SbPerson> Forbears { get; set; }

    /// <summary>Holt einen Wert, der angibt, ob alle Daten für den Report vorhanden sind.</summary>
    protected override bool DataOk
    {
      get
      {
        return Descendants != null || Forbears != null;
      }
    }

    /// <summary>Parser für Ahnen-Formatierung.</summary>
    private static readonly Regex Parser = new("^( *)([0-9\\+]+ )<b>(.*)</b> (.*)$", RegexOptions.Compiled);

    /// <summary>Konstruktor legt den Style fest.</summary>
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

    /// <summary>Internes Erzeugen des Reports.</summary>
    protected override void DoGenerate()
    {
      if (WriteList(Undertitle, Descendants))
        AddNewLine(2);
      WriteList(UndertitleForbears, Forbears);
    }

    private bool WriteList(string undertitle, List<SbPerson> list)
    {
      if (list == null || !list.Any())
        return false;
      if (!string.IsNullOrEmpty(undertitle))
      {
        xml.WriteStartElement("h3");
        xml.WriteString(undertitle);
        xml.WriteEndElement();
        AddNewLine();
      }
      foreach (var p in list)
      {
        var ind = 1;
        var normal1 = p.Bemerkung;
        var bold = "";
        var normal2 = "";
        var m = Parser.Match(p.Bemerkung);
        if (m.Success)
        {
          ind = m.Groups[1].Length + 1;
          normal1 = m.Groups[2].Value;
          bold = m.Groups[3].Value;
          normal2 = m.Groups[4].Value;
        }
        xml.WriteStartElement("div");
        xml.WriteAttributeString("style", $"padding-left: {ind * 1}em; text-indent: {-1 * 1}em;");
        xml.WriteString(normal1);
        xml.WriteStartElement("span");
        xml.WriteAttributeString("class", "bold");
        xml.WriteString(bold);
        xml.WriteEndElement();
        xml.WriteString(normal2);
        AddNewLine();
        xml.WriteEndElement();
      }
      return true;
    }

  }
}
