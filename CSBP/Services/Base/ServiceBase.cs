// <copyright file="ServiceBase.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base
{
  using System;
  using System.Collections.Generic;
  using System.Data.Common;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Net.Security;
  using System.Security.Cryptography.X509Certificates;
  using System.Text;
  using System.Xml.Serialization;
  using CSBP.Apis.Services;
  using CSBP.Base;
  using CSBP.Services.Repositories;
  using CSBP.Services.Repositories.Base;
  using CSBP.Services.Undo;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.EntityFrameworkCore.Storage;
  using NLog;

  /// <summary>
  /// Basis-Klasse für alle Services.
  /// </summary>
  public class ServiceBase
  {
    /// <summary>
    /// Logger-Instanz von NLog.
    /// </summary>
    public static readonly ILogger Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Undo-Stack.
    /// </summary>
    static Stack<UndoList> UndoStack = new Stack<UndoList>();

    /// <summary>
    /// Redo-Stack.
    /// </summary>
    static Stack<UndoList> RedoStack = new Stack<UndoList>();

    protected static readonly AdAdresseRep AdAdresseRep = new AdAdresseRep();
    protected static readonly AdPersonRep AdPersonRep = new AdPersonRep();
    protected static readonly AdSitzRep AdSitzRep = new AdSitzRep();
    protected static readonly BenutzerRep BenutzerRep = new BenutzerRep();
    protected static readonly ByteDatenRep ByteDatenRep = new ByteDatenRep();
    protected static readonly FzBuchRep FzBuchRep = new FzBuchRep();
    protected static readonly FzBuchautorRep FzBuchautorRep = new FzBuchautorRep();
    protected static readonly FzBuchserieRep FzBuchserieRep = new FzBuchserieRep();
    protected static readonly FzBuchstatusRep FzBuchstatusRep = new FzBuchstatusRep();
    protected static readonly FzFahrradRep FzFahrradRep = new FzFahrradRep();
    protected static readonly FzFahrradstandRep FzFahrradstandRep = new FzFahrradstandRep();
    protected static readonly FzNotizRep FzNotizRep = new FzNotizRep();
    protected static readonly HhBilanzRep HhBilanzRep = new HhBilanzRep();
    protected static readonly HhBuchungRep HhBuchungRep = new HhBuchungRep();
    protected static readonly HhEreignisRep HhEreignisRep = new HhEreignisRep();
    protected static readonly HhKontoRep HhKontoRep = new HhKontoRep();
    protected static readonly HhPeriodeRep HhPeriodeRep = new HhPeriodeRep();
    protected static readonly MaMandantRep MaMandantRep = new MaMandantRep();
    protected static readonly MaParameterRep MaParameterRep = new MaParameterRep();
    protected static readonly SbEreignisRep SbEreignisRep = new SbEreignisRep();
    protected static readonly SbFamilieRep SbFamilieRep = new SbFamilieRep();
    protected static readonly SbKindRep SbKindRep = new SbKindRep();
    protected static readonly SbPersonRep SbPersonRep = new SbPersonRep();
    protected static readonly SbQuelleRep SbQuelleRep = new SbQuelleRep();
    protected static readonly TbEintragRep TbEintragRep = new TbEintragRep();
    protected static readonly TbEintragOrtRep TbEintragOrtRep = new TbEintragOrtRep();
    protected static readonly TbOrtRep TbOrtRep = new TbOrtRep();
    protected static readonly WpAnlageRep WpAnlageRep = new WpAnlageRep();
    protected static readonly WpBuchungRep WpBuchungRep = new WpBuchungRep();
    protected static readonly WpKonfigurationRep WpKonfigurationRep = new WpKonfigurationRep();
    protected static readonly WpStandRep WpStandRep = new WpStandRep();
    protected static readonly WpWertpapierRep WpWertpapierRep = new WpWertpapierRep();

    protected static readonly HttpClient httpsclient = new HttpClient();
    // protected static readonly HttpClient httpsclient = new HttpClient(new SocketsHttpHandler()
    // {
    //   // Enable multiple HTTP/2 connections.
    //   EnableMultipleHttp2Connections = true,

    //   // Log each newly created connection and create the connection the same way as it would be without the callback.
    //   ConnectCallback = async (context, token) =>
    //   {
    //     Debug.Print($"New connection to {context.DnsEndPoint} with request:{Environment.NewLine}{context.InitialRequestMessage}");
    //     var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
    //     await socket.ConnectAsync(context.DnsEndPoint, token).ConfigureAwait(false);
    //     return new NetworkStream(socket, ownsSocket: true);
    //   },
    // })
    // {
    //   // Allow only HTTP/2, no downgrades or upgrades.
    //   DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
    //   DefaultRequestVersion = HttpVersion.Version20
    // };

    protected const int HttpTimeout = 10000; // Für fixer.io statt 5000.

    static ServiceBase()
    {
      // Alle Zertifikate akzeptieren, TLS 1.3 als Standard-Protokoll
      ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13; // | SecurityProtocolType.Tls12;
      // 20.05.2020 TLS 1.2 eingestellt wegen Fehler bei yahoo und fixer.io (oder Router-Problem).
      // ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
      httpsclient.Timeout = TimeSpan.FromMilliseconds(HttpTimeout);
      httpsclient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:97.0) Gecko/20100101 Firefox/97.0");
      // EnableMultipleHttp2Connections = true;
    }

    private static bool MyRemoteCertificateValidationCallback(System.Object sender,
        X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
      bool ok = true;
      // If there are errors in the certificate chain, look at each error to determine the cause.
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
    /// Are there undo or redo actions?
    /// </summary>
    /// <returns>Are there undo or redo actions?</returns>
    public static bool IsUndoRedo()
    {
      return UndoStack.Any() || RedoStack.Any();
    }

    /// <summary>
    /// Erzeugen einer Log-Eintrags auf der Stufe Error.
    /// </summary>
    /// <param name="daten">Service data for database access.</param>
    /// <param name="text">Zu loggender Text.</param>
    /// <param name="error" >Schalter, ob Eintrag in Log-Datei als Error erfolgen soll?</param>
    public static void LogString(ServiceDaten daten, string text, bool error = false)
    {
      if (daten == null || string.IsNullOrEmpty(text))
        return;
      var sb = new StringBuilder();
      if (daten.MandantNr != 0 || !string.IsNullOrEmpty(daten.BenutzerId))
        sb.Append("(").Append(daten.MandantNr).Append(" ").Append(daten.BenutzerId).Append(") ");
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
      var db = daten.Context as CsbpContext;
      if (db == null)
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
          if (ul == null)
            ul = new UndoList();
          ul.List.AddRange(db.PreUndoList.List);
          db.PreUndoList.List.Clear();
        }
        Commit(db, ul);
      }
    }

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

    internal static void Commit(DbContext db, UndoList ul)
    {
      if (db == null || ul == null)
        return;
      if (ul.List.Count > 0)
      {
        UndoStack.Push(ul);
        RedoStack.Clear(); // Alle Redos sind durch das neue Commit ungültig.
      }
    }

    private void Insert(DbConnection con, ModelBase m)
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
        sql.Append("@").Append(p.Name);
        var parm = cmd.CreateParameter();
        parm.ParameterName = $"@{p.Name}";
        parm.Value = v == null ? DBNull.Value : v;
        cmd.Parameters.Add(parm);
      }
      sql.Append(")");
      cmd.CommandText = sql.ToString();
      var c = cmd.ExecuteNonQuery();
      if (c != 1)
        throw new Exception("Wrong number of inserts.");
    }

    private void Update(DbConnection con, ModelBase m, ModelBase o)
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
          parm.Value = v == null ? DBNull.Value : v;
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
          parm.Value = v == null ? DBNull.Value : v;
          cmd.Parameters.Add(parm);
        }
      }
      cmd.CommandText = sql.ToString();
      var c = cmd.ExecuteNonQuery();
      if (c != 1)
        throw new Exception("Wrong number of updates."); // Rep.Update vergessen?
    }

    private void Delete(DbConnection con, ModelBase m)
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
          parm.Value = v == null ? DBNull.Value : v;
          cmd.Parameters.Add(parm);
        }
      }
      cmd.CommandText = sql.ToString();
      var c = cmd.ExecuteNonQuery();
      if (c != 1)
        throw new Exception("Wrong number of deletes.");
    }

    protected bool Undo0(ServiceDaten daten)
    {
      if (UndoStack.Count <= 0)
        return false;
      var ul = UndoStack.Peek();
      var db = daten.Context as DbContext;
      var con = db.Database.GetDbConnection();
      foreach (var e in ul.List)
      {
        if (e.IsInsert)
        {
          //db.Entry(e.Actual).State = EntityState.Deleted;
          Delete(con, e.Actual);
        }
        else if (e.IsUpdate)
        {
          //var clone = Clone(e.Actual);
          //db.Entry(clone).State = EntityState.Unchanged;
          //// var props = clone.GetType().GetProperties().Where(a => a.CanWrite);
          //var props = clone.ColumnProperties;
          //foreach (var p in props) {
          //    var v = p.GetGetMethod().Invoke(e.Original, null);
          //    p.GetSetMethod().Invoke(clone, new[] { v });
          //}
          Update(con, e.Original, e.Actual);
        }
        else if (e.IsDelete)
        {
          //db.Entry(e.Original).State = EntityState.Added;
          Insert(con, e.Original);
        }
      }
      db.SaveChanges();
      UndoStack.Pop();
      RedoStack.Push(ul);
      return ul.List.Any();
    }

    protected bool Redo0(ServiceDaten daten)
    {
      if (RedoStack.Count <= 0)
        return false;
      var ul = RedoStack.Peek();
      var db = daten.Context as DbContext;
      var con = db.Database.GetDbConnection();
      foreach (var e in ul.List)
      {
        if (e.IsInsert)
        {
          //db.Entry(e.Actual).State = EntityState.Added;
          Insert(con, e.Actual);
        }
        else if (e.IsUpdate)
        {
          //var clone = Clone(e.Original);
          //db.Entry(clone).State = EntityState.Unchanged;
          //foreach (var p in clone.ColumnProperties) {
          //    var v = p.GetGetMethod().Invoke(e.Actual, null);
          //    p.GetSetMethod().Invoke(clone, new[] { v });
          //}
          Update(con, e.Actual, e.Original);
        }
        else if (e.IsDelete)
        {
          //db.Entry(e.Original).State = EntityState.Deleted;
          Delete(con, e.Original);
        }
        // db.SaveChanges(); // langsamer bei großen Listen
      }
      db.SaveChanges();
      RedoStack.Pop();
      UndoStack.Push(ul);
      return ul.List.Any();
    }

    public static T Clone<T>(T source)
    {
      if (!typeof(T).IsSerializable)
      {
        throw new ArgumentException($"The type {typeof(T).Name} must be serializable.", nameof(source));
      }

      // Don't serialize a null object, simply return the default for that object
      if (Object.ReferenceEquals(source, null))
      {
        return default(T);
      }

      //IFormatter formatter = new BinaryFormatter();
      Stream stream = new MemoryStream();
      using (stream)
      {
        var binSerializer = new XmlSerializer(typeof(T));
        binSerializer.Serialize(stream, source);
        //formatter.Serialize(stream, source);
        stream.Seek(0, SeekOrigin.Begin);
        //return (T)formatter.Deserialize(stream);
        return (T)binSerializer.Deserialize(stream);
      }
    }

    protected string ToStr(object o)
    {
      if (o == null)
        return "";
      if (o is int)
        return Functions.ToString((int)o);
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

    protected string FromStr(string s)
    {
      if (string.IsNullOrEmpty(s))
        return "";
      s = s.Replace("<br>", "\r\n");
      return s;
    }

    protected string N(string s)
    {
      return Functions.TrimNull(s);
    }
  }
}
