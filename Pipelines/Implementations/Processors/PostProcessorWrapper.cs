using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Processor that combines two methods together to execute processors one by one.
    /// </summary>
    public class PostProcessorWrapper : PostProcessorConcept
    {
        public IProcessor AdditionalProcessor { get; }

        public PostProcessorWrapper(IProcessor processor, IProcessor additionalProcessor) : base(processor)
        {
            AdditionalProcessor = additionalProcessor ?? throw new ArgumentNullException(nameof(additionalProcessor));
        }

        public override async Task CustomExecute(object arguments)
        {
            await AdditionalProcessor.Execute(arguments);
        }
    }
    /// <summary>
    /// Processor that combines two methods together to execute processors one by one.
    /// </summary>
    /// <typeparam name="TArgs">
    /// Type of arguments which will be processed by this processor.
    /// </typeparam>
    public class PostProcessorWrapper<TArgs> : PostProcessorConcept<TArgs>
    {
        public IProcessor AdditionalProcessor { get; }

        public PostProcessorWrapper(IProcessor processor, IProcessor additionalProcessor) : base(processor)
        {
            AdditionalProcessor = additionalProcessor ?? throw new ArgumentNullException(nameof(additionalProcessor));
        }

        public override async Task CustomExecute(TArgs arguments)
        {
            await AdditionalProcessor.Execute(arguments);
        }
    }
}