using System;

namespace AutoPipe
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StrictAttribute : Attribute
    {
    }
}
