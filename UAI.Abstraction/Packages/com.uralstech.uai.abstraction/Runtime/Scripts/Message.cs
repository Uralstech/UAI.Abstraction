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
    /// A message in the conversation.
    /// </summary>
    [JsonObject]
    public record Message
    {
        /// <summary>
        /// The role of the creator of the message.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly Role Role;

        /// <summary>
        /// The content of the message.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly string Content;

        /// <param name="role">The role of the creator of the message.</param>
        /// <param name="content">The content of the message.</param>
        [JsonConstructor]
        public Message(Role role, string content)
        {
            Role = role;
            Content = content;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
