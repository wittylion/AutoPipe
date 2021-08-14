using System;

namespace Pipelines
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SkipAttribute : Attribute
    {
    }
}