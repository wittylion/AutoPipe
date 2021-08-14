using System;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// This attribute is supposed to be used for parameters of
    /// methods marked with <see cref="RunAttribute"/>.
    /// Allows to provide some data before method will be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class ContextParameterAttribute : Attribute
    {
        /// <summary>
        /// The name of the property to be searched 
        /// in <see cref="PipelineContext"/> properties.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// If true method will only be executed if
        /// <see cref="PipelineContext"/> contains a property
        /// called as <see cref="Name"/> or parameter name.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// If true pipeline will abort execution if
        /// <see cref="PipelineContext"/> does not contain 
        /// a property called as <see cref="Name"/> or parameter name.
        /// Will add an <see cref="ErrorMessage"/> as the abort reason.
        /// </summary>
        public bool AbortIfNotExist { get; set; }

        /// <summary>
        /// In case <see cref="AbortIfNotExist"/> true and property is not found,
        /// this message will be added to the messages collection of the <see cref="PipelineContext"/>.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// If <see cref="PipelineContext"/> does not contain a property
        /// called as <see cref="Name"/> or parameter name will use this
        /// value to pass into the method.
        /// </summary>
        public object DefaultValue { get; set; }
    }
}
