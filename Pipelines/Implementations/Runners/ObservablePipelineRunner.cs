using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Runners
{
    /// <summary>
    /// Pipeline runner that uses observers to notify about actions.
    /// </summary>
    public class ObservablePipelineRunner : ObservableConcept<RunningPipelineObservableInformation>, IPipelineRunner
    {
        /// <summary>
        /// The original runner that is being wrapped by this class.
        /// </summary>
        public IPipelineRunner OriginalRunner { get; }

        /// <summary>
        /// Default constructor uses <see cref="PipelineRunner.StaticInstance"/>.
        /// </summary>
        public ObservablePipelineRunner() : this(PipelineRunner.StaticInstance)
        {
        }

        /// <summary>
        /// Constructor that wraps an instance of another pipeline
        /// runner and uses observers.
        /// </summary>
        /// <param name="originalRunner">
        /// Instance of the runner to be wrapped by this class.
        /// </param>
        public ObservablePipelineRunner(IPipelineRunner originalRunner)
        {
            OriginalRunner = originalRunner;
        }

        /// <inheritdoc cref="IPipelineRunner.Run{TArgs}"/>
        public virtual async Task Run<TArgs>(IPipeline pipeline, TArgs args)
        {
            if (this.HasObservers())
            {
                var info = new RunningPipelineObservableInformation()
                {
                    Pipeline = pipeline,
                    Arguments = args
                };
                this.OnNext(info);
            }

            try
            {
                await this.OriginalRunner.Run(pipeline, args).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }

            this.OnCompleted();
        }
    }
}