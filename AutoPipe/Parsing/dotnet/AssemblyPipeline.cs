using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoPipe
{
    public class AssemblyPipeline : IPipeline
    {
        public IEnumerable<Assembly> Assemblies { get; }
        public ITypeFilter TypeFilter { get; }
        public IMethodFilter MethodFilter { get; }

        public AssemblyPipeline(Assembly assembly, ITypeFilter typeFilter, IMethodFilter methodFilter) : this(new[] { assembly }, typeFilter, methodFilter)
        {
        }

        public AssemblyPipeline(IEnumerable<Assembly> assemblies, ITypeFilter typeFilter, IMethodFilter methodFilter)
        {
            Assemblies = assemblies.OnlyValuable() ?? throw new ArgumentNullException(nameof(assemblies), "Assemblies parameter was null. Please review the collection of passed assemblies.");
            TypeFilter = typeFilter;
            MethodFilter = methodFilter;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            var result = new List<IProcessor>();
            foreach (var assembly in Assemblies)
            {
                var processorsFromAssembly = GetProcessorsFromAssembly(assembly);
                result.AddRange(processorsFromAssembly);
            }
            return result;
        }

        private IEnumerable<IProcessor> GetProcessorsFromAssembly(Assembly assembly)
        {
            var types = GetTypesFromAssembly(assembly);
            return types
                .Where(TypeFilter.Matches)
                .Select(ConstructProcessor);
        }

        protected virtual IProcessor ConstructProcessor(Type type)
        {
            var emptyConstructor = type?.GetConstructor(Type.EmptyTypes);
            if (emptyConstructor != null)
            {
                var obj = emptyConstructor.Invoke(new object[0]);
                if (obj is IProcessor processor)
                {
                    return processor;
                }

                return new AutoProcessor(obj, MethodFilter);
            }

            return null;
        }

        protected virtual IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            IEnumerable<Type> result;

            try
            {
                result = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Trace.TraceError(errorMessage);
                result = Enumerable.Empty<Type>();
            }

            return result;
        }
    }
}
