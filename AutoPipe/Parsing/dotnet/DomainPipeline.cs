using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoPipe
{
    public class DomainPipeline : IPipeline
    {
        public AppDomain Domain { get; }
        public ITypeFilter Filter { get; }

        public DomainPipeline(AppDomain domain = null, ITypeFilter typeFilter = null)
        {
            Domain = domain ?? AppDomain.CurrentDomain;
            Filter = typeFilter ?? TypeFilter.Default;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            var assemblies = GetAssembliesThatMayContainProcessors(Domain);

            var parser = new AssemblyPipeline(assemblies, Filter, MethodFilterByAttributes.Instance);
            return parser.GetProcessors();
        }

        protected virtual IEnumerable<Assembly> GetAssembliesThatMayContainProcessors(AppDomain domain)
        {
            return domain.GetAssemblies();
        }
    }
}
