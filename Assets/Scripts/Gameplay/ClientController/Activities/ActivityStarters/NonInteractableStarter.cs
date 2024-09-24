using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class NonInteractableStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private bool _singlePlayer;

    private ClientController _controller;

    [Inject]
    private void Construct(ClientController controller)
    {
        _controller = controller;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        await UniTask.Yield();
        return;
    }

    protected override void OnInitialize(Image screen)
    {
        _controller.Interacted += FinishDefault;
    }

    private void FinishDefault()
    {
        _controller.Interacted -= FinishDefault;
        Finish(_singlePlayer);
    }
}
