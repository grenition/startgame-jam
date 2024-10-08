using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private string _name;
    [SerializeField] private AudioClip _takeItemSound;

    public Sprite Icon => _icon;
    public string Name => _name;
    public AudioClip TakeItemSound => _takeItemSound;
    public int Id { get; set; } = 0;
}
