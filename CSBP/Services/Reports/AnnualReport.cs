// <copyright file="AnnualReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Reports;

using System.Collections.Generic;
using System.Linq;
using CSBP.Base;
using CSBP.Services.Budget;
using static CSBP.Resources.Messages;

/// <summary>Creates a html annual report.</summary>
public partial class AnnualReport : ReportBase
{
  /// <summary>Initializes a new instance of the <see cref="AnnualReport"/> class.</summary>
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

  /// <summary>Gets or sets the opening balance.</summary>
  public List<AccountRow> Ebliste { get; set; }

  /// <summary>Gets or sets the profit loss balance.</summary>
  public List<AccountRow> Gvliste { get; set; }

  /// <summary>Gets or sets the final balance.</summary>
  public List<AccountRow> Sbliste { get; set; }

  /// <summary>Gets a value indicating whether all values for the report are present.</summary>
  protected override bool DataOk
  {
    get { return !(Ebliste == null && Gvliste == null && Sbliste == null); }
  }

  /// <summary>Internal generation of report.</summary>
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
    Xml.WriteStartElement("h2");
    Xml.WriteString(title);
    Xml.WriteEndElement();

    Xml.WriteStartElement("table");
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("th");
    Xml.WriteAttributeString("class", "thead1 alignleft");
    Xml.WriteAttributeString("colspan", "2");
    Xml.WriteString(debit.Replace("_", ""));
    Xml.WriteEndElement();
    Xml.WriteStartElement("th");
    Xml.WriteAttributeString("class", "thead alignright");
    Xml.WriteAttributeString("colspan", "2");
    Xml.WriteString(credit.Replace("_", ""));
    Xml.WriteEndElement();
    Xml.WriteEndElement(); // tr
    var sum1 = 0M;
    var sum2 = 0M;
    foreach (var r in list)
    {
      Xml.WriteStartElement("tr");
      if (string.IsNullOrEmpty(r.Nr))
      {
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("class", "td1");
        Xml.WriteAttributeString("colspan", "2");
        Xml.WriteEndElement();
      }
      else
      {
        // xml.WriteStartElement("td");
        // xml.WriteString(r.Nr);
        // xml.WriteEndElement();
        Xml.WriteStartElement("td");
        Xml.WriteString(r.Name);
        Xml.WriteEndElement();
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("class", "td1 alignright");
        Xml.WriteString(Functions.ToString(r.Value, 2));
        Xml.WriteEndElement();
      }
      if (string.IsNullOrEmpty(r.Nr2))
      {
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("colspan", "2");
        Xml.WriteEndElement();
      }
      else
      {
        // xml.WriteStartElement("td");
        // xml.WriteString(r.Nr);
        // xml.WriteEndElement();
        Xml.WriteStartElement("td");
        Xml.WriteString(r.Name2);
        Xml.WriteEndElement();
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("class", "alignright");
        Xml.WriteString(Functions.ToString(r.Value2, 2));
        Xml.WriteEndElement();
      }
      Xml.WriteEndElement(); // tr
      sum1 += r.Value ?? 0;
      sum2 += r.Value2 ?? 0;
    }
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "tfoot alignleft bold");
    Xml.WriteString(HH500_sollSumme);
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "tfoot td1 alignright bold");
    Xml.WriteString(Functions.ToString(sum1, 2));
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "tfoot alignleft bold");
    Xml.WriteString(HH500_habenSumme);
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "tfoot alignright bold");
    Xml.WriteString(Functions.ToString(sum2, 2));
    Xml.WriteEndElement();
    Xml.WriteEndElement(); // tr
    Xml.WriteEndElement(); // table
  }
}
