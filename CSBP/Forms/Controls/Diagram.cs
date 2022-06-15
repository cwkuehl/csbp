// <copyright file="Diagram.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Forms.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using Cairo;
using CSBP.Base;

/// <summary>
/// Function for drawing a diagram.
/// </summary>
public class Diagram
{
  /// <summary>
  /// Initializes a new instance of the <see cref="Diagram"/> class.
  /// </summary>
  public Diagram()
  {
    Functions.MachNichts();
  }

  /// <summary>Draws a diagram.</summary>
  /// <param name="name1">Affected name for values.</param>
  /// <param name="c1">Affected values.</param>
  /// <param name="pc">Affected context.</param>
  /// <param name="w0">Affected diagram width.</param>
  /// <param name="h0">Affected diagram height.</param>
  /// <param name="ww">Affected window width.</param>
  /// <param name="wh">Affected window height.</param>
  public static void Draw(string name1, List<KeyValuePair<string, decimal>> c1, Cairo.Context pc, int w0, int h0, int ww, int wh)
  {
    // pc.SetSourceRGBA(1, 0, 0, 1);
    // pc.LineWidth = 2;
    // pc.MoveTo(w0, h0);
    // pc.LineTo(w0, h0 + wh);
    // pc.LineTo(w0 + ww, h0 + wh);
    // pc.LineTo(w0 + ww, h0);
    // pc.LineTo(w0, h0);
    // pc.Stroke();
    var c = c1;
    if (c == null || c.Count <= 1)
      return;
    //// Assumption: Werte >= 0
    decimal xanzahl = c.Count;
    var ymin = c.Min(a => a.Value);
    var ymax = c.Max(a => a.Value);
    if (ymax == ymin)
      ymax++;
    var diff = ymax - ymin;
    var stellen = (decimal)Math.Pow(10, Math.Max(1, Math.Floor(Math.Log10((double)diff))));
    ymin = Math.Floor(ymin / stellen) * stellen;
    ymax = Math.Ceiling(ymax / stellen) * stellen;
    diff = ymax - ymin;
    decimal xgroesse = (ww + wh) / 50; // Box size
    decimal ygroesse = (ww + wh) / 50;
    decimal xlegende = xgroesse * (decimal)Math.Log10((double)ymax) / 1.5m; // Legende waagerecht rechts
    decimal ylegende = ygroesse; // Legende waagerecht unten
    var xoffset = xgroesse * 0.5m;
    var yoffset = ygroesse * 0.2m;
    var xstep = (ww - (xoffset * 2) - xlegende) / (c.Count - 1);
    var ystep = (wh - (yoffset * 2) - ylegende) / diff;
    var white = new Cairo.Color(1, 1, 1);
    var black = new Cairo.Color(0, 0, 0);
    var red = new Cairo.Color(1, 0, 0);
    var blue = new Cairo.Color(0, 0, 1);
    var green = new Cairo.Color(0, 0.5, 0);
    var lightgray = new Cairo.Color(0.83, 0.83, 0.83);
    var darkviolet = new Cairo.Color(0.55, 0, 0.55);
    pc.SelectFontFace("TimesRoman", FontSlant.Normal, FontWeight.Normal); // monospace
    pc.SetFontSize((int)(ygroesse / 1.0m));
    var fontx = pc.GetContextFontFace();
    pc.SelectFontFace("TimesRoman", FontSlant.Normal, FontWeight.Normal);
    pc.SetFontSize((int)(ygroesse / 1.5m));
    var fontplain = pc.GetContextFontFace();
    pc.SelectFontFace("TimesRoman", FontSlant.Normal, FontWeight.Bold);
    pc.SetFontSize((int)(ygroesse / 1.3m));
    var fontbold = pc.GetContextFontFace();
    var color = black;
    var font = fontplain;

    // Background
    pc.SetSourceColor(white);
    pc.Rectangle(w0, h0, w0 + ww, h0 + wh);
    pc.Fill();
    pc.SetSourceColor(black);

    var xl = w0 + xoffset;
    var yl = -1m;
    var xl2 = w0 + ww - xoffset - xlegende;
    var yl2 = -1m;
    var val = ymax;
    //// Right vertical lines
    DrawLine(pc, w0 + ww - xoffset - xlegende, h0 + yoffset, w0 + ww - xoffset - xlegende, h0 + wh - yoffset - ylegende, lightgray);
    while (val >= ymin)
    {
      yl = (decimal)h0 + (decimal)wh - yoffset - ylegende - ((val - ymin) * ystep);
      //// waagerechte Gitterlinie mit y-Wert
      DrawLine(pc, xl, yl, xl2, yl, lightgray);
      DrawString(pc, xl2 + yoffset, yl + (ygroesse / 3), Functions.ToString(val, 0), fontplain, black);
      val -= stellen;
    }
    xl = -1m;
    yl = -1m;
    xl2 = w0 + xoffset - xstep;
    yl2 = -1m;
    var xbez = w0 + xoffset;
    foreach (var v in c)
    {
      xl2 += (decimal)xstep;
      yl2 = (decimal)h0 + (decimal)wh - yoffset - ylegende - ((v.Value - ymin) * ystep);
      if (xl2 >= xbez && xl2 <= w0 + ww - xoffset - (v.Key.Length * xgroesse / 2.5m))
      {
        // senkrechte Gitterlinie mit x-Wert
        DrawLine(pc, xl2, h0 + yoffset, xl2, h0 + wh - yoffset - ylegende, lightgray);
        DrawString(pc, xl2, h0 + wh - yoffset, v.Key, fontplain, black);
        xbez += v.Key.Length * xgroesse / 2m;
      }
      if (xl != -1)
        DrawLine(pc, xl, yl, xl2, yl2, red);
      DrawCircle(pc, xl2, yl2, 2, red);
      xl = xl2;
      yl = yl2;
    }
    DrawString(pc, w0 + (xoffset * 2), h0 + yoffset + ylegende, name1, fontx, red);

    // Warnungen verhindern: Cairo.FontFace is leaking, programmer is missing a call to Dispose
    fontx.Dispose();
    fontplain.Dispose();
    fontbold.Dispose();
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
  /// <param name="vertikal">Is it a vertical text or not.</param>
  private static void DrawString(Cairo.Context pc, decimal x, decimal y, string str,
      FontFace font = null, Cairo.Color? color = null, bool vertikal = false)
  {
    // Position links unten
    if (string.IsNullOrEmpty(str))
      return;
    pc.Save();
    if (color.HasValue)
      pc.SetSourceColor(color.Value);
    if (font != null)
      pc.SetContextFontFace(font);
    pc.MoveTo((double)x, (double)y);
    //// pc.SetFontSize(20);
    if (vertikal)
      pc.Rotate(Math.PI / 2);
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
    // Position links oben
    pc.Save();
    if (color.HasValue)
      pc.SetSourceColor(color.Value);
    pc.MoveTo((int)x, (int)y);
    pc.LineTo((int)x2, (int)y2);
    pc.Stroke();
    pc.Restore();
  }

  /// <summary>
  /// Draws a circle.
  /// </summary>
  /// <param name="pc">Affected context.</param>
  /// <param name="x">Affected x coordinate.</param>
  /// <param name="y">Affected y coordinate.</param>
  /// <param name="radius">Affected radius.</param>
  /// <param name="color">Affected color.</param>
  private static void DrawCircle(Cairo.Context pc, decimal x, decimal y, int radius, Cairo.Color? color = null)
  {
    // Position links oben
    pc.Save();
    if (color.HasValue)
      pc.SetSourceColor(color.Value);
    pc.MoveTo((int)x, (int)y);
    pc.Arc((int)x, (int)y, radius, 0, 2 * Math.PI);
    pc.Stroke();
    pc.Restore();
  }
}
