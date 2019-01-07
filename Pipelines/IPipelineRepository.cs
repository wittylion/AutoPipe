namespace Pipelines
{
    public interface IPipelineRepository
    {
        void GetSingle<TQuery>(TQuery query) where TQuery : QueryContext<IPipeline>;
        void GetMany<TQuery>(TQuery query) where TQuery : QueryContext<IPipeline[]>;
    }
}