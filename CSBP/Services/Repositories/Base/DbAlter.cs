// <copyright file="DbAlter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base;

using System.Collections.Generic;
using System.Text;
using CSBP.Apis.Enums;
using CSBP.Base;

/// <summary>
/// Changes the database structure.
/// </summary>
public partial class DbAlter
{
  /// <summary>Pause for Jet-Engine.</summary>
  private const string JetPause = "--pause";

  /// <summary>Beginning for special jet engine column names.</summary>
  private const string JetSqlAnfang = "[";

  /// <summary>End for special jet engine column names.</summary>
  private const string JetSqlEnde = "]";

  /** Mask character for Mysql. */
  private const string MysqlSqlRahmen = "`";

  /** Mask character for Hsqldb. */
  private const string HsqldbSqlRahmen = "\"";

  /// <summary>Database type for script.</summary>
  private readonly DatabaseTypeEnum zieldb = DatabaseTypeEnum.SqLite;

  /// <summary>User or schema name.</summary>
  private readonly string mstrConBenutzer = "";

  /// <summary>List of column definitions for CREATE TABLE.</summary>
  private List<string> mcFeldC = null;

  /// <summary>List of column names.</summary>
  private List<string> mcFeld = null;

  /// <summary>List of column types.</summary>
  private List<string> mcTyp = null;

  /// <summary>List of column nullability.</summary>
  private List<string> mcNull = null;

  /// <summary>List of column default values.</summary>
  private List<string> mcDef = null;

  /// <summary>List of new columns.</summary>
  private List<string> mcNeu = null;

  /// <summary>Dictionary of all column types.</summary>
  private Dictionary<string, string> mhtTyp = null;

  /// <summary>
  /// Initializes a new instance of the <see cref="DbAlter"/> class.
  /// </summary>
  /// <param name="dt">Affected database type.</param>
  public DbAlter(DatabaseTypeEnum dt)
  {
    zieldb = dt;
  }

  /// <summary>
  /// Initializes fields for CreateTab functions.
  /// </summary>
  public void CreateTab0()
  {
    mcFeldC = Init(mcFeldC);
  }

  /// <summary>
  /// Defines a column for CREATE TABLE.
  /// </summary>
  /// <param name="strFeld">Affected column name.</param>
  /// <param name="strTyp">Affected column type.</param>
  /// <param name="bNull">Are NULL values allowed or not.</param>
  public void CreateTab1(string strFeld, string strTyp, bool bNull)
  {
    string strNull = GetNull(bNull);
    mcFeldC.Add(Functions.Append(Functions.Append(GetColumn(zieldb, strFeld), " ", DbTyp(strTyp)), " ", strNull));
  }

  /// <summary>
  /// Adds CREATE TABLE command to string list.
  /// </summary>
  /// <param name="mout">Affected string list.</param>
  /// <param name="tab">Affected table name.</param>
  /// <param name="index">Comma separated list for primary key columns.</param>
  public void CreateTab2(List<string> mout, string tab, string index)
  {
    var str1 = new StringBuilder();
    var replId = false;
    var mandant = false;

    foreach (var strF in mcFeldC)
    {
      if (str1.Length > 0)
        str1.Append(',');
      str1.Append(strF);
      if (strF.StartsWith("Replikation_UID"))
        replId = true;
      if (strF.StartsWith("Mandant_Nr"))
        mandant = true;
    }
    Execute(mout, DropTab(zieldb, tab));
    var praefix = Praefix(zieldb, mstrConBenutzer);
    var ende = Ende(zieldb);
    string sql;
    if (zieldb == DatabaseTypeEnum.Jet || zieldb == DatabaseTypeEnum.SqlServer)
    {
      sql = string.Format("CREATE TABLE {0} ({1}{2}){3}", tab, str1,
        string.IsNullOrEmpty(index) ? "" : $", CONSTRAINT XPK{tab} PRIMARY KEY ({index})", ende);
    }
    else if (zieldb == DatabaseTypeEnum.MySql)
    {
      sql = string.Format("CREATE TABLE {0} ({1}{2}{1}) ENGINE=INNODB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci{3}",
        tab, Constants.CRLF, str1, ende);
      if (!string.IsNullOrEmpty(index))
      {
        Execute(mout, sql);
        sql = string.Format("ALTER TABLE {0}{1} ADD (CONSTRAINT XPK{0} PRIMARY KEY ({2})){3}", tab, Constants.CRLF, index, ende);
      }
    }
    else if (zieldb == DatabaseTypeEnum.HsqlDb)
    {
      sql = string.Format("CREATE CACHED TABLE {0} ({1}{2}{1}){3}", tab, Constants.CRLF, str1, ende);
      if (!string.IsNullOrEmpty(index))
      {
        Execute(mout, sql);
        sql = string.Format("ALTER TABLE {0}{1} ADD CONSTRAINT XPK{0} PRIMARY KEY ({2}){3}", tab, Constants.CRLF, index, ende);
      }
    }
    else if (zieldb == DatabaseTypeEnum.SqLite)
    {
      sql = string.Format("CREATE TABLE IF NOT EXISTS {0}({1}{2}){3}", tab, str1, string.IsNullOrEmpty(index) ? "" : $", PRIMARY KEY ({index})", ende);
    }
    else
    {
      sql = string.Format("CREATE TABLE {0} ({1}){2}", tab, str1, ende);
      if (!string.IsNullOrEmpty(index))
      {
        Execute(mout, sql);
        sql = string.Format("CREATE UNIQUE INDEX XPK{0} ON {1}{0} ({2}){3}", tab, praefix, index, ende);
      }
    }
    Execute(mout, sql);
    if (replId)
    {
      // Index über Replikation_UID erstellen
      var strReplIndex = "Replikation_UID";
      if (mandant)
        strReplIndex += ", Mandant_Nr";
      sql = string.Format("CREATE INDEX XRK{0} ON {1}{0} ({2}){3}", tab, praefix, strReplIndex, ende);
      Execute(mout, sql);
    }
  }

  /// <summary>
  /// Adds the command for another index to string list.
  /// </summary>
  /// <param name="mout">Affected string list.</param>
  /// <param name="tab">Affected table name.</param>
  /// <param name="index">Affected index name.</param>
  /// <param name="unique">Is index unique or not.</param>
  /// <param name="spalten">Comma separated column list of index.</param>
  public void CreateTab3(List<string> mout, string tab, string index, bool unique, string spalten)
  {
    string sql = null;
    var ende = Ende(zieldb);
    if (zieldb == DatabaseTypeEnum.Jet)
    {
      if (unique)
      {
        // Jet unterstützt nur Unique-Constraints
        sql = string.Format("ALTER TABLE {0} ADD CONSTRAINT {1}{2} ({3}){4}", tab, index, " UNIQUE", spalten, ende);
      }
    }
    else if (zieldb == DatabaseTypeEnum.SqlServer)
    {
      // strSql = string.Format(
      // "CREATE TABLE {0} ({1}{2}){3}",
      // strTabelle,
      // str1,
      // Global.iif(string.IsNullOrEmpty(strIndex), "", ", CONSTRAINT XPK"
      // + strTabelle + " PRIMARY KEY (" + strIndex
      // + ")"), strEnde);
    }
    else if (zieldb == DatabaseTypeEnum.MySql)
    {
      sql = string.Format("ALTER TABLE {0}{1} ADD {2} {3}({4}){5}", tab, Constants.CRLF, unique ? "UNIQUE" : "INDEX", index, spalten, ende);

      // ALTER TABLE `DB`.`VM_Buchung_2` ADD INDEX `Schluessel` (
      // `Mandant_Nr` , `Schluessel` )
      // ALTER TABLE `DB`.`VM_Buchung_2` ADD UNIQUE `Schluessel` (
      // `Mandant_Nr` , `Schluessel` )
    }
    else if (zieldb == DatabaseTypeEnum.HsqlDb)
    {
      if (unique)
        sql = string.Format("ALTER TABLE {0}{1} ADD CONSTRAINT {2} UNIQUE ({3}){4}", tab, Constants.CRLF, index, spalten, ende);
    }
    else
    {
      // strSql = string.Format(
      // "CREATE UNIQUE INDEX XPK{0} ON {1}{0} ({2}){3}",
      // strTabelle, strPraefix, strIndex, strEnde);
    }
    if (!string.IsNullOrEmpty(sql))
    {
      Execute(mout, sql);
    }
  }

  /// <summary>
  /// Initializes all lists for AddTab functions.
  /// </summary>
  public void AddTab0()
  {
    mcFeld = Init(mcFeld);
    mcTyp = Init(mcTyp);
    mcNull = Init(mcNull);
    mcDef = Init(mcDef);
    mcNeu = Init(mcNeu);
  }

  /// <summary>
  /// Adds column definition for existing column.
  /// </summary>
  /// <param name="feld">Affected column name.</param>
  /// <param name="typ">Affected column type.</param>
  /// <param name="bNull">Are NULL values allowed or not.</param>
  public void AddTab1(string feld, string typ, bool bNull)
  {
    var strNull = GetNull(bNull);
    mcFeld.Add(GetColumn(zieldb, feld));
    mcTyp.Add(typ);
    mcNull.Add(strNull);
    mcDef.Add("");
    mcNeu.Add("0");
  }

  /// <summary>
  /// Adds definition for new column.
  /// </summary>
  /// <param name="feld">Affected column name.</param>
  /// <param name="typ">Affected column type.</param>
  /// <param name="bNull">Are NULL values allowed or not.</param>
  /// <param name="def">Affected default value for data transfer.</param>
  public void AddTab1a(string feld, string typ, bool bNull, string def)
  {
    string strNull = GetNull(bNull);
    if (def == "CInt")
    {
      // alte Spalten mit Transformation
      string strTrans = GetSqlCInt(feld, zieldb);
      mcFeld.Add(feld);
      mcTyp.Add(typ);
      mcNull.Add(strNull);
      mcDef.Add(strTrans);
      mcNeu.Add("2");
    }
    else
    {
      mcFeld.Add(feld);
      mcTyp.Add(typ);
      mcNull.Add(strNull);
      mcDef.Add(def);
      mcNeu.Add("1");
    }
  }

  /// <summary>
  /// Adds sql commands for adding columns to a table. All data is preserved: Create TMP_Tab, copy, Create Tab, copy.
  /// </summary>
  /// <param name="mout">Affected string list.</param>
  /// <param name="tab">Affected table name.</param>
  /// <param name="index">New comma separated column list.</param>
  /// <param name="indexalt">Old comma separated column list.</param>
  public void AddTab2(List<string> mout, string tab, string index, string indexalt)
  {
    string sql;
    var str2 = new StringBuilder(); // alte und neue Spalten
    var str3 = new StringBuilder(); // alte Spalten und neue Werte
    var praefix = Praefix(zieldb, mstrConBenutzer);

    // Temporäre Tabelle erzeugen
    CreateTab0();
    int i;
    for (i = 0; i < mcFeld.Count; i++)
    {
      CreateTab1(mcFeld[i], mcTyp[i], mcNull[i] != "NOT NULL");
    }
    CreateTab2(mout, Tmp(tab), index);
    if (zieldb == DatabaseTypeEnum.Jet)
    {
      Execute(mout, JetPause);
    }
    for (i = 0; i < mcFeld.Count; i++)
    {
      if (Functions.ToInt64(mcNeu[i]) == 0)
      {
        if (str2.Length > 0)
          str2.Append(',');
        str2.Append(mcFeld[i]);
        if (str3.Length > 0)
          str3.Append(',');
        str3.Append(mcFeld[i]);
      }
      else if (Functions.ToInt64(mcNeu[i]) == 1)
      {
        if (str2.Length > 0)
          str2.Append(',');
        str2.Append(mcFeld[i]);
        if (str3.Length > 0)
          str3.Append(',');
        str3.Append(mcDef[i]);
      }
      else
      {
        if (str2.Length > 0)
          str2.Append(',');
        str2.Append(mcFeld[i]);
        if (str3.Length > 0)
          str3.Append(',');
        str3.Append(mcDef[i]);
      }
    }
    if (true)
    {
      // bKopieren
      // Temporäre Tabelle aus Tabelle füllen
      sql = string.Format("INSERT INTO {0}{1} ({2}) SELECT {3} FROM {0}{4}{5}", praefix, Tmp(tab),
        str2, str3, tab, zieldb == DatabaseTypeEnum.Jet ? " ORDER BY " + indexalt : "");
      Execute(mout, sql);
    }

    // Tabelle neu erzeugen
    CreateTab0();
    for (i = 0; i < mcFeld.Count; i++)
    {
      CreateTab1(mcFeld[i], mcTyp[i], mcNull[i] != "NOT NULL");
    }
    CreateTab2(mout, tab, index);
    if (zieldb == DatabaseTypeEnum.Jet)
      Execute(mout, JetPause);
    if (true)
    {
      // bKopieren
      // Temporäre Tabelle in Tabelle kopieren
      sql = string.Format("INSERT INTO {0}{1} ({2}) SELECT {3} FROM {0}{4}", praefix, tab, str2, str2, Tmp(tab));
      Execute(mout, sql);
    }

    // Temporäre Tabelle löschen
    Execute(mout, DropTab(zieldb, Tmp(tab)));
  }

  /// <summary>
  /// Initializes or clears a list of string.
  /// </summary>
  /// <param name="columns">Affected list can be null.</param>
  /// <returns>Empty list of string.</returns>
  private static List<string> Init(List<string> columns)
  {
    List<string> col = columns;
    if (col == null)
    {
      col = new List<string>();
    }
    col.Clear();
    return col;
  }

  /// <summary>
  /// Gets NULL definiation for CREATE TABLE command.
  /// </summary>
  /// <param name="bNull">Are NULL values allowed or not.</param>
  /// <returns>Empty string or NOT NULL.</returns>
  private static string GetNull(bool bNull)
  {
    return bNull ? "" : "NOT NULL";
  }

  /// <summary>
  /// Gets temporary table name.
  /// </summary>
  /// <param name="tab">Affected table name.</param>
  /// <returns>Temporary table name.</returns>
  private static string Tmp(string tab)
  {
    return "TMP_" + tab;
  }

  /// <summary>
  /// Gets sql command for DROP TABLE.
  /// </summary>
  /// <param name="dt">Affected database type.</param>
  /// <param name="tab">Affected table name.</param>
  /// <returns>Sql command for DROP TABLE.</returns>
  private static string DropTab(DatabaseTypeEnum dt, string tab)
  {
    var sb = new StringBuilder("DROP TABLE ");
    if (dt == DatabaseTypeEnum.MySql || dt == DatabaseTypeEnum.SqLite)
      sb.Append("IF EXISTS ");
    sb.Append(tab);
    sb.Append(Ende(dt));
    var sql = sb.ToString();
    return sql;
  }

  /// <summary>
  /// Gets command line ending for database type.
  /// </summary>
  /// <param name="dt">Affected database type.</param>
  /// <returns>Command line ending.</returns>
  private static string Ende(DatabaseTypeEnum dt)
  {
    var ende = "";
    if (dt == DatabaseTypeEnum.MySql || dt == DatabaseTypeEnum.HsqlDb || dt == DatabaseTypeEnum.SqLite)
      ende = ";";
    else if (dt == DatabaseTypeEnum.SqlServer)
      ende = Constants.CRLF + "GO";
    return ende;
  }

  /// <summary>
  /// Gets table prefix.
  /// </summary>
  /// <param name="dt">Affected database type.</param>
  /// <param name="ben">Affected database scheme.</param>
  /// <returns>Table prefix.</returns>
  private static string Praefix(DatabaseTypeEnum dt, string ben)
  {
    var praefix = dt == DatabaseTypeEnum.SqlServer ? $"{ben}." : "";
    return praefix;
  }

  /// <summary>
  /// Gets possibly masked column name.
  /// </summary>
  /// <param name="dt">Affected database type.</param>
  /// <param name="col">Affected column name.</param>
  /// <returns>Possibly masked column name.</returns>
  private static string GetColumn(DatabaseTypeEnum dt, string col)
  {
    if (col == null)
      return null;
    string feld = col;
    if (string.Compare(feld, "Jet-Engine", true) == 0)
    {
      if (dt == DatabaseTypeEnum.MySql)
        feld = MysqlSqlRahmen + feld + MysqlSqlRahmen;
      else if (dt == DatabaseTypeEnum.HsqlDb)
        feld = HsqldbSqlRahmen + feld + HsqldbSqlRahmen;
      else
        feld = JetSqlAnfang + feld + JetSqlEnde;
    }
    return feld;
  }

  /// <summary>
  /// Gets sql conversion function for column to integer.
  /// </summary>
  /// <param name="feld">Affected column name.</param>
  /// <param name="dt">Affected database type.</param>
  /// <returns>Sql conversion function for column to integer.</returns>
  private static string GetSqlCInt(string feld, DatabaseTypeEnum dt)
  {
    string trans;
    if (dt == DatabaseTypeEnum.Jet)
      trans = "CInt(" + feld + ")";
    else if (dt == DatabaseTypeEnum.SqlServer)
      trans = "CONVERT(INT," + feld + ")";
    else if (dt == DatabaseTypeEnum.MySql)
      trans = "CONVERT(" + feld + ",SIGNED)";
    else if (dt == DatabaseTypeEnum.HsqlDb)
      trans = "CAST(" + feld + " AS INTEGER)";
    else
      trans = "Fehler";
    return trans;
  }

  /// <summary>
  /// Converts database type from SQL Server to other database type.
  /// </summary>
  /// <param name="typ">Affected column type.</param>
  /// <returns>Converted column type.</returns>
  private string DbTyp(string typ)
  {
    string dbtyp;
    if (zieldb == DatabaseTypeEnum.SqlServer)
      dbtyp = typ;
    else
    {
      if (mhtTyp == null)
      {
        mhtTyp = new Dictionary<string, string>();
        string db = DatabaseTypeEnum.Jet + "#";
        mhtTyp.Add(db + "D_DATE", "date");
        mhtTyp.Add(db + "D_DATETIME", "datetime");
        mhtTyp.Add(db + "D_GELDBETRAG", "currency");
        mhtTyp.Add(db + "D_GELDBETRAG2", "double");
        mhtTyp.Add(db + "D_INTEGER", "int");
        mhtTyp.Add(db + "D_LONG", "long");
        mhtTyp.Add(db + "D_MEMO", "memo");
        mhtTyp.Add(db + "D_BLOB", "xxx");
        mhtTyp.Add(db + "D_REPL_ID", "varchar(35)");
        mhtTyp.Add(db + "D_STRING_01", "varchar(1)");
        mhtTyp.Add(db + "D_STRING_02", "varchar(2)");
        mhtTyp.Add(db + "D_STRING_03", "varchar(3)");
        mhtTyp.Add(db + "D_STRING_04", "varchar(4)");
        mhtTyp.Add(db + "D_STRING_05", "varchar(5)");
        mhtTyp.Add(db + "D_STRING_10", "varchar(10)");
        mhtTyp.Add(db + "D_STRING_20", "varchar(20)");
        mhtTyp.Add(db + "D_STRING_35", "varchar(35)");
        mhtTyp.Add(db + "D_STRING_40", "varchar(40)");
        mhtTyp.Add(db + "D_STRING_50", "varchar(50)");
        mhtTyp.Add(db + "D_STRING_100", "varchar(100)");
        mhtTyp.Add(db + "D_STRING_120", "varchar(120)");
        mhtTyp.Add(db + "D_STRING_255", "varchar(255)");
        mhtTyp.Add(db + "D_SWITCH", "bit");
        db = DatabaseTypeEnum.MySql + "#";
        mhtTyp.Add(db + "D_DATE", "date");
        mhtTyp.Add(db + "D_DATETIME", "datetime");
        mhtTyp.Add(db + "D_GELDBETRAG", "double(21,4)");
        mhtTyp.Add(db + "D_GELDBETRAG2", "double(21,6)");
        mhtTyp.Add(db + "D_INTEGER", "int");
        mhtTyp.Add(db + "D_LONG", "bigint");
        mhtTyp.Add(db + "D_MEMO", "longtext");
        mhtTyp.Add(db + "D_BLOB", "longblob");
        mhtTyp.Add(db + "D_REPL_ID", "varchar(35)");
        mhtTyp.Add(db + "D_STRING_01", "varchar(1)");
        mhtTyp.Add(db + "D_STRING_02", "varchar(2)");
        mhtTyp.Add(db + "D_STRING_03", "varchar(3)");
        mhtTyp.Add(db + "D_STRING_04", "varchar(4)");
        mhtTyp.Add(db + "D_STRING_05", "varchar(5)");
        mhtTyp.Add(db + "D_STRING_10", "varchar(10)");
        mhtTyp.Add(db + "D_STRING_20", "varchar(20)");
        mhtTyp.Add(db + "D_STRING_35", "varchar(35)");
        mhtTyp.Add(db + "D_STRING_40", "varchar(40)");
        mhtTyp.Add(db + "D_STRING_50", "varchar(50)");
        mhtTyp.Add(db + "D_STRING_100", "varchar(100)");
        mhtTyp.Add(db + "D_STRING_120", "varchar(120)");
        mhtTyp.Add(db + "D_STRING_255", "varchar(255)");
        mhtTyp.Add(db + "D_SWITCH", "bit");
        db = DatabaseTypeEnum.HsqlDb + "#";
        mhtTyp.Add(db + "D_DATE", "date");
        mhtTyp.Add(db + "D_DATETIME", "datetime");
        mhtTyp.Add(db + "D_GELDBETRAG", "decimal(21,4)");
        mhtTyp.Add(db + "D_GELDBETRAG2", "decimal(21,6)");
        mhtTyp.Add(db + "D_INTEGER", "int");
        mhtTyp.Add(db + "D_LONG", "bigint");
        mhtTyp.Add(db + "D_MEMO", "varchar(1M)"); // clob // COLLATE SQL_TEXT_UCC
        mhtTyp.Add(db + "D_BLOB", "blob");
        mhtTyp.Add(db + "D_REPL_ID", "varchar(35)");
        mhtTyp.Add(db + "D_STRING_01", "varchar(1) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_02", "varchar(2) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_03", "varchar(3) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_04", "varchar(4) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_05", "varchar(5) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_10", "varchar(10) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_20", "varchar(20) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_35", "varchar(35) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_40", "varchar(40) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_50", "varchar(50) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_100", "varchar(100) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_120", "varchar(120) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_STRING_255", "varchar(255) COLLATE SQL_TEXT_UCC");
        mhtTyp.Add(db + "D_SWITCH", "boolean");
        db = DatabaseTypeEnum.SqLite + "#";
        mhtTyp.Add(db + "D_DATE", "DATE");
        mhtTyp.Add(db + "D_DATETIME", "TIMESTAMP DEFAULT NULL");
        mhtTyp.Add(db + "D_GELDBETRAG", "DECIMAL(21,4) ");
        mhtTyp.Add(db + "D_GELDBETRAG2", "DECIMAL(21,6)");
        mhtTyp.Add(db + "D_INTEGER", "INTEGER");
        mhtTyp.Add(db + "D_LONG", "BIGINT");
        mhtTyp.Add(db + "D_MEMO", "VARCHAR(1048576)");
        mhtTyp.Add(db + "D_BLOB", "BLOB");
        mhtTyp.Add(db + "D_REPL_ID", "VARCHAR(36) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_01", "VARCHAR(1) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_02", "VARCHAR(2) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_03", "VARCHAR(3) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_04", "VARCHAR(4) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_05", "VARCHAR(5) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_10", "VARCHAR(10) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_20", "VARCHAR(20) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_35", "VARCHAR(35) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_40", "VARCHAR(40) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_50", "VARCHAR(50) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_100", "VARCHAR(100) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_120", "VARCHAR(120) COLLATE NOCASE");
        mhtTyp.Add(db + "D_STRING_255", "VARCHAR(255) COLLATE NOCASE");
        mhtTyp.Add(db + "D_SWITCH", "BOOLEAN");
      }
      dbtyp = mhtTyp[zieldb + "#" + typ];
    }
    if (string.IsNullOrEmpty(dbtyp))
      dbtyp = @"Data type {strTyp} is missing.";
    return dbtyp;
  }

  /// <summary>
  /// Adds sql command to string list.
  /// </summary>
  /// <param name="mout">Affected string list.</param>
  /// <param name="sql">Affected sql command.</param>
  private void Execute(List<string> mout, string sql)
  {
    if (zieldb == DatabaseTypeEnum.SqLite)
      sql = sql.ToUpper();
    mout.Add(sql + Constants.CRLF);
  }
}
