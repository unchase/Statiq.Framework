﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Wyam.Common.Documents;
using Wyam.Common.Util;
using Wyam.Testing;
using Wyam.Testing.Documents;
using Wyam.Testing.Execution;

namespace Wyam.Minification.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self | ParallelScope.Children)]
    public class MinifyCssFixture : BaseFixture
    {
        public class ExecuteTests : MinifyCssFixture
        {
            [Test]
            public async Task Minify()
            {
                // Given
                // Example taken from http://yui.github.io/yuicompressor/css.html
                const string input = @"
/*****
  Multi-line comment
  before a new class name
*****/
.classname {
    /* comment in declaration block */
    font-weight: normal;
}";
                const string output = ".classname{font-weight:normal}";
                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument(input);
                MinifyCss minifyCss = new MinifyCss();

                // When
                IList<IDocument> results = await minifyCss.ExecuteAsync(new[] { document }, context).ToListAsync();  // Make sure to materialize the result list

                // Then
                results.Single().Content.ShouldBe(output, StringCompareShould.IgnoreLineEndings);
            }
        }
    }
}