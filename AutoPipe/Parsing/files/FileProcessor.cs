using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AutoPipe
{
    [Aka("Files Pipeline")]
    [Is("collecting the files from directory")]
    public class FilesPipeline : IPipeline
    {
        public IEnumerable<FileSystemInfo> Items { get; }
        public IFileParser FileParser { get; }
        public bool Recursive { get; }

        public FilesPipeline(IEnumerable<FileSystemInfo> items, IFileParser fileParser = null, bool recursive = true)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items), "Collection of items and/or directories has to be provided.");
            FileParser = fileParser ?? AutoPipe.FileParser.Default;
            Recursive = recursive;
        }

        public IEnumerable<IProcessor> GetProcessors()
        {
            var result = new List<IProcessor>();

            foreach (var item in Items)
            {
                var processors = CreateProcessors(item);

                if (!processors.IsEmpty())
                {
                    result.AddRange(processors);
                }
            }

            return result;
        }

        private IEnumerable<IProcessor> CreateProcessors(FileSystemInfo item)
        {
            if (item is FileInfo file)
            {
                return FileParser.GetProcessors(file);
            }
            else if (item is DirectoryInfo directory)
            {
                if (!Recursive)
                {
                    return Enumerable.Empty<IProcessor>();
                }

                return new FilesPipeline(directory.GetFileSystemInfos(), FileParser, recursive: true).GetProcessors();
            }
            else
            {
                return Enumerable.Empty<IProcessor>();
            }
        }
    }
}
