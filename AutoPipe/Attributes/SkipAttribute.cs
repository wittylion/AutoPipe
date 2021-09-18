using System;

namespace AutoPipe
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SkipAttribute : Attribute
    {
    }
}