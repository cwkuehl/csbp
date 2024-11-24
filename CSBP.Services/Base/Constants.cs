// <copyright file="Constants.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

/// <summary>
/// All public constants.
/// </summary>
public static class Constants
{
#pragma warning disable SA1310

  /// <summary>Benutzer-ID für Initialisierung.</summary>
  public const string USER_ID = "Benutzer-ID";

  /// <summary>Zeit in Millisekunden für Änderungsintervall.</summary>
  public const int AEND_ZEIT = 60000;

  /// <summary>Faktor für EURO-DM-Umrechnung.</summary>
  public const decimal EUROFAKTOR = 1.95583m;

  /// <summary>Einstellung: DB_INIT.</summary>
  public const string EINST_DB_INIT = "DB_INIT";

  /// <summary>Einstellung: DB_VERSION.</summary>
  public const string EINST_DB_VERSION = "DB_VERSION";

  /// <summary>Einstellung: DATENBANK.</summary>
  public const string EINST_DATENBANK = "DATENBANK";

  /// <summary>Mandant-Einstellung: REPLIKATION_UID.</summary>
  public const string EINST_MA_REPLIKATION_UID = "REPLIKATION_UID";

  /// <summary>Mandant-Einstellung: REPLIKATION_BEGINN.</summary>
  public const string EINST_MA_REPLIKATION_BEGINN = "REPLIKATION_BEGINN";

  /// <summary>Mandant-Einstellung: OHNE_ANMELDUNG.</summary>
  public const string EINST_MA_OHNE_ANMELDUNG = "OHNE_ANMELDUNG";

  /// <summary>Mandant-Einstellung: EXAMPLES.</summary>
  public const string EINST_MA_EXAMPLES = "EXAMPLES";

  /// <summary>Zeilenumbruch bei Windows.</summary>
  public const string CRLF = "\r\n";

  /// <summary>Halbes Jahr in Tagen.</summary>
  public const int STOCK_DAYS = 183;

  /// <summary>Kleinste Perioden-Nummer.</summary>
  public const int MIN_PERIODE = 0;

  /// <summary>Erste zu vergebende Perioden-Nummer.</summary>
  public const int START_PERIODE = 10000;

  /// <summary>Größte Perioden-Nummer.</summary>
  public const int MAX_PERIODE = 99999;

  /// <summary>Periodennummer, unter der die berechnete Periode gespeichert wird.</summary>
  public const int PN_BERECHNET = -1;

  /// <summary>Kontoart: Aktivkonto.</summary>
  public const string ARTK_AKTIVKONTO = "AK";

  /// <summary>Kontoart: Passivkonto.</summary>
  public const string ARTK_PASSIVKONTO = "PK";

  /// <summary>Kontoart: Aufwandskonto.</summary>
  public const string ARTK_AUFWANDSKONTO = "AW";

  /// <summary>Kontoart: Ertragskonto.</summary>
  public const string ARTK_ERTRAGSKONTO = "ER";

  /// <summary>Kennzeichen Konto: Eigenkapital.</summary>
  public const string KZK_EK = "E";

  /// <summary>Kennzeichen Konto: Gewinn oder Verlust.</summary>
  public const string KZK_GV = "G";

  /// <summary>Kennzeichen Konto: Depot.</summary>
  public const string KZK_DEPOT = "D";

  /// <summary>Kennzeichen Buchung: Aktiv.</summary>
  public const string KZB_AKTIV = "A";

  /// <summary>Kennzeichen Buchung: Storniert.</summary>
  public const string KZB_STORNO = "S";

  /// <summary>Kennzeichen Bilanz: Eröffnungbilanz.</summary>
  public const string KZBI_EROEFFNUNG = "EB";

  /// <summary>Kennzeichen Bilanz: Schlussbilanz.</summary>
  public const string KZBI_SCHLUSS = "SB";

  /// <summary>Kennzeichen Bilanz: Gewinn- und Verlust-Rechnung.</summary>
  public const string KZBI_GV = "GV";

  /// <summary>Kennzeichen Bilanz: Einzelnes Konto.</summary>
  public const string KZBI_KONTO = "KO";

  /// <summary>Kennzeichen Bilanz: Plan für Folgejahr.</summary>
  public const string KZBI_PLAN = "PL";

  /// <summary>Soll-Haben-Kennzeichen in Bilanz: Aktiv, linke Seite.</summary>
  public const string KZSH_A = "A";

  /// <summary>Soll-Haben-Kennzeichen in Bilanz: Passiv, rechte Seite.</summary>
  public const string KZSH_P = "P";

#pragma warning disable SA1310
}
