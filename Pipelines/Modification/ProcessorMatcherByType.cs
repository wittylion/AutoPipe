using System;

namespace AutoPipe.Modifications
{
    /// <summary>
    /// Matcher that contains a method <see cref="Matches(IProcessor)"/>
    /// which compares a type passed in the constructor to a type of an
    /// instance passed to the method.
    /// </summary>
    public class ProcessorMatcherByType : IProcessorMatcher
    {
        public static readonly string TypeMustBeSpecified = "You have to specify a type of the processor to match in ProcessorMatcherByType.";

        /// <summary>
        /// Creates an instance of matcher with a predefined type
        /// to compare later.
        /// </summary>
        /// <param name="type">
        /// A type to be used by <see cref="Matches(IProcessor)"/> method.
        /// </param>
        public ProcessorMatcherByType(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type), TypeMustBeSpecified);
        }

        /// <summary>
        /// A type to be used by <see cref="Matches(IProcessor)"/> method.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Compares a predefined <see cref="Type"/> to a <paramref name="processor"/>'s type.
        /// </summary>
        /// <param name="processor">
        /// A processor which type has to be compared to predefined <see cref="Type"/>.
        /// </param>
        /// <returns>
        /// Value indicating whether a type of a passed 
        /// processor is equal by reference to a predefined <see cref="Type"/>.
        /// </returns>
        public bool Matches(IProcessor processor)
        {
            return processor.GetType() == Type;
        }
    }
}
