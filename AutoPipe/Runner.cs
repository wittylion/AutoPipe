using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoPipe
{
    public delegate void PipelineStarting(PipelineInfo pipelineInfo);
    public delegate void PipelineEnded(PipelineInfo pipelineInfo);
    public delegate void ProcessorStarting(ProcessorInfo pipelineInfo);
    public delegate void ProcessorEnded(ProcessorInfo pipelineInfo);

    /// <summary>
    /// Runs instances of <see cref="IProcessor"/> and <see cref="IPipeline"/>.
    /// </summary>
    public class Runner : IPipelineRunner, IProcessorRunner
    {
        /// <summary>
        /// Default instance of the <see cref="Runner"/>.
        /// </summary>
        public static Runner Instance => instance ?? (instance = new Runner());
        private static Runner instance;

        public Runner(PipelineStarting onPipelineStart = null, ProcessorStarting onProcessorStart = null, ProcessorEnded onProcessorEnd = null, PipelineEnded onPipelineEnd = null)
        {
            if (onPipelineStart != null)
            {
                OnPipelineStart += onPipelineStart;
            }

            if (onProcessorStart != null)
            {
                OnProcessorStart += onProcessorStart;
            }

            if (onProcessorEnd != null)
            {
                OnProcessorEnd += onProcessorEnd;
            }

            if (onPipelineEnd != null)
            {
                OnPipelineEnd += onPipelineEnd;
            }
        }

        public event PipelineStarting OnPipelineStart;
        public event ProcessorStarting OnProcessorStart;
        public event ProcessorEnded OnProcessorEnd;
        public event PipelineEnded OnPipelineEnd;

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
        public virtual async Task Run(IPipeline pipeline, Bag bag)
        {
            if (pipeline.HasNoValue())
            {
                bag.Debug("Cannot run the pipeline because its value is null.");
                return;
            }

            if (bag.Ended)
            {
                bag.Debug($"The bag contained end property set to True. Skipping pipeline: [{pipeline.Name()}].");
                return;
            }

            PipelineInfo pipelineInfo = null;
            if (OnPipelineStart != null || OnPipelineEnd != null)
            {
                pipelineInfo = new PipelineInfo() { Context = bag, Pipeline = pipeline };
            }

            if (OnPipelineStart != null)
            {
                OnPipelineStart(pipelineInfo);
            }

            var processors = pipeline.GetProcessors();

            if (bag.Debug)
            {
                var pipelineName = pipeline.Name();
                var description = pipeline.Description();

                if (description.HasValue())
                {
                    bag.Debug("Running pipeline [{0}]. Pipeline is {1}".FormatWith(pipelineName, description.ToLower()));
                }
                else
                {
                    bag.Debug("Running pipeline [{0}].".FormatWith(pipelineName));
                }

                await Run(processors, bag).ConfigureAwait(false);

                bag.Debug("Completed pipeline [{0}].".FormatWith(pipelineName));
            }
            else
            {
                await Run(processors, bag).ConfigureAwait(false);
            }

            if (OnPipelineEnd != null)
            {
                OnPipelineEnd(pipelineInfo);
            }
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
        public virtual async Task Run(IEnumerable<IProcessor> processors, Bag bag)
        {
            if (bag.Ended)
            {
                bag.Debug("The bag contained end property set to True. Skipping all processors.");
                return;
            }

            int index = 0;
            processors = processors ?? Enumerable.Empty<IProcessor>();
            foreach (var processor in processors)
            {
                bag.Debug("Running processor #{0}.".FormatWith(index));
                await Run(processor, bag).ConfigureAwait(false);

                if (bag.Ended)
                {
                    bag.Debug("Completed processor #{0}. Ending loop.".FormatWith(index));
                    break;
                }
                else
                {
                    bag.Debug("Completed processor #{0}. Continue loop.".FormatWith(index));
                }

                ++index;
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
        public virtual async Task Run(IProcessor processor, Bag bag)
        {
            if (!processor.HasValue())
            {
                bag.Debug("Cannot run the processor because its value is null.");
                return;
            }

            if (bag.Ended)
            {
                bag.Debug("The bag contained end property set to True. Skipping processor.");
                return;
            }

            ProcessorInfo processorInfo = null;
            if (OnPipelineStart != null || OnPipelineEnd != null)
            {
                processorInfo = new ProcessorInfo() { Context = bag, Processor = processor };
            }

            if (OnProcessorStart != null)
            {
                bag.Debug("Running processor's start event.");
                OnProcessorStart(processorInfo);
            }

            if (bag.Debug)
            {
                var processorName = processor.Name();
                var description = processor.Description();

                if (description.HasValue())
                {
                    bag.Debug("Running processor [{0}] that is {2}".FormatWith(processorName, description.ToLower()));
                }
                else
                {
                    bag.Debug("Running processor [{0}].".FormatWith(processorName));
                }

                await processor.Run(bag).ConfigureAwait(false);

                if (bag.Ended)
                {
                    bag.Debug("Processor [{0}] completed with end pipeline signal.".FormatWith(processorName));
                }
                else
                {
                    bag.Debug("Processor [{0}] completed.".FormatWith(processorName));
                }
            }
            else
            {
                await processor.Run(bag).ConfigureAwait(false);
            }

            if (OnProcessorEnd != null)
            {
                bag.Debug("Running processor end event.");
                OnProcessorEnd(processorInfo);
            }
        }
    }
}