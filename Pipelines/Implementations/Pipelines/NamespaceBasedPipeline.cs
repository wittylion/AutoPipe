using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Pipelines.Implementations.Processors;

namespace Pipelines.Implementations.Pipelines
{
    public class NamespaceBasedPipeline : IPipeline
    {
        public string Namespace { get; }

        public NamespaceBasedPipeline(string @namespace)
        {
            Namespace = @namespace;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            var result = new List<IProcessor>();
            var assemblies = GetAssembliesThatMayContainProcessors();

            foreach (var assembly in assemblies)
            {
                var types = GetTypesInNamespaceSafe(assembly);

                if (types.Any())
                {
                    var processors = from type in types
                           let constructor = type.GetConstructor(Type.EmptyTypes)
                           where constructor != null
                           let attributes = type.GetCustomAttributes()
                           where !attributes.OfType<SkipProcessorAttribute>().Any()
                           let orderAttribute = attributes.OfType<ProcessorOrderAttribute>().FirstOrDefault()
                           orderby orderAttribute?.Order ?? default, type.Name
                           select constructor.Invoke(new object[0]) as IProcessor;

                    result.AddRange(processors);
                }
            }

            return result.AsReadOnly();
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
            var types = assembly.GetTypes();

            return from type in types
                    where type.Namespace == Namespace && typeof(IProcessor).IsAssignableFrom(type)
                    select type;
        }

        protected virtual IEnumerable<Assembly> GetAssembliesThatMayContainProcessors()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }
}