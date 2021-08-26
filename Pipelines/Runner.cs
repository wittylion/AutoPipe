﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Runs instances of <see cref="IProcessor"/> and <see cref="IPipeline"/>.
    /// </summary>
    public class Runner : IPipelineRunner, IProcessorRunner
    {
        /// <summary>
        /// Default instance of the <see cref="Runner"/>.
        /// </summary>
        public static readonly Runner Instance = new Runner();

        /// <summary>
        /// The object that is responsible for running single processor in <see cref="Run{TArgs}(IProcessor, TArgs)"/>.
        /// </summary>
        public IProcessorRunner ProcessorsRunner { get; }

        public Runner() : this(processorRunner: ProcessorRunner.Instance)
        {
        }

        public Runner(IProcessorRunner processorRunner)
        {
            if (processorRunner == null)
            {
                throw new ArgumentNullException(nameof(processorRunner), "The value of the processor runner should be specified.");
            }

            ProcessorsRunner = processorRunner;
        }

        /// <summary>
        /// Runs pipeline's processors one by one in an order
        /// they are returned from <see cref="IPipeline.GetProcessors"/>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Type of the arguments used in each processors of the pipeline.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline which processors should be executed.
        /// </param>
        /// <param name="args">
        /// The arguments that has to be passed to each processor
        /// of the executed pipeline.
        /// </param>
        /// <returns>
        /// Returns a promise of the pipeline execution.
        /// </returns>
        public virtual Task Run<TArgs>(IPipeline pipeline, TArgs args)
        {
            if (pipeline.HasNoValue())
            {
                return PipelineTask.CompletedTask;
            }

            return Run(pipeline.GetProcessors(), args);
        }

        /// <summary>
        /// Runs a collection of the processors. If collection is null or empty no processor will be executed.
        /// If any processor of the collection is null it will be skipped.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of arguments that has to be passed to each processor
        /// of the executed collection.
        /// </typeparam>
        /// <param name="processors">
        /// The collection of the processors to be executed.
        /// </param>
        /// <param name="args">
        /// The arguments that has to be passed to each processor
        /// of the executed collection.
        /// </param>
        /// <returns>
        /// Returns a promise of the processors execution.
        /// </returns>
        public virtual async Task Run<TArgs>(IEnumerable<IProcessor> processors, TArgs args)
        {
            processors = processors ?? Enumerable.Empty<IProcessor>();
            foreach (var processor in processors)
            {
                await Run(processor, args).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Runs a processor by executing its <see cref="IProcessor.Run"/> method.
        /// If processor is null it will be skipped.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of arguments that has to be passed to the processor.
        /// </typeparam>
        /// <param name="processor">
        /// The processor to be executed.
        /// </param>
        /// <param name="args">
        /// The arguments that has to be passed to the processor.
        /// </param>
        /// <returns>
        /// Returns a promise of the processor execution.
        /// </returns>
        public virtual async Task Run<TArgs>(IProcessor processor, TArgs args)
        {
            if (processor.HasValue())
            {
                await ProcessorsRunner.Run(processor, args).ConfigureAwait(false);
            }
        }
    }
}