using System;
using System.Linq;
using System.Threading.Tasks;
using Pipelines.Modifications;

namespace Pipelines
{
    /// <summary>
    /// Pipeline extension methods intended to simplify
    /// an experience of using pipelines at the start point.
    /// </summary>
    public static class PipelineExtensionMethods
    {
        /// <summary>
        /// Converts a pipeline to a simple processor.
        /// This way pipelines may be nested and reused in other pipelines.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to be converted to a processor.
        /// </param>
        /// <returns>
        /// The processor that simply wraps a call to pipeline with a static
        /// instance of <see cref="Runner"/> - <see cref="Runner.Instance"/>.
        /// </returns>
        public static IProcessor ToProcessor(this IPipeline pipeline)
        {
            return pipeline.ToProcessor(Runner.Instance);
        }

        /// <summary>
        /// Converts a pipeline to a simple processor.
        /// This way pipelines may be nested and reused in other pipelines.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to be converted to a processor.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        /// <returns>
        /// The processor that simply wraps a call to pipeline with a <paramref name="runner"/>
        /// or if it is not specified, with a static
        /// instance of <see cref="Runner"/> - <see cref="Runner.Instance"/>.
        /// </returns>
        public static IProcessor ToProcessor(this IPipeline pipeline, IPipelineRunner runner)
        {
            return Processor.From(async args => await pipeline.Run(args, runner).ConfigureAwait(false));
        }

        /// <summary>
        /// Runs a pipeline with <paramref name="args"/> context
        /// and <paramref name="runner"/>.
        /// </summary>
        /// <typeparam name="TContext">
        /// The type of the context that is used during pipeline execution.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="IPipelineRunner.Run{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static Task Run(this IPipeline pipeline, Bag args = null, IPipelineRunner runner = null)
        {
            runner = runner ?? Runner.Instance;
            args = args ?? new Bag();
            return runner.Run(pipeline, args);
        }

        public static Task Run(this IPipeline pipeline, object args, IPipelineRunner runner = null)
        {
            runner = runner ?? Runner.Instance;
            if (!(args is Bag bag))
            {
                bag = new Bag(args);
            }
            return runner.Run(pipeline, bag);
        }

        /// <summary>
        /// Runs a pipeline with <paramref name="args"/> context
        /// and <paramref name="runner"/>, and then returns a result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result that is supposed to be returned from the method.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="IPipelineRunner.Run{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static async Task<TResult> Run<TResult>(this IPipeline pipeline, Bag args = null, IPipelineRunner runner = null) where TResult : class
        {
            args = args ?? new Bag();
            await pipeline.Run(args, runner).ConfigureAwait(false);
            return args.ResultOrThrow<TResult>();
        }

        public static async Task<TResult> Run<TResult>(this IPipeline pipeline, object args = null, IPipelineRunner runner = null) where TResult : class
        {
            if (!(args is Bag bag))
            {
                bag = new Bag(args);
            }
            await pipeline.Run(bag, runner).ConfigureAwait(false);
            return bag.ResultOrThrow<TResult>();
        }

        /// <summary>
        /// Runs a pipeline with <paramref name="args"/> context
        /// and <paramref name="runner"/> synchronously, waiting until
        /// all processors of the pipeline will be executed.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="IPipelineRunner.Run{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        public static void RunSync(this IPipeline pipeline, Bag args = null, IPipelineRunner runner = null)
        {
            pipeline.Run(args, runner).Wait();
        }

        public static void RunSync(this IPipeline pipeline, object args, IPipelineRunner runner = null)
        {
            pipeline.Run(args, runner).Wait();
        }

        /// <summary>
        /// Runs a pipeline with <paramref name="args"/> context
        /// and <paramref name="runner"/> synchronously, waiting until
        /// all processors of the pipeline will be executed
        /// and then returns a result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result that is supposed to be returned from the method.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="IPipelineRunner.Run{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static TResult RunSync<TResult>(this IPipeline pipeline, Bag args = null, IPipelineRunner runner = null) where TResult : class
        {
            return pipeline.Run<TResult>(args, runner).Result;
        }

        public static TResult RunSync<TResult>(this IPipeline pipeline, object args = null, IPipelineRunner runner = null) where TResult : class
        {
            return pipeline.Run<TResult>(args, runner).Result;
        }

        /// <summary>
        /// Returns pipeline wrapper that caches processors after the moment 
        /// of processors first request from <paramref name="pipeline"/>.
        /// The processors will never be updated after first request.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline which processors should be cached in memory.
        /// </param>
        /// <param name="useLazyLoading">
        /// Defines whether processors should be cached after 
        /// first request (true) or if processors should be 
        /// cached during the call of this method (false).
        /// </param>
        /// <returns>
        /// Returns pipeline wrapper that caches processors.
        /// </returns>
        public static IPipeline CacheInMemory(this IPipeline pipeline, bool useLazyLoading = true)
        {
            if (!useLazyLoading)
            {
                return Pipeline.From(pipeline.GetProcessors().ToArray());
            }

            var loaded = false;
            return new CachedPipeline(pipeline, () => {
                var result = !loaded;
                loaded = true;
                return result;
            }, useLazyLoading);
        }

        /// <summary>
        /// Returns pipeline wrapper that caches processors for the specified <paramref name="period"/>.
        /// The period starts at the moment you request processors from <paramref name="pipeline"/>
        /// and updates on next processors request when period is over.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline which processors should be cached in memory.
        /// </param>
        /// <param name="period">
        /// The TimeSpan period that defines how long processors will be cached.
        /// </param>
        /// <param name="useLazyLoading">
        /// Defines whether processors should be cached after 
        /// first request (true) or if processors should be 
        /// cached during the call of this method (false).
        /// </param>
        /// <returns>
        /// Returns pipeline wrapper that caches processors for the specified <paramref name="period"/>.
        /// </returns>
        public static IPipeline CacheInMemoryForPeriod(this IPipeline pipeline, TimeSpan period, bool useLazyLoading = true)
        {
            DateTime? startedTime = null;
            return new CachedPipeline(pipeline, () =>
            {
                if (!startedTime.HasValue || DateTime.Now - startedTime > period)
                {
                    startedTime = DateTime.Now;
                    return true;
                }

                return false;
            }, useLazyLoading);
        }

        /// <summary>
        /// Returns pipeline wrapper that caches processors for the specified <paramref name="minutes"/>.
        /// The period starts at the moment you request processors from <paramref name="pipeline"/>
        /// and updates on next processors request when period is over.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline which processors should be cached in memory.
        /// </param>
        /// <param name="minutes">
        /// The period in minutes that defines how long processors will be cached.
        /// </param>
        /// <param name="useLazyLoading">
        /// Defines whether processors should be cached after 
        /// first request (true) or if processors should be 
        /// cached during the call of this method (false).
        /// </param>
        /// <returns>
        /// Returns pipeline wrapper that caches processors for the specified <paramref name="minutes"/>.
        /// </returns>
        public static IPipeline CacheInMemoryForMinutes(this IPipeline pipeline, int minutes, bool useLazyLoading = true)
        {
            return pipeline.CacheInMemoryForPeriod(TimeSpan.FromMinutes(minutes), useLazyLoading);
        }

        /// <summary>
        /// Returns pipeline wrapper that caches processors for the specified <paramref name="hours"/>.
        /// The period starts at the moment you request processors from <paramref name="pipeline"/>
        /// and updates on next processors request when period is over.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline which processors should be cached in memory.
        /// </param>
        /// <param name="hours">
        /// The period in hours that defines how long processors will be cached.
        /// </param>
        /// <param name="useLazyLoading">
        /// Defines whether processors should be cached after 
        /// first request (true) or if processors should be 
        /// cached during the call of this method (false).
        /// </param>
        /// <returns>
        /// Returns pipeline wrapper that caches processors for the specified <paramref name="hours"/>.
        /// </returns>
        public static IPipeline CacheInMemoryForHours(this IPipeline pipeline, int hours, bool useLazyLoading = true)
        {
            return pipeline.CacheInMemoryForPeriod(TimeSpan.FromHours(hours), useLazyLoading);
        }

        /// <summary>
        /// Creates a new pipeline which will apply <see cref="IModificationConfiguration.GetModifications(IProcessor)"/>
        /// method on each processor of the original pipeline.
        /// </summary>
        /// <param name="pipeline">
        /// An original pipeline to be modified by <paramref name="configuration"/>.
        /// </param>
        /// <param name="configuration">
        /// A configuration that describes which processors should be used instead of original.
        /// </param>
        /// <returns>
        /// A new instance of pipeline, that applies <paramref name="configuration"/> to the processors
        /// of an original pipeline.
        /// </returns>
        public static IPipeline Modify(this IPipeline pipeline, IModificationConfiguration configuration)
        {
            return new ModifiedPipeline(pipeline, configuration).CacheInMemory(false);
        }

        /// <summary>
        /// Creates a new pipeline which will apply <see cref="IModificationConfiguration.GetModifications(IProcessor)"/>
        /// method on each processor of the original pipeline.
        /// </summary>
        /// <param name="pipeline">
        /// An original pipeline to be modified by <paramref name="configuration"/>.
        /// </param>
        /// <param name="modification">
        /// A modification object that describes which processors should be used instead of original.
        /// </param>
        /// <returns>
        /// A new instance of pipeline, that applies <paramref name="configuration"/> to the processors
        /// of an original pipeline.
        /// </returns>
        public static IPipeline Modify(this IPipeline pipeline, ChainingModification modification)
        {
            var configuration = modification.GetConfiguration();
            return Modify(pipeline, configuration);
        }

        /// <summary>
        /// Creates a new pipeline which will apply <see cref="IModificationConfiguration.GetModifications(IProcessor)"/>
        /// method on each processor of the original pipeline.
        /// </summary>
        /// <param name="pipeline">
        /// An original pipeline to be modified by <paramref name="configuration"/>.
        /// </param>
        /// <param name="modification">
        /// An action object describing how processors should be modified.
        /// </param>
        /// <returns>
        /// A new instance of pipeline, that applies <paramref name="configuration"/> to the processors
        /// of an original pipeline.
        /// </returns>
        public static IPipeline Modify(this IPipeline pipeline, params Action<ChainingModification>[] configurators)
        {
            var modification = Modification.Configure(configurators);
            return Modify(pipeline, modification);
        }

        public static string Name(this IPipeline pipeline)
        {
            return pipeline.GetType().GetName();
        }

        public static string Description(this IPipeline pipeline)
        {
            return pipeline.GetType().GetDescription();
        }
    }
}