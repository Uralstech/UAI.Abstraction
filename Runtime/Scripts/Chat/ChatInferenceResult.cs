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

namespace Uralstech.UAI.Abstraction.Chat
{
    /// <summary>
    /// Represents the response of a chat request.
    /// </summary>
    [JsonObject]
    public record ChatInferenceResult
    {
        /// <summary>
        /// The generated content.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly Message[] Messages;

        /// <summary>
        /// The total token usage of the inference request.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly Usage Usage;

        /// <param name="messages">The generated content.</param>
        /// <param name="usage">The total token usage of the inference request.</param>
        [JsonConstructor]
        public ChatInferenceResult(Message[] messages, Usage usage)
        {
            Messages = messages;
            Usage = usage;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
