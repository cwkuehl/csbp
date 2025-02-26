// <copyright file="AddressReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Reports;

using System.Collections.Generic;
using System.Text;
using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using static CSBP.Services.Base.Functions;

/// <summary>Creates a html address report.</summary>
public partial class AddressReport : ReportBase
{
  /// <summary>Initializes a new instance of the <see cref="AddressReport"/> class.</summary>
  public AddressReport()
  {
    Style = @"
* {
 font-family: Arial;
 font-size: 10px;
 margin: 0;
 padding 0;
}
.row {
 width:100%;
}
.column1 {
 width: 50%;
 vertical-align: top;
}
.column2 {
 width: 50%;
 vertical-align: top;
}
.person {
 font-weight: bold;
 margin: 0;
 padding 0;
}
.site {
 padding-left:4px;
 margin: 0;
 padding 0;
}
";
  }

  /// <summary>Gets or sets the list of persons, sites and addresses.</summary>
  public List<AdSitz> Sites { get; set; }

  /// <summary>Internal generation of report.</summary>
  protected override void DoGenerate()
  {
    if (Sites == null)
      return;
    Xml.WriteStartElement("table");
    Xml.WriteAttributeString("class", "row");
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "column1");
    var uid = "";
    var anzahl = 0;
    foreach (var s in Sites)
    {
      if (s?.Person.Uid != uid)
        anzahl++;
      if (anzahl % 2 == 1)
      {
        if (s?.Person.Uid != uid)
        {
          Xml.WriteStartElement("p");
          Xml.WriteAttributeString("class", "person");
          Xml.WriteString(GetPersonName(s.Person));
          Xml.WriteEndElement();
        }
        Xml.WriteStartElement("p");
        Xml.WriteAttributeString("class", "site");
        Xml.WriteString(GetSitzName(s));
        Xml.WriteEndElement();
      }
      uid = s?.Person.Uid;
    }
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "column2");
    uid = "";
    anzahl = 0;
    foreach (var s in Sites)
    {
      if (s?.Person.Uid != uid)
        anzahl++;
      if (anzahl % 2 == 0)
      {
        if (s?.Person.Uid != uid)
        {
          Xml.WriteStartElement("p");
          Xml.WriteAttributeString("class", "person");
          Xml.WriteString(GetPersonName(s.Person));
          Xml.WriteEndElement();
        }
        Xml.WriteStartElement("p");
        Xml.WriteAttributeString("class", "site");
        Xml.WriteString(GetSitzName(s));
        Xml.WriteEndElement();
      }
      uid = s?.Person.Uid;
    }
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr
    Xml.WriteEndElement(); // table
  }

  /// <summary>
  /// Gets a person string.
  /// </summary>
  /// <param name="a">Affected person.</param>
  /// <returns>Person string.</returns>
  private static string GetPersonName(AdPerson a)
  {
    if (a == null)
      return "";
    var sb = new StringBuilder();
    sb.Append(" ", a.Praedikat).Append(" ", a.Name1).Append(" ", a.Name2).Append(", ", a.Vorname);
    sb.Append(" (", a.Titel, ")").Append(", ", Functions.ToString(a.Geburt));
    return sb.ToString();
  }

  private static string GetSitzName(AdSitz s)
  {
    var sb = new StringBuilder();
    sb.Append(null, s.Name).Append(':');
    if (s.Address != null)
    {
      var a = s.Address;
      if (a.Staat != "D")
      {
        sb.Append(" ", a.Staat, "-");
      }
      sb.Append(" ", a.Strasse);
      sb.Append(" ", s.Postfach);
      sb.Append(" ", a.HausNr);
      sb.Append(" ", a.Plz);
      sb.Append(" ", a.Ort);
    }
    sb.Append(" ", s.Telefon);
    sb.Append(" ", s.Fax);
    sb.Append(" ", s.Mobil);
    sb.Append(" ", s.Email);
    sb.Append(" ", s.Homepage);
    sb.Append(" ", s.Bemerkung);
    return sb.ToString();
  }
}
