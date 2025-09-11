using System;

namespace AutoPipe
{
    /// <summary>
    /// This attribute is supposed to be used for parameters of
    /// methods marked with <see cref="RunAttribute"/>.
    /// Allows to provide some data before method will be executed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute
    {
        /// <summary>
        /// If true pipeline will end execution if
        /// <see cref="Bag"/> does not contain 
        /// a property called as <see cref="Name"/> or parameter name.
        /// Will add an <see cref="ErrorMessage"/> as the end reason.
        /// </summary>
        public bool Halt { get; set; }

        /// <summary>
        /// In case <see cref="HaltIfNotExist"/> true and property is not found,
        /// this message will be added to the messages collection of the <see cref="Bag"/>.
        /// </summary>
        public string Message { get; set; }

    }
}
