// <copyright file="CashReport.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using CSBP.Apis.Models;
using CSBP.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

namespace CSBP.Services.Reports
{
  /// <summary>Klasse für die Erstellung eines Kassen-Berichts.</summary>
  public partial class CashReport : ReportBase
  {
    /// <summary>Holt einen Wert, der angibt, ob der Bericht Monate enthalten soll.</summary>
    public bool Monatlich { get; set; }

    /// <summary>Holt oder setzt das Anfangsdatum.</summary>
    public DateTime? From { get; set; }

    /// <summary>Holt oder setzt das Enddatum.</summary>
    public DateTime? To { get; set; }

    /// <summary>Holt oder setzt das Enddatum.</summary>
    public string Titel { get; set; }

    /// <summary>Holt oder setzt den Vortrag aus dem Vorjahr.</summary>
    public decimal Vortrag { get; set; }

    /// <summary>Holt oder setzt die Einnahmen.</summary>
    public decimal Einnahmen { get; set; }

    /// <summary>Holt oder setzt die Ausgaben.</summary>
    public decimal Ausgaben { get; set; }

    /// <summary>Holt oder setzt den Saldo.</summary>
    public decimal Saldo { get; set; }

    /// <summary>Holt oder setzt die Konto.</summary>
    public List<HhKonto> Kliste { get; set; }

    /// <summary>Holt oder setzt die Gewinn+Verlust-Rechnung.</summary>
    public List<HhBilanz> Gvliste { get; set; }

    /// <summary>Holt oder setzt die Ausgabebuchungen.</summary>
    public List<HhBuchung> BlisteA { get; set; }

    /// <summary>Holt oder setzt die Einnahmebuchungen.</summary>
    public List<HhBuchung> BlisteE { get; set; }

    /// <summary>Holt oder setzt die Buchungen.</summary>
    public List<HhBuchung> Bliste { get; set; }

    /// <summary>Holt einen Wert, der angibt, ob alle Daten für den Report vorhanden sind.</summary>
    protected override bool DataOk
    {
      get
      {
        return From.HasValue && To.HasValue && !string.IsNullOrEmpty(Titel) && Kliste != null && Gvliste != null
          && BlisteA != null && BlisteE != null && Bliste != null;
      }
    }

    /// <summary>Konstruktor legt den Style fest.</summary>
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

    /// <summary>Internes Erzeugen des Reports.</summary>
    protected override void DoGenerate()
    {
      // xml.WriteStartElement("h2");
      // xml.WriteString(title);
      // xml.WriteEndElement();

      xml.WriteStartElement("table");
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      //xml.WriteAttributeString("class", "thead1 alignleft");
      //xml.WriteAttributeString("colspan", "2");
      xml.WriteString(M0(HH076));
      xml.WriteEndElement(); // td
      xml.WriteStartElement("td");
      xml.WriteString(Functions.ToString(From.Value.AddDays(-1)));
      xml.WriteEndElement();
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "alignright");
      xml.WriteString(Functions.ToString(Vortrag, 2));
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      xml.WriteAttributeString("colspan", "2");
      xml.WriteString(M0(HH065));
      xml.WriteEndElement(); // td
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "alignright");
      xml.WriteString(Functions.ToString(Einnahmen, 2));
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "thead");
      xml.WriteAttributeString("colspan", "2");
      xml.WriteString(M0(HH066));
      xml.WriteEndElement(); // td
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "thead alignright");
      xml.WriteString(Functions.ToString(Ausgaben, 2));
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "bold tfoot");
      xml.WriteString(M0(HH067));
      xml.WriteEndElement(); // td
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "bold");
      xml.WriteString(Functions.ToString(To.Value));
      xml.WriteEndElement();
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "alignright bold");
      xml.WriteString(Functions.ToString(Saldo, 2));
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr

      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      //xml.WriteRaw("&nbsp;");
      AddNewLine(2);
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr
      var bestand = 0m;
      foreach (var k in Kliste)
      {
        bestand += k.EBetrag;
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("colspan", "2");
        xml.WriteString(HH072(k.Name));
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "alignright");
        xml.WriteString(Functions.ToString(k.EBetrag, 2));
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
      }
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "bold tfoot");
      xml.WriteString(M0(HH068));
      xml.WriteEndElement(); // td
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "bold tfoot");
      xml.WriteString(Functions.ToString(To.Value));
      xml.WriteEndElement();
      xml.WriteStartElement("td");
      xml.WriteAttributeString("class", "alignright bold tfoot");
      xml.WriteString(Functions.ToString(bestand, 2));
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr

      // Vermerk
      xml.WriteStartElement("tr");
      xml.WriteStartElement("td");
      xml.WriteAttributeString("colspan", "4");
      AddNewLine(2);
      xml.WriteString(HH063(Titel, From.Value.Year));
      AddNewLine();
      xml.WriteString(M0(HH064));
      AddNewLine(4);
      xml.WriteEndElement(); // td
      xml.WriteEndElement(); // tr

      // Unterschriften
      for (var i = 0; i < 2; i++)
      {
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "tfoot");
        xml.WriteAttributeString("colspan", "2");
        xml.WriteString(M0(HH077));
        if (i == 0)
          AddNewLine(4);
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "tfoot");
        xml.WriteAttributeString("colspan", "2");
        xml.WriteString(M0(HH069));
        if (i == 0)
          AddNewLine(4);
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
      }
      xml.WriteEndElement(); // table

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

    private void EinnahmenAusgaben(DateTime to, List<HhBilanz> gvliste)
    {
      xml.WriteStartElement("table");
      for (int i = 0; i < 2; i++)
      {
        var einaus = M0(i == 0 ? HH065 : HH066);
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteAttributeString("colspan", "2");
        xml.WriteString(einaus);
        AddNewLine();
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
        var summe = 0m;
        foreach (var b in gvliste)
        {
          if (b.AccountName != null && ((i == 0 && b.AccountType <= 0) || (i == 1 && b.AccountType > 0)))
          {
            xml.WriteStartElement("tr");
            xml.WriteStartElement("td");
            xml.WriteString(b.AccountName);
            xml.WriteEndElement(); // td
            xml.WriteStartElement("td");
            xml.WriteAttributeString("class", "alignright");
            xml.WriteString(Functions.ToString(b.AccountEsum, 2));
            xml.WriteEndElement(); // td
            xml.WriteEndElement(); // tr
            summe += b.AccountEsum;
          }
        }
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold tfoot");
        xml.WriteString(HH073(einaus, to));
        if (i == 0)
          AddNewLine(2);
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold alignright tfoot");
        xml.WriteString(Functions.ToString(summe, 2));
        if (i == 0)
          AddNewLine(2);
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
      }
      xml.WriteEndElement(); // table
    }

    private void Aufschluesselung(DateTime to, List<HhBuchung> blisteE, List<HhBuchung> blisteA)
    {
      xml.WriteStartElement("table");
      for (int i = 0; i < 2; i++)
      {
        var einaus = M0(i == 0 ? HH065 : HH066);
        var bliste = i == 0 ? blisteE : blisteA;
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteAttributeString("colspan", "3");
        xml.WriteString(HH070(einaus));
        AddNewLine();
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
        var summe = 0m;
        foreach (var b in bliste)
        {
          xml.WriteStartElement("tr");
          xml.WriteStartElement("td");
          xml.WriteString(b.DebitName);
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          xml.WriteString(b.CreditName);
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          xml.WriteAttributeString("class", "alignright");
          xml.WriteString(Functions.ToString(b.EBetrag, 2));
          xml.WriteEndElement(); // td
          xml.WriteEndElement(); // tr
          xml.WriteStartElement("tr");
          xml.WriteStartElement("td");
          xml.WriteAttributeString("colspan", "3");
          xml.WriteString(b.Beleg_Nr);
          xml.WriteEndElement(); // td
          xml.WriteEndElement(); // tr
          summe += b.EBetrag;
        }
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold tfoot");
        xml.WriteAttributeString("colspan", "2");
        xml.WriteString(HH073(einaus, to));
        if (i == 0)
          AddNewLine(2);
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold alignright tfoot");
        xml.WriteString(Functions.ToString(summe, 2));
        if (i == 0)
          AddNewLine(2);
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
      }
      xml.WriteEndElement(); // table
    }

    private void Abrechnung(DateTime from, DateTime to, List<HhKonto> kliste, List<HhBuchung> bliste)
    {
      var newLine = false;
      xml.WriteStartElement("table");
      foreach (var k in kliste)
      {
        var bliste2 = bliste.Where(a => a.Soll_Konto_Uid == k.Uid || a.Haben_Konto_Uid == k.Uid);
        if (!bliste2.Any())
          continue;
        var summe = k.Betrag;
        if (newLine)
        {
          xml.WriteStartElement("tr");
          xml.WriteStartElement("td");
          AddNewLine(3);
          xml.WriteEndElement(); // td
          xml.WriteEndElement(); // tr
        }
        else
          newLine = true;
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteAttributeString("colspan", "4");
        xml.WriteString(HH071(k.Name, from));
        AddNewLine();
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold alignright");
        xml.WriteString(Functions.ToString(summe, 2));
        AddNewLine();
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr

        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteString(M0(HH077));
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteString(M0(HH081));
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteString(M0(HH082));
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold alignright");
        xml.WriteString(M0(HH078));
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold alignright");
        xml.WriteString(M0(HH079));
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteString(M0(HH080));
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
        foreach (var b in bliste2)
        {
          var einnahme = k.Uid == b.Soll_Konto_Uid;
          xml.WriteStartElement("tr");
          xml.WriteStartElement("td");
          xml.WriteString(Functions.ToString(b.Soll_Valuta));
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          xml.WriteString(b.Beleg_Nr);
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          xml.WriteString(b.BText);
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          xml.WriteAttributeString("class", "alignright");
          if (einnahme)
          {
            xml.WriteString(Functions.ToString(b.EBetrag, 2));
            summe += b.EBetrag;
          }
          else
            AddNewLine();
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          xml.WriteAttributeString("class", "alignright");
          if (einnahme)
            AddNewLine();
          else
          {
            xml.WriteString(Functions.ToString(b.EBetrag, 2));
            summe -= b.EBetrag;
          }
          xml.WriteEndElement(); // td
          xml.WriteStartElement("td");
          if (einnahme)
            xml.WriteString(b.CreditName);
          else
            xml.WriteString(b.DebitName);
          xml.WriteEndElement(); // td
          xml.WriteEndElement(); // tr
        }
        xml.WriteStartElement("tr");
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold");
        xml.WriteAttributeString("colspan", "4");
        xml.WriteString(HH071(k.Name, to));
        AddNewLine(2);
        xml.WriteEndElement(); // td
        xml.WriteStartElement("td");
        xml.WriteAttributeString("class", "bold alignright");
        xml.WriteString(Functions.ToString(summe, 2));
        AddNewLine(2);
        xml.WriteEndElement(); // td
        xml.WriteEndElement(); // tr
        if (summe != k.EBetrag)
          throw new MessageException(HH074);
      }
    }
  }
}
