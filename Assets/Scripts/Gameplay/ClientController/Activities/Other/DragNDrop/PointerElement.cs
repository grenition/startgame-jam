using UnityEngine;
using UnityEngine.EventSystems;

public class PointerElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool Entered { get; private set; } = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Entered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Entered = false;
    }
}
