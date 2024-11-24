// <copyright file="StartDialog.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base;

using System;
using System.Collections.Generic;
using CSBP.Forms.AG;
using CSBP.Forms.AM;
using CSBP.Forms.FZ;
using CSBP.Forms.HH;
using CSBP.Forms.SB;
using CSBP.Forms.TB;
using CSBP.Forms.WP;
using CSBP.Services.Base;
using static CSBP.Services.Base.Functions;
using static CSBP.Services.Resources.Messages;

/// <summary>
/// List of start dialogs.
/// </summary>
public class StartDialog
{
  /// <summary>
  /// Initializes static members of the <see cref="StartDialog"/> class.
  /// </summary>
  static StartDialog()
  {
    Dialoge = new Dictionary<string, StartDialog>
    {
      { nameof(AG100Clients).Left(5), new StartDialog { Type = typeof(AG100Clients), Title = AG100_title } },
      { nameof(AG200Users).Left(5), new StartDialog { Type = typeof(AG200Users), Title = AG200_title } },
      { nameof(AG400Backups).Left(5), new StartDialog { Type = typeof(AG400Backups), Title = AG400_title } },
      { nameof(AG500Ai).Left(5), new StartDialog { Type = typeof(AG500Ai), Title = AG500_title } },
      { nameof(AM500Options).Left(5), new StartDialog { Type = typeof(AM500Options), Title = AM500_title } },
      { nameof(AM510Dialogs).Left(5), new StartDialog { Type = typeof(AM510Dialogs), Title = AM510_title } },
      { nameof(FZ100Statistics).Left(5), new StartDialog { Type = typeof(FZ100Statistics), Title = FZ100_title } },
      { nameof(FZ200Bikes).Left(5), new StartDialog { Type = typeof(FZ200Bikes), Title = FZ200_title } },
      { nameof(FZ250Mileages).Left(5), new StartDialog { Type = typeof(FZ250Mileages), Title = FZ250_title } },
      { nameof(FZ300Authors).Left(5), new StartDialog { Type = typeof(FZ300Authors), Title = FZ300_title } },
      { nameof(FZ320Series).Left(5), new StartDialog { Type = typeof(FZ320Series), Title = FZ320_title } },
      { nameof(FZ340Books).Left(5), new StartDialog { Type = typeof(FZ340Books), Title = FZ340_title } },
      { nameof(FZ700Memos).Left(5), new StartDialog { Type = typeof(FZ700Memos), Title = FZ700_title } },
      { nameof(HH100Periods).Left(5), new StartDialog { Type = typeof(HH100Periods), Title = HH100_title } },
      { nameof(HH200Accounts).Left(5), new StartDialog { Type = typeof(HH200Accounts), Title = HH200_title } },
      { nameof(HH300Events).Left(5), new StartDialog { Type = typeof(HH300Events), Title = HH300_title } },
      { nameof(HH400Bookings).Left(5), new StartDialog { Type = typeof(HH400Bookings), Title = HH400_title } },
      { nameof(HH500Balance).Left(5) + ";" + Constants.KZBI_EROEFFNUNG, new StartDialog { Type = typeof(HH500Balance), Title = HH500_title_EB } },
      { nameof(HH500Balance).Left(5) + ";" + Constants.KZBI_GV, new StartDialog { Type = typeof(HH500Balance), Title = HH500_title_GV } },
      { nameof(HH500Balance).Left(5) + ";" + Constants.KZBI_SCHLUSS, new StartDialog { Type = typeof(HH500Balance), Title = HH500_title_SB } },
      { nameof(SB200Ancestors).Left(5), new StartDialog { Type = typeof(SB200Ancestors), Title = SB200_title } },
      { nameof(SB300Families).Left(5), new StartDialog { Type = typeof(SB300Families), Title = SB300_title } },
      { nameof(SB400Sources).Left(5), new StartDialog { Type = typeof(SB400Sources), Title = SB400_title } },
      { nameof(TB100Diary).Left(5), new StartDialog { Type = typeof(TB100Diary), Title = TB100_title } },
      //// { nameof(WP100Chart).Left(5), new StartDialog { Type = typeof(WP100Chart), Title = WP100_title } },
      { nameof(WP200Stocks).Left(5), new StartDialog { Type = typeof(WP200Stocks), Title = WP200_title } },
      { nameof(WP250Investments).Left(5), new StartDialog { Type = typeof(WP250Investments), Title = WP250_title } },
      { nameof(WP300Configurations).Left(5), new StartDialog { Type = typeof(WP300Configurations), Title = WP300_title } },
      { nameof(WP400Bookings).Left(5), new StartDialog { Type = typeof(WP400Bookings), Title = WP400_title } },
      { nameof(WP500Prices).Left(5), new StartDialog { Type = typeof(WP500Prices), Title = WP500_title } },
    };
  }

  /// <summary>Gets list of startable dialog in the notebook.</summary>
  public static Dictionary<string, StartDialog> Dialoge { get; private set; }

  /// <summary>Gets or sets the type.</summary>
  public Type Type { get; set; }

  /// <summary>Gets or sets the title.</summary>
  public string Title { get; set; }
}
