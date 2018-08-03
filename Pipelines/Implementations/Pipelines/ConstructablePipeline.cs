using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pipelines.Implementations.Processors;

namespace Pipelines.Implementations.Pipelines
{
    public abstract class ConstructablePipeline : IPipeline
    {
        public abstract IEnumerable<IProcessor> GetProcessors();

        public virtual ActionProcessor<TArgs> Action<TArgs>(Action<TArgs> action)
        {
            return CommonProcessors.Action<TArgs>(action);
        }

        public virtual ActionProcessor<TArgs> Action<TArgs>(Func<TArgs, Task> action)
        {
            return CommonProcessors.Action<TArgs>(action);
        }

        public virtual EnsurePropertyProcessor<TValue> EnsureProperty<TValue>(string name, TValue value)
        {
            return CommonProcessors.EnsureProperty<TValue>(name, value);
        }

        public virtual ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<PipelineContext, TElement> action, string propertyName)
        {
            return CommonProcessors.ExecuteForEachElementInProperty<TElement>(action, propertyName);
        }
    }
}