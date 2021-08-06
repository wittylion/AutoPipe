using System;

namespace Pipelines
{
    /// <summary>
    /// Object representing a message and its type.
    /// </summary>
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
    }
}