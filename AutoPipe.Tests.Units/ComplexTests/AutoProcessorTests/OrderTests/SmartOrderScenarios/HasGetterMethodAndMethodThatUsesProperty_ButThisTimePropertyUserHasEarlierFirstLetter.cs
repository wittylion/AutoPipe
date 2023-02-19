namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.OrderTests.SmartOrderScenarios
{
    public class HasGetterMethodAndMethodThatUsesProperty_ButThisTimePropertyUserHasEarlierFirstLetter : AutoProcessor
    {
        [Run]
        public string GetProperty() { return nameof(HasGetterMethodAndMethodThatUsesProperty_ButThisTimePropertyUserHasEarlierFirstLetter); }

        [Run]
        public void APropertyUser(string property) { }
    }
}
