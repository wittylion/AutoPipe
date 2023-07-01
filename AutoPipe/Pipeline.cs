using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoPipe
{
    /// <summary>
    /// Pipeline with processors specified once in a constructor.
    /// </summary>
    public class Pipeline : IPipeline
    {
        public static readonly IPipeline Empty = Pipeline.From(Enumerable.Empty<IProcessor>());
        public static IPipeline From(params IProcessor[] processors)
        {
            return new Pipeline(processors);
        }

        public static NamespacePipeline FromNamespace(string @namespace, bool recursive = true, bool includeSkipped = false, IServiceProvider serviceProvider = null)
        {
            return NamespacePipeline.From(@namespace: @namespace, recursive: recursive, includeSkipped: includeSkipped, serviceProvider: serviceProvider);
        }

        public static IPipeline From(IEnumerable<IProcessor> processors)
        {
            return new Pipeline(processors);
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
}