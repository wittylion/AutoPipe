using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pipelines.Implementations.Processors;

namespace Pipelines.Implementations.Pipelines
{
    public class NamespaceBasedPipeline : IPipeline
    {
        public string Namespace { get; }

        public NamespaceBasedPipeline(string @namespace)
        {
            Namespace = @namespace;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            return from type in Assembly.GetCallingAssembly().GetTypes()
                where type.Namespace == Namespace && typeof(IProcessor).IsAssignableFrom(type)
                let constructor = type.GetConstructor(Type.EmptyTypes)
                where constructor != null
                let orderAttribute = type.GetCustomAttributes().OfType<ProcessorOrderAttribute>().FirstOrDefault()
                orderby orderAttribute?.Order ?? Int32.MaxValue
                select constructor.Invoke(new object[0]) as IProcessor;
        }
    }
}