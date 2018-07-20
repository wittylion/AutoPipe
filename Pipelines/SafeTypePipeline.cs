using System.Collections.Generic;

namespace Pipelines
{
    /// <summary>
    /// Implementation of <see cref="IPipeline"/>
    /// which is intended to handle <see cref="SafeTypeProcessor{TArgs}"/>
    /// with arguments of type <see cref="TArgs"/>.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which has to be handled by each processor of this pipeline.
    /// </typeparam>
    public abstract class SafeTypePipeline<TArgs> : IPipeline
    {
        /// <summary>
        /// Implementation of <see cref="IPipeline.GetProcessors"/> method
        /// which ensures that will be retrieved only processors that can
        /// handle arguments of type <see cref="TArgs"/>.
        /// </summary>
        /// <returns>
        /// Processors that can handle arguments of type <see cref="TArgs"/>.
        /// </returns>
        public IEnumerable<IProcessor> GetProcessors()
        {
            return this.GetProcessorsOfType();
        }

        /// <summary>
        /// Retrieves all processors that can handle arguments of type <see cref="TArgs"/>.
        /// </summary>
        /// <returns>
        /// Enumerable instance of <see cref="SafeTypeProcessor{TArgs}"/>
        /// intended to handle the arguments of type <see cref="TArgs"/>.
        /// </returns>
        public abstract IEnumerable<SafeTypeProcessor<TArgs>> GetProcessorsOfType();
    }
}