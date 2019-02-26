using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Pipelines.Implementations.Reflection
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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ProcessorOrderAttribute : Attribute
    {
        public int Order { get; }

        public ProcessorOrderAttribute(int order)
        {
            Order = order;
        }
    }
}