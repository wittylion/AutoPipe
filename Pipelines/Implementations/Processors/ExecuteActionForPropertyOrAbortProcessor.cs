using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Processors
{
    public class ExecuteActionForPropertyOrAbortProcessor<TContext, TProperty>
        : ExecuteActionForPropertyProcessor<TContext, TProperty> where TContext : Bag
    {
        public string AbortMessage { get; }
        public MessageType MessageType { get; }

        public ExecuteActionForPropertyOrAbortProcessor(string propertyName, Action<TContext, TProperty> action,
            string abortMessage, MessageType messageType) : this(action.ToAsync(), propertyName, abortMessage, messageType)
        {
        }

        public ExecuteActionForPropertyOrAbortProcessor(Action<TContext, TProperty> action, string propertyName,
            string abortMessage, MessageType messageType) : this(action.ToAsync(), propertyName, abortMessage, messageType)
        {
        }

        public ExecuteActionForPropertyOrAbortProcessor(string propertyName, Func<TContext, TProperty, Task> action,
            string abortMessage, MessageType messageType) : this(action, propertyName, abortMessage, messageType)
        {
        }

        public ExecuteActionForPropertyOrAbortProcessor(Func<TContext, TProperty, Task> action, string propertyName,
            string abortMessage, MessageType messageType) : base(action, propertyName)
        {
            AbortMessage = abortMessage ?? throw new ArgumentNullException(nameof(propertyName),
                               ExecuteActionForPropertyOrAbortProcessor.AbortMessageMustBeSpecifiedInGeneric);
            MessageType = messageType;
        }

        public override Task PropertyExecution(TContext args, TProperty property)
        {
            return this.Action(args, property);
        }

        public override string GetPropertyName(TContext args)
        {
            return this.PropertyName;
        }

        public override Task MissingPropertyHandler(TContext args)
        {
            args.AbortPipelineWithTypedMessage(AbortMessage, MessageType);
            return base.MissingPropertyHandler(args);
        }

        public override Task WrongPropertyTypeHandler(TContext args, object property)
        {
            args.AbortPipelineWithTypedMessage(AbortMessage, MessageType);
            return base.WrongPropertyTypeHandler(args, property);
        }
    }

    public static class ExecuteActionForPropertyOrAbortProcessor
    {
        public static readonly string AbortMessageMustBeSpecifiedInGeneric =
            "Creating a generic class used to execute action for property or abort in case it is absent, you have to specify a message that will used for abort pipeline.";
    }
}