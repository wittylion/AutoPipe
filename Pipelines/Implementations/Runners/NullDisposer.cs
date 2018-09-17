using System;

namespace Pipelines.Implementations.Runners
{
    internal class NullDisposer : IDisposable
    {
        public static readonly IDisposable Instance = new NullDisposer();

        public void Dispose()
        {
        }
    }
}