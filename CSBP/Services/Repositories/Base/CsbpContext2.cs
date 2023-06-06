// <copyright file="CsbpContext2.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Repositories.Base;

using CSBP.Apis.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Generated part of database context.
/// </summary>
public partial class CsbpContext : DbContext
{
  /// <summary>Gets or sets the set of rows of table AD_Adresse.</summary>
  public DbSet<AdAdresse> AD_Adresse { get; set; }

  /// <summary>Gets or sets the set of rows of table AD_Person.</summary>
  public DbSet<AdPerson> AD_Person { get; set; }

  /// <summary>Gets or sets the set of rows of table AD_Sitz.</summary>
  public DbSet<AdSitz> AD_Sitz { get; set; }

  /// <summary>Gets or sets the set of rows of table AG_Dialog.</summary>
  public DbSet<AgDialog> AG_Dialog { get; set; }

  /// <summary>Gets or sets the set of rows of table Benutzer.</summary>
  public DbSet<Benutzer> Benutzer { get; set; }

  /// <summary>Gets or sets the set of rows of table Byte_Daten.</summary>
  public DbSet<ByteDaten> Byte_Daten { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Buch.</summary>
  public DbSet<FzBuch> FZ_Buch { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Buchautor.</summary>
  public DbSet<FzBuchautor> FZ_Buchautor { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Buchserie.</summary>
  public DbSet<FzBuchserie> FZ_Buchserie { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Buchstatus.</summary>
  public DbSet<FzBuchstatus> FZ_Buchstatus { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Fahrrad.</summary>
  public DbSet<FzFahrrad> FZ_Fahrrad { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Fahrradstand.</summary>
  public DbSet<FzFahrradstand> FZ_Fahrradstand { get; set; }

  /// <summary>Gets or sets the set of rows of table FZ_Notiz.</summary>
  public DbSet<FzNotiz> FZ_Notiz { get; set; }

  /// <summary>Gets or sets the set of rows of table HH_Bilanz.</summary>
  public DbSet<HhBilanz> HH_Bilanz { get; set; }

  /// <summary>Gets or sets the set of rows of table HH_Buchung.</summary>
  public DbSet<HhBuchung> HH_Buchung { get; set; }

  /// <summary>Gets or sets the set of rows of table HH_Ereignis.</summary>
  public DbSet<HhEreignis> HH_Ereignis { get; set; }

  /// <summary>Gets or sets the set of rows of table HH_Konto.</summary>
  public DbSet<HhKonto> HH_Konto { get; set; }

  /// <summary>Gets or sets the set of rows of table HH_Periode.</summary>
  public DbSet<HhPeriode> HH_Periode { get; set; }

  /// <summary>Gets or sets the set of rows of table MA_Mandant.</summary>
  public DbSet<MaMandant> MA_Mandant { get; set; }

  /// <summary>Gets or sets the set of rows of table MA_Parameter.</summary>
  public DbSet<MaParameter> MA_Parameter { get; set; }

  /// <summary>Gets or sets the set of rows of table SB_Ereignis.</summary>
  public DbSet<SbEreignis> SB_Ereignis { get; set; }

  /// <summary>Gets or sets the set of rows of table SB_Familie.</summary>
  public DbSet<SbFamilie> SB_Familie { get; set; }

  /// <summary>Gets or sets the set of rows of table SB_Kind.</summary>
  public DbSet<SbKind> SB_Kind { get; set; }

  /// <summary>Gets or sets the set of rows of table SB_Person.</summary>
  public DbSet<SbPerson> SB_Person { get; set; }

  /// <summary>Gets or sets the set of rows of table SB_Quelle.</summary>
  public DbSet<SbQuelle> SB_Quelle { get; set; }

  /// <summary>Gets or sets the set of rows of table TB_Eintrag.</summary>
  public DbSet<TbEintrag> TB_Eintrag { get; set; }

  /// <summary>Gets or sets the set of rows of table TB_Eintrag_Ort.</summary>
  public DbSet<TbEintragOrt> TB_Eintrag_Ort { get; set; }

  /// <summary>Gets or sets the set of rows of table TB_Ort.</summary>
  public DbSet<TbOrt> TB_Ort { get; set; }

  /// <summary>Gets or sets the set of rows of table TB_Wetter.</summary>
  public DbSet<TbWetter> TB_Wetter { get; set; }

  /// <summary>Gets or sets the set of rows of table WP_Anlage.</summary>
  public DbSet<WpAnlage> WP_Anlage { get; set; }

  /// <summary>Gets or sets the set of rows of table WP_Buchung.</summary>
  public DbSet<WpBuchung> WP_Buchung { get; set; }

  /// <summary>Gets or sets the set of rows of table WP_Konfiguration.</summary>
  public DbSet<WpKonfiguration> WP_Konfiguration { get; set; }

  /// <summary>Gets or sets the set of rows of table WP_Stand.</summary>
  public DbSet<WpStand> WP_Stand { get; set; }

  /// <summary>Gets or sets the set of rows of table WP_Wertpapier.</summary>
  public DbSet<WpWertpapier> WP_Wertpapier { get; set; }

  /// <summary>
  /// On the model creating generated.
  /// </summary>
  /// <param name="modelBuilder">Model builder.</param>
  private static void OnModelCreatingGenerated(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<AdAdresse>().HasKey(a => new { a.Mandant_Nr, a.Uid });
    modelBuilder.Entity<AdPerson>().HasKey(a => new { a.Mandant_Nr, a.Uid });
    modelBuilder.Entity<AdSitz>().HasKey(a => new { a.Mandant_Nr, a.Person_Uid, a.Reihenfolge, a.Uid });
    modelBuilder.Entity<AgDialog>().HasKey(a => new { a.Mandant_Nr, a.Api, a.Datum, a.Nr });
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
    modelBuilder.Entity<TbWetter>().HasKey(a => new { a.Mandant_Nr, a.Datum, a.Ort_Uid, a.Api });
    modelBuilder.Entity<WpAnlage>().HasKey(a => new { a.Mandant_Nr, a.Uid });
    modelBuilder.Entity<WpBuchung>().HasKey(a => new { a.Mandant_Nr, a.Uid });
    modelBuilder.Entity<WpKonfiguration>().HasKey(a => new { a.Mandant_Nr, a.Uid });
    modelBuilder.Entity<WpStand>().HasKey(a => new { a.Mandant_Nr, a.Wertpapier_Uid, a.Datum });
    modelBuilder.Entity<WpWertpapier>().HasKey(a => new { a.Mandant_Nr, a.Uid });
  }
}
