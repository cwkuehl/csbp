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
    private DateTime? datum = null;
    /** Inhalt der Säule, bestehend aus X oder O. */
    private StringBuilder sb = new StringBuilder();
    /** Inhalt der Säule, bestehend aus X oder O oder Monatszeichen (1-C). */
    private StringBuilder sbm = new StringBuilder();

    public PnfColumn(decimal min, decimal max, int boxtyp, int anzahl, PnfDate datum)
    {
      this.min = min;
      this.max = max;
      this.boxtyp = boxtyp;
      this.datum = datum.getDatum();
      for (int i = 0; i < anzahl; i++)
      {
        zeichne(datum);
      }
    }

    public void zeichne(PnfDate datum)
    {
      char c = boxtyp == 1 ? 'X' : 'O';
      char cm = datum.getNeuerMonat(c);
      sb.Append(c);
      sbm.Append(cm);
      datum.setMonatVerwendet();
    }

    public bool isO()
    {
      if (sb.Length <= 0)
      {
        return getBoxtyp() == 2;
      }
      char eins = sb[0];
      // return (getBoxtyp() == 2 && eins == 'O');
      return eins == 'O';
    }

    public int getSize()
    {
      return sb.Length;
    }

    public string getString()
    {
      return sb.ToString();
    }

    public char[] getChars()
    {
      if (sbm.Length <= 0)
      {
        return new char[0];
      }
      var array = sbm.ToString().ToCharArray();
      if (getBoxtyp() == 2)
      {
        Array.Reverse(array);
      }
      return array;
    }

    public decimal getMin()
    {
      return min ?? decimal.MinValue;
    }

    /**
     * Setzen eines neuen Maximums. Wenn das Maximum erhöht wird, wird ein X gezeichnet.
     * @param max Neues mögliches Maximum.
     */
    public void setMin(decimal min, PnfDate datum)
    {
      if (Functions.compDouble4(this.min, min) > 0)
      {
        this.min = min;
        zeichne(datum);
      }
    }

    public decimal getMax()
    {
      return max ?? decimal.MaxValue;
    }

    /**
     * Setzen eines neuen Maximums. Wenn das Maximum erhöht wird, wird ein X gezeichnet.
     * @param max Neues mögliches Maximum.
     */
    public void setMax(decimal max, PnfDate datum)
    {
      if (Functions.compDouble4(this.max, max) < 0)
      {
        this.max = max;
        zeichne(datum);
      }
    }

    public int getBoxtyp()
    {
      return boxtyp;
    }

    public void setBoxtyp(int boxtyp)
    {
      this.boxtyp = boxtyp;
    }

    public int getYpos()
    {
      return ypos;
    }

    public void setYpos(int pos)
    {
      this.ypos = pos;
    }

    public int getYtop()
    {
      return ypos + getSize();
    }

    public DateTime? getDatum()
    {
      return datum;
    }
  }
}
