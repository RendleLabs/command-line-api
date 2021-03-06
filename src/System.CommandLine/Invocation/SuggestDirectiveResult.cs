﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace System.CommandLine.Invocation
{
    internal class SuggestDirectiveResult : IInvocationResult
    {
        private readonly int _position;

        public SuggestDirectiveResult(int position)
        {
            _position = position;
        }

        public void Apply(InvocationContext context)
        {
            var commandLineToSuggest = context.ParseResult.Tokens.LastOrDefault(t => t.Type != TokenType.Directive)?.Value ?? "";

            var suggestionParseResult = context.Parser.Parse(commandLineToSuggest);

            var suggestions = suggestionParseResult.Suggestions(_position);

            context.Console.Out.WriteLine(
                string.Join(
                    Environment.NewLine,
                    suggestions));
        }
    }
}
