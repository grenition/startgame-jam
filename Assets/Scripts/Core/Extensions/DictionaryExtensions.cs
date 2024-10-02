using System.Collections.Generic;

namespace Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return;
            } 
            
            dictionary[key] = value;
        }

        public static void Delete<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key))
                return;

            dictionary.Remove(key);
        }
        
        public static TValue Get<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        {
            if(key == null || !dictionary.ContainsKey(key))
                return default;

            return dictionary[key];
        }
    }
}
