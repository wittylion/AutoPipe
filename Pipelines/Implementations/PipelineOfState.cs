using System;
using System.Collections.Generic;

namespace Pipelines.Implementations
{
    public class PipelineOfState : IPipeline
    {
        public static IPipeline From<T>(T state, Func<T, IEnumerable<IProcessor>> processorsRetriever)
        {
            return new PipelineOfState<T>(state, processorsRetriever);
        }

        public static IPipeline From(object state, Func<object, IEnumerable<IProcessor>> processorsRetriever)
        {
            return new PipelineOfState(state, processorsRetriever);
        }

        public object State { get; }
        public Func<object, IEnumerable<IProcessor>> ProcessorsRetriever { get; }

        public PipelineOfState(object state, Func<object, IEnumerable<IProcessor>> processorsRetriever)
        {
            State = state;
            ProcessorsRetriever = processorsRetriever ?? throw new ArgumentNullException(nameof(processorsRetriever));
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            return this.ProcessorsRetriever(State);
        }
    }

    public class PipelineOfState<T> : IPipeline
    {
        public T State { get; }
        public Func<T, IEnumerable<IProcessor>> ProcessorsRetriever { get; }

        public PipelineOfState(T state, Func<T, IEnumerable<IProcessor>> processorsRetriever)
        {
            State = state;
            ProcessorsRetriever = processorsRetriever ?? throw new ArgumentNullException(nameof(processorsRetriever));
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            return this.ProcessorsRetriever(State);
        }
    }
}