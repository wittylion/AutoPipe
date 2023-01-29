using System;

namespace AutoPipe.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AfterAttribute : Attribute
    {
        public AfterAttribute(string previousName)
        {
            PreviousName = previousName;
        }

        public string PreviousName { get; }
    }
}
