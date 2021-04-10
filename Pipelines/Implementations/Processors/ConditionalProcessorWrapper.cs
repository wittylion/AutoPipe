using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor, that wraps an existing processor with conditional function, that defines whether action should be executed.
    /// </summary>
    public class ConditionalProcessorWrapper : ConditionalProcessorConcept
    {
        public static readonly string ProcessorMustBeSpecifiedInGeneric =
            "Creating a generic 'conditional' processor wrapper, that wraps another processor with if statement, you have to provide a processor to wrap.";

        public static readonly string ProcessorMustBeSpecified =
            "Creating a 'conditional' processor wrapper, that wraps another processor with if statement, you have to provide a processor to wrap.";

        public IProcessor Processor { get; }

        public ConditionalProcessorWrapper(Predicate<object> condition, IProcessor processor) : base(condition)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor),
                            ConditionalProcessorWrapper.ProcessorMustBeSpecified);
        }

        public override async Task CustomExecute(object arguments)
        {
            await Processor.Execute(arguments).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Processor, that wraps an existing processor with conditional function, that defines whether action should be executed.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public class ConditionalProcessorWrapper<TArgs> : ConditionalProcessorConcept<TArgs>
    {
        public IProcessor Processor { get; }

        public ConditionalProcessorWrapper(Predicate<TArgs> condition, IProcessor processor) : base(condition)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor),
                            ConditionalProcessorWrapper.ProcessorMustBeSpecifiedInGeneric);
        }

        public override async Task CustomExecute(TArgs arguments)
        {
            await Processor.Execute(arguments).ConfigureAwait(false);
        }
    }

}