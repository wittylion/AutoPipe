﻿using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
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
            await Processor.Execute(arguments);
        }
    }

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
            await Processor.Execute(arguments);
        }
    }

}