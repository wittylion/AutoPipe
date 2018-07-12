using System;
using System.Threading.Tasks;
using Pipelines.Implementations.Pipelines;
using Pipelines.Implementations.Processors;

namespace Pipelines.ExtensionMethods
{
    public static class PipelineExtensionMethods
    {
        public static IProcessor ToProcessor(this IPipeline pipeline)
        {
            return pipeline.ToProcessor(PipelineRunner.StaticInstance);
        }

        public static IProcessor ToProcessor(this IPipeline pipeline, PipelineRunner runner)
        {
            return ActionProcessor.FromAction(async args => await runner.Ensure(PipelineRunner.StaticInstance).RunPipeline(pipeline, args));
        }

        public static SafeTypeProcessor<T> ToProcessor<T>(this SafeTypePipeline<T> pipeline)
        {
            return pipeline.ToProcessor(PipelineRunner.StaticInstance);
        }

        public static SafeTypeProcessor<T> ToProcessor<T>(this SafeTypePipeline<T> pipeline, PipelineRunner runner)
        {
            return ActionProcessor.FromAction<T>(async args => await runner.Ensure(PipelineRunner.StaticInstance).RunPipeline(pipeline, args));
        }

        public static IPipeline RepeatProcessorsAsPipelineWhile<T>(this IPipeline pipeline, Func<bool> condition)
        {
            return new RepeatingProcessorsWhileConditionPipeline(pipeline.GetProcessors(), condition);
        }

        public static SafeTypePipeline<T> RepeatProcessorsAsPipelineWhile<T>(this SafeTypePipeline<T> pipeline, Func<bool> condition)
        {
            return new RepeatingProcessorsWhileConditionPipeline<T>(pipeline.GetProcessorsOfType(), condition);
        }

        public static Task Run(this IPipeline pipeline, object args)
        {
            return pipeline.Run(args, PipelineRunner.StaticInstance);
        }

        public static Task Run(this IPipeline pipeline, object args, PipelineRunner runner)
        {
            return runner.Ensure(PipelineRunner.StaticInstance).RunPipeline(pipeline, args);
        }

        public static Task RunPipelineWhile<TArgs>(this IPipeline pipeline, TArgs args,
            Predicate<TArgs> condition)
        {
            return pipeline.RunPipelineWhile(args, condition, PipelineRunner.StaticInstance);
        }

        public static async Task RunPipelineWhile<TArgs>(this IPipeline pipeline, TArgs args,
            Predicate<TArgs> condition, PipelineRunner runner)
        {
            runner = runner.Ensure(PipelineRunner.StaticInstance);
            while (condition(args))
            {
                await pipeline.Run(args, runner);
            }
        }
    }
}