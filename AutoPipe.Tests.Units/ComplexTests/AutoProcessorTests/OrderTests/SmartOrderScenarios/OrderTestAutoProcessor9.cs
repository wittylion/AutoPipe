namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class OrderTestAutoProcessor9 : AutoProcessor
    {
        [Run]
        public string GetFirst(string second) { return second + nameof(OrderTestAutoProcessor9); }

        [Run]
        public string GetSecond() { return nameof(OrderTestAutoProcessor9); }

        [Run]
        public void APropertyUser(string first, string second, string third) { }
    }
}
