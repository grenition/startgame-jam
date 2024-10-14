using System;
using System.Collections.Generic;
using Core.SaveSystem.Savable;
using ToolBox.Serialization;

namespace Core.SaveSystem.System
{
    public class SaveSystem : ISaveSystem
    {
        public class SavableActions
        {
            public Action LoadAction;
            public Action SaveAction;
            public Action ResetAction;
        }

        private Dictionary<object, SavableActions> _savables = new();
        
        #region Savables registration
        public void RegisterSavable<TData>(ISavable<TData> savable)
        {
            if(_savables.ContainsKey(savable)) return;
            
            _savables.Add(savable, new SavableActions()
            {
                LoadAction = () => savable.ApplyData(DataSerializer.Load<TData>(savable.Key) ?? savable.GetDefaultData()),
                SaveAction = () => DataSerializer.Save(savable.Key, savable.GetData()),
                ResetAction = () => DataSerializer.Save(savable.Key, savable.GetDefaultData())
            });
        }
        public void UnregisterSavable<TData>(ISavable<TData> savable)
        {
            if(!_savables.ContainsKey(savable)) return;

            _savables.Remove(savable);
        }
        #endregion
        
        #region Interactions
        public void SaveData()
        {
            foreach(var savable in _savables)
                savable.Value.SaveAction.Invoke();
        }
        public void LoadData()
        {
            foreach(var savable in _savables)
                savable.Value.LoadAction.Invoke();
        }
        public void ResetData()
        {
            DataSerializer.DeleteAll();
            foreach(var savable in _savables)
                savable.Value.ResetAction.Invoke();
        }
        public void SaveDataFrom<TData>(ISavable<TData> savable)
        {
            if(!_savables.ContainsKey(savable)) return;
            _savables[savable].SaveAction.Invoke();
        }
        public void LoadDataTo<TData>(ISavable<TData> savable)
        {
            if(!_savables.ContainsKey(savable)) return;
            _savables[savable].LoadAction.Invoke();
        }
        public void ResetDataIn<TData>(ISavable<TData> savable)
        {
            if(!_savables.ContainsKey(savable)) return;
            _savables[savable].ResetAction.Invoke();
            LoadDataTo(savable);
        }
        public void SetSlot(int slotId)
        {
            DataSerializer.ChangeProfile(slotId);
        }
        #endregion
    }
}
