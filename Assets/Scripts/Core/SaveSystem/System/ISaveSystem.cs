using Core.SaveSystem.Savable;

namespace Core.SaveSystem.System
{
    public interface ISaveSystem
    {
        public void RegisterSavable<TData>(ISavable<TData> savable);
        public void UnregisterSavable<TData>(ISavable<TData> savable);

        public void SaveData();
        public void LoadData();
        public void ResetData();
        public void SaveDataFrom<TData>(ISavable<TData> savable);
        public void LoadDataTo<TData>(ISavable<TData> savable);
        public void ResetDataIn<TData>(ISavable<TData> savable);
        public void SetSlot(int slotId);
    }
}
