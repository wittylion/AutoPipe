using System;

namespace Pipelines
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class IsAttribute : Attribute
    {
        public static readonly string DescriptionParameterIsEmpty = "The description parameter must contain a meaningful value describing the target, but contained null or empty string.";

        public string Description { get; }
        public IsAttribute(string description)
        {
            if (description.HasNoValue())
            {
                throw new ArgumentException(DescriptionParameterIsEmpty, nameof(description));
            }

            Description = description;
        }
    }
}
