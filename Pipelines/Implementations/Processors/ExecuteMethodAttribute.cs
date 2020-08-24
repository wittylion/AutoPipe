using System;

namespace Pipelines.Implementations.Processors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExecuteMethodAttribute : Attribute
    {
        public int Order { get; set; }
    }
}
