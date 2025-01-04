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
using System.ComponentModel;

namespace Uralstech.UAI.Abstraction.Tools
{
    /// <summary>
    /// Declares a parameter for a <see cref="Function"/>.
    /// </summary>
    [JsonObject]
    public record Parameter
    {
        /// <summary>
        /// The name of the parameter.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string Name;

        /// <summary>
        /// The description of the parameter.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string Description;

        /// <summary>
        /// The type of the parameter.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public ParameterType Type;

        /// <summary>
        /// Enum values for the parameter, if the type is <see cref="ParameterType.String"/>.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy), DefaultValueHandling = DefaultValueHandling.Ignore), DefaultValue(null)]
        public string[] Enum = null;

        /// <summary>
        /// Is the parameter required?
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public bool Required = true;

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
