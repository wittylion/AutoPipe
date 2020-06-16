using System.Collections.Generic;

namespace Pipelines.Implementations.Pipelines
{
    public interface IModificationConfiguration 
    {
        bool HasModifications(IProcessor processor);
        IEnumerable<IProcessor> GetModifications(IProcessor processor);
    }

}
