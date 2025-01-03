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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using UnityEngine;

namespace Uralstech.UAI.Abstraction.Tools
{
    /// <summary>
    /// A function that can be invoked by a model.
    /// </summary>
    [JsonObject]
    public class Function
    {
        /// <summary>
        /// The name/ID of the function.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly string Name;

        /// <summary>
        /// The description of the function.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly string Description;

        /// <summary>
        /// The parameters of the function.
        /// </summary>
        [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public readonly Parameter[] Parameters;

        /// <summary>
        /// The underlying C# function that will be invoked.
        /// </summary>
        [JsonIgnore]
        public Func<JToken, Awaitable<JObject>> OnInvokeAsync;

        /// <param name="name">The name/ID of the function.</param>
        /// <param name="description">The description of the function.</param>
        /// <param name="parameters">The parameters of the function.</param>
        /// <param name="onInvokeAsync">The underlying C# function that will be invoked.</param>
        public Function(string name, string description, Parameter[] parameters = null, Func<JToken, Awaitable<JObject>> onInvokeAsync = null)
        {
            Name = name;
            Description = description;
            Parameters = parameters;
            OnInvokeAsync = onInvokeAsync;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
