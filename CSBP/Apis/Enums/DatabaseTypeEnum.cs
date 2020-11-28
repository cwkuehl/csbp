// <copyright file="DatabaseTypeEnum.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Enums
{
  /// <summary>Arten von Datenbanken.</summary>
  public enum DatabaseTypeEnum
  {
    /// <summary>Keine Art.<summary>
    None = 0,
    /// <summary>Jet-Engine.<summary>
    Jet = 1,
    /// <summary>SQL Server ab 7.0.<summary>
    SqlServer = 2,
    /// <summary>MySql.<summary>
    MySql = 3,
    /// <summary>HSQLDB.<summary>
    HsqlDb = 4,
    /// <summary>SQLite.<summary>
    SqLite = 5,
  }
}
