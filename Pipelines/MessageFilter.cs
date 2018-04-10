using System;

namespace Pipelines
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
        Informations = 1,

        /// <summary>
        /// Represents a filter for warning messages only.
        /// </summary>
        Warnings = 2,

        /// <summary>
        /// Represents a filter for error messages only.
        /// </summary>
        Errors = 4,

        /// <summary>
        /// Represents a filter for all possible messages,
        /// including: Informations, Warnings and Errors.
        /// </summary>
        All = 7
    }
}