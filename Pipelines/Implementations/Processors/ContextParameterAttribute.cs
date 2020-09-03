using System;

namespace Pipelines.Implementations.Processors
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ContextParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Required { get; set; }
        public bool AbortIfNotExist { get; set; }
        public string ErrorMessage { get; set; }
        public object DefaultValue { get; set; }
    }
}
