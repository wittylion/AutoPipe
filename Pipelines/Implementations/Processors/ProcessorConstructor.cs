using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public class ProcessorConstructor
    {
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

        public virtual ExecuteActionForPropertyProcessor<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForProperty(action, propertyName);
        }

        public virtual ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForPropertyOrAbort(action, propertyName, abortMessage, messageType);
        }

        public virtual ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<PipelineContext, TElement> action, string propertyName)
        {
            return CommonProcessors.ExecuteForEachElementInProperty<TElement>(action, propertyName);
        }
    }
}