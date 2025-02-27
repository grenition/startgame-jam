using Core.Networking.NetworkObjectsFactory;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using TMPro;
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
    [SerializeField] private ClientControllerPainter _painter;

    [SerializeField] private Image _infoImage;
    [SerializeField] private TMP_Text _infoText;
    [SerializeField] private ActivityInfo _comicsActivity;

    [SerializeField] private string _cannotStartMultiplayerTaskReason;

    public enum States { Nothing, SimpleSprite, WaitCallback, OnMiniGame }

    private IObjectsFactory _factory;
    private ControllerNetworkBus _bus;
    private ActivityInfo _showedInfo = null;
    private IObjectResolver _container;
    private ClientIdentification _identification;
    private CompletedTasks _tasks;
    private ComicsViewer _viewer;
    private Vector3 _prevMoveDirection = Vector3.zero;

    private float _sinMove, _cosMove;

    private Coroutine _infoAnimCor;

    public event Action Interacted;
    public event Action MiniGameSpawned;
    public event Action ActivityHided;

    public States State { get; private set; } = States.Nothing;
    public ActivityStarter PlayingMiniGame { get; private set; } = null;
    public Inventory InventoryModel { get; private set; }

    [field: SerializeField] public float MoveAngle { get; private set; } = 45f;

    [Inject]
    private void Construct(
        IObjectsFactory factory, 
        ControllerNetworkBus bus, 
        Inventory inventory,
        IObjectResolver container,
        ClientIdentification identification,
        CompletedTasks tasks,
        ComicsViewer viewer)
    {
        _factory = factory;
        _bus = bus;
        InventoryModel = inventory;
        container.Inject(inventory);
        _container = container;
        _identification = identification;
        _tasks = tasks;
        _bus.ResolveInfos(container);
        _viewer = viewer;
        _viewer.OpenedClient += OnComicsOpened;
    }

    public void OnFinishGame()
    {
        InventoryModel.Clear();
        _tasks.Tasks.Clear();
        HideActivity();
    }

    private void OnDestroy()
    {
        _bus.ResetClientController();
        _viewer.OpenedClient -= OnComicsOpened;
    }

    private void Start()
    {
        _bus.SetClientController(this);
        _painter.Paint(_identification.PlayerType);
        _interactBtn.onClick.AddListener(new(Interact));
        _screenBtn.onClick.AddListener(new(OnScreenTouched));
        HideActivity();

        _sinMove = Mathf.Sin(MoveAngle * Mathf.Deg2Rad);
        _cosMove = Mathf.Cos(MoveAngle * Mathf.Deg2Rad);
    }

    private void OnComicsOpened()
    {
        _showedInfo = null;
        SpawnMiniGame(_comicsActivity);
    }

    public void ShowActivity(ActivityInfo info)
    {
        if (State is States.SimpleSprite)
        {
            if(_showedInfo != null)
            {
                _bus.HideMarker(_showedInfo);
            }
            State = States.Nothing;
        }

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
        _bus.InvokeInteractOnServer(_identification.PlayerType);

        if (State is not States.SimpleSprite)
            return;

        if (_showedInfo == null || _showedInfo.MiniGamePrefab == null)
            return;

        _container.Inject(_showedInfo);
        if(!_bus.TestMode && !_showedInfo.CanInteract(out var reason))
        {
            ShowMessage(reason);
            return;
        }

        if(!_showedInfo.CanInteractWithTwoPlayers(_bus, _identification, out reason))
        {
            ShowMessage(reason);
            return;
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

    public void ResetState()
    {
        if(State is States.WaitCallback)
        {
            State = _showedInfo == null ? States.Nothing : States.SimpleSprite;
            ShowMessage(_cannotStartMultiplayerTaskReason);
        }
    }

    public void ShowMessage(string mess)
    {
        if (_infoAnimCor != null)
            StopCoroutine(_infoAnimCor);
        _infoAnimCor = StartCoroutine(ShowMessageAnim(mess));
    }

    private IEnumerator ShowMessageAnim(string mess)
    {
        _infoImage.gameObject.SetActive(true);
        var clr = _infoImage.color;
        clr.a = 0;
        _infoImage.color = clr;
        var txtClr = _infoText.color;
        txtClr.a = 0;
        _infoText.color = clr;
        clr.a = .8f;
        txtClr.a = 1;

        _infoImage.DOColor(clr, .5f);
        _infoText.DOColor(txtClr, .5f);
        _infoText.text = mess;

        yield return new WaitForSeconds(3);

        clr.a = 0;
        txtClr.a = 0;
        _infoText.DOColor(txtClr, .5f);
        _infoImage.DOColor(clr, .5f);

        yield return new WaitForSeconds(.5f);

        _infoImage.gameObject.SetActive(false);
    }

    public async void SpawnMiniGame(ActivityInfo info)
    {
        if (PlayingMiniGame != null)
        {
            Destroy(PlayingMiniGame.gameObject);
        }

        if (info != null && info.MiniGamePrefab != null)
        {
            _showedInfo = info;
            _bus.InvokeActivityStarted(info);
            State = States.OnMiniGame;

            if(info.OnlyOnePlayer)
            {
                _bus.FinishOnlyOnePlayerActivity(info);
            }

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

    public async UniTask FinishActivity(ActivityInfo info)
    {
        if(!_tasks.Tasks.Contains(info))
        {
            _tasks.Tasks.Add(info);
            _bus.AddTask(info);
        }

        if (_showedInfo != info)
            return;

        if(PlayingMiniGame != null)
        {
            await PlayingMiniGame.OnFinish();
            if(PlayingMiniGame != null)
            {
                Destroy(PlayingMiniGame.gameObject);
            }
            
            PlayingMiniGame = null;
        }
        HideActivity();
    }

    public void FinishOnlyOnePlayerActivity(ActivityInfo info)
    {
        if(_showedInfo == info && State is States.WaitCallback or States.OnMiniGame)
        {
            info.CanInteractWithTwoPlayers(_bus, _identification, out var reason);
            _bus.FinishActivity(PlayerTypes.LocalPlayers, PlayingMiniGame);
            ShowMessage(reason);
        }
    }

    private void Update()
    {
        if (State is States.OnMiniGame or States.WaitCallback)
        {
            if(_prevMoveDirection.sqrMagnitude > 0)
            {
                _prevMoveDirection = Vector3.zero;
                _bus.SetMoveDirection(Vector3.zero);
            }
            return;
        } 

        var direction = new Vector3(_moveJoy.Horizontal, 0, _moveJoy.Vertical);
        
        if(Vector3.Distance(direction, _prevMoveDirection) > DeathZoneForJoystick)
        {
            _prevMoveDirection = direction;

            float x = direction.x * _cosMove - direction.z * _sinMove;
            float y = direction.x * _sinMove + direction.z * _cosMove;

            direction = new(x, 0, y);

            _bus.SetMoveDirection(direction);
        }
    }
}
