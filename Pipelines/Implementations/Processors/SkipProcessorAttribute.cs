using System;

namespace Pipelines.Implementations.Processors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SkipProcessorAttribute : Attribute
    {
    }
}