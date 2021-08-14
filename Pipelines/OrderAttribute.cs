using System;

namespace Pipelines
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute(int order)
        {
            Order = order;
        }
    }
}