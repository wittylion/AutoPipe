using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoPipe
{
    public class NamespacePipeline : IPipeline
    {
        public NamespacePipeline(string @namespace = null, bool recursive = true, bool includeSkipped = false)
        {
            if (@namespace == null)
            {
                var stackFrame = new StackTrace().GetFrame(1);
                if (stackFrame != null)
                {
                    @namespace = stackFrame.GetMethod().DeclaringType.Namespace;
                }
            }

            Filter = TypeFilter.Default.And(TypeFilter.Namespace(@namespace, recursive, includeSkipped));
        }

        public ITypeFilter Filter { get; }

        public virtual IEnumerable<IProcessor> GetProcessors()
        {
            IEnumerable<IProcessor> processors = new DomainPipeline(typeFilter: Filter).GetProcessors();
            return processors.OrderBy(processor => processor.Order());
        }
    }
}