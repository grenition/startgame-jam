using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

[Serializable]
public class Inventory
{
    public const int MaxItems = 6;

    [SerializeField] private InventoryItem[] _allItems;
    [SerializeField] private InventoryItemNames _names;
    [SerializeField] private InventoryModelView _modelView;

    private ControllerNetworkBus _bus;
    private ClientIdentification _identification;

    public event Action InventoryChanged;
    public event Action<InventoryItem> ItemRemoved;

    public InventoryItem[] AllItems => _allItems;
    public InventoryItemNames Names => _names;

    private readonly List<InventoryItem> Items = new(MaxItems);

    [Inject]
    private void Construct(ControllerNetworkBus bus, ClientIdentification identification)
    {
        for (int i = 0; i < _allItems.Length; i++)
        {
            _allItems[i].Id = i;
        }
        _bus = bus;
        _identification = identification;
        _modelView.Initialize(this);
    }

    public void AddItem(InventoryItem item)
    {
        Items.Add(item);
        InventoryChanged?.Invoke();
    }

    public void AddItemFromStorage(string name)
    {
        AddItem(GetItemFromStorage(name));
    }

    public void RemoveItem(InventoryItem item)
    {
        Items.Remove(item);
        InventoryChanged?.Invoke();
        ItemRemoved?.Invoke(item);
    }

    public List<InventoryItem> GetCopy()
    {
        return new List<InventoryItem>(Items);
    }

    public bool HasItem(InventoryItem item)
    {
        return Items.Contains(item);
    }

    public bool HasItemByName(string name)
    {
        return Items.Exists(x => x.Name == name);
    }

    public InventoryItem GetItem(int index)
    {
        return Items[index];
    }

    public bool RemoveItemByName(string name)
    {
        foreach(var item in Items)
        {
            if(item.Name == name)
            {
                RemoveItem(item);
                return true;
            }
        }
        return false;
    }

    public void GiveToAnotherPlayer(InventoryItem item)
    {
        if(Items.Contains(item))
        {
            Items.Remove(item);
            _bus.GiveInventoryItem(item, _identification.PlayerType is PlayerTypes.Small ?
                PlayerTypes.Big : PlayerTypes.Small);
            InventoryChanged?.Invoke();
        }
    }

    public InventoryItem GetItemFromStorage(string name)
    {
        foreach(var item in _allItems)
        {
            if(item.Name == name)
            {
                return item;
            }
        }
        return null;
    }
}

[Serializable]
public class InventoryItemNames
{
    [SerializeField] private string _key, _basket, _fullBasket, _light, _stone, _pruner, _toy;

    public string Key => _key;
    public string Basket => _basket;
    public string FullBasket => _fullBasket;
    public string Light => _light;
    public string Stone => _stone;
    public string Pruner => _pruner;
    public string Toy => _toy;
}