using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Pipelines
{
    public class NamespacePipeline : IPipeline
    {
        public string Namespace { get; }
        public bool Recursive { get; }

        public NamespacePipeline(bool recursive = true)
        {
            var stackFrame = new StackTrace().GetFrame(1);
            if (stackFrame != null)
            {
                Namespace = stackFrame.GetMethod().DeclaringType.Namespace;
            }

            Recursive = recursive;
        }

        public NamespacePipeline(string @namespace, bool recursive = true)
        {
            Namespace = @namespace;
            Recursive = recursive;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            var result = new List<Type>();
            var assemblies = GetAssembliesThatMayContainProcessors();

            foreach (var assembly in assemblies)
            {
                var types = GetTypesInNamespaceSafe(assembly);
                result.AddRange(types);
            }

            return result.OrderBy(GetProcessorOrder).Select(ConstructProcessor);
        }

        protected virtual bool FilterProcessors(Type type)
        {
            bool matchesNamespace = false;
            if (Recursive)
            {
                if (!string.IsNullOrWhiteSpace(type.Namespace))
                {
                    matchesNamespace = type.Namespace.StartsWith(Namespace);
                }
            }
            else
            {
                matchesNamespace = type.Namespace == Namespace;
            }

            return matchesNamespace
                && typeof(IProcessor).IsAssignableFrom(type) 
                && type.GetCustomAttribute<SkipAttribute>() == null;
        }

        protected virtual int GetProcessorOrder(Type type)
        {
            return type?.GetCustomAttribute<OrderAttribute>()?.Order ?? default;
        }

        protected virtual IProcessor ConstructProcessor(Type type)
        {
            return type?.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]) as IProcessor ?? null;
        }

        protected virtual IEnumerable<Type> GetTypesInNamespaceSafe(Assembly assembly)
        {
            IEnumerable<Type> result;

            try
            {
                result = GetTypesInNamespace(assembly);
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

        protected virtual IEnumerable<Type> GetTypesInNamespace(Assembly assembly)
        {
            return assembly.GetTypes().Where(FilterProcessors);
        }

        protected virtual IEnumerable<Assembly> GetAssembliesThatMayContainProcessors()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}