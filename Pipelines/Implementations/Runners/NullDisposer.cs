using System;

namespace Pipelines.Implementations.Runners
{
    /// <summary>
    /// An implementation of <see cref="IDisposable"/> interface
    /// that does not do any action.
    /// </summary>
    internal class NullDisposer : IDisposable
    {
        /// <summary>
        /// Static and only instance of a <see cref="NullDisposer"/>.
        /// </summary>
        public static readonly IDisposable Instance = new NullDisposer();

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
        }
    }
}