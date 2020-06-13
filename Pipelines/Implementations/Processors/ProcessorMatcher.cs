using System;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// A static helper that helps creating instances of <see cref="IProcessorMatcher"/>.
    /// </summary>
    public static class ProcessorMatcher
    {
        /// <summary>
        /// Creates <see cref="ProcessorMatcherByInstance"/> by passing an <paramref name="instance"/>
        /// to the constructor.
        /// </summary>
        /// <param name="instance">
        /// A processor to be compared to others.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ProcessorMatcherByInstance"/>.
        /// </returns>
        public static ProcessorMatcherByInstance ByInstance(IProcessor instance)
        {
            return new ProcessorMatcherByInstance(instance);
        }

        /// <summary>
        /// Creates <see cref="ProcessorMatcherByType"/> by passing a <typeparamref name="TProcessor"/> as type
        /// to the constructor.
        /// </summary>
        /// <typeparam name="TProcessor">
        /// A type to be compared to types of other processors.
        /// </typeparam>
        /// <returns>
        /// A new instance of <see cref="ProcessorMatcherByType"/>.
        /// </returns>
        public static ProcessorMatcherByType ByType<TProcessor>() where TProcessor: IProcessor
        {
            return ByType(typeof(TProcessor));
        }

        public static ProcessorMatcherByType ByType(Type type)
        {
            return new ProcessorMatcherByType(type);
        }
    }
}
