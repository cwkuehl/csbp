// <copyright file="PedigreeService.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using CSBP.Apis.Models;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Base;
  using static CSBP.Resources.Messages;
  using static CSBP.Resources.M;
  using CSBP.Apis.Enums;
  using CSBP.Services.Pedigree;
  using System.Text;
  using CSBP.Services.Reports;
  using System.Text.RegularExpressions;

  public class PedigreeService : ServiceBase, IPedigreeService
  {
    /// <summary>
    /// Gets a list of ancestors.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="name">Affected last or birth name.</param>
    /// <param name="vorname">Affected first name.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <returns>List of ancestors.</returns>
    public ServiceErgebnis<List<SbPerson>> GetAncestorList(ServiceDaten daten, string name = null, string firstname = null, string uid = null)
    {
      var l = SbPersonRep.GetList(daten, name, firstname);
      return new ServiceErgebnis<List<SbPerson>>(l);
    }

    /// <summary>
    /// Gets an ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <returns>Ancestor or null.</returns>
    public ServiceErgebnis<SbPerson> GetAncestor(ServiceDaten daten, string uid)
    {
      var e = SbPersonRep.GetList(daten, uid: uid).FirstOrDefault();
      return new ServiceErgebnis<SbPerson>(e);
    }

    /// <summary>
    /// Gets a list of bytes.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <returns>list of bytes.</returns>
    public ServiceErgebnis<List<ByteDaten>> GetBytes(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<List<ByteDaten>>(ByteDatenRep.GetList(daten, null, uid));
      return r;
    }

    /// <summary>
    /// Saves an ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="vorname">Affected first names.</param>
    /// <param name="gebname">Affected birth name.</param>
    /// <returns>Created or changed ancestor.</returns>
    public ServiceErgebnis<SbPerson> SaveAncestor(ServiceDaten daten, string uid, string name, string vorname,
      string gebname, string geschlecht, string titel, string konfession, string bemerkung, string quid, int status1,
      int status2, int status3, string geburtsdatum, string geburtsort, string geburtsbem, string geburtsQuelle,
      string taufdatum, string taufort, string taufbem, String taufQuelle, string todesdatum, string todesort,
      string todesbem, string todesQuelle, string begraebnisdatum, string begraebnisort, string begraebnisbem,
      string begraebnisQuelle, string gatteNeu, string vaterUidNeu, string mutterUidNeu, List<ByteDaten> byteliste)
    {
      var r = new ServiceErgebnis<SbPerson>();
      if (string.IsNullOrEmpty(gebname))
        r.Errors.Add(Message.New(SB001));
      if (!string.IsNullOrEmpty(gatteNeu) && (!string.IsNullOrEmpty(vaterUidNeu) || !string.IsNullOrEmpty(mutterUidNeu)))
        r.Errors.Add(Message.New(SB002));
      // Typ prüfen
      var gesch = N(geschlecht);
      var g = (GenderEnum)gesch;
      if (g == GenderEnum.MANN)
        g = GenderEnum.MAENNLICH;
      else if (g == GenderEnum.FRAU)
        g = GenderEnum.WEIBLICH;
      gesch = g.ToString();
      var name2 = N(name);
      if (string.IsNullOrEmpty(name2))
        name2 = gebname;
      if (!r.Ok)
        return r;
      var e = SbPersonRep.Save(daten, daten.MandantNr, uid, N(name2), N(vorname), N(gebname), N(gesch), N(titel),
        N(konfession), N(bemerkung), N(quid), status1, status2, status3);
      SpeichereEreignis(daten, e.Uid, "", GedcomEventEnum.BIRTH.ToString(), geburtsdatum, geburtsort, geburtsbem, geburtsQuelle);
      SpeichereEreignis(daten, e.Uid, "", GedcomEventEnum.CHRIST.ToString(), taufdatum, taufort, taufbem, taufQuelle);
      SpeichereEreignis(daten, e.Uid, "", GedcomEventEnum.DEATH.ToString(), todesdatum, todesort, todesbem, todesQuelle);
      SpeichereEreignis(daten, e.Uid, "", GedcomEventEnum.BURIAL.ToString(), begraebnisdatum, begraebnisort, begraebnisbem, begraebnisQuelle);
      SaveChanges(daten);
      if (byteliste != null)
        ByteDatenRep.SaveList(daten, "SB_Person", e.Uid, byteliste);
      if (!string.IsNullOrEmpty(vaterUidNeu) || !string.IsNullOrEmpty(mutterUidNeu))
        NeueFamilie(daten, null, vaterUidNeu, mutterUidNeu, e.Uid, false);
      if (!string.IsNullOrEmpty(gatteNeu))
      {
        var fuid = SbFamilieRep.GetList(daten, null, null, null, gatteNeu).FirstOrDefault()?.Uid;
        if (gesch == GenderEnum.MAENNLICH.ToString())
          NeueFamilie(daten, fuid, e.Uid, gatteNeu, null, true);
        else
          NeueFamilie(daten, fuid, gatteNeu, e.Uid, null, true);
      }
      return r;
    }

    /// <summary>
    /// Deletes an ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeleteAncestor(ServiceDaten daten, SbPerson e)
    {
      DeletePersonIntern(daten, e);
      return new ServiceErgebnis();
    }

    /// <summary>
    /// Gets the next ancestor by name.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <param name="name">Affected name.</param>
    /// <param name="firstname">Affected first name.</param>
    /// <returns>Ancestor uid or null.</returns>
    public ServiceErgebnis<string> GetNextAncestorName(ServiceDaten daten, string uid, string name, string firstname)
    {
      var r = new ServiceErgebnis<string>();
      var p0 = string.IsNullOrEmpty(uid) ? null : SbPersonRep.Get(daten, daten.MandantNr, uid);
      var p = SbPersonRep.GetNext(daten, p0, name, firstname);
      if (p == null && p0 != null)
      {
        // 1. Treffer suchen.
        p = SbPersonRep.GetNext(daten, null, name, firstname);
      }
      r.Ergebnis = p?.Uid;
      return r;
    }

    /// <summary>
    /// Gets the first child of the ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <param name="fuid">Affected family uid.</param>
    /// <returns>Ancestor uid or null.</returns>
    public ServiceErgebnis<string> GetFirstChild(ServiceDaten daten, string uid, string fuid = null)
    {
      string kuid = null;
      if (string.IsNullOrEmpty(fuid))
      {
        var f = GetElternFamilieIntern(daten, uid);
        if (f != null)
          fuid = f.Uid;
      }
      if (!string.IsNullOrEmpty(fuid))
      {
        var kliste = SbKindRep.GetList(daten, fuid);
        kuid = kliste.FirstOrDefault()?.Kind_Uid;
      }
      var r = new ServiceErgebnis<string>(kuid);
      return r;
    }

    /// <summary>
    /// Gets the next spouse of the ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <returns>Ancestor uid or null.</returns>
    public ServiceErgebnis<SbPerson> GetNextSpouse(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<SbPerson>(null);
      if (uid == null)
        return r;
      var liste = GetAlleEhegatten(daten, uid).OrderBy(a => a).ToList();
      var i = liste.IndexOf(uid);
      if (i >= 0)
      {
        i = (i + 1) % liste.Count;
        r.Ergebnis = SbPersonRep.GetList(daten, uid: liste.ElementAt(i)).FirstOrDefault();
      }
      return r;
    }


    /// <summary>
    /// Gets all spouses of the ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <returns>List of Ancestors.</returns>
    public ServiceErgebnis<List<SbPerson>> GetSpouseList(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<List<SbPerson>>(new List<SbPerson>());
      var liste = GetAlleEhegatten(daten, uid).OrderBy(a => a).ToList();
      foreach (var a in liste)
      {
        if (a != uid)
        {
          var p = SbPersonRep.Get(daten, daten.MandantNr, a);
          if (p != null)
            r.Ergebnis.Add(p);
        }
      }
      return r;
    }

    /// <summary>
    /// Gets the next sibling of the ancestor.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of ancestor.</param>
    /// <returns>Ancestor uid or null.</returns>
    public ServiceErgebnis<SbPerson> GetNextSibling(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<SbPerson>();
      if (string.IsNullOrEmpty(uid))
        return r;
      var p = SbPersonRep.GetList(daten, uid: uid).FirstOrDefault();
      var euid = p == null ? null : p.Mother == null ? p.Father?.Uid : p.Mother?.Uid;
      if (string.IsNullOrEmpty(euid))
        return r;
      var liste = new List<string>();
      var eliste = GetAlleEhegatten(daten, euid).OrderBy(a => a).ToList();
      foreach (var e in eliste)
      {
        var kliste = SbKindRep.GetList(daten, personuid: e);
        foreach (var c in kliste)
        {
          if (!liste.Contains(c.Kind_Uid))
            liste.Add(c.Kind_Uid);
        }
      }
      var i = liste.IndexOf(uid);
      if (i >= 0)
      {
        i = (i + 1) % liste.Count;
        r.Ergebnis = SbPersonRep.GetList(daten, uid: liste.ElementAt(i)).FirstOrDefault();
      }
      return r;
    }

    /// <summary>
    /// Gets a list of families.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>List of families.</returns>
    public ServiceErgebnis<List<SbFamilie>> GetFamilyList(ServiceDaten daten)
    {
      var r = new ServiceErgebnis<List<SbFamilie>>(SbFamilieRep.GetList(daten));
      return r;
    }

    /// <summary>
    /// Gets a family.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of family.</param>
    /// <param name="puid">Affected father or mother uid.</param>
    /// <param name="cuid">Affected child uid.</param>
    /// <returns>Familiy or null.</returns>
    public ServiceErgebnis<SbFamilie> GetFamily(ServiceDaten daten, string uid, string puid = null, string cuid = null)
    {
      var r = new ServiceErgebnis<SbFamilie>();
      if (string.IsNullOrEmpty(cuid))
        r.Ergebnis = SbFamilieRep.GetList(daten, uid, personuid: puid).FirstOrDefault();
      else
      {
        var k = GetKindFamilieIntern(daten, cuid);
        if (k != null)
          r.Ergebnis = SbFamilieRep.GetList(daten, k.Familie_Uid).FirstOrDefault();
      }
      return r;
    }

    /// <summary>
    /// Gets a list of children.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of family.</param>
    /// <returns>list of children.</returns>
    public ServiceErgebnis<List<SbPerson>> GetChildList(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<List<SbPerson>>(SbPersonRep.GetList(daten, fuid: uid));
      return r;
    }

    /// <summary>
    /// Saves a family.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of family.</param>
    /// <param name="fuid">Affected father uid.</param>
    /// <param name="muid">Affected mother uid.</param>
    /// <param name="mdate">Affected marriage date.</param>
    /// <param name="mplace">Affected marriage place.</param>
    /// <param name="mmemo">Affected marriage memo.</param>
    /// <returns>Created or changed family.</returns>
    public ServiceErgebnis<SbFamilie> SaveFamily(ServiceDaten daten, string uid, string fuid, string muid,
      string mdate, string mplace, string mmemo, string suid, List<string> children)
    {
      var e = NeueFamilie(daten, uid, fuid, muid, null, true);
      SpeichereEreignis(daten, "", e.Uid, GedcomEventEnum.MARRIAGE.ToString(), mdate, mplace, mmemo, suid);
      // bestehende Kinder lesen
      var listeAlt = SbKindRep.GetList(daten, e.Uid);
      foreach (var i in children)
      {
        var vo = listeAlt.FirstOrDefault(a => a.Kind_Uid == i);
        if (vo == null)
          SbKindRep.Save(daten, daten.MandantNr, e.Uid, i);
        else
          listeAlt.Remove(vo);
      }
      // überflüssige Kinder löschen.
      foreach (var vo in listeAlt)
      {
        SbKindRep.Delete(daten, vo);
      }
      var r = new ServiceErgebnis<SbFamilie>(e);
      return r;
    }

    /// <summary>
    /// Deletes a family.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeleteFamily(ServiceDaten daten, SbFamilie e)
    {
      DeleteFamilieIntern(daten, e);
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Gets a list of sources.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <returns>List of sources.</returns>
    public ServiceErgebnis<List<SbQuelle>> GetSourceList(ServiceDaten daten)
    {
      var r = new ServiceErgebnis<List<SbQuelle>>();
      r.Ergebnis = SbQuelleRep.GetList(daten, daten.MandantNr);
      return r;
    }

    /// <summary>
    /// Gets a source.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of source.</param>
    /// <returns>Source or null.</returns>
    public ServiceErgebnis<SbQuelle> GetSource(ServiceDaten daten, string uid)
    {
      var r = new ServiceErgebnis<SbQuelle>();
      r.Ergebnis = SbQuelleRep.Get(daten, daten.MandantNr, uid);
      return r;
    }

    /// <summary>
    /// Saves a source.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">ID of source.</param>
    /// <param name="author">Affected author.</param>
    /// <param name="desc">Affected description.</param>
    /// <param name="quotation">Affected quotation.</param>
    /// <param name="memo">Affected memo.</param>
    /// <returns>Created or changed source.</returns>
    public ServiceErgebnis<SbQuelle> SaveSource(ServiceDaten daten, string uid, string author, string desc,
      string quotation, string memo)
    {
      var r = new ServiceErgebnis<SbQuelle>();
      author = Functions.Cut(N(author), 255);
      desc = Functions.Cut(N(desc), 255);
      if (string.IsNullOrEmpty(author))
        r.Errors.Add(Message.New(SB013));
      if (string.IsNullOrEmpty(desc))
        r.Errors.Add(Message.New(SB014));
      if (!r.Ok)
        return r;
      r.Ergebnis = SbQuelleRep.Save(daten, daten.MandantNr, uid, desc, N(quotation), N(memo), author, 0, 0, 0);
      return r;
    }

    /// <summary>
    /// Deletes a source.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="e">Affected entity.</param>
    /// <returns>Possibly errors.</returns>
    public ServiceErgebnis DeleteSource(ServiceDaten daten, SbQuelle e)
    {
      if (e != null)
        DeleteQuelleIntern(daten, e);
      var r = new ServiceErgebnis();
      return r;
    }

    /// <summary>
    /// Liefert den Ahnenbericht als PDF-Dokument in Bytes.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="uid">Affected Ahnancestor uid.</param>
    /// <param name="generations">Number of generations.</param>
    /// <param name="siblings">With siblings?</param>
    /// <param name="descendants">List of descendants?</param>
    /// <param name="forbears">List of forbears?</param>
    /// <returns>Ahnenbericht als PDF-Dokument in Bytes.</returns>
    public ServiceErgebnis<byte[]> GetAncestorReport(ServiceDaten daten, string uid, int generations, bool siblings, bool descendants, bool forbears)
    {
      // Nachfahren-Liste
      // 1 Vorname <b>Name</b> * tt.mm.jjjj Ort
      // + Vorname <b>Name</b> * tt.mm.jjjj Ort
      //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
      //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
      //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
      // Vorfahren-Liste
      // 1 Vorname <b>Name</b> * tt.mm.jjjj Ort
      // + Vorname <b>Name</b> * tt.mm.jjjj Ort
      // + Vorname <b>Name</b> * tt.mm.jjjj Ort
      //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
      //   2 Vorname <b>Name</b> * tt.mm.jjjj Ort
      var r = new ServiceErgebnis<byte[]>();
      var anzahl = Math.Max(1, generations);
      var p = SbPersonRep.Get(daten, daten.MandantNr, uid);
      if (p == null)
        throw new MessageException(SB017(uid));
      var dlist = new List<SbPerson>();
      if (descendants)
        GetNachfahrenRekursiv(daten, uid, 0, 1, anzahl, dlist);
      var flist = new List<SbPerson>();
      if (forbears)
        GetVorfahrenRekursiv(daten, uid, true, siblings, true, 1, anzahl, flist);
      var ueberschrift = SB018(daten.Jetzt);
      var untertitel = SB019(p.AncestorName, anzahl);
      var untertitel2 = SB021(p.AncestorName, anzahl, siblings ? M0(SB022) : "");
      var rp = new AncestorReport
      {
        Caption = ueberschrift,
        Undertitle = untertitel,
        UndertitleForbears = untertitel2,
        Descendants = dlist,
        Forbears = flist,
      };
      r.Ergebnis = rp.Generate();
      return r;
    }

    /// <summary>
    /// Erstellen einer GEDCOM-Datei der Version 4.0 oder 5.5.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="skript">Name der Datei mit Pfad.</param>
    /// <param name="name">Name des Stammbaums in der Datei.</param>
    /// <param name="filter">Filter-Kriterium für Ahnen.</param>
    /// <param name="version">Version kann null sein, dann gilt Version "5.5".</param>
    /// <returns>Datei als String-Array.</returns>
    public ServiceErgebnis<List<string>> ExportAncestorList(ServiceDaten daten, string skript, string name, string filter, string version = null)
    {
      // Status2=1 heißt Export in Datei, sonst nicht.
      if (string.IsNullOrEmpty(version))
        version = "5.5";
      if (string.IsNullOrEmpty(name))
        throw new MessageException(SB023);
      if (string.IsNullOrEmpty(skript))
        throw new MessageException(M1012);
      if (!(version == "4.0" || version == "5.5"))
        throw new MessageException(SB024);
      var status2 = 1;
      var op = ">=";
      var tot = 0;
      if (string.IsNullOrEmpty(filter))
      {
        SbPersonRep.UpdateStatus2(daten, null, 0, status2);
        SbFamilieRep.UpdateStatus2(daten, status2);
        SbQuelleRep.UpdateStatus2(daten, status2);
      }
      else
      {
        var re = new Regex("^status1\\s*(<|<=|=|>=|>)\\s*(\\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var m = re.Match(filter);
        if (m.Success)
        {
          op = m.Groups[1].Value;
          tot = Functions.ToInt32(m.Groups[2].Value);
        }
        else
          throw new MessageException(SB025);
        status2 = 0;
        SbPersonRep.UpdateStatus2(daten, null, 0, status2);
        SbFamilieRep.UpdateStatus2(daten, status2);
        SbQuelleRep.UpdateStatus2(daten, status2);
        status2 = 1;
        SbPersonRep.UpdateStatus2(daten, op, tot, status2);
        SbFamilieRep.UpdateParentStatus2(daten, status2);
        SbQuelleRep.UpdatePersonStatus2(daten, status2);
      }
      var list = new List<String>();
      var map = new Dictionary<string, int>();
      SchreibeKopf(daten, list, version, skript);
      SchreibeIndividual(daten, list, map, version, status2);
      SchreibeFamily(daten, list, map, status2, op, tot);
      SchreibeSource(daten, list, map, status2);
      SchreibeFuss(daten, list, version, name);
      // var l = map.Where(a => a.Value < 0).Select(a => a.Key).ToList();
      // foreach (var i in l)
      //   Debug.WriteLine(i);
      var r = new ServiceErgebnis<List<string>>(list);
      return r;
    }

    /// <summary>
    /// Imports list of ancestors from GEDCOM file.
    /// </summary>
    /// <returns>Message of import.</returns>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="lines">GEDCOM file as list of lines.</param>
    public ServiceErgebnis<string> ImportAncestorList(ServiceDaten daten, List<string> lines)
    {
      var pliste = SbPersonRep.GetList(daten, daten.MandantNr);
      foreach (var p in pliste)
      {
        DeletePersonIntern(daten, p);
      }
      SaveChanges(daten);
      var fliste = SbFamilieRep.GetList(daten, daten.MandantNr);
      foreach (var f in fliste)
      {
        DeleteFamilieIntern(daten, f);
      }
      SaveChanges(daten);
      var qliste = SbQuelleRep.GetList(daten, daten.MandantNr);
      foreach (var q in qliste)
      {
        DeleteQuelleIntern(daten, q);
      }
      SaveChanges(daten);
      var map = new Dictionary<string, string>();
      var anzahl = ImportiereAhnen(daten, lines, map);
      var r = new ServiceErgebnis<string>(SB026(anzahl));
      return r;
    }

    /// <summary>
    /// Calculate the death year of all ancestors (Status1).
    /// </summary>
    /// <returns>Message of import.</returns>
    /// <param name="daten">Service data for database access.</param>
    public ServiceErgebnis CalculateDeathYear(ServiceDaten daten)
    {
      // int anzahl1 = 0;
      // int anzahl2 = 0;
      // int anzahl3 = 0;
      // const int alter = 80;
      // const int generation = 30;
      var r = new ServiceErgebnis();
      return r;
    }

    private static Regex level = new Regex("^([\\d]+) (.*)$", RegexOptions.Compiled);
    private static Regex indi0 = new Regex("^0 @I([\\da-fA-F;\\-]+)@ INDI$", RegexOptions.Compiled); // 297f9764:141887cfc37:-8000-
    private static Regex name1 = new Regex("^1 NAME [^/]*/([^/]+)/$", RegexOptions.Compiled);
    private static Regex givn2 = new Regex("^2 GIVN (.*)$", RegexOptions.Compiled);
    private static Regex surn2 = new Regex("^2 SURN (.*)$", RegexOptions.Compiled);
    private static Regex sex1 = new Regex("^1 SEX ([M|F])$", RegexOptions.Compiled);
    private static Regex birt1 = new Regex("^1 BIRT$", RegexOptions.Compiled);
    private static Regex buri1 = new Regex("^1 BURI$", RegexOptions.Compiled);
    private static Regex chr1 = new Regex("^1 CHR$", RegexOptions.Compiled);
    private static Regex deat1 = new Regex("^1 DEAT$", RegexOptions.Compiled);
    private static Regex date2 = new Regex("^2 DATE (.*)$", RegexOptions.Compiled);
    private static Regex plac2 = new Regex("^2 PLAC (.*)$", RegexOptions.Compiled);
    private static Regex note2 = new Regex("^2 NOTE (.*)$", RegexOptions.Compiled);
    private static Regex cont3 = new Regex("^3 CONT (.*)$", RegexOptions.Compiled);
    private static Regex reli1 = new Regex("^1 RELI (.*)$", RegexOptions.Compiled);
    private static Regex note1 = new Regex("^1 NOTE (.*)$", RegexOptions.Compiled);
    private static Regex cont2 = new Regex("^2 CONT (.*)$", RegexOptions.Compiled);
    private static Regex sour1 = new Regex("^1 SOUR @Q([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);
    private static Regex fam0 = new Regex("^0 @F([\\da-fA-F;\\-]+)@ FAM$", RegexOptions.Compiled);
    private static Regex husb1 = new Regex("^1 HUSB @I([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);
    private static Regex wife1 = new Regex("^1 WIFE @I([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);
    private static Regex marr1 = new Regex("^1 MARR$", RegexOptions.Compiled);
    private static Regex chil1 = new Regex("^1 CHIL @I([\\da-fA-F;\\-]+)@$", RegexOptions.Compiled);
    private static Regex sour0 = new Regex("^0 @Q([\\da-fA-F;\\-]+)@ SOUR$", RegexOptions.Compiled);
    private static Regex auth1 = new Regex("^1 AUTH (.*)$", RegexOptions.Compiled);
    private static Regex titl1 = new Regex("^1 TITL (.*)$", RegexOptions.Compiled);
    private static Regex text1 = new Regex("^1 TEXT (.*)$", RegexOptions.Compiled);

    private int ImportiereAhnen(ServiceDaten daten, List<string> datei, Dictionary<string, string> map)
    {
      Match m;
      string uid = null;
      var v = new List<string>();
      var anzahl = 0;

      foreach (var s in datei)
      {
        var str = s ?? "";
        if (str.StartsWith("0"))
        {
          try
          {
            if (v.Any())
            {
              var str0 = v[0];
              var g = false;
              v.RemoveAt(0);
              m = indi0.Match(str0);
              if (m.Success)
              {
                uid = GetUid(map, m.Groups[1].Value);
                ImportIndividual(daten, v, uid, map);
                anzahl++;
                g = true;
              }
              if (!g)
              {
                m = fam0.Match(str0);
                if (m.Success)
                {
                  uid = GetUid(map, m.Groups[1].Value);
                  ImportFamily(daten, v, uid, map);
                  g = true;
                }
              }
              if (!g)
              {
                m = sour0.Match(str0);
                if (m.Success)
                {
                  uid = GetUid(map, m.Groups[1].Value);
                  //ImportSource(daten, v, uid);
                  g = true;
                }
              }
            }
          }
          finally
          {
            v.Clear();
            v.Add(str);
          }
        }
        else
          v.Add(str);
      }
      return anzahl;
    }

    private void ImportIndividual(ServiceDaten daten, List<string> v, string uid, Dictionary<string, string> map)
    {
      var ve = new List<string>();
      var p = new SbPerson
      {
        Mandant_Nr = daten.MandantNr,
        Uid = uid,
        Geschlecht = GenderEnum.MAENNLICH.ToString(),
      };
      while (v.Any())
      {
        var str = v[0];
        v.RemoveAt(0);
        var g = false;
        var m = name1.Match(str);
        if (m.Success)
        {
          p.Name = N(m.Groups[1].Value);
          g = true;
        }
        if (!g)
        {
          m = givn2.Match(str);
          if (m.Success)
          {
            p.Vorname = N(m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = surn2.Match(str);
          if (m.Success)
          {
            p.Geburtsname = N(m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = sex1.Match(str);
          if (m.Success)
          {
            if ("F" == m.Groups[1].Value)
              p.Geschlecht = GenderEnum.WEIBLICH.ToString();
            g = true;
          }
        }
        if (!g)
        {
          m = birt1.Match(str);
          if (m.Success)
          {
            ve.Clear();
            while (GetLevel(v[0]) > 1)
            {
              ve.Add(v[0]);
              v.RemoveAt(0);
            }
            ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.BIRTH);
            g = true;
          }
        }
        if (!g)
        {
          m = buri1.Match(str);
          if (m.Success)
          {
            ve.Clear();
            while (GetLevel(v[0]) > 1)
            {
              ve.Add(v[0]);
              v.RemoveAt(0);
            }
            ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.BURIAL);
            g = true;
          }
        }
        if (!g)
        {
          m = chr1.Match(str);
          if (m.Success)
          {
            ve.Clear();
            while (GetLevel(v[0]) > 1)
            {
              ve.Add(v[0]);
              v.RemoveAt(0);
            }
            ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.CHRIST);
            g = true;
          }
        }
        if (!g)
        {
          m = deat1.Match(str);
          if (m.Success)
          {
            ve.Clear();
            while (GetLevel(v[0]) > 1)
            {
              ve.Add(v[0]);
              v.RemoveAt(0);
            }
            ImportEvent(daten, ve, uid, "INDI", GedcomEventEnum.DEATH);
            g = true;
          }
        }
        if (!g)
        {
          m = reli1.Match(str);
          if (m.Success)
          {
            p.Konfession = N(m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = note1.Match(str);
          if (m.Success)
          {
            p.Bemerkung = N(m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = cont2.Match(str);
          if (m.Success)
          {
            p.Bemerkung = Functions.Append(p.Bemerkung, null, N(m.Groups[1].Value));
            g = true;
          }
        }
        if (!g)
        {
          m = sour1.Match(str);
          if (m.Success)
          {
            p.Quelle_Uid = GetUid(map, m.Groups[1].Value);
            g = true;
          }
        }
      }
      SbPersonRep.Insert(daten, p);
    }

    private void ImportFamily(ServiceDaten daten, List<string> v, string uid, Dictionary<string, string> map)
    {
      var ve = new List<string>();
      var f = new SbFamilie
      {
        Mandant_Nr = daten.MandantNr,
        Uid = uid,
      };
      while (v.Any())
      {
        var str = v[0];
        v.RemoveAt(0);
        var g = false;
        var m = husb1.Match(str);
        if (m.Success)
        {
          f.Mann_Uid = GetUid(map, m.Groups[1].Value);
          g = true;
        }
        if (!g)
        {
          m = wife1.Match(str);
          if (m.Success)
          {
            f.Frau_Uid = GetUid(map, m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = marr1.Match(str);
          if (m.Success)
          {
            ve.Clear();
            while (GetLevel(v[0]) > 1)
            {
              ve.Add(v[0]);
              v.RemoveAt(0);
            }
            ImportEvent(daten, ve, uid, "FAM", GedcomEventEnum.MARRIAGE);
            g = true;
          }
        }
        if (!g)
        {
          m = chil1.Match(str);
          if (m.Success)
          {
            var k = new SbKind
            {
              Mandant_Nr = daten.MandantNr,
              Familie_Uid = uid,
              Kind_Uid = GetUid(map, m.Groups[1].Value),
            };
            SbKindRep.Insert(daten, k);
            g = true;
          }
        }
      }
      SbFamilieRep.Insert(daten, f);
    }

    private void ImportSource(ServiceDaten daten, List<string> v, string uid)
    {
      var zuletzt = 0;
      var q = new SbQuelle
      {
        Mandant_Nr = daten.MandantNr,
        Uid = uid,
      };
      while (v.Any())
      {
        var str = v[0];
        v.RemoveAt(0);
        var g = false;
        var m = auth1.Match(str);
        if (m.Success)
        {
          q.Autor = N(m.Groups[1].Value);
          zuletzt = 1;
          g = true;
        }
        if (!g)
        {
          m = titl1.Match(str);
          if (m.Success)
          {
            q.Beschreibung = N(m.Groups[1].Value);
            zuletzt = 2;
            g = true;
          }
        }
        if (!g)
        {
          m = text1.Match(str);
          if (m.Success)
          {
            q.Zitat = N(m.Groups[1].Value);
            zuletzt = 3;
            g = true;
          }
        }
        if (!g)
        {
          m = note1.Match(str);
          if (m.Success)
          {
            q.Bemerkung = N(m.Groups[1].Value);
            zuletzt = 4;
            g = true;
          }
        }
        if (!g)
        {
          m = cont2.Match(str);
          if (m.Success)
          {
            if (zuletzt == 1)
              q.Autor = Functions.Append(q.Autor, null, N(m.Groups[1].Value));
            else if (zuletzt == 2)
              q.Beschreibung = Functions.Append(q.Beschreibung, null, N(m.Groups[1].Value));
            else if (zuletzt == 3)
              q.Zitat = Functions.Append(q.Zitat, null, N(m.Groups[1].Value));
            else if (zuletzt == 4)
              q.Bemerkung = Functions.Append(q.Bemerkung, null, N(m.Groups[1].Value));
            g = true;
          }
        }
      }
      SbQuelleRep.Insert(daten, q);
    }

    private void ImportEvent(ServiceDaten daten, List<string> v, string uid, string nrTyp, GedcomEventEnum typ)
    {
      var datum = new PedigreeTimeData();
      var e = new SbEreignis
      {
        Mandant_Nr = daten.MandantNr,
        Person_Uid = nrTyp == "INDI" ? uid : "",
        Familie_Uid = nrTyp == "INDI" ? "" : uid,
        Typ = typ.ToString(),
      };
      while (v.Any())
      {
        var str = v[0];
        v.RemoveAt(0);
        var g = false;
        var m = date2.Match(str);
        if (m.Success)
        {
          datum.Parse(m.Groups[1].Value, true);
          e.Datum_Typ = datum.DateType;
          e.Tag1 = datum.Date1.Tag;
          e.Monat1 = datum.Date1.Monat;
          e.Jahr1 = datum.Date1.Jahr;
          e.Tag2 = datum.Date2.Tag;
          e.Monat2 = datum.Date2.Monat;
          e.Jahr2 = datum.Date2.Jahr;
          g = true;
        }
        if (!g)
        {
          m = plac2.Match(str);
          if (m.Success)
          {
            e.Ort = N(m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = note2.Match(str);
          if (m.Success)
          {
            e.Bemerkung = N(m.Groups[1].Value);
            g = true;
          }
        }
        if (!g)
        {
          m = cont3.Match(str);
          if (m.Success)
          {
            e.Bemerkung = Functions.Append(e.Bemerkung, null, N(m.Groups[1].Value));
            g = true;
          }
        }
      }
      e.Replikation_Uid = Functions.GetUid();
      SbEreignisRep.Insert(daten, e);
    }

    private int GetLevel(string str)
    {
      var l = -1;
      if (!string.IsNullOrEmpty(str))
      {
        var m = level.Match(str);
        if (m.Success)
          l = Functions.ToInt32(m.Groups[1].Value);
      }
      return l;
    }

    /**
     * Liefert eine Uid aus Objekt-Referenz.
     * @param map Mapping zwischen Objekt-Referenz und Uid für Import kleinerer IDs.
     * @param xref Objekt-Referenz als Zeichenkette.
     * @return Uid als Zeichenkette.
     */
    private String GetUid(Dictionary<string, string> map, string xref)
    {
      if (xref == null)
        return null;
      if (map == null)
        return Functions.ToUid(xref);
      if (!map.TryGetValue(xref, out var uid))
      {
        uid = xref.Length <= 6 ? Functions.GetUid() : Functions.ToUid(xref);
        map[xref] = uid;
      }
      return uid;
    }

    // /**
    //  * Automatisches Setzen von Status-Feldern.
    //  * @param stufe Gibt die Status-Nummer an.
    //  * @param nr Gibt die Nummer f�r bestimmte Stufen an.
    //  * @throws JhhException im unerwarteten Fehlerfalle.
    //  */
    // public Void setzeStatus(int stufe, int nr)
    // {
    //   int mandantNr = context.getMandantNr();
    //   SbEreignisKey sbEreignisKey = new SbEreignisKey();
    //   boolean bLog = false;

    //   if (stufe == 1)
    //   {
    //     personDao.clearStatus1(mandantNr);
    //     anzahl1 = personDao.countStatus1(mandantNr);
    //     if (bLog)
    //     {
    //       log.warn("Anzahl Alle=" + anzahl1);
    //     }
    //     // Todesjahr �bernehmen
    //     sbEreignisKey.setMandantNr(mandantNr);
    //     sbEreignisKey.setNrTyp("INDI");
    //     sbEreignisKey.setTyp(GedcomEreignis.eTOD().wert());
    //     sbEreignisKey.setNr(0);
    //     personDao.updateEreignis(sbEreignisKey);
    //     anzahl2 = personDao.countStatus1(mandantNr);
    //     if (bLog)
    //     {
    //       log.warn("Anzahl Todesjahr=" + anzahl2);
    //     }
    //     // Geburtsjahr �bernehmen
    //     sbEreignisKey.setTyp(GedcomEreignis.eGEBURT().wert());
    //     sbEreignisKey.setNr(alter);
    //     personDao.updateEreignis(sbEreignisKey);
    //     anzahl3 = personDao.countStatus1(mandantNr);
    //     if (bLog)
    //     {
    //       log.warn("Anzahl mit Geburtsjahr=" + anzahl3);
    //     }
    //     do
    //     {
    //       anzahl2 = anzahl3;
    //       // Status der Frau �bernehmen
    //       personDao.updateStatusFrau(mandantNr);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl mit Tod der Frau=" + anzahl3);
    //       }
    //       // Status des Mannes �bernehmen
    //       personDao.updateStatusMann(mandantNr);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl mit Tod des Mannes=" + anzahl3);
    //       }
    //       // Status vom Kind her als Vater sch�tzen
    //       personDao.updateStatusMannKind(mandantNr, generation);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl mit Tod des Vater-Kindes=" + anzahl3);
    //       }
    //       // Status vom Kind her als Mutter sch�tzen
    //       personDao.updateStatusFrauKind(mandantNr, generation);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl mit Tod des Mutter-Kindes=" + anzahl3);
    //       }
    //       // Status vom Vater her sch�tzen
    //       personDao.updateStatusVaterTod(mandantNr, generation);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl mit Tod des Vaters=" + anzahl3);
    //       }
    //       // Status von Mutter her sch�tzen
    //       personDao.updateStatusMutterTod(mandantNr, generation);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl mit Tod der Mutter=" + anzahl3);
    //       }
    //     } while (anzahl3 != anzahl2);
    //   }
    //   else if (stufe == 2)
    //   {
    //     SbPerson sbPerson = new SbPerson();
    //     personDao.clearStatus1(mandantNr);
    //     sbPerson.setMandantNr(mandantNr);
    //     sbPerson.setNr(nr);
    //     sbPerson.setStatus1(nr);
    //     personDao.updateStatus1(sbPerson);
    //     anzahl3 = personDao.countStatus1(mandantNr);
    //     do
    //     {
    //       anzahl2 = anzahl3;
    //       // Status der Frau �bernehmen
    //       personDao.updateStatusFrau(mandantNr);
    //       // Status des Mannes �bernehmen
    //       personDao.updateStatusMann(mandantNr);
    //       // Status von Kind auf Mutter �bertragen
    //       personDao.updateStatusFrauKind(mandantNr, 0);
    //       // Status von Kind auf Vater �bertragen
    //       personDao.updateStatusMannKind(mandantNr, 0);
    //       anzahl3 = personDao.countStatus1(mandantNr);
    //       if (bLog)
    //       {
    //         log.warn("Anzahl Status1: " + anzahl3);
    //       }
    //     } while (anzahl3 != anzahl2);
    //   }
    //   return null;
    // }

    /**
     * Schreibt den Kopf einer GEDCOM-Datei.
     * @param daten Service-Daten mit Mandantennummer.
     * @param l String-Liste zum Schreiben.
     * @param version Version der zu schreibenden GEDCOM-Schnittstelle, z.B. '5.5'.
     * @param datei Dateiname wird im Kopf vermerkt.
     */
    private void SchreibeKopf(ServiceDaten daten, List<string> l, string version, string datei)
    {
      l.Add("0 HEAD");
      l.Add("1 SOUR WKUEHL");
      l.Add("2 VERS 0.1");
      l.Add("2 NAME CSBP-Programm");
      l.Add("1 DEST ANSTFILE or TempleReady");
      l.Add("1 DATE " + daten.Jetzt.ToString("dd MMM yyyy", Functions.CultureInfoEn).ToUpper());
      l.Add("2 TIME " + daten.Jetzt.ToString("hh:mm:ss", Functions.CultureInfoEn));
      if (version.CompareTo("5.5") >= 0)
      {
        l.Add("1 SUBM @999999@"); // submitter
        l.Add("1 SUBN @999998@"); // submission
      }
      l.Add("1 FILE " + datei);
      l.Add("1 GEDC");
      l.Add("2 VERS " + version);
      l.Add("2 FORM LINEAGE-LINKED");
      // l.Add("1 CHAR ANSI");
      // UNICODE: Fehler wegen fehlendem BOM
      l.Add("1 CHAR UTF-8");
    }

    /**
     * Schreibt einen Ahnen in eine GEDCOM-Datei.
     * @param daten Service-Daten mit Mandantennummer.
     * @param l String-Liste zum Schreiben.
     * @param map Mapping zwischen Uid und Integer für Export kleinerer IDs.
     * @param vers Version der zu schreibenden GEDCOM-Schnittstelle, z.B. '5.5'.
     * @param strFilter Filter-Kriterium für Ahnen und Familien.
     */
    private void SchreibeIndividual(ServiceDaten daten, List<string> l, Dictionary<string, int> map, string vers, int status2)
    {
      var liste = SbPersonRep.GetList(daten, status2: status2);
      foreach (var p in liste)
      {
        var uid = p.Uid;
        var vn = p.Vorname;
        var gn = p.Geburtsname;
        var pn = Functions.Append(vn, " ", "/" + p.Name + "/");
        var bem = p.Bemerkung;
        var konf = p.Konfession;
        var quid = p.Quelle_Uid;
        // TODO VergleicheInt(Status1, op, tot)
        l.Add($"0 {GetXref(map, "INDI", uid)} INDI");
        l.Add("1 NAME " + pn);
        if (vers.CompareTo("5.5") >= 0)
        {
          if (!string.IsNullOrEmpty(vn))
            l.Add("2 GIVN " + vn);
          if (!string.IsNullOrEmpty(gn))
            l.Add("2 SURN " + gn);
        }
        if (p.Geschlecht != null)
        {
          var geschlecht = p.Geschlecht.ToUpper();
          geschlecht = geschlecht.Replace('W', 'F');
          if (Regex.Match(geschlecht, "M|F").Success)
            l.Add("1 SEX " + geschlecht);
        }
        // Ereignisse
        var ereignisse = SbEreignisRep.GetList(daten, uid, null, null, null);
        foreach (var e in ereignisse)
        {
          var zeitangabe = new PedigreeTimeData(e);
          l.Add("1 " + e.Typ);
          l.Add("2 DATE " + zeitangabe.Deparse(true).ToUpper());
          var ort = e.Ort;
          var ebem = e.Bemerkung;
          if (!string.IsNullOrEmpty(ort))
            l.Add("2 PLAC " + ort);
          if (!string.IsNullOrEmpty(ebem))
            SchreibeFortsetzung(l, 2, "NOTE", ebem);
        }
        if (!string.IsNullOrEmpty(konf))
          l.Add("1 RELI " + konf);
        if (!string.IsNullOrEmpty(quid))
          l.Add("1 SOUR " + GetXref(map, "SOUR", quid));
        if (!string.IsNullOrEmpty(bem))
          SchreibeFortsetzung(l, 1, "NOTE", bem);
        var kind = SbKindRep.GetList(daten, kuid: uid, status2: status2).FirstOrDefault();
        if (kind != null)
          l.Add("1 FAMC " + GetXref(map, "FAM", kind.Familie_Uid));
        var eltern = SbFamilieRep.GetList(daten, personuid: uid, status2: status2);
        string fuid = null;
        foreach (var f in eltern)
        {
          // keine doppelten Einträge
          if (f.Uid != fuid)
            l.Add("1 FAMS " + GetXref(map, "FAM", f.Uid));
          fuid = f.Uid;
        }
      }
    }

    /**
     * Schreibt eine Familie in eine GEDCOM-Datei.
     * @param daten Service-Daten mit Mandantennummer.
     * @param l String-Vector zum Schreiben.
     * @param map Mapping zwischen Uid und Integer für Export kleinerer IDs.
     * @param strFilter Filter-Kriterium für Familien.
     * @param op Vergleichsoperator für tot.
     * @param tot Todesjahr zum Vergleichen und Filtern.
     */
    private void SchreibeFamily(ServiceDaten daten, List<string> l, Dictionary<string, int> map, int status2,
      string op, int tot)
    {
      var familien = SbFamilieRep.GetList(daten, status2: status2);
      foreach (var f in familien)
      {
        var familienUid = f.Uid;
        var muid = f.Mann_Uid;
        var fuid = f.Frau_Uid;
        var mannTot = f.Father?.Status1 ?? 0;
        var frauTot = f.Mother?.Status1 ?? 0;
        // TODO if (!Functions.VergleicheInt(mannTot, op, tot))
        //   muid = null;
        // if (!Functions.VergleicheInt(frauTot, op, tot))
        //   fuid = null;
        var filter = !string.IsNullOrEmpty(muid) || !string.IsNullOrEmpty(fuid);
        if (filter)
        {
          l.Add(("0 " + GetXref(map, "FAM", familienUid)) + " FAM");
          var ereignisse = SbEreignisRep.GetList(daten, null, familienUid);
          foreach (var e in ereignisse)
          {
            var zeitangabe = new PedigreeTimeData(e);
            l.Add("1 " + e.Typ);
            l.Add("2 DATE " + zeitangabe.Deparse(true).ToUpper());
            var ort = e.Ort;
            var ebem = e.Bemerkung;
            if (!string.IsNullOrEmpty(ort))
              l.Add("2 PLAC " + ort);
            if (!string.IsNullOrEmpty(ebem))
              SchreibeFortsetzung(l, 2, "NOTE", ebem);
          }
          if (!string.IsNullOrEmpty(muid))
            l.Add("1 HUSB " + GetXref(map, "INDI", muid));
          if (!string.IsNullOrEmpty(fuid))
            l.Add("1 WIFE " + GetXref(map, "INDI", fuid));
          var children = SbKindRep.GetList(daten, familienUid);
          foreach (var c in children)
          {
            // TODO var kindUid = f.Kind_Uid;
            // var kindTot = f.kindStatus1;
            // if (!Functions.VergleicheInt(kindTot, op, tot))
            //   kindUid = null;
            // if (!string.IsNullOrEmpty(kindUid))
            l.Add("1 CHIL " + GetXref(map, "INDI", c.Kind_Uid));
          }
        }
      }
    }

    /**
     * Schreibt alle Quellen in eine GEDCOM-Datei.
     * @param daten Service-Daten mit Mandantennummer.
     * @param l String-Vector zum Schreiben.
     * @param map Mapping zwischen Uid und Integer für Export kleinerer IDs.
     * @param status2 Status2.
     */
    private void SchreibeSource(ServiceDaten daten, List<String> l, Dictionary<string, int> map, int status2)
    {
      var quellen = SbQuelleRep.GetList(daten, null, status2);
      foreach (var q in quellen)
      {
        var quid = q.Uid;
        var autor = q.Autor;
        var beschreibung = q.Beschreibung;
        var zitat = q.Zitat;
        var bemerkung = q.Bemerkung;
        l.Add($"0 {GetXref(map, "SOUR", quid)} SOUR");
        SchreibeFortsetzung(l, 1, "AUTH", autor);
        SchreibeFortsetzung(l, 1, "TITL", beschreibung);
        SchreibeFortsetzung(l, 1, "TEXT", zitat);
        SchreibeFortsetzung(l, 1, "NOTE", bemerkung);
      }
    }

    /**
     * Schreibt den Schluss einer GEDCOM-Datei.
     * @param l String-Vector zum Schreiben.
     * @param version Version der zu schreibenden GEDCOM-Schnittstelle, z.B. '5.5'.
     * @param name Bezeichnung des Stammbaums.
     */
    private void SchreibeFuss(ServiceDaten daten, List<String> l, String version, String name)
    {
      if (version.CompareTo("5.5") >= 0)
      {
        var p = Parameter.GetValue(Parameter.SB_SUBMITTER);
        if (!string.IsNullOrEmpty(p))
        {
          var arr = p.Split(';');
          l.Add("0 @999999@ SUBM");
          if (arr.Length > 0)
            l.Add("1 NAME " + arr[0]);
          if (arr.Length > 1)
            l.Add("1 ADDR " + arr[1]);
          if (arr.Length > 2)
            l.Add("2 CONT " + arr[2]);
          if (arr.Length > 3)
            l.Add("2 CONT " + arr[3]);
          if (arr.Length > 4)
            l.Add("2 CONT " + arr[4]);
          // l.Add("1 PHON 00000-11111");
        }
      }
      if (version.CompareTo("5.5") >= 0)
      {
        // submission
        l.Add("0 @999998@ SUBN");
        l.Add("1 SUBM @999999@");
        l.Add("1 FAMF " + name);
      }
      l.Add("0 TRLR");
    }

    /**
     * Liefert aus typ und nr eine Objekt-Referenz.
     * @param map Mapping zwischen Uid und Integer für Export kleinerer IDs.
     * @param typ "FAM" für Familie, "INDI" für Individuum, "SOUR" für Quelle.
     * @param uid Nummer des Objekts.
     * @return Objekt-Referenz als Zeichenkette.
     */
    private string GetXref(Dictionary<string, int> map, string typ, string uid)
    {
      string s;
      if (map == null)
        s = Functions.ToXref(uid);
      else
      {
        if (!map.TryGetValue(uid, out var i))
        {
          i = map.Count + 1;
          // if (typ == "SOUR")
          //   i = -i;
          map[uid] = i;
        }
        s = i.ToString();
      }
      if (typ == "FAM")
        return $"@F{s}@";
      else if (typ == "INDI")
        return $"@I{s}@";
      else
      { // SOUR
        return $"@Q{s}@";
      }
    }

    /**
     * Schreibt einen längeren Text in eine GEDCOM-Datei, der in maximal 248 Zeichen lange Zeilen umgebrochen wird.
     * @param l String-Vector zum Schreiben.
     * @param level Stufennummer.
     * @param type Typ des Eintrags, z.B. TITL, TEXT, AUTH.
     * @param text Auszugebender Inhalt.
     */
    private void SchreibeFortsetzung(List<string> l, int level, string type, string text)
    {
      // Maximal Länge einer GEDCOM-Zeile.
      const int MAX_GEDCOM_ZEILE = 248;
      string str = null;
      var iLen = text == null ? 0 : text.Length;
      if (iLen > 0)
      {
        var iMax = (iLen - 1) / MAX_GEDCOM_ZEILE + 1;
        for (var i = 1; i <= iMax; i++)
        {
          if (i < iMax)
            str = text.Substring((i - 1) * MAX_GEDCOM_ZEILE, i * MAX_GEDCOM_ZEILE);
          else
            str = text.Substring((i - 1) * MAX_GEDCOM_ZEILE); // i == iMax
          if (i == 1)
            l.Add(level + " " + type + " " + str);
          else
            l.Add((level + 1) + " CONT " + str);
        }
      }
    }

    /**
     * Liefert Daten zu einem Nachfahren.
     * @param daten Service-Daten mit Mandantennummer.
     * @param uid Ahnen-Nummer aus Datenbank.
     * @param stufe Daten mit (0,2) oder ohne (1,3) Ereignisse sowie mit (0,1) oder ohne (2,3)
     *        Familien-/Ehepartner-Bestimmung.
     * @param generation Hiervon abhängig werden weniger Zeichen pro Zeile zugelassen.
     * @param max maximale Anzahl von Generationen.
     * @param liste Liste von Nachfahren.
     * @return Daten als NachfahrDaten.
     */
    private void GetNachfahrenRekursiv(ServiceDaten daten, string uid, int stufe, int generation, int max, List<SbPerson> liste)
    {
      var mitEreignis = (stufe & 1) == 0;
      var mitPartner = (stufe & 2) == 0;
      var zeile = new StringBuilder();
      var auchKinder = false;

      var sbPerson = SbPersonRep.Get(daten, daten.MandantNr, uid);
      if (sbPerson == null)
        throw new MessageException(SB017(uid));
      var strGen = mitPartner ? "" + generation : "+";
      var strPraefix = new string(' ', (Math.Abs(generation) - 1) * 1);
      zeile.Append(strPraefix).Append(strGen).Append(" ").Append(
        Functions.AhnString(sbPerson.Uid, sbPerson.Geburtsname, sbPerson.Vorname, true));
      if (mitEreignis)
      {
        var ereignisse = SbEreignisRep.GetList(daten, uid);
        foreach (var e in ereignisse)
        {
          zeile.Append(" ").Append(((GedcomEventEnum)e.Typ).ToSymbol());
          var zeitangabe = new PedigreeTimeData(e);
          zeile.Append(zeitangabe.Deparse());
          Functions.Append(zeile, " in ", e.Ort);
          Functions.Append(zeile, ", ", e.Bemerkung);
        }
      }
      sbPerson.Bemerkung = zeile.ToString();
      liste.Add(sbPerson);
      if (mitPartner)
      {
        var familien = SbFamilieRep.GetList(daten, personuid: uid);
        foreach (var f in familien)
        {
          if (f.Mann_Uid == uid)
          {
            if (!string.IsNullOrEmpty(f.Frau_Uid))
            {
              GetNachfahrenRekursiv(daten, f.Frau_Uid, stufe | 2, generation, max, liste);
              // auchKinder = true;
            }
          }
          else
          {
            if (!string.IsNullOrEmpty(f.Mann_Uid))
            {
              GetNachfahrenRekursiv(daten, f.Mann_Uid, stufe | 2, generation, max, liste);
              // auchKinder = true;
            }
          }
          auchKinder = true;
          if (auchKinder && generation <= max)
          {
            var kinder = SbKindRep.GetList(daten, f.Uid);
            foreach (var k in kinder)
            {
              GetNachfahrenRekursiv(daten, k.Kind_Uid, stufe, generation + 1, max, liste);
            }
          }
        }
      }
    }

    /**
     * Liefert Daten zu einem Vorfahren.
     * @param daten Service-Daten mit Mandantennummer.
     * @param uid Ahnen-Nummer aus Datenbank.
     * @param mitEreignis Mit Ereignissen?
     * @param mitGeschwistern Mit Geschwistern?
     * @param mitEltern Mit Eltern?
     * @param generation Hiervon abhängig werden weniger Zeichen pro Zeile zugelassen.
     * @param max maximale Anzahl von Generationen.
     * @param liste Liste von Nachfahren.
     * @return Daten als NachfahrDaten.
     */
    private void GetVorfahrenRekursiv(ServiceDaten daten, string uid, bool mitEreignis, bool mitGeschwistern,
      bool mitEltern, int generation, int max, List<SbPerson> liste)
    {
      var zeile = new StringBuilder();
      var sbPerson = SbPersonRep.Get(daten, daten.MandantNr, uid);
      if (sbPerson == null)
        throw new MessageException(SB017(uid));
      var strGen = mitEltern ? "" + generation : "+";
      var strPraefix = new string(' ', (Math.Abs(generation) - 1) * 1);
      zeile.Append(strPraefix).Append(strGen).Append(" ").Append(
        Functions.AhnString(sbPerson.Uid, sbPerson.Geburtsname, sbPerson.Vorname, true));
      if (mitEreignis)
      {
        var ereignisse = SbEreignisRep.GetList(daten, uid);
        foreach (var e in ereignisse)
        {
          zeile.Append(" ").Append(((GedcomEventEnum)e.Typ).ToSymbol());
          var zeitangabe = new PedigreeTimeData(e);
          zeile.Append(zeitangabe.Deparse());
          Functions.Append(zeile, " in ", e.Ort);
          Functions.Append(zeile, ", ", e.Bemerkung);
        }
      }
      sbPerson.Bemerkung = zeile.ToString();
      liste.Add(sbPerson);
      var familieUid = GetKindFamilieIntern(daten, uid)?.Familie_Uid;
      if (mitGeschwistern && !string.IsNullOrEmpty(familieUid))
      {
        var kinder = SbKindRep.GetList(daten, familieUid, null, null, uid);
        foreach (var k in kinder)
        {
          GetVorfahrenRekursiv(daten, k.Kind_Uid, mitEreignis, false, false, generation, max, liste);
        }
      }
      if (mitEltern && generation < max && !string.IsNullOrEmpty(familieUid))
      {
        var f = SbFamilieRep.Get(daten, daten.MandantNr, familieUid);
        if (f != null)
        {
          if (!string.IsNullOrEmpty(f.Mann_Uid))
            GetVorfahrenRekursiv(daten, f.Mann_Uid, mitEreignis, mitGeschwistern, true, generation + 1, max, liste);
          if (!string.IsNullOrEmpty(f.Frau_Uid))
            GetVorfahrenRekursiv(daten, f.Frau_Uid, mitEreignis, mitGeschwistern, true, generation + 1, max, liste);
        }
      }
    }

    private void DeleteQuelleIntern(ServiceDaten daten, SbQuelle q)
    {
      var pliste = SbPersonRep.GetList(daten, suid: q.Uid);
      if (pliste.Any())
        throw new MessageException(SB015);
      var eliste = SbEreignisRep.GetList(daten, suid: q.Uid);
      if (eliste.Any())
        throw new MessageException(SB016);
      SbQuelleRep.Delete(daten, q);
    }

    /**
     * Speichert ein Personen- oder Familien-Ereignis.
     * @param daten Service-Daten mit Mandantennummer.
     * @param puid Ahnen-Nummer.
     * @param typ Typ des Ereignisses, z.B. BIRT.
     * @param datum Ereignis-Datum.
     * @param ort Ereignis-Ort.
     * @param bemerkung Ereignis-Bemerkung.
     * @param quid Quellen-Nummer zum Ereignis.
     */
    private void SpeichereEreignis(ServiceDaten daten, string puid, string fuid, string typ, string datum,
      string ort, string bemerkung, string quid)
    {
      var zeitangabe = new PedigreeTimeData();
      if (zeitangabe.Parse(datum))
        throw new MessageException(SB003(datum));
      if (string.IsNullOrEmpty(puid) && string.IsNullOrEmpty(fuid))
        throw new MessageException(SB004);
      if (string.IsNullOrEmpty(typ))
        throw new MessageException(SB005);
      if (string.IsNullOrEmpty(zeitangabe.DateType) && !zeitangabe.Date1.Empty)
        throw new MessageException(SB006);
      var loeschen = zeitangabe.Date1.Empty && string.IsNullOrEmpty(zeitangabe.DateType) && zeitangabe.Date2.Empty &&
        string.IsNullOrEmpty(ort) && string.IsNullOrEmpty(bemerkung);
      if (loeschen)
      {
        var e = SbEreignisRep.Get(daten, daten.MandantNr, puid, fuid, typ);
        if (e != null)
          SbEreignisRep.Delete(daten, e);
        return;
      }
      SbEreignisRep.Save(daten, daten.MandantNr, puid, fuid, typ, zeitangabe.Date1.Tag, zeitangabe.Date1.Monat,
        zeitangabe.Date1.Jahr, zeitangabe.Date2.Tag, zeitangabe.Date2.Monat, zeitangabe.Date2.Jahr,
        zeitangabe.DateType, ort, bemerkung, quid);
    }

    /**
     * Evtl. Anlegen einer neuen Familie.
     * @param daten Service-Daten mit Mandantennummer.
     * @param uid Familien-Nummer wird neu bestimmt, falls null.
     * @param mannUid Ahnen-Nummer des Mannes.
     * @param frauUid Ahnen-Nummer der Frau.
     * @param kindUid Ahnen-Nummer des Kindes.
     * @param doppelt Exception bei anderer Familie.
     * @return Evtl. neu bestimmte Familien-Nummer.
     * @throws Exception falls die neue Familien-Nummer nicht bestimmt werden konnte oder Ahn schon Kind in anderer
     *         Familie ist.
     */
    private SbFamilie NeueFamilie(ServiceDaten daten, string uid, string mannUid, string frauUid, string kindUid,
      bool doppelt)
    {
      var fuid = uid;
      string fuid2 = null;
      SbPerson sbPerson;
      var maUid = mannUid;
      var frUid = frauUid;
      if (string.IsNullOrEmpty(maUid) && string.IsNullOrEmpty(frUid))
        throw new MessageException(SB007);
      if (!string.IsNullOrEmpty(maUid))
      {
        sbPerson = SbPersonRep.Get(daten, daten.MandantNr, maUid);
        if (sbPerson == null || GenderEnum.MAENNLICH.ToString() != sbPerson.Geschlecht)
          throw new MessageException(SB008);
      }
      if (!string.IsNullOrEmpty(frUid))
      {
        sbPerson = SbPersonRep.Get(daten, daten.MandantNr, frUid);
        if (sbPerson == null || GenderEnum.WEIBLICH.ToString() != sbPerson.Geschlecht)
          throw new MessageException(SB009);
      }
      var fliste = SbFamilieRep.GetList(daten, null, maUid, frUid, null, fuid);
      var sbFamilie = fliste.FirstOrDefault();
      if (sbFamilie != null)
      {
        fuid2 = sbFamilie.Uid;
        maUid = sbFamilie.Mann_Uid;
        frUid = sbFamilie.Frau_Uid;
      }
      if (!string.IsNullOrEmpty(fuid2))
      {
        if (doppelt)
          throw new MessageException(SB010(fuid2));
        fuid = fuid2;
      }
      var f = IuFamilie(daten, fuid, maUid, frUid);
      fuid = f.Uid;
      if (!string.IsNullOrEmpty(kindUid))
        IuKind(daten, fuid, kindUid);
      return f;
    }

    private SbFamilie IuFamilie(ServiceDaten daten, string uid, string mannUid, string frauUid)
    {
      if (string.IsNullOrEmpty(mannUid) && string.IsNullOrEmpty(frauUid))
        throw new MessageException(SB007);
      var f = SbFamilieRep.Save(daten, daten.MandantNr, uid, mannUid, frauUid, 0, 0, 0);
      return f;
    }

    /**
     * Anlegen oder Ändern einer Familien-Kind-Zuordung.
     * @param daten Service-Daten mit Mandantennummer.
     * @param fuid Familien-Nummer.
     * @param kindUid Ahnen-Nummer des Kindes.
     */
    private SbKind IuKind(ServiceDaten daten, string fuid, string kindUid)
    {
      if (string.IsNullOrEmpty(fuid) || string.IsNullOrEmpty(kindUid))
        throw new MessageException(SB011);
      var kliste = SbKindRep.GetList(daten, null, kindUid, fuid);
      var vo2 = kliste.FirstOrDefault();
      if (vo2 != null)
        throw new MessageException(SB012(vo2.Familie_Uid));
      var k = SbKindRep.Save(daten, daten.MandantNr, fuid, kindUid);
      return k;
    }

    private void DeletePersonIntern(ServiceDaten daten, SbPerson p)
    {
      // Person aus Familie löschen
      var liste = SbFamilieRep.GetList(daten, null, null, null, p.Uid);
      foreach (var f in liste)
      {
        if (f.Mann_Uid == p.Uid)
          f.Mann_Uid = null;
        if (f.Frau_Uid == p.Uid)
          f.Frau_Uid = null;
        if (string.IsNullOrEmpty(f.Mann_Uid) && string.IsNullOrEmpty(f.Frau_Uid))
          DeleteFamilieIntern(daten, f);
        else
          SbFamilieRep.Update(daten, f);
      }

      // Person als Kind löschen
      var kliste = SbKindRep.GetList(daten, null, p.Uid);
      foreach (var k in kliste)
      {
        SbKindRep.Delete(daten, k);
      }

      // Person-Ereignisse löschen
      var eliste = SbEreignisRep.GetList(daten, p.Uid);
      foreach (var e in eliste)
      {
        SbEreignisRep.Delete(daten, e);
      }

      // Bilder löschen
      var bliste = ByteDatenRep.GetList(daten, "SB_Person", p.Uid);
      foreach (var b in bliste)
      {
        ByteDatenRep.Delete(daten, b);
      }
      SbPersonRep.Delete(daten, p);
    }

    /**
     * Löschen einer Familie mit Kindern und Ereignissen.
     * @param daten Service-Daten mit Mandantennummer.
     * @param uid Familie-Nummer.
     */
    private void DeleteFamilieIntern(ServiceDaten daten, SbFamilie f)
    {
      // Kinder löschen
      var liste = SbKindRep.GetList(daten, f.Uid);
      foreach (var k in liste)
      {
        SbKindRep.Delete(daten, k);
      }

      // Familien-Ereignisse löschen
      var eliste = SbEreignisRep.GetList(daten, null, f.Uid, null, null);
      foreach (var e in eliste)
      {
        SbEreignisRep.Delete(daten, e);
      }

      // Holt die unveränderte Familie.
      f = SbFamilieRep.Get(daten, daten.MandantNr, f.Uid);
      if (f != null)
        SbFamilieRep.Delete(daten, f);
    }

    private List<string> GetAlleEhegatten(ServiceDaten daten, string puid)
    {
      var liste = new List<string>();
      if (puid == null)
        return liste;

      var liste0 = new List<string>();
      var liste1 = new List<string>();
      liste0.Add(puid);
      var anz = -1;
      while (anz < liste.Count)
      {
        anz = liste.Count;
        liste1.Clear();
        foreach (var s in liste0)
        {
          var l = GetEhegatten0(daten, s);
          foreach (var e in l)
          {
            if (!liste.Contains(e))
            {
              liste.Add(e);
              liste1.Add(e);
            }
          }
        }
        liste0.Clear();
        liste0.AddRange(liste1);
      }
      return liste;
    }

    private List<string> GetEhegatten0(ServiceDaten daten, string puid)
    {
      var liste = new List<String>();
      if (!string.IsNullOrEmpty(puid))
      {
        var fliste = SbFamilieRep.GetList(daten, null, null, null, puid);
        foreach (var f in fliste)
        {
          if (!string.IsNullOrEmpty(f.Mann_Uid) && puid != f.Mann_Uid)
            liste.Add(f.Mann_Uid);
          if (!string.IsNullOrEmpty(f.Frau_Uid) && puid != f.Frau_Uid)
            liste.Add(f.Frau_Uid);
        }
      }
      return liste;
    }

    /**
     * Liefert erste Familie, in der der Ahn Vater oder Mutter ist.
     * @param daten Service-Daten mit Mandantennummer.
     * @param uid Ahnen-Nummer.
     * @return Familie des Ahnen.
     */
    private SbFamilie GetElternFamilieIntern(ServiceDaten daten, String uid)
    {
      var fliste = SbFamilieRep.GetList(daten, personuid: uid);
      return fliste.FirstOrDefault();
    }

    /**
     * Liefert Kind-Datensatz der Familie, in der der Ahn Kind ist.
     * @param daten Service-Daten mit Mandantennummer.
     * @param uid Ahnen-Nummer.
     * @return Kind-Datensatz des Ahnen.
     */
    private SbKind GetKindFamilieIntern(ServiceDaten daten, string uid)
    {
      var k = SbKindRep.GetList(daten, null, uid).FirstOrDefault();
      return k;
    }
  }
}
