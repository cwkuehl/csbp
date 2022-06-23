// <copyright file="CashReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Reports;

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

/// <summary>Creates a html cash report.</summary>
public partial class CashReport : ReportBase
{
  /// <summary>Initializes a new instance of the <see cref="CashReport"/> class.</summary>
  public CashReport()
  {
    Style = @"
.thead1 {
  border-right: 0px solid black;
  border-bottom: 1px solid black;
}
.thead {
  border-bottom: 1px solid black;
}
.tfoot {
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

  /// <summary>Gets or sets a value indicating whether there a monthly periods.</summary>
  public bool Monatlich { get; set; }

  /// <summary>Gets or sets the from date.</summary>
  public DateTime? From { get; set; }

  /// <summary>Gets or sets the to date.</summary>
  public DateTime? To { get; set; }

  /// <summary>Gets or sets the report title.</summary>
  public string Titel { get; set; }

  /// <summary>Gets or sets the balance of the previous period.</summary>
  public decimal Vortrag { get; set; }

  /// <summary>Gets or sets the revenues.</summary>
  public decimal Einnahmen { get; set; }

  /// <summary>Gets or sets the expenses.</summary>
  public decimal Ausgaben { get; set; }

  /// <summary>Gets or sets the balance.</summary>
  public decimal Saldo { get; set; }

  /// <summary>Gets or sets the list of accounts.</summary>
  public List<HhKonto> Kliste { get; set; }

  /// <summary>Gets or sets the profit loss balance.</summary>
  public List<HhBilanz> Gvliste { get; set; }

  /// <summary>Gets or sets the expenses bookings.</summary>
  public List<HhBuchung> BlisteA { get; set; }

  /// <summary>Gets or sets the revenue bookings.</summary>
  public List<HhBuchung> BlisteE { get; set; }

  /// <summary>Gets or sets the bookings.</summary>
  public List<HhBuchung> Bliste { get; set; }

  /// <summary>Gets a value indicating whether all values for the report are present.</summary>
  protected override bool DataOk
  {
    get
    {
      return From.HasValue && To.HasValue && !string.IsNullOrEmpty(Titel) && Kliste != null && Gvliste != null
        && BlisteA != null && BlisteE != null && Bliste != null;
    }
  }

  /// <summary>Internal generation of report.</summary>
  protected override void DoGenerate()
  {
    // xml.WriteStartElement("h2");
    // xml.WriteString(title);
    // xml.WriteEndElement();
    Xml.WriteStartElement("table");
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    //// xml.WriteAttributeString("class", "thead1 alignleft");
    //// xml.WriteAttributeString("colspan", "2");
    Xml.WriteString(M0(HH076));
    Xml.WriteEndElement(); // td
    Xml.WriteStartElement("td");
    Xml.WriteString(Functions.ToString(From.Value.AddDays(-1)));
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "alignright");
    Xml.WriteString(Functions.ToString(Vortrag, 2));
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("colspan", "2");
    Xml.WriteString(M0(HH065));
    Xml.WriteEndElement(); // td
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "alignright");
    Xml.WriteString(Functions.ToString(Einnahmen, 2));
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "thead");
    Xml.WriteAttributeString("colspan", "2");
    Xml.WriteString(M0(HH066));
    Xml.WriteEndElement(); // td
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "thead alignright");
    Xml.WriteString(Functions.ToString(Ausgaben, 2));
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "bold tfoot");
    Xml.WriteString(M0(HH067));
    Xml.WriteEndElement(); // td
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "bold");
    Xml.WriteString(Functions.ToString(To.Value));
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "alignright bold");
    Xml.WriteString(Functions.ToString(Saldo, 2));
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr

    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    //// xml.WriteRaw("&nbsp;");
    AddNewLine(2);
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr
    var bestand = 0m;
    foreach (var k in Kliste)
    {
      bestand += k.EBetrag;
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("colspan", "2");
      Xml.WriteString(HH072(k.Name));
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "alignright");
      Xml.WriteString(Functions.ToString(k.EBetrag, 2));
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
    }
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "bold tfoot");
    Xml.WriteString(M0(HH068));
    Xml.WriteEndElement(); // td
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "bold tfoot");
    Xml.WriteString(Functions.ToString(To.Value));
    Xml.WriteEndElement();
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("class", "alignright bold tfoot");
    Xml.WriteString(Functions.ToString(bestand, 2));
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr

    // Vermerk
    Xml.WriteStartElement("tr");
    Xml.WriteStartElement("td");
    Xml.WriteAttributeString("colspan", "4");
    AddNewLine(2);
    Xml.WriteString(HH063(Titel, From.Value.Year));
    AddNewLine();
    Xml.WriteString(M0(HH064));
    AddNewLine(4);
    Xml.WriteEndElement(); // td
    Xml.WriteEndElement(); // tr

    // Unterschriften
    for (var i = 0; i < 2; i++)
    {
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "tfoot");
      Xml.WriteAttributeString("colspan", "2");
      Xml.WriteString(M0(HH077));
      if (i == 0)
        AddNewLine(4);
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "tfoot");
      Xml.WriteAttributeString("colspan", "2");
      Xml.WriteString(M0(HH069));
      if (i == 0)
        AddNewLine(4);
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
    }
    Xml.WriteEndElement(); // table

    // Einnahmen + Ausgaben
    AddNewLine(2);
    EinnahmenAusgaben(To.Value, Gvliste);

    // Aufschlüsselung
    AddNewLine(2);
    Aufschluesselung(To.Value, BlisteE, BlisteA);

    // Abrechnung
    AddNewLine(2);
    Abrechnung(From.Value, To.Value, Kliste, Bliste);
  }

  /// <summary>
  /// Write revenues and expenses.
  /// </summary>
  /// <param name="to">Affected to date.</param>
  /// <param name="gvliste">Affected profit loss balances.</param>
  private void EinnahmenAusgaben(DateTime to, List<HhBilanz> gvliste)
  {
    Xml.WriteStartElement("table");
    for (int i = 0; i < 2; i++)
    {
      var einaus = M0(i == 0 ? HH065 : HH066);
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteAttributeString("colspan", "2");
      Xml.WriteString(einaus);
      AddNewLine();
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
      var summe = 0m;
      foreach (var b in gvliste)
      {
        if (b.AccountName != null && ((i == 0 && b.AccountType <= 0) || (i == 1 && b.AccountType > 0)))
        {
          Xml.WriteStartElement("tr");
          Xml.WriteStartElement("td");
          Xml.WriteString(b.AccountName);
          Xml.WriteEndElement(); // td
          Xml.WriteStartElement("td");
          Xml.WriteAttributeString("class", "alignright");
          Xml.WriteString(Functions.ToString(b.AccountEsum, 2));
          Xml.WriteEndElement(); // td
          Xml.WriteEndElement(); // tr
          summe += b.AccountEsum;
        }
      }
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold tfoot");
      Xml.WriteString(HH073(einaus, to));
      if (i == 0)
        AddNewLine(2);
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold alignright tfoot");
      Xml.WriteString(Functions.ToString(summe, 2));
      if (i == 0)
        AddNewLine(2);
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
    }
    Xml.WriteEndElement(); // table
  }

  /// <summary>
  /// Writes bookings details.
  /// </summary>
  /// <param name="to">Affected to date.</param>
  /// <param name="blisteE">Affected revenue bookings.</param>
  /// <param name="blisteA">Affected expense bookings.</param>
  private void Aufschluesselung(DateTime to, List<HhBuchung> blisteE, List<HhBuchung> blisteA)
  {
    Xml.WriteStartElement("table");
    for (int i = 0; i < 2; i++)
    {
      var einaus = M0(i == 0 ? HH065 : HH066);
      var bliste = i == 0 ? blisteE : blisteA;
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteAttributeString("colspan", "3");
      Xml.WriteString(HH070(einaus));
      AddNewLine();
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
      var summe = 0m;
      foreach (var b in bliste)
      {
        Xml.WriteStartElement("tr");
        Xml.WriteStartElement("td");
        Xml.WriteString(b.DebitName);
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        Xml.WriteString(b.CreditName);
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("class", "alignright");
        Xml.WriteString(Functions.ToString(b.EBetrag, 2));
        Xml.WriteEndElement(); // td
        Xml.WriteEndElement(); // tr
        Xml.WriteStartElement("tr");
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("colspan", "3");
        Xml.WriteString(b.Beleg_Nr);
        Xml.WriteEndElement(); // td
        Xml.WriteEndElement(); // tr
        summe += b.EBetrag;
      }
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold tfoot");
      Xml.WriteAttributeString("colspan", "2");
      Xml.WriteString(HH073(einaus, to));
      if (i == 0)
        AddNewLine(2);
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold alignright tfoot");
      Xml.WriteString(Functions.ToString(summe, 2));
      if (i == 0)
        AddNewLine(2);
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
    }
    Xml.WriteEndElement(); // table
  }

  /// <summary>
  /// Writes settlement.
  /// </summary>
  /// <param name="from">Affected from date.</param>
  /// <param name="to">Affected to date.</param>
  /// <param name="kliste">Affected accounts.</param>
  /// <param name="bliste">Affected bookings.</param>
  private void Abrechnung(DateTime from, DateTime to, List<HhKonto> kliste, List<HhBuchung> bliste)
  {
    var newLine = false;
    Xml.WriteStartElement("table");
    foreach (var k in kliste)
    {
      var bliste2 = bliste.Where(a => a.Soll_Konto_Uid == k.Uid || a.Haben_Konto_Uid == k.Uid);
      if (!bliste2.Any())
        continue;
      var summe = k.Betrag;
      if (newLine)
      {
        Xml.WriteStartElement("tr");
        Xml.WriteStartElement("td");
        AddNewLine(3);
        Xml.WriteEndElement(); // td
        Xml.WriteEndElement(); // tr
      }
      else
        newLine = true;
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteAttributeString("colspan", "4");
      Xml.WriteString(HH071(k.Name, from));
      AddNewLine();
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold alignright");
      Xml.WriteString(Functions.ToString(summe, 2));
      AddNewLine();
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr

      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteString(M0(HH077));
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteString(M0(HH081));
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteString(M0(HH082));
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold alignright");
      Xml.WriteString(M0(HH078));
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold alignright");
      Xml.WriteString(M0(HH079));
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteString(M0(HH080));
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
      foreach (var b in bliste2)
      {
        var einnahme = k.Uid == b.Soll_Konto_Uid;
        Xml.WriteStartElement("tr");
        Xml.WriteStartElement("td");
        Xml.WriteString(Functions.ToString(b.Soll_Valuta));
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        Xml.WriteString(b.Beleg_Nr);
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        Xml.WriteString(b.BText);
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("class", "alignright");
        if (einnahme)
        {
          Xml.WriteString(Functions.ToString(b.EBetrag, 2));
          summe += b.EBetrag;
        }
        else
          AddNewLine();
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        Xml.WriteAttributeString("class", "alignright");
        if (einnahme)
          AddNewLine();
        else
        {
          Xml.WriteString(Functions.ToString(b.EBetrag, 2));
          summe -= b.EBetrag;
        }
        Xml.WriteEndElement(); // td
        Xml.WriteStartElement("td");
        if (einnahme)
          Xml.WriteString(b.CreditName);
        else
          Xml.WriteString(b.DebitName);
        Xml.WriteEndElement(); // td
        Xml.WriteEndElement(); // tr
      }
      Xml.WriteStartElement("tr");
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold");
      Xml.WriteAttributeString("colspan", "4");
      Xml.WriteString(HH071(k.Name, to));
      AddNewLine(2);
      Xml.WriteEndElement(); // td
      Xml.WriteStartElement("td");
      Xml.WriteAttributeString("class", "bold alignright");
      Xml.WriteString(Functions.ToString(summe, 2));
      AddNewLine(2);
      Xml.WriteEndElement(); // td
      Xml.WriteEndElement(); // tr
      if (summe != k.EBetrag)
        throw new MessageException(HH074);
    }
  }
}
