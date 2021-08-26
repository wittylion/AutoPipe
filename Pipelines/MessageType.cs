namespace Pipelines
{
    /// <summary>
    /// Represents an information, to be used in message object
    /// to specify an importance of the message, which can
    /// be used later to make some decisions.
    /// </summary>
    public enum MessageType : int
    {
        /// <summary>
        /// Represents an information message type.
        /// It can be used to provide some information
        /// about the flow or a process of execution.
        /// </summary>
        Information = 1,

        /// <summary>
        /// Represents a warning message type.
        /// It can be used to provide warnings
        /// of the flow execution, which will
        /// indicate some difficulties of the execution,
        /// that can lead to a not predictable flow.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Represents an error message type.
        /// This message type must be used to
        /// indicate problems of the flow, so user
        /// can quickly make a decision regarding
        /// the incorrect flow.
        /// </summary>
        Error = 4,

        Debug = 8,
    }
}