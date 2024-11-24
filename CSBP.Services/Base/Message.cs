// <copyright file="Message.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.Base;

/// <summary>
/// Message with message number, text and optionally parameters.
/// </summary>
public class Message
{
  /// <summary>
  /// Initializes a new instance of the <see cref="Message"/> class.
  /// </summary>
  /// <param name="m">Affected message text.</param>
  /// <param name="nurtext">Only text or text with leading number.</param>
  public Message(string m, bool nurtext = false)
    : this(m, nurtext, null)
  {
    Functions.MachNichts();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Message"/> class.
  /// </summary>
  /// <param name="nrtext">Message text with 5 leading characters as number.</param>
  /// <param name="parameter">Affected parameters for filling into message text.</param>
  public Message(string nrtext, params object[] parameter)
    : this(nrtext, false, parameter)
  {
    Functions.MachNichts();
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Message"/> class.
  /// </summary>
  /// <param name="m">Message text with or without 5 leading characters as number.</param>
  /// <param name="nurtext">Only text or text with leading number.</param>
  /// <param name="parameter">Affected parameters for filling into message text.</param>
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
      Number = m[..5].Trim();
      Text = m[5..];
    }
    Parameter = parameter;
  }

  /// <summary>Gets the message number.</summary>
  public string Number { get; private set; }

  /// <summary>Gets the message text.</summary>
  public string Text { get; private set; }

  /// <summary>Gets the message parameter.</summary>
  public object[] Parameter { get; private set; }

  /// <summary>Gets the message with text and parameters.</summary>
  public string MessageText
  {
    get
    {
      if (Parameter == null)
        return Text; // reiner Text
      return string.Format(Text, Parameter);
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="Message"/> class.
  /// </summary>
  /// <param name="nrtext">Message text with 5 leading characters as number.</param>
  /// <param name="parameter">Affected parameters for filling into message text.</param>
  /// <returns>New instance of the <see cref="Message"/> class.</returns>
  public static Message New(string nrtext, params object[] parameter)
  {
    var m = new Message(nrtext, parameter);
    return m;
  }

  /// <summary>
  /// Extends the message text with a postfix.
  /// </summary>
  /// <param name="postfix">Affected extension.</param>
  /// <returns>Own instance.</returns>
  public Message Postfix(string postfix)
  {
    if (!string.IsNullOrEmpty(postfix))
      Text += postfix;
    return this;
  }
}
