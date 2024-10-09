using Cysharp.Threading.Tasks;
using Gameplay.QuestSystem;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MapActivity : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private InteractionPoint _nextScene;
    [SerializeField] private string _comingSoonMessage;

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
        _controller.Interacted -= OnInteracted;
        await UniTask.Yield();
        return;
    }

    //UnityEvent
    public void OnLevelClicked(int index)
    {
        if(index == 0)
        {
            _nextScene.Interact();
            Finish(true);
        }
        else
        {
            _controller.ShowMessage(_comingSoonMessage);
        }
    }

    private void OnInteracted()
    {
        Finish(true);
    }

    protected override void OnInitialize(Image screen)
    {
        _controller.Interacted += OnInteracted;
    }
}
