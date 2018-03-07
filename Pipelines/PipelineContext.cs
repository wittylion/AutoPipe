using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Pipelines
{
    [Serializable]
    public abstract class PipelineContext : ISerializable
    {
        public bool IsAborted { get; set; }

        protected Lazy<LinkedList<PipelineMessage>> Messages { get; } =
            new Lazy<LinkedList<PipelineMessage>>(() => new LinkedList<PipelineMessage>());
        
        public virtual PipelineMessage[] GetMessages(MessageFilter filter)
        {
            if (Messages.IsValueCreated && Messages.Value.Count > 0)
            {
                if (filter == MessageFilter.All)
                {
                    return Messages.Value.ToArray();
                }
                return Messages.Value.Where(message => ((int)message.MessageType & (int)filter) > 0).ToArray();
            }
            return new PipelineMessage[0];
        }

        public virtual void AbortPipeline()
        {
            IsAborted = true;
        }

        public virtual void AddMessageObject(PipelineMessage message)
        {
            Messages.Value.AddLast(message);
        }

        public virtual void AddMessage(string message, MessageType messageType = MessageType.Information)
        {
            AddMessageObject(new PipelineMessage(message, messageType));
        }

        protected PipelineContext()
        {
        }

        protected PipelineContext(SerializationInfo info, StreamingContext context)
        {
        }

        public virtual void AbortPipelineWithMessage(string message)
        {
            AbortPipeline();
            AddMessage(message);
        }

        public virtual void AbortPipelineWithTypedMessage(string message, MessageType type)
        {
            AbortPipeline();
            AddMessage(message, type);
        }

        public virtual void AbortPipelineWithErrorMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Error);
        }

        public virtual void AbortPipelineWithWarningMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Warning);
        }

        public virtual void AbortPipelineWithInformationMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Information);
        }

        public virtual void AddInformation(string message)
        {
            AddMessage(message, MessageType.Information);
        }

        public virtual void AddWarning(string message)
        {
            AddMessage(message, MessageType.Warning);
        }

        public virtual void AddError(string message)
        {
            AddMessage(message, MessageType.Error);
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(PipelineContext)}.{nameof(IsAborted)}", IsAborted);
            info.AddValue($"{nameof(PipelineContext)}.{nameof(Messages)}", Messages, typeof(LinkedList<PipelineMessage>));
        }
    }
}