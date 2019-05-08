﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Wyam.Core.Shortcodes.Html;
using Wyam.Testing;
using Wyam.Testing.Documents;
using Wyam.Testing.Execution;

namespace Wyam.Core.Tests.Shortcodes.Html
{
    [TestFixture]
    public class GistFixture : BaseFixture
    {
        public class ExecuteTests : GistFixture
        {
            [Test]
            public async Task RendersGist()
            {
                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument();
                KeyValuePair<string, string>[] args = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(null, "abc"),
                    new KeyValuePair<string, string>(null, "def"),
                    new KeyValuePair<string, string>(null, "ghi"),
                };
                Gist shortcode = new Gist();

                // When
                IShortcodeResult result = await shortcode.ExecuteAsync(args, null, document, context);

                // Then
                using (TextReader reader = new StreamReader(result.Stream))
                {
                    reader.ReadToEnd().ShouldBe(
                        "<script src=\"//gist.github.com/def/abc.js?file=ghi\" type=\"text/javascript\"></script>");
                }
            }

            [Test]
            public async Task RendersGistWithoutUsername()
            {
                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument();
                KeyValuePair<string, string>[] args = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(null, "abc"),
                    new KeyValuePair<string, string>("File", "ghi"),
                };
                Gist shortcode = new Gist();

                // When
                IShortcodeResult result = await shortcode.ExecuteAsync(args, null, document, context);

                // Then
                using (TextReader reader = new StreamReader(result.Stream))
                {
                    reader.ReadToEnd().ShouldBe(
                        "<script src=\"//gist.github.com/abc.js?file=ghi\" type=\"text/javascript\"></script>");
                }
            }

            [Test]
            public async Task RendersGistWithoutFile()
            {
                TestExecutionContext context = new TestExecutionContext();
                TestDocument document = new TestDocument();
                KeyValuePair<string, string>[] args = new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string, string>(null, "abc"),
                    new KeyValuePair<string, string>(null, "def")
                };
                Gist shortcode = new Gist();

                // When
                IShortcodeResult result = await shortcode.ExecuteAsync(args, null, document, context);

                // Then
                using (TextReader reader = new StreamReader(result.Stream))
                {
                    reader.ReadToEnd().ShouldBe(
                        "<script src=\"//gist.github.com/def/abc.js\" type=\"text/javascript\"></script>");
                }
            }
        }
    }
}
