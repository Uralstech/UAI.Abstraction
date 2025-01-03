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
using System.Collections.Generic;
using Uralstech.UAI.Abstraction.Tools;

namespace Uralstech.UAI.Abstraction.Providers.OAI
{
    /// <summary>
    /// Extensions to convert between generic and OpenAI types.
    /// </summary>
    public static class OAIExtensions
    {
        /// <summary>
        /// Converts a generic role to an OpenAI role.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if a generic role has no known OpenAI equivalent.</exception>
        public static OpenAI.Role ToOAI(this Role role)
        {
            return role switch
            {
                Role.System => OpenAI.Role.System,
                Role.User => OpenAI.Role.User,
                Role.Assistant => OpenAI.Role.Assistant,
                Role.ToolResponse => OpenAI.Role.Tool,
                _ => throw new System.NotImplementedException($"Cannot convert generic role to OAI: {role}")
            };
        }

        /// <summary>
        /// Converts a generic message to an OpenAI message.
        /// </summary>
        public static OpenAI.Chat.Message ToOAI(this Message message)
        {
            return new OpenAI.Chat.Message(message.Role.ToOAI(), message.Content);
        }

        /// <summary>
        /// Converts a collection of generic messages to an array of OpenAI messages.
        /// </summary>
        public static OpenAI.Chat.Message[] ToOAI(this IReadOnlyList<Message> messages)
        {
            OpenAI.Chat.Message[] oaiMessages = new OpenAI.Chat.Message[messages.Count];
            for (int i = 0; i < oaiMessages.Length; i++)
                oaiMessages[i] = messages[i].ToOAI();

            return oaiMessages;
        }

        /// <summary>
        /// Converts a generic function to an OpenAI function.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if a generic <see cref="ParameterType"/> has no known OpenAI equivalent.</exception>
        public static OpenAI.Function ToOAI(this Function function)
        {
            JObject parameters = null;
            if (function.Parameters != null)
            {
                JArray required = new();
                JObject properties = new();

                parameters = new()
                {
                    ["type"] = "object",
                    ["properties"] = properties,
                    ["required"] = required,
                };

                foreach (Parameter param in function.Parameters)
                {
                    JObject parameter = new()
                    {
                        ["type"] = param.Type switch
                        {
                            ParameterType.String => "string",
                            ParameterType.Float => "number",
                            ParameterType.Integer => "integer",
                            ParameterType.Boolean => "boolean",
                            _ => throw new System.NotImplementedException($"Cannot convert generic parameter type to OAI: {param.Type}")
                        },

                        ["description"] = param.Description
                    };

                    if (param.Enum != null)
                        parameter["enum"] = new JArray(param.Enum);

                    properties[param.Name] = parameter;

                    if (param.Required)
                        required.Add(param.Name);
                }
            }

            return new OpenAI.Function(function.Name, function.Description, parameters);
        }

        /// <summary>
        /// Converts a collection of generic functions to an array of <see cref="OpenAI.Tool"/>s.
        /// </summary>
        public static OpenAI.Tool[] ToOAI(this IReadOnlyList<Function> functions)
        {
            OpenAI.Tool[] tools = new OpenAI.Tool[functions.Count];
            for (int i = 0; i < tools.Length; i++)
                tools[i] = functions[i].ToOAI();

            return tools;
        }

        /// <summary>
        /// Converts an OpenAI role to a generic role.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Thrown if a OpenAI role has no known generic equivalent.</exception>
        public static Role ToGeneric(this OpenAI.Role role)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return role switch
            {
                OpenAI.Role.System => Role.System,
                OpenAI.Role.User => Role.User,
                OpenAI.Role.Assistant => Role.Assistant,
                OpenAI.Role.Tool or OpenAI.Role.Function => Role.ToolResponse,
                _ => throw new System.NotImplementedException($"Cannot convert OAI role to generic: {role}")
            };
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Converts an OpenAI message to a generic message.
        /// </summary>
        public static Message ToGeneric(this OpenAI.Chat.Message message)
        {
            return new Message(message.Role.ToGeneric(), (string)message.Content);
        }

        /// <summary>
        /// Converts a collection of OpenAI messages to an array of generic messages.
        /// </summary>
        public static Message[] ToGeneric(this IReadOnlyList<OpenAI.Chat.Message> messages)
        {
            Message[] genericMessages = new Message[messages.Count];
            for (int i = 0; i < genericMessages.Length; i++)
                genericMessages[i] = messages[i].ToGeneric();

            return genericMessages;
        }

        /// <summary>
        /// Converts an OpenAI function to a generic function.
        /// </summary>
        public static Usage ToGeneric(this OpenAI.Usage usage)
        {
            return new Usage(OAIModelClient.ModelProvider, usage?.PromptTokens ?? 0, usage?.CompletionTokens ?? 0);
        }
    }
}

#endif
