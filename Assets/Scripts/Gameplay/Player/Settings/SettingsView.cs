using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SettingsView
{
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private TMP_Dropdown _qualityDropdown;
    [SerializeField] private Button _applyBtn, _quitBtn;

    private SettingsModelView _viewModel;

    public event Action SettingsAplied, QuitPressed;
    public event Action<int> QualityChanged;
    public event Action<float> SoundChanged;

    public void Initialize(SettingsModelView modelView, float soundValue)
    {
        _viewModel = modelView;

        _qualityDropdown.SetValueWithoutNotify(_viewModel.QualityIndex.Value);
        _applyBtn.onClick.AddListener(new(() => SettingsAplied?.Invoke()));
        _quitBtn.onClick.AddListener(new(OnQuitPress));
        _qualityDropdown.onValueChanged.AddListener(new(i => QualityChanged?.Invoke(i)));
        _soundSlider.onValueChanged.AddListener(new(v => SoundChanged?.Invoke(v)));
        _soundSlider.value = soundValue;
    }

    private void OnQuitPress()
    {
        QuitPressed?.Invoke();
    }
}
