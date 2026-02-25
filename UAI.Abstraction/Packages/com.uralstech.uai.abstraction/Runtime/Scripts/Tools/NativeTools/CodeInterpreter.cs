// Copyright 2025 URAV ADVANCED LEARNING SYSTEMS PRIVATE LIMITED
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

namespace Uralstech.UAI.Abstraction.Tools
{
    /// <summary>
    /// A code interpreter tool.
    /// </summary>
    public class CodeInterpreter : INativeTool
    {
        /// <inheritdoc/>
        public ToolType Type => ToolType.NativeTool;

        /// <inheritdoc/>
        public Function Fallback { get; }

        public CodeInterpreter()
        {
        }

        /// <param name="fallback"> Fallback function if this native tool is not supported by the service.</param>
        public CodeInterpreter(Function fallback)
        {
            Fallback = fallback;
        }
    }
}