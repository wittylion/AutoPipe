using System.Collections.Generic;
using System.Linq;

namespace Pipelines.Implementations.Pipelines
{
    public class ModificationConfigurationFacade : IModificationConfiguration
    {
        public ModificationConfigurationFacade(IEnumerable<IModificationConfiguration> configurations)
        {
            Configurations = configurations;
        }

        public IEnumerable<IModificationConfiguration> Configurations { get; }

        public IEnumerable<IProcessor> GetModifications(IProcessor processor)
        {
            foreach (var configuration in Configurations)
            {
                if (configuration.HasModifications(processor))
                {
                    foreach (var substitute in configuration.GetModifications(processor))
                    {
                        yield return substitute;
                    }
                }
            }
        }

        public bool HasModifications(IProcessor processor)
        {
            return Configurations.Any(configuration => configuration.HasModifications(processor));
        }
    }
}
