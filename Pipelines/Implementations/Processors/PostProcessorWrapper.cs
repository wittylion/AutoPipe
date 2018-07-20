using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
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