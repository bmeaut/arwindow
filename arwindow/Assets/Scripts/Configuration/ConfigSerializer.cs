using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Serialization
{
    public static class ConfigSerializer
    {
        public static T ReadToClass<T>(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            T t = JsonConvert.DeserializeObject<T>(jsonString);

            return t;
        }

        public static void WriteFromClass<T>(string filePath, T t)
        {
            string json = JsonConvert.SerializeObject(t, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static JObject ReadJsonFile(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            return JObject.Parse(jsonString);
        }
    } 
}
