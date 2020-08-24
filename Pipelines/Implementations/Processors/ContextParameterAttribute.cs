using System;

namespace Pipelines.Implementations.Processors
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ContextParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Required { get; set; }
    }
}
