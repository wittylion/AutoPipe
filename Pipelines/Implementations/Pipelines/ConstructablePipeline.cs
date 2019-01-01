using System.Collections.Generic;
using Pipelines.Implementations.Processors;

namespace Pipelines.Implementations.Pipelines
{
    /// <summary>
    /// Pipeline that has a constructor of processors inside.
    /// </summary>
    public abstract class ConstructablePipeline : ConstructablePipeline<ProcessorConstructor>
    {
        /// <summary>
        /// Default constructor creates an instance with a <see cref="ProcessorConstructor"/>.
        /// </summary>
        protected ConstructablePipeline() : this(new ProcessorConstructor())
        {
        }

        /// <summary>
        /// Creates an instance with a custom <see cref="ProcessorConstructor"/> or derived class.
        /// </summary>
        protected ConstructablePipeline(ProcessorConstructor constructor) : base(constructor)
        {
        }
    }

    /// <summary>
    /// Pipeline that has a constructor of processors inside.
    /// </summary>
    /// <typeparam name="TConstructor">
    /// Type of processors constructor.
    /// </typeparam>
    public abstract class ConstructablePipeline<TConstructor> : IPipeline where TConstructor : ProcessorConstructor
    {
        /// <summary>
        /// Processors constructor allows creating processors when
        /// using <see cref="GetProcessors"/> method.
        /// </summary>
        protected TConstructor Constructor { get; }

        /// <summary>
        /// Creates an instance with a custom <see cref="TConstructor"/> or derived class.
        /// </summary>
        protected ConstructablePipeline(TConstructor constructor)
        {
            Constructor = constructor;
        }

        /// <summary>
        /// Returns processors in preferable order of execution.
        /// </summary>
        /// <returns>Instances of <see cref="IProcessor"/> class.</returns>
        public abstract IEnumerable<IProcessor> GetProcessors();
    }
}