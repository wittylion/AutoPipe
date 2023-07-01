using System;
using System.Diagnostics;

namespace AutoPipe
{
    /// <summary>
    /// Object representing a message and its type.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class PipelineMessage
    {
        /// <summary>
        /// Message that is used in exception thrown in constructor when message object is null.
        /// </summary>
        public static readonly string MessageIsNotSetError = "String representing a message text should be specified.";

        /// <summary>
        /// The message representing this class.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// The type of the message that describes its importance.
        /// </summary>
        public MessageType MessageType { get; }

        protected virtual string DebuggerDisplay => $"{MessageType}: {Message}";

        /// <summary>
        /// Constructor that accepts message and its type.
        /// </summary>
        /// <param name="message">
        /// The message representing this class.
        /// </param>
        /// <param name="messageType">
        /// The type of the message that describes its importance.
        /// </param>
        public PipelineMessage(string message, MessageType messageType)
        {
            if (message.HasNoValue())
            {
                throw new ArgumentException(MessageIsNotSetError, nameof(message));
            }

            Message = message;
            MessageType = messageType;
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        public static implicit operator string(PipelineMessage message)
        {
            return message.Message;
        }

        public static implicit operator PipelineMessage(string message)
        {
            return new PipelineMessage(message, MessageType.Information);
        }

        public bool IsError => MessageType == MessageType.Error;
        public bool IsWarning => MessageType == MessageType.Warning;
        public bool IsDebug => MessageType == MessageType.Debug;
        public bool IsInfo => MessageType == MessageType.Information;
    }
}