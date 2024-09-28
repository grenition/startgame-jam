using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class RasberryStarter : ActivityStarter
{
    public const string WinMessageId = "RsbryCllctWin";

    [SerializeField] private RectTransform _parent;
    [SerializeField] private Sprite _smallBG, _bigBG;
    [SerializeField] private Transform _smallRasberries, _bigRasberries;
    [SerializeField] private RectTransform _smallBasket, _bigBasket;
    [SerializeField] private ActivityInfo _rasberyInfo;

    private RectTransform _curBucket;
    private int _maxBerries = 0, _curBerries = 0;
    private bool _smallWinned = false, _bigWinned = false;

    private Inventory _inventory;
    private ClientController _controller;

    [Inject]
    private void Construct(Inventory inventory, ClientController controller)
    {
        _inventory = inventory;
        _controller = controller;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        _inventory.RemoveItemByName(_inventory.Names.Basket);
        _inventory.AddItemFromStorage(_inventory.Names.FullBasket);
        _controller.ActivityHided += OnHideActivity;
        await UniTask.Yield();
        return;
    }

    private void OnHideActivity()
    {
        _controller.ActivityHided -= OnHideActivity;
        _controller.ShowActivity(_rasberyInfo);
        _controller.Interact();
    }

    protected override void OnInitialize(Image screen)
    {
        Bus.SpecialDataTransmitted += ReceiveData;

        if(Identification.PlayerType is PlayerTypes.Small)
        {
            InitializeBerries(_smallRasberries, _smallBasket, _smallBG);
        }
        else
        {
            InitializeBerries(_bigRasberries, _bigBasket, _bigBG);
        }
    }

    private void ReceiveData(string receiverId, int[] data)
    {
        if(receiverId == WinMessageId)
        {
            var type = (PlayerTypes)data[0];
            if(type is PlayerTypes.Small)
            {
                _smallWinned = true;
            }
            else if(type is PlayerTypes.Big)
            {
                _bigWinned = true;
            }

            if(Identification.PlayerType is PlayerTypes.Big && _smallWinned && _bigWinned)
            {
                Bus.SpecialDataTransmitted -= ReceiveData;
                Finish();
            }
        }
    }

    private void InitializeBerries(Transform berriesParent, RectTransform bucket, Sprite bg)
    {
        _curBucket = bucket;
        berriesParent.gameObject.SetActive(true);
        for(int i = 0; i < berriesParent.childCount; i++)
        {
            var child = berriesParent.GetChild(i);
            _maxBerries++;
            if(child.TryGetComponent<DragNDropElement>(out var drag))
            {
                drag.Droped += OnDrop;
            }
        }
        bucket.gameObject.SetActive(true);
        if(_parent.TryGetComponent<Image>(out var img))
        {
            img.sprite = bg;
        }
    }

    private void OnDrop(DragNDropElement drop)
    {
        var size2d = _curBucket.offsetMax - _curBucket.offsetMin;

        if(Vector3.Distance(drop.transform.position, _curBucket.transform.position) <= size2d.magnitude / 2)
        {
            Destroy(drop.gameObject);
            _curBerries++;

            if(_curBerries == _maxBerries)
            {
                var data = new int[] { (int)Identification.PlayerType };
                Bus.SendBusMessage(WinMessageId, data, PlayerTypes.LocalPlayers);
            }
        }
    }
}
