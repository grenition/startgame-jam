using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;

    public Sprite Icon => _icon;
    public string Name => _name;
    public int Id { get; set; } = 0;
}
