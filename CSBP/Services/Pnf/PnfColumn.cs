// <copyright file="PnfColumn.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

using System;
using System.Text;
using CSBP.Base;

namespace CSBP.Services.Pnf
{
  public class PnfColumn
  {
    /** Minimum der Säule. */
    private decimal? min = null;
    /** Maximum der Säule. */
    private decimal? max = null;
    /** Position der Säule. */
    private int ypos = 0;
    /** Boxtyp: 0 unbestimmt; 1 aufwärts (X); 2 abwärts (O). */
    private int boxtyp = 0;
    /** Anfangsdatum der Säule. */
    private readonly DateTime? datum = null;
    /** Inhalt der Säule, bestehend aus X oder O. */
    private readonly StringBuilder sb = new();
    /** Inhalt der Säule, bestehend aus X oder O oder Monatszeichen (1-C). */
    private readonly StringBuilder sbm = new();

    public PnfColumn(decimal min, decimal max, int boxtyp, int anzahl, PnfDate datum)
    {
      this.min = min;
      this.max = max;
      this.boxtyp = boxtyp;
      this.datum = datum.GetDatum();
      for (int i = 0; i < anzahl; i++)
      {
        Zeichne(datum);
      }
    }

    public void Zeichne(PnfDate datum)
    {
      char c = boxtyp == 1 ? 'X' : 'O';
      char cm = datum.GetNeuerMonat(c);
      sb.Append(c);
      sbm.Append(cm);
      datum.SetMonatVerwendet();
    }

    public bool IsO
    {
      get
      {
        if (sb.Length <= 0)
        {
          return Boxtyp == 2;
        }
        char eins = sb[0];
        // return (getBoxtyp() == 2 && eins == 'O');
        return eins == 'O';
      }
    }

    public int Size
    {
      get
      {
        return sb.Length;
      }
    }

    public string String
    {
      get
      {
        return sb.ToString();
      }
    }

    public char[] Chars
    {
      get
      {
        if (sbm.Length <= 0)
        {
          return Array.Empty<char>();
        }
        var array = sbm.ToString().ToCharArray();
        if (Boxtyp == 2)
        {
          Array.Reverse(array);
        }
        return array;
      }
    }

    public decimal Min
    {
      get
      {
        return min ?? decimal.MinValue;
      }
    }

    /**
     * Setzen eines neuen Maximums. Wenn das Maximum erhöht wird, wird ein X gezeichnet.
     * @param max Neues mögliches Maximum.
     */
    public void SetMin(decimal min, PnfDate datum)
    {
      if (Functions.CompDouble4(this.min, min) > 0)
      {
        this.min = min;
        Zeichne(datum);
      }
    }

    public decimal Max
    {
      get
      {
        return max ?? decimal.MaxValue;
      }
    }

    /**
     * Setzen eines neuen Maximums. Wenn das Maximum erhöht wird, wird ein X gezeichnet.
     * @param max Neues mögliches Maximum.
     */
    public void SetMax(decimal max, PnfDate datum)
    {
      if (Functions.CompDouble4(this.max, max) < 0)
      {
        this.max = max;
        Zeichne(datum);
      }
    }

    public int Boxtyp
    {
      get
      {
        return boxtyp;
      }
      set
      {
        this.boxtyp = value;
      }
    }

    public int Ypos
    {
      get
      {
        return ypos;
      }
      set
      {
        this.ypos = value;
      }
    }

    public int Ytop
    {
      get
      {
        return ypos + Size;
      }
    }

    public DateTime? Datum
    {
      get
      {
        return datum;
      }
    }
  }
}
