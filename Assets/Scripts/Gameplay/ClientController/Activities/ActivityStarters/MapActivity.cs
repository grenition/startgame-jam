using Cysharp.Threading.Tasks;
using Gameplay.QuestSystem;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MapActivity : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private InteractionPoint _nextScene;
    [SerializeField] private string _comingSoonMessage, _waitingPlayersMessage;

    private ClientController _controller;
    private ControllerNetworkBus _bus;
    private bool _goToNextSceneAfterFinish = false;

    [Inject]
    private void Construct(ClientController controller, ControllerNetworkBus bus)
    {
        _controller = controller;
        _bus = bus;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        _controller.Interacted -= OnInteracted;
        await UniTask.Yield();
        if(_goToNextSceneAfterFinish)
        {
            _nextScene.Interact();
        }
        return;
    }

    //UnityEvent
    public void OnLevelClicked(int index)
    {
        if(index == 0)
        {
            _bus.IsAllPlayersConnected(b =>
            {
                if(b)
                {
                    _goToNextSceneAfterFinish = true;
                    Finish(true);
                }
                else
                {
                    _controller.ShowMessage(_waitingPlayersMessage);
                    return;
                }
            });
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
