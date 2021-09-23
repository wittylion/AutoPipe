namespace AutoPipe.Tests.Units.ComplexTests.AutoProcessorTests.NonProcessorClassTest
{
    public class Getter
    {
        public static readonly string Name = $"{nameof(Getter)}.{nameof(GetMessage)}";

        [Run]
        public string GetMessage()
        {
            return Name;
        }
    }
}
