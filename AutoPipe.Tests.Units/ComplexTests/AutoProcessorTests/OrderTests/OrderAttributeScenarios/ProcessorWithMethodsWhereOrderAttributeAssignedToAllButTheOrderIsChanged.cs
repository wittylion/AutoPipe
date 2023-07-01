namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.OrderAttributeScenarios
{
    public class ProcessorWithMethodsWhereOrderAttributeAssignedToAllButTheOrderIsChanged : AutoProcessor
    {
        [Run]
        [Order(1)]
        public string GetProperty() { return nameof(ProcessorWithMethodsWhereOrderAttributeAssignedToAllButTheOrderIsChanged); }

        [Run]
        [Order(2)]
        public void UseProperty(string property) { }
    }
}
