using System;
using System.Collections.Generic;
using System.Linq;

namespace Pipelines
{
    /// <summary>
    /// Pipeline with processors specified once in a constructor.
    /// </summary>
    public class Pipeline : IPipeline
    {
        public static readonly IPipeline Empty = Pipeline.From(Enumerable.Empty<IProcessor>());
        public static SafeTypePipeline<T> GetEmpty<T>() => PredefinedPipeline<T>.Empty;
        public static IPipeline From(params IProcessor[] processors)
        {
            return new Pipeline(processors);
        }

        public static IPipeline From(IEnumerable<IProcessor> processors)
        {
            return new Pipeline(processors);
        }

        public static SafeTypePipeline<TArgs> From<TArgs>(params SafeTypeProcessor<TArgs>[] processors)
        {
            return new PredefinedPipeline<TArgs>(processors);
        }

        public static SafeTypePipeline<TArgs> From<TArgs>(IEnumerable<SafeTypeProcessor<TArgs>> processors)
        {
            return new PredefinedPipeline<TArgs>(processors);
        }

        public static IPipeline From<TProcessor>()
            where TProcessor : IProcessor, new()
        {
            return From(new TProcessor());
        }

        public static IPipeline From<TProcessor1, TProcessor2>()
            where TProcessor1 : IProcessor, new()
            where TProcessor2 : IProcessor, new()
        {
            return From(new TProcessor1(), new TProcessor2());
        }

        public static IPipeline From<TProcessor1, TProcessor2, TProcessor3>() 
            where TProcessor1 : IProcessor, new() 
            where TProcessor2 : IProcessor, new()
            where TProcessor3 : IProcessor, new()
        {
            return From(new TProcessor1(), new TProcessor2(), new TProcessor3());
        }

        public static IPipeline From<TProcessor1, TProcessor2, TProcessor3, TProcessor4>()
            where TProcessor1 : IProcessor, new()
            where TProcessor2 : IProcessor, new()
            where TProcessor3 : IProcessor, new()
            where TProcessor4 : IProcessor, new()
        {
            return From(new TProcessor1(), new TProcessor2(), new TProcessor3(), new TProcessor4());
        }

        public static IPipeline From<TProcessor1, TProcessor2, TProcessor3, TProcessor4, TProcessor5>()
            where TProcessor1 : IProcessor, new()
            where TProcessor2 : IProcessor, new()
            where TProcessor3 : IProcessor, new()
            where TProcessor4 : IProcessor, new()
            where TProcessor5 : IProcessor, new()
        {
            return From(new TProcessor1(), new TProcessor2(), new TProcessor3(), new TProcessor4(), new TProcessor5());
        }

        public static readonly string ProcessorsMustNotBeNull = "Creating a pipeline with predefined processor, be sure to pass a not null list of processors.";
        public static readonly string ProcessorsMustNotBeNullForGeneric = "Creating a generic pipeline with predefined processor, be sure to pass a not null list of processors.";

        public IEnumerable<IProcessor> Processors { get; }

        public Pipeline(IEnumerable<IProcessor> processors)
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
        public static readonly SafeTypePipeline<TArgs> Empty = Pipeline.From(Enumerable.Empty<SafeTypeProcessor<TArgs>>());
        public IEnumerable<SafeTypeProcessor<TArgs>> Processors { get; }

        public PredefinedPipeline(IEnumerable<SafeTypeProcessor<TArgs>> processors)
        {
            Processors = processors ?? throw new ArgumentNullException(nameof(processors), Pipeline.ProcessorsMustNotBeNullForGeneric);
        }

        public override IEnumerable<SafeTypeProcessor<TArgs>> GetProcessorsOfType()
        {
            return this.Processors;
        }
    }
}