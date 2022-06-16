// <copyright file="SudokuContext.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using CSBP.Base;
using static CSBP.Resources.M;
using static CSBP.Resources.Messages;

namespace CSBP.Services.NonService;

/// <summary>
/// Class for sudoku handling.
/// </summary>
[Serializable]
public class SudokuContext
{
  /// <summary>
  /// Json constructor for sudoku context.
  /// </summary>
  /// <param name="numbers">Array of numbers.</param>
  /// <param name="diagonal">Are the diagonal numbers different?.</param>
  /// <param name="maxx">Number of columns.</param>
  /// <param name="maxy">Number of rows.</param>
  /// <param name="xb">Number of horizontal boxes.</param>
  /// <param name="yb">Number of vertical boxes.</param>
  [JsonConstructor]
  public SudokuContext(int[] numbers, bool diagonal, int maxx, int maxy, int maxxb, int maxyb, int max) =>
    (Numbers, Diagonal, Maxx, Maxxb, Maxy, Maxyb, Max) = (numbers, diagonal, maxx, maxxb, maxy, maxyb, max);

  /// <summary>
  /// Constructor for sudoku context.
  /// </summary>
  /// <param name="arr">Array of numbers.</param>
  /// <param name="diagonal">Are the diagonal numbers different?.</param>
  /// <param name="x">Number of columns.</param>
  /// <param name="y">Number of rows.</param>
  /// <param name="xb">Number of horizontal boxes.</param>
  /// <param name="yb">Number of vertical boxes.</param>
  public SudokuContext(int[] arr, bool diagonal, int x = 9, int y = 9, int xb = 3, int yb = 3)
  {
    if (x < 1 || y < 1 || xb < 1 || yb < 1 || x % xb != 0 || y % yb != 0)
    {
      x = 9;
      xb = 3;
      y = 9;
      yb = 3;
    }
    Maxx = x;
    Maxxb = xb;
    Maxy = y;
    Maxyb = yb;
    Max = x * y;
    Diagonal = x == y && diagonal;
    Numbers = new int[Max];
    Copy(arr, Numbers);
  }

  /// <summary>
  /// Copy constructor for sudoku context.
  /// </summary>
  /// <param name="c">Affected context to clone.</param>
  /// <param name="arr">Array of numbers.</param>
  /// <param name="diagonal">Are the diagonal numbers different?.</param>
  public SudokuContext(SudokuContext c, int[] arr = null, bool? diagonal = null)
  {
    Maxx = c.Maxx;
    Maxxb = c.Maxxb;
    Maxy = c.Maxy;
    Maxyb = c.Maxyb;
    Max = c.Max;
    Diagonal = diagonal ?? c.Diagonal;
    Numbers = SudokuContext.Clone(arr ?? c.Numbers);
  }

  /// <summary>Gets number of columns.</summary>
  public int Maxx { get; private set; }

  /// <summary>Getst number of horizontal boxes.</summary>
  public int Maxxb { get; private set; }

  /// <summary>Gets number of rows.</summary>
  public int Maxy { get; private set; }

  /// <summary>Gets number of vertikal boxes.</summary>
  public int Maxyb { get; private set; }

  /// <summary>Gets number of fields.</summary>
  public int Max { get; private set; }

  /// <summary>Gets the field of numbers.</summary>
  public int[] Numbers { get; private set; }

  /// <summary>Gets a value indicating whether shows if the diagonal values are all different.</summary>
  public bool Diagonal { get; private set; }

  /// <summary>Add sudoku context to undo list if it is different to last one.</summary>
  /// <param name="list">Affected undo list.</param>
  /// <param name="c">Sudoku context to add.</param>
  /// <returns>Was sudoku context added?</returns>
  public static bool Add(Stack<SudokuContext> list, SudokuContext c)
  {
    if (list == null)
      return false;
    if (list.Count > 0)
    {
      var c0 = list.Peek();
      var eq = c.Maxx == c0.Maxx && c.Maxxb == c0.Maxxb && c.Maxy == c0.Maxy && c.Maxyb == c.Maxyb && c.Max == c0.Max && c.Diagonal == c0.Diagonal && c.Numbers.Length == c0.Numbers.Length && c.Numbers.SequenceEqual(c0.Numbers);
      if (eq)
        return false;
    }
    list.Push(c);
    return true;
  }

  /// <summary>Copy Sudoku array to another.</summary>
  /// <param name="source">Affected source array.</param>
  /// <param name="dest">Affected destination array.</param>
  private static void Copy(int[] source, int[] dest)
  {
    if (dest == null)
      return;
    var min = 0;
    var max = dest.Length;
    if (source != null)
      min = Math.Min(source.Length, dest.Length);
    for (var i = 0; i < max; i++)
    {
      if (i < min)
        dest[i] = source[i];
      else
        dest[i] = -1; // not used
    }
  }

  /// <summary>Gets clone of array.</summary>
  /// <param name="source">Affected array.</param>
  private static int[] Clone(int[] source)
  {
    return (int[])source.Clone();
  }

  /// <summary>Creates int array.</summary>
  /// <param name="count">Affected count.</param>
  private static int[] NumberArray(int count)
  {
    if (count < 0)
      return null;
    return new int[count];
  }

  /// <summary>Gets count of filled (>0) fields.</summary>
  /// <param name="source">Affected array.</param>
  private static int Count(int[] source)
  {
    if (source == null)
      return 0;
    return source.Count(a => a > 0);
  }

  /// <summary>Gets number of filled (>0) fields.</summary>
  /// <returns>Number of filled (>0) fields.</returns>
  public int Count()
  {
    return Numbers.Count(a => a > 0);
  }

  /// <summary>Clear all fields.</summary>
  public void Clear()
  {
    for (var i = 0; i < Numbers.Length; i++)
    {
      Numbers[i] = 0;
    }
  }

  /// <summary>Test sudoku for discrepancy.</summary>
  /// <param name="c">Affected sudoku context.</param>
  /// <param name="exception">Throw exception if error?</param>
  /// <returns>-1 OK, >= 0 field number with error.</returns>
  public static int Test(SudokuContext c, bool exception)
  {
    var feld = -1;

    try
    {
      // Zeilen, Spalten und Kästen bestimmen
      var zeilen = NumberArray(c.Maxx * c.Maxx);
      var spalten = NumberArray(c.Maxx * c.Maxx);
      var kaesten = NumberArray(c.Maxx * c.Maxx);
      var diagonalen = NumberArray(c.Maxx * 2);
      for (var row = 0; row < c.Maxy; row++)
      {
        for (var col = 0; col < c.Maxx; col++)
        {
          var wert = c.Numbers[row * c.Maxx + col];
          if (wert > 0)
          {
            var knr = (row / c.Maxyb) * c.Maxyb + (col / c.Maxxb);
            if (zeilen[row * c.Maxx + wert - 1] == 0)
            {
              zeilen[row * c.Maxx + wert - 1] = wert;
            }
            else
            {
              if (exception)
                throw new MessageException(SO007(row + 1, wert));
              return row * c.Maxx + col;
            }
            if (spalten[col * c.Maxx + wert - 1] == 0)
            {
              spalten[col * c.Maxx + wert - 1] = wert;
            }
            else
            {
              if (exception)
                throw new MessageException(SO008(col + 1, wert));
              return row * c.Maxx + col;
            }
            if (kaesten[knr * c.Maxx + wert - 1] == 0)
            {
              kaesten[knr * c.Maxx + wert - 1] = wert;
            }
            else
            {
              if (exception)
                throw new MessageException(SO009(knr + 1, wert));
              return row * c.Maxx + col;
            }
            if (c.Diagonal)
            {
              if (row == col)
              {
                if (diagonalen[wert - 1] == 0)
                {
                  diagonalen[wert - 1] = wert;
                }
                else
                {
                  if (exception)
                    throw new MessageException(SO010(1, row + 1, wert));
                  return row * c.Maxx + col;
                }
              }
              if (row == c.Maxx - 1 - col)
              {
                if (diagonalen[c.Maxx + wert - 1] == 0)
                {
                  diagonalen[c.Maxx + wert - 1] = wert;
                }
                else
                {
                  if (exception)
                    throw new MessageException(SO010(2, row + 1, wert));
                  return row * c.Maxx + col;
                }
              }
            }
          }
        }
      }
    }
    finally
    {
      if (feld == -1 && SudokuContext.Count(c.Numbers) >= c.Maxx * c.Maxy)
      {
#pragma warning disable IDE0059
        feld = -2; // vollständig gelöst
#pragma warning restore IDE0059
      }
    }
    return feld;
  }

  /// <summary>Find a single solution number for a sudoku.</summary>
  /// <param name="c">Affected sudoku context.</param>
  /// <param name="maxcount">Find field with a maximum number of possibilities.</param>
  /// <returns>Number of field which is changed or -1 no solution found -2 completely solved -3 solvable with variants.</returns>
  private static int SolveSingle(SudokuContext c, int maxcount, int[] feldv, int[] varianten)
  {
    var feld = -1; // no solution

    try
    {
      // Zeilen, Spalten und Kästen bestimmen
      var zeilen = NumberArray(c.Maxx * c.Maxx);
      var spalten = NumberArray(c.Maxx * c.Maxx);
      var kaesten = NumberArray(c.Maxx * c.Maxx);
      var diagonalen = NumberArray(c.Maxx * 2);
      for (var row = 0; row < c.Maxx; row++)
      {
        for (var col = 0; col < c.Maxx; col++)
        {
          var wert = c.Numbers[row * c.Maxx + col];
          if (wert > 0)
          {
            var knr = (row / c.Maxyb) * c.Maxyb + (col / c.Maxxb);
            zeilen[row * c.Maxx + wert - 1] = wert;
            spalten[col * c.Maxx + wert - 1] = wert;
            kaesten[knr * c.Maxx + wert - 1] = wert;
            if (row == col)
            {
              // 1. Diagonale
              diagonalen[wert - 1] = wert;
            }
            if (row == c.Maxx - 1 - col)
            {
              // 2. Diagonale
              diagonalen[c.Maxx + wert - 1] = wert;
            }
          }
        }
      }
      // neue Zahl bestimmen, wenn nur noch eine fehlt
      for (var row = 0; row < c.Maxx; row++)
      {
        for (var col = 0; col < c.Maxx; col++)
        {
          var wert = c.Numbers[row * c.Maxx + col];
          // leeres Feld untersuchen
          if (wert == 0)
          {
            var knr = (row / c.Maxyb) * c.Maxyb + (col / c.Maxxb);
            var versuchz = 0;
            var versuchs = 0;
            var versuchk = 0;
            var versuch1 = 0;
            var versuch2 = 0;
            var anzahlz = 0;
            var anzahls = 0;
            var anzahlk = 0;
            var anzahl1 = 0;
            var anzahl2 = 0;
            var varianten1 = NumberArray(c.Maxx);
            var varianten2 = NumberArray(c.Maxx);
            var variantenZ = NumberArray(c.Maxx);
            var variantenS = NumberArray(c.Maxx);
            var variantenK = NumberArray(c.Maxx);
            for (var i = 0; i < c.Maxx; i++)
            {
              varianten[i] = 0;
            }
            for (var i = 0; i < c.Maxx; i++)
            {
              if (zeilen[row * c.Maxx + i] == 0)
              {
                versuchz = i + 1;
                anzahlz++;
                variantenZ[i] = 1;
              }
              if (spalten[col * c.Maxx + i] == 0)
              {
                versuchs = i + 1;
                anzahls++;
                variantenS[i] = 1;
              }
              if (kaesten[knr * c.Maxx + i] == 0)
              {
                versuchk = i + 1;
                anzahlk++;
                variantenK[i] = 1;
              }
              if (c.Diagonal)
              {
                if (row == col)
                {
                  // 1. Diagonale
                  if (diagonalen[i] == 0)
                  {
                    versuch1 = i + 1;
                    anzahl1++;
                    varianten1[i] = 1;
                  }
                }
                if (row == c.Maxx - 1 - col)
                {
                  // 2. Diagonale
                  if (diagonalen[c.Maxx + i] == 0)
                  {
                    versuch2 = i + 1;
                    anzahl2++;
                    varianten2[i] = 1;
                  }
                }
              }
            }
            // Genau eine Zahl passt in der Zeile.
            if (anzahlz == 1)
            {
              if (anzahls < 1 || anzahlk < 1)
              {
                throw new Exception($"Widerspruch Zeile in ({(row + 1)},{(col + 1)})");
              }
              c.Numbers[row * c.Maxx + col] = versuchz;
              feld = row * c.Maxx + col;
              return feld;
            }
            // Genau eine Zahl passt in der Spalte.
            if (anzahls == 1)
            {
              if (anzahlz < 1 || anzahlk < 1)
              {
                throw new Exception($"Widerspruch Spalte in ({(row + 1)},{(col + 1)})");
              }
              c.Numbers[row * c.Maxx + col] = versuchs;
              feld = row * c.Maxx + col;
              return feld;
            }
            // Genau eine Zahl passt im Kasten.
            if (anzahlk == 1)
            {
              if (anzahlz < 1 || anzahls < 1)
              {
                throw new Exception($"Widerspruch Kasten in ({(row + 1)},{(col + 1)})");
              }
              c.Numbers[row * c.Maxx + col] = versuchk;
              feld = row * c.Maxx + col;
              return feld;
            }
            // Genau eine Zahl passt in Diagonale 1.
            if (anzahl1 == 1)
            {
              c.Numbers[row * c.Maxx + col] = versuch1;
              feld = row * c.Maxx + col;
              return feld;
            }
            // Genau eine Zahl passt in Diagonale 2.
            if (anzahl2 == 1)
            {
              c.Numbers[row * c.Maxx + col] = versuch2;
              feld = row * c.Maxx + col;
              return feld;
            }
            var anzahlv = 0; // Anzahl Varianten.
            for (var i = 0; i < c.Maxx; i++)
            {
              if (variantenZ[i] > 0 && variantenS[i] > 0 && variantenK[i] > 0)
              {
                varianten[anzahlv] = i + 1;
                anzahlv++;
              }
            }
            if (anzahlv == 1)
            {
              c.Numbers[row * c.Maxx + col] = varianten[0];
              feld = row * c.Maxx + col;
              return feld;
            }
            else if (anzahlv <= maxcount)
            {
              feldv[0] = row * c.Maxx + col;
              if (varianten[0] == 0)
              {
                return -1;
              }
              return -3; // Lösen mit Varianten
            }
          }
        }
      }
      // neue Zahl für einen Kasten bestimmen mit Ausschluss über Zeilen und Spalten
      var anzahl = NumberArray(c.Maxx);
      var pos = NumberArray(c.Maxx);
      for (var krow = 0; krow < c.Maxyb; krow++)
      {
        for (var kcol = 0; kcol < c.Maxxb; kcol++)
        {
          // Untersuchung eines Kastens
          for (var i = 0; i < c.Maxx; i++)
          {
            anzahl[i] = 0;
            pos[i] = -1;
            if (kaesten[(krow * c.Maxxb + kcol) * c.Maxx + i] > 0)
            {
              // Zahl ist erledigt.
              anzahl[i] = -1;
            }
          }
          var knr = krow * c.Maxxb + kcol;
          for (var irow = 0; irow < c.Maxyb; irow++)
          {
            for (var icol = 0; icol < c.Maxxb; icol++)
            {
              var row = krow * c.Maxxb + irow;
              var col = kcol * c.Maxyb + icol;
              var wert = c.Numbers[row * c.Maxx + col];
              if (wert == 0)
              {
                for (var i = 0; i < c.Maxx; i++)
                {
                  if (anzahl[i] >= 0 && zeilen[row * c.Maxx + i] == 0 && spalten[col * c.Maxx + i] == 0
                    && kaesten[knr * c.Maxx + i] == 0)
                  {
                    anzahl[i]++;
                    pos[i] = row * c.Maxx + col;
                  }
                }
              }
            }
          }
          for (var i = 0; i < c.Maxx; i++)
          {
            if (anzahl[i] == 1)
            {
              feld = pos[i];
              c.Numbers[feld] = i + 1;
              return feld;
            }
          }
        }
      }
    }
    finally
    {
      if (feld == -1 && SudokuContext.Count(c.Numbers) >= c.Maxx * c.Maxy)
      {
#pragma warning disable IDE0059
        feld = -2; // vollständig gelöst
#pragma warning restore IDE0059
      }
    }
    return feld;
  }

  /// <summary>Try to solve a sudoku.</summary>
  /// <param name="c">Affected sudoku context.</param>
  /// <param name="move1">Find only one number?</param>
  public static void Solve(SudokuContext c, bool move1)
  {
    var ende = false;
    int[] clone1 = null;
    int[] loesung = null;
    var list = new Stack<int[]>();
    var feld = NumberArray(1);
    var varianten = NumberArray(c.Maxx);

    SudokuContext.Test(c, true);
    if (SudokuContext.Count(c.Numbers) >= c.Maxx * c.Maxy)
    {
      throw new MessageException(SO011);
    }
    if (move1)
    {
      clone1 = SudokuContext.Clone(c.Numbers);
    }
    int ergebnis;
    do
    {
      var anzahl = 0;
      do
      {
        anzahl++;
        ergebnis = SudokuContext.SolveSingle(c, anzahl, feld, varianten);
        // System.out.println("Anzahl: " + miAnzahl + " Variante: " +
        // varianten + " Ergebnis: " + ergebnis);
        if (ergebnis == -3)
        {
          c.Numbers[feld[0]] = varianten[0];
          // Andere Varianten merken.
          for (var i = 1; i < anzahl; i++)
          {
            var clone = SudokuContext.Clone(c.Numbers);
            clone[feld[0]] = varianten[i];
            list.Push(clone);
          }
          ergebnis = 0;
        }
        else if (ergebnis >= 0)
        {
          if (SudokuContext.Test(c, false) >= 0)
          {
            // Andere Variante versuchen wegen Widerspruch.
            if (list.Count <= 0)
            {
              if (loesung == null)
              {
                throw new MessageException(SO012);
              }
              ende = true;
            }
            else
            {
              SudokuContext.Copy(list.Pop(), c.Numbers);
            }
          }
          else if (move1 && !list.Any())
          {
            ende = true;
          }
        }
      } while (!ende && anzahl < c.Maxx && (ergebnis == -1));
      if (SudokuContext.Count(c.Numbers) >= c.Maxx * c.Maxx)
      {
        if (loesung == null)
        {
          loesung = SudokuContext.Clone(c.Numbers);
          // Andere Variante versuchen.
          if (list.Any())
          {
            SudokuContext.Copy(list.Pop(), c.Numbers);
          }
        }
        else
        {
          SudokuContext.Copy(loesung, c.Numbers);
          if (list.Any())
          {
            throw new MessageException(SO013);
          }
        }
      }
    } while (!ende && ergebnis >= 0);
    if (loesung != null)
    {
      SudokuContext.Copy(loesung, c.Numbers);
    }
    if (move1)
    {
      var i = 0;
      var clone2 = SudokuContext.Clone(c.Numbers);
      for (; i < clone1.Length; i++)
      {
        if (clone1[i] != clone2[i])
        {
          clone1[i] = clone2[i];
          break;
        }
      }
      if (i >= clone1.Length)
        throw new MessageException(SO014);
      SudokuContext.Copy(clone1, c.Numbers);
    }
    else if (SudokuContext.Count(c.Numbers) < c.Maxx * c.Maxx)
      throw new MessageException(SO015);
  }
}
