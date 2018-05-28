using System;
using System.Collections.Generic;

namespace Pipelines.Implementations
{
    public class PipelineOfState<T> : IPipeline
    {
        public T State { get; }
        public Func<T, IEnumerable<IProcessor>> ProcessorsRetriever { get; }

        public PipelineOfState(T state, Func<T, IEnumerable<IProcessor>> processorsRetriever)
        {
            State = state;
            ProcessorsRetriever = processorsRetriever;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            return this.ProcessorsRetriever(State);
        }
    }
}