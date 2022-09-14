using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoPipe
{
    public static class FileParser
    {
        public static readonly IFileParser Default = new BasicFileParser();

        public static IFileParser Custom(Func<FileInfo, IEnumerable<IProcessor>> function)
        {
            return new DelegateFileParser(function);
        }
    }

    public class BasicFileParser : IFileParser
    {
        public IEnumerable<IProcessor> GetProcessors(FileInfo file)
        {
            switch (file.Extension)
            {
                case ".dll":
                    return new AssemblyPipeline(Assembly.LoadFrom(file.FullName)).GetProcessors();

                default: 
                    return Enumerable.Empty<IProcessor>();
            }
        }
    }

    public class DelegateFileParser : IFileParser
    {
        public DelegateFileParser(Func<FileInfo, IEnumerable<IProcessor>> function)
        {
            ProcessorsProvider = function ?? throw new ArgumentNullException(nameof(function), "The function that returns processors must be provided.");
        }

        public Func<FileInfo, IEnumerable<IProcessor>> ProcessorsProvider { get; }

        public IEnumerable<IProcessor> GetProcessors(FileInfo file)
        {
            return ProcessorsProvider(file);
        }
    }
}
