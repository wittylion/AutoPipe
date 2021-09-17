using System;

namespace AutoPipe
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class AkaAttribute : Attribute
    {
        public static readonly string NameParameterIsEmpty = "The name parameter must contain a meaningful value, but contained null or empty string.";
        public static readonly string AliasIsEmpty = "The {0} parameter must contain a meaningful name, but contained null or empty string.";

        public string[] Aliases { get; }
        public AkaAttribute(string name, params string[] aliases)
        {
            Aliases = new string[aliases.Length + 1];
            if (name.HasNoValue())
            {
                throw new ArgumentException(NameParameterIsEmpty, nameof(name));
            }
            Aliases[0] = name;

            for (int i = 0; i < aliases.Length; i++)
            {
                var alias = aliases[i];
                if (alias.HasNoValue())
                {
                    throw new ArgumentException(AliasIsEmpty.FormatWith(i + 2), nameof(aliases));
                }
                Aliases[i + 1] = alias;
            }
        }
    }
}
