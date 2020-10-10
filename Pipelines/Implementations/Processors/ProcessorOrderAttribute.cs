using System;

namespace Pipelines.Implementations.Processors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ProcessorOrderAttribute : Attribute
    {
        public int Order { get; }

        public ProcessorOrderAttribute(int order)
        {
            Order = order;
        }
    }
}