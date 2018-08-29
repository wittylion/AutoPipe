namespace Pipelines
{
    public struct PipelineProperty
    {
        public string Name { get; }
        public object Value { get; }

        public PipelineProperty(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}