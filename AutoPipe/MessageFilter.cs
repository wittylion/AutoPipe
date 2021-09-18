using System;

namespace AutoPipe
{
    /// <summary>
    /// Represents an information, to be used to filter
    /// a set of messages, which were added to a non-sorted
    /// collection.
    /// </summary>
    [Flags]
    public enum MessageFilter : int
    {
        /// <summary>
        /// Represents a filter for information messages only.
        /// </summary>
        Info = MessageType.Information,

        /// <summary>
        /// Represents a filter for warning messages only.
        /// </summary>
        Warning = MessageType.Warning,

        /// <summary>
        /// Represents a filter for error messages only.
        /// </summary>
        Error = MessageType.Error,

        Debug = MessageType.Debug,

        InfoWarning = Info | Warning,

        InfoError = Info | Error,

        WarningError = Warning | Error,

        /// <summary>
        /// Represents a filter for all possible messages,
        /// including: Informations, Warnings and Errors.
        /// </summary>
        All = int.MaxValue,
    }
}