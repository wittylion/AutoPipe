using System;
using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public class AddLastProcessorModification : IModificationConfiguration
    {
        public AddLastProcessorModification(IEnumerable<IProcessor> processors)
        {
            Processors = processors ?? throw new ArgumentNullException(nameof(processors), "Processors must be specified for modification that adds processors at the end.");
        }

        public IEnumerable<IProcessor> Processors { get; }

        public IEnumerable<IProcessor> GetModifications(IEnumerable<IProcessor> processors)
        {
            foreach (var processor in processors)
            {
                yield return processor;
            }

            foreach (var processor in Processors)
            {
                yield return processor;
            }
        }
    }
}
