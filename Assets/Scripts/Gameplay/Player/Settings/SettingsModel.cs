using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsModel : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private SettingsModelView _view;

    public void Close()
    {
        Finish(true);
    }

    protected override void OnInitialize(Image screen)
    {
        _view.Initialize(this, Bus);
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public void ApplySettings(int quality, float volume)
    {
        Bus.ApplySettings(quality, volume);
    }

    public override async UniTask OnFinish()
    {
        await UniTask.Yield();
        return;
    }
}
