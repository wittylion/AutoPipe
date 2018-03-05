using System;

namespace Pipelines
{
    [Flags]
    public enum MessageFilter : int
    {
        Informations = 1,
        Warnings = 2,
        Errors = 4,
        All = 7
    }
}