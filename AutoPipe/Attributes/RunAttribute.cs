using System;

namespace AutoPipe
{
    /// <summary>
    /// Marks a method to be executed within an <see cref="AutoProcessor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RunAttribute : Attribute
    {
        public bool ClaimAllParameters { get; set; }
    }
}
