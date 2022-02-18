// <copyright file="AnnualReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using CSBP.Base;
using CSBP.Services.Budget;
using static CSBP.Resources.Messages;

namespace CSBP.Services.Reports
{
  /// <summary>Klasse für die Erstellung eines Jahres-Berichts.</summary>
  public partial class AnnualReport : ReportBase
  {
    /// <summary>Holt oder setzt die Eröffnungsbilanz.</summary>
    public List<AccountRow> Ebliste { get; set; }

    /// <summary>Holt oder setzt die Gewinn+Verlust-Rechnung.</summary>
    public List<AccountRow> Gvliste { get; set; }

    /// <summary>Holt oder setzt die Schlussbilanz.</summary>
    public List<AccountRow> Sbliste { get; set; }

    /// <summary>Holt einen Wert, der angibt, ob alle Daten für den Report vorhanden sind.</summary>
    protected override bool DataOk
    {
      get
      {
        return !(Ebliste == null && Gvliste == null && Sbliste == null);
      }
    }

    /// <summary>Konstruktor legt den Style fest.</summary>
    public AnnualReport()
    {
      Style = @"
th.thead1 {
  border-right: 0px solid black;
  border-bottom: 1px solid black;
}
th.thead {
  border-bottom: 1px solid black;
}
td.tfoot {
  border-top: 1px solid black;
}
td, th {
  padding: 2px 8px 2px 8px;
}
td.td1 {
  border-right: 1px solid black;
}
table {
  border-collapse: collapse;
  border-left: 0;
  #margin: 0;
}
.alignleft {
  text-align: left;
}
.alignright {
  text-align: right;
}
.bold {
  font-weight: bold;
}
";
    }

    /// <summary>Internes Erzeugen des Reports.</summary>
    protected override void DoGenerate()
    {
      if (Ebliste != null && Ebliste.Any())
        BalanceAccount(HH500_title_EB, HH500_soll_EB, HH500_haben_EB, Ebliste);
      if (Gvliste != null && Gvliste.Any())
        BalanceAccount(HH500_title_GV, HH500_soll_GV, HH500_haben_GV, Gvliste);
      if (Sbliste != null && Sbliste.Any())
        BalanceAccount(HH500_title_SB, HH500_soll_EB, HH500_haben_EB, Sbliste);
    }

    private void BalanceAccount(string title, string debit, string credit, List<AccountRow> list)
    {
      xml.WriteStartElement("h2");
      xml.WriteString(title);
      xml.WriteEndElement();

      xml.WriteStartElement("table");
      xml.WriteStartElement("tr");
      xml.WriteStartElement("th");
      xml.WriteAttributeString("class", "thead1 alignleft");
      xml.WriteAttributeString("colspan", "2");
      xml.WriteString(debit.Replace("_", ""));
      xml.WriteEndElement();
      xml.WriteStartElement("th");
      xml.WriteAttributeString("class", "thead alignright");
      xml.WriteAttributeString("colspan", "2");
      xml.WriteString(credit.Replace("_", ""));
      xml.WriteEndElement();
      xml.WriteEndElement(); // tr
      var sum1 = 0M;
      var sum2 = 0M;
      foreach (var r in list)
      {
        xml.WriteStartElement("tr");
        if (string.IsNullOrEmpty(r.Nr))
        {
          xml.WriteStartElement("td");
          xml.WriteAttributeString("class", "td1");
          xml.WriteAttributeString("colspan", "2");
          xml.WriteEndElement();
        }
        else
        {
          //xml.WriteStartElement("td");
          //xml.WriteString(r.Nr);
          //xml.WriteEndElement();
          xml.WriteStartElement("td");
          xml.WriteString(r.Name);
          xml.WriteEndElement();
          xml.WriteStartElement("td");
          xml.WriteAttributeString("class", "td1 alignright");
          xml.WriteString(Functions.ToString(r.Value, 2));
          xml.WriteEndElement();
        }
        if (string.IsNullOrEmpty(r.Nr2))
        {
          xml.WriteStartElement("td");
          xml.WriteAttributeString("colspan", "2");
          xml.WriteEndElement();
        }
        else
        {
          //xml.WriteStartElement("td");
          //xml.WriteString(r.Nr);
          //xml.WriteEndElement();
          xml.WriteStartElement("td");
          xml.WriteString(r.Name2);
          xml.WriteEndElement();
          xml.WriteStartElement("td");
          xml.WriteAttributeString("class", "alignright");
          xml.WriteString(Functions.ToString(r.Value2, 2));
          xml.WriteEndElement();
        }
        xml.WriteEndElement(); // tr
        sum1 += r.Value ?? 0;
        sum2 += r.Value2 ?? 0;
      }
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "tfoot alignleft bold");
      xml.WriteString(HH500_sollSumme);
      xml.WriteEndElement();
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "tfoot td1 alignright bold");
      xml.WriteString(Functions.ToString(sum1, 2));
      xml.WriteEndElement();
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "tfoot alignleft bold");
      xml.WriteString(HH500_habenSumme);
      xml.WriteEndElement();
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "tfoot alignright bold");
      xml.WriteString(Functions.ToString(sum2, 2));
      xml.WriteEndElement();
      xml.WriteEndElement(); // tr
      xml.WriteEndElement(); // table
    }
  }
}
