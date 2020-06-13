namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Matcher that contains a method <see cref="Matches(IProcessor)"/>
    /// which compares an instance passed in the constructor and an
    /// instance passed to the method.
    /// </summary>
    public class ProcessorMatcherByInstance : IProcessorMatcher
    {
        /// <summary>
        /// Creates an instance of matcher with a predefined processor
        /// to compare later. This class is probably the only
        /// exception that allows null instance in the constructor
        /// because it might be needed to replace all nulls in pipeline
        /// to something meaningful.
        /// </summary>
        /// <param name="processor">
        /// A processor to be used by <see cref="Matches(IProcessor)"/> method.
        /// </param>
        public ProcessorMatcherByInstance(IProcessor processor)
        {
            Processor = processor;
        }

        /// <summary>
        /// A processor to be used by <see cref="Matches(IProcessor)"/> method.
        /// </summary>
        public IProcessor Processor { get; }

        /// <summary>
        /// Compares a predefined <see cref="Processor"/> to a <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">
        /// A processor to be compared to predefined <see cref="Processor"/>.
        /// </param>
        /// <returns>
        /// Value indicating whether two processors are equal by reference.
        /// </returns>
        public bool Matches(IProcessor processor)
        {
            return processor == Processor;
        }
    }
}
