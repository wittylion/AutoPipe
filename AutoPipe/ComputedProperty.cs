using System;
using System.Linq.Expressions;

namespace AutoPipe
{
    public class ComputedProperty
    {
        public LambdaExpression Lambda { get; set; }
        private Delegate compliledDelegate;
        public Delegate Compliled { get { return compliledDelegate ?? (compliledDelegate = Lambda.Compile()); } }

        public ComputedProperty(LambdaExpression lambda)
        {
            Lambda = lambda;
        }

        public TValue Invoke<TValue>(Bag bag)
        {
            return (TValue) Invoke(bag);
        }

        public object Invoke(Bag bag)
        {
            var parameters = new object[Lambda.Parameters.Count];
            for (int i = 0; i < Lambda.Parameters.Count; i++)
            {
                var parameter = Lambda.Parameters[i];
                var obj = bag.Get(parameter.Name, or: () => parameter.Type.GetDefault());
                if (parameter.Type.IsAssignableFrom(obj?.GetType()))
                {
                    parameters[i] = obj;
                }
                else if (obj is ComputedProperty computed && parameter.Type.IsAssignableFrom(computed.Lambda.ReturnType))
                {
                    parameters[i] = computed.Invoke(bag);
                }
                else
                {
                    parameters[i] = parameter.Type.GetDefault();
                }
            }
            var result = Compliled.DynamicInvoke(parameters);
            return result;
        }

        public static implicit operator ComputedProperty(LambdaExpression lambda)
        {
            return new ComputedProperty(lambda);
        }
    }
}
