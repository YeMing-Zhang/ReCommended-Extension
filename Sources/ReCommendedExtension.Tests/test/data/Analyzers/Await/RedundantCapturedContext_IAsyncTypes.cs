﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReCommendedExtension.Tests.test.data.Analyzers.Await
{
    public class RedundantCapturedContext
    {
        class Disposable : IAsyncDisposable
        {
            public async ValueTask DisposeAsync() { }
        }

        class AsyncEnumerable : IAsyncEnumerable<int>
        {
            public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }

        public async ValueTask Dispose()
        {
            await using (new Disposable()) { }
        }

        public async ValueTask Dispose_ConfigureAwait()
        {
            await using (new Disposable().ConfigureAwait(false)) { }
        }

        public async ValueTask Dispose2()
        {
            await using var x = new Disposable();
        }

        public async ValueTask Dispose2_ConfigureAwait()
        {
            await using var x = new Disposable().ConfigureAwait(false);
        }

        public async ValueTask Iterator()
        {
            await foreach (var item in new AsyncEnumerable()) { }
        }

        public async ValueTask Iterator_ConfigureAwait()
        {
            await foreach (var item in new AsyncEnumerable().ConfigureAwait(false)) { }
        }
    }
}