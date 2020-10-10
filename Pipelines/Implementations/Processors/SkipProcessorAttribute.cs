using System;

namespace Pipelines.Implementations.Processors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SkipProcessorAttribute : Attribute
    {
    }
}