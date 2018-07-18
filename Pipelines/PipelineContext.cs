using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Pipelines.ExtensionMethods;

namespace Pipelines
{
    /// <summary>
    /// Introduces possibility to keep context information
    /// about the flow of the pipeline. By default it has
    /// messages collection which can be accessed by using
    /// <see cref="GetMessages"/> method and a flag
    /// <see cref="IsAborted"/> identifying whether pipeline was aborted.
    /// </summary>
    [Serializable]
    public class PipelineContext : ISerializable
    {
        /// <summary>
        /// Flag identifying whether pipeline must be aborted/stopped,
        /// it can be used as a cancelation identifier for the execution flow.
        /// </summary>
        public bool IsAborted { get; set; }

        /// <summary>
        /// Collection of messages that keeps all the text passed to the
        /// context during the pipeline execution.
        /// </summary>
        protected Lazy<ICollection<PipelineMessage>> Messages { get; } =
            new Lazy<ICollection<PipelineMessage>>(() => new PipelineMessageCollection());

        /// <summary>
        /// Method returns messages of the context according to the passed
        /// parameter <paramref name="filter"/>. This method is more flexible
        /// than other message returning methods of this class because you
        /// can specify your own filter.
        /// </summary>
        /// <param name="filter">
        /// Filter of type <see cref="MessageFilter"/> that allows
        /// to specify what kind of messages will be returned from the method.
        /// </param>
        /// <example>
        ///
        /// PipelineContext context = new PipelineContext();
        /// 
        /// context.AddInformation("The request was sent successfully");
        /// context.AddWarning("Could not recognize the id of the site, continued with the default");
        /// context.AddError("Request to the database failed, review the connection string");
        ///
        /// context.GetMessages(MessageFilter.Informations);
        /// context.GetMessages(MessageFilter.Informations | MessageFilter.Errors);
        /// context.GetMessages(MessageFilter.Errors| MessageFilter.Warnings);
        /// context.GetMessages(MessageFilter.All);
        /// 
        /// </example>
        /// <returns>
        /// An array of messages that belong to the context of pipeline,
        /// filtered specifically to parameter <paramref name="filter"/>.
        /// </returns>
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
        
        /// <summary>
        /// Returns all messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations, warnings and errors.
        /// </summary>
        /// <returns>
        /// All messages of the context, that have been added
        /// during pipeline execution.
        /// </returns>
        public virtual PipelineMessage[] GetAllMessages()
        {
            return this.GetMessages(MessageFilter.All);
        }
        
        /// <summary>
        /// Returns messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: informations and warnings.
        /// </summary>
        /// <returns>
        /// Information and warning messages of the context,
        /// that have been added during pipeline execution.
        /// </returns>
        public virtual PipelineMessage[] GetInformationsAndWarnings()
        {
            return this.GetMessages(MessageFilter.Informations | MessageFilter.Warnings);
        }

        /// <summary>
        /// Returns messages of the pipeline context
        /// that have been added during pipeline execution,
        /// including: warnings and errors.
        /// </summary>
        /// <returns>
        /// Warning and error messages of the context,
        /// that have been added during pipeline execution.
        /// </returns>
        public virtual PipelineMessage[] GetWarningsAndErrors()
        {
            return this.GetMessages(MessageFilter.Warnings | MessageFilter.Errors);
        }
        
        /// <summary>
        /// Returns information messages that have been added during
        /// pipeline execution to the context. It might contain
        /// some useful information about the status of the
        /// pipeline that has been running.
        /// </summary>
        /// <returns>
        /// Information messages of the pipeline execution context.
        /// </returns>
        public virtual PipelineMessage[] GetInformationMessages()
        {
            return this.GetMessages(MessageFilter.Informations);
        }

        /// <summary>
        /// Returns warning messages that have been added during
        /// pipeline execution to the context. It might contain
        /// some messages about the behavior that was not expected
        /// while pipeline has been running.
        /// </summary>
        /// <returns>
        /// Warning messages of the pipeline execution context.
        /// </returns>
        public virtual PipelineMessage[] GetWarningMessages()
        {
            return this.GetMessages(MessageFilter.Warnings);
        }

        /// <summary>
        /// Returns error messages that have been added during
        /// pipeline execution to the context. It might contain
        /// messages about the things that went wrong and caused
        /// lack of the expected result while pipeline was running.
        /// </summary>
        /// <returns>
        /// Error messages of the pipeline execution context.
        /// </returns>
        public virtual PipelineMessage[] GetErrorMessages()
        {
            return this.GetMessages(MessageFilter.Errors);
        }

        /// <summary>
        /// Aborts pipeline by setting a flag <see cref="IsAborted"/> to true.
        /// It allows to tell all the other users of this context that pipeline
        /// cannot be run further.
        /// </summary>
        public virtual void AbortPipeline()
        {
            IsAborted = true;
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
        public virtual void AddMessageObject(PipelineMessage message)
        {
            Messages.Value.Add(message);
        }

        /// <summary>
        /// Adds message objects to the context, which allows all the users
        /// of the context to see the status of the current operation.
        /// This method might be useful when you are copying messages
        /// from another context, or adding messages in scope.
        /// </summary>
        /// <param name="messages">
        /// Pipeline message collection, that contains <see cref="PipelineMessage"/>
        /// objects indicating the status of the operation.
        /// </param>
        public virtual void AddMessageObjects(IEnumerable<PipelineMessage> messages)
        {
            foreach (var message in messages.EnsureAtLeastEmpty())
            {
                this.AddMessageObject(message);
            }
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
        public virtual void AddMessage(string message, MessageType messageType = MessageType.Information)
        {
            AddMessageObject(new PipelineMessage(message, messageType));
        }

        /// <summary>
        /// Default parameterless constructor allowing you to create and use pipeline context.
        /// </summary>
        public PipelineContext()
        {
        }

        /// <summary>
        /// Constructor that is used by serialization and can dynamically
        /// recreate your pipeline context from serialization information.
        /// </summary>
        /// <param name="info">
        /// Serialization information, containing data of the serialized object.
        /// </param>
        /// <param name="context">Streaming context.</param>
        protected PipelineContext(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds a default message
        /// by using <see cref="AddMessage"/> method.
        /// </summary>
        /// <param name="message">
        /// Text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithMessage(string message)
        {
            AbortPipeline();
            AddMessage(message);
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds a message
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="message">
        /// Text that describes a cause of the abortion.
        /// </param>
        /// <param name="type">
        /// A type of the message, it will help you to find message
        /// by using <see cref="GetMessages"/> method.
        /// </param>
        public virtual void AbortPipelineWithTypedMessage(string message, MessageType type)
        {
            AbortPipeline();
            AddMessage(message, type);
        }
        
        /// <summary>
        /// Executes two actions: aborts pipeline and adds an error message
        /// which signals about wrong pipeline abortion.
        /// </summary>
        /// <param name="message">
        /// Error message text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithErrorMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Error);
        }

        /// <summary>
        /// Executes two actions: aborts pipeline and adds a warning message
        /// which signals about wrong pipeline execution.
        /// </summary>
        /// <param name="message">
        /// Warning message text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithWarningMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Warning);
        }
        
        /// <summary>
        /// Executes two actions: aborts pipeline and adds an information message
        /// which signals about early pipeline end.
        /// </summary>
        /// <param name="message">
        /// Information message text that describes a cause of the abortion.
        /// </param>
        public virtual void AbortPipelineWithInformationMessage(string message)
        {
            AbortPipelineWithTypedMessage(message, MessageType.Information);
        }

        /// <summary>
        /// Adds an information message. Useful method to track what happens
        /// during pipeline execution. It should be used often to provide
        /// clear and understandable flow.
        /// </summary>
        /// <param name="message">
        /// An information message, used to describe execution status.
        /// </param>
        public virtual void AddInformation(string message)
        {
            AddMessage(message, MessageType.Information);
        }

        /// <summary>
        /// Adds a warning message. Useful method to track some unoptimized
        /// pieces or things which could have cause an error.
        /// </summary>
        /// <param name="message">
        /// Warning message, used to describe warning status.
        /// </param>
        public virtual void AddWarning(string message)
        {
            AddMessage(message, MessageType.Warning);
        }
        
        /// <summary>
        /// Adds an error message. Use this method when during the pipeline
        /// execution, something goes wrong, and pipeline cannot proceed,
        /// so it must be stopped.
        /// </summary>
        /// <param name="message">
        /// Error message, used to describe an error occured during execution.
        /// </param>
        public virtual void AddError(string message)
        {
            AddMessage(message, MessageType.Error);
        }

        /// <summary>
        /// This method is used by serialization to convert an object to a
        /// text representation, including all properties and fields.
        /// </summary>
        /// <param name="info">
        /// Serialization information, containing data of the serialized object.
        /// </param>
        /// <param name="context">
        /// Streaming context.
        /// </param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(PipelineContext)}.{nameof(IsAborted)}", IsAborted);
            info.AddValue($"{nameof(PipelineContext)}.{nameof(Messages)}", Messages, typeof(ICollection<PipelineMessage>));
        }
    }
}