namespace Pipelines
{
    /// <summary>
    /// Class intended to hold name and value composing together a property.
    /// </summary>
    public struct PipelineProperty
    {
        /// <summary>
        /// The name of the object contained in this class.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The value that is associated with a property.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Constructor that accepts name and value composing a property object.
        /// </summary>
        /// <param name="name">
        /// The name of the object contained in this class.
        /// </param>
        /// <param name="value">
        /// The value that is associated with a property.
        /// </param>
        public PipelineProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}