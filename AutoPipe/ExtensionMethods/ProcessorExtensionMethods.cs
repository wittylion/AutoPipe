﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoPipe.Modifications;

namespace AutoPipe
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
        public static async Task<Bag> Run(this IProcessor processor, Bag args = null, IProcessorRunner runner = null)
        {
            runner = runner ?? Runner.Instance;
            args = args ?? new Bag();
            await runner.Run(processor, args);
            return args;
        }

        public static Task<Bag> Run(this IProcessor processor, object args, IProcessorRunner runner = null)
        {
            var bag = args.ToBag();
            return processor.Run(bag, runner);
        }

        /// <summary>
        /// Runs a processor with <paramref name="bag"/> context
        /// and <paramref name="runner"/> synchronously, waiting until
        /// all processors of the pipeline will be executed.
        /// </summary>
        /// <param name="processor">
        /// The processor to be executed. It will be executed
        /// by <see cref="IProcessorRunner.Run{TArgs}"/>
        /// method with <paramref name="bag"/> passed.
        /// </param>
        /// <param name="bag">
        /// The context which will be passed to the processor during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the processor.
        /// </param>
        public static Bag RunSync(this IProcessor processor, Bag bag = null, IProcessorRunner runner = null)
        {
            var task = processor.Run(bag, runner);
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }

            return task.Result;
        }

        public static Bag RunSync(this IProcessor processor, object args, IProcessorRunner runner = null)
        {
            var bag = args.ToBag();
            return processor.RunSync(bag, runner);
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

        public static IEnumerable<string> Names(this IProcessor processor)
        {
            return processor.GetType().GetNames();
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
