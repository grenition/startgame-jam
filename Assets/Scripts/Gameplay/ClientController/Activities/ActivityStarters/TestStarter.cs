using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TestStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        await UniTask.Yield();
    }

    protected override void OnInitialize(Image screen)
    {
        
    }
}
