using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations
{
    class PostProcessor<T> : SafeTypeProcessor<T>
    {
        public IProcessor Processor { get; }
        public IProcessor AdditionalProcessor { get; }

        public PostProcessor(IProcessor processor, Action<T> additionalProcessor)
        {
            Processor = processor;
            AdditionalProcessor = ActionProcessor.From(additionalProcessor);
        }

        public PostProcessor(IProcessor processor, IProcessor additionalProcessor)
        {
            Processor = processor;
            AdditionalProcessor = additionalProcessor;
        }

        public override Task SafeExecute(T arguments)
        {
            Processor.Execute(arguments);
            AdditionalProcessor.Execute(arguments);

            return Task.CompletedTask;
        }
    }

    class PostProcessor : IProcessor
    {
        public IProcessor Processor { get; }
        public IProcessor AdditionalProcessor { get; }

        public PostProcessor(IProcessor processor, Action<object> additionalProcessor)
        {
            Processor = processor;
            AdditionalProcessor = ActionProcessor.From(additionalProcessor);
        }

        public PostProcessor(IProcessor processor, IProcessor additionalProcessor)
        {
            Processor = processor;
            AdditionalProcessor = additionalProcessor;
        }

        public Task Execute(object arguments)
        {
            Processor.Execute(arguments);
            AdditionalProcessor.Execute(arguments);

            return Task.CompletedTask;
        }
    }
}