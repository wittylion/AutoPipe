namespace Pipelines.Implementations.Processors
{
    /// <summary>
    /// An implementation of <see cref="EnsurePropertyProcessorConcept{TValue}"/>
    /// which accepts key and value in the constructor and when the
    /// <see cref="IProcessor.Execute"/> method of the processor is called
    /// the name and value pair is added to the context.
    /// </summary>
    /// <typeparam name="TValue">
    /// Type of the value object that will be added for a given key.
    /// </typeparam>
    public class EnsurePropertyProcessor<TValue> : EnsurePropertyProcessorConcept<TValue>
    {
        private readonly string _name;
        private readonly TValue _value;
        
        public EnsurePropertyProcessor(string name, TValue value)
        {
            _name = name;
            _value = value;
        }

        public override string GetName(Bag args)
        {
            return this._name;
        }

        public override TValue GetValue(Bag args)
        {
            return this._value;
        }
    }
}