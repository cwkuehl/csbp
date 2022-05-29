// <copyright file="PnfChart.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSBP.Apis.Models;
using CSBP.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

namespace CSBP.Services.Pnf
{
  /// <summary>
  /// Point and Figure-Chart.
  /// Eine X-Box(aufwärts) beinhaltet Werte größer oder gleich der Bezeichnung.
  /// Eine O-Box(abwärts) beinhaltet Werte kleiner oder gleich der Bezeichnung.
  /// </summary>
  public class PnfChart
  {
    /// <summary>Bezeichnung des Charts.</summary>
    private string bezeichnung = null;
    private int methode;
    private decimal box;
    private readonly List<SoKurse> kurse = new();
    /// <summary>Box Skalierung. 0: feste Boxgröße; 1: prozentual; 2: dynamisch.</summary>
    private int skala = 1;
    /// <summary>Anzahl der Umkehr-Boxen.</summary>
    private int umkehr = 1;
    /// <summary>Handelt es sich um eine Relation?</summary>
    private bool relativ = false;
    /// <summary>Anzahl der Tage.</summary>
    private readonly int dauer = 0;
    /// <summary>Boxbezeichnung.</summary>
    private decimal bb = 0;
    /// <summary>Aktueller Kurs.</summary>
    private decimal kurs = 0;
    /// <summary>Minimum.</summary>
    private decimal min = decimal.MaxValue;
    /// <summary>Maximum.</summary>
    private decimal max = decimal.MinValue;
    /// <summary>Maximales Maximum der Säulen.</summary>
    private int posmax = 0;
    /// <summary>Boxtyp: 0 unbestimmt; 1 aufwärts (X); 2 abwärts (O).</summary>
    private int boxtyp = 0;
    /// <summary>Liste der Säulen.</summary>
    private readonly List<PnfColumn> saeulen = new();
    /// <summary>Aktuelle Säule.</summary>
    private PnfColumn saeule = null;
    /// <summary>Liste der Werte.</summary>
    private readonly List<decimal> werte = new();
    /// <summary>Liste der Trendlinien.</summary>
    private readonly List<PnfTrend> trends = new();
    /// <summary>Liste der Muster.</summary>
    private readonly List<PnfPattern> pattern = new();
    /// <summary>Zielkurs.</summary>
    private decimal ziel = 0;
    /// <summary>Stopkurs.</summary>
    private decimal stop = 0;
    /// <summary>Trend.</summary>
    private decimal trend = 0;
    /// <summary>Zur Bestimmung des Monatzeichens.</summary>
    private readonly PnfDate datumm = new();

    public PnfChart(int method, decimal box, int scale, int reversal, string desc)
    {
      methode = method;
      this.box = box;
      skala = scale;
      umkehr = reversal;
      bezeichnung = desc;
    }

    /// <summary>Hinzufügen von neuen Kursen zum PnfChart.</summary>
    /// <param name="liste">Hinzuzufügende Liste.</param>
    public void AddKurse(List<SoKurse> liste)
    {
      if (liste == null)
      {
        return;
      }
      foreach (var k in liste)
      {
        AddKurse(k);
      }
      if (liste.Count >= 2)
      {
        kurs = liste.Last().Close; // letzter Kurs
        var datum = liste.Last().Datum;
        var k1 = liste[^2].Close; // vorletzter Kurs
        var p = PnfPattern.GetMusterKurse(this, datum, kurs, k1, ziel, ziel);
        if (p != null)
        {
          pattern.Add(p);
        }
      }

      // nur das letzte Signal behalten
      if (pattern.Any())
      {
        var p = pattern.Last();
        pattern.Clear();
        pattern.Add(p);
      }

      // Werte in Kästchen umrechnen
      min = saeulen.Any() ? saeulen.Min(a => a.Min) : 0;
      max = saeulen.Any() ? saeulen.Max(a => a.Max) : 0;
      if (min == 0 || max == 0)
      {
        min = kurse.Any() ? kurse.Min(a => a.Close) : 0;
        max = kurse.Any() ? kurse.Max(a => a.Close) : 0;
      }
      int anzahl = 0;
      decimal m = min;

      werte.Clear();
      saeulen.ForEach(a => a.Ypos = 0);
      while (Functions.CompDouble4(m, max) <= 0 && saeulen.Count > 0)
      {
        werte.Add(m);
        anzahl++;
        var fm = m;
        var fanzahl = anzahl;
        saeulen.ForEach(a =>
        {
          if (a.Ypos == 0 && Functions.CompDouble4(a.Min, fm) == 0)
          {
            if (a.IsO)
            {
              a.Ypos = fanzahl + 1;
            }
            else
            {
              a.Ypos = fanzahl;
            }
          }
        });
        pattern.ForEach(a =>
        {
          if (a.Ypos == 0 && Functions.CompDouble4(a.Wert, fm) == 0)
          {
            if (a.IsO)
            {
              a.Ypos = fanzahl + 1;
            }
            else
            {
              a.Ypos = fanzahl - 2;
            }
          }
        });
        m = NextBox(m);
      }
      posmax = anzahl;

      // Stopkurs berechnen
      stop = 0;
      int xstop = -1;
      PnfColumn tief = null;
      if (saeulen.Count > 1 && saeulen.Last().IsO)
      {
        xstop = saeulen.Count - 1;
        tief = saeulen.Last();
      }
      else if (saeulen.Count > 2 && saeulen[^2].IsO)
      {
        xstop = saeulen.Count - 2;
        tief = saeulen[xstop];
      }
      trends.Clear();
      if (tief != null)
      {
        stop = tief.Min * 100m / 105m; // 5% des letzten Tiefs
        int y = 0;
        for (int i = 0; i < posmax; i++)
        {
          if (Functions.CompDouble4(stop, werte[i]) <= 0)
          {
            y = i;
            break;
          }
        }
        var ts = new PnfTrend(xstop, y, 0);
        trends.Add(ts);
      }

      trend = 0;
      if (umkehr >= 3)
      {
        GetTrendlinien(0);
        if (kurse.Count >= 2 || trends.Any())
        {
          // Trendlinien-Durchbrüche bestimmen
          kurs = kurse.Last().Close; // letzter Kurs
          var k1 = kurse[^2].Close; // vorletzter Kurs
          var datum = kurse.Last().Datum;
          PnfPattern p = PnfPattern.GetMusterTrend(this, datum, kurs, k1);
          if (p != null)
          {
            pattern.Add(p);
            for (int i = 0; i < posmax; i++)
            {
              if (Functions.CompDouble4(p.Wert, werte[i]) == 0)
              {
                if (p.IsO)
                {
                  p.Ypos = i + 1 + 1;
                }
                else
                {
                  p.Ypos = i + 1 - 2;
                }
                break;
              }
            }
          }
        }
        // Trend bestimmen
        if (trends.Any())
        {
          // Trend setzen: letzten Auf- und Abwärts-Trend suchen
          var trend1 = 0m;
          var trend2 = 0m;
          var akt = Kurs;
          PnfTrend auf = null;
          PnfTrend ab = null;
          List<PnfTrend> l = trends;
          int anzahls = saeulen.Count;
          int j = l.Count - 1;
          int yakt = -1;
          while (j >= 0 && (auf == null || ab == null))
          {
            var x = l[j];
            if (x.Xpos + x.Laenge >= anzahls)
            {
              // bis zum Ende
              if (x.Boxtyp == 1 && auf == null)
              {
                // aufwärts
                auf = x;
              }
              else if (x.Boxtyp == 2 && ab == null)
              {
                // abwärts
                ab = x;
              }
            }
            j--;
          }
          if (Functions.CompDouble4(akt, 0) > 0)
          {
            var d = Max + 1;
            var yanzahl = Werte.Count;
            for (int i = 0; i < yanzahl; i++)
            {
              if (Functions.CompDouble4(Werte[i], d) < 0 && Functions.CompDouble4(Werte[i],
                      akt) > 0)
              {
                d = Werte[i];
                yakt = i;
              }
            }
          }
          if (auf != null && yakt >= 0)
          {
            int e = auf.Ypos + auf.Laenge;
            if (yakt <= e - 1)
            {
              trend1 = -2;
            }
            else if (yakt <= e)
            {
              trend1 = -1;
            }
            else if (yakt <= e + 1)
            {
              trend1 = -0.5m;
            }
          }
          if (ab != null && yakt >= 0)
          {
            int e = ab.Ypos - ab.Laenge;
            if (yakt >= e + 1)
            {
              trend2 = 2;
            }
            else if (yakt >= e)
            {
              trend2 = 1;
            }
            else if (yakt >= e - 1)
            {
              trend2 = 0.5m;
            }
          }
          if (trend1 != 0 || trend2 != 0)
          {
            trend = trend1 + trend2;
          }
        }
      }
    }

    /// <summary>Hinzufügen von neuen Kursen zum PnfChart. Dabei werden auch Muster bzw. Signale erkannt.</summary>
    /// <param name="k0">Neue Kurse.</param>
    private void AddKurse(SoKurse k0)
    {
      if (k0 == null)
      {
        return;
      }
      if (methode == 1 && Functions.CompDouble4(k0.Close, 0) <= 0)
      {
        throw new MessageException(WP031);
      }
      else if ((methode == 2 || methode == 3) && (Functions.CompDouble4(k0.High, 0) <= 0 || Functions.CompDouble4(k0
            .Low, 0) <= 0))
      {
        throw new MessageException(WP032);
      }
      else if (methode == 4 && (Functions.CompDouble4(k0.Open, 0) <= 0 || Functions.CompDouble4(k0.High, 0) <= 0
            || Functions.CompDouble4(k0.Low, 0) <= 0 || Functions.CompDouble4(k0.Close, 0) <= 0))
      {
        throw new MessageException(WP033);
      }
      else if (methode == 5 && (Functions.CompDouble4(k0.High, 0) <= 0 || Functions.CompDouble4(k0.Low, 0) <= 0
            || Functions.CompDouble4(k0.Close, 0) <= 0))
      {
        throw new MessageException(WP034);
      }
      kurse.Add(k0);
      var k = k0.Close;
      if (bb == 0)
      {
        bb = k;
        if (skala != 1)
        {
          var x = 0m;
          var x1 = 0m;
          while (x1 < k)
          {
            x = x1;
            x1 = NextBox(x);
          }
          bb = Functions.Round(x) ?? 0;
        }
      }
      var neu = 0;
      if (boxtyp == 0)
      {
        neu = Basisalgorithmus(k, k0.Datum);
      }
      else if (methode == 4)
      {
        // OHLC-Methode
        bool ohlc;
        if (Functions.CompDouble4(k0.Close, k0.Open) > 0)
        {
          ohlc = false;
        }
        else if (Functions.CompDouble4(k0.Close, k0.Open) < 0)
        {
          ohlc = true;
        }
        else
        {
          if (Functions.CompDouble4(k0.Close, k0.Low) == 0)
          {
            ohlc = true;
          }
          else if (Functions.CompDouble4(k0.Close, k0.High) == 0)
          {
            ohlc = false;
          }
          else
          {
            var m = (k0.High + k0.Low) / 2;
            if (Functions.CompDouble4(k0.Close, m) < 0)
            {
              ohlc = true;
            }
            else if (Functions.CompDouble4(k0.Close, m) > 0)
            {
              ohlc = false;
            }
            else if (boxtyp == 1)
            {
              ohlc = true;
            }
            else
            {
              ohlc = false;
            }
          }
        }
        var neu2 = Basisalgorithmus(k0.Open, k0.Datum);
        if (neu2 != 0)
        {
          neu = neu2;
        }
        neu2 = Basisalgorithmus(ohlc ? k0.High : k0.Low, k0.Datum);
        if (neu2 != 0)
        {
          neu = neu2;
        }
        neu2 = Basisalgorithmus(ohlc ? k0.Low : k0.High, k0.Datum);
        if (neu2 != 0)
        {
          neu = neu2;
        }
        neu2 = Basisalgorithmus(k0.Close, k0.Datum);
        if (neu2 != 0)
        {
          neu = neu2;
        }
      }
      else
      {
        // Auf- oder Abwärtstrend
        if (methode == 2)
        {
          k = boxtyp == 1 ? k0.High : k0.Low;
        }
        else if (methode == 3)
        {
          k = boxtyp == 1 ? k0.Low : k0.High;
        }
        else if (methode == 5)
        {
          k = (k0.High + k0.Low + k0.Close) / 3;
        }
        neu = Basisalgorithmus(k, k0.Datum);
        if (methode == 2 && neu != 1)
        {
          k = boxtyp == 1 ? k0.Low : k0.High;
          neu = Basisalgorithmus(k, k0.Datum);
        }
        else if (methode == 3 && neu != -1)
        {
          k = boxtyp == 1 ? k0.High : k0.Low;
          neu = Basisalgorithmus(k, k0.Datum);
        }
      }
      min = Math.Min(min, k);
      max = Math.Max(max, k);
      if (neu != 0)
      {
        var p = PnfPattern.GetMuster(this, k0.Datum, 20);
        if (p != null)
        {
          pattern.Add(p);
        }
      }
    }

    /// <summary>Basisalgorithmus für die Berechnung eines PnfCharts.</summary>
    /// <param name="k">Neuer Kurs.</param>
    /// <param name="datum">Zugehöriges Datum.</param>
    /// <returns>-1 Trendumkehr mit neuer Säule; 1 Säule wurde verlängert; 0 keine Änderung</returns>
    public int Basisalgorithmus(decimal k, DateTime datum)
    {
      int neu = 0;
      datumm.SetDatum(datum);

      if (boxtyp == 0)
      {
        // unbestimmter Trend
        var bb0 = bb;
        var bbx = NextBox(bb);

        if (k >= bbx)
        {
          boxtyp = 1;
          bb = bbx;
          bbx = NextBox(bb);
          AddSaeule(new PnfColumn(bb0, bbx, boxtyp, 2, datumm));
          while (k > bbx)
          {
            bb = bbx;
            bbx = NextBox(bb);
            saeule.SetMax(bbx, datumm);
          }
        }
        else
        {
          var bbo = PrevBox(bb);
          if (k <= bbo)
          {
            boxtyp = 2;
            bb = bbo;
            bbo = PrevBox(bb);
            AddSaeule(new PnfColumn(bbo, bb0, boxtyp, 2, datumm));
            while (k < bbo)
            {
              bb = bbo;
              bbo = PrevBox(bb);
              saeule.SetMin(bbo, datumm);
            }
          }
        }
      }
      else if (boxtyp == 1)
      {
        // Aufwärtstrend
        var m = saeule.Max;
        var bbx = NextBox(bb);
        if (k >= bbx)
        {
          // Fall I: Prüfe, ob Aufwärtstrend verlängert wird.
          while (k > bbx)
          { // Fehler: bbx statt bb!
            saeule.SetMax(NextBox(bbx), datumm); // obere Grenze bei X
            bb = bbx;
            bbx = NextBox(bb);
          }
        }
        neu = Functions.CompDouble4(m, saeule.Max) != 0 ? 1 : 0;
        var bbu = PrevBoxUmkehr(bb, umkehr);
        if (k <= bbu)
        {
          // Fall II: Prüfe, ob Trendumkehr von X zu O erfolgt.
          boxtyp = 2;
          if (saeule.Size > 1)
          {
            AddSaeule(new PnfColumn(PrevBox(bbu), PrevBox(bb), boxtyp, umkehr, datumm));
          }
          else
          {
            // One-Step-Back bei 1-Box Umkehr: keine neue Säule.
            saeule.Boxtyp = boxtyp;
            saeule.SetMin(bbu, datumm);
          }
          bb = bbu;
          bbu = PrevBox(bb);
          if (k <= bbu)
          {
            // Prüfe, ob Abwärtstrend verlängert wird.
            while (k < bbu)
            {
              saeule.SetMin(PrevBox(bbu), datumm);
              bb = bbu;
              bbu = PrevBox(bb);
            }
          }
          neu = -1;
        }
      }
      else if (boxtyp == 2)
      {
        // Abwärtstrend
        var m = saeule.Min;
        var bbo = PrevBox(bb);
        if (k <= bbo)
        {
          // Fall I: Prüfe, ob Abwärtstrend verlängert wird.
          while (k < bbo)
          { // Fehler: bbo statt bb!
            saeule.SetMin(PrevBox(bbo), datumm); // untere Grenze bei O
            bb = bbo;
            bbo = PrevBox(bb);
          }
        }
        neu = Functions.CompDouble4(m, saeule.Min) != 0 ? 1 : 0;
        var bbu = NextBoxUmkehr(bb, umkehr);
        if (k >= bbu)
        {
          // Fall II: Prüfe, ob Trendumkehr von O zu X erfolgt.
          boxtyp = 1;
          if (saeule.Size > 1)
          {
            AddSaeule(new PnfColumn(NextBox(bb), NextBox(bbu), boxtyp, umkehr, datumm));
          }
          else
          {
            // One-Step-Back bei 1-Box Umkehr: keine neue Säule.
            saeule.Boxtyp = boxtyp;
            saeule.SetMax(bbu, datumm);
          }
          bb = bbu;
          bbu = NextBox(bb);
          if (k >= bbu)
          {
            // Prüfe, ob Aufwärtstrend verlängert wird.
            while (k > bbu)
            {
              saeule.SetMax(NextBox(bbu), datumm);
              bb = bbu;
              bbu = NextBox(bb);
            }
          }
          neu = -1;
        }
      }
      return neu;
    }

    private decimal NextBoxUmkehr(decimal b, int umk)
    {
      var u = b;
      for (int i = 0; i < umk; i++)
      {
        u = NextBox(u);
      }
      return u;
    }

    private decimal PrevBoxUmkehr(decimal b, int umk)
    {
      var u = b;
      for (int i = 0; i < umk; i++)
      {
        u = PrevBox(u);
      }
      return u;
    }

    private decimal NextBox(decimal b)
    {
      if (skala == 0)
      {
        return b + box;
      }
      if (skala == 1)
      {
        return b * (100.0m + box) / 100.0m;
      }
      if (b < 0.25005m - box / 16)
      { // <=
        return b + box / 16;
      }
      else if (b < 1.00005m - box / 8)
      {
        return b + box / 8;
      }
      else if (b < 5.00005m - box / 4)
      {
        return b + box / 4;
      }
      else if (b < 20.00005m - box / 2)
      {
        return b + box / 2;
      }
      else if (b < 100.00005m - box)
      {
        return b + box;
      }
      else if (b < 200.00005m - box * 2)
      {
        return b + box * 2;
      }
      else if (b < 500.00005m - box * 4)
      {
        return b + box * 4;
      }
      else if (b < 1000.00005m - box * 5)
      {
        return b + box * 5;
      }
      else if (b < 25000.00005m - box * 50)
      {
        return b + box * 50;
      }
      return b + box * 500;
    }

    private decimal PrevBox(decimal b)
    {
      if (skala == 0)
      {
        return b - box;
      }
      if (skala == 1)
      {
        return b * 100.0m / (100.0m + box);
      }
      if (b < 0.25005m)
      { // <=
        return b - box / 16;
      }
      else if (b < 1.00005m)
      {
        return b - box / 8;
      }
      else if (b < 5.00005m)
      {
        return b - box / 4;
      }
      else if (b < 20.00005m)
      {
        return b - box / 2;
      }
      else if (b < 100.00005m)
      {
        return b - box;
      }
      else if (b < 200.00005m)
      {
        return b - box * 2;
      }
      else if (b < 500.00005m)
      {
        return b - box * 4;
      }
      else if (b < 1000.00005m)
      {
        return b - box * 5;
      }
      else if (b < 25000.00005m)
      {
        return b - box * 50;
      }
      return b - box * 500;
    }

    private void AddSaeule(PnfColumn c)
    {
      if (c == null)
      {
        return;
      }
      saeule = c;
      saeulen.Add(c);
    }

    private int xgroesse = 10;
    private int ygroesse = 10;

    /// <summary>Liefert Größe für das Chart und führt die Berechnung der Kästchen aus.</summary>
    /// <param name="xgroesse">Größe für ein X oder O in x-Richtung.</param>
    /// <param name="ygroesse">Größe für ein X oder O in y-Richtung.</param>
    /// <returns>Größe für das Chart.</returns>
    public Tuple<int, int> GetDimension(int xgroesse, int ygroesse)
    {
      this.xgroesse = xgroesse;
      this.ygroesse = ygroesse;

      int breite = 12 * xgroesse; // 21
      int hoehe = 4 * ygroesse; // 7
      int h = (werte.Count + 1) * ygroesse + hoehe;
      int b = (saeulen.Count + 1) * xgroesse + breite;
      int l = string.IsNullOrEmpty(bezeichnung) ? 0 : bezeichnung.Length;
      l = Math.Max(20, l);
      b = Math.Max(b, (10 + l) * xgroesse); // min. Größe für Überschriften mit Beschreibung und Kursen
      return new Tuple<int, int>(b, h);
    }

    /// <summary>Liefert Dimension mit Größe für ein X oder O in x- und y-Richtung.</summary>
    /// <param name="b">Breite des Charts in Pixeln.</param>
    /// <param name="h">Höhe des Charts in Pixeln.</param>
    /// <returns>Dimension mit Größe für ein X oder O in x- und y-Richtung.</returns>
    public Tuple<int, int> ComputeDimension(int b, int h)
    {
      var xg = 15;
      var yg = 15;
      var d = GetDimension(xg, yg);
      while (b > 0 && h > 0 && xg > 0 && (d.Item1 < b && d.Item2 < h))
      {
        xg++;
        yg++;
        d = GetDimension(xg, yg);
      }
      while (b > 0 && h > 0 && xg > 0 && (d.Item1 > b || d.Item2 > h))
      {
        xg--;
        yg--;
        d = GetDimension(xg, yg);
      }
      return new Tuple<int, int>(xg, yg);
    }

    private void GetTrendlinien(int ab)
    {
      int anzahl = saeulen.Count;
      if (anzahl - ab < 2)
      {
        return;
      }
      PnfColumn c = saeulen[ab];
      PnfColumn c2 = saeulen[ab + 1];
      var ende = 2;
      var aufende = false;
      var abende = false;
      var maxi = 99;
      var xminab = ab;
      var xminauf = ab;

      // 1. Trendlinie
      bool aufwaerts = c.IsO && !c2.IsO;
      PnfTrend t;
      if (aufwaerts)
      {
        t = new PnfTrend(ab + 1, c.Ypos - 1, aufwaerts ? 1 : 2);
        xminauf = t.Xpos + 1;
      }
      else
      {
        t = new PnfTrend(ab + 1, c.Ytop, aufwaerts ? 1 : 2);
        xminab = t.Xpos + 1;
      }
      BerechneLaenge(t, anzahl);
      if (t.Xpos + t.Laenge >= anzahl)
      {
        if (aufwaerts)
        {
          aufende = true;
        }
        else
        {
          abende = true;
        }
      }
      trends.Add(t);

      while (ende > 0)
      {
        int i = Math.Min(anzahl - 1, t.Xpos + t.Laenge);
        if (aufwaerts)
        {
          // Säule mit höchsten X des vorherigen Abwärtstrends finden.
          int c0 = -1;
          int ym = 0;
          bool gefunden = false;
          for (int j = i; j >= xminab; j--)
          {
            c = saeulen[j];
            if (!c.IsO && (c0 < 0 || c.Ytop > ym))
            {
              c0 = j;
              ym = c.Ypos;
            }
          }
          while (!gefunden && c0 < anzahl)
          {
            if (c0 >= 0)
            {
              c = saeulen[c0];
              var t0 = new PnfTrend(c0 + 1, c.Ytop, aufwaerts ? 2 : 1);
              BerechneLaenge(t0, anzahl);
              if (i <= t0.Xpos + t0.Laenge && t0.Laenge > 1)
              {
                gefunden = true;
                aufwaerts = !aufwaerts;
                xminab = t0.Xpos + 1;
                if (t0.Xpos + t0.Laenge < anzahl || !abende)
                {
                  t = t0;
                  trends.Add(t);
                  if (t0.Xpos + t0.Laenge >= anzahl)
                  {
                    abende = true;
                  }
                }
              }
              else
              {
                c0 += 2; // benachbarte X-Säule
              }
            }
            else
            {
              if (saeulen[i].IsO)
              {
                c0 = i;
              }
              else
              {
                c0 = i + 1;
              }
            }
          }
          if (gefunden)
          {
            ende = (aufende ? 0 : 1) + (abende ? 0 : 1);
          }
          else
          {
            aufwaerts = !aufwaerts;
            ende--;
          }
          maxi--;
          if (maxi <= 0)
          {
            ende = 0;
          }
        }
        else
        {
          // Säule mit tiefsten O des vorherigen Abwärtstrends finden.
          int c0 = -1;
          int ym = 0;
          bool gefunden = false;
          for (int j = i; j >= xminauf; j--)
          {
            c = saeulen[j];
            if (c.IsO && (c0 < 0 || c.Ypos < ym))
            {
              c0 = j;
              ym = c.Ypos;
            }
          }
          while (!gefunden && c0 < anzahl)
          {
            if (c0 >= 0)
            {
              c = saeulen[c0];
              var t0 = new PnfTrend(c0 + 1, c.Ypos - 1, aufwaerts ? 2 : 1);
              BerechneLaenge(t0, anzahl);
              if (i <= t0.Xpos + t0.Laenge && t0.Laenge > 1)
              {
                gefunden = true;
                aufwaerts = !aufwaerts;
                xminauf = t.Xpos + 1;
                if (t0.Xpos + t0.Laenge < anzahl || !aufende)
                {
                  t = t0;
                  trends.Add(t);
                  if (t0.Xpos + t0.Laenge >= anzahl)
                  {
                    aufende = true;
                  }
                }
              }
              else
              {
                c0 += 2; // benachbarte O-Säule
              }
            }
            else
            {
              if (saeulen[i].IsO)
              {
                c0 = i;
              }
              else
              {
                c0 = i + 1;
              }
            }
          }
          if (gefunden)
          {
            ende = (aufende ? 0 : 1) + (abende ? 0 : 1);
          }
          else
          {
            aufwaerts = !aufwaerts;
            ende--;
          }
          maxi--;
          if (maxi <= 0)
          {
            ende = 0;
          }
        }
      }
    }

    private void BerechneLaenge(PnfTrend t, int anzahl)
    {
      if (t == null)
      {
        return;
      }
      bool aufwaerts = t.Boxtyp == 1;
      int grenze = t.Ypos;
      int d = aufwaerts ? 1 : -1;
      bool bruch = false;
      for (int i = t.Xpos; i < anzahl; i++)
      {
        var c = saeulen[i];
        if (aufwaerts && c.IsO)
        {
          if (c.Ypos <= grenze)
          {
            bruch = true;
          }
        }
        else if (!aufwaerts && !c.IsO)
        {
          if (c.Ytop > grenze)
          {
            bruch = true;
          }
        }
        if (bruch)
        {
          break;
        }
        t.SetLaenge(t.Laenge + 1);
        grenze += d;
      }
    }

    public static string GetBezeichnung(string bezeichnung, decimal box, int skala, int umkehr, int methode,
      bool relativ, int dauer)
    {
      var sb = new StringBuilder();
      if (!string.IsNullOrEmpty(bezeichnung))
      {
        sb.Append(bezeichnung);
      }
      if (sb.Length > 0)
      {
        sb.Append(' ');
      }
      sb.Append('(').Append(M0(WP035));
      if (Functions.CompDouble(box, 0) > 0)
      {
        sb.Append(' ').Append(Functions.ToString(box));
      }
      sb.Append(' ');
      if (skala == 0)
      {
        sb.Append(Enum_scale_fix);
      }
      else if (skala == 1)
      {
        sb.Append(Enum_scale_pc);
      }
      else
      {
        sb.Append(Enum_scale_dyn);
      }
      if (sb.Length > 0)
      {
        sb.Append(", ");
      }
      sb.Append(WP039(umkehr, true));
      if (sb.Length > 0)
      {
        sb.Append(", ");
      }
      switch (methode)
      {
        case 2:
          sb.Append(Enum_method_hl);
          break;
        case 3:
          sb.Append(Enum_method_hlr);
          break;
        case 4:
          sb.Append(Enum_method_ohlc);
          break;
        case 5:
          sb.Append(Enum_method_tp);
          break;
        default:
          sb.Append(Enum_method_c);
          break;
      }
      if (relativ)
      {
        sb.Append(' ').Append(M0(WP045));
      }
      if (dauer > 0)
      {
        sb.Append(", ").Append(WP046(dauer));
      }
      sb.Append(')');
      return sb.ToString();
    }

    public string Bezeichnung
    {
      get { return GetBezeichnung(bezeichnung, box, skala, umkehr, methode, relativ, dauer); }
      set { this.bezeichnung = value; }
    }

    public string GetBezeichnung2()
    {
      var sb = new StringBuilder();
      if (kurse.Any())
      {
        DateTime von = kurse[0].Datum;
        DateTime bis = kurse[^1].Datum;
        sb.Append(WP047(von, bis, true));
        sb.Append(" O:").Append(Functions.ToString(Functions.Round(kurse[0].Close), 2));
        sb.Append(" H:").Append(Functions.ToString(Functions.Round(max), 2));
        sb.Append(" L:").Append(Functions.ToString(Functions.Round(min), 2));
        sb.Append(" C:").Append(Functions.ToString(Functions.Round(kurse[^1].Close), 2));
      }
      return sb.ToString();
    }

    // public int getMethode() {
    // return methode;
    // }

    public int Methode
    {
      set
      {
        if (1 <= value && value <= 5)
        {
          this.methode = value;
        }
      }
    }

    public decimal Box
    {
      get { return box; }
      set
      {
        if (Functions.CompDouble4(value, 0) > 0)
        {
          this.box = value;
        }
      }
    }

    // public boolean isProzentual() {
    // return prozentual;
    // }

    public int Skala
    {
      set { this.skala = value; }
    }

    // public int getUmkehr() {
    // return umkehr;
    // }

    public int Umkehr
    {
      set
      {
        if (umkehr >= 1)
        {
          this.umkehr = value;
        }
      }
    }

    /**
     * Liefert den aktuellen Schlusskurs.
     * @return Aktueller Schlusskurs.
     */
    public decimal Kurs
    {
      get { return kurs; }
    }

    public List<PnfColumn> Saeulen
    {
      get { return saeulen; }
    }

    // public double getMin() {
    // return min;
    // }

    public decimal Max
    {
      get { return max; }
    }

    public int Xgroesse
    {
      get { return xgroesse; }
      // set { xgroesse = value;}
    }

    public int Ygroesse
    {
      get { return ygroesse; }
      // set { ygroesse = value;}
    }

    public int Posmax
    {
      get { return posmax; }
      // set { posmax = value;}
    }

    public List<decimal> Werte
    {
      get { return werte; }
      // set { werte = value;}
    }

    public List<PnfTrend> Trends
    {
      get { return trends; }
    }

    public List<PnfPattern> Pattern
    {
      get { return pattern; }
    }

    public bool Relativ
    {
      get { return relativ; }
      set
      {
        this.relativ = value;
        if (!value && !string.IsNullOrEmpty(bezeichnung))
        {
          // Relation aus Bezeichnung entfernen.
          var array = bezeichnung.Split(new[] { " \\(" }, StringSplitOptions.None);
          bezeichnung = array[0];
        }
      }
    }

    public decimal Ziel
    {
      get { return ziel; }
      set { ziel = value; }
    }

    public decimal Stop
    {
      get { return stop; }
      set { stop = value; }
    }

    public decimal Trend
    {
      get { return trend; }
    }
  }
}
