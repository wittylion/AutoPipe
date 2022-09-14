using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoPipe
{
    public class NamespaceOptions
    {
        internal static string FromStackTrace()
        {
            int frame = 2;

            var stackFrame = new StackTrace().GetFrame(frame);
            while (stackFrame != null)
            {
                var assemblyName = stackFrame.GetMethod().DeclaringType.Assembly.GetName().Name;
                if (assemblyName != "AutoPipe")
                {
                    return stackFrame.GetMethod().DeclaringType.Namespace;
                }

                stackFrame = new StackTrace().GetFrame(++frame);
            }

            return null;
        }
    }

    public class NamespacePipeline : IPipeline
    {
        public NamespacePipeline(string @namespace = null, bool recursive = true, bool includeSkipped = false)
        {
            @namespace = @namespace ?? NamespaceOptions.FromStackTrace();

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