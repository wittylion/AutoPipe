using System;
using System.Threading.Tasks;
using Pipelines.ExtensionMethods;

namespace Pipelines.Implementations.Contexts
{
    public class ChainingContextBuilder<TContext> where TContext : PipelineContext
    {
        public ChainingContextBuilder(TContext originalContext)
        {
            OriginalContext = originalContext;
        }

        public TContext OriginalContext { get; }

        public virtual ChainingContextBuilder<TContext> Use<TValue>(string property, TValue value,
            PropertyModificator modificator = PropertyModificator.SkipIfExists)
        {
            OriginalContext.ApplyProperty(property, value, modificator);
            return this;
        }

        public virtual ChainingContextBuilder<TContext> Use<TValue>(string property, Func<TContext, TValue> valueProvider,
            PropertyModificator modificator = PropertyModificator.SkipIfExists)
        {
            OriginalContext.ApplyProperty(property, valueProvider, modificator);
            return this;
        }

        public virtual async Task<ChainingContextBuilder<TContext>> RunWith(IPipeline pipeline, IPipelineRunner runner)
        {
            await OriginalContext.RunWithPipeline(pipeline, runner);
            return this;
        }
    }
}