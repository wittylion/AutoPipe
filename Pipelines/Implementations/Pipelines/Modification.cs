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

        public static ChainingModification Configure(Action<ChainingModification> configurator)
        {
            var result = new ChainingModification();

            if (configurator.IsNotNull())
            {
                configurator(result);
            }

            return result;
        }
    }
}
