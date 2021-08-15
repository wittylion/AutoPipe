using System;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Contains pipeline and executes it when needed.
    /// </summary>
    public class PipelineExecutor
    {
        /// <summary>
        /// Message that is used in exception thrown when pipeline is not specified.
        /// </summary>
        public static readonly string PipelineIsNotSetError = "Pipeline object is not specified, pipeline executor requires an instance of pipeline object, which will be executed.";

        /// <summary>
        /// Message that is used in exception thrown when pipeline runner is not specified.
        /// </summary>
        public static readonly string RunnerIsNotSetError = "Pipeline runner is not specified, provide an instance of pipeline runner or use a default constructor.";

        /// <summary>
        /// The pipeline that is hold to be executed later.
        /// </summary>
        public IPipeline Pipeline { get; }

        /// <summary>
        /// The pipeline runner that is hold to be used when <see cref="Run{TContext}(TContext)"/> method is used.
        /// </summary>
        public IPipelineRunner Runner { get; }

        /// <summary>
        /// The constructor that accepts a <paramref name="pipeline"/> hold to be used when <see cref="Run{TContext}(TContext)"/> method is used.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline that is hold to be executed later.
        /// </param>
        public PipelineExecutor(IPipeline pipeline) : this(pipeline, Pipelines.Runner.StaticInstance)
        {
        }

        /// <summary>
        /// The constructor that accepts a <paramref name="pipeline"/> hold to be used when <see cref="Run{TContext}(TContext)"/> method is used
        /// and <see cref="runner"/> that runs a pipeline.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline that is hold to be executed later.
        /// </param>
        /// <param name="runner">
        /// The pipeline runner that is hold to be used when <see cref="Run{TContext}(TContext)"/> method is used.
        /// </param>
        public PipelineExecutor(IPipeline pipeline, IPipelineRunner runner)
        {
            if (pipeline.HasNoValue())
            {
                throw new ArgumentNullException(nameof(pipeline), PipelineIsNotSetError);
            }

            if (runner.HasNoValue())
            {
                throw new ArgumentNullException(nameof(runner), RunnerIsNotSetError);
            }

            Pipeline = pipeline;
            Runner = runner;
        }

        /// <summary>
        /// Executes a method with predefined <see cref="Pipeline"/> and <see cref="Runner"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context that is used during pipeline execution.
        /// </typeparam>
        /// <param name="arguments">
        /// The arguments that are passed to each executed processor in pipeline.
        /// </param>
        /// <returns>
        /// Returns a promise of the executed pipeline.
        /// </returns>
        public async Task Run(object arguments)
        {
            await this.Runner.Run(this.Pipeline, arguments).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes a method with predefined <see cref="Pipeline"/> and <see cref="Runner"/>.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result that is supposed to be returned from the method.
        /// </typeparam>
        /// <param name="arguments">
        /// The arguments that are passed to each executed processor in pipeline.
        /// </param>
        /// <returns>
        /// Returns a promise of the executed pipeline.
        /// </returns>
        public async Task<TResult> Run<TResult>(Bag arguments) where TResult : class
        {
            await this.Runner.Run(this.Pipeline, arguments).ConfigureAwait(false);
            return arguments.GetResultOrThrow<TResult>();
        }
    }
}