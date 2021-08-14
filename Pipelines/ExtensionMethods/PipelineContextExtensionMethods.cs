using System;
using System.Threading.Tasks;

namespace Pipelines
{
    /// <summary>
    /// Extensions for the <see cref="PipelineContext"/>.
    /// </summary>
    public static class PipelineContextExtensionMethods
    {
        public static void TransformProperty<TContext, TValue, TNewValue>(this TContext context, string property,
            Func<TValue, TNewValue> transformFunction, PropertyModificator modificator)
            where TContext : Bag
        {
            context.TransformProperty<TContext, TValue, TNewValue>(property, property, (ctx, val) => transformFunction(val), modificator);
        }

        public static void TransformProperty<TContext, TValue, TNewValue>(this TContext context, string property,
            Func<TContext, TValue, TNewValue> transformFunction, PropertyModificator modificator)
            where TContext : Bag
        {
            context.TransformProperty(property, property, transformFunction, modificator);
        }

        public static void TransformProperty<TContext, TValue, TNewValue>(this TContext context, string fromProperty,
            string toProperty, Func<TContext, TValue, TNewValue> transformFunction, PropertyModificator modificator)
            where TContext : Bag
        {
            if (context.HasNoValue() || transformFunction.HasNoValue())
            {
                return;
            }

            if (modificator == PropertyModificator.SkipIfExists)
            {
                return;
            }

            if (context.ContainsProperty(fromProperty, out TValue value))
            {
                var newValue = transformFunction(context, value);
                context.ApplyProperty(toProperty, newValue, modificator);
            }
        }

        public static void ApplyProperty<TContext, TValue>(this TContext context,
            string propertyName, Func<TContext, TValue> createValueFunction, PropertyModificator modificator)
            where TContext : Bag
        {
            if (context.HasNoValue() || createValueFunction.HasNoValue())
            {
                return;
            }

            if (modificator == PropertyModificator.SkipIfExists && context.ContainsProperty<TValue>(propertyName))
            {
                return;
            }

            var newValue = createValueFunction(context);
            context.ApplyProperty(propertyName, newValue, modificator);
        }

        public static async Task<TContext> RunWith<TContext>(this TContext context, IPipeline pipeline, IPipelineRunner runner = null)
        {
            await pipeline.Run(context, runner);
            return context;
        }

        public static async Task<TContext> RunWith<TContext>(this TContext context, IProcessor processor, IProcessorRunner runner = null)
        {
            await processor.Run(context, runner);
            return context;
        }

        /// <summary>
        /// Executes an action for pipeline context in case there is a property
        /// passed by <paramref name="property"/> exists.
        /// </summary>
        /// <typeparam name="TContext">Type of the pipeline context that is extended.</typeparam>
        /// <param name="context">
        /// Pipeline context that is extended.
        /// </param>
        /// <param name="property">
        /// The property to be checked before executing action.
        /// </param>
        /// <param name="action">
        /// The action to be executed when the property exists.
        /// </param>
        public static void IfHasProperty<TContext>(this TContext context, string property, Action<TContext> action)
            where TContext : Bag
        {
            if (context.HasValue() && context.ContainsProperty<object>(property))
            {
                action(context);
            }
        }

        /// <summary>
        /// Executes an action for pipeline context in case there is no property
        /// passed by <paramref name="property"/> exists.
        /// </summary>
        /// <typeparam name="TContext">Type of the pipeline context that is extended.</typeparam>
        /// <param name="context">
        /// Pipeline context that is extended.
        /// </param>
        /// <param name="property">
        /// The property to be checked before executing action.
        /// </param>
        /// <param name="action">
        /// The action to be executed when the property does not exist.
        /// </param>
        public static void IfHasNoProperty<TContext>(this TContext context, string property, Action<TContext> action)
            where TContext : Bag
        {
            if (context.HasValue() && !context.ContainsProperty<object>(property))
            {
                action(context);
            }
        }

        public static TBag Use<TBag>(this TBag bag, string property, object value,
            bool skipIfExists = true) where TBag: Bag
        {
            bag.Set(property, value, skipIfExists);
            return bag;
        }

        public static TBag Use<TBag>(this TBag bag, string property, Func<TBag, object> valueProvider,
            bool skipIfExists = true) where TBag : Bag
        {
            var value = valueProvider(bag);
            bag.Set(property, value, skipIfExists);
            return bag;
        }
    }
}