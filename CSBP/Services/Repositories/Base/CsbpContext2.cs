// <copyright file="CsbpContext2.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base
{
  using CSBP.Apis.Models;
  using Microsoft.EntityFrameworkCore;

  /// <summary>
  /// Generierter Teil des Datenbank-Context.
  /// </summary>
  public partial class CsbpContext : DbContext
  {
    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle AD_Adresse.</summary>
    public DbSet<AdAdresse> AD_Adresse { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle AD_Person.</summary>
    public DbSet<AdPerson> AD_Person { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle AD_Sitz.</summary>
    public DbSet<AdSitz> AD_Sitz { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle Benutzer.</summary>
    public DbSet<Benutzer> Benutzer { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle Byte_Daten.</summary>
    public DbSet<ByteDaten> Byte_Daten { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Buch.</summary>
    public DbSet<FzBuch> FZ_Buch { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Buchautor.</summary>
    public DbSet<FzBuchautor> FZ_Buchautor { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Buchserie.</summary>
    public DbSet<FzBuchserie> FZ_Buchserie { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Buchstatus.</summary>
    public DbSet<FzBuchstatus> FZ_Buchstatus { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Fahrrad.</summary>
    public DbSet<FzFahrrad> FZ_Fahrrad { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Fahrradstand.</summary>
    public DbSet<FzFahrradstand> FZ_Fahrradstand { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle FZ_Notiz.</summary>
    public DbSet<FzNotiz> FZ_Notiz { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle HH_Bilanz.</summary>
    public DbSet<HhBilanz> HH_Bilanz { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle HH_Buchung.</summary>
    public DbSet<HhBuchung> HH_Buchung { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle HH_Ereignis.</summary>
    public DbSet<HhEreignis> HH_Ereignis { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle HH_Konto.</summary>
    public DbSet<HhKonto> HH_Konto { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle HH_Periode.</summary>
    public DbSet<HhPeriode> HH_Periode { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle MA_Mandant.</summary>
    public DbSet<MaMandant> MA_Mandant { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle MA_Parameter.</summary>
    public DbSet<MaParameter> MA_Parameter { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle SB_Ereignis.</summary>
    public DbSet<SbEreignis> SB_Ereignis { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle SB_Familie.</summary>
    public DbSet<SbFamilie> SB_Familie { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle SB_Kind.</summary>
    public DbSet<SbKind> SB_Kind { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle SB_Person.</summary>
    public DbSet<SbPerson> SB_Person { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle SB_Quelle.</summary>
    public DbSet<SbQuelle> SB_Quelle { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle TB_Eintrag.</summary>
    public DbSet<TbEintrag> TB_Eintrag { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle TB_Eintrag_Ort.</summary>
    public DbSet<TbEintragOrt> TB_Eintrag_Ort { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle TB_Ort.</summary>
    public DbSet<TbOrt> TB_Ort { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle WP_Anlage.</summary>
    public DbSet<WpAnlage> WP_Anlage { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle WP_Buchung.</summary>
    public DbSet<WpBuchung> WP_Buchung { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle WP_Konfiguration.</summary>
    public DbSet<WpKonfiguration> WP_Konfiguration { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle WP_Stand.</summary>
    public DbSet<WpStand> WP_Stand { get; set; }

    /// <summary>Holt oder setzt die Menge von Sätzen der Tabelle WP_Wertpapier.</summary>
    public DbSet<WpWertpapier> WP_Wertpapier { get; set; }

    /// <summary>
    /// On the model creating generated.
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    void OnModelCreatingGenerated(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<AdAdresse>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<AdPerson>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<AdSitz>().HasKey(a => new { a.Mandant_Nr, a.Person_Uid, a.Reihenfolge, a.Uid });
      modelBuilder.Entity<Benutzer>().HasKey(a => new { a.Mandant_Nr, a.Benutzer_ID });
      modelBuilder.Entity<ByteDaten>().HasKey(a => new { a.Mandant_Nr, a.Typ, a.Uid, a.Lfd_Nr });
      modelBuilder.Entity<FzBuch>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<FzBuchautor>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<FzBuchserie>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<FzBuchstatus>().HasKey(a => new { a.Mandant_Nr, a.Buch_Uid });
      modelBuilder.Entity<FzFahrrad>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<FzFahrradstand>().HasKey(a => new { a.Mandant_Nr, a.Fahrrad_Uid, a.Datum, a.Nr });
      modelBuilder.Entity<FzNotiz>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<HhBilanz>().HasKey(a => new { a.Mandant_Nr, a.Periode, a.Kz, a.Konto_Uid });
      modelBuilder.Entity<HhBuchung>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<HhEreignis>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<HhKonto>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<HhPeriode>().HasKey(a => new { a.Mandant_Nr, a.Nr });
      modelBuilder.Entity<MaMandant>().HasKey(a => a.Nr);
      modelBuilder.Entity<MaParameter>().HasKey(a => new { a.Mandant_Nr, a.Schluessel });
      modelBuilder.Entity<SbEreignis>().HasKey(a => new { a.Mandant_Nr, a.Person_Uid, a.Familie_Uid, a.Typ });
      modelBuilder.Entity<SbFamilie>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<SbKind>().HasKey(a => new { a.Mandant_Nr, a.Familie_Uid, a.Kind_Uid });
      modelBuilder.Entity<SbPerson>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<SbQuelle>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<TbEintrag>().HasKey(a => new { a.Mandant_Nr, a.Datum });
      modelBuilder.Entity<TbEintragOrt>().HasKey(a => new { a.Mandant_Nr, a.Ort_Uid, a.Datum_Von, a.Datum_Bis });
      modelBuilder.Entity<TbOrt>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<WpAnlage>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<WpBuchung>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<WpKonfiguration>().HasKey(a => new { a.Mandant_Nr, a.Uid });
      modelBuilder.Entity<WpStand>().HasKey(a => new { a.Mandant_Nr, a.Wertpapier_Uid, a.Datum });
      modelBuilder.Entity<WpWertpapier>().HasKey(a => new { a.Mandant_Nr, a.Uid });
    }
  }
}
