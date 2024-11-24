// <copyright file="ChartPane.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System;
using Cairo;
using CSBP.Services.Base;
using CSBP.Services.Pnf;

/// <summary>
/// Functions for drawing charts.
/// </summary>
public class ChartPane
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ChartPane"/> class.
  /// </summary>
  public ChartPane()
  {
    Functions.MachNichts();
  }

  /// <summary>Draws a Point and Figure charts.</summary>
  /// <param name="c">Affected chart.</param>
  /// <param name="pc">Affected context.</param>
  /// <param name="ww">Affected window width.</param>
  /// <param name="wh">Affected window height.</param>
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
    decimal xanzahl = c.Saeulen.Count;
    decimal yanzahl = c.Werte.Count;
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

    // Background
    pc.SetSourceColor(white);
    pc.Rectangle(0, 0, ww, wh);
    pc.Fill();
    pc.SetSourceColor(black);

    // Columns
    DrawString(pc, xoffset, ygroesse * 0.9m, c.Bezeichnung, font, color);
    DrawString(pc, xoffset, ygroesse * 1.8m, c.GetBezeichnung2(), font, color);
    var b = xoffset + xgroesse;
    decimal x;
    decimal y;
    decimal h;
    foreach (var s in c.Saeulen)
    {
      h = s.Ypos;
      var array = s.Chars;
      foreach (var xo in array)
      {
        x = b;
        y = ((max - h) * ygroesse) + yoffset;
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
    //// gc.SetLineAttributes(1, LineStyle.Solid, CapStyle.Butt, JoinStyle.Bevel);
    x = xoffset + ((xanzahl + 2) * xgroesse);
    y = yoffset + (yanzahl * ygroesse);
    var aktkurs = c.Kurs;
    var iakt = -1;
    var yakt = -1m;
    if (Functions.CompDouble4(aktkurs, 0) > 0)
    {
      var d = c.Max + 1;
      for (int i = 0; i < yanzahl; i++)
      {
        if (Functions.CompDouble4(c.Werte[i], d) < 0
                && Functions.CompDouble4(c.Werte[i], aktkurs) > 0)
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
      //// Horizontal lines
      DrawLine(pc, xoffset, y, x, y, color);
      y -= ygroesse;
    }

    // Writes dates.
    x = xoffset;
    y = yoffset + (yanzahl * ygroesse);
    for (int i = 0; i < xanzahl + 3; i++)
    {
      // Vertical lines.
      DrawLine(pc, x, yoffset, x, y);
      if (i % 6 == 0 && i < xanzahl && c.Saeulen[i].Date != null)
      {
        DrawString(pc, x + xgroesse, y + (ygroesse * 0.5m), Functions.ToString(c.Saeulen[i].Date), font, color);
      }
      x += xgroesse;
    }

    // Trend lines
    // gc.SetLineAttributes(2, LineStyle.Solid, CapStyle.Butt, JoinStyle.Bevel);
    foreach (var t in c.Trends)
    {
      x = ((t.Xpos + 1) * xgroesse) + xoffset;
      y = ((max - t.Ypos) * ygroesse) + yoffset;
      b = t.Laenge * xgroesse;
      if (t.Boxtype == 0)
      {
        b += xgroesse;
        h = 0;
        color = red;
      }
      else if (t.Boxtype == 1)
      {
        h = -t.Laenge * ygroesse;
        color = blue;
      }
      else
      {
        h = t.Laenge * ygroesse;
        y += ygroesse;
        color = blue;
      }
      DrawLine(pc, x, y, x + b, y + h, color);
    }

    // Pattern
    color = darkviolet;
    foreach (var pa in c.Pattern)
    {
      x = ((pa.Xpos + 2) * xgroesse) + xoffset;
      y = ((max - pa.Ypos) * ygroesse) + yoffset;
      if (yakt >= 0)
      {
        if (Math.Abs(y - yakt) < ygroesse)
        {
          y -= ygroesse; // Moves up
          if (y < 0)
            y += ygroesse * 2; // Moves down
        }
      }
      DrawString(pc, x, y - ygroesse, pa.Bezeichnung, font, color);
    }
  }

  /// <summary>
  /// Draws a string.
  /// </summary>
  /// <param name="pc">Affected context.</param>
  /// <param name="x">Affected x coordinate.</param>
  /// <param name="y">Affected y coordinate.</param>
  /// <param name="str">Affectd string.</param>
  /// <param name="font">Affected font.</param>
  /// <param name="color">Affected color.</param>
  private static void DrawString(Cairo.Context pc, /*Window p, Layout layout, Gdk.GC gc,*/ decimal x, decimal y, string str,
    FontFace font = null, Cairo.Color? color = null)
  {
    // Position left bottom
    if (string.IsNullOrEmpty(str)) // || string.IsNullOrEmpty(color))
      return;
    pc.Save();
    if (color.HasValue)
      pc.SetSourceColor(color.Value);
    if (font != null)
      pc.SetContextFontFace(font);
    pc.MoveTo((double)x, (double)y);
    //// pc.SetFontSize(20);
    pc.ShowText(str);
    pc.Restore();
  }

  /// <summary>
  /// Draws a line.
  /// </summary>
  /// <param name="pc">Affected context.</param>
  /// <param name="x">Affected x coordinate.</param>
  /// <param name="y">Affected y coordinate.</param>
  /// <param name="x2">Affected x2 coordinate of target.</param>
  /// <param name="y2">Affected yy coordinate or target.</param>
  /// <param name="color">Affected color.</param>
  private static void DrawLine(Cairo.Context pc, decimal x, decimal y,
    decimal x2, decimal y2, Cairo.Color? color = null)
  {
    // Position left top
    pc.Save();
    if (color.HasValue)
      pc.SetSourceColor(color.Value);
    pc.MoveTo((int)x, (int)y);
    pc.LineTo((int)x2, (int)y2);
    pc.Restore();
  }
}
