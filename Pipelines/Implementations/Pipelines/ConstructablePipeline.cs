using System.Collections.Generic;
using Pipelines.Implementations.Processors;

namespace Pipelines.Implementations.Pipelines
{
    public abstract class ConstructablePipeline : ConstructablePipeline<ProcessorConstructor>
    {
        protected ConstructablePipeline() : this(new ProcessorConstructor())
        {
        }

        protected ConstructablePipeline(ProcessorConstructor constructor) : base(constructor)
        {
        }
    }

    public abstract class ConstructablePipeline<TConstructor> : IPipeline where TConstructor : ProcessorConstructor
    {
        protected TConstructor Constructor { get; }
        
        protected ConstructablePipeline(TConstructor constructor)
        {
            Constructor = constructor;
        }

        public abstract IEnumerable<IProcessor> GetProcessors();
    }
}