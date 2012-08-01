namespace dotless.Core.Cache
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Caching;
    using Abstractions;
    using Input;

    public class HttpCache : ICache
    {
        private readonly IHttp _http;
        public IPathResolver PathResolver { get; set; }

        public HttpCache(IHttp http, IPathResolver pathResolver)
        {
            _http = http;
            PathResolver = pathResolver;
        }

        public void Insert(string cacheKey, IList<string> fileDependancies, string css)
        {
            var fullPaths = fileDependancies.SelectArray(f => PathResolver.GetFullPath(f));

            var cache = GetCache();

            cache.Insert(cacheKey, css, new CacheDependency(fullPaths));
        }

        public bool Exists(string filename)
        {
            return Retrieve(filename) != null;
        }

        public string Retrieve(string filename)
        {
            return (string)GetCache()[filename];
        }

        private Cache GetCache()
        {
            return _http.Context.Cache;
        }
    }
}