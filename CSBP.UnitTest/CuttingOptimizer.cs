// <copyright file="CuttingOptimizer.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>
// <summary>
// ChatGPT C# Code:
// 1. ich habe einige lange holzleisten, die ich in kürzere stück schneiden möchte. wie kann ich das optimieren?
// 2. schreibe eine c# funktion, parameter liste von vorhandenen lattenlängen und liste von geünschten lattenlängen, berechne die optimalen schnitte für jede latte  und gebe für jede vorhandene latte die notwendigen schnitte an.
// 3. das ist schon ganz gut, aber es soll jede gewünschte latte genau einmal aus den vorhandenen latten geschnitten werden.
// </summary>

namespace CSBP.UnitTest;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Klasse, die die optimalen Schnitte berechnet.
/// </summary>
public class CuttingOptimizer
{
  /// <summary>
  /// Berechnet die optimalen Schnitte für jede Latte.
  /// </summary>
  /// <param name="vorhandeneLatten">Betroffene Liste der vorhandenen Latten.</param>
  /// <param name="gewuenschteLatten">Betroffene Liste der gewünschten Latten.</param>
  /// <returns>Ergebnis der Berechnung.</returns>
  public static Dictionary<decimal, List<decimal>> BerechneSchnitte(List<decimal> vorhandeneLatten, List<decimal> gewuenschteLatten)
  {
    var schnitteProLatte = new Dictionary<decimal, List<decimal>>();

    // Kopiere die Liste der gewünschten Latten und sortiere sie absteigend nach Länge
    var verbleibendeLattenLängen = gewuenschteLatten.OrderByDescending(x => x).ToList();

    // Überprüfen, ob es möglich ist, alle gewünschten Latten zu schneiden
    var schnitte = new List<decimal>();

    // Versuche, die gewünschten Latten aus den vorhandenen Latten zu schneiden
    foreach (var latte in vorhandeneLatten)
    {
      var latteSchnitte = new List<decimal>();
      var verbleibendeLänge = latte;

      // Versuche, jede gewünschte Latte aus der vorhandenen Latte zu schneiden
      for (int i = 0; i < verbleibendeLattenLängen.Count;)
      {
        var gewünschteLänge = verbleibendeLattenLängen[i];

        // Wenn der Schnitt in die verbleibende Länge passt, mache den Schnitt
        if (verbleibendeLänge >= gewünschteLänge)
        {
          latteSchnitte.Add(gewünschteLänge);
          verbleibendeLänge -= gewünschteLänge;

          // Entferne die geschnittene Länge aus der verbleibenden Liste
          verbleibendeLattenLängen.RemoveAt(i);
        }
        else
        {
          i++; // Versuche den nächsten Schnitt
        }
      }

      if (latteSchnitte.Count > 0)
      {
        schnitteProLatte[latte] = latteSchnitte;
      }
    }
    Print(schnitteProLatte);
    if (verbleibendeLattenLängen.Count > 0)
      Console.WriteLine($"Nicht geschnittene Latten: {string.Join(", ", verbleibendeLattenLängen)}");
    return schnitteProLatte;
  }

  /// <summary>
  /// Testen der Funktion.
  /// </summary>
  public static void Test0()
  {
    var vorhandeneLatten = new List<decimal> { 240, 300, 500 };
    var gewuenschteLatten = new List<decimal> { 60, 120, 150, 60, 120, 150 };
    var schnitte = BerechneSchnitte(vorhandeneLatten, gewuenschteLatten);
  }

  /// <summary>
  /// Testen der Funktion.
  /// </summary>
  public static void Test()
  {
    var vorhandeneLatten = new List<decimal> { 265, 257, 218.5m, 214.5m, 200, 164.5m, 146.01m, 146, 145, 112, 104.5m, 91, 59.5m, 46, 40.51m, 40.5m, 40.03m, 40.02m, 40.01m, 40, 39.51m, 39.5m, 29, 27, 25, 23 };
    var gewuenschteLatten = new List<decimal>();
    for (var i = 10; i <= 58; i++)
    {
      gewuenschteLatten.Add(6 + (i * 1.3m));
    }
    var schnitte = BerechneSchnitte(vorhandeneLatten, gewuenschteLatten);
  }

  private static void Print(Dictionary<decimal, List<decimal>> schnitte)
  {
    // Ausgabe der Ergebnisse
    foreach (var latte in schnitte)
    {
      Console.WriteLine($"Latte ({latte.Key} cm) kann geschnitten werden in: {string.Join(", ", latte.Value)}");
    }
  }
}
