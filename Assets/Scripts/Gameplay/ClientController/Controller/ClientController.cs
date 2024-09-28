using Core.Networking.NetworkObjectsFactory;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ClientController : MonoBehaviour
{
    public const float DeathZoneForJoystick = .05f;

    [SerializeField] private Button _interactBtn, _screenBtn;
    [SerializeField] private Image _screen;
    [SerializeField] private Transform _miniGameParent;
    [SerializeField] private Joystick _moveJoy;

    public enum States { Nothing, SimpleSprite, WaitCallback, OnMiniGame }

    private IObjectsFactory _factory;
    private ControllerNetworkBus _bus;
    private ActivityInfo _showedInfo = null;
    private IObjectResolver _container;
    private Vector3 _prevMoveDirection = Vector3.zero;

    public event Action Interacted;
    public event Action MiniGameSpawned;
    public event Action ActivityHided;

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
        _container = container;
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
        if (State is not States.Nothing)
            return;

        _screen.sprite = info.Image;
        _screen.color = Color.white;
        _showedInfo = info;
        State = States.SimpleSprite;
    }

    public void HideActivity()
    {
        _screen.color = new(0, 0, 0, 0);
        _screen.sprite = null;
        _showedInfo = null;
        State = States.Nothing;
        if(PlayingMiniGame != null)
        {
            Destroy(PlayingMiniGame.gameObject);
            PlayingMiniGame = null;
        }
        ActivityHided?.Invoke();
    }

    public void OnScreenTouched()
    {
        Interact();
    }

    public void Interact()
    {
        Interacted?.Invoke();

        if (State is not States.SimpleSprite)
            return;

        if (_showedInfo == null || _showedInfo.MiniGamePrefab == null)
            return;

        _container.Inject(_showedInfo);
        if(!_showedInfo.CanInteract(out var reason))
        {
            ShowMessage(reason);
        }

        State = States.WaitCallback;
        _bus.InvokeActivityStarted(_showedInfo);
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

            var obj = _factory.SpawnLocalObject(info.MiniGamePrefab, TargetScene.None);
            PlayingMiniGame = obj;
            var screenChild = obj.GetScreenChild();

            if (screenChild != null)
            {
                screenChild.SetParent(_miniGameParent);
                screenChild.offsetMin = Vector2.zero;
                screenChild.offsetMax = Vector2.zero;
                screenChild.localScale = Vector3.one;
            }

            obj.Initialize(_screen, _bus);

            MiniGameSpawned?.Invoke();
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

    private void Update()
    {
        var direction = new Vector3(_moveJoy.Horizontal, 0, _moveJoy.Vertical);
        if(Vector3.Distance(direction, _prevMoveDirection) > DeathZoneForJoystick)
        {
            _prevMoveDirection = direction;
            _bus.SetMoveDirection(direction);
        }
    }
}
