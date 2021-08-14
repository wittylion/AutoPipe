using System.Collections.Generic;

namespace Pipelines
{
    /// <summary>
    /// Intended to provide a common interface to deal with
    /// many pipelines, so user can retrieve one or more
    /// pipelines using <see cref="GetSingle{TQuery}(TQuery)"/>
    /// or <see cref="GetMany{TQuery}(TQuery)"/> methods.
    /// </summary>
    public interface IPipelineRepository
    {
        /// <summary>
        /// In case you know some parameters about your
        /// pipeline, specify parameters in <paramref name="query"/>
        /// and pass them to this method, which will try to find
        /// a pipeline and put it in <typeparamref name="TQuery"/> result.
        /// </summary>
        /// <typeparam name="TQuery">
        /// A type of <see cref="QueryContext{TResult}"/> that should have
        /// an <see cref="IPipeline"/> in the result.
        /// </typeparam>
        /// <param name="query">
        /// A context that is supposed to have a query on the start
        /// and retrieve pipeline in the end of the execution.
        /// </param>
        void GetSingle<TQuery>(TQuery query) where TQuery : IPipeline;

        /// <summary>
        /// In case you know some parameters about a collection of pipelines
        /// specify these parameters in <paramref name="query"/>
        /// and pass them to this method, which will try to find a collection of
        /// pipelines and put them in <typeparamref name="TQuery"/> result.
        /// </summary>
        /// <typeparam name="TQuery">
        /// A type of <see cref="QueryContext{TResult}"/> that should have
        /// an <see cref="IPipeline"/> collection in the result.
        /// </typeparam>
        /// <param name="query">
        /// A context that is supposed to have a query on the start
        /// and retrieve pipelines collection in the end of the execution.
        /// </param>
        void GetMany<TQuery>(TQuery query) where TQuery : IEnumerable<IPipeline>;
    }
}