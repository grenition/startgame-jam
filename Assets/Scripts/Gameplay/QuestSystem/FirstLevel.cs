using Cysharp.Threading.Tasks;
using Gameplay.QuestSystem;
using System;
using UnityEngine;
using VContainer;

[Serializable]
public class FirstLevel : NetworkBusLevelMessageReceiver, IDisposable
{
    [SerializeField] private GameObject[] _openedFence, _closedFence;
    [SerializeField] private Sprite[] _startComics, _endComics;
    [SerializeField] private InteractionPoint _loadNextScene;
    [SerializeField] private Animator _bearAnimator;

    public const string BearDoorsID = "BearDoorsID";
    public const string BearAnimatorBool = "Sleeping";

    private ComicsViewer _comics;
    private ControllerNetworkBus _bus;

    private bool _finishedFinalActivity = false;

    [Inject]
    private void Construct(
        ComicsViewer comics,
        ControllerNetworkBus bus)
    {
        _comics = comics;
        _bus = bus;

        _bus.ActivityFinished += OnActivityFinished;
        _comics.OpenComicsWithControllers(_startComics);
    }

    private void OnActivityFinished(string activityName)
    {
        Debug.Log(activityName);
        if(activityName == nameof(StringStarter))
        {
            _finishedFinalActivity = true;
            _ = WaitForFrame(() => _comics.OpenComicsWithControllers(_endComics));
        }
        else if(activityName == nameof(ComicsControllerStarter) && _finishedFinalActivity)
        {
            _finishedFinalActivity = false;
            _ = WaitForFrame(() =>
            {
                _loadNextScene.Interact();
            });
        }
    }

    private async UniTask WaitForFrame(Action callback)
    {
        await UniTask.Yield();
        callback?.Invoke();
    }

    public override void OnReceiveMessage(string id, params int[] data)
    {
        if(id == BearDoorsID)
        {
            BearOpenDoors();
        }
    }

    private void BearOpenDoors()
    {
        _bearAnimator.SetBool(BearAnimatorBool, true);
        foreach (var opened in _openedFence)
        {
            opened.SetActive(true);
        }
        foreach (var closed in _closedFence)
        {
            closed.SetActive(false);
        }
    }

    public void Dispose()
    {
        _bus.ActivityFinished -= OnActivityFinished;
    }
}
