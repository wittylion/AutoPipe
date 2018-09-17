using System;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines
{
    public class PipelineExecutor
    {
        public static readonly string PipelineIsNotSetError = "Pipeline object is not specified, pipeline executor requires an instance of pipeline object, which will be executed.";
        public static readonly string RunnerIsNotSetError = "Pipeline runner is not specified, provide an instance of pipeline runner or use a default constructor.";

        public IPipeline Pipeline { get; }
        public IPipelineRunner Runner { get; }

        public PipelineExecutor(IPipeline pipeline) : this(pipeline, PipelineRunner.StaticInstance)
        {
        }

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

        public async Task Execute(object arguments)
        {
            await this.Runner.RunPipeline(this.Pipeline, arguments);
        }
    }
}