using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public class WhileProcessorWrapper : WhileProcessorConcept
    {
        public static readonly string ProcessorMustBeSpecifiedInGeneric =
            "Creating a generic 'while' processor wrapper, that wraps another processor with while statement, you have to provide a processor to wrap.";

        public static readonly string ProcessorMustBeSpecified =
            "Creating a 'while' processor wrapper, that wraps another processor with while statement, you have to provide a processor to wrap.";

        public IProcessor Processor { get; }

        public WhileProcessorWrapper(Predicate<object> condition, IProcessor processor) : base(condition)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor),
                            WhileProcessorWrapper.ProcessorMustBeSpecified);
        }

        public override async Task CustomExecute(object arguments)
        {
            await Processor.Execute(arguments);
        }
    }

    public class WhileProcessorWrapper<T> : WhileProcessorConcept<T>
    {
        public IProcessor Processor { get; }

        public WhileProcessorWrapper(Predicate<T> condition, IProcessor processor) : base(condition)
        {
            Processor = processor ?? throw new ArgumentNullException(nameof(processor),
                            WhileProcessorWrapper.ProcessorMustBeSpecifiedInGeneric);
        }

        public override async Task CustomExecute(T arguments)
        {
            await Processor.Execute(arguments);
        }
    }

}