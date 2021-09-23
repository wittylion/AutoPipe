using System.Collections.Generic;

namespace AutoPipe
{
    /// <summary>
    /// Binds together logical processors and itself
    /// represents a complete action instruction,
    /// which defines how processors must be executed.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Returns processors in preferable order of execution.
        /// </summary>
        /// <returns>Instances of <see cref="IProcessor"/> class.</returns>
        IEnumerable<IProcessor> GetProcessors();
    }
}