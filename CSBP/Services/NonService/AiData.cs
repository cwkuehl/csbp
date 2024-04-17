// <copyright file="AiData.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.NonService;

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Class for AI data.
/// </summary>
[Serializable]
public class AiData
{
  /// <summary>AI model constant for GPT 4 (Input $10/1M tokens Output $30/1M tokens).</summary>
  public const string Gpt4 = "gpt-4-turbo";

  /// <summary>AI model constant for GPT 3.5 (Input $0.5/1M tokens Output $1.5/1M tokens).</summary>
  public const string Gpt35 = "gpt-3.5-turbo";

  /// <summary>AI model constant for GPT 3.5 instruct (Input $1.5/1M tokens Output $2.0/1M tokens).</summary>
  public const string Gpt35instruct = "gpt-3.5-turbo-instruct";

  /// <summary>AI model constant for DALL E.</summary>
  public const string Dalle = "dalle";

  /// <summary>Gets or sets the AI model.</summary>
  public string Model { get; set; } = Gpt35;

  /// <summary>Gets or sets the maximal number of tokens.</summary>
  public int MaxTokens { get; set; } = 16;

  /// <summary>Gets or sets the temperature between 0 and 2, default 1.</summary>
  public decimal Temperature { get; set; } = 1;

  /// <summary>Gets or sets the top percentage for tokens, alternative to temperature, default 1.</summary>
  public decimal Topp { get; set; } = 1;

  /// <summary>Gets or sets the number of completions, default 1.</summary>
  public int N { get; set; } = 1;

  /// <summary>Gets or sets the input system prompt string for AI.</summary>
  public string SystemPrompt { get; set; }

  /// <summary>Gets the previous assistant prompts from the AI. Dialog history.</summary>
  public List<string> AssistantPrompts { get; private set; } = new();

  /// <summary>Gets or sets the input prompt string for AI.</summary>
  public string Prompt { get; set; }

  /// <summary>Gets the finish reasons: stop, length.</summary>
  public List<string> FinishReasons { get; private set; } = new();

  /// <summary>Gets the response messages.</summary>
  public List<string> Messages { get; private set; } = new();

  /// <summary>Gets response byte data arrays.</summary>
  public List<byte[]> Bytes { get; private set; } = new();

  /// <summary>Gets or sets the number of prompt tokens.</summary>
  public decimal PromptTokens { get; set; }

  /// <summary>Gets or sets the number of completion tokens.</summary>
  public decimal CompletionTokens { get; set; }

  /// <summary>Gets or sets a value indicating whether to continue as dialog.</summary>
  public bool ContinueDialog { get; set; }

  /// <summary>Gets the response messages as string.</summary>
  public string GetMessages
  {
     get { return string.Join(Environment.NewLine, Messages); }
  }

  /// <summary>Gets the dialog history as string.</summary>
  public string GetDialogHistory
  {
    get
    {
      if (!ContinueDialog)
        return GetMessages;
      var sb = new StringBuilder();
      var i = 0;
      foreach (var p in AssistantPrompts)
      {
        if (string.IsNullOrWhiteSpace(p))
          continue;
        if (sb.Length > 0)
          sb.Append(Environment.NewLine);
        if (i % 2 == 0)
          sb.Append("User: ");
        else
          sb.Append("Assistant: ");
        sb.AppendLine(p);
        i++;
      }
      return sb.ToString();
    }
  }
}
