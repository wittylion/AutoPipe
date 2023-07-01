using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoPipe
{
    public class Repository
    {
        private static Lazy<Repository> _instance = new Lazy<Repository>(() => new Repository());
        public static Repository Instance => _instance.Value;
        public static readonly BindingFlags RunningMethodsFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        public IEnumerable<Type> Types { get; }

        public Repository()
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
            return type.IsProcessor() || type.ShouldRun() || type.GetMembers(RunningMethodsFlags).Any(x => x.ShouldRun());
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
