// <copyright file="PnfTrend.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Pnf
{
  /// <summary>Point and Figure trend line mit 45° oder 0°.</summary>
  public class PnfTrend
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PnfTrend"/> class.
    /// </summary>
    /// <param name="x">X coordinate of starting point.</param>
    /// <param name="y">Y coordinate of starting point.</param>
    /// <param name="aufwaerts">Affected box type: 0 horizontal, 1 up X, 2 down O.</param>
    public PnfTrend(int x, int y, int aufwaerts)
    {
      Xpos = x;
      Ypos = y;
      Boxtype = aufwaerts;
      Laenge = 1;
    }

    /// <summary>Gets or sets the length of line.</summary>
    public int Laenge { get; set; }

    /// <summary>Gets x coordinate of starting point.</summary>
    public int Xpos { get; private set; }

    /// <summary>Gets y coordinate of starting point.</summary>
    public int Ypos { get; private set; }

    /// <summary>Gets box type: 0 horizontal, 1 up X, 2 down O.</summary>
    public int Boxtype { get; private set; }
  }
}
