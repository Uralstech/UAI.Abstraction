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

#if COM_OPENAI_UNITY

using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using ChatInferenceResult = Uralstech.UAI.Abstraction.Chat.ChatInferenceResult;
using Function = Uralstech.UAI.Abstraction.Tools.Function;

namespace Uralstech.UAI.Abstraction.Providers.OAI
{
    /// <summary>
    /// A client for interacting with OpenAI models through <see cref="IModelClient"/>.
    /// </summary>
    public class OAIModelClient : IModelClient
    {
        /// <summary>
        /// Constant for the model provider.
        /// </summary>
        public const string ModelProvider = "openai";

        /// <inheritdoc/>
        string IModelClient.ModelProvider => ModelProvider;

        /// <inheritdoc/>
        public string DefaultModelId { get; set; } = Model.GPT4oMini;

        private readonly OpenAIClient _client = new();

        /// <inheritdoc/>
        public async Awaitable<ChatInferenceResult> Chat(IReadOnlyList<Message> messages, string model = null, bool tryRemoveFilters = false, CancellationToken token = default)
        {
            Debug.Log("Running chat request through OAI client.");
            
            ChatResponse result = await _client.ChatEndpoint.GetCompletionAsync(
                new ChatRequest(messages.ToOAI(), model: !string.IsNullOrEmpty(model) ? model : DefaultModelId, number: 1), token);

            Debug.Log("OAI request completed.");
            return new ChatInferenceResult(new Message[]
                {
                    result.FirstChoice.Message.ToGeneric()
                }, result.Usage.ToGeneric());
        }

        /// <inheritdoc/>
        public async Awaitable<ChatInferenceResult> Chat(IReadOnlyList<Message> messages, IReadOnlyList<Function> tools, string model = null, int maxToolCalls = 10, bool tryRemoveFilters = false, CancellationToken token = default)
        {
            Debug.Log("Running chat request through OAI client.");
            if (string.IsNullOrEmpty(model))
                model = DefaultModelId;

            Tool[] oaiTools = tools.ToOAI();
            Dictionary<string, Function> functionMap = new();
            foreach (Function function in tools)
                functionMap[function.Name] = function;

            List<OpenAI.Chat.Message> history = new(messages.ToOAI());
            int initialHistoryLength = history.Count;
            OpenAI.Usage usage = null;
            ChatResponse result;
            int toolCalls = 0;

            do
            {
                result = await _client.ChatEndpoint.GetCompletionAsync(new ChatRequest(history, model: model, tools: oaiTools, number: 1), token);
                usage = usage == null ? result.Usage : usage + result.Usage;
                history.Add(result.FirstChoice.Message);

                if (result.FirstChoice.FinishReason != "tool_calls")
                    break;

                foreach (ToolCall call in result.FirstChoice.Message.ToolCalls)
                {
                    token.ThrowIfCancellationRequested();
                    if (!call.IsFunction)
                        continue;

                    string functionResponse;
                    if (functionMap?.TryGetValue(call.Function.Name, out Function function) == true)
                        functionResponse = (await function.OnInvokeAsync(JObject.Parse(call.Function.Arguments.ToObject<string>()))).ToString();
                    else
                    {
                        Debug.LogWarning($"Unknown function \"{call.Function.Name}\", using {nameof(call.Function.InvokeAsync)}.");
                        functionResponse = await call.Function.InvokeAsync();
                    }

                    history.Add(new OpenAI.Chat.Message(call, functionResponse));
                    toolCalls++;
                }
            } while (toolCalls < maxToolCalls);

            Debug.Log("OAI request completed.");
            return new ChatInferenceResult(history.GetRange(initialHistoryLength, history.Count - initialHistoryLength).ToGeneric(), usage.ToGeneric());
        }
    }
}

#endif
