using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsModel : MonoBehaviour
{
    [SerializeField] private SettingsModelView _view;

    private void Start()
    {
        _view.Initialize(this);
    }

    public void Open()
    {
        _view.Open();
    }

    public void Close()
    {

    }
}
