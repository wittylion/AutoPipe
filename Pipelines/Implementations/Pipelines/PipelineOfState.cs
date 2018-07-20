using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
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

    public class PipelineOfState<TStateObject> : IPipeline
    {
        public TStateObject State { get; }
        public Func<TStateObject, IEnumerable<IProcessor>> ProcessorsRetriever { get; }

        public PipelineOfState(TStateObject state, Func<TStateObject, IEnumerable<IProcessor>> processorsRetriever)
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