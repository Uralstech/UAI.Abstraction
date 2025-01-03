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

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uralstech.UAI.Abstraction.Tools;

namespace Uralstech.UAI.Abstraction.Providers.Gemini
{
    /// <summary>
    /// Extensions to convert between generic and Gemini types.
    /// </summary>
    public static class GeminiExtensions
    {
        /// <summary>
        /// Converts a generic role to an Gemini role. <see cref="Role.System"/> will throw an error as it requires special handling.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if a generic role has no known Gemini equivalent.</exception>
        public static UGemini.Models.Content.GeminiRole ToGemini(this Role role)
        {
            return role switch
            {
                Role.User => UGemini.Models.Content.GeminiRole.User,
                Role.Assistant => UGemini.Models.Content.GeminiRole.Assistant,
                Role.ToolResponse => UGemini.Models.Content.GeminiRole.ToolResponse,
                _ => throw new System.NotImplementedException($"Cannot convert generic role to gemini: {role}")
            };
        }

        /// <summary>
        /// Converts a generic message to an Gemini message. <see cref="Role.System"/> will throw an error as it requires special handling.
        /// </summary>
        public static UGemini.Models.Content.GeminiContent ToGemini(this Message message)
        {
            return UGemini.Models.Content.GeminiContent.GetContent(message.Content, message.Role.ToGemini());
        }

        /// <summary>
        /// Converts a collection of generic messages to an array of Gemini messages.
        /// </summary>
        /// <param name="systemMessage">Special handling for system role.</param>
        /// <exception cref="System.InvalidOperationException">Thrown if more than one system message is found in the messages.</exception>
        public static UGemini.Models.Content.GeminiContent[] ToGemini(this IReadOnlyList<Message> messages, out UGemini.Models.Content.GeminiContent systemMessage)
        {
            List<UGemini.Models.Content.GeminiContent> geminiMessages = new(messages.Count);
            StringBuilder systemMessageBuilder = new();

            for (int i = 0; i < messages.Count; i++)
            {
                Message message = messages[i];
                if (message.Role == Role.System)
                {
                    systemMessageBuilder.Append($" {message.Content}");
                    continue;
                }

                geminiMessages.Add(message.ToGemini());
            }

            systemMessage = systemMessageBuilder.Length > 0
                ? UGemini.Models.Content.GeminiContent.GetContent(systemMessageBuilder.ToString())
                : null;

            return geminiMessages.ToArray();
        }

        /// <summary>
        /// Converts a generic function to an Gemini function.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if a generic <see cref="ParameterType"/> has no known Gemini equivalent.</exception>
        public static UGemini.Models.Generation.Tools.Declaration.GeminiFunctionDeclaration ToGemini(this Function function)
        {
            UGemini.Models.Generation.Schema.GeminiSchema parameters = null;
            if (function.Parameters != null)
            {
                List<string> required = new();
                Dictionary<string, UGemini.Models.Generation.Schema.GeminiSchema> properties = new();

                parameters = new()
                {
                    Type = UGemini.Models.Generation.Schema.GeminiSchemaDataType.Object,
                    Properties = properties,
                };

                foreach (Parameter param in function.Parameters)
                {
                    UGemini.Models.Generation.Schema.GeminiSchema parameter = new()
                    {
                        Type = param.Type switch
                        {
                            ParameterType.String => UGemini.Models.Generation.Schema.GeminiSchemaDataType.String,
                            ParameterType.Float => UGemini.Models.Generation.Schema.GeminiSchemaDataType.Float,
                            ParameterType.Integer => UGemini.Models.Generation.Schema.GeminiSchemaDataType.Integer,
                            ParameterType.Boolean => UGemini.Models.Generation.Schema.GeminiSchemaDataType.Boolean,
                            _ => throw new System.NotImplementedException($"Cannot convert generic parameter type to gemini: {param.Type}")
                        },

                        Description = param.Description,
                        Nullable = !param.Required,
                    };

                    if (param.Enum != null)
                    {
                        parameter.Format = UGemini.Models.Generation.Schema.GeminiSchemaDataFormat.Enum;
                        parameter.Enum = param.Enum;
                    }

                    properties[param.Name] = parameter;

                    if (param.Required)
                        required.Add(param.Name);
                }

                parameters.Required = required.ToArray();
            }

            return new UGemini.Models.Generation.Tools.Declaration.GeminiFunctionDeclaration()
            {
                Name = function.Name,
                Description = function.Description,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Converts a collection of generic functions to an array of <see cref="UGemini.Models.Generation.Tools.Declaration.GeminiTool"/>s.
        /// </summary>
        public static UGemini.Models.Generation.Tools.Declaration.GeminiTool ToGemini(this IReadOnlyList<Function> functions)
        {
            UGemini.Models.Generation.Tools.Declaration.GeminiFunctionDeclaration[] geminiFunctions =
                new UGemini.Models.Generation.Tools.Declaration.GeminiFunctionDeclaration[functions.Count];
            for (int i = 0; i < geminiFunctions.Length; i++)
                geminiFunctions[i] = functions[i].ToGemini();

            return new UGemini.Models.Generation.Tools.Declaration.GeminiTool()
            {
                FunctionDeclarations = geminiFunctions
            };
        }

        /// <summary>
        /// Converts an Gemini role to a generic role.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if a Gemini role has no known generic equivalent.</exception>
        public static Role ToGeneric(this UGemini.Models.Content.GeminiRole role)
        {
            return role switch
            {
                UGemini.Models.Content.GeminiRole.Unspecified => Role.None,
                UGemini.Models.Content.GeminiRole.User => Role.User,
                UGemini.Models.Content.GeminiRole.Assistant => Role.Assistant,
                UGemini.Models.Content.GeminiRole.ToolResponse => Role.ToolResponse,
                _ => throw new System.NotImplementedException($"Cannot convert gemini role to generic: {role}")
            };
        }

        /// <summary>
        /// Converts an Gemini message to a generic message.
        /// </summary>
        public static Message ToGeneric(this UGemini.Models.Content.GeminiContent message)
        {
            return new Message(message.Role.ToGeneric(), string.Join(' ', from part in message.Parts where !string.IsNullOrEmpty(part?.Text) select part.Text));
        }

        /// <summary>
        /// Converts a collection of Gemini messages to an array of generic messages.
        /// </summary>
        public static Message[] ToGeneric(this IReadOnlyList<UGemini.Models.Content.GeminiContent> messages)
        {
            Message[] genericMessages = new Message[messages.Count];
            for (int i = 0; i < genericMessages.Length; i++)
                genericMessages[i] = messages[i].ToGeneric();

            return genericMessages;
        }

        /// <summary>
        /// Converts an Gemini function to a generic function.
        /// </summary>
        public static Usage ToGeneric(this UGemini.Models.Generation.Candidate.GeminiUsageMetadata usage)
        {
            return new Usage(GeminiModelClient.ModelProvider, usage?.PromptTokenCount ?? 0, usage?.CandidatesTokenCount ?? 0);
        }
    }
}

#endif
