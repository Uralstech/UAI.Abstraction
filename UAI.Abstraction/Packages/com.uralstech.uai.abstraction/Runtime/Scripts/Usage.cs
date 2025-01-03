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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Uralstech.UAI.Abstraction
{
    /// <summary>
    /// Represents the token usage of an inference request.
    /// </summary>
    [JsonObject]
    public record Usage
    {
        /// <summary>
        /// The model provider that the usage is associated with.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly string ModelProvider;

        /// <summary>
        /// The number of input tokens used in the inference request.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly int InputTokens;

        /// <summary>
        /// The number of output tokens the inference request generated.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly int OutputTokens;

        /// <summary>
        /// Empty value.
        /// </summary>
        public static readonly Usage Empty = new(string.Empty, 0, 0);

        /// <param name="modelProvider">The model provider that the usage is associated with.</param>
        /// <param name="inputTokens">The number of input tokens used in the inference request.</param>
        /// <param name="outputTokens">The number of output tokens the inference request generated.</param>
        [JsonConstructor]
        public Usage(string modelProvider, int inputTokens, int outputTokens)
        {
            ModelProvider = modelProvider;
            InputTokens = inputTokens;
            OutputTokens = outputTokens;
        }

        /// <summary>
        /// Adds two usage objects together.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the model providers of the objects are different and neither are <see cref="Empty"/>.</exception>
        public static Usage operator +(Usage a, Usage b)
        {
            return a.ModelProvider != b.ModelProvider && !ReferenceEquals(a, Empty) && !ReferenceEquals(b, Empty)
                ? throw new System.InvalidOperationException($"Cannot add together two {nameof(Usage)} objects with different model providers.")
                : new Usage(
                    string.IsNullOrEmpty(b.ModelProvider) ? a.ModelProvider : b.ModelProvider,
                    a.InputTokens + b.InputTokens,
                    a.OutputTokens + b.OutputTokens
                );
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
