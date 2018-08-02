namespace Pipelines.Implementations.Processors
{
    public class EnsurePropertyProcessor<TValue> : EnsurePropertyProcessorConcept<TValue>
    {
        private readonly string _name;
        private readonly TValue _value;
        
        public EnsurePropertyProcessor(string name, TValue value)
        {
            _name = name;
            _value = value;
        }

        public override string GetName()
        {
            return this._name;
        }

        public override TValue GetValue()
        {
            return this._value;
        }
    }
}