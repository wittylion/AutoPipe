using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AutoPipe
{
    public interface IFileParser
    {
        IEnumerable<IProcessor> GetProcessors(FileInfo file);
    }
}