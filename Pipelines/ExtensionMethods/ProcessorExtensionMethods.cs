using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pipelines.Modifications;

namespace Pipelines
{
    /// <summary>
    /// Processor extension methods intended to simplify
    /// an experience of using processors at the start point.
    /// </summary>
    public static class ProcessorExtensionMethods
    {
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
        /// Runs a processor with <paramref name="args"/> context
        /// and <paramref name="runner"/>.
        /// </summary>
        /// <param name="processor">
        /// The processor to be executed. It will be executed
        /// by <see cref="IProcessorRunner.Run{TArgs}"/>
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
        public static Task Run(this IProcessor processor, Bag args = null, IProcessorRunner runner = null)
        {
            runner = runner ?? Runner.Instance;
            args = args ?? new Bag();
            return runner.Run(processor, args);
        }

        public static Task Run(this IProcessor processor, object args, IProcessorRunner runner = null)
        {
            if (!(args is Bag bag))
            {
                bag = new Bag(args);
            }
            return processor.Run(bag, runner);
        }

        public static async Task<TResult> Run<TResult>(this IProcessor processor, Bag args = null, IProcessorRunner runner = null)
        {
            args = args ?? new Bag();
            await processor.Run(args, runner).ConfigureAwait(false);
            return args.ResultOrThrow<TResult>();
        }

        public static async Task<TResult> Run<TResult>(this IProcessor processor, object args, IProcessorRunner runner = null)
        {
            if (!(args is Bag bag))
            {
                bag = new Bag(args);
            }
            await processor.Run(bag, runner).ConfigureAwait(false);
            return bag.ResultOrThrow<TResult>();
        }

        /// <summary>
        /// Runs a processor with <paramref name="args"/> context
        /// and <paramref name="runner"/> synchronously, waiting until
        /// all processors of the pipeline will be executed.
        /// </summary>
        /// <param name="processor">
        /// The processor to be executed. It will be executed
        /// by <see cref="IProcessorRunner.Run{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to the processor during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the processor.
        /// </param>
        public static void RunSync(this IProcessor processor, Bag args = null, IProcessorRunner runner = null)
        {
            processor.Run(args, runner).Wait();
        }

        public static void RunSync(this IProcessor processor, object args, IProcessorRunner runner = null)
        {
            processor.Run(args, runner).Wait();
        }

        public static TResult RunSync<TResult>(this IProcessor processor, Bag args = null, IProcessorRunner runner = null)
        {
            return processor.Run<TResult>(args, runner).Result;
        }

        public static TResult RunSync<TResult>(this IProcessor processor, object args, IProcessorRunner runner = null)
        {
            return processor.Run<TResult>(args, runner).Result;
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

        public static string Name(this IProcessor processor)
        {
            return processor.GetType().GetName();
        }

        public static string Description(this IProcessor processor)
        {
            return processor.GetType().GetDescription();
        }

        public static int Order(this IProcessor processor)
        {
            return processor.GetType().GetOrder();
        }

        public static bool Skip(this IProcessor processor)
        {
            return processor.GetType().ShouldSkip();
        }
    }
}
