// <copyright file="ChartPane.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using Gdk;
using CSBP.Base;
using CSBP.Services.Pnf;
using Cairo;
// using Pango;

namespace CSBP.Forms.Controls
{
  public class ChartPane
  {
    public ChartPane()
    {
    }

    private static void DrawString(Cairo.Context pc, /*Window p, Layout layout, Gdk.GC gc,*/ decimal x, decimal y, string str,
        FontFace font = null, Cairo.Color? color = null)
    {
      // Position links unten
      if (string.IsNullOrEmpty(str)) // || string.IsNullOrEmpty(color))
        return;
      pc.Save();
      if (color.HasValue)
        pc.SetSourceColor(color.Value);
      if (font != null)
        pc.SetContextFontFace(font);
      pc.MoveTo((double)x, (double)y);
      //pc.SetFontSize(20);
      pc.ShowText(str);
      pc.Restore();
      // //var b1 = bold ? "<b>" : "";
      // //var b2 = bold ? "</b>" : "";
      // //layout.SetMarkup($"<span color='{color}'>{b1}{str}{b2}</span>");
      // layout.SetText(str);
      // if (font != null)
      //   layout.FontDescription = font;
      // if (color.HasValue)
      //   gc.RgbFgColor = color.Value;
      // p.DrawLayout(gc, (int)x, (int)y, layout);
    }

    private static void DrawLine(Cairo.Context pc, /*Window p, Gdk.GC gc,*/ decimal x, decimal y,
        decimal x2, decimal y2, Cairo.Color? color = null)
    {
      // Position links oben
      pc.Save();
      if (color.HasValue)
        pc.SetSourceColor(color.Value);
      pc.MoveTo((int)x, (int)y);
      pc.LineTo((int)x2, (int)y2);
      pc.Restore();
    }

    /// <summary>Zeichnen eines Point and Figure-Charts.</summary>
    public static void DrawChart(PnfChart c, Cairo.Context pc, int ww, int wh)
    {
      // pc.SetSourceRGBA(1, 0, 0, 1);
      // pc.LineWidth = 2;
      // pc.MoveTo(200, 100);
      // pc.LineTo(200, 300);
      // pc.MoveTo(100, 200);
      // pc.LineTo(300, 200);
      // pc.Stroke();

      decimal xgroesse = c.Xgroesse;
      decimal ygroesse = c.Ygroesse;
      decimal max = c.Posmax;
      var xoffset = xgroesse * 1.5m;
      var yoffset = ygroesse * 3.2m;
      decimal xanzahl = c.GetSaeulen().Count;
      decimal yanzahl = c.Werte.Count;
      var b = 0m;
      var h = 0m;
      var x = 0m;
      var y = 0m;
      var white = new Cairo.Color(1, 1, 1);
      var black = new Cairo.Color(0, 0, 0);
      var red = new Cairo.Color(1, 0, 0);
      var blue = new Cairo.Color(0, 0, 1);
      var green = new Cairo.Color(0, 0.5, 0);
      var lightgray = new Cairo.Color(0.83, 0.83, 0.83);
      var darkviolet = new Cairo.Color(0.55, 0, 0.55);
      pc.SelectFontFace("TimesRoman", FontSlant.Normal, FontWeight.Normal); // monospace
      pc.SetFontSize((int)(ygroesse / 1.1m));
      var fontx = pc.GetContextFontFace();
      pc.SelectFontFace("TimesRoman", FontSlant.Normal, FontWeight.Normal);
      pc.SetFontSize((int)(ygroesse / 1.3m));
      var fontplain = pc.GetContextFontFace();
      pc.SelectFontFace("TimesRoman", FontSlant.Normal, FontWeight.Bold);
      pc.SetFontSize((int)(ygroesse / 1.3m));
      var fontbold = pc.GetContextFontFace();
      var color = black;
      var font = fontplain;

      // Hintergrund
      pc.SetSourceColor(white);
      pc.Rectangle(0, 0, ww, wh);
      pc.Fill();
      pc.SetSourceColor(black);

      // Säulen
      DrawString(pc, xoffset, ygroesse * 0.9m, c.GetBezeichnung(), font, color);
      DrawString(pc, xoffset, ygroesse * 1.8m, c.GetBezeichnung2(), font, color);
      b = xoffset + xgroesse;
      h = 0;
      foreach (var s in c.GetSaeulen())
      {
        h = s.getYpos();
        var array = s.getChars();
        foreach (var xo in array)
        {
          x = b;
          y = (max - h) * ygroesse + yoffset;
          if (xo == 'O')
          {
            color = red;
            DrawString(pc, x + 1, y - 1, "O", fontx, color);
          }
          else if (xo == 'X')
          {
            color = green;
            DrawString(pc, x + 1, y - 1, "X", fontx, color);
          }
          else
          {
            color = black;
            DrawString(pc, x + 1, y - 1, xo.ToString(), fontx, color);
          }
          h += 1;
        }
        b += xgroesse;
      }

      // Werte schreiben
      color = lightgray;
      //gc.SetLineAttributes(1, LineStyle.Solid, CapStyle.Butt, JoinStyle.Bevel);
      x = xoffset + (xanzahl + 2) * xgroesse;
      y = yoffset + yanzahl * ygroesse;
      var aktkurs = c.GetKurs();
      var iakt = -1;
      var yakt = -1m;
      if (Functions.compDouble4(aktkurs, 0) > 0)
      {
        var d = c.GetMax() + 1;
        for (int i = 0; i < yanzahl; i++)
        {
          if (Functions.compDouble4(c.Werte[i], d) < 0
                  && Functions.compDouble4(c.Werte[i], aktkurs) > 0)
          {
            d = c.Werte[i];
            iakt = i;
          }
        }
      }
      for (int i = 0; i < yanzahl + 1; i++)
      {
        if (i < yanzahl)
        {
          if (i == iakt)
          {
            color = black;
            DrawString(pc, x + 5, y - ygroesse, Functions.ToString(Functions.Round(aktkurs), 2), fontbold, color);
            color = lightgray;
            yakt = y;
          }
          else
          {
            DrawString(pc, x + 5, y - ygroesse, Functions.ToString(Functions.Round(c.Werte[i]), 2), font, color);
          }
        }
        // waagerechte Linien
        DrawLine(pc, xoffset, y, x, y, color);
        y -= ygroesse;
      }

      // Datumswerte schreiben
      x = xoffset;
      y = yoffset + yanzahl * ygroesse;
      for (int i = 0; i < xanzahl + 3; i++)
      {
        // senkrechte Linien
        DrawLine(pc, x, yoffset, x, y);
        if (i % 6 == 0 && i < xanzahl && c.GetSaeulen()[i].getDatum() != null)
        {
          DrawString(pc, x + xgroesse, y + ygroesse * 0.5m, Functions.ToString(c.GetSaeulen()[i].getDatum()), font, color);
        }
        x += xgroesse;
      }

      // Trendlinien
      //gc.SetLineAttributes(2, LineStyle.Solid, CapStyle.Butt, JoinStyle.Bevel);
      foreach (var t in c.GetTrends())
      {
        x = (t.getXpos() + 1) * xgroesse + xoffset;
        y = (max - t.getYpos()) * ygroesse + yoffset;
        b = t.getLaenge() * xgroesse;
        if (t.getBoxtyp() == 0)
        {
          b += xgroesse;
          h = 0;
          color = red;
        }
        else if (t.getBoxtyp() == 1)
        {
          h = -t.getLaenge() * ygroesse;
          color = blue;
        }
        else
        {
          h = t.getLaenge() * ygroesse;
          y += ygroesse;
          color = blue;
        }
        DrawLine(pc, x, y, x + b, y + h, color);
      }

      // Muster
      color = darkviolet;
      foreach (var pa in c.GetPattern())
      {
        x = (pa.getXpos() + 2) * xgroesse + xoffset;
        y = (max - pa.getYpos()) * ygroesse + yoffset;
        if (yakt >= 0)
        {
          if (Math.Abs(y - yakt) < ygroesse)
          {
            y -= ygroesse; // nach oben verschieben
            if (y < 0)
              y += ygroesse * 2; // nach unten verschieben
          }
        }
        DrawString(pc, x, y - ygroesse, pa.getBezeichnung(), font, color);
      }
    }
  }
}