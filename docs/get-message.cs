using System;
using Pipelines;

namespace Awesome.PipelinesNet
{
    class Program : AutoProcessor
    {
        static void Main(string[] args)
        {
            new Program().RunSync();
        }

        [Run]
        string GetMessage()
        {
            return "Hello from GetMessage";
        }

        [Run]
        void WriteToConsole(string message)
        {
            Console.WriteLine(message);
        }
    }
}
