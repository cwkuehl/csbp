// <copyright file="CsbpContext.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base;

using CSBP.Services.Apis.Models;
using CSBP.Services.Base;
using CSBP.Services.Undo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

/// <summary>
/// Entity framework context.
/// </summary>
public partial class CsbpContext : DbContext
{
  /// <summary>Logger factory instance.</summary>
  private static readonly ILoggerFactory LoggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });

  /// <summary>Logger instance of NLog.</summary>
  private static readonly NLog.ILogger Log = NLog.LogManager.GetCurrentClassLogger();

  /// <summary>Database connection string.</summary>
  private static string conString;

  /// <summary>Gets or sets parameter for DB_DRIVER_CONNECT from command line.</summary>
  public static string DbDriverConnect { get; set; }

  /// <summary>Gets preliminary undo list for early SaveChanges.</summary>
  public UndoList PreUndoList { get; } = new UndoList();

  /// <summary>
  /// Configures the context with Sqlite database.
  /// </summary>
  /// <param name="optionsBuilder">Affected options builder.</param>
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (string.IsNullOrWhiteSpace(conString))
    {
      var con = DbDriverConnect ?? Parameter.Connect ?? "Data Source=../../../Data/csbp.db";
      Parameter.Connect = con;
      Log.Debug($"Connection: {con}");

      // if (con.Contains(";"))
      // {
      //   var arr = con.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
      //   con = arr[0];
      // }
      conString = con;
    }

    // optionsBuilder.UseSqlite("Data Source=/daten/wolfgang/Entwicklung/cs/csbp/CSBP/Data/csbp.db;");
    if (Functions.MachNichts() != 0)
    {
      optionsBuilder
        .UseLoggerFactory(LoggerFactory)
        .EnableSensitiveDataLogging(); // Shows SQL parameter values.
    }
    optionsBuilder.UseSqlite(conString);
  }

  /// <summary>
  /// Configures entities with keys.
  /// </summary>
  /// <param name="modelBuilder">Affected model builder.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    OnModelCreatingGenerated(modelBuilder);

    // var converter = new ValueConverter<DateTime, string>(
    //   v => v.ToString("yyyy-MM-dd HH:mm:ss.f"),
    //   v => Functions.ToDateTime(v) ?? new DateTime());
    // modelBuilder.Entity<FzFahrradstand>().Property(a => a.Datum).HasConversion(converter);

    // Error: SQLite does not support expressions of type 'decimal' in ORDER BY clauses.
    modelBuilder.Entity<FzFahrradstand>().Property(e => e.Zaehler_km).HasConversion<int>();
    modelBuilder.Entity<FzFahrradstand>().Property(e => e.Periode_km).HasConversion<int>();
    modelBuilder.Entity<FzFahrradstand>().Property(e => e.Periode_Schnitt).HasConversion<double>();
  }
}
