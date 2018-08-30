using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public static class CommonProcessors
    {
        public static ActionProcessor<TArgs> Action<TArgs>(Action<TArgs> action)
        {
            return ActionProcessor.FromAction<TArgs>(action);
        }

        public static ActionProcessor<TArgs> Action<TArgs>(Func<TArgs, Task> action)
        {
            return ActionProcessor.FromAction<TArgs>(action);
        }

        public static EnsurePropertyProcessor<TValue> EnsureProperty<TValue>(string name, TValue value)
        {
            return new EnsurePropertyProcessor<TValue>(name, value);
        }

        public static ExecuteActionForPropertyProcessor<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName) where TContext : PipelineContext
        {
            return new ExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName);
        }

        public static ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : PipelineContext
        {
            return new ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>(action, propertyName, abortMessage, messageType);
        }

        public static ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<PipelineContext, TElement> action, string propertyName)
        {
            return new ExecuteForEachElementInPropertyProcessor<TElement>(action, propertyName);
        }

        public static ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<TElement> action, string propertyName)
        {
            return new ExecuteForEachElementInPropertyProcessor<TElement>(action, propertyName);
        }

        public static ExecuteForEachElementInPropertyProcessor<TContext, TElement> ExecuteForEachElementInProperty<TContext, TElement>(
            Action<TContext, TElement> action, string propertyName) where TContext : PipelineContext
        {
            return new ExecuteForEachElementInPropertyProcessor<TContext, TElement>(action, propertyName);
        }

        public static ExecuteForEachElementInPropertyProcessor<TContext, TElement> ExecuteForEachElementInProperty<TContext, TElement>(
            Action<TElement> action, string propertyName) where TContext : PipelineContext
        {
            return new ExecuteForEachElementInPropertyProcessor<TContext, TElement>(action, propertyName);
        }
    }
}