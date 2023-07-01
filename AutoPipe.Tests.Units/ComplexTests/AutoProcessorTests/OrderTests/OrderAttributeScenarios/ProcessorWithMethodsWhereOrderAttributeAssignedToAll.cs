namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.OrderAttributeScenarios
{
    public class ProcessorWithMethodsWhereOrderAttributeAssignedToAll : AutoProcessor
    {
        [Run]
        [Order(2)]
        public string GetProperty() { return nameof(ProcessorWithMethodsWhereOrderAttributeAssignedToAll); }

        [Run]
        [Order(1)]
        public void UseProperty(string property) { }
    }
}
