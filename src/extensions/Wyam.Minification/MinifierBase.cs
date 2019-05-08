﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMarkupMin.Core;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.Tracing;
using Wyam.Common.Util;

namespace Wyam.Minification
{
    public abstract class MinifierBase
    {
        public async Task<IEnumerable<IDocument>> MinifyAsync(IReadOnlyList<IDocument> inputs, IExecutionContext context, Func<string, MinificationResultBase> minify, string minifierType)
        {
            return await inputs.SelectAsync(async input =>
            {
                try
                {
                    MinificationResultBase result = minify(input.Content);

                    if (result.Errors.Count > 0)
                    {
                        Trace.Error("{0} errors found while minifying {4} for {1}:{2}{3}", result.Errors.Count, input.SourceString(), Environment.NewLine, string.Join(Environment.NewLine, result.Errors.Select(MinificationErrorInfoToString)), minifierType);
                        return input;
                    }

                    if (result.Warnings.Count > 0)
                    {
                        Trace.Warning("{0} warnings found while minifying {4} for {1}:{2}{3}", result.Warnings.Count, input.SourceString(), Environment.NewLine, string.Join(Environment.NewLine, result.Warnings.Select(MinificationErrorInfoToString)), minifierType);
                    }

                    return await context.GetDocumentAsync(input, source: result.MinifiedContent);
                }
                catch (Exception ex)
                {
                    Trace.Error("Exception while minifying {2} for {0}: {1}", input.SourceString(), ex.Message, minifierType);
                    return input;
                }
            });
        }

        private string MinificationErrorInfoToString(MinificationErrorInfo info) => $"Line {info.LineNumber}, Column {info.ColumnNumber}:{Environment.NewLine}{info.Category} {info.Message}{Environment.NewLine}{info.SourceFragment}";
    }
}