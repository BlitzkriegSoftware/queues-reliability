using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blitz.Reliability.Demo.Extensions
{
    /// <summary>
    /// Dictionary to Json
    /// </summary>
    public static class DictToJson
    {
        /// <summary>
        /// Returns Dictionary as Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this IDictionary<string,object> data)
        {
            if (data == null) return "[]";
            return JsonConvert.SerializeObject(data);           
        }
    }
}
