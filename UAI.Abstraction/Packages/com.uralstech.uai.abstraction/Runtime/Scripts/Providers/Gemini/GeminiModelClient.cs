// Copyright 2024 URAV ADVANCED LEARNING SYSTEMS PRIVATE LIMITED
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if COM_URALSTECH_UGEMINI

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Uralstech.UAI.Abstraction.Chat;
using Uralstech.UAI.Abstraction.Tools;
using Uralstech.UGemini;
using Uralstech.UGemini.Models;
using Uralstech.UGemini.Models.Content;
using Uralstech.UGemini.Models.Generation;
using Uralstech.UGemini.Models.Generation.Chat;
using Uralstech.UGemini.Models.Generation.Safety;
using Uralstech.UGemini.Models.Generation.Tools;
using Uralstech.UGemini.Models.Generation.Tools.Declaration;

namespace Uralstech.UAI.Abstraction.Providers.Gemini
{
    public class GeminiModelClient : IModelClient
    {
        /// <summary>
        /// Constant for the model provider.
        /// </summary>
        public const string ModelProvider = "gemini";

        /// <inheritdoc/>
        string IModelClient.ModelProvider => ModelProvider;

        /// <inheritdoc/>
        public string DefaultModelId { get; set; } = GeminiModel.Gemini1_5Flash;

        private static readonly GeminiSafetySettings[] s_disabledSafetySettings = new GeminiSafetySettings[]
        {
            new()
            {
                Category = GeminiSafetyHarmCategory.HateSpeech,
                Threshold = GeminiSafetyHarmBlockThreshold.Off,
            },
            new()
            {
                Category = GeminiSafetyHarmCategory.SexuallyExplicit,
                Threshold = GeminiSafetyHarmBlockThreshold.Off,
            },
            new()
            {
                Category = GeminiSafetyHarmCategory.DangerousContent,
                Threshold = GeminiSafetyHarmBlockThreshold.Off,
            },
            new()
            {
                Category = GeminiSafetyHarmCategory.Harassment,
                Threshold = GeminiSafetyHarmBlockThreshold.Off,
            },
        };

        /// <inheritdoc/>
        public async Awaitable<ChatInferenceResult> Chat(IReadOnlyList<Message> messages, string model = null, bool tryRemoveFilters = false, CancellationToken token = default)
        {
            Debug.Log("Running chat request through Gemini client.");
            Task<GeminiChatResponse> task = GeminiManager.Instance.Request<GeminiChatResponse>(
                new GeminiChatRequest(!string.IsNullOrEmpty(model) ? model : DefaultModelId, true)
                {
                    Contents = messages.ToGemini(out GeminiContent systemMessage),
                    SystemInstruction = systemMessage,
                    SafetySettings = tryRemoveFilters ? s_disabledSafetySettings : null,
                    GenerationConfig = new GeminiGenerationConfiguration()
                    {
                        CandidateCount = 1,
                    }
                });

            while (!task.IsCompleted)
            {
                token.ThrowIfCancellationRequested();
                await Awaitable.FixedUpdateAsync();
            }

            GeminiChatResponse response = task.Result;
            Debug.Log("Gemini request completed.");

            return new ChatInferenceResult(response.Parts?.Length is null or 0 ? null : new Message[]
            {
                response.Candidates[0].Content.ToGeneric()
            }, response.UsageMetadata.ToGeneric());
        }

        /// <inheritdoc/>
        public async Awaitable<ChatInferenceResult> Chat(IReadOnlyList<Message> messages, IReadOnlyList<Function> tools, string model = null, int maxToolCalls = 10, bool tryRemoveFilters = false, CancellationToken token = default)
        {
            Debug.Log("Running chat request through Gemini client.");
            if (string.IsNullOrEmpty(model))
                model = DefaultModelId;

            GeminiTool[] geminiTools = new GeminiTool[] { tools.ToGemini() };
            Dictionary<string, Function> functionMap = new();

            foreach (Function function in tools)
                functionMap[function.Name] = function;

            List<GeminiContent> history = new(messages.ToGemini(out GeminiContent systemMessage));
            GeminiSafetySettings[] safetySettings = tryRemoveFilters ? s_disabledSafetySettings : null;

            GeminiChatResponse chatResponse;
            int initialHistoryLength = history.Count;
            Usage totalUsage = Usage.Empty;
            int toolCalls = 0;

            do
            {
                GeminiChatRequest request = new(model, true)
                {
                    Contents = history.ToArray(),
                    Tools = geminiTools,
                    SystemInstruction = systemMessage,
                    SafetySettings = safetySettings,
                    GenerationConfig = new GeminiGenerationConfiguration()
                    {
                        CandidateCount = 1,
                    }
                };

                Task<GeminiChatResponse> task = GeminiManager.Instance.Request<GeminiChatResponse>(request);
                while (!task.IsCompleted)
                {
                    token.ThrowIfCancellationRequested();
                    await Awaitable.FixedUpdateAsync();
                }

                chatResponse = task.Result;
                totalUsage += chatResponse.UsageMetadata.ToGeneric();
                if (chatResponse.Parts?.Length is null or 0)
                    break;
                
                history.Add(chatResponse.Candidates[0].Content);

                GeminiContentPart[] allFunctionCalls = Array.FindAll(chatResponse.Parts, part => part?.FunctionCall != null);
                if (allFunctionCalls?.Length is null or 0)
                    break;

                foreach (GeminiContentPart part in allFunctionCalls)
                {
                    token.ThrowIfCancellationRequested();

                    GeminiFunctionCall functionCall = part.FunctionCall;
                    if (!functionMap.TryGetValue(functionCall.Name, out Function function))
                        throw new NullReferenceException($"Unknown function: {functionCall.Name}");

                    history.Add(GeminiContent.GetContent(functionCall.GetResponse(await function.OnInvokeAsync(functionCall.Arguments))));
                }
            } while (toolCalls < maxToolCalls);

            Debug.Log("Gemini request completed.");
            return new ChatInferenceResult(history.GetRange(initialHistoryLength, history.Count - initialHistoryLength).ToGeneric(), totalUsage);
        }
    }
}

#endif
