// <copyright file="ServiceErgebnis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CSBP.Base;

/// <summary>
/// Diese Klasse beinhaltet die Ergebnisse einer Service-Funktion in Form von Fehlermeldungen, Meldungen und Entscheidungen.
/// </summary>
public class ServiceErgebnis
{
  /// <summary>Initializes a new instance of the <see cref="ServiceErgebnis"/> class.</summary>
  public ServiceErgebnis()
  {
    Messages = new List<Message>();
    Errors = new List<Message>();
    //// Entscheidungen = new List<Entscheidung>();
  }

  /// <summary>Gets the messages for the user.</summary>
  public ICollection<Message> Messages { get; private set; }

  /// <summary>Gets the errors for the user.</summary>
  public ICollection<Message> Errors { get; private set; }

  /// <summary>Gets a value indicating whether it is everything OK with no errors.</summary>
  public bool Ok
  {
    get { return Errors.Count <= 0; }
  }

  /// <summary>
  /// Gets all errors as string.
  /// </summary>
  /// <returns>The errors.</returns>
  public string GetErrors()
  {
    if (Errors.Any())
    {
      var s = string.Join(Constants.CRLF, Errors.Select(a => a.MessageText).ToArray());
      return s;
    }
    return null;
  }

  /// <summary>
  /// Copies messages and errors from another instance. Identical messages and errors are ignored.
  /// </summary>
  /// <param name="result">Affected result.</param>
  /// <returns>Is result OK or not.</returns>
  public bool Get(ServiceErgebnis result)
  {
    if (result == null)
      return false;
    AddMeldungen(result);
    return result.Ok;
  }

  /// <summary>
  /// Copies messages and errors from another instance. Identical messages and errors are ignored.
  /// </summary>
  /// <param name="result">Affected result.</param>
  /// <typeparam name="T">Result type.</typeparam>
  /// <returns>Result of result.</returns>
  public T Get<T>(ServiceErgebnis<T> result)
  {
    if (result == null)
      return default;
    AddMeldungen(result);
    return result.Ergebnis;
  }

  /// <summary>
  /// Throws first error as MessageException.
  /// </summary>
  /// <param name="postfix">Appendix for thrown messages.</param>
  public void ThrowFirstError(string postfix = null)
  {
    if (Errors.Any())
    {
      var f = Errors.First();
      if (!string.IsNullOrEmpty(postfix))
        f.Postfix(postfix);
      throw new MessageException(f);
    }
  }

  /// <summary>
  /// Throws all error as MessageException.
  /// </summary>
  /// <param name="postfix">Appendix for thrown messages.</param>
  public void ThrowAllErrors(string postfix = null)
  {
    if (Errors.Any())
    {
      var f = new Message(GetErrors(), true);
      if (!string.IsNullOrEmpty(postfix))
        f.Postfix(postfix);
      throw new MessageException(f);
    }
  }

  /// <summary>
  /// Copies messages and errors from another instance. Identical messages and errors are ignored.
  /// </summary>
  /// <param name="result">Affected result.</param>
  protected void AddMeldungen(ServiceErgebnis result)
  {
    if (result == null)
      return;
    if (result.Errors != null)
    {
      foreach (var f in result.Errors)
      {
        // Gleiche Meldungen verhindern
        if (Errors.All(f2 => f2.Text != f.Text))
          Errors.Add(f);
      }
    }
    if (result.Messages != null)
    {
      foreach (var f in result.Messages)
      {
        // Gleiche Meldungen verhindern
        if (Messages.All(f2 => f2.Text != f.Text))
          Messages.Add(f);
      }
    }
  }
}

/// <summary>
/// Service result with possibly messages and errors.
/// </summary>
/// <typeparam name="T">Result type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Gleichnamige Klassen.")]
public class ServiceErgebnis<T> : ServiceErgebnis
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ServiceErgebnis{T}"/> class.
  /// </summary>
  public ServiceErgebnis()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ServiceErgebnis{T}"/> class.
  /// </summary>
  /// <param name="result">Affected result value.</param>
  public ServiceErgebnis(T result)
  {
    Ergebnis = result;
  }

  /// <summary>Gets or sets the result value of a service function.</summary>
  public T Ergebnis { get; set; }

  /// <summary>
  /// Copies messages and errors from another instance. Identical messages and errors are ignored.
  /// </summary>
  /// <param name="result">Affected result.</param>
  /// <returns>Result of result.</returns>
  public T Get(ServiceErgebnis<T> result)
  {
    if (result == null)
      return default;
    AddMeldungen(result);
    return result.Ergebnis;
  }
}
