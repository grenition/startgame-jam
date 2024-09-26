using UnityEngine;

public class BookHoleItem : DragNDropElement
{
    public enum Colors { Green, Blue, Red }

    [field: SerializeField] public Colors MyColor { get; set; }
}
