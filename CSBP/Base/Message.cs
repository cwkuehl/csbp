// <copyright file="Message.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Base
{
  using System;

  public class Message
  {
    string Number { get; set; }

    /// <summary>
    /// Holt den Meldungstext.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Holt die Meldungsparameter.
    /// </summary>
    public object[] Parameter { get; private set; }

    /// <summary>
    /// Holt den zusammengesetzten Meldungstext aus Text und Parametern.
    /// </summary>
    public string MessageText
    {
      get
      {
        if (Parameter == null)
          return Text; // reiner Text
        return string.Format(Text, Parameter);
      }
    }

    public Message(string m, bool nurtext = false) : this(m, nurtext, null)
    {
      Functions.MachNichts();
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="Message" /> Klasse.
    /// </summary>
    /// <param name="nrtext">String mit 5-stelliger Nummer und Text kommt aus Resourcen-Datei.</param>
    /// <param name="parameter">Parameter, die in die Meldung eingefügt werden müssen.</param>
    public Message(string nrtext, params object[] parameter) : this(nrtext, false, parameter)
    {
      Functions.MachNichts();
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="Message" /> Klasse.
    /// </summary>
    /// <param name="m">String mit 5-stelliger Nummer und Text kommt aus Resourcen-Datei.</param>
    /// <param name="nurtext">Ist es nur Text und keine Nummer?</param>
    /// <param name="parameter">Parameter, die in die Meldung eingefügt werden müssen.</param>
    public Message(string m, bool nurtext, params object[] parameter)
    {
      if (nurtext)
      {
        Text = m;
      }
      else if (string.IsNullOrEmpty(m) || m.Length < 5)
      {
        Text = $"Unbekannter Text mit Nr.: {m}";
      }
      else
      {
        Number = m.Substring(0, 5).Trim();
        Text = m.Substring(5);
      }
      Parameter = parameter;
    }

    /// <summary>
    /// Erweitert den Text um ein Postfix.
    /// </summary>
    /// <param name="postfix">Postfix einer erweiterten Meldung.</param>
    /// <returns>Eigene Instanz.</returns>
    public Message Postfix(string postfix)
    {
      if (!string.IsNullOrEmpty(postfix))
        Text = Text + postfix;
      return this;
    }

    /// <summary>
    /// Initialisiert eine neue Instanz der <see cref="Message" /> Klasse.
    /// </summary>
    /// <param name="nrtext">String mit 5-stelliger Nummer und Text kommt aus Resourcen-Datei.</param>
    /// <param name="parameter">Parameter, die in die Meldung eingefügt werden müssen.</param>
    /// <returns>Neue Instanz von <see cref="Message"/>.</returns>
    public static Message New(string nrtext, params object[] parameter)
    {
      var m = new Message(nrtext, parameter);
      return m;
    }
  }
}
