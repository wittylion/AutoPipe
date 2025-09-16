using System;

namespace AutoPipe
{
    /// <summary>
    /// Attribute to mark a method parameter as required for pipeline execution.
    /// Use this on parameters of methods executed by <see cref="AutoProcessor"/>.
    /// Ensures that required data is present in the pipeline context before execution.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute
    {
        /// <summary>
        /// If true, the pipeline will stop if the <see cref="Bag"/> does not contain
        /// a property matching the parameter name or the value.
        /// The pipeline will halt and use <see cref="Message"/> as the reason.
        /// </summary>
        public bool Halt { get; set; }

        /// <summary>
        /// The message to add to the <see cref="Bag"/> messages collection if
        /// <see cref="Halt"/> is true and the required property is missing.
        /// </summary>
        public string Message { get; set; }

    }
}
