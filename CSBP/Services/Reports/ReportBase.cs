// <copyright file="ReportBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System.IO;
using System.Text;
using System.Xml;
using CSBP.Base;

namespace CSBP.Services.Reports
{
  /// <summary>Basis-Klasse für alle Reports.</summary>
  public partial class ReportBase
  {
    /// <summary>Holt oder setzt den Wert der Überschrift.</summary>
    public string Caption { get; set; }

    /// <summary>Holt oder setzt das HTML-Stylesheet.</summary>
    protected string Style { get; set; }

    /// <summary>Holt den XML-Writer.</summary>
    protected XmlTextWriter xml { get; private set; }

    /// <summary>Holt einen Wert, der angibt, ob alle Daten für den Report vorhanden sind.</summary>
    protected virtual bool DataOk { get { return true; } }

    /// <summary>Erzeugen des Reports.</summary>
    /// <returns>Ergebnis als Bytes oder null.</returns>
    public byte[] Generate()
    {
      if (!DataOk)
        return null;
      using (var stream = new MemoryStream())
      using (var sw = new StreamWriter(stream, new UTF8Encoding(false)))
      using (xml = new XmlTextWriter(sw))
      {
        //xml.WriteStartDocument();
        xml.WriteStartElement("html");
        xml.WriteStartElement("head");
        xml.WriteStartElement("title");
        xml.WriteString(Caption);
        xml.WriteEndElement();
        xml.WriteStartElement("meta");
        xml.WriteAttributeString("charset", "UTF-8");
        xml.WriteEndElement();
        if (!string.IsNullOrEmpty(Style))
        {
          xml.WriteStartElement("style");
          xml.WriteString(Style);
          xml.WriteEndElement(); // style
        }
        xml.WriteEndElement(); // head
        xml.WriteStartElement("body");
        DoGenerate();
        xml.WriteEndElement(); // body
        xml.WriteEndElement(); // html
        //xml.WriteEndDocument();
        xml.Flush();
        return stream.ToArray();
      }
    }

    /// <summary>Internes Erzeugen des Reports muss überschrieben werden.</summary>
    protected virtual void DoGenerate()
    {
      Functions.MachNichts();
    }

    /// <summary>Erzeugen von neuen Zeilen.</summary>
    /// <param name="anzahl">Betroffene Anzahl.</param>
    protected void AddNewLine(int anzahl = 1)
    {
      for (var i = 0; i < anzahl; i++)
        xml.WriteRaw("</br>");
    }
  }
}
