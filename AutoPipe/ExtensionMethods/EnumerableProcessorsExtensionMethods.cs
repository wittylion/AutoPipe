﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Extension methods for classes <see cref="IEnumerable{T}"/>
    /// where T is of type <see cref="IProcessor"/>.
    /// </summary>
    public static class EnumerableProcessorsExtensionMethods
    {
        /// <summary>
        /// Creates a pipeline from several processors.
        /// Allows to quickly create a pipeline and avoid
        /// declaring the class of <see cref="IPipeline"/>.
        /// </summary>
        /// <remarks>
        /// In case enumerable object is null, an empty
        /// pipeline will be returned.
        /// </remarks>
        /// <param name="enumerable">
        /// Collection of processors, to be used in pipeline.
        /// </param>
        /// <returns>
        /// Pipeline created from enumerable object of <see cref="IProcessor"/>.
        /// </returns>
        public static IPipeline ToPipeline(this IEnumerable<IProcessor> enumerable)
        {
            if (enumerable.IsNull())
            {
                return Pipeline.Empty;
            }

            return Pipeline.From(enumerable);
        }

        /// <summary>
        /// Repeats processors of the <paramref name="enumerable"/>,
        /// while the condition specified in parameter
        /// <paramref name="condition"/> returns <c>true</c>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="enumerable"/> or <paramref name="condition"/>
        /// is null, returns no objects.
        /// </remarks>
        /// <param name="enumerable">
        /// Collection of processors, to be used in pipeline.
        /// </param>
        /// <param name="condition">
        /// Function, that returns a value indicating whether processors
        /// of the <paramref name="enumerable"/> have to be repeated
        /// and returned one more time.
        /// </param>
        /// <returns>
        /// Enumerable object retutning instances of <see cref="IProcessor"/>
        /// repeated as many times as <paramref name="condition"/> returned <c>true</c>.
        /// </returns>
        public static IEnumerable<IProcessor> RepeatProcessorsWhile(
            this IEnumerable<IProcessor> enumerable, Func<bool> condition)
        {
            if (condition.HasNoValue() || enumerable.HasNoValue())
            {
                yield break;
            }

            while (condition())
            {
                foreach (var processor in enumerable)
                {
                    yield return processor;
                }
            }
        }

        /// <summary>
        /// Runs processors of <paramref name="enumerable"/> one by one
        /// with a default <see cref="Runner"/> instance.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Type of arguments to be passed in each processor.
        /// </typeparam>
        /// <param name="enumerable">
        /// Enumerable object of <see cref="IProcessor"/> to be
        /// executed one by one with a default <see cref="Runner"/>.
        /// </param>
        /// <param name="args">Arguments to be passed in each processor.</param>
        /// <returns>
        /// Returns <see cref="Task"/> object, which indicates a status
        /// of execution of processors.
        /// </returns>
        public static Task Run(this IEnumerable<IProcessor> enumerable, Bag args)
        {
            return enumerable.Run(args, Runner.Instance);
        }

        /// <summary>
        /// Runs processors of <paramref name="enumerable"/> one by one
        /// with a specified <see cref="Runner"/> instance.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Type of arguments to be passed in each processor.
        /// </typeparam>
        /// <param name="enumerable">
        /// Enumerable object of <see cref="IProcessor"/> to be
        /// executed one by one with a specified <see cref="Runner"/>.
        /// </param>
        /// <param name="args">Arguments to be passed in each processor.</param>
        /// <param name="runner">
        /// Pipeline runner instance which will be used
        /// to execute collection of processors.
        /// </param>
        /// <returns>
        /// Returns <see cref="Task"/> object, which indicates a status
        /// of execution of processors.
        /// </returns>
        public static async Task Run(this IEnumerable<IProcessor> enumerable, Bag args, IProcessorRunner runner)
        {
            runner = runner ?? Runner.Instance;
            if (enumerable.HasNoValue())
            {
                return;
            }

            foreach (var processor in enumerable)
            {
                await runner.Run(processor, args).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Method that allows add a processor into <paramref name="enumerable"/> object.
        /// </summary>
        /// <param name="enumerable">
        /// Enumerable object of <see cref="IProcessor"/> where <paramref name="nextProcessor"/>
        /// has to be added.
        /// </param>
        /// <param name="nextProcessor">
        /// A processor to be added into <paramref name="enumerable"/>.
        /// </param>
        /// <returns>
        /// Enumerable object with a concatenated processor.
        /// </returns>
        public static IEnumerable<IProcessor> ThenProcessor(this IEnumerable<IProcessor> enumerable, IProcessor nextProcessor)
        {
            return enumerable.ThenProcessors(new[] {nextProcessor});
        }

        /// <summary>
        /// Method that allows concatenate processors into <paramref name="enumerable"/> object.
        /// </summary>
        /// <param name="enumerable">
        /// Enumerable object of <see cref="IProcessor"/> where <paramref name="nextProcessors"/>
        /// has to be added.
        /// </param>
        /// <param name="nextProcessors">
        /// Processors to be added into <paramref name="enumerable"/>.
        /// </param>
        /// <returns>
        /// Enumerable object with concatenated processors.
        /// </returns>
        public static IEnumerable<IProcessor> ThenProcessors(this IEnumerable<IProcessor> enumerable, IEnumerable<IProcessor> nextProcessors)
        {
            if (enumerable.HasNoValue())
            {
                if (nextProcessors.HasNoValue())
                {
                    return Enumerable.Empty<IProcessor>();
                }

                return nextProcessors;
            }

            if (nextProcessors.HasNoValue())
            {
                return enumerable;
            }

            return enumerable.Concat(nextProcessors);
        }
    }
}