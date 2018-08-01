using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines.Implementations.Pipelines
{
    /// <summary>
    /// Pipeline with processors specified once in a constructor.
    /// </summary>
    public class PredefinedPipeline : IPipeline
    {
        public static readonly IPipeline Empty = PredefinedPipeline.FromProcessors(Enumerable.Empty<IProcessor>());
        public static SafeTypePipeline<T> GetEmpty<T>() => PredefinedPipeline<T>.Empty;
        public static IPipeline FromProcessors(params IProcessor[] processors)
        {
            return new PredefinedPipeline(processors);
        }

        public static IPipeline FromProcessors(IEnumerable<IProcessor> processors)
        {
            return new PredefinedPipeline(processors);
        }

        public static SafeTypePipeline<T> FromProcessors<T>(params SafeTypeProcessor<T>[] processors)
        {
            return new PredefinedPipeline<T>(processors);
        }

        public static SafeTypePipeline<T> FromProcessors<T>(IEnumerable<SafeTypeProcessor<T>> processors)
        {
            return new PredefinedPipeline<T>(processors);
        }

        public static readonly string ProcessorsMustNotBeNull = "Creating a pipeline with predefined processor, be sure to pass a not null list of processors.";
        public static readonly string ProcessorsMustNotBeNullForGeneric = "Creating a generic pipeline with predefined processor, be sure to pass a not null list of processors.";

        public IEnumerable<IProcessor> Processors { get; }

        public PredefinedPipeline(IEnumerable<IProcessor> processors)
        {
            Processors = processors ?? throw new ArgumentNullException(nameof(processors), ProcessorsMustNotBeNull);
        }

        public virtual IEnumerable<IProcessor> GetProcessors()
        {
            return this.Processors;
        }
    }

    /// <summary>
    /// Pipeline with processors specified once in a constructor.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which has to be handled by each processor of this pipeline.
    /// </typeparam>
    public class PredefinedPipeline<TArgs> : SafeTypePipeline<TArgs>
    {
        public static readonly SafeTypePipeline<TArgs> Empty = PredefinedPipeline.FromProcessors(Enumerable.Empty<SafeTypeProcessor<TArgs>>());
        public IEnumerable<SafeTypeProcessor<TArgs>> Processors { get; }

        public PredefinedPipeline(IEnumerable<SafeTypeProcessor<TArgs>> processors)
        {
            Processors = processors ?? throw new ArgumentNullException(nameof(processors), PredefinedPipeline.ProcessorsMustNotBeNullForGeneric);
        }

        public override IEnumerable<SafeTypeProcessor<TArgs>> GetProcessorsOfType()
        {
            return this.Processors;
        }
    }
}