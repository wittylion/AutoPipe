using System;

namespace AutoPipe
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RunAllAttribute : RunAttribute
    {
    }
}
