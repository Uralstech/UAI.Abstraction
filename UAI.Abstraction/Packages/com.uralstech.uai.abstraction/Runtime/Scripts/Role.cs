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

namespace Uralstech.UAI.Abstraction
{
    /// <summary>
    /// Role of the creator of a message.
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// Default value, ignore.
        /// </summary>
        None,

        /// <summary>
        /// System message.
        /// </summary>
        System,

        /// <summary>
        /// User query.
        /// </summary>
        User,

        /// <summary>
        /// AI response.
        /// </summary>
        Assistant,

        /// <summary>
        /// Tool call.
        /// </summary>
        ToolCall,

        /// <summary>
        /// Tool response.
        /// </summary>
        ToolResponse
    }
}
