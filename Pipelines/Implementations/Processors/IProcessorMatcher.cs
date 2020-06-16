namespace Pipelines.Implementations.Processors
{
    public interface IProcessorMatcher
    {
        bool Matches(IProcessor processor);
    }
}
