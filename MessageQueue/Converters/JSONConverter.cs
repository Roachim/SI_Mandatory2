using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections;
//using System.Text.Json;
using Newtonsoft.Json;

namespace MessageQueue.Converters
{
    public static class JSONConverter
    {
        public static object DeserializeJSON(string JSON)
        {
            //var deserializedJson = JsonSerializer.Deserialize<object>(JSON);            

            return JsonConvert.DeserializeObject(JSON);

        }

        public static string SerializeJSON(object item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}