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

        public static EnsurePropertyProcessorConcept<TValue> EnsureProperty<TValue>(string name, TValue value)
        {
            return new EnsurePropertyProcessor<TValue>(name, value);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName) where TContext : Bag
        {
            return new ExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(string propertyName,
                Func<TContext, TProperty, Task> action) where TContext : Bag
        {
            return new ExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Action<TContext, TProperty> action,
                string propertyName) where TContext : Bag
        {
            return new ExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(string propertyName,
                Action<TContext, TProperty> action) where TContext : Bag
        {
            return new ExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : Bag
        {
            return new ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>(action, propertyName, abortMessage,
                messageType);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Action<TContext, TProperty> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : Bag
        {
            return new ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>(action, propertyName, abortMessage,
                messageType);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(string propertyName,
                Func<TContext, TProperty, Task> action, string abortMessage, MessageType messageType)
            where TContext : Bag
        {
            return new ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>(action, propertyName, abortMessage,
                messageType);
        }

        public static ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(string propertyName,
                Action<TContext, TProperty> action, string abortMessage, MessageType messageType)
            where TContext : Bag
        {
            return new ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>(action, propertyName, abortMessage,
                messageType);
        }

        public static TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext, TProperty, Exception> exceptionHandler)
            where TContext : Bag
        {
            return new TryExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName,
                exceptionHandler);
        }

        public static TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext, Exception> exceptionHandler) where TContext : Bag
        {
            return new TryExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName,
                exceptionHandler);
        }

        public static TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<Exception> exceptionHandler) where TContext : Bag
        {
            return new TryExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName,
                exceptionHandler);
        }

        public static TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext> exceptionHandler) where TContext : Bag
        {
            return new TryExecuteActionForPropertyProcessor<TContext, TProperty>(action, propertyName,
                exceptionHandler);
        }

        public static ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<Bag, TElement> action, string propertyName)
        {
            return new ExecuteForEachElementInPropertyProcessor<TElement>(action, propertyName);
        }

        public static ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<TElement> action, string propertyName)
        {
            return new ExecuteForEachElementInPropertyProcessor<TElement>(action, propertyName);
        }

        public static ExecuteForEachElementInPropertyProcessorConcept<TContext, TElement>
            ExecuteForEachElementInProperty<TContext, TElement>(
                Action<TContext, TElement> action, string propertyName) where TContext : Bag
        {
            return new ExecuteForEachElementInPropertyProcessor<TContext, TElement>(action, propertyName);
        }

        public static ExecuteForEachElementInPropertyProcessorConcept<TContext, TElement>
            ExecuteForEachElementInProperty<TContext, TElement>(
                Action<TElement> action, string propertyName) where TContext : Bag
        {
            return new ExecuteForEachElementInPropertyProcessor<TContext, TElement>(action, propertyName);
        }

        public static DisposeProcessorConcept<Bag> DisposeProperties(params string[] properties)
        {
            return new DisposePropertiesProcessor(properties);
        }

        public static SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TContext, TProperty, TNewProperty> transformFunction, string transformToProperty)
            where TContext : Bag
        {
            return new TransformPropertyProcessor<TContext, TProperty, TNewProperty>(propertyToTransform,
                transformFunction, transformToProperty);
        }

        public static SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TProperty, TNewProperty> transformFunction, string transformToProperty)
            where TContext : Bag
        {
            return new TransformPropertyProcessor<TContext, TProperty, TNewProperty>(propertyToTransform,
                transformFunction, transformToProperty);
        }

        public static SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform, string transformToProperty,
                Func<TContext, TProperty, TNewProperty> transformFunction) where TContext : Bag
        {
            return new TransformPropertyProcessor<TContext, TProperty, TNewProperty>(propertyToTransform,
                transformToProperty, transformFunction);
        }

        public static SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform, string transformToProperty,
                Func<TProperty, TNewProperty> transformFunction) where TContext : Bag
        {
            return new TransformPropertyProcessor<TContext, TProperty, TNewProperty>(propertyToTransform,
                transformToProperty, transformFunction);
        }

        public static SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TProperty, TNewProperty> transformFunction) where TContext : Bag
        {
            return new TransformPropertyProcessor<TContext, TProperty, TNewProperty>(propertyToTransform,
                transformFunction);
        }

        public static SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TContext, TProperty, TNewProperty> transformFunction) where TContext : Bag
        {
            return new TransformPropertyProcessor<TContext, TProperty, TNewProperty>(propertyToTransform,
                transformFunction);
        }
    }
}