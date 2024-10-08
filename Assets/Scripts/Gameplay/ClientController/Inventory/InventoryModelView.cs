using System;
using UniRx;
using UnityEngine;

[Serializable]
public class InventoryModelView
{
    [SerializeField] private InventoryView _view;

    public Inventory Model { get; private set; }
    public InventoryView View => _view;

    public InventoryItem[] Items { get; private set; } = new InventoryItem[Inventory.MaxItems];
    public event Action<InventoryItem[]> ItemsChanged;
    public event Action<InventoryItem> ItemRemoved;

    public ReactiveProperty<int> ChoosenItemIndex { get; private set; } = new(-1);

    public void Initialize(Inventory model)
    {
        Model = model;
        Model.InventoryChanged += UpdateInventoryGrid;
        Model.ItemRemoved += i => ItemRemoved?.Invoke(i);
        View.Initialize(this);
    }

    public void UpdateInventoryGrid()
    {
        var items = Model.GetCopy();
        for(int i = 0; i < Inventory.MaxItems; i++)
        {
            if(i < items.Count)
            {
                Items[i] = items[i];
            }
            else
            {
                Items[i] = null;
            }
        }
        ItemsChanged?.Invoke(Items);
    }

    public void SlotClicked(int index)
    {
        if (Items[index] != null)
        {
            ChoosenItemIndex.Value = index;
        }
        else
        {
            ChoosenItemIndex.Value = -1;
        }
    }

    public void JoinItem()
    {
        if(ChoosenItemIndex.Value > -1)
        {
            Model.CombineItem(Items[ChoosenItemIndex.Value]);
        }
    }

    public void GiveToFriend()
    {
        if(ChoosenItemIndex.Value >= 0 && Model.GetItem(ChoosenItemIndex.Value) != null)
        {
            Model.GiveToAnotherPlayer(Model.GetItem(ChoosenItemIndex.Value));
            ChoosenItemIndex.Value = -1;
        }
    }

    public void CancelActionList()
    {
        ChoosenItemIndex.Value = -1;
    }
}
