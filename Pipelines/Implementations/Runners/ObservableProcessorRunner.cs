using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Runners
{
    public class ObservableProcessorRunner : ObservableConcept<RunningProcessorObservableInformation>, IProcessorRunner
    {
        public static readonly string OriginalRunnerIsNull = "The original processor runner is not specified. Observable processor must wrap an instance of processor runner, otherwise use a default constructor.";
        public IProcessorRunner OriginalRunner { get; }

        public ObservableProcessorRunner() : this(PipelineRunner.StaticInstance)
        {
        }

        public ObservableProcessorRunner(IProcessorRunner originalRunner)
        {
            OriginalRunner = originalRunner ?? throw new ArgumentNullException(ObservableProcessorRunner.OriginalRunnerIsNull);
        }

        public virtual async Task RunProcessor<TArgs>(IProcessor processor, TArgs args)
        {
            if (this.HasObservers())
            {
                var info = new RunningProcessorObservableInformation()
                {
                    Processor = processor,
                    Arguments = args
                };
                this.OnNext(info);
            }

            try
            {
                await this.OriginalRunner.RunProcessor(processor, args);
            }
            catch (Exception exception)
            {
                this.OnError(exception);
            }

            this.OnCompleted();
        }
    }
}