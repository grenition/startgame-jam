using System;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public class SettingsModelView
{
    [SerializeField] private SettingsView _view;
    [SerializeField] private AudioMixer _soundMixer;
    public ReactiveProperty<int> QualityIndex { get; private set; } = new();

    public const string MasterVolume = "MasterVolume";

    private SettingsModel _model;
    private float _soundLevel = 0f;

    public void Initialize(SettingsModel model)
    {
        _model = model;

        QualityIndex.Value = QualitySettings.GetQualityLevel();
        _soundMixer.GetFloat(MasterVolume, out var sound);
        _soundLevel = sound;
        _view.SettingsAplied += OnApply;
        _view.QualityChanged += OnQualityChange;
        _view.QuitPressed += OnQuit;
        _view.SoundChanged += OnSoundLevelChanged;

        _view.Initialize(this, sound);
    }

    private void OnQualityChange(int level)
    {
        QualityIndex.Value = level;
    }

    private void OnApply()
    {
        _model.ApplySettings(QualityIndex.Value, _soundLevel);
    }

    private void OnQuit()
    {
        _model.Close();
    }

    private void OnSoundLevelChanged(float value)
    {
        _soundLevel = value;
    }
}
