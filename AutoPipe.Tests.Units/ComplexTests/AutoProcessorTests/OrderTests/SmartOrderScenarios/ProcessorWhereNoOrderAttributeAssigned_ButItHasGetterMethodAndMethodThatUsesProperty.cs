namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class ProcessorWhereNoOrderAttributeAssigned_ButItHasGetterMethodAndMethodThatUsesProperty : AutoProcessor
    {
        [Run]
        public string GetProperty() { return nameof(ProcessorWhereNoOrderAttributeAssigned_ButItHasGetterMethodAndMethodThatUsesProperty); }

        [Run]
        public void UseProperty(string property) { }
    }
}
