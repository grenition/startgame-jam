using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class HoleStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private GameObject _small, _big;
    //Small
    [SerializeField] private PointerElement _blueLayout, _greenLayout, _redLayout;
    [SerializeField] private BookHoleItem[] _books;
    [SerializeField] private GameObject _doorClosed, _doorOpened;
    [SerializeField] private Button _toy;
    [SerializeField] private Image _darkness;
    //Big
    [SerializeField] private Image _lightBG;
    [SerializeField] private Color _lightBGColor, _noLightBGColor;

    //Small
    private int _booksCount = 0;
    private Inventory _inventory;
    public const float MinimumLightLevelForWork = .3f;
    private bool _cantWork = true;
    //Big
    public const float MaxLightLevel = 1f;
    public const float LightLevelSpeed = .1f;
    public const float SyncRate = .1f;
    private Color _bgColor;
    private float _lightLevel = 0f, _syncTime = 0f;
    private bool _isBigAnim = false;
    //Other
    private int _syncIndex = 0;
    public const float SyncFloatAccuracy = 10000f;
    public const string WinMessageID = "HoleStarterWin";
    public const string SyncMessageID = "HoleStarterSync";

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        Bus.Tester.AutoWined -= OnAutoWin;
        await UniTask.Yield();
        return;
    }

    private void OnDragStart(DragNDropElement el)
    {
        if(_cantWork)
        {
            el.StopDrag();
            return;
        }

        el.GetComponent<Image>().raycastTarget = false;
    }

    private void OnDragEnd(DragNDropElement el)
    {
        el.GetComponent<Image>().raycastTarget = true;
        if(el is BookHoleItem book && !_cantWork)
        {
            PointerElement dropZone = _blueLayout;
            if (book.MyColor is BookHoleItem.Colors.Green) dropZone = _greenLayout;
            else if (book.MyColor is BookHoleItem.Colors.Red) dropZone = _redLayout;

            if(dropZone.Entered)
            {
                book.CanDrag = false;
                book.StayElementAfterDrop = true;
                book.transform.SetParent(dropZone.transform);
                _booksCount--;

                if(_booksCount == 0)
                {
                    OpenDoor();
                }
            }
        }
    }

    private void OpenDoor()
    {
        _doorClosed.SetActive(false);
        _doorOpened.SetActive(true);
        _toy.gameObject.SetActive(true);
        _toy.onClick.AddListener(new(OnTakeToy));
    }

    private void OnTakeToy()
    {
        Destroy(_toy.gameObject);
        _inventory.AddItemFromStorage(_inventory.Names.Toy);
        Bus.SpecialDataTransmitted -= OnReceiveMessage;
        Finish();
    }

    private void Update()
    {
        if(_isBigAnim)
        {
            _lightLevel = Mathf.Clamp(_lightLevel - Time.deltaTime * LightLevelSpeed, 0, MaxLightLevel);
            _bgColor = _lightBGColor * (_lightLevel / MaxLightLevel) +
                _noLightBGColor * ((MaxLightLevel - _lightLevel) / MaxLightLevel);
            _lightBG.color = _bgColor;

            if(Input.acceleration.magnitude > 5f || Input.GetKeyDown(KeyCode.T))
            {
                _lightLevel = Mathf.Clamp(_lightLevel + LightLevelSpeed, 0, MaxLightLevel);
            }

            _syncTime += Time.deltaTime;
            if(_syncTime >= SyncRate)
            {
                _syncTime = 0f;
                var data = new int[] { ++_syncIndex, (int)(_lightLevel * SyncFloatAccuracy) };
                Bus.SendBusMessage(SyncMessageID, data, PlayerTypes.Small);
            }
        }
    }

    private void OnReceiveMessage(string id, int[] data)
    {
        if(id == SyncMessageID)
        {
            int index = data[0];
            if(index > _syncIndex || Mathf.Abs(index - _syncIndex) > 1000)
            {
                _syncIndex = index;
                float val = data[1] / SyncFloatAccuracy;
                _lightLevel = val;
                var clr = _darkness.color;
                clr.a = (MaxLightLevel - val) / MaxLightLevel;
                _darkness.color = clr;

                if(_lightLevel < MinimumLightLevelForWork && !_cantWork)
                {
                    _cantWork = true;
                    foreach(var book in _books)
                    {
                        if(book.Dragging)
                        {
                            book.StopDrag();
                            break;
                        }
                    }
                }
                else if(_lightLevel >= MinimumLightLevelForWork && _cantWork)
                {
                    _cantWork = false;
                }
            }
        }
    }

    private void OnAutoWin()
    {
        OpenDoor();
    }

    protected override void OnInitialize(Image screen)
    {
        Bus.Tester.AutoWined += OnAutoWin;

        if(_inventory.HasItemByName(_inventory.Names.Light))
        {
            _inventory.RemoveItemByName(_inventory.Names.Light);
        }

        if(Identification.PlayerType is PlayerTypes.Small)
        {
            Bus.SpecialDataTransmitted += OnReceiveMessage;
            _small.SetActive(true);
            _big.SetActive(false);

            foreach(var book in _books)
            {
                book.Droped += OnDragEnd;
                book.StartedDraging += OnDragStart;
                _booksCount++;
            }

            _doorClosed.SetActive(true);
            _doorOpened.SetActive(false);
            _toy.gameObject.SetActive(false);
        }
        else
        {
            _small.SetActive(false);
            _big.SetActive(true);

            _isBigAnim = true;
            _bgColor = _noLightBGColor;
        }
    }
}
