// <copyright file="AiData.cs" company="cwkuehl.de">
// Copyright (c) cwkuehl.de. All rights reserved.
// </copyright>

namespace CSBP.Services.NonService;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using CSBP.Base;

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

  /// <summary>AI model constant for DALL E 2 (1024×1024 $0.020/image 512×512 $0.018/image 256×256 $0.016/image).</summary>
  public const string Dalle2 = "dall-e-2";

  /// <summary>Local AI model constant for Llama 3.</summary>
  public const string LocalLlama3 = "llama3_max";

  /// <summary>Local AI model constant for CodeLlama 7B.</summary>
  public const string LocalCodeLlama7B = "codellama_max";

  /// <summary>List with all AI models.</summary>
  private static readonly List<Tuple<string, string>> Ailist =
  [
    new Tuple<string, string>(Gpt35, "GPT-3.5"),
    new Tuple<string, string>(Gpt4, "GPT-4"),
    new Tuple<string, string>(Gpt35instruct, "GPT-3.5 instruct"),
    new Tuple<string, string>(Dalle2, "DALL E 2"),
    new Tuple<string, string>(LocalLlama3, "Local Llama 3"),
    new Tuple<string, string>(LocalCodeLlama7B, "Local Code Llama 7B"),
  ];

  /// <summary>Gets list with AI models.</summary>
  public static List<Tuple<string, string>> GetAiList
  {
    get { return Ailist; }
  }

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

  /// <summary>Gets or sets the dialog uid.</summary>
  public string DialogUid { get; set; }

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

  /// <summary>
  /// Parses the request and response string.
  /// </summary>
  /// <param name="req">Affected request string.</param>
  /// <param name="resp">Affected response string.</param>
  /// <param name="data">Affected ai data or null.</param>
  /// <returns>Parsed data.</returns>
  public static AiData ParseRequestResponse(string req, string resp, AiData data = null)
  {
    data ??= new AiData();
    if (!string.IsNullOrEmpty(req))
    {
      using var doc = JsonDocument.Parse(req);
      var root = doc.RootElement;
      if (root.TryGetProperty("model", out var model))
      {
        data.Model = Functions.TrimNull(model.GetString());
      }
      if (root.TryGetProperty("prompt", out var prompt))
      {
        // gpt-3.5-turbo-instruct
        data.Prompt = Functions.TrimNull(prompt.GetString());
        data.AssistantPrompts.Add(Functions.TrimNull(prompt.GetString()));
      }
      if (root.TryGetProperty("temperature", out var temperature))
      {
        data.Temperature = temperature.GetDecimal();
      }
      if (root.TryGetProperty("max_tokens", out var mt))
      {
        data.MaxTokens = mt.GetInt32();
      }
      if (root.TryGetProperty("messages", out var messages))
      {
        var arr = messages.EnumerateArray();
        while (arr.MoveNext())
        {
          var arr1 = arr.Current;
          if (arr1.TryGetProperty("content", out var c) && arr1.TryGetProperty("role", out var role))
          {
            if (role.GetString() == "system")
              data.SystemPrompt = Functions.TrimNull(c.GetString());
            else if (role.GetString() == "user")
            {
              data.Prompt = Functions.TrimNull(c.GetString());
              data.AssistantPrompts.Add(Functions.TrimNull(c.GetString()));
            }
            else if (role.GetString() == "assistant")
              data.AssistantPrompts.Add(Functions.TrimNull(c.GetString()));
          }
        }
      }
    }
    if (!string.IsNullOrEmpty(resp))
    {
      using var doc = JsonDocument.Parse(resp);
      var root = doc.RootElement;
      if (root.TryGetProperty("usage", out var usage))
      {
        if (usage.TryGetProperty("prompt_tokens", out var t1))
        {
          data.PromptTokens = t1.GetDecimal();
        }
        if (usage.TryGetProperty("completion_tokens", out var t2))
        {
          data.CompletionTokens = t2.GetDecimal();
        }
      }
      if (root.TryGetProperty("choices", out var choices))
      {
        var arr = choices.EnumerateArray();
        while (arr.MoveNext())
        {
          var arr1 = arr.Current;
          if (arr1.TryGetProperty("message", out var message))
          {
            // gpt-3.5-turbo
            if (message.TryGetProperty("content", out var c))
            {
              data.Messages.Add(Functions.TrimNull(c.GetString()));
              data.AssistantPrompts.Add(Functions.TrimNull(c.GetString()));
            }
            break;
          }
          else if (arr1.TryGetProperty("text", out var ptext))
          {
            // gpt-3.5-turbo-instruct
            data.Messages.Add(Functions.TrimNull(ptext.GetString()));
            data.AssistantPrompts.Add(Functions.TrimNull(ptext.GetString()));
          }
          if (arr1.TryGetProperty("finish_reason", out var t3))
          {
            data.FinishReasons.Add(Functions.TrimNull(t3.GetString()));
          }
        }
      }
      else if (root.TryGetProperty("message", out var message))
      {
        // Local Llama
        // {"model":"llama3_max","created_at":"2024-04-28T20:23:37.556685488Z","message":{"role":"assistant","content":"Das ist ein Test, okay! Ich bin bereit, um meine Fähigkeiten zu zeigen. Los geht's! Was ist das nächste Problem?"},"done":true,"total_duration":169306417245,"load_duration":24180638401,"prompt_eval_count":58,"prompt_eval_duration":16433220000,"eval_count":33,"eval_duration":128422227000}
        if (message.TryGetProperty("content", out var c))
        {
          data.Messages.Add(Functions.TrimNull(c.GetString()));
          data.AssistantPrompts.Add(Functions.TrimNull(c.GetString()));
        }
        if (root.TryGetProperty("prompt_eval_count", out var t1))
        {
          data.PromptTokens = t1.GetDecimal();
        }
        if (root.TryGetProperty("eval_count", out var t2))
        {
          data.CompletionTokens = t2.GetDecimal();
        }
        var duration = root.TryGetProperty("total_duration", out var t3) ? t3.GetDecimal() : 0;
        if (duration > 0)
          data.FinishReasons.Add($"{Functions.ToString(duration / 1e9m)} s");
      }
      else if (root.TryGetProperty("data", out var data1))
      {
        var arr = data1.EnumerateArray();
        while (arr.MoveNext())
        {
          var arr1 = arr.Current;
          if (arr1.TryGetProperty("url", out var c))
          {
            data.Messages.Add(Functions.TrimNull(c.GetString()));
          }
        }
      }
    }
    if (data.AssistantPrompts.Count > 2)
      data.ContinueDialog = true;
    return data;
  }
}
