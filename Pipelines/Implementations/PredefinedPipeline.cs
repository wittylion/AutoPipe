using System;
using System.Collections.Generic;

namespace Pipelines.Implementations
{
    public class PredefinedPipeline : IPipeline
    {
        public static IPipeline From(params IProcessor[] processors)
        {
            return new PredefinedPipeline(processors);
        }

        public static IPipeline From(IEnumerable<IProcessor> processors)
        {
            return new PredefinedPipeline(processors);
        }

        public static SafeTypePipeline<T> From<T>(params SafeTypeProcessor<T>[] processors)
        {
            return new PredefinedPipeline<T>(processors);
        }

        public static SafeTypePipeline<T> From<T>(IEnumerable<SafeTypeProcessor<T>> processors)
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

        public IEnumerable<IProcessor> GetProcessors()
        {
            return this.Processors;
        }
    }

    public class PredefinedPipeline<T> : SafeTypePipeline<T>
    {
        public IEnumerable<SafeTypeProcessor<T>> Processors { get; }

        public PredefinedPipeline(IEnumerable<SafeTypeProcessor<T>> processors)
        {
            Processors = processors ?? throw new ArgumentNullException(nameof(processors), PredefinedPipeline.ProcessorsMustNotBeNullForGeneric);
        }

        public override IEnumerable<SafeTypeProcessor<T>> GetProcessorsOfType()
        {
            return this.Processors;
        }
    }
}