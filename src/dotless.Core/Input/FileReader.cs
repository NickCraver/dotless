namespace dotless.Core.Input
{
    using System.IO;
    using System.Collections.Concurrent;

    public class FileReader : IFileReader
    {
        private static ConcurrentDictionary<string, string> Cache = new ConcurrentDictionary<string, string>();

        public IPathResolver PathResolver { get; set; }
        private bool NoCache;

        public FileReader(string curDir, bool noCache = false) : this(new RelativePathResolver(curDir))
        {
            NoCache = noCache;
        }

        public FileReader(IPathResolver pathResolver)
        {
            PathResolver = pathResolver;
        }

        public string GetFileContents(string fileName)
        {
            fileName = PathResolver.GetFullPath(fileName);

            string cached;
            if (!NoCache && Cache.TryGetValue(fileName, out cached)) return cached;

            var ret = File.ReadAllText(fileName);

            Cache[fileName] = ret;

            return ret;
        }
    }
}