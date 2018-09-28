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

        public virtual EnsurePropertyProcessorConcept<TValue> EnsureProperty<TValue>(string name, TValue value)
        {
            return CommonProcessors.EnsureProperty<TValue>(name, value);
        }

        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForProperty(action, propertyName);
        }

        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Action<TContext, TProperty> action,
                string propertyName) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForProperty(action, propertyName);
        }

        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForPropertyOrAbort(action, propertyName, abortMessage, messageType);
        }

        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Action<TContext, TProperty> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForPropertyOrAbort(action, propertyName, abortMessage, messageType);
        }

        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext, TProperty, Exception> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext, Exception> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<Exception> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        public virtual ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<PipelineContext, TElement> action, string propertyName)
        {
            return CommonProcessors.ExecuteForEachElementInProperty<TElement>(action, propertyName);
        }

        public DisposeProcessorConcept<PipelineContext> DisposeProperties(params string[] properties)
        {
            return CommonProcessors.DisposeProperties(properties);
        }
    }
}