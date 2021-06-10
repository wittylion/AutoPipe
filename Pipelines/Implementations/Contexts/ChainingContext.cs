using System;
using System.Threading.Tasks;

namespace Pipelines.Implementations.Contexts
{
    public class ChainingContext<TContext> where TContext : PipelineContext
    {
        public TContext OriginalContext { get; }

        public ChainingContext(TContext originalContext)
        {
            OriginalContext = originalContext;
        }

        public virtual ChainingContext<TContext> Use<TValue>(string property, TValue value,
            PropertyModificator modificator = PropertyModificator.SkipIfExists)
        {
            OriginalContext.ApplyProperty(property, value, modificator);
            return this;
        }

        public virtual ChainingContext<TContext> Use<TValue>(string property, Func<TContext, TValue> valueProvider,
            PropertyModificator modificator = PropertyModificator.SkipIfExists)
        {
            OriginalContext.ApplyProperty(property, valueProvider, modificator);
            return this;
        }

        public virtual async Task<ChainingContext<TContext>> RunWith(IPipeline pipeline, IPipelineRunner runner = null)
        {
            await OriginalContext.RunWithPipeline(pipeline, runner).ConfigureAwait(false);
            return this;
        }
    }
}