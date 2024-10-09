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
    [SerializeField] private ActivityInfo _stringsActivity, _comicsActivity;
    [SerializeField] private InteractionPoint _loadNextScene;

    public const string BearDoorsID = "BearDoorsID";

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

    private void OnActivityFinished(ActivityInfo activity)
    {
        if(activity == _stringsActivity)
        {
            _finishedFinalActivity = true;
            _ = WaitForFrame(() => _comics.OpenComicsWithControllers(_endComics));
        }
        else if(activity == _comicsActivity && _finishedFinalActivity)
        {
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
        Debug.Log("Message Received");
        if(id == BearDoorsID)
        {
            BearOpenDoors();
        }
    }

    private void BearOpenDoors()
    {
        Debug.Log("Open doors");
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
