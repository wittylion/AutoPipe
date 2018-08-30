using System.Collections.Generic;
using Pipelines.Implementations.Processors;

namespace Pipelines.Implementations.Pipelines
{
    public abstract class ConstructablePipeline : IPipeline
    {
        protected ProcessorConstructor Constructor { get; }

        protected ConstructablePipeline() : this(new ProcessorConstructor())
        {
        }

        protected ConstructablePipeline(ProcessorConstructor constructor)
        {
            Constructor = constructor;
        }

        public abstract IEnumerable<IProcessor> GetProcessors();
    }
}