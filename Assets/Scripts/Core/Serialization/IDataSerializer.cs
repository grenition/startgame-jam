namespace Core.Serialization
{
    public interface IDataSerializer
    {
        public string Serialize(object data);
        public T Deserialize<T>(string s);
    }
}
