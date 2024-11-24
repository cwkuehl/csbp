// <copyright file="ReportBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Reports;

using System.IO;
using System.Text;
using System.Xml;
using CSBP.Services.Base;

/// <summary>Base class for html reports.</summary>
public partial class ReportBase
{
  /// <summary>Gets or sets the report caption.</summary>
  public string Caption { get; set; }

  /// <summary>Gets or sets the html stylesheet.</summary>
  protected string Style { get; set; }

  /// <summary>Gets the internal xml writer.</summary>
  protected XmlTextWriter Xml { get; private set; }

  /// <summary>Gets a value indicating whether all values for the report are present.</summary>
  protected virtual bool DataOk
  {
    get { return true; }
  }

  /// <summary>Erzeugen des Reports.</summary>
  /// <returns>Ergebnis als Bytes oder null.</returns>
  public byte[] Generate()
  {
    if (!DataOk)
      return null;
    using (var stream = new MemoryStream())
    using (var sw = new StreamWriter(stream, new UTF8Encoding(false)))
    using (Xml = new XmlTextWriter(sw))
    {
      //// xml.WriteStartDocument();
      Xml.WriteStartElement("html");
      Xml.WriteStartElement("head");
      Xml.WriteStartElement("title");
      Xml.WriteString(Caption);
      Xml.WriteEndElement();
      Xml.WriteStartElement("meta");
      Xml.WriteAttributeString("charset", "UTF-8");
      Xml.WriteEndElement();
      if (!string.IsNullOrEmpty(Style))
      {
        Xml.WriteStartElement("style");
        Xml.WriteString(Style);
        Xml.WriteEndElement(); // style
      }
      Xml.WriteEndElement(); // head
      Xml.WriteStartElement("body");
      DoGenerate();
      Xml.WriteEndElement(); // body
      Xml.WriteEndElement(); // html
      //// xml.WriteEndDocument();
      Xml.Flush();
      return stream.ToArray();
    }
  }

  /// <summary>Internal generation of report has to be overridden.</summary>
  protected virtual void DoGenerate()
  {
    Functions.MachNichts();
  }

  /// <summary>Erzeugen von neuen Zeilen.</summary>
  /// <param name="anzahl">Betroffene Anzahl.</param>
  protected void AddNewLine(int anzahl = 1)
  {
    for (var i = 0; i < anzahl; i++)
      Xml.WriteRaw("</br>");
  }
}
