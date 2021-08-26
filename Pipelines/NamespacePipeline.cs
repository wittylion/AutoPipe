using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentNullException(nameof(@namespace), $"You cannot set empty namespace. Please check the namespace parameter.");
            }

            Namespace = @namespace;
            Recursive = recursive;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            return Repository.Instance.Types.Where(FilterProcessors).OrderBy(GetProcessorOrder).Select(ConstructProcessor);
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
                && !type.ShouldSkip();
        }

        protected virtual int GetProcessorOrder(Type type)
        {
            return type.GetOrder();
        }

        protected virtual IProcessor ConstructProcessor(Type type)
        {
            return type?.GetConstructor(Type.EmptyTypes)?.Invoke(new object[0]) as IProcessor ?? null;
        }
    }
}