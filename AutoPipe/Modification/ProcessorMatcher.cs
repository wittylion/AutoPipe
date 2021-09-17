using System;

namespace AutoPipe.Modifications
{
    /// <summary>
    /// A static helper that helps creating instances of <see cref="IProcessorMatcher"/>.
    /// </summary>
    public static class ProcessorMatcher
    {
        /// <summary>
        /// A matcher that always returns true will be useful when you need to log something
        /// or debug anything right after each processor.
        /// </summary>
        public static readonly IProcessorMatcher True = new DelegateProcessorMatcher(_ => true);

        /// <summary>
        /// A matcher that always returns false.
        /// </summary>
        public static readonly IProcessorMatcher False = new DelegateProcessorMatcher(_ => false);

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
        public static ProcessorMatcherByType ByType<TProcessor>() where TProcessor : IProcessor
        {
            return ByType(typeof(TProcessor));
        }

        /// <summary>
        /// Creates <see cref="ProcessorMatcherByType"/> by passing a <paramref name="type"/> as type
        /// to the constructor.
        /// </summary>
        /// <param name="type">
        /// A type to be compared to types of other processors.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ProcessorMatcherByType"/>.
        /// </returns>
        public static ProcessorMatcherByType ByType(Type type)
        {
            return new ProcessorMatcherByType(type);
        }

        /// <summary>
        /// Creates <see cref="DelegateProcessorMatcher"/> by passing a <paramref name="predicate"/> as a delegate
        /// to the constructor.
        /// </summary>
        /// <param name="predicate">
        /// A predicate that will be used to match a processor.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="DelegateProcessorMatcher"/>.
        /// </returns>
        public static DelegateProcessorMatcher Custom(Predicate<IProcessor> predicate)
        {
            return new DelegateProcessorMatcher(predicate);
        }
    }
}
