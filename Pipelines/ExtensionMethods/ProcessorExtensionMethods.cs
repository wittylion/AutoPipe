using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipelines.Implementations.Processors;

namespace Pipelines
{
    /// <summary>
    /// Processor extension methods intended to simplify
    /// an experience of using processors at the start point.
    /// </summary>
    public static class ProcessorExtensionMethods
    {
        /// <summary>
        /// Wraps the processor into an if statement, returning a new processor.
        /// </summary>
        /// <param name="processor">
        /// The processor to be wrapped with a conditional statement.
        /// </param>
        /// <param name="condition">
        /// The condition that will be checked before executing processor. 
        /// If condition returns <c>true</c> then processor will be executed, otherwise not.
        /// </param>
        /// <returns>
        /// Returns new processor wrapped into an if statement provided by condition argument.
        /// </returns>
        public static IProcessor If(this IProcessor processor, Predicate<object> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new ConditionalProcessorWrapper(condition, processor);
        }

        /// <summary>
        /// Wraps the processor into an if statement, returning a new processor.
        /// </summary>
        /// <param name="processor">
        /// The processor to be wrapped with a conditional statement.
        /// </param>
        /// <param name="condition">
        /// The condition that will be checked before executing processor. 
        /// If condition returns <c>true</c> then processor will be executed, otherwise not.
        /// </param>
        /// <typeparam name="TArgs">Type of arguments which is to be processed by this processor.</typeparam>
        /// <returns>
        /// Returns new processor wrapped into an if statement provided by condition argument.
        /// </returns>
        public static SafeTypeProcessor<TArgs> If<TArgs>(this SafeTypeProcessor<TArgs> processor, Predicate<TArgs> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new ConditionalProcessorWrapper<TArgs>(condition, processor);
        }

        /// <summary>
        /// Wraps the processor into a while statement, returning a new processor.
        /// </summary>
        /// <param name="processor">
        /// The processor to be wrapped with a while loop.
        /// </param>
        /// <param name="condition">
        /// The condition that will be checked before executing processor. 
        /// If condition returns <c>true</c> then processor will be executed, otherwise execution will stop.
        /// </param>
        /// <returns>
        /// Returns new processor wrapped into a while loop statement provided by condition argument.
        /// </returns>
        public static IProcessor While(this IProcessor processor, Predicate<object> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new WhileProcessorWrapper(condition, processor);
        }


        /// <summary>
        /// Wraps the processor into a while statement, returning a new processor.
        /// </summary>
        /// <param name="processor">
        /// The processor to be wrapped with a while loop.
        /// </param>
        /// <param name="condition">
        /// The condition that will be checked before executing processor. 
        /// If condition returns <c>true</c> then processor will be executed, otherwise execution will stop.
        /// </param>
        /// <typeparam name="TArgs">Type of arguments which is to be processed by this processor.</typeparam>
        /// <returns>
        /// Returns new processor wrapped into a while loop statement provided by condition argument.
        /// </returns>
        public static SafeTypeProcessor<TArgs> While<TArgs>(this SafeTypeProcessor<TArgs> processor, Predicate<TArgs> condition)
        {
            if (processor.HasNoValue() || condition.HasNoValue())
                return processor;

            return new WhileProcessorWrapper<TArgs>(condition, processor);
        }

        /// <summary>
        /// Extends processor with another passed <paramref name="action"/>.
        /// </summary>
        /// <param name="processor">The processor to be extended.</param>
        /// <param name="action">The action that should be executed after execution of main action.</param>
        /// <returns>A new processor that is extended with a passed action.</returns>
        public static IProcessor ThenActionOf(this IProcessor processor, Action<object> action)
        {
            if (processor.HasNoValue() || action.HasNoValue())
                return processor;
            
            return new PostActionProcessor(processor, action.ToAsync());
        }

        /// <summary>
        /// Extends processor with another passed <paramref name="action"/>.
        /// </summary>
        /// <param name="processor">The processor to be extended.</param>
        /// <param name="action">The action that should be executed after execution of main action.</param>
        /// <typeparam name="TArgs">Type of arguments which is to be processed by this processor.</typeparam>
        /// <returns>A new processor that is extended with a passed action.</returns>
        public static SafeTypeProcessor<TArgs> ThenActionOf<TArgs>(this SafeTypeProcessor<TArgs> processor, Action<TArgs> action)
        {
            if (processor.HasNoValue() || action.HasNoValue())
                return processor;

            return new PostActionProcessor<TArgs>(processor, action.ToAsync());
        }

        /// <summary>
        /// Extends processor with another action of the passed <paramref name="nextProcessor"/>.
        /// </summary>
        /// <param name="processor">The processor to be extended.</param>
        /// <param name="nextProcessor">
        /// The processor, which action should be executed after execution of main action.
        /// </param>
        /// <returns>
        /// A new processor that is extended with <paramref name="nextProcessor"/> action.
        /// </returns>
        public static IProcessor ThenActionOf(this IProcessor processor, IProcessor nextProcessor)
        {
            if (processor.HasNoValue() || nextProcessor.HasNoValue())
                return processor;

            return new PostProcessorWrapper(processor, nextProcessor);
        }

        /// <summary>
        /// Extends processor with another action of the passed <paramref name="nextProcessor"/>.
        /// </summary>
        /// <param name="processor">The processor to be extended.</param>
        /// <param name="nextProcessor">
        /// The processor, which action should be executed after execution of main action.
        /// </param>
        /// <typeparam name="TArgs">Type of arguments which is to be processed by this processor.</typeparam>
        /// <returns>
        /// A new processor that is extended with <paramref name="nextProcessor"/> action.
        /// </returns>
        public static SafeTypeProcessor<TArgs> ThenActionOf<TArgs>(this SafeTypeProcessor<TArgs> processor, IProcessor nextProcessor)
        {
            if (processor.HasNoValue() || nextProcessor.HasNoValue())
                return processor;

            return new PostProcessorWrapper<TArgs>(processor, nextProcessor);
        }

        /// <summary>
        /// Composes two processors together into one enumerable chain.
        /// </summary>
        /// <param name="processor">
        /// First processor. In case it is not specified, it will not be included in collection.
        /// </param>
        /// <param name="nextProcessor">
        /// Second processor that will be executed after the first one.
        /// In case it is not specified, it will not be included in collection.
        /// </param>
        /// <returns>
        /// Enumerable object of two bound processors.
        /// </returns>
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

        /// <summary>
        /// Composes processor and action processor created from <paramref name="nextAction"/> together into one enumerable chain.
        /// </summary>
        /// <param name="processor">
        /// First processor. In case it is not specified, it will not be included in collection.
        /// </param>
        /// <param name="nextAction">
        /// Action that will be transformed into processor and executed after the first one.
        /// In case it is not specified, it will not be included in collection.
        /// </param>
        /// <returns>
        /// Enumerable object of two bound processors.
        /// </returns>
        public static IEnumerable<IProcessor> ThenProcessor(this IProcessor processor, Action<object> nextAction)
        {
            return processor.ThenProcessor(nextAction.ToProcessor());
        }

        /// <summary>
        /// Composes processor and action processor created from <paramref name="nextAction"/> together into one enumerable chain.
        /// </summary>
        /// <param name="processor">
        /// First processor. In case it is not specified, it will not be included in collection.
        /// </param>
        /// <param name="nextAction">
        /// Action that will be transformed into processor and executed after the first one.
        /// In case it is not specified, it will not be included in collection.
        /// </param>
        /// <returns>
        /// Enumerable object of two bound processors.
        /// </returns>
        public static IEnumerable<IProcessor> ThenProcessor(this IProcessor processor, Action nextAction)
        {
            return processor.ThenProcessor(nextAction.ToProcessor());
        }

        /// <summary>
        /// Composes processor and action processor created from <paramref name="nextAction"/> together into one enumerable chain.
        /// </summary>
        /// <param name="processor">
        /// First processor. In case it is not specified, it will not be included in collection.
        /// </param>
        /// <param name="nextAction">
        /// Action that will be transformed into processor and executed after the first one.
        /// In case it is not specified, it will not be included in collection.
        /// </param>
        /// <returns>
        /// Enumerable object of two bound processors.
        /// </returns>
        public static IEnumerable<IProcessor> ThenProcessor(this IProcessor processor, Func<Task> nextAction)
        {
            return processor.ThenProcessor(nextAction.ToProcessor());
        }

        /// <summary>
        /// Composes processor and action processor created from <paramref name="nextAction"/> together into one enumerable chain.
        /// </summary>
        /// <param name="processor">
        /// First processor. In case it is not specified, it will not be included in collection.
        /// </param>
        /// <param name="nextAction">
        /// Action that will be transformed into processor and executed after the first one.
        /// In case it is not specified, it will not be included in collection.
        /// </param>
        /// <typeparam name="TArgs">Type of arguments which is to be processed by this processor.</typeparam>
        /// <returns>
        /// Enumerable object of two bound processors.
        /// </returns>
        public static IEnumerable<SafeTypeProcessor<TArgs>> ThenProcessor<TArgs>(this SafeTypeProcessor<TArgs> processor, Action<TArgs> nextAction)
        {
            return processor.ThenProcessor(nextAction.ToProcessor());
        }

        /// <summary>
        /// Composes two processors together into one enumerable chain.
        /// </summary>
        /// <param name="processor">
        /// First processor. In case it is not specified, it will not be included in collection.
        /// </param>
        /// <param name="nextProcessor">
        /// Second processor that will be executed after the first one.
        /// In case it is not specified, it will not be included in collection.
        /// </param>
        /// <typeparam name="TArgs">Type of arguments which is to be processed by this processor.</typeparam>
        /// <returns>
        /// Enumerable object of two bound processors.
        /// </returns>
        public static IEnumerable<SafeTypeProcessor<TArgs>> ThenProcessor<TArgs>(this SafeTypeProcessor<TArgs> processor,
            SafeTypeProcessor<TArgs> nextProcessor)
        {
            if (nextProcessor.HasNoValue() && processor.HasNoValue())
            {
                return Enumerable.Empty<SafeTypeProcessor<TArgs>>();
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

        /// <summary>
        /// Runs a processor with <paramref name="args"/> context
        /// and <paramref name="runner"/>.
        /// </summary>
        /// <param name="processor">
        /// The processor to be executed. It will be executed
        /// by <see cref="IProcessorRunner.RunProcessor{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to the processor during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the processor.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static Task Run(this IProcessor processor, object args = null, IProcessorRunner runner = null)
        {
            return runner.Ensure(PipelineRunner.StaticInstance).RunProcessor(processor, args);
        }

        /// <summary>
        /// Runs a processor with <paramref name="args"/> context
        /// and <paramref name="runner"/> synchronously, waiting until
        /// all processors of the pipeline will be executed.
        /// </summary>
        /// <param name="processor">
        /// The processor to be executed. It will be executed
        /// by <see cref="IProcessorRunner.RunProcessor{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to the processor during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the processor.
        /// </param>
        public static void RunSync(this IProcessor processor, object args = null, IProcessorRunner runner = null)
        {
            processor.Run(args, runner).Wait();
        }

        /// <summary>
        /// Creates an instance of <see cref="ProcessorMatcherByInstance"/>
        /// passing there a <paramref name="processor"/>.
        /// </summary>
        /// <param name="processor">
        /// A processor to be matched by the processor matcher.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ProcessorMatcherByInstance"/>
        /// using an original processor to compare.
        /// </returns>
        public static IProcessorMatcher GetMatcher(this IProcessor processor)
        {
            return ProcessorMatcher.ByInstance(processor);
        }
    }
}
