using UnityEngine;
using UnityEngine.UI;

public class ClientControllerPainter : MonoBehaviour
{
    [SerializeField] private Image _window, _inventoryBg;
    [SerializeField] private Image[] _boxes;
    [SerializeField] private Image _actionBar;
    [SerializeField] private Sprite _smallWindow, _smallBox, _smallInventoryBg;
    [SerializeField] private Sprite _bigWindow, _bigBox, _bigInventoryBg;
    [SerializeField] private Sprite _smallActionBar, _bigActionBar;
    [SerializeField] private Image[] _colorized, _lightColorized;
    [SerializeField] private Color _smallColor, _bigColor;
    [SerializeField] private Color _smallLightColor, _bigLightColor;

    public void Paint(PlayerTypes type)
    {
        Debug.Log($"Your player type is {type}");
        if(type is PlayerTypes.Small)
        {
            Paint(_smallWindow, _smallBox, _smallInventoryBg, _smallActionBar, _smallColor, _smallLightColor);
        }
        else
        {
            Paint(_bigWindow, _bigBox, _bigInventoryBg, _bigActionBar, _bigColor, _bigLightColor);
        }
    }

    private void Paint(Sprite window, Sprite box, Sprite inventoryBg, Sprite actionBar, Color clr, Color lightClr)
    {
        _window.sprite = window;
        _inventoryBg.sprite = inventoryBg;
        _actionBar.sprite = actionBar;

        foreach(var box1 in _boxes)
        {
            box1.sprite = box;
        }

        foreach(var img in _colorized)
        {
            img.color = clr;
        }
        foreach(var img in _lightColorized)
        {
            img.color = lightClr;
        }

        Destroy(gameObject);
    }
}
