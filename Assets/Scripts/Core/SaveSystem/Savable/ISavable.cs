namespace Core.SaveSystem.Savable
{
    public interface ISavable<TData>
    {
        public string Key { get; }
        public void ApplyData(TData data);
        public TData GetData();
        public TData GetDefaultData();
    }
}
