using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AutoPipe
{
    public class NamespacePipeline : IPipeline
    {
        public static readonly string NamespaceCannotBeEmptyMessage = "You cannot set empty namespace. Please check the namespace parameter.";
        public string Namespace { get; }
        public bool Recursive { get; }
        public bool IncludeSkipped { get; }

        public NamespacePipeline(string @namespace = null, bool recursive = true, bool includeSkipped = false)
        {
            if (@namespace == null)
            {
                var stackFrame = new StackTrace().GetFrame(1);
                if (stackFrame != null)
                {
                    Namespace = stackFrame.GetMethod().DeclaringType.Namespace;
                }
            }
            else if (string.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentNullException(nameof(@namespace), NamespaceCannotBeEmptyMessage);
            }
            else
            {
                Namespace = @namespace;
            }

            Recursive = recursive;
            IncludeSkipped = includeSkipped;
        }

        public virtual IEnumerable<IProcessor> GetProcessors()
        {
            return Repository.Instance.Types
                .Where(FilterProcessors)
                .OrderBy(GetProcessorOrder)
                .Select(ConstructProcessor);
        }

        protected virtual bool FilterProcessors(Type type)
        {
            if (Recursive)
            {
                if (string.IsNullOrWhiteSpace(type.Namespace))
                {
                    return false;
                }

                if (!type.Namespace.StartsWith(Namespace))
                {
                    return false;
                }
            }
            else
            {
                if (type.Namespace != Namespace)
                {
                    return false;
                }
            }

            if (!IncludeSkipped && type.ShouldSkip()) return false;

            return true;
        }

        protected virtual int GetProcessorOrder(Type type)
        {
            return type.GetOrder();
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

                return new AutoProcessor(obj);
            }

            return null;
        }
    }
}