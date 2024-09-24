using Newtonsoft.Json;

namespace Core.Serialization.Json
{
    public class JsonDataSerializer : IDataSerializer
    {
        public string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(string s)
        {
            T data = default(T);
            
            if (string.IsNullOrEmpty(s))
            {
                return data;
            }
            
            return JsonConvert.DeserializeObject<T>(s) ?? data;
        }  
    }
}
