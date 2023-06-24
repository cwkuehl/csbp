// <copyright file="AntlrTest.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.UnitTest;

using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using NUnit.Framework;

/// <summary>
/// Class for testing and generating files.
/// </summary>
public class AntlrTest
{
  /// <summary>Test setup.</summary>
  [SetUp]
  public void Setup()
  {
  }

  /// <summary>Test of parser.</summary>
  [Test]
  public void TestParser()
  {
    var text = @"Modell ABC
";
    var stream = new AntlrInputStream(text);
    var lexer = new IdgLexer(stream);
    var tokens = new CommonTokenStream(lexer);
    var parser = new IdgParser(tokens);
    parser.RemoveErrorListeners();
    parser.AddErrorListener(new ParserErrorListener());
    var chatContext = parser.model();
    var visitor = new BasicIdgVisitor();
    visitor.Visit(chatContext);

    foreach (var line in visitor.Lines)
    {
      Console.WriteLine("{0} has said {1}", line.Person, line.Text);
    }
  }

  /// <summary>Record for strings.</summary>
  public class SpeakLine
  {
    /// <summary>Gets or sets the person.</summary>
    public string Person { get; set; }

    /// <summary>Gets or sets the text.</summary>
    public string Text { get; set; }
  }

  /// <summary>Idg Visitor.</summary>
  public class BasicIdgVisitor : IdgBaseVisitor<object>
  {
    /// <summary>Gets the list of lines.</summary>
    public List<SpeakLine> Lines { get; } = new();

    /// <summary>Visit of mainmodel.</summary>
    /// <param name="context">Affected context.</param>
    /// <returns>Affected line.</returns>
    public override object VisitMainmodel(IdgParser.MainmodelContext context)
    {
      // NameContext name = context.name();
      // OpinionContext opinion = context.opinion();
      // var line = new SpeakLine() { Person = name.GetText(), Text = opinion.GetText().Trim('"') };
      var line = new SpeakLine() { Person = context.GetText(), Text = context.GetText() };
      Lines.Add(line);
      return line;
    }
  }

  /// <summary>Parser error listener.</summary>
  public class ParserErrorListener : BaseErrorListener
  {
    /// <summary>Handles a syntax error.</summary>
    /// <param name="output">Affected output.</param>
    /// <param name="recognizer">Affected recognizer.</param>
    /// <param name="offendingSymbol">Affected offending symbol.</param>
    /// <param name="line">Affected line number.</param>
    /// <param name="charPositionInLine">Affected position in line.</param>
    /// <param name="msg">Affected message text.</param>
    /// <param name="e">Affected exception.</param>
    public override void SyntaxError(
        TextWriter output, IRecognizer recognizer,
        IToken offendingSymbol, int line,
        int charPositionInLine, string msg,
        RecognitionException e)
    {
      string sourceName = recognizer.InputStream.SourceName;
      Console.WriteLine("line:{0} col:{1} src:{2} msg:{3}", line, charPositionInLine, sourceName, msg);
      Console.WriteLine("--------------------");
      Console.WriteLine(e);
      Console.WriteLine("--------------------");
    }
  }
}
