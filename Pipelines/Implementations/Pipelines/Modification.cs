using Pipelines.ExtensionMethods;
using System;

namespace Pipelines.Implementations.Pipelines
{
    public static class Modification
    {
        public static ChainingModification Configure()
        {
            return new ChainingModification();
        }

        public static ChainingModification Configure(params Action<ChainingModification>[] configurators)
        {
            var result = new ChainingModification();

            if (configurators.IsNotNull())
            {
                foreach (var configurator in configurators)
                {
                    configurator(result);
                }
            }

            return result;
        }
    }
}
