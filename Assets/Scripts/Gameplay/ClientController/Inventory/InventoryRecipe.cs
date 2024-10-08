using System;
using UnityEngine;

[Serializable]
public class InventoryRecipe
{
    [SerializeField] private int _item1Index, _item2Index, _resultIndex;

    public InventoryItem Item1 { get; private set; }
    public InventoryItem Item2 { get; private set; }
    public InventoryItem Result { get; private set; }

    public void Initialize(InventoryItem[] allItems)
    {
        Item1 = allItems[_item1Index];
        Item2 = allItems[_item2Index];
        Result = allItems[_resultIndex];
    }

    public bool CanCombine(InventoryItem item1, InventoryItem item2)
    {
        return 
            (item1 == Item1 && item2 == Item2) ||
            (item1 == Item2 && item2 == Item1);
    }

    public bool CanCombine(InventoryItem item)
    {
        return Item1 == item || Item2 == item;
    }
}
