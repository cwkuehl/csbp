// <copyright file="Diagram.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using Cairo;
using CSBP.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSBP.Forms.Controls
{
  public class Diagram
  {
    public Diagram()
    {
    }

    private static void DrawString(Cairo.Context pc, decimal x, decimal y, string str,
        FontFace font = null, Cairo.Color? color = null, bool vertikal = false)
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
      if (vertikal)
        pc.Rotate(Math.PI / 2);
      pc.ShowText(str);
      pc.Restore();
    }

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

    /// <summary>Zeichnen eines Diagramms.</summary>
    public static void Draw(string werte1, List<KeyValuePair<string, decimal>> c1, Cairo.Context pc, int w0, int h0, int ww, int wh)
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
      // Annahme: Werte >= 0
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
      decimal xgroesse = (ww + wh) / 50; // Kästchengröße
      decimal ygroesse = (ww + wh) / 50;
      decimal xlegende = xgroesse * (decimal)Math.Log10((double)ymax) / 1.5m; // Legende waagerecht rechts
      decimal ylegende = ygroesse; // Legende waagerecht unten
      // decimal max = c.Posmax;
      var xoffset = xgroesse * 0.5m;
      var yoffset = ygroesse * 0.2m;
      var xstep = (ww - xoffset * 2 - xlegende) / (c.Count - 1);
      var ystep = (wh - yoffset * 2 - ylegende) / diff;
      // decimal yanzahl = c.Werte.Count;
      // var b = 0m;
      // var h = 0m;
      // var x = 0m;
      // var y = 0m;
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

      // Hintergrund
      pc.SetSourceColor(white);
      pc.Rectangle(w0, h0, w0 + ww, h0 + wh);
      pc.Fill();
      pc.SetSourceColor(black);

      var xl = w0 + xoffset;
      var yl = -1m;
      var xl2 = w0 + ww - xoffset - xlegende;
      var yl2 = -1m;
      var val = ymax;
      // rechte senkrechte Gitterlinie
      DrawLine(pc, w0 + ww - xoffset - xlegende, h0 + yoffset, w0 + ww - xoffset - xlegende, h0 + wh - yoffset - ylegende, lightgray);
      while (val >= ymin)
      {
        yl = (decimal)h0 + (decimal)wh - yoffset - ylegende - (val - ymin) * ystep;
        // waagerechte Gitterlinie mit y-Wert
        DrawLine(pc, xl, yl, xl2, yl, lightgray);
        DrawString(pc, xl2 + yoffset, yl + ygroesse / 3, Functions.ToString(val, 0), fontplain, black);
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
        yl2 = (decimal)h0 + (decimal)wh - yoffset - ylegende - (v.Value - ymin) * ystep;
        if (xl2 >= xbez && xl2 <= w0 + ww - xoffset - v.Key.Length * xgroesse / 2.5m)
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
      DrawString(pc, w0 + xoffset * 2, h0 + yoffset + ylegende, werte1, fontx, red);

      // Warnungen verhindern: Cairo.FontFace is leaking, programmer is missing a call to Dispose
      fontx.Dispose();
      fontplain.Dispose();
      fontbold.Dispose();
    }
  }
}