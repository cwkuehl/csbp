// <copyright file="CsbpContext.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base
{
  using CSBP.Base;
  using CSBP.Services.Undo;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Debug;

  public partial class CsbpContext : DbContext
  {
    /// <summary>Vorläufige Undo-Liste für vorzeitige SaveChanges.</summary>
    public UndoList PreUndoList { get; } = new UndoList();
    public static string DbDriverConnect { get => dbDriverConnect; set => dbDriverConnect = value; }

    /// <summary>Logger-Instanz von NLog.</summary>
    static readonly NLog.ILogger Log = NLog.LogManager.GetCurrentClassLogger();

    //static LoggerFactory object
    public static readonly ILoggerFactory loggerFactory = new LoggerFactory(new[] { new DebugLoggerProvider() });

    /// <summary>Einstellung für DB_DRIVER_CONNECT aus der Commando-Zeile.</summary>
    private static string dbDriverConnect;

    /// <summary>Verbindungszeichenfolge zur Datenbank.</summary>
    static string ConString;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (string.IsNullOrWhiteSpace(ConString))
      {
        var con = DbDriverConnect ?? Parameter.Connect ?? "Data Source=../../../Data/csbp.db";
        Parameter.Connect = con;
        Log.Debug($"Connection: {con}");
        // if (con.Contains(";"))
        // {
        //   var arr = con.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        //   con = arr[0];
        // }
        ConString = con;
      }
      //optionsBuilder.UseSqlite("Data Source=/daten/wolfgang/Entwicklung/cs/csbp/CSBP/Data/csbp.db;");
      if (Functions.MachNichts() != 0)
      {
        optionsBuilder
          .UseLoggerFactory(loggerFactory)
          .EnableSensitiveDataLogging(); // SQL-Parameter-Werte anzeigen
      }
      optionsBuilder.UseSqlite(ConString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      OnModelCreatingGenerated(modelBuilder);

      // var converter = new ValueConverter<DateTime, string>(
      //   v => v.ToString("yyyy-MM-dd HH:mm:ss.f"),
      //   v => Functions.ToDateTime(v) ?? new DateTime());
      // modelBuilder.Entity<FzFahrradstand>().Property(a => a.Datum).HasConversion(converter);
    }
  }
}