using System;

namespace Pipelines
{
    [System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class OrAttribute : Attribute
    {
        /// <summary>
        /// If <see cref="PipelineContext"/> does not contain a property
        /// called as <see cref="Name"/> or parameter name will use this
        /// value to pass into the method.
        /// </summary>
        public object DefaultValue { get; set; }

        public OrAttribute(object defaultValue)
        {
            DefaultValue = defaultValue;
        }
    }
}
