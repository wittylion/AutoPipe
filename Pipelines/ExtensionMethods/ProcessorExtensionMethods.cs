using System;
using System.Collections.Generic;
using System.Linq;
using Pipelines.Implementations;

namespace Pipelines.ExtensionMethods
{
    public static class ProcessorExtensionMethods
    {
        public static IProcessor If(this IProcessor processor, Predicate<object> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new ConditionalProcessorWrapper(condition, processor);
        }

        public static SafeTypeProcessor<T> If<T>(this SafeTypeProcessor<T> processor, Predicate<T> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new ConditionalProcessorWrapper<T>(condition, processor);
        }

        public static IProcessor While(this IProcessor processor, Predicate<object> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new WhileProcessorWrapper(condition, processor);
        }

        public static SafeTypeProcessor<T> While<T>(this SafeTypeProcessor<T> processor, Predicate<T> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new WhileProcessorWrapper<T>(condition, processor);
        }

        public static IProcessor ThenActionOf(this IProcessor processor, Action<object> action)
        {
            if (processor.HasNoValue() || action.HasNoValue())
                return processor;
            
            return new PostActionProcessor(processor, action.ToAsync());
        }

        public static SafeTypeProcessor<T> ThenActionOf<T>(this SafeTypeProcessor<T> processor, Action<T> action)
        {
            if (processor.HasNoValue() || action.HasNoValue())
                return processor;

            return new PostActionProcessor<T>(processor, action.ToAsync());
        }

        public static IProcessor ThenActionOf(this IProcessor processor, IProcessor nextProcessor)
        {
            if (processor.HasNoValue() || nextProcessor.HasNoValue())
                return processor;

            return new PostProcessorWrapper(processor, nextProcessor);
        }

        public static SafeTypeProcessor<T> ThenActionOf<T>(this SafeTypeProcessor<T> processor, IProcessor nexProcessor)
        {
            if (processor.HasNoValue() || nexProcessor.HasNoValue())
                return processor;

            return new PostProcessorWrapper<T>(processor, nexProcessor);
        }

        public static IEnumerable<IProcessor> ThenProcessor(this IProcessor processor, IProcessor nextProcessor)
        {
            if (nextProcessor.HasNoValue() && processor.HasNoValue())
            {
                return Enumerable.Empty<IProcessor>();
            }

            if (nextProcessor.HasNoValue() && processor.HasValue())
            {
                return processor.ToAnArray();
            }

            if (nextProcessor.HasValue() && processor.HasNoValue())
            {
                return nextProcessor.ToAnArray();
            }

            return new[]
            {
                processor,
                nextProcessor
            };
        }
        
        public static IEnumerable<IProcessor> ThenProcessor(this IProcessor processor, Action<object> nextAction)
        {
            return processor.ThenProcessor(nextAction.ToProcessor());
        }

        public static IEnumerable<SafeTypeProcessor<T>> ThenProcessor<T>(this SafeTypeProcessor<T> processor, Action<T> nextAction)
        {
            return processor.ThenProcessor(nextAction.ToProcessor());
        }

        public static IEnumerable<SafeTypeProcessor<T>> ThenProcessor<T>(this SafeTypeProcessor<T> processor,
            SafeTypeProcessor<T> nextProcessor)
        {
            if (nextProcessor.HasNoValue() && processor.HasNoValue())
            {
                return Enumerable.Empty<SafeTypeProcessor<T>>();
            }

            if (nextProcessor.HasNoValue() && processor.HasValue())
            {
                return processor.ToAnArray();
            }

            if (nextProcessor.HasValue() && processor.HasNoValue())
            {
                return nextProcessor.ToAnArray();
            }

            return new[]
            {
                processor,
                nextProcessor
            };
        }
    }
}
