using Pipelines.Observable;
using System;

namespace Pipelines
{
    public static class RunnerExtensionMethods
    {
        public static ObservablePipelineRunner Observable(this IPipelineRunner runner, Action<PipelineInfo> next = null, Action<Exception, PipelineInfo> error = null, Action<PipelineInfo> completed = null)
        {
            return new ObservablePipelineRunner(runner).On(next, error, completed);
        }

        public static ObservableProcessorRunner Observable(this IProcessorRunner runner, Action<ProcessorInfo> next = null, Action<Exception, ProcessorInfo> error = null, Action<ProcessorInfo> completed = null)
        {
            return new ObservableProcessorRunner(runner).On(next, error, completed);
        }

        public static ObservableProcessorRunner On(this ObservableProcessorRunner runner, Action<ProcessorInfo> next = null, Action<Exception, ProcessorInfo> error = null, Action<ProcessorInfo> completed = null)
        {
            if (next != null || error != null || completed != null) 
            { 
                var observer = new ActionObserver<ProcessorInfo>(next, error, completed);
                runner.Subscribe(observer);
            }
            return runner;
        }

        public static ObservablePipelineRunner On(this ObservablePipelineRunner runner, Action<PipelineInfo> next = null, Action<Exception, PipelineInfo> error = null, Action<PipelineInfo> completed = null)
        {
            if (next != null || error != null || completed != null)
            {
                var observer = new ActionObserver<PipelineInfo>(next, error, completed);
                runner.Subscribe(observer);
            }
            return runner;
        }
    }
}
