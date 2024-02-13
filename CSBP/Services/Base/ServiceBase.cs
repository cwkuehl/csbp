// <copyright file="ServiceBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using CSBP.Apis.Models;
using CSBP.Apis.Services;
using CSBP.Base;
using CSBP.Services.Repositories;
using CSBP.Services.Repositories.Base;
using CSBP.Services.Undo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;

/// <summary>
/// Base class for services.
/// </summary>
public class ServiceBase
{
  /// <summary>Logger instance of NLog.</summary>
  public static readonly ILogger Log = LogManager.GetCurrentClassLogger();

  /// <summary>Milliseconds for http request timeout (10000 for fixer.io instead of 5000).</summary>
  protected const int HttpTimeout = 10000;

  /// <summary>Milliseconds between http requests.</summary>
  protected const int HttpDelay = 1; // 500

  /// <summary>Repository for table AD_Adresse.</summary>
  protected static readonly AdAdresseRep AdAdresseRep = new();

  /// <summary>Repository for table AD_Person.</summary>
  protected static readonly AdPersonRep AdPersonRep = new();

  /// <summary>Repository for table AD_Sitz.</summary>
  protected static readonly AdSitzRep AdSitzRep = new();

  /// <summary>Repository for table AG_Dialog.</summary>
  protected static readonly AgDialogRep AgDialogRep = new();

  /// <summary>Repository for table Benutzer.</summary>
  protected static readonly BenutzerRep BenutzerRep = new();

  /// <summary>Repository for table Byte_Daten.</summary>
  protected static readonly ByteDatenRep ByteDatenRep = new();

  /// <summary>Repository for table FZ_Buch.</summary>
  protected static readonly FzBuchRep FzBuchRep = new();

  /// <summary>Repository for table FZ_Buchautor.</summary>
  protected static readonly FzBuchautorRep FzBuchautorRep = new();

  /// <summary>Repository for table FZ_Buchserie.</summary>
  protected static readonly FzBuchserieRep FzBuchserieRep = new();

  /// <summary>Repository for table FZ_Buchstatus.</summary>
  protected static readonly FzBuchstatusRep FzBuchstatusRep = new();

  /// <summary>Repository for table FZ_Fahrrad.</summary>
  protected static readonly FzFahrradRep FzFahrradRep = new();

  /// <summary>Repository for table FZ_Fahrradstand.</summary>
  protected static readonly FzFahrradstandRep FzFahrradstandRep = new();

  /// <summary>Repository for table FZ_Notiz.</summary>
  protected static readonly FzNotizRep FzNotizRep = new();

  /// <summary>Repository for table HH_Bilanz.</summary>
  protected static readonly HhBilanzRep HhBilanzRep = new();

  /// <summary>Repository for table HH_Buchung.</summary>
  protected static readonly HhBuchungRep HhBuchungRep = new();

  /// <summary>Repository for table HH_Ereignis.</summary>
  protected static readonly HhEreignisRep HhEreignisRep = new();

  /// <summary>Repository for table HH_Konto.</summary>
  protected static readonly HhKontoRep HhKontoRep = new();

  /// <summary>Repository for table HH_Periode.</summary>
  protected static readonly HhPeriodeRep HhPeriodeRep = new();

  /// <summary>Repository for table MA_Mandant.</summary>
  protected static readonly MaMandantRep MaMandantRep = new();

  /// <summary>Repository for table MA_Parameter.</summary>
  protected static readonly MaParameterRep MaParameterRep = new();

  /// <summary>Repository for table SB_Ereignis.</summary>
  protected static readonly SbEreignisRep SbEreignisRep = new();

  /// <summary>Repository for table SB_Familie.</summary>
  protected static readonly SbFamilieRep SbFamilieRep = new();

  /// <summary>Repository for table SB_Kind.</summary>
  protected static readonly SbKindRep SbKindRep = new();

  /// <summary>Repository for table SB_Person.</summary>
  protected static readonly SbPersonRep SbPersonRep = new();

  /// <summary>Repository for table SB_Quelle.</summary>
  protected static readonly SbQuelleRep SbQuelleRep = new();

  /// <summary>Repository for table TB_Eintrag.</summary>
  protected static readonly TbEintragRep TbEintragRep = new();

  /// <summary>Repository for table TB_Eintrag_Ort.</summary>
  protected static readonly TbEintragOrtRep TbEintragOrtRep = new();

  /// <summary>Repository for table TB_Ort.</summary>
  protected static readonly TbOrtRep TbOrtRep = new();

  /// <summary>Repository for table TB_Wetter.</summary>
  protected static readonly TbWetterRep TbWetterRep = new();

  /// <summary>Repository for table WP_Anlage.</summary>
  protected static readonly WpAnlageRep WpAnlageRep = new();

  /// <summary>Repository for table WP_Buchung.</summary>
  protected static readonly WpBuchungRep WpBuchungRep = new();

  /// <summary>Repository for table WP_Konfiguration.</summary>
  protected static readonly WpKonfigurationRep WpKonfigurationRep = new();

  /// <summary>Repository for table WP_Stand.</summary>
  protected static readonly WpStandRep WpStandRep = new();

  /// <summary>Repository for table WP_Wertpapier.</summary>
  protected static readonly WpWertpapierRep WpWertpapierRep = new();

  /// <summary>Https client.</summary>
  protected static readonly HttpClient Httpsclient = new();

  /// <summary>Undo stack.</summary>
  private static readonly Stack<UndoList> UndoStack = new();

  /// <summary>Redo stack.</summary>
  private static readonly Stack<UndoList> RedoStack = new();

  //// protected static readonly HttpClient httpsclient = new HttpClient(new SocketsHttpHandler()
  //// {
  ////   // Enable multiple HTTP/2 connections.
  ////   EnableMultipleHttp2Connections = true,
  ////   // Log each newly created connection and create the connection the same way as it would be without the callback.
  ////   ConnectCallback = async (context, token) =>
  ////   {
  ////     Debug.Print($"New connection to {context.DnsEndPoint} with request:{Environment.NewLine}{context.InitialRequestMessage}");
  ////     var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
  ////     await socket.ConnectAsync(context.DnsEndPoint, token).ConfigureAwait(false);
  ////     return new NetworkStream(socket, ownsSocket: true);
  ////   },
  //// })
  //// {
  ////   // Allow only HTTP/2, no downgrades or upgrades.
  ////   DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
  ////   DefaultRequestVersion = HttpVersion.Version20
  //// };

  /// <summary>
  /// Initializes static members of the <see cref="ServiceBase"/> class.
  /// </summary>
  static ServiceBase()
  {
    // Accepts all certificates, TLS 1.3 as default protokoll
    ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13; // | SecurityProtocolType.Tls12;
    //// 20.05.2020 TLS 1.2 removed because of error with yahoo and fixer.io (or router problem).
    Httpsclient.Timeout = TimeSpan.FromMilliseconds(HttpTimeout);
    Httpsclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:97.0) Gecko/20100101 Firefox/97.0");
    //// EnableMultipleHttp2Connections = true;
  }

  /// <summary>
  /// Checks if there are undo or redo actions.
  /// </summary>
  /// <returns>Are there undo or redo actions or not.</returns>
  public static bool IsUndoRedo()
  {
    return UndoStack.Any() || RedoStack.Any();
  }

  /// <summary>
  /// Creates a log entry of error level.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="text">Affected text.</param>
  /// <param name="error">Prepend 'INFO: ' text or not.</param>
  public static void LogString(ServiceDaten daten, string text, bool error = false)
  {
    if (daten == null || string.IsNullOrEmpty(text))
      return;
    var sb = new StringBuilder();
    if (daten.MandantNr != 0 || !string.IsNullOrEmpty(daten.BenutzerId))
      sb.Append('(').Append(daten.MandantNr).Append(' ').Append(daten.BenutzerId).Append(") ");
    if (!error)
      sb.Append("INFO: ");
    sb.Append(text);
    Log.Error(sb.ToString());
  }

  /// <summary>
  /// Save database changes.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="transaction">Affected transaction.</param>
  public static void SaveChanges(ServiceDaten daten, IDbContextTransaction transaction = null)
  {
    if (daten.Context is not CsbpContext db)
      return;
    var ul = PreCommit(db);
    db.SaveChanges(); // <== Nur hier, sonst kein Undo, Redo möglich.
    if (transaction == null)
    {
      if (ul != null)
        db.PreUndoList.List.AddRange(ul.List);
    }
    else
    {
      transaction.Commit();
      if (db.PreUndoList.List.Any())
      {
        ul ??= new UndoList();
        ul.List.AddRange(db.PreUndoList.List);
        db.PreUndoList.List.Clear();
      }
      Commit(ul);
    }
  }

  /// <summary>
  /// Clones an object instance by serialization.
  /// </summary>
  /// <param name="source">Affected object instance.</param>
  /// <typeparam name="T">Affected object type.</typeparam>
  /// <returns>Clone of source.</returns>
  public static T Clone<T>(T source)
  {
    // Obsolete SYSLIB0050
    // if (!typeof(T).IsSerializable)
    // {
    //   throw new ArgumentException($"The type {typeof(T).Name} must be serializable.", nameof(source));
    // }

    // Don't serialize a null object, simply return the default for that object
    if (object.ReferenceEquals(source, null))
    {
      return default;
    }

    // IFormatter formatter = new BinaryFormatter();
    Stream stream = new MemoryStream();
    using (stream)
    {
      var binSerializer = new XmlSerializer(typeof(T));
      binSerializer.Serialize(stream, source);
      //// formatter.Serialize(stream, source);
      stream.Seek(0, SeekOrigin.Begin);
      //// return (T)formatter.Deserialize(stream);
      return (T)binSerializer.Deserialize(stream);
    }
  }

  /// <summary>
  /// Creates undo entries.
  /// </summary>
  /// <param name="db">Affected database context.</param>
  /// <returns>List of undo entries.</returns>
  internal static UndoList PreCommit(DbContext db)
  {
    var ct = db.ChangeTracker;
    var ul = new UndoList();
    foreach (var e in ct.Entries())
    {
      if (e.State == EntityState.Added)
        ul.Insert(e.CurrentValues.ToObject() as ModelBase);
      else if (e.State == EntityState.Modified)
        ul.Update(e.OriginalValues.ToObject() as ModelBase, e.CurrentValues.ToObject() as ModelBase);
      else if (e.State == EntityState.Deleted)
        ul.Delete(e.CurrentValues.ToObject() as ModelBase);
    }
    return ul.List.Count <= 0 ? null : ul;
  }

  /// <summary>
  /// Commit a new undo list.
  /// </summary>
  /// <param name="ul">Affected undo list.</param>
  internal static void Commit(UndoList ul)
  {
    if (ul == null)
      return;
    if (ul.List.Count > 0)
    {
      UndoStack.Push(ul);
      RedoStack.Clear(); // All redos are invalid by the new commit.
    }
  }

  /// <summary>
  /// Undoes first undo list of undo stack.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Is anything changed or not.</returns>
  protected static bool Undo0(ServiceDaten daten)
  {
    if (UndoStack.Count <= 0)
      return false;
    var ul = UndoStack.Peek();
    var db = daten.Context as DbContext;
    var con = db.Database.GetDbConnection();
    foreach (var e in ul.List)
    {
      if (e.IsFileData)
      {
        if (e.IsInsert)
        {
          if (e.Actual is FileData fd)
            File.Delete(fd.Name);
        }
      }
      else if (e.IsInsert)
      {
        Delete(con, e.Actual);
      }
      else if (e.IsUpdate)
      {
        Update(con, e.Original, e.Actual);
      }
      else if (e.IsDelete)
      {
        Insert(con, e.Original);
      }
    }
    db.SaveChanges();
    UndoStack.Pop();
    RedoStack.Push(ul);
    return ul.List.Any();
  }

  /// <summary>
  /// Redoes first redo list of redo stack.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <returns>Is anything changed or not.</returns>
  protected static bool Redo0(ServiceDaten daten)
  {
    if (RedoStack.Count <= 0)
      return false;
    var ul = RedoStack.Peek();
    var db = daten.Context as DbContext;
    var con = db.Database.GetDbConnection();
    foreach (var e in ul.List)
    {
      if (e.IsFileData)
      {
        if (e.IsInsert)
        {
          if (e.Actual is FileData fd)
            File.WriteAllBytes(fd.Name, fd.Bytes);
        }
      }
      else if (e.IsInsert)
      {
        // db.Entry(e.Actual).State = EntityState.Added;
        Insert(con, e.Actual);
      }
      else if (e.IsUpdate)
      {
        // var clone = Clone(e.Original);
        // db.Entry(clone).State = EntityState.Unchanged;
        // foreach (var p in clone.ColumnProperties) {
        //    var v = p.GetGetMethod().Invoke(e.Actual, null);
        //    p.GetSetMethod().Invoke(clone, new[] { v });
        // }
        Update(con, e.Actual, e.Original);
      }
      else if (e.IsDelete)
      {
        // db.Entry(e.Original).State = EntityState.Deleted;
        Delete(con, e.Original);
      }
      //// db.SaveChanges(); // langsamer bei großen Listen
    }
    db.SaveChanges();
    RedoStack.Pop();
    UndoStack.Push(ul);
    return ul.List.Any();
  }

  /// <summary>
  /// Converts object to string with additional replacing line break by &lt; br &gt;.
  /// </summary>
  /// <param name="o">Affected object.</param>
  /// <returns>Converted string.</returns>
  protected static string ToStr(object o)
  {
    if (o == null)
      return "";
    if (o is int @int)
      return Functions.ToString(@int);
    if (o is int?)
      return Functions.ToString((int?)o);
    if (o is string)
    {
      var s = Functions.ToString(o as string);
      s = s.Replace("\r\n", "<br>");
      s = s.Replace("\r", "<br>");
      s = s.Replace("\n", "<br>");
      return s;
    }
    if (o is DateTime?)
      return Functions.ToString(o as DateTime?, true);
    if (o is decimal?)
      return Functions.ToString(o as decimal?, 2);
    return "";
  }

  /// <summary>
  /// Converts string by replacing &lt; br &gt; with line break.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Converted string.</returns>
  protected static string FromStr(string s)
  {
    if (string.IsNullOrEmpty(s))
      return "";
    s = s.Replace("<br>", "\r\n");
    return s;
  }

  /// <summary>
  /// Trins a string. An empty string will be returned as null.
  /// </summary>
  /// <param name="s">Affected string.</param>
  /// <returns>Trimmed string or null.</returns>
  protected static string N(string s)
  {
    return Functions.TrimNull(s);
  }

  /// <summary>
  /// Get a string value from a json property.
  /// </summary>
  /// <param name="a">Affected json element.</param>
  /// <param name="prop">Affected property name.</param>
  /// <returns>Value of json property.</returns>
  protected static string GetString(JsonElement a, string prop)
  {
    return Functions.FilterWindows1252(a.TryGetProperty(prop, out var p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null);
  }

  /// <summary>
  /// Get a datetime value from a json property.
  /// </summary>
  /// <param name="a">Affected json element.</param>
  /// <param name="prop">Affected property name.</param>
  /// <param name="tolocal">Change to local time.</param>
  /// <returns>Value of json property.</returns>
  protected static DateTime? GetDateTime(JsonElement a, string prop, bool tolocal = true)
  {
    var d = a.TryGetProperty(prop, out var p) && p.ValueKind == JsonValueKind.String ? p.GetDateTime() : (DateTime?)null;
    if (tolocal)
      return Functions.ToDateTimeLocal(d);
    return d;
  }

  /// <summary>
  /// Get a decimal value from a json property.
  /// </summary>
  /// <param name="a">Affected json element.</param>
  /// <param name="prop">Affected property name.</param>
  /// <returns>Value of json property.</returns>
  protected static decimal? GetDecimal(JsonElement a, string prop)
  {
    return a.TryGetProperty(prop, out var p) && p.ValueKind == JsonValueKind.Number ? p.GetDecimal() : (decimal?)null;
  }

  /// <summary>
  /// Replicates a diary entry.
  /// </summary>
  /// <param name="daten">Service data for database access.</param>
  /// <param name="e">Local entity.</param>
  protected void ReplicateDiary(ServiceDaten daten, TbEintrag e)
  {
    if (daten == null || e == null)
      return;
    var es = TbEintragRep.Get(daten, daten.MandantNr, e.Datum);
    if (es == null)
      TbEintragRep.Save(daten, daten.MandantNr, e.Datum, e.Eintrag, e.Angelegt_Von, e.Angelegt_Am, e.Geaendert_Von, e.Geaendert_Am);
    else if (es.Eintrag != e.Eintrag)
    {
      // Wenn es.angelegtAm != e.angelegtAm, Einträge zusammenkopieren
      // Wenn es.angelegtAm == e.angelegtAm und (e.geaendertAm == null oder es.geaendertAm > e.geaendertAm), Eintrag lassen
      // Wenn es.angelegtAm == e.angelegtAm und es.geaendertAm <= e.geaendertAm, Eintrag überschreiben
      if (e.Angelegt_Am.HasValue && (!es.Angelegt_Am.HasValue || es.Angelegt_Am != e.Angelegt_Am))
      {
        // Zusammenkopieren
        es.Eintrag = @$"Server: {e.Eintrag}
Lokal: {es.Eintrag}";
        es.Angelegt_Am = e.Angelegt_Am;
        es.Angelegt_Von = e.Angelegt_Von;
        es.Geaendert_Am = e.Geaendert_Am;
        es.Geaendert_Von = e.Geaendert_Von;
        TbEintragRep.Save(daten, daten.MandantNr, es.Datum, es.Eintrag,
          es.Angelegt_Von, es.Angelegt_Am, es.Geaendert_Von, es.Geaendert_Am);
      }
      else if (es.Angelegt_Am.HasValue && e.Angelegt_Am.HasValue && es.Angelegt_Am == e.Angelegt_Am
        && es.Geaendert_Am.HasValue && (!e.Geaendert_Am.HasValue || es.Geaendert_Am > e.Geaendert_Am))
      {
        // Lassen
      }
      else
      {
        // Überschreiben
        es.Eintrag = e.Eintrag;
        es.Angelegt_Am = e.Angelegt_Am;
        es.Angelegt_Von = e.Angelegt_Von;
        es.Geaendert_Am = e.Geaendert_Am;
        es.Geaendert_Von = e.Geaendert_Von;
        TbEintragRep.Save(daten, daten.MandantNr, es.Datum, es.Eintrag,
          es.Angelegt_Von, es.Angelegt_Am, es.Geaendert_Von, es.Geaendert_Am);
      }
    }
  }

  /// <summary>
  /// Checks whether a certificate is valid or not.
  /// </summary>
  /// <param name="sender">Affected sender.</param>
  /// <param name="certificate">Affected certificate.</param>
  /// <param name="chain">Affected certificate chain.</param>
  /// <param name="sslPolicyErrors">Affected policy errors.</param>
  /// <returns>Certificate is valid or not.</returns>
  private static bool MyRemoteCertificateValidationCallback(object sender,
    X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
  {
    bool ok = true;
    //// If there are errors in the certificate chain, look at each error to determine the cause.
    if (sslPolicyErrors != SslPolicyErrors.None)
    {
      for (int i = 0; i < chain.ChainStatus.Length; i++)
      {
        if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown)
          continue;
        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
        bool chainIsValid = chain.Build((X509Certificate2)certificate);
        if (!chainIsValid)
        {
          ok = false;
          break;
        }
      }
    }
    return ok;
  }

  /// <summary>
  /// Creates and executes an insert statement.
  /// </summary>
  /// <param name="con">Affected database connection.</param>
  /// <param name="m">Affected entity.</param>
  private static void Insert(DbConnection con, ModelBase m)
  {
    var t = m.TableName;
    var props = m.ColumnProperties;
    var sql = new StringBuilder($"INSERT INTO {t} (");
    var cmd = con.CreateCommand();
    var and = false;
    foreach (var p in props)
    {
      if (and)
        sql.Append(", ");
      else
        and = true;
      sql.Append(p.Name);
    }
    sql.Append(") VALUES (");
    and = false;
    foreach (var p in props)
    {
      var v = p.GetGetMethod().Invoke(m, null);
      if (and)
        sql.Append(", ");
      else
        and = true;
      sql.Append('@').Append(p.Name);
      var parm = cmd.CreateParameter();
      parm.ParameterName = $"@{p.Name}";
      parm.Value = v ?? DBNull.Value;
      cmd.Parameters.Add(parm);
    }
    sql.Append(')');
    cmd.CommandText = sql.ToString();
    var c = cmd.ExecuteNonQuery();
    if (c != 1)
      throw new Exception("Wrong number of inserts.");
  }

  /// <summary>
  /// Creates and executes an update statement.
  /// </summary>
  /// <param name="con">Affected database connection.</param>
  /// <param name="m">Affected actual entity.</param>
  /// <param name="o">Affected original entity.</param>
  private static void Update(DbConnection con, ModelBase m, ModelBase o)
  {
    var t = m.TableName;
    var props = m.ColumnProperties;
    var sql = new StringBuilder($"UPDATE {t} SET ");
    var cmd = con.CreateCommand();
    var and = false;
    foreach (var p in props)
    {
      var v = p.GetGetMethod().Invoke(m, null);
      if (and)
        sql.Append(", ");
      else
        and = true;
      if (v == null) // || ((v is DateTime?) && !(v as DateTime?).HasValue))
        sql.Append(p.Name).Append("=NULL");
      else
      {
        sql.Append(p.Name).Append("=@").Append(p.Name);
        var parm = cmd.CreateParameter();
        parm.ParameterName = $"@{p.Name}";
        parm.Value = v ?? DBNull.Value;
        cmd.Parameters.Add(parm);
      }
    }
    sql.Append(" WHERE ");
    and = false;
    foreach (var p in props)
    {
      var v = p.GetGetMethod().Invoke(o, null);
      if (and)
        sql.Append(" AND ");
      else
        and = true;
      if (v == null) // || ((v is DateTime?) && !(v as DateTime?).HasValue))
        sql.Append(p.Name).Append(" IS NULL");
      else
      {
        sql.Append(p.Name).Append("=@").Append(p.Name).Append("_O");
        var parm = cmd.CreateParameter();
        parm.ParameterName = $"@{p.Name}_O";
        parm.Value = v ?? DBNull.Value;
        cmd.Parameters.Add(parm);
      }
    }
    cmd.CommandText = sql.ToString();
    var c = cmd.ExecuteNonQuery();
    if (c != 1)
    {
      Debug.WriteLine(cmd.CommandText);
      foreach (DbParameter p in cmd.Parameters)
        Debug.WriteLine($"{p.ParameterName}: {p.Value?.ToString()}");
      throw new Exception("Wrong number of updates."); // Call of Rep.Update not made?
    }
  }

  /// <summary>
  /// Creates and executes a delete statement.
  /// </summary>
  /// <param name="con">Affected database connection.</param>
  /// <param name="m">Affected entity.</param>
  private static void Delete(DbConnection con, ModelBase m)
  {
    var t = m.TableName;
    var props = m.ColumnProperties;
    var sql = new StringBuilder($"DELETE FROM {t} WHERE ");
    var cmd = con.CreateCommand();
    var and = false;
    foreach (var p in props)
    {
      var v = p.GetGetMethod().Invoke(m, null);
      if (and)
        sql.Append(" AND ");
      else
        and = true;
      if (v == null) // || ((v is DateTime?) && !(v as DateTime?).HasValue))
        sql.Append(p.Name).Append(" IS NULL");
      else
      {
        sql.Append(p.Name).Append("=@").Append(p.Name);
        var parm = cmd.CreateParameter();
        parm.ParameterName = $"@{p.Name}";
        parm.Value = v ?? DBNull.Value;
        cmd.Parameters.Add(parm);
      }
    }
    cmd.CommandText = sql.ToString();
    var c = cmd.ExecuteNonQuery();
    if (c != 1)
      throw new Exception("Wrong number of deletes.");
  }
}
