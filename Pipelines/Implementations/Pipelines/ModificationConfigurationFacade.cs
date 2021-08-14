using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class ModificationConfigurationFacade : IModificationConfiguration
    {
        public ModificationConfigurationFacade(IEnumerable<IModificationConfiguration> configurations)
        {
            Configurations = configurations;
        }

        public IEnumerable<IModificationConfiguration> Configurations { get; }

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var configuration in Configurations)
            {
                processors = configuration.GetModifications(processors);
            }

            return processors;
        }
    }
}
