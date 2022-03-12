using System.Collections.Generic;

namespace AutoPipe
{
    public class ContextPipeline : IPipeline
    {
        public ContextPipeline(IContextParser contextParser)
        {
            ContextParser = contextParser;
        }

        public IContextParser ContextParser { get; }

        public IEnumerable<IProcessor> GetProcessors()
        {
            return ContextParser.FindProcessors();
        }
    }
}
