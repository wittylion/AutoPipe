using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public abstract class DisposeProcessorConcept : IProcessor
    {
        public Task Execute(object arguments)
        {
            var disposables = GetDisposables(arguments);

            foreach (var disposable in disposables.EnsureAtLeastEmpty())
            {
                if (disposable.HasValue())
                    disposable.Dispose();
            }

            return PipelineTask.CompletedTask;
        }

        public abstract IEnumerable<IDisposable> GetDisposables(object arguments);
    }

    public abstract class DisposeProcessorConcept<TContext> : SafeTypeProcessor<TContext>
    {
        public override Task SafeExecute(TContext arguments)
        {
            var disposables = GetDisposables(arguments);

            foreach (var disposable in disposables.EnsureAtLeastEmpty())
            {
                if (disposable.HasValue())
                    disposable.Dispose();
            }

            return PipelineTask.CompletedTask;
        }

        public abstract IEnumerable<IDisposable> GetDisposables(TContext arguments);
    }
}