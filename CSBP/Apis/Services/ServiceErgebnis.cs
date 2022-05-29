// <copyright file="ServiceErgebnis.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Apis.Services
{
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;
  using System.Linq;
  using CSBP.Base;

  /// <summary>
  /// Diese Klasse beinhaltet die Ergebnisse einer Service-Funktion in Form von Fehlermeldungen, Meldungen und Entscheidungen.
  /// </summary>
  public class ServiceErgebnis
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ServiceErgebnis" /> Klasse.
    /// </summary>
    public ServiceErgebnis()
    {
      Messages = new List<Message>();
      Errors = new List<Message>();
      // Entscheidungen = new List<Entscheidung>();
    }

    /// <summary>
    /// Holt die Meldungen für den Benutzer.
    /// </summary>
    public ICollection<Message> Messages { get; private set; }

    /// <summary>
    /// Holt die Fehlermeldungen für den Benutzer.
    /// </summary>
    public ICollection<Message> Errors { get; private set; }

    /// <summary>
    /// Holt einen Wert, der angibt, ob alles OK ist, d.h. keine Fehlermeldungen
    /// und keine Entscheidungen vorliegen.
    /// </summary>
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
    /// Übernimmt Meldungen, Fehlermeldungen von einem anderen Ergebnis.
    /// Doppelte Einträge werden verhindert.
    /// </summary>
    /// <param name="ergebnis">Ein anderes Ergebnis.</param>
    /// <returns>Ist ergebnis OK?</returns>
    public bool Get(ServiceErgebnis ergebnis)
    {
      if (ergebnis == null)
        return false;
      AddMeldungen(ergebnis);
      return ergebnis.Ok;
    }

    /// <summary>
    /// Übernimmt Meldungen, Fehlermeldungen von einem anderen Ergebnis.
    /// Doppelte Einträge werden verhindert.
    /// </summary>
    /// <param name="ergebnis">Ein anderes Ergebnis.</param>
    /// <returns>Ist ergebnis OK?</returns>
    public S Get<S>(ServiceErgebnis<S> ergebnis)
    {
      if (ergebnis == null)
        return default;
      AddMeldungen(ergebnis);
      return ergebnis.Ergebnis;
    }

    /// <summary>
    /// Wirft den ersten Fehler als MessageException.
    /// </summary>
    /// <param name="postfix">Sollen Angaben zur Fehlermeldung hinzugefügt werden?</param>
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
    /// Wirft alle Fehler als MessageException.
    /// </summary>
    /// <param name="postfix">Sollen Angaben zur Fehlermeldung hinzugefügt werden?</param>
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
    /// Übernimmt Meldungen, Fehlermeldungen von einem anderen Ergebnis.
    /// Doppelte Einträge werden verhindert.
    /// </summary>
    /// <param name="ergebnis">Ein ServiceErgebnis.</param>
    protected void AddMeldungen(ServiceErgebnis ergebnis)
    {
      if (ergebnis == null)
        return;
      if (ergebnis.Errors != null)
      {
        foreach (var f in ergebnis.Errors)
        {
          // Gleiche Meldungen verhindern
          if (Errors.All(f2 => f2.Text != f.Text))
            Errors.Add(f);
        }
      }
      if (ergebnis.Messages != null)
      {
        foreach (var f in ergebnis.Messages)
        {
          // Gleiche Meldungen verhindern
          if (Messages.All(f2 => f2.Text != f.Text))
            Messages.Add(f);
        }
      }
    }
  }

  /// <summary>
  /// Diese Klasse beinhaltet die Ergebnisse einer Service-Funktion in Form von Rückgabewert, Fehlermeldungen, Meldungen und Entscheidungen.
  /// Die Klasse übernimmt auch die Funktionen der Klasse WebServiceRueckgabe.
  /// </summary>
  /// <typeparam name="T">Typ des Ergebnis.</typeparam>
  [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Gleichnamige Klassen.")]
  public class ServiceErgebnis<T> : ServiceErgebnis
  {
    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ServiceErgebnis{T}" /> Klasse.
    /// </summary>
    public ServiceErgebnis()
    {
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="ServiceErgebnis{T}" /> Klasse.
    /// </summary>
    /// <param name="ergebnis">Der eigentliche Rückgabewert.</param>
    public ServiceErgebnis(T ergebnis)
    {
      Ergebnis = ergebnis;
    }

    /// <summary>
    /// Holt oder setzt den eigentlichen Rückgabewert der Service-Funktion.
    /// </summary>
    public T Ergebnis { get; set; }

    /// <summary>
    /// Übernimmt Meldungen, Fehlermeldungen von einem anderen Ergebnis.
    /// Doppelte Einträge werden verhindert.
    /// </summary>
    /// <param name="ergebnis">Ein anderes Ergebnis.</param>
    /// <returns>Das Ergebnis von ergebnis.</returns>
    public T Get(ServiceErgebnis<T> ergebnis)
    {
      if (ergebnis == null)
        return default;
      AddMeldungen(ergebnis);
      return ergebnis.Ergebnis;
    }
  }
}
