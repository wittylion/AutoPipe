using AutoPipe.Observable;

namespace AutoPipe
{
    public static class RunnerExtensionMethods
    {
        public static ObservablePipelineRunner Observable(this IPipelineRunner runner)
        {
            return new ObservablePipelineRunner(runner);
        }

        public static ObservableProcessorRunner Observable(this IProcessorRunner runner)
        {
            return new ObservableProcessorRunner(runner);
        }

        public static ObservableConcept<TInfo> Subscribe<TInfo>(this ObservableConcept<TInfo> runner, IRunnerObserver<TInfo> observer)
        {
            if (runner.HasValue() && observer.HasValue()) 
            {
                runner.Subscribe(observer);
            }
            return runner;
        }
    }
}
