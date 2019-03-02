using System.Collections.Generic;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Contexts
{
    /// <summary>
    /// Context constructor is intended to bring more
    /// self-explanatory code when creating a new pipeline context.
    /// </summary>
    public static class ContextConstructor
    {
        public static ChainingContext<PipelineContext> BuildContext() =>
            BuildContext<PipelineContext>();

        public static ChainingContext<TContext> BuildContext<TContext>() where TContext : PipelineContext, new() =>
            BuildContext(new TContext());

        public static ChainingContext<TContext> BuildContext<TContext>(TContext context) where TContext : PipelineContext =>
            new ChainingContext<TContext>(context);

        public static ChainingContext<QueryContext<TValue>> BuildQueryContext<TValue>() where TValue : class =>
            BuildContext<QueryContext<TValue>>();

        /// <summary>
        /// Creates a new PipelineContext that has a parameter-less constructor.
        /// </summary>
        /// <returns>
        /// A new pipeline context.
        /// </returns>
        public static PipelineContext Create()
        {
            return new PipelineContext();
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with
        /// properties of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TProperties">The type of property container.</typeparam>
        /// <param name="propertyContainer">
        /// Object which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an object passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static PipelineContext Create<TProperties>(TProperties propertyContainer)
        {
            return ContextConstructor.CreateFromProperties(propertyContainer);
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with properties composed from
        /// keys and values of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
        /// <param name="propertyContainer">
        /// Dictionary which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an dictionary passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static PipelineContext Create<TValue>(IDictionary<string, TValue> propertyContainer)
        {
            return ContextConstructor.CreateFromDictionary(propertyContainer);
        }

        /// <summary>
        /// Creates a new PipelineContext or derived type that has
        /// a parameter-less constructor.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the pipeline context that is derived from
        /// <see cref="PipelineContext"/> and has a parameter-less constructor.
        /// </typeparam>
        /// <returns>
        /// A new pipeline context.
        /// </returns>
        public static TContext Create<TContext>() where TContext : PipelineContext, new()
        {
            return new TContext();
        }

        /// <summary>
        /// Creates a new <see cref="TContext"/> with properties composed from
        /// keys and values of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the pipeline context that is derived from
        /// <see cref="PipelineContext"/> and has a parameter-less constructor.
        /// </typeparam>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
        /// <param name="propertyContainer">
        /// Dictionary which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an dictionary passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static TContext Create<TContext, TValue>(IDictionary<string, TValue> propertyContainer) where TContext : PipelineContext, new()
        {
            var context = Create<TContext>();
            if (propertyContainer.HasValue())
            {
                foreach (var item in propertyContainer)
                {
                    context.SetOrAddProperty(item.Key, item.Value);
                }
            }
            return context;
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with
        /// properties of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TProperties">The type of property container.</typeparam>
        /// <param name="propertyContainer">
        /// Object which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an object passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static PipelineContext CreateFromProperties<TProperties>(TProperties propertyContainer)
        {
            return new PipelineContext(propertyContainer);
        }

        /// <summary>
        /// Creates a new <see cref="PipelineContext"/> with properties composed from
        /// keys and values of the object passed in <paramref name="propertyContainer"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
        /// <param name="propertyContainer">
        /// Dictionary which properties will be used in pipeline context when it will be created.
        /// </param>
        /// <returns>
        /// New pipeline context with properties from an dictionary passed in parameter <paramref name="propertyContainer"/>.
        /// </returns>
        public static PipelineContext CreateFromDictionary<TValue>(IDictionary<string, TValue> propertyContainer)
        {
            var context = new PipelineContext();
            if (propertyContainer.HasValue())
            {
                foreach (var item in propertyContainer)
                {
                    context.SetOrAddProperty(item.Key, item.Value);
                }
            }
            return context;
        }
    }
}