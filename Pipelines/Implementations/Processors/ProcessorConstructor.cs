using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// Constructs instances of <see cref="IProcessor"/>
    /// </summary>
    public class ProcessorConstructor
    {
        /// <summary>
        /// Creates a new instance of <see cref="ActionProcessor"/>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Type of arguments used in <see cref="ActionProcessor"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is passed into instance of <see cref="ActionProcessor"/>.
        /// </param>
        /// <returns>
        /// New instance of <see cref="ActionProcessor"/> with a passed <paramref name="action"/>.
        /// </returns>
        public virtual ActionProcessor<TArgs> Action<TArgs>(Action<TArgs> action)
        {
            return CommonProcessors.Action<TArgs>(action);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ActionProcessor"/>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// Type of arguments used in <see cref="ActionProcessor"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is passed into instance of <see cref="ActionProcessor"/>.
        /// </param>
        /// <returns>
        /// New instance of <see cref="ActionProcessor"/> with a passed <paramref name="action"/>.
        /// </returns>
        public virtual ActionProcessor<TArgs> Action<TArgs>(Func<TArgs, Task> action)
        {
            return CommonProcessors.Action<TArgs>(action);
        }

        /// <summary>
        /// Returns a new instance of <see cref="EnsurePropertyProcessorConcept{TValue}"/>.
        /// </summary>
        /// <typeparam name="TValue">
        /// Type of arguments used in <see cref="EnsurePropertyProcessorConcept{TValue}"/>
        /// </typeparam>
        /// <param name="name">
        /// The property that has to be ensured in <see cref="EnsurePropertyProcessorConcept{TValue}"/>.
        /// </param>
        /// <param name="value">
        /// The value that is used when the property is not set.
        /// </param>
        /// <returns>
        /// New instance of <see cref="EnsurePropertyProcessorConcept{TValue}"/>.
        /// </returns>
        public virtual EnsurePropertyProcessorConcept<TValue> EnsureProperty<TValue>(string name, TValue value)
        {
            return CommonProcessors.EnsureProperty<TValue>(name, value);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ExecuteActionForPropertyProcessorConcept{TContext,TProperty}"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.</param>
        /// <returns>New instance of <see cref="ExecuteActionForPropertyProcessorConcept{TContext,TProperty}"/>.
        /// </returns>
        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForProperty(action, propertyName);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ExecuteActionForPropertyProcessorConcept{TContext,TProperty}"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <returns>
        /// New instance of <see cref="ExecuteActionForPropertyProcessorConcept{TContext,TProperty}"/>.
        /// </returns>
        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForProperty<TContext, TProperty>(Action<TContext, TProperty> action,
                string propertyName) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForProperty(action, propertyName);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ExecuteActionForPropertyOrAbortProcessor"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <param name="abortMessage">
        /// The message that is added to the context when pipeline is aborted.
        /// </param>
        /// <param name="messageType">
        /// The type of the message that is added to the context when pipeline is aborted.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ExecuteActionForPropertyOrAbortProcessor"/>
        /// </returns>
        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForPropertyOrAbort(action, propertyName, abortMessage, messageType);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExecuteActionForPropertyOrAbortProcessor"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <param name="abortMessage">
        /// The message that is added to the context when pipeline is aborted.
        /// </param>
        /// <param name="messageType">
        /// The type of the message that is added to the context when pipeline is aborted.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ExecuteActionForPropertyOrAbortProcessor"/>
        /// </returns>
        public virtual ExecuteActionForPropertyProcessorConcept<TContext, TProperty>
            ExecuteActionForPropertyOrAbort<TContext, TProperty>(Action<TContext, TProperty> action,
                string propertyName, string abortMessage, MessageType messageType) where TContext : PipelineContext
        {
            return CommonProcessors.ExecuteActionForPropertyOrAbort(action, propertyName, abortMessage, messageType);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <param name="exceptionHandler"></param>
        /// <returns>
        /// Returns a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </returns>
        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext, TProperty, Exception> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <param name="exceptionHandler"></param>
        /// <returns>
        /// Returns a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </returns>
        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext, Exception> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <param name="exceptionHandler"></param>
        /// <returns>
        /// Returns a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </returns>
        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<Exception> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be used in <paramref name="action"/>.
        /// </typeparam>
        /// <param name="action">
        /// The action that is executed when property exists.
        /// </param>
        /// <param name="propertyName">
        /// The property name to be checked before action is executed.
        /// </param>
        /// <param name="exceptionHandler"></param>
        /// <returns>
        /// Returns a new instance of the <see cref="TryExecuteActionForPropertyProcessor"/>.
        /// </returns>
        public virtual TryExecuteActionForPropertyProcessor<TContext, TProperty>
            TryExecuteActionForProperty<TContext, TProperty>(Func<TContext, TProperty, Task> action,
                string propertyName, Action<TContext> exceptionHandler) where TContext : PipelineContext
        {
            return CommonProcessors.TryExecuteActionForProperty(action, propertyName, exceptionHandler);
        }

        /// <summary>
        /// Returns new instance of <see cref="ExecuteForEachElementInPropertyProcessor{TElement}"/>.
        /// </summary>
        /// <typeparam name="TElement">
        /// The type of the element used in collection.
        /// </typeparam>
        /// <param name="action">
        /// The action that is used for each element if the property is found.
        /// </param>
        /// <param name="propertyName">
        /// The property that contains collection of <see cref="TElement"/>.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="ExecuteForEachElementInPropertyProcessor{TElement}"/>.
        /// </returns>
        public virtual ExecuteForEachElementInPropertyProcessor<TElement> ExecuteForEachElementInProperty<TElement>(
            Action<PipelineContext, TElement> action, string propertyName)
        {
            return CommonProcessors.ExecuteForEachElementInProperty<TElement>(action, propertyName);
        }

        /// <summary>
        /// Returns new instance of <see cref="DisposeProcessorConcept"/>.
        /// </summary>
        /// <param name="properties">
        /// The properties that should be disposed.
        /// </param>
        /// <returns>
        /// New instance of <see cref="DisposeProcessorConcept"/>.
        /// </returns>
        public DisposeProcessorConcept<PipelineContext> DisposeProperties(params string[] properties)
        {
            return CommonProcessors.DisposeProperties(properties);
        }

        /// <summary>
        /// Returns a new processor that transforms property by taking a value 
        /// of propertry <paramref name="propertyToTransform"/> from <typeparamref name="TContext"/>
        /// and passing it to the <paramref name="transformFunction"/>, which returns a value
        /// that will be written into <paramref name="transformToProperty"/> property in <typeparamref name="TContext"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context, that is derived from <see cref="PipelineContext"/>.
        /// </typeparam>
        /// <typeparam name="TProperty">
        /// The type of the property that is expected to be transformed in <paramref name="transformFunction"/>.
        /// </typeparam>
        /// <typeparam name="TNewProperty">
        /// The type of the property that is expected to be out of <paramref name="transformFunction"/>.
        /// </typeparam>
        /// <param name="propertyToTransform">
        /// A property in the <typeparamref name="TContext"/> which value will be taken to be transformed
        /// by the <paramref name="transformFunction"/>.
        /// </param>
        /// <param name="transformFunction">
        /// A function that takes a value of property <paramref name="propertyToTransform"/>
        /// and returns a new value, that will be written into <paramref name="transformToProperty"/>.
        /// </param>
        /// <param name="transformToProperty">
        /// The name of the new property to be generated in <typeparamref name="TContext"/> 
        /// by taking <paramref name="propertyToTransform"/> and passing it to the
        /// <paramref name="transformFunction"/>.
        /// </param>
        /// <returns>
        /// A new processor that transforms property by taking a value 
        /// of propertry <paramref name="propertyToTransform"/> from <typeparamref name="TContext"/>
        /// and passing it to the <paramref name="transformFunction"/>, which returns a value
        /// that will be written into <paramref name="transformToProperty"/> property in <typeparamref name="TContext"/>.
        /// </returns>
        public SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TContext, TProperty, TNewProperty> transformFunction, string transformToProperty)
            where TContext : PipelineContext
        {
            return CommonProcessors.TransformProperty<TContext, TProperty, TNewProperty>(propertyToTransform, transformFunction, transformToProperty);
        }

        public SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TProperty, TNewProperty> transformFunction, string transformToProperty)
            where TContext : PipelineContext
        {
            return CommonProcessors.TransformProperty<TContext, TProperty, TNewProperty>(propertyToTransform, transformFunction, transformToProperty);
        }

        public SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform, string transformToProperty,
                Func<TContext, TProperty, TNewProperty> transformFunction) where TContext : PipelineContext
        {
            return CommonProcessors.TransformProperty<TContext, TProperty, TNewProperty>(propertyToTransform, transformToProperty, transformFunction);
        }

        public SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform, string transformToProperty,
                Func<TProperty, TNewProperty> transformFunction) where TContext : PipelineContext
        {
            return CommonProcessors.TransformProperty<TContext, TProperty, TNewProperty>(propertyToTransform, transformToProperty, transformFunction);
        }

        public SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TProperty, TNewProperty> transformFunction) where TContext : PipelineContext
        {
            return CommonProcessors.TransformProperty<TContext, TProperty, TNewProperty>(propertyToTransform, transformFunction);
        }

        public SafeProcessor<TContext>
            TransformProperty<TContext, TProperty, TNewProperty>(string propertyToTransform,
                Func<TContext, TProperty, TNewProperty> transformFunction) where TContext : PipelineContext
        {
            return CommonProcessors.TransformProperty<TContext, TProperty, TNewProperty>(propertyToTransform, transformFunction);
        }
    }
}