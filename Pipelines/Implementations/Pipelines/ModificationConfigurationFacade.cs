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

        public IEnumerable<IProcessor> GetModifications(IProcessor processorType)
        {
            foreach (var configuration in Configurations)
            {
                foreach (var processor in configuration.GetModifications(processorType))
                {
                    yield return processor;
                }
            }
        }
    }
}
