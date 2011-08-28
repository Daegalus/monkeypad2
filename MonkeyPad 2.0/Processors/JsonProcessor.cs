using System;
using Newtonsoft.Json;

namespace MonkeyPad2.Processors
{
    public class JsonProcessor
    {
        public static String ToJson<T>(T inObject)
        {
            var jsonSettings = new JsonSerializerSettings
                                   {
                                       MissingMemberHandling = MissingMemberHandling.Ignore,
                                       NullValueHandling = NullValueHandling.Ignore
                                   };
            return JsonConvert.SerializeObject(inObject, Formatting.Indented, jsonSettings);
        }

        public static T FromJson<T>(String jsonContent)
        {
            var jsonSettings = new JsonSerializerSettings
                                   {
                                       MissingMemberHandling = MissingMemberHandling.Ignore,
                                       NullValueHandling = NullValueHandling.Ignore
                                   };
            return JsonConvert.DeserializeObject<T>(jsonContent, jsonSettings);
        }
    }
}