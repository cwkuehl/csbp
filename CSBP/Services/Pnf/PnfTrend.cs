// <copyright file="PnfTrend.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Pnf
{
  /// <summary>Point and Figure-Trennlinien mit 45° oder 0°.</summary>
  public class PnfTrend
  {
    /** Start-Position der Säule. */
    private int xpos = 0;
    /** Start-Position der Säule. */
    private int ypos = 0;
    /** Boxtyp: 0 horizontal; 1 aufwärts (X); 2 abwärts (O). */
    private int boxtyp = 0;
    /** Länge der Trendlinie in Anzahl Säulen. */
    private int laenge = 0;

    /**
     * Konstruktor mit Initialisierung.
     * @param x Startpunkt der Linie.
     * @param y Startpunkt der Linie.
     * @param aufwaerts 0 horizontal; 1 aufwärts (X); 2 abwärts (O).
     */
    public PnfTrend(int x, int y, int aufwaerts)
    {
      this.xpos = x;
      this.ypos = y;
      this.boxtyp = aufwaerts;
      this.laenge = 1;
    }

    public int Laenge
    {
      get { return laenge; }
    }

    public void setLaenge(int laenge)
    {
      this.laenge = laenge;
    }

    public int Xpos
    {
      get { return xpos; }
    }

    public int Ypos
    {
      get { return ypos; }
    }

    public int Boxtyp
    {
      get { return boxtyp; }
    }
  }
}
