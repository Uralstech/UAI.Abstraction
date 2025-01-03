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

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Uralstech.UAI.Abstraction.Chat;
using Uralstech.UAI.Abstraction.Tools;

namespace Uralstech.UAI.Abstraction
{
    /// <summary>
    /// A client for interacting with a model.
    /// </summary>
    public interface IModelClient
    {
        /// <summary>
        /// The provider of the model.
        /// </summary>
        public string ModelProvider { get; }

        /// <summary>
        /// The default model to use for inference.
        /// </summary>
        public string DefaultModelId { get; set; }

        /// <summary>
        /// Runs a chat query with the model.
        /// </summary>
        /// <param name="messages">The conversation messages.</param>
        /// <param name="model">The ID of the model to use.</param>
        /// <param name="tryRemoveFilters">Try to remove filters if supported by the model provider.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The model's response.</returns>
        public Awaitable<ChatInferenceResult> Chat(
            IReadOnlyList<Message> messages,
            string model = default,
            bool tryRemoveFilters = false,
            CancellationToken token = default);

        /// <summary>
        /// Runs a chat query with the model, with tools.
        /// </summary>
        /// <param name="messages">The conversation messages.</param>
        /// <param name="tools">The tools the model can use.</param>
        /// <param name="model">The ID of the model to use.</param>
        /// <param name="maxToolCalls">Maximum number of allowed tool calls.</param>
        /// <param name="tryRemoveFilters">Try to remove filters if supported by the model provider.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The model's response.</returns>
        public Awaitable<ChatInferenceResult> Chat(
            IReadOnlyList<Message> messages,
            IReadOnlyList<Function> tools,
            string model = default,
            int maxToolCalls = 10,
            bool tryRemoveFilters = false,
            CancellationToken token = default);
    }
}
