﻿using System;
using System.Threading.Tasks;

namespace Pipelines.Observable
{
    /// <summary>
    /// Pipeline runner that uses observers to notify about actions.
    /// </summary>
    public class ObservablePipelineRunner : ObservableConcept<PipelineInfo>, IPipelineRunner
    {
        /// <summary>
        /// The original runner that is being wrapped by this class.
        /// </summary>
        public IPipelineRunner OriginalRunner { get; }

        /// <summary>
        /// Default constructor uses <see cref="Runner.Instance"/>.
        /// </summary>
        public ObservablePipelineRunner() : this(Runner.Instance)
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
            OriginalRunner = originalRunner ?? throw new ArgumentNullException(nameof(originalRunner), "Wrapped pipeline runner cannot be null.");
        }

        /// <inheritdoc cref="IPipelineRunner.Run{TArgs}"/>
        public virtual async Task Run(IPipeline pipeline, object args)
        {
            var info = new PipelineInfo()
            {
                Pipeline = pipeline,
                Context = args
            };

            this.OnNext(info);
            

            try
            {
                await this.OriginalRunner.Run(pipeline, args).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.OnError(exception, info);
            }

            this.OnCompleted(info);
        }
    }
}