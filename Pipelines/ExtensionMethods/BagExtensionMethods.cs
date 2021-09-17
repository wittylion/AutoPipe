using System;
using System.Threading.Tasks;

namespace AutoPipe
{
    /// <summary>
    /// Extensions for the <see cref="PipelineContext"/>.
    /// </summary>
    public static class BagExtensionMethods
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

            if (context.Contains(fromProperty, out TValue value))
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

            if (modificator == PropertyModificator.SkipIfExists && context.Contains<TValue>(propertyName))
            {
                return;
            }

            var newValue = createValueFunction(context);
            context.ApplyProperty(propertyName, newValue, modificator);
        }

        public static async Task<TContext> Run<TContext>(this TContext context, IPipeline pipeline, IPipelineRunner runner = null) where TContext : Bag
        {
            await pipeline.Run(context, runner).ConfigureAwait(false);
            return context;
        }

        public static async Task<TContext> Run<TContext>(this TContext context, IProcessor processor, IProcessorRunner runner = null) where TContext : Bag
        {
            await processor.Run(context, runner).ConfigureAwait(false);
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
            if (context.HasValue() && context.Contains<object>(property))
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
            if (context.HasValue() && !context.Contains<object>(property))
            {
                action(context);
            }
        }

        public static TBag Use<TBag>(this TBag bag, string property, object value,
            bool skipIfExists = true) where TBag : Bag
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


        /// <summary>
        /// Adds message object to the context, which allows all the users
        /// of the context to see the status of the current operation.
        /// This method is more flexible than other methods adding messages,
        /// because allows to specify custom <see cref="PipelineMessage"/> object.
        /// </summary>
        /// <param name="message">
        /// A pipeline message object, that contains a text and a value of the message.
        /// </param>
        public static TBag Message<TBag>(this TBag bag, PipelineMessage message) where TBag : Bag
        {
            bag.AddMessage(message);
            return bag;
        }

        /// <summary>
        /// Adds a message to the pipeline execution context, which allows
        /// other users of the context see what happens in current operation.
        /// This method differs from other adding message methods by having
        /// <paramref name="message"/> text for the message and an optional
        /// <paramref name="messageType"/> allowing to specify kind of
        /// the message and by default is set to information.
        /// </summary>
        /// <param name="message">The text of the context message.</param>
        /// <param name="messageType">
        /// Message type indicating status of the operation. Default is information.
        /// </param>
        public static TBag Message<TBag>(this TBag bag, string message, MessageType messageType = MessageType.Information) where TBag : Bag
        {
            return bag.Message(new PipelineMessage(message, messageType));
        }

        /// <summary>
        /// Ends pipeline by setting a flag <see cref="Ended"/> to true.
        /// It allows to tell all the other users of this context that pipeline
        /// cannot be run further.
        /// </summary>
        public static TBag End<TBag>(this TBag bag) where TBag : Bag
        {
            bag.EndPipeline();
            return bag;
        }

        /// <summary>
        /// Executes two actions: ends pipeline and adds a default message
        /// by using <see cref="Message"/> method.
        /// </summary>
        /// <param name="message">
        /// Text that describes a cause of the end.
        /// </param>
        public static TBag End<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.End().Message(message);
        }

        /// <summary>
        /// Executes two actions: ends pipeline and adds a message
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="message">
        /// Text that describes a cause of the end.
        /// </param>
        /// <param name="type">
        /// A type of the message, it will help you to find message
        /// by using <see cref="GetMessages"/> method.
        /// </param>
        public static TBag End<TBag>(this TBag bag, string message, MessageType type) where TBag : Bag
        {
            return bag.End().Message(message, type);
        }

        /// <summary>
        /// Executes two actions: ends pipeline and adds an error message
        /// which signals about wrong pipeline end.
        /// </summary>
        /// <param name="message">
        /// Error message text that describes a cause of the end.
        /// </param>
        public static TBag ErrorEnd<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.End(message, MessageType.Error);
        }

        /// <summary>
        /// Executes two actions: end pipeline and adds a warning message
        /// which signals about wrong pipeline execution.
        /// </summary>
        /// <param name="message">
        /// Warning message text that describes a cause of the end.
        /// </param>
        public static TBag WarningEnd<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.End(message, MessageType.Warning);
        }

        /// <summary>
        /// Executes two actions: end pipeline and adds an information message
        /// which signals about early pipeline end.
        /// </summary>
        /// <param name="message">
        /// Information message text that describes a cause of the end.
        /// </param>
        public static TBag InfoEnd<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.End(message, MessageType.Information);
        }

        /// <summary>
        /// Adds an information message. Useful method to track what happens
        /// during pipeline execution. It should be used often to provide
        /// clear and understandable flow.
        /// </summary>
        /// <param name="message">
        /// An information message, used to describe execution status.
        /// </param>
        public static TBag Info<TBag>(this TBag bag, string message, bool end = false) where TBag : Bag
        {
            bag.Message(message, MessageType.Information);
            if (end) { bag.End(); }
            return bag;
        }

        public static TBag Debug<TBag>(this TBag bag, string message, bool end = false, bool ignoreDebugOption = false) where TBag : Bag
        {
            if (ignoreDebugOption || bag.Debug)
            {
                bag.Message(message, MessageType.Debug);
            }

            if (end)
            {
                bag.End();
            }

            return bag;
        }

        public static TBag Debug<TBag>(this TBag bag, Func<string> messageGetter, bool end = false, bool ignoreDebugOption = false) where TBag : Bag
        {
            if (ignoreDebugOption || bag.Debug)
            {
                var message = messageGetter();
                bag.Message(message, MessageType.Debug);
            }

            if (end)
            {
                bag.End();
            }

            return bag;
        }

        /// <summary>
        /// Adds a warning message. Useful method to track some unoptimized
        /// pieces or things which could have cause an error.
        /// </summary>
        /// <param name="message">
        /// Warning message, used to describe warning status.
        /// </param>
        public static TBag Warning<TBag>(this TBag bag, string message, bool end = false) where TBag : Bag
        {
            bag.Message(message, MessageType.Warning);
            if (end) { bag.End(); }
            return bag;
        }

        /// <summary>
        /// Adds an error message. Use this method when during the pipeline
        /// execution, something goes wrong, and pipeline cannot proceed,
        /// so it must be stopped.
        /// </summary>
        /// <param name="message">
        /// Error message, used to describe an error occured during execution.
        /// </param>
        public static TBag Error<TBag>(this TBag bag, string message, bool end = false) where TBag : Bag
        {
            bag.Message(message, MessageType.Error);
            if (end) { bag.End(); }
            return bag;
        }

        public static TBag Delete<TBag>(this TBag bag, string name) where TBag : Bag
        {
            bag.DeleteProperty(name);
            return bag;
        }

        public static TBag Set<TBag>(this TBag bag, string name, object value, bool skipIfExists = false) where TBag : Bag
        {
            bag.SetProperty(name, value, skipIfExists);
            return bag;
        }

        public static TBag SetResult<TBag>(this TBag bag, object result) where TBag : Bag
        {
            return bag.Set(Bag.ResultProperty, result);
        }

        public static TBag UnsetResult<TBag>(this TBag bag) where TBag : Bag
        {
            return bag.Delete(Bag.ResultProperty);
        }

        /// <summary>
        /// Provide a result and some information about the result
        /// or about the process of getting this result.
        /// </summary>
        /// <param name="result">
        /// Result object which is obtained during pipeline execution.
        /// </param>
        /// <param name="message">
        /// Provide several words about the result or about the process
        /// of obtaining this result. It will be helpful to understand
        /// why this result was provided.
        /// </param>
        public static TBag InfoResult<TBag>(this TBag bag, object result, string message) where TBag : Bag
        {
            return bag.SetResult(result).Info(message);
        }

        /// <summary>
        /// Provide a result and warning message indicating some
        /// problems related to the result or to the process of
        /// getting this result.
        /// </summary>
        /// <param name="result">
        /// Result object which is obtained during pipeline execution.
        /// </param>
        /// <param name="message">
        /// Provide several words about the result or about the process
        /// of obtaining this result. It will be helpful to understand
        /// why this result was provided.
        /// </param>
        public static TBag WarningResult<TBag>(this TBag bag, object result, string message) where TBag : Bag
        {
            return bag.SetResult(result).Warning(message);
        }

        /// <summary>
        /// Provide a result and error message indicating encountered
        /// problems related to the result or to the process of
        /// getting this result.
        /// </summary>
        /// <param name="result">
        /// Result object which is obtained during pipeline execution.
        /// </param>
        /// <param name="message">
        /// Provide several words about the result or about the process
        /// of obtaining this result. It will be helpful to understand
        /// why this result was provided.
        /// </param>
        public static TBag ErrorResult<TBag>(this TBag bag, object result, string message) where TBag : Bag
        {
            return bag.SetResult(result).Error(message);
        }

        /// <summary>
        /// Executes 3 actions: end pipeline, adds error message,
        /// resets result to null.
        /// </summary>
        /// <param name="message">
        /// Error message indicating the reason of the ended pipeline and no result.
        /// </param>
        public static TBag ErrorEndNoResult<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.UnsetResult().ErrorEnd(message);
        }

        /// <summary>
        /// Executes 3 actions: end pipeline, adds warning message,
        /// resets result to null.
        /// </summary>
        /// <param name="message">
        /// Warning message indicating the reason of the ended pipeline and no result.
        /// </param>
        public static TBag WarningEndNoResult<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.UnsetResult().WarningEnd(message);
        }

        /// <summary>
        /// Executes 3 actions: end pipeline, adds information message,
        /// resets result to null.
        /// </summary>
        /// <param name="message">
        /// Information message indicating the reason of the ended pipeline and no result.
        /// </param>
        public static TBag InfoEndNoResult<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.UnsetResult().InfoEnd(message);
        }

        /// <summary>
        /// Resets the result to null and adds an information message
        /// describing the reason of the reset result.
        /// </summary>
        /// <param name="message">
        /// Information message describing the reason of the reset result.
        /// </param>
        public static TBag ResetResultWithInformation<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.UnsetResult().Info(message);
        }

        /// <summary>
        /// Resets the result to null and adds a warning message
        /// describing the reason of the reset result.
        /// </summary>
        /// <param name="message">
        /// Warning message describing the reason of the reset result.
        /// </param>
        public static TBag ResetResultWithWarning<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.UnsetResult().Warning(message);
        }

        /// <summary>
        /// Resets the result to null and adds a error message
        /// describing the reason of the reset result.
        /// </summary>
        /// <param name="message">
        /// Error message describing the reason of the reset result.
        /// </param>
        public static TBag ResetResultWithError<TBag>(this TBag bag, string message) where TBag : Bag
        {
            return bag.UnsetResult().Error(message);
        }

        public static TBag EndResult<TBag>(this TBag bag, object result) where TBag : Bag
        {
            return bag.SetResult(result).End();
        }

        public static TBag InfoEndResult<TBag>(this TBag bag, object result, string message) where TBag : Bag
        {
            return bag.SetResult(result).InfoEnd(message);
        }

        public static TBag ErrorEndResult<TBag>(this TBag bag, object result, string message) where TBag : Bag
        {
            return bag.SetResult(result).ErrorEnd(message);
        }


        public static TBag WarningEndResult<TBag>(this TBag bag, object result, string message) where TBag : Bag
        {
            return bag.SetResult(result).WarningEnd(message);
        }

    }
}