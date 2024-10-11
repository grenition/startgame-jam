using Cysharp.Threading.Tasks;
using Gameplay.QuestSystem;
using UnityEngine;
using UnityEngine.UI;

public class GoToHubStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private InteractionPoint _toNextScene;

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        await UniTask.Yield();
        if(Identification.PlayerType is PlayerTypes.Big)
        {
            _toNextScene.Interact();
        }
        
        Bus.FinishGame();
        return;
    }

    protected override void OnInitialize(Image screen)
    {
        OnInitializeAsync();
    }

    private async void OnInitializeAsync()
    {
        await UniTask.Yield();
        Finish();
    }
}
