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
        void WriteToConsole()
        {
            Console.WriteLine("Hello world!");
        }
    }
}
