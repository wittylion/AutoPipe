using System;

namespace Pipelines
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class AkaAttribute : Attribute
    {
        public string[] Aliases { get; }
        public AkaAttribute(string name, params string[] aliases)
        {
            Aliases = new string[aliases.Length + 1];
            Aliases[0] = name;
            if (aliases.Length > 0)
            {
                Array.Copy(aliases, 0, Aliases, 1, aliases.Length);
            }
        }
    }
}
