namespace Pipelines.Implementations.Pipelines
{
    public interface IProcessorMatcher
    {
        bool Matches(IProcessor processor);
    }
}
