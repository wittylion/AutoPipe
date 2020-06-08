using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public interface IModificationConfiguration 
    {
        IEnumerable<IProcessor> GetModifications(IProcessor processorType);
    }

}
