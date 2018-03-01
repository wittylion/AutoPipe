using System.Collections.Generic;

namespace Pipelines
{
    public interface IPipeline
    {
        IEnumerable<IProcessor> GetProcessors();
    }
}