using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Runners
{
    public class ObservablePipelineRunner : ObservableConcept<RunningPipelineObservableInformation>, IPipelineRunner
    {
        public IPipelineRunner OriginalRunner { get; }

        public ObservablePipelineRunner() : this(PipelineRunner.StaticInstance)
        {
        }

        public ObservablePipelineRunner(IPipelineRunner originalRunner)
        {
            OriginalRunner = originalRunner;
        }

        public virtual async Task RunPipeline<TArgs>(IPipeline pipeline, TArgs args)
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
                await this.OriginalRunner.RunPipeline(pipeline, args);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }

            this.OnCompleted();
        }
    }
}