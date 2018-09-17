using System;
using System.Threading.Tasks;
using Pipelines.Implementations.Pipelines;
using Pipelines.Implementations.Processors;

namespace Pipelines.ExtensionMethods
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
        /// instance of <see cref="PipelineRunner"/> - <see cref="PipelineRunner.StaticInstance"/>.
        /// </returns>
        public static IProcessor ToProcessor(this IPipeline pipeline)
        {
            return pipeline.ToProcessor(PipelineRunner.StaticInstance);
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
        /// instance of <see cref="PipelineRunner"/> - <see cref="PipelineRunner.StaticInstance"/>.
        /// </returns>
        public static IProcessor ToProcessor(this IPipeline pipeline, IPipelineRunner runner)
        {
            return ActionProcessor.FromAction(async args => await runner.Ensure(PipelineRunner.StaticInstance).RunPipeline(pipeline, args));
        }

        /// <summary>
        /// Converts a pipeline to a simple processor.
        /// This way pipelines may be nested and reused in other pipelines.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of the arguments that are used during pipeline execution.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be converted to a processor.
        /// </param>
        /// <returns>
        /// The processor that simply wraps a call to pipeline with a static
        /// instance of <see cref="PipelineRunner"/> - <see cref="PipelineRunner.StaticInstance"/>.
        /// </returns>
        public static SafeTypeProcessor<TArgs> ToProcessor<TArgs>(this SafeTypePipeline<TArgs> pipeline)
        {
            return pipeline.ToProcessor(PipelineRunner.StaticInstance);
        }

        /// <summary>
        /// Converts a pipeline to a simple processor.
        /// This way pipelines may be nested and reused in other pipelines.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of the arguments that are used during pipeline execution.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be converted to a processor.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        /// <returns>
        /// The processor that simply wraps a call to pipeline with a <paramref name="runner"/>
        /// or if it is not specified, with a static
        /// instance of <see cref="PipelineRunner"/> - <see cref="PipelineRunner.StaticInstance"/>.
        /// </returns>
        public static SafeTypeProcessor<TArgs> ToProcessor<TArgs>(this SafeTypePipeline<TArgs> pipeline, IPipelineRunner runner)
        {
            return ActionProcessor.FromAction<TArgs>(async args => await runner.Ensure(PipelineRunner.StaticInstance).RunPipeline(pipeline, args));
        }

        /// <summary>
        /// Use this mthod in case you need to repeat pipeline's processors several times. 
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline which processors must be repeated.
        /// </param>
        /// <param name="condition">
        /// The condition, that specifies till which point processors must be retrieved.
        /// </param>
        /// <returns>
        /// New pipeline that contains processors of the <paramref name="pipeline"/>
        /// repeated as many times as <paramref name="condition"/> returned <c>true</c>.
        /// </returns>
        public static IPipeline RepeatProcessorsAsPipelineWhile(this IPipeline pipeline, Func<bool> condition)
        {
            return new RepeatingProcessorsWhileConditionPipeline(pipeline.GetProcessors(), condition);
        }


        /// <summary>
        /// Use this mthod in case you need to repeat pipeline's processors several times. 
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of the arguments that are used during pipeline execution.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline which processors must be repeated.
        /// </param>
        /// <param name="condition">
        /// The condition, that specifies till which point processors must be retrieved.
        /// </param>
        /// <returns>
        /// New pipeline that contains processors of the <paramref name="pipeline"/>
        /// repeated as many times as <paramref name="condition"/> returned <c>true</c>.
        /// </returns>
        public static SafeTypePipeline<TArgs> RepeatProcessorsAsPipelineWhile<TArgs>(this SafeTypePipeline<TArgs> pipeline, Func<bool> condition)
        {
            return new RepeatingProcessorsWhileConditionPipeline<TArgs>(pipeline.GetProcessorsOfType(), condition);
        }

        /// <summary>
        /// Runs a pipeline with <c>null</c> context and <see cref="PipelineRunner.StaticInstance"/>.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="PipelineRunner.RunPipeline{TArgs}"/>
        /// method with <c>null</c> passed.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static Task Run(this IPipeline pipeline)
        {
            return pipeline.Run(null, PipelineRunner.StaticInstance);
        }

        /// <summary>
        /// Runs a pipeline with <paramref name="args"/> context
        /// and <see cref="PipelineRunner.StaticInstance"/>.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="PipelineRunner.RunPipeline{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static Task Run(this IPipeline pipeline, object args)
        {
            return pipeline.Run(args, PipelineRunner.StaticInstance);
        }

        /// <summary>
        /// Runs a pipeline with <paramref name="args"/> context
        /// and <paramref name="runner"/>.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="PipelineRunner.RunPipeline{TArgs}"/>
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
        public static Task Run(this IPipeline pipeline, object args, IPipelineRunner runner)
        {
            return runner.Ensure(PipelineRunner.StaticInstance).RunPipeline(pipeline, args);
        }

        /// <summary>
        /// Runs pipeline while a certain condition returns <c>true</c>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of the arguments that are used during pipeline execution.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="PipelineRunner.RunPipeline{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <param name="condition">
        /// The condition, that specifies till which point pipeline must be executed.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static Task RunPipelineWhile<TArgs>(this IPipeline pipeline, TArgs args,
            Predicate<TArgs> condition)
        {
            return pipeline.RunPipelineWhile(args, condition, PipelineRunner.StaticInstance);
        }

        /// <summary>
        /// Runs pipeline while a certain condition returns <c>true</c>.
        /// </summary>
        /// <typeparam name="TArgs">
        /// The type of the arguments that are used during pipeline execution.
        /// </typeparam>
        /// <param name="pipeline">
        /// The pipeline to be executed. Each processor of this pipeline
        /// will be executed by <see cref="PipelineRunner.RunPipeline{TArgs}"/>
        /// method with <paramref name="args"/> passed.
        /// </param>
        /// <param name="args">
        /// The context which will be passed to each processor of
        /// the pipeline during execution.
        /// </param>
        /// <param name="condition">
        /// The condition, that specifies till which point pipeline must be executed.
        /// </param>
        /// <param name="runner">
        /// The runner which will be used to run the wrapped pipeline.
        /// </param>
        /// <returns>
        /// The task object indicating the status of an executing pipeline.
        /// </returns>
        public static async Task RunPipelineWhile<TArgs>(this IPipeline pipeline, TArgs args,
            Predicate<TArgs> condition, IPipelineRunner runner)
        {
            runner = runner.Ensure(PipelineRunner.StaticInstance);
            while (condition(args))
            {
                await pipeline.Run(args, runner);
            }
        }
    }
}