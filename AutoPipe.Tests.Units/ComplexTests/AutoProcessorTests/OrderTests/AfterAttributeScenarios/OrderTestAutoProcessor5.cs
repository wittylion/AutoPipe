namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.AfterAttributeScenarios
{
    public class OrderTestAutoProcessor5 : AutoProcessor
    {
        [Run]
        public string GetProperty() { return nameof(OrderTestAutoProcessor5); }

        [Run]
        [After(nameof(GetProperty))]
        public void APropertyUser(string property) { }
    }
}
