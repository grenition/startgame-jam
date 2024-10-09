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
    [SerializeField] private string _letsGoToTheBear;

    public const string BearDoorsID = "BearDoorsID";
    public const string BearAnimatorBool = "Sleeping";

    private ComicsViewer _comics;
    private ControllerNetworkBus _bus;
    private CompletedTasks _tasks;

    private bool _finishedFinalActivity = false;

    [Inject]
    private void Construct(
        ComicsViewer comics,
        ControllerNetworkBus bus,
        CompletedTasks tasks)
    {
        _comics = comics;
        _bus = bus;
        _tasks = tasks;

        _bus.ActivityFinished += OnActivityFinished;
        _comics.Closed += OnFinishComicsFirstTime;
        _comics.OpenComicsWithControllers(_startComics);
    }

    private void OnFinishComicsFirstTime()
    {
        _comics.Closed -= OnFinishComicsFirstTime;
        var player = _bus.BigPlayer;
        if (player == null)
        {
            player = _bus.SmallPlayer;
        }
        player?.ShowMessage(_letsGoToTheBear);
    }

    private void OnActivityFinished(string activityName)
    {
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
                _bus.FinishGame();
                _tasks.Tasks.Clear();
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
