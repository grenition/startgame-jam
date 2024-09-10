using System;
using UnityEngine;
using UnityEngine.UI;

public class ClientController : MonoBehaviour
{
    [SerializeField] private Button _interactBtn;
    [SerializeField] private Image _screen;

    public event Action Interacted;

    private void Start()
    {
        _interactBtn.onClick.AddListener(() => Interacted?.Invoke());
    }

    public void ShowActivity(ActivityInfo info)
    {
        _screen.sprite = info.Image;
    }

    public void HideActivity()
    {

    }
}
