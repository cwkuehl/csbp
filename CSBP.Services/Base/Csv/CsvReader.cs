// <copyright file="CsvReader.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Zum Lesen von Windows-1252- oder UTF-8-Zeichen aus einem Stream als Zeilen oder Csv-Zeilen.
/// </summary>
public class CsvReader
{
  /// <summary>UTF-8 Byte Order Mark.</summary>
  private static readonly byte[] Utf8bom = new byte[] { 0xEF, 0xBB, 0xBF };

  /// <summary>Interner Byte-Puffer.</summary>
  private readonly byte[] bytes = new byte[2000]; // >= 10

  /// <summary>Interner Zeichen-Puffer.</summary>
  private readonly char[] chars = null;

  /// <summary>Aktuelles Zeichen im Zeichen-Puffer.</summary>
  private int charindex = -1;

  /// <summary>Anzahl Anzahl der Zeichen im Puffer.</summary>
  private int maxchars = -1;

  /// <summary>
  /// Initializes a new instance of the <see cref="CsvReader" /> class.
  /// </summary>
  /// <param name="ms">Betroffener Stream.</param>
  public CsvReader(Stream ms)
  {
      Ms = ms ?? new MemoryStream(new byte[0]);
      CsvEncoding = CheckEncoding();
      Ms.Position = 0;
      chars = new char[CsvEncoding.GetMaxCharCount(bytes.Length)];
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="CsvReader" /> class.
  /// </summary>
  /// <param name="bytes">Betroffene Bytes.</param>
  public CsvReader(byte[] bytes)
    : this(new MemoryStream(bytes ?? new byte[0]))
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="CsvReader" /> class.
  /// </summary>
  /// <param name="filename">Betroffene Datei.</param>
  public CsvReader(string filename)
    : this(new FileStream(filename ?? "", FileMode.Open, FileAccess.Read))
  {
  }

  /// <summary>Gets a value indicating whether the stream is at the end.</summary>
  public bool IsEnd { get; private set; }

  /// <summary>Gets the byte encoding.</summary>
  public Encoding CsvEncoding { get; private set; }

  /// <summary>Gets or sets the internal stream.</summary>
  private Stream Ms { get; set; }

  /// <summary>
  /// Parsen einer CSV-Zeile (ohne Zeilenumbrüche am Ende) liefert Array mit Werten. Es kann keinen Fehler geben.
  /// </summary>
  /// <param name="s">Betroffene CSV-Zeile.</param>
  /// <param name="t1">Erster möglicher Spalten-Trenner.</param>
  /// <param name="t2">Zweiter möglicher Spalten-Trenner.</param>
  /// <returns>Werte einer CSV-Zeile als Array.</returns>
  public static string[] ParseCsvLine(string s, char t1 = ';', char t2 = ';')
  {
      if (string.IsNullOrEmpty(s))
          return new string[0];
      string[] array = null;
      int spalte = 0;
      var list = new List<StringBuilder> { new StringBuilder() }; // 1. Spalte leer
      bool isend;
      var i = 0;
      var l = s.Length;
      var state = 0;
      do
      {
          var sb = list[spalte];
          var c = s[i]; // Peek
          switch (state)
          {
              case 0:
                  if (c == '\"')
                  {
                      i++;
                      state = 10; // Zeichenkette mit "
                  }
                  else if (c == t1 || c == t2)
                  {
                      list.Add(new StringBuilder());
                      spalte++;
                      i++;
                      state = 0; // nächste Spalte
                  }
                  else if (c == '\r')
                  {
                      i++;
                      state = 90; // Zeilenende-Anfang
                  }
                  else if (c == '\n')
                  {
                      i++;
                      state = 95; // Zeilenende-Ende
                  }
                  else
                  {
                      sb.Append(c);
                      i++;
                      state = 50; // normale Zeichenkette ohne "
                  }
                  break;
              case 10: // Zeichenkette mit "
                  if (c == '\"')
                  {
                      i++;
                      state = 20; // Zeichenketten-Ende oder erstes doppeltes "
                  }
                  else
                  {
                      sb.Append(c);
                      i++;
                  }
                  break;
              case 20: // Zeichenketten-Ende oder erstes doppeltes "
                  if (c == '\"')
                  {
                      // zweites Anführungszeichen
                      sb.Append(c);
                      i++;
                      state = 10; // Zeichenkette mit "
                  }
                  else if (c == t1 || c == t2)
                  {
                      list.Add(new StringBuilder());
                      spalte++;
                      i++;
                      state = 0; // nächste Spalte
                  }
                  else
                  {
                      sb.Append(c);
                      i++;
                  }
                  break;
              case 50: // Zeichenketten ohne "
                  if (c == t1 || c == t2)
                  {
                      list.Add(new StringBuilder());
                      spalte++;
                      i++;
                      state = 0; // nächste Spalte
                  }
                  else if (c == '\r')
                  {
                      i++;
                      state = 90; // Zeilenende-Anfang
                  }
                  else if (c == '\n')
                  {
                      i++;
                      state = 95; // Zeilenende-Ende
                  }
                  else
                  {
                      sb.Append(c);
                      i++;
                  }
                  break;
              case 90: // Zeilenende-Anfang mit CR
                  if (c == '\n')
                  {
                      i++;
                  }
                  state = 95; // Zeilenende-Ende
                  break;
              case 95: // Zeilenende-Ende
                  state = 100; // Ende, nächstes Zeichen noch lesen für evtl. komplettes Ende
                  break;
          }
          isend = i >= l;
          if (isend)
          {
              if (state == 10)
                  throw new Exception("CSV-Format-Fehler");
              break;
          }
      }
      while (!isend && state != 100);
      if (array == null || array.Length < spalte + 1)
          array = new string[spalte + 1];
      for (var j = 0; j <= spalte; j++)
      {
          array[j] = list[j].ToString();
      }
      return array;
  }

  /// <summary>
  /// Lesen der nächsten Zeile bis zum Ende oder den Zeichen CRLF, CR oder LF.
  /// </summary>
  /// <returns>Nächste Zeile oder null, wenn Zeichen zu Ende sind.</returns>
  public string GetLine()
  {
      if (IsEnd)
          return null;
      var sb = new StringBuilder();
      do
      {
          var c = GetChar();
          if (IsEnd)
              break;
          if (c == '\r')
          {
              // CRLF oder nur CR.
              var c1 = GetChar(true);
              if (!IsEnd && c1 == '\n')
              {
                  GetChar();
                  GetChar(true);
              }
              break;
          }
          else if (c == '\n')
          {
              GetChar(true);
              break;
          }
          sb.Append(c);
      }
      while (!IsEnd);
      return sb.ToString();
  }

  /// <summary>
  /// Liefert alle (restlichen) Zeilen.
  /// </summary>
  /// <returns>Liste von Zeilen.</returns>
  public List<string> GetLines()
  {
      var list = new List<string>();
      string l;
      while ((l = GetLine()) != null)
          list.Add(l);
      return list;
  }

  /// <summary>
  /// Lesen der nächsten CSV-Zeile bis zum Ende oder den Zeichen CRLF, CR oder LF,
  /// die nicht in einer Anführungszeichen-begrenzten Zeichenkette sind.
  /// Es gibt eine Exception 'CSV-Format-Fehler', falls die letzte Spalte nicht mit Anführungszeichen abgeschlossen wird,
  /// obwohl sie damit begonnen wurde.
  /// </summary>
  /// <param name="t1">Erster Spalten-Trenner.</param>
  /// <param name="t2">Zweiter Spalten-Trenner.</param>
  /// <param name="t3">Dritter Spalten-Trenner.</param>
  /// <returns>Nächste CSV-Zeile oder null, wenn Zeichen zu Ende sind.</returns>
  public string GetCsvLine(char t1 = ';', char t2 = ';', char t3 = ';')
  {
      if (IsEnd)
          return null;
      var sb = new StringBuilder();
      var state = 0;
      do
      {
          var c = GetChar(true);
          if (IsEnd)
          {
              if (state == 10)
                  throw new Exception("CSV-Format-Fehler");
              break;
          }
          switch (state)
          {
              case 0:
                  if (c == '\"')
                  {
                      sb.Append(GetChar());
                      state = 10; // Zeichenkette mit "
                  }
                  else if (c == t1 || c == t2 || c == t3)
                  {
                      sb.Append(GetChar());
                      state = 0; // nächste Spalte
                  }
                  else if (c == '\r')
                  {
                      GetChar();
                      state = 90; // Zeilenende-Anfang
                  }
                  else if (c == '\n')
                  {
                      GetChar();
                      state = 95; // Zeilenende-Ende
                  }
                  else
                  {
                      sb.Append(GetChar());
                      state = 50; // normale Zeichenkette ohne "
                  }
                  break;
              case 10: // Zeichenkette mit "
                  if (c == '\"')
                  {
                      state = 20; // Zeichenketten-Ende oder erstes doppeltes "
                  }
                  sb.Append(GetChar());
                  break;
              case 20: // Zeichenketten-Ende oder erstes doppeltes "
                  if (c == '\"')
                  {
                      // zweites Anführungszeichen
                      sb.Append(GetChar());
                      state = 10; // Zeichenkette mit "
                  }
                  else if (c == t1 || c == t2 || c == t3)
                  {
                      sb.Append(GetChar());
                      state = 0; // nächste Spalte
                  }
                  else if (c == '\r')
                  {
                      GetChar();
                      state = 90; // Zeilenende-Anfang
                  }
                  else if (c == '\n')
                  {
                      GetChar();
                      state = 95; // Zeilenende-Ende
                  }
                  break;
              case 50: // Zeichenketten ohne "
                  if (c == t1 || c == t2 || c == t3)
                  {
                      sb.Append(GetChar());
                      state = 0; // nächste Spalte
                  }
                  else if (c == '\r')
                  {
                      GetChar();
                      state = 90; // Zeilenende-Anfang
                  }
                  else if (c == '\n')
                  {
                      GetChar();
                      state = 95; // Zeilenende-Ende
                  }
                  else
                      sb.Append(GetChar());
                  break;
              case 90: // Zeilenende-Anfang mit CR
                  if (c == '\n')
                  {
                      GetChar();
                  }
                  state = 95; // Zeilenende-Ende
                  break;
              case 95: // Zeilenende-Ende
                  state = 100; // Ende, nächstes Zeichen noch lesen für evtl. komplettes Ende
                  break;
          }
      }
      while (!IsEnd && state != 100);
      return sb.ToString();
  }

  /// <summary>
  /// Lesen aller nächsten CSV-Zeilen.
  /// </summary>
  /// <param name="t1">Erster Spalten-Trenner.</param>
  /// <param name="t2">Zweiter Spalten-Trenner.</param>
  /// <param name="t3">Dritter Spalten-Trenner.</param>
  /// <returns>Liste von CSV-Zeilen.</returns>
  public List<string> GetCsvLines(char t1 = ';', char t2 = ';', char t3 = ';')
  {
      var list = new List<string>();
      string l;
      while ((l = GetCsvLine(t1, t2, t3)) != null)
          list.Add(l);
      return list;
  }

  /// <summary>
  /// Liest das nächste Zeichen. Falls es keine Zeichen mehr gibt wird 0 zurückgegeben.
  /// 0 heißt aber nicht automatisch Ende. Entscheidend ist der Wert von IsEnd.
  /// </summary>
  /// <param name="peek">Nur gucken, es wird nicht weiter gespult.</param>
  /// <returns>Nächstes Zeichen oder 0, falls Ende.</returns>
  public char GetChar(bool peek = false)
  {
      if (IsEnd)
          return (char)0;
      if (maxchars < 0 || charindex >= maxchars)
      {
          var baseindex = 0;
          var maxbytes = bytes.Length - 3; // Es können max. 3 Bytes am Ende fehlen, weil UTF-8-Characters aus max. 4 Bytes bestehen.
          var inputByteCount = Ms.Read(bytes, 0, maxbytes);
          if (inputByteCount >= maxbytes)
          {
              // Evtl. fehlende Bytes nachlesen.
              var nachlesen = 0;
              if ((bytes[inputByteCount - 1] & (1 << 7)) != 0)
              {
                  // Letztes Byte gehört zu einer Gruppe von Bytes.
                  if ((bytes[inputByteCount - 1] & 0xF8) == 0xF0)
                      nachlesen = 3; // 0b11110000 = 0xF0
                  else if ((bytes[inputByteCount - 1] & 0xF0) == 0xE0)
                      nachlesen = 2; // 0b11100000 = 0xE0
                  else if ((bytes[inputByteCount - 1] & 0xE0) == 0xC0)
                      nachlesen = 1; // 0b11000000 = 0xC0
                  else if ((bytes[inputByteCount - 2] & 0xF8) == 0xF0)
                      nachlesen = 2; // 0b11110000 = 0xF0
                  else if ((bytes[inputByteCount - 2] & 0xF0) == 0xE0)
                      nachlesen = 1; // 0b11100000 = 0xE0
                  else if ((bytes[inputByteCount - 3] & 0xF8) == 0xF0)
                      nachlesen = 1; // 0b11110000 = 0xF0
              }
              if (nachlesen > 0)
              {
                  var ibc = Ms.Read(bytes, inputByteCount, nachlesen);
                  inputByteCount += ibc;
              }
          }
          if (charindex < 0 && inputByteCount >= Utf8bom.Length)
          {
              // Evtl. vorhandenen BOM entfernen.
              var bom = true;
              for (var i = 0; i < Utf8bom.Length && bom; i++)
              {
                  if (bytes[i] != Utf8bom[i])
                      bom = false;
              }
              if (bom)
              {
                  baseindex = Utf8bom.Length;
                  inputByteCount -= Utf8bom.Length;
              }
          }
          charindex = 0;
          if (inputByteCount > 0)
          {
              maxchars = CsvEncoding.GetChars(bytes, baseindex, inputByteCount, chars, 0);
          }
          else
              maxchars = -1;
          if (inputByteCount <= 0 || maxchars <= 0)
          {
              IsEnd = true;
              return (char)0;
          }
      }
      var c = chars[charindex];
      if (!peek)
          charindex++;
      return c;
  }

  /// <summary>
  /// Lesen der aller Zeichen als ein String.
  /// </summary>
  /// <returns>String oder null, wenn Zeichen zu Ende sind.</returns>
  public string GetString()
  {
      if (IsEnd)
          return null;
      var sb = new StringBuilder();
      do
      {
          var c = GetChar();
          if (IsEnd)
              break;
          sb.Append(c);
      }
      while (!IsEnd);
      return sb.ToString();
  }

  /// <summary>
  /// Liefert die Codierung der Daten: Windows-1252 oder UTF-8.
  /// </summary>
  /// <returns>Bestimmte Codierung.</returns>
  private Encoding CheckEncoding()
  {
      var utf8 = true;
      var next = 0;
      int c;
      while (utf8 && (c = Ms.ReadByte()) != -1)
      {
          if (next > 0)
          {
              // Folge-Byte prüfen: 0b10xxxxxx
              if ((c & 0xC0) == 0x80)
                  next--;
              else
                  utf8 = false; // Kein korrektes UTF-8-Folge-Byte.
          }
          else if ((c & 0x80) == 0x80)
          {
              // Start-Byte prüfen
              if ((c & 0xF8) == 0xF0)
                  next = 3; // 0b11110xxx = 0xF0
              else if ((c & 0xF0) == 0xE0)
                  next = 2; // 0b1110xxxx = 0xE0
              else if ((c & 0xE0) == 0xC0)
                  next = 1; // 0b110xxxxx = 0xC0
              else
                  utf8 = false; // Kein korrektes UTF-8-Start-Byte.
          }
      }
      if (utf8 && next > 0)
          utf8 = false; // Folge-Bytes fehlen.
      return utf8 ? Encoding.UTF8 : Encoding.GetEncoding(1252);
  }
}
