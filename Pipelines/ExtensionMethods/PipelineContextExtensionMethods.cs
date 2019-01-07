using System;

namespace Pipelines.ExtensionMethods
{
    /// <summary>
    /// Extensions for the <see cref="PipelineContext"/>.
    /// </summary>
    public static class PipelineContextExtensionMethods
    {
        public static void TransformProperty<TContext, TValue, TNewValue>(this TContext context, string fromProperty,
            string toProperty, Func<TContext, TValue, TNewValue> transformFunction, PropertyModificator modificator)
            where TContext : PipelineContext
        {
            if (context.HasNoValue() || transformFunction.HasNoValue())
            {
                return;
            }

            if (modificator == PropertyModificator.SkipIfExists && context.HasProperty(toProperty))
            {
                return;
            }

            var property = context.GetPropertyObjectOrNull(fromProperty);
            if (property.HasValue && property.Value.Value is TValue value)
            {
                var newValue = transformFunction(context, value);
                context.ApplyProperty(toProperty, newValue, modificator);
            }
        }

        public static void ApplyProperty<TContext, TValue>(this TContext context,
            string propertyName, Func<TContext, TValue> createValueFunction, PropertyModificator modificator)
            where TContext : PipelineContext
        {
            if (context.HasNoValue() || createValueFunction.HasNoValue())
            {
                return;
            }

            if (modificator == PropertyModificator.SkipIfExists && context.HasProperty(propertyName))
            {
                return;
            }

            var newValue = createValueFunction(context);
            context.ApplyProperty(propertyName, newValue, modificator);
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
            where TContext : PipelineContext
        {
            if (context.HasValue() && context.HasProperty(property))
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
            where TContext : PipelineContext
        {
            if (context.HasValue() && !context.HasProperty(property))
            {
                action(context);
            }
        }
    }
}