namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.AfterAttributeScenarios
{
    public class OrderTestAutoProcessor6 : AutoProcessor
    {
        [Run]
        [After(nameof(APropertyUser))]
        public string GetProperty() { return nameof(OrderTestAutoProcessor6); }

        [Run]
        public void APropertyUser(string property) { }
    }
}
