using Microsoft.AspNetCore.Http;

namespace SharedLibrary.Extensions
{
    public static class HeadersDictionaryExtensions
    {
        public static void AddHeaderIfNotInDict(this IHeaderDictionary headerDict, string key, string value) 
        {
            if (!headerDict.ContainsKey(key)) 
            {
                headerDict.Add(key, value);
            }
        }
    }
}