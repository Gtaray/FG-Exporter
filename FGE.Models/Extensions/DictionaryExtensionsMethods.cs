using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FGE.Models.Extensions
{
    internal static class DictionaryExtensionsMethods
    {
        public static void AddMany<TKey, TValue>(this Dictionary<TKey, TValue> parent, Dictionary<TKey, TValue> other)
        {
            foreach (var kvp in other)
            {
                parent.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
