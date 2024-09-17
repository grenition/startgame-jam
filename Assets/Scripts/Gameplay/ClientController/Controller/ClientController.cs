using Core.Networking.NetworkObjectsFactory;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ClientController : MonoBehaviour
{
    [SerializeField] private Button _interactBtn, _screenBtn;
    [SerializeField] private Image _screen;
    [SerializeField] private Sprite _defaultScreen;

    public enum States { Nothing, SimpleSprite, WaitCallback, OnMiniGame }

    private IObjectsFactory _factory;
    private ControllerNetworkBus _bus;
    private ActivityInfo _showedInfo = null;

    public event Action Interacted;

    public States State { get; private set; } = States.Nothing;
    public ActivityStarter PlayingMiniGame { get; private set; } = null;
    public Inventory InventoryModel { get; private set; }

    [Inject]
    private void Construct(
        IObjectsFactory factory, 
        ControllerNetworkBus bus, 
        Inventory inventory,
        IObjectResolver container)
    {
        _factory = factory;
        _bus = bus;
        InventoryModel = inventory;
        container.Inject(inventory);
    }

    private void Awake()
    {
        _bus.SetClientController(this);
    }
    private void OnDestroy()
    {
        _bus.ResetClientController();
    }

    private void Start()
    {
        _interactBtn.onClick.AddListener(new(Interact));
        _screenBtn.onClick.AddListener(new(OnScreenTouched));
        HideActivity();
    }

    public void ShowActivity(ActivityInfo info)
    {
        _screen.sprite = info.Image;
        _showedInfo = info;
        State = States.SimpleSprite;
    }

    public void HideActivity()
    {
        _screen.sprite = _defaultScreen;
        _showedInfo = null;
        State = States.Nothing;
    }

    public void OnScreenTouched()
    {
        Interact();
    }

    public void Interact()
    {
        if (State is not States.SimpleSprite)
            return;

        if (_showedInfo == null || _showedInfo.MiniGamePrefab == null)
            return;

        if(!_showedInfo.CanInteract(out var reason))
        {
            ShowMessage(reason);
        }

        State = States.WaitCallback;
        if(_showedInfo.SinglePlayer)
        {
            SpawnMiniGame(_showedInfo);
        }
        else
        {
            _bus.WaitForTeammateForMiniGame(_showedInfo);
        }
    }

    public void ShowMessage(string mess)
    {
        //TODO: Message
    }

    public void SpawnMiniGame(ActivityInfo info)
    {
        if (info != null && info.MiniGamePrefab != null)
        {
            State = States.OnMiniGame;

            var obj = _factory.SpawnLocalObject(info.MiniGamePrefab);
            PlayingMiniGame = obj;
            var screenChild = obj.GetScreenChild();

            if (screenChild != null)
            {
                screenChild.SetParent(_screen.transform);
                screenChild.offsetMin = Vector2.zero;
                screenChild.offsetMax = Vector2.zero;
            }

            obj.Initialize(_screen, _bus);

            Interacted?.Invoke();
        }
        else
        {
            State = States.SimpleSprite;
        }
    }

    public async UniTask FinishActivity()
    {
        if(PlayingMiniGame != null)
        {
            await PlayingMiniGame.OnFinish();
            Destroy(PlayingMiniGame.gameObject);
            PlayingMiniGame = null;
        }
        HideActivity();
    }
}
