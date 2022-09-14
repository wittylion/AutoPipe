using System;

namespace AutoPipe
{

    public class TypeFilterByNamespace : ITypeFilter
    {
        public static readonly string NamespaceCannotBeEmptyMessage = "You cannot set empty namespace. Please check the namespace parameter.";
        public string Namespace { get; }
        public bool Recursive { get; }
        public bool IncludeSkipped { get; }

        public TypeFilterByNamespace(string @namespace, bool recursive = true, bool includeSkipped = false)
        {
            if (string.IsNullOrWhiteSpace(@namespace))
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

        public bool Matches(Type type)
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
    }
}
