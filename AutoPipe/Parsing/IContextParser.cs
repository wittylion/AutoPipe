using System.Collections.Generic;

namespace AutoPipe
{
    public interface IContextParser
    {
        IEnumerable<IProcessor> FindProcessors();
    }
}
