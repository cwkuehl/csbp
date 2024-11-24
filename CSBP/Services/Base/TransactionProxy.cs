// <copyright file="TransactionProxy.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CSBP.Services.Repositories.Base;
using CSBP.Services.Undo;

/// <summary>
/// Intercepts service function call for transactions.
/// </summary>
/// <typeparam name="T">Affected interface type.</typeparam>
public class TransactionProxy<T> : DispatchProxy
{
  /// <summary>Proxied instance.</summary>
  private T instance;

  /// <summary>
  /// Create a proxy with decorated instance.
  /// </summary>
  /// <param name="decorated">Decorated instance.</param>
  /// <returns>A proxy.</returns>
  public static T Create(T decorated)
  {
    object proxy = Create<T, TransactionProxy<T>>();
    ((TransactionProxy<T>)proxy).SetParameters(decorated);
    return (T)proxy;
  }

  /// <summary>
  /// Invocation of service function call. Puts exection into ServiceErgebnis.
  /// </summary>
  /// <param name="method">Affected method.</param>
  /// <param name="args">Affected parameters.</param>
  /// <returns>Possibly errors.</returns>
  protected override object Invoke(MethodInfo method, object[] args)
  {
    if (method == null)
      throw new ArgumentException(null, nameof(method));

    // var methodCall = msg as IMethodCallMessage;
    object returnValue = null;
    ServiceDaten daten = null;
    CsbpContext db = null;
    Message fehlermeldung = null;

    // try {
    //    Console.WriteLine("Before invoke: " + method.Name);
    //    var result = method.Invoke(_instance, methodCall.InArgs);
    //    Console.WriteLine("After invoke: " + method.Name);
    //    return new ReturnMessage(result, null, 0, methodCall.LogicalCallContext, methodCall);
    // } catch (Exception e) {
    //    Console.WriteLine("Exception: " + e);
    //    if (e is TargetInvocationException && e.InnerException != null) {
    //        return new ReturnMessage(e.InnerException, msg as IMethodCallMessage);
    //    }
    //    return new ReturnMessage(e, msg as IMethodCallMessage);
    // }
    try
    {
      var tx = false;
      if (args != null)
      {
        foreach (var d in args)
        {
          if (d is ServiceDaten)
          {
            daten = d as ServiceDaten;
            daten.Tiefe++;
            if (daten.Context == null)
            {
              // Es läuft keine Transaktion.
              db = new CsbpContext();
              daten.Context = db;
              daten.UndoList = new UndoList();
              tx = true;
            }
            break;
          }
        }
      }

      // Transaction-Attribute auswerten
      // var attribute =
      //    (TransactionAttribute[])invocation.Method.GetCustomAttributes(typeof(TransactionAttribute), false);
      // if (attribute.Length > 0) {
      //    var il = attribute[0].IsolationLevel;
      //    if (attribute[0].ReadOnly || il == IsolationLevel.Unspecified)
      //        tx = false;
      // }
      if (tx)
      {
        using var transaction = db.Database.BeginTransaction();
        try
        {
          daten.Tx = transaction;
          returnValue = method.Invoke(instance, args);
          //// UndoList füllen und Commit
          ServiceBase.SaveChanges(daten, transaction);
          //// var ul = ServiceBase.PreCommit(db);
          //// db.SaveChanges(); // <== Nur hier, sonst kein Undo, Redo möglich.
          //// transaction.Commit();
          //// ServiceBase.Commit(db, ul);
        }
        catch (Exception e)
        {
          StringBuilder sb = null;
          var ex = e.InnerException ?? e;

          if (daten != null && daten.Tiefe == 1)
          {
            // nur auf Einstiegsebene
            if (ServiceBase.Log.IsErrorEnabled && ex is not MessageException)
            {
              // 26.05.15 WK: Keine MeldungException protokollieren, da sie gewollt ist.
              //    if (ex is DbEntityValidationException) {
              //        sb = new StringBuilder();
              //        foreach (var v in (ex as DbEntityValidationException).EntityValidationErrors) {
              //            if (v.Entry != null && v.Entry.Entity != null)
              //                sb.Append("Entity ").Append(v.Entry.Entity.GetType().Name).Append(" ");
              //            foreach (var e in v.ValidationErrors) {
              //                sb.Append(e.ErrorMessage).Append(" ").Append(e.PropertyName);
              //                sb.Append("\r\n");
              //            }
              //        }
              //        ServiceBase.Log.Error(
              //            string.Format("{0} mit Validierungsfehlern: {1}",
              //                MethodError(invocation.Method.Name, daten), sb), ex);
              //    } else if (ex.InnerException is OptimisticConcurrencyException) {
              //        sb = new StringBuilder();
              //        foreach (
              //            var v in (ex.InnerException as OptimisticConcurrencyException).StateEntries) {
              //            if (v.Entity != null)
              //                sb.Append("Entity ").Append(v.Entity.GetType().Name).Append("\r\n");
              //        }
              //        ServiceBase.Log.Error(
              //            string.Format("{0} mit Aktualisierungsfehlern: {1}",
              //                MethodError(invocation.Method.Name, daten), sb), ex);
              //    } else
              ServiceBase.Log.Error(ex, MethodError(ex, method.Name, daten));
            }
          }

          transaction.Rollback();

          if (daten != null && daten.Tiefe == 1)
          {
            // Only on initial level.
            if (ex is MessageException)
              fehlermeldung = (ex as MessageException).GetMessage();
            else
              fehlermeldung = new Message(ex.Message + ((sb == null) ? "" : " Validierungsfehler: " + sb), true);
          }
          else
            throw;
        }
        finally
        {
          daten.Context = null;
          daten.Tx = null;
        }
      }
      else
      {
        returnValue = method.Invoke(instance, args);
      }
    }
    catch (Exception ex)
    {
      if (daten != null && daten.Tiefe == 1)
      {
        // Only on initial level.
        if (ServiceBase.Log.IsErrorEnabled && ex is not MessageException)
          ServiceBase.Log.Error(ex, MethodError(ex, method.Name, daten));
        if (ex is MessageException)
          fehlermeldung = (ex as MessageException).GetMessage();
        else
          fehlermeldung = new Message(ex.Message, true);
      }
      else if (ex is TargetInvocationException && ex.InnerException != null)
        throw ex.InnerException;
      else
        throw;
    }
    finally
    {
      if (daten != null)
        daten.Tiefe--;
      db?.Dispose();
    }

    if (returnValue == null && method.ReturnParameter != null && method.ReturnParameter.ParameterType != typeof(void))
    {
      // Standardwerte für Rückgabewerte setzen, damit keine NullReferenceException entsteht.
      if (method.ReturnParameter.ParameterType == typeof(bool))
        returnValue = false;
      else
        returnValue = Activator.CreateInstance(method.ReturnParameter.ParameterType);
    }

    if (fehlermeldung != null && returnValue != null && typeof(ServiceErgebnis).IsAssignableFrom(returnValue.GetType()))
    {
      // Fehlermeldung in den Rückgabewert einfügen
      var mi = returnValue.GetType().GetProperty("Errors");
      if (mi.GetGetMethod().Invoke(returnValue, null) is ICollection<Message> errors)
        errors.Add(fehlermeldung);
    }
    return returnValue;
  }

  /// <summary>
  /// Gets string with method name and service data.
  /// </summary>
  /// <param name="ex">Affected exception.</param>
  /// <param name="methode">Affected method.</param>
  /// <param name="daten">Affected service data.</param>
  /// <returns>String with method name and service data.</returns>
  private static string MethodError(Exception ex, string methode, ServiceDaten daten)
  {
    var sb = new StringBuilder("Fehler in Methode '");
    sb.Append(methode).Append('\'');
    if (daten != null)
    {
      sb.Append(" Mandant ").Append(daten.MandantNr);
      sb.Append(" Benutzer ").Append(daten.BenutzerId);
    }
    if (ex != null)
    {
      sb.Append(": ").Append(ex.Message);
    }
    var st = new System.Diagnostics.StackTrace(4, true);
    sb.Append(" Herkunft ").Append(st);
    return sb.ToString();
  }

  /// <summary>
  /// Sets the decorated instance as instance.
  /// </summary>
  /// <param name="decorated">Decorated instance.</param>
  private void SetParameters(T decorated)
  {
    if (decorated == null)
      throw new ArgumentNullException(nameof(decorated));
    instance = decorated;
  }
}

//// Verwenden
//// var intf = TransactionProxy<IMyInterface>.Create(new MyClass());
//// intf.MyProcedure();
