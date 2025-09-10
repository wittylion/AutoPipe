using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoPipe
{
    public class ProcessorTypeRepository
    {
        private static Lazy<ProcessorTypeRepository> _instance = new Lazy<ProcessorTypeRepository>(() => new ProcessorTypeRepository());
        public static ProcessorTypeRepository Instance => _instance.Value;
        public IEnumerable<Type> Types { get; private set; }

        public ProcessorTypeRepository()
        {
            Reload();
        }

        public void Reload()
        {
            Types = GetTypes();
        }

        public List<Type> GetTypes()
        {
            var result = new List<Type>();
            var assemblies = GetAssembliesThatMayContainProcessors();

            foreach (var assembly in assemblies)
            {
                var types = GetTypesFromAssembly(assembly);
                result.AddRange(types);
            }

            return result;
        }

        protected virtual IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            IEnumerable<Type> result;

            try
            {
                result = GetProcessorTypes(assembly);
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

        protected virtual bool FilterProcessors(Type type)
        {
            return !type.IsAbstract 
                && (type.IsProcessor() || type.ShouldRun() || type.HasRunningMembers());
        }

        protected virtual IEnumerable<Type> GetProcessorTypes(Assembly assembly)
        {
            return assembly.GetTypes().Where(FilterProcessors);
        }

        protected virtual IEnumerable<Assembly> GetAssembliesThatMayContainProcessors()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}
