using System;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Collections;
//using System.Text.Json;
using Newtonsoft.Json;

namespace MessageQueue
{
    public static class JSONConverter
    {
        private static object DeserializeJSON(string JSON)
        {
            //var deserializedJson = JsonSerializer.Deserialize<object>(JSON);

            var deserializedJson = JsonConvert.DeserializeObject(JSON);            

            

        }
    }
}