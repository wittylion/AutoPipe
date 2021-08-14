using System;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Marks a method to be executed within an <see cref="AutoProcessor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RunAttribute : Attribute
    {
        /// <summary>
        /// Specifies an order of execution among other methods.
        /// </summary>
        public int Order { get; set; }
    }
}
