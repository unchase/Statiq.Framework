﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.IO;
using Wyam.Common.Meta;
using Wyam.Common.Util;
using Wyam.Core.Modules.Contents;
using Wyam.Testing;
using Wyam.Testing.Documents;
using Wyam.Testing.Execution;

namespace Wyam.Core.Tests.Modules.Contents
{
    [TestFixture]
    [NonParallelizable]
    public class RedirectFixture : BaseFixture
    {
        [Test]
        public async Task SingleRedirect()
        {
            // Given
            IDocument redirected = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("foo.html") } }
            });
            IDocument notRedirected = new TestDocument();
            IExecutionContext context = new TestExecutionContext();
            Redirect redirect = new Redirect();

            // When
            List<IDocument> results = await redirect.ExecuteAsync(new[] { redirected, notRedirected }, context).ToListAsync();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEqual(new[] { "foo.html" }, results.Select(x => x.Get<FilePath>(Keys.WritePath).FullPath));
        }

        [TestCase("foo/bar", "foo/bar.html")]
        [TestCase("foo/bar.html", "foo/bar.html")]
        [TestCase("foo/bar.baz", "foo/bar.baz.html")]
        public async Task AddsExtension(string input, string expected)
        {
            // Given
            IDocument redirected = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath(input) } }
            });
            IDocument notRedirected = new TestDocument();
            IExecutionContext context = new TestExecutionContext();
            Redirect redirect = new Redirect();

            // When
            List<IDocument> results = await redirect.ExecuteAsync(new[] { redirected, notRedirected }, context).ToListAsync();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEqual(new[] { expected }, results.Select(x => x.Get<FilePath>(Keys.WritePath).FullPath));
        }

        [Test]
        public async Task WarnsForAbsoluteRedirectFromPath()
        {
            // Given
            IDocument redirected = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("/foo/bar") } }
            })
            {
                Source = new FilePath("/")
            };
            IDocument notRedirected = new TestDocument();
            IExecutionContext context = new TestExecutionContext();
            Redirect redirect = new Redirect();
            ThrowOnTraceEventType(null);

            // When
            List<IDocument> results = await redirect.ExecuteAsync(new[] { redirected, notRedirected }, context).ToListAsync();  // Make sure to materialize the result list

            // Then
            Assert.IsTrue(Listener.Messages.ToList().Single(x => x.Key == TraceEventType.Warning).Value.StartsWith("The redirect path must be relative"));
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public async Task MultipleRedirects()
        {
            // Given
            IDocument redirected1 = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("foo.html") } }
            });
            IDocument redirected2 = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("bar/baz.html") } }
            });
            IExecutionContext context = new TestExecutionContext();
            Redirect redirect = new Redirect();

            // When
            List<IDocument> results = await redirect.ExecuteAsync(new[] { redirected1, redirected2 }, context).ToListAsync();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { "foo.html", "bar/baz.html" }, results.Select(x => x.Get<FilePath>(Keys.WritePath).FullPath));
        }

        [Test]
        public async Task WithAdditionalOutput()
        {
            // Given
            TestDocument redirected1 = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("foo.html") } },
                { Keys.RelativeFilePath, new FilePath("foo2.html") }
            });
            TestDocument redirected2 = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("bar/baz.html") } }
            });
            IExecutionContext context = new TestExecutionContext();
            Redirect redirect = new Redirect().WithAdditionalOutput(new FilePath("a/b"), x => string.Join("|", x.Select(y => $"{y.Key} {y.Value}")));

            // When
            List<IDocument> results = await redirect.ExecuteAsync(new[] { redirected1, redirected2 }, context).ToListAsync();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { "foo.html", "bar/baz.html", "a/b" }, results.Select(x => x.Get<FilePath>(Keys.WritePath).FullPath));
            Assert.IsTrue(results.Single(x => x.Get<FilePath>(Keys.WritePath).FullPath == "a/b").Content.Contains("foo.html /foo2.html"));
        }

        [Test]
        public async Task WithAdditionalOutputWithoutMetaRefresh()
        {
            // Given
            TestDocument redirected1 = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("foo.html") } },
                { Keys.RelativeFilePath, new FilePath("foo2.html") }
            });
            TestDocument redirected2 = new TestDocument(new MetadataItems
            {
                { Keys.RedirectFrom, new List<FilePath> { new FilePath("bar/baz.html") } }
            });
            IExecutionContext context = new TestExecutionContext();
            Redirect redirect = new Redirect()
                .WithAdditionalOutput(new FilePath("a/b"), x => string.Join("|", x.Select(y => $"{y.Key} {y.Value}")))
                .WithMetaRefreshPages(false);

            // When
            List<IDocument> results = await redirect.ExecuteAsync(new[] { redirected1, redirected2 }, context).ToListAsync();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { "a/b" }, results.Select(x => x.Get<FilePath>(Keys.WritePath).FullPath));
        }
    }
}
