using Pipelines.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Runners
{
    /// <summary>
    /// Wrapper for pipeline processor, that allow to iterate
    /// through processor and use a passed processor executor
    /// for each of them.
    /// </summary>
    public class CustomizedPipelineRunner : IPipelineRunner
    {
        public static readonly string ProcessorRunnerMustBeSpecified = "Processor runner should have object value, but was null.";
        
        /// <summary>
        /// A runner that is executed for each processor from the pipeline.
        /// </summary>
        public IProcessorRunner ProcessorRunner { get; }

        /// <summary>
        /// Creates a new pipeline runner with a custom <paramref name="processorRunner"/>.
        /// </summary>
        /// <param name="processorRunner">
        /// Processor runner that defines how the processor should be executed.
        /// </param>
        public CustomizedPipelineRunner(IProcessorRunner processorRunner)
        {
            ProcessorRunner = processorRunner ?? throw new ArgumentNullException(nameof(processorRunner), ProcessorRunnerMustBeSpecified);
        }

        /// <summary>
        /// Runs pipeline's processors one by one with a <see cref="ProcessorRunner"/> 
        /// in an order they are returned from <see cref="IPipeline.GetProcessors"/>.
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
        public virtual Task RunPipeline<TArgs>(IPipeline pipeline, TArgs args)
        {
            if (pipeline.HasNoValue())
            {
                return PipelineTask.CompletedTask;
            }

            return RunProcessors(pipeline.GetProcessors(), args);
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
        public virtual async Task RunProcessors<TArgs>(IEnumerable<IProcessor> processors, TArgs args)
        {
            processors = processors.Ensure(Enumerable.Empty<IProcessor>());
            foreach (var processor in processors)
            {
                await ProcessorRunner.RunProcessor(processor, args);
            }
        }
    }
}
