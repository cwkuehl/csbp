// <copyright file="DbAlter.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base
{
  using System.Collections.Generic;
  using System.Text;
  using CSBP.Apis.Enums;
  using CSBP.Base;

  public partial class DbAlter
  {
    /** String für Pause bei Jet-Engine. */
    public const string JET_PAUSE = "--pause";

    /** Datenbankart für Skripterzeugung. */
    private readonly DatabaseTypeEnum zieldb = DatabaseTypeEnum.SqLite;

    /** Liste für Spaltendefinition beim CREATE TABLE-Befehl. */
    private List<string> mcFeldC = null;
    /** Liste für Spaltenname. */
    private List<string> mcFeld = null;
    /** Liste für Spaltentyp. */
    private List<string> mcTyp = null;
    /** Liste für Merker, ob Spalte NULL sein darf. */
    private List<string> mcNull = null;
    /** Liste für Default-Wert der Spalte. */
    private List<string> mcDef = null;
    /** Liste für Merker, ob Spalte neu in der Tabelle ist. */
    private List<string> mcNeu = null;
    /** Alle benutzten Spaltentypen werden hier vorgehalten. */
    private Dictionary<string, string> mhtTyp = null;
    /** Benutzer- oder Schema-Name für das Anlegen von Tabellen. */
    private readonly string mstrConBenutzer = "";

    /** Besondere Spaltennamen werden am Anfang maskiert. */
    private const string JET_SQL_ANFANG = "[";
    /** Besondere Spaltennamen werden am Ende maskiert. */
    private const string JET_SQL_ENDE = "]";
    /** Besondere Spaltennamen werden maskiert. */
    private const string MYSQL_SQL_RAHMEN = "`";
    /** Besondere Spaltennamen werden maskiert. */
    private const string HSQLDB_SQL_RAHMEN = "\"";

    /**
     * Standard-Konstruktor mit Initialisierung.
     */
    public DbAlter(DatabaseTypeEnum dt)
    {
      this.zieldb = dt;
    }

    /**
     * Interne Initialisierung für CreateTab-Funktionen.
     */
    public void createTab0()
    {
      mcFeldC = init(mcFeldC);
    }

    /**
     * Definition einer Spalte für CREATE TABLE-Befehl.
     * @param strFeld Spaltenname.
     * @param strTyp Spaltentyp wird für aktuelle Datenbank übersetzt.
     * @param bNull True, wenn Spalte NULL sein darf.
     */
    public void createTab1(string strFeld, string strTyp, bool bNull)
    {
      string strNull = getNull(bNull);
      mcFeldC.Add(Functions.Append(Functions.Append(getColumn(zieldb, strFeld), " ", dbTyp(strTyp)), " ", strNull));
    }

    /**
     * Erzeugt eine Tabelle in der Datenbank mittels CREATE TABLE-Befehl mit Primärschlüssel.
     * @param tab Tabellenname.
     * @param index String mit Komma-getrennten Spaltenname des Primärindex.
     * @return True, wenn alles OK.
     */
    public bool createTab2(List<string> mout, string tab, string index)
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
      execute(mout, dropTab(zieldb, tab));
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
          execute(mout, sql);
          sql = string.Format("ALTER TABLE {0}{1} ADD (CONSTRAINT XPK{0} PRIMARY KEY ({2})){3}", tab, Constants.CRLF, index, ende);
        }
      }
      else if (zieldb == DatabaseTypeEnum.HsqlDb)
      {
        sql = string.Format("CREATE CACHED TABLE {0} ({1}{2}{1}){3}", tab, Constants.CRLF, str1, ende);
        if (!string.IsNullOrEmpty(index))
        {
          execute(mout, sql);
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
          execute(mout, sql);
          sql = string.Format("CREATE UNIQUE INDEX XPK{0} ON {1}{0} ({2}){3}", tab, praefix, index, ende);
        }
      }
      execute(mout, sql);
      if (replId)
      {
        // Index über Replikation_UID erstellen
        var strReplIndex = "Replikation_UID";
        if (mandant)
          strReplIndex += ", Mandant_Nr";
        sql = string.Format("CREATE INDEX XRK{0} ON {1}{0} ({2}){3}", tab, praefix, strReplIndex, ende);
        execute(mout, sql);
      }
      var bTabCreate2 = true;
      return bTabCreate2;
    }

    /**
     * Erzeugt einen weiteren Index zu einer Tabelle.
     * @param tab Tabellenname.
     * @param index Name des neuen Index.
     * @param spalten String mit Komma-getrennten Spaltennamen des Index.
     * @return True, wenn alles OK.
     */
    public bool createTab3(List<string> mout, string tab, string index, bool unique, string spalten)
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
        execute(mout, sql);
      }
      var bTabCreate3 = true;
      return bTabCreate3;
    }

    /**
     * Interne Initialisierung für AddTab-Funktionen.
     */
    public void addTab0()
    {
      mcFeld = init(mcFeld);
      mcTyp = init(mcTyp);
      mcNull = init(mcNull);
      mcDef = init(mcDef);
      mcNeu = init(mcNeu);
    }

    /**
     * Spaltendefinition für bestehende Spalte hinzufügen.
     * @param feld Spaltenname.
     * @param typ Spaltentyp wird für aktuelle Datenbank übersetzt.
     * @param bNull True, wenn Spalte NULL sein darf.
     */
    public void addTab1(string feld, string typ, bool bNull)
    {
      var strNull = getNull(bNull);
      mcFeld.Add(getColumn(zieldb, feld));
      mcTyp.Add(typ);
      mcNull.Add(strNull);
      mcDef.Add("");
      mcNeu.Add("0");
    }

    /**
     * Spaltendefinition für neue Spalte hinzufügen.
     * @param feld Spaltenname.
     * @param typ Spaltentyp wird für aktuelle Datenbank übersetzt.
     * @param bNull True, wenn Spalte NULL sein darf.
     * @param def Default-Wert für Datenüberleitung.
     */
    public void addTab1a(string feld, string typ, bool bNull, string def)
    {
      string strNull = getNull(bNull);
      if (def == "CInt")
      {
        // alte Spalten mit Transformation
        string strTrans = holeSqlCInt(feld, zieldb);
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

    /**
     * Hinzufügen von Spalten in eine Tabelle, die schon in der Datenbank besteht. Dabei bleiben mit folgendem Vorgehen
     * alle Datensätze erhalten: Create TMP_Tab, Kopieren, Create Tab, Kopieren.
     * @param tab Tabellenname.
     * @param index String mit Komma-getrennten Spaltenname des Primärindex.
     * @param indexalt String mit Komma-getrennten Spaltenname des alten Primärindex.
     * @return True, wenn alles OK.
     */
    public bool addTab2(List<string> mout, string tab, string index, string indexalt)
    {
      string sql;
      var str2 = new StringBuilder(); // alte und neue Spalten
      var str3 = new StringBuilder(); // alte Spalten und neue Werte
      // List<string> cFeldTyp = null;
      // cFeldTyp = init(cFeldTyp);
      var praefix = Praefix(zieldb, mstrConBenutzer);
      // Temporäre Tabelle erzeugen
      createTab0();
      int i;
      for (i = 0; i < mcFeld.Count; i++)
      {
        createTab1(mcFeld[i], mcTyp[i], mcNull[i] != "NOT NULL");
      }
      createTab2(mout, tmp(tab), index);
      if (zieldb == DatabaseTypeEnum.Jet)
      {
        execute(mout, JET_PAUSE);
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
      { // bKopieren
        // Temporäre Tabelle aus Tabelle füllen
        sql = string.Format("INSERT INTO {0}{1} ({2}) SELECT {3} FROM {0}{4}{5}", praefix, tmp(tab),
          str2, str3, tab, zieldb == DatabaseTypeEnum.Jet ? " ORDER BY " + indexalt : "");
        execute(mout, sql);
      }
      // Tabelle neu erzeugen
      createTab0();
      for (i = 0; i < mcFeld.Count; i++)
      {
        createTab1(mcFeld[i], mcTyp[i], mcNull[i] != "NOT NULL");
      }
      createTab2(mout, tab, index);
      if (zieldb == DatabaseTypeEnum.Jet)
        execute(mout, JET_PAUSE);
      if (true)
      { // bKopieren
        // Temporäre Tabelle in Tabelle kopieren
        sql = string.Format("INSERT INTO {0}{1} ({2}) SELECT {3} FROM {0}{4}", praefix, tab, str2, str2, tmp(tab));
        execute(mout, sql);
      }
      // Temporäre Tabelle löschen
      execute(mout, dropTab(zieldb, tmp(tab)));
      var bTabAdd2 = true;
      return bTabAdd2;
    }

    /**
     * Neuanlegen oder Leeren eines Vectors.
     * @param columns Zu leerender Vector.
     * @return Evtl. neu angelegter Vector.
     */
    private static List<string> init(List<string> columns)
    {
      List<string> col = columns;
      if (col == null)
      {
        col = new List<string>();
      }
      col.Clear();
      return col;
    }

    /**
     * Liefert NULL-Definition für CREATE TABLE-Befehl.
     * @param bNull True, wenn Spalte NULL sein darf.
     * @return "" oder "NOT NULL".
     */
    private static string getNull(bool bNull)
    {
      return bNull ? "" : "NOT NULL";
    }

    /**
     * Übersetzung eines Spaltentyps vom SQL Server in andere Datenbanken.
     * @param typ Spaltentyp vom SQL Server.
     * @return Spaltentyp in aktueller Ziel-Datenbank.
     */
    private string dbTyp(string typ)
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

    /**
     * Liefert Tabellenname für temporäre Tabelle.
     * @param tab Tabellenname.
     * @return Tabellenname für temporäre Tabelle.
     */
    private static string tmp(string tab)
    {
      return "TMP_" + tab;
    }

    /**
     * Ausführung eines SQL-Befehls oder Schreiben des Befehls in ein Skript.
     * @param sql SQL-Befehl.
     */
    private void execute(List<string> mout, string sql)
    {
      if (zieldb == DatabaseTypeEnum.SqLite)
        sql = sql.ToUpper();
      mout.Add(sql + Constants.CRLF);
    }

    /**
     * Liefert DROP TABLE-Befehl für eine Tabelle.
     * @param dt Datenbank-Art.
     * @param tab Tabellenname.
     * @return DROP TABLE-Befehl für eine Tabelle.
     */
    public static string dropTab(DatabaseTypeEnum dt, string tab)
    {
      var sb = new StringBuilder("DROP TABLE ");
      if (dt == DatabaseTypeEnum.MySql || dt == DatabaseTypeEnum.SqLite)
        sb.Append("IF EXISTS ");
      sb.Append(tab);
      sb.Append(Ende(dt));
      var sql = sb.ToString();
      return sql;
    }

    public static string Ende(DatabaseTypeEnum dt)
    {
      var ende = "";
      if (dt == DatabaseTypeEnum.MySql || dt == DatabaseTypeEnum.HsqlDb || dt == DatabaseTypeEnum.SqLite)
        ende = ";";
      else if (dt == DatabaseTypeEnum.SqlServer)
        ende = Constants.CRLF + "GO";
      return ende;
    }

    public static string Praefix(DatabaseTypeEnum dt, string ben)
    {
      var praefix = dt == DatabaseTypeEnum.SqlServer ? $"{ben}." : "";
      return praefix;
    }

    /**
     * Liefert Spaltenname für SQL-Befehl. Besondere Spaltennamen werden bei Jet-Engine in eckige Klammern eingerahmt.
     * @param dt Datenbank-Art.
     * @param col Spaltenname.
     * @return Evtl. angepasster Spaltenname.
     */
    private static string getColumn(DatabaseTypeEnum dt, string col)
    {
      if (col == null)
        return null;
      string feld = col;
      if (string.Compare(feld, "Jet-Engine", true) == 0)
      {
        if (dt == DatabaseTypeEnum.MySql)
          feld = MYSQL_SQL_RAHMEN + feld + MYSQL_SQL_RAHMEN;
        else if (dt == DatabaseTypeEnum.HsqlDb)
          feld = HSQLDB_SQL_RAHMEN + feld + HSQLDB_SQL_RAHMEN;
        else
          feld = JET_SQL_ANFANG + feld + JET_SQL_ENDE;
      }
      return feld;
    }

    /**
     * Liefert Konvertierungsfunktion nach Datenbankart.
     * @param feld Zu konvertierender Wert.
     * @param dt Datenbankart.
     * @return Wert mit Konvertierungsfunktion als String.
     */
    private static string holeSqlCInt(string feld, DatabaseTypeEnum dt)
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
  }
}
