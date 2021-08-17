using System;

namespace Pipelines
{
    [System.AttributeUsage(AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class OrAttribute : Attribute
    {
        public static readonly string DefaultValueShouldNotBeNull = "The default value of the parameter cannot be null.";

        /// <summary>
        /// If <see cref="PipelineContext"/> does not contain a property
        /// called as <see cref="Name"/> or parameter name will use this
        /// value to pass into the method.
        /// </summary>
        public object DefaultValue { get; set; }

        public OrAttribute(object defaultValue)
        {
            if (defaultValue == null)
            {
                throw new ArgumentNullException(nameof(defaultValue), DefaultValueShouldNotBeNull);
            }

            DefaultValue = defaultValue;
        }
    }
}
