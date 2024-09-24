using UnityEngine;
using UnityEngine.UI;

public class ClientControllerPainter : MonoBehaviour
{
    [SerializeField] private Image _window, _inventoryBg;
    [SerializeField] private Image[] _boxes;
    [SerializeField] private Sprite _smallWindow, _smallBox, _smallInventoryBg;
    [SerializeField] private Sprite _bigWindow, _bigBox, _bigInventoryBg;
    [SerializeField] private Image[] _colorized;
    [SerializeField] private Color _smallColor, _bigColor;

    public void Paint(PlayerTypes type)
    {
        if(type is PlayerTypes.Small)
        {
            Paint(_smallWindow, _smallBox, _smallInventoryBg, _smallColor);
        }
        else
        {
            Paint(_bigWindow, _bigBox, _bigInventoryBg, _bigColor);
        }
    }

    private void Paint(Sprite window, Sprite box, Sprite inventoryBg, Color clr)
    {
        _window.sprite = window;
        _inventoryBg.sprite = inventoryBg;

        foreach(var box1 in _boxes)
        {
            box1.sprite = box;
        }

        foreach(var img in _colorized)
        {
            img.color = clr;
        }

        Destroy(gameObject);
    }
}
