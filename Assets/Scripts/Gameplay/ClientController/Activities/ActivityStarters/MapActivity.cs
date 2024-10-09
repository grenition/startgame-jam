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
            if(!_nextScene.CheckConditions())
            {
                _controller.ShowMessage(_waitingPlayersMessage);
                return;
            }
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
