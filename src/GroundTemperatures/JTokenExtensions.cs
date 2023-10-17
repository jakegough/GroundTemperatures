using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GroundTemperatures
{
    public static class JTokenExtensions
    {
        public static IEnumerable<KeyValuePair<string, JToken?>> ToEnumerable(this JObject jtoken)
        {
            foreach (var item in jtoken)
            {
                yield return item;
            }
        }

        public static IEnumerable<KeyValuePair<string, T?>> ToEnumerable<T>(this JObject jtoken)
            where T : JToken
        {
            foreach (var item in jtoken)
            {
                yield return new KeyValuePair<string, T?>(item.Key, item.Value as T);
            }
        }
    }
}
