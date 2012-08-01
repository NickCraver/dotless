namespace dotless.Core.Cache
{
    using System.Collections.Generic;

    public interface ICache
    {
        void Insert(string cacheKey, IList<string> fileDependancies, string css);
        bool Exists(string cacheKey);
        string Retrieve(string cacheKey);
    }
}