using System;
using UniRx;
using UnityEngine;

[Serializable]
public class SettingsModelView
{
    [Serializable]
    public class QualityLevel
    {
        public string Name;
        public int Index;
    }

    [SerializeField] private SettingsView _view;

    [field: SerializeField] public QualityLevel[] Levels { get; private set; }
    public ReactiveProperty<int> QualityIndex { get; private set; }

    private SettingsModel _model;

    public void Initialize(SettingsModel model)
    {
        QualityIndex.Value = QualitySettings.GetQualityLevel();
        _view.SettingsAplied += OnApply;
        _view.QualityChanged += OnQualityChange;

        _view.Initialize(this);
    }

    public void Open()
    {
        _view.Open();
    }

    private void OnQualityChange(int level)
    {
        QualityIndex.Value = level;
    }

    private void OnApply()
    {
        QualitySettings.SetQualityLevel(QualityIndex.Value);
    }
}
