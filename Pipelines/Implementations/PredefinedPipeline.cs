using System;
using System.Collections.Generic;
using System.Linq;
using Pipelines.ExtensionMethods;

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

        public static readonly string ProcessorsMustNotBeNull = "Creating a pipeline with predefined processor, be sure to pass a not null list of processors.";

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
}