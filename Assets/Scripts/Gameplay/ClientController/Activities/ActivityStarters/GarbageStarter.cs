using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GarbageStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Sprite _backgroundSprite;
    [SerializeField] private DragNDropElement[] _firstGarbage, _secondGarbage, _thirdGarbage;
    [SerializeField] private RectTransform[] _containers;
    [SerializeField] private Button _item;

    private Inventory _inventory;
    private int _maxItems = 0, _curItems = 0;
    private bool _smallWin = false, _bigWin = false;

    public const string MessageID = "GrbgeCollctrWin";

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
        Bus.SpecialDataTransmitted -= OnReceiveMessage;
        await UniTask.Yield();
        return;
    }

    private void OnDrop(DragNDropElement drop, int index)
    {
        var size = _containers[index].offsetMax - _containers[index].offsetMin;
        var dist = size.magnitude / 2f;

        if(Vector3.Distance(drop.transform.position, _containers[index].transform.position) <= dist)
        {
            Destroy(drop.gameObject);
            _curItems++;

            if(_curItems == _maxItems)
            {
                var data = new int[] { (int)Identification.PlayerType };
                Bus.SendBusMessage(MessageID, data, PlayerTypes.LocalPlayers);
            }
        }
    }

    private void OnReceiveMessage(string id, int[] data)
    {
        if(id == MessageID)
        {
            var type = (PlayerTypes)data[0];
            if (type is PlayerTypes.Small) _smallWin = true;
            else if (type is PlayerTypes.Big) _smallWin = true;

            if(Identification.PlayerType is PlayerTypes.Small && _smallWin && _bigWin)
            {
                _backgroundImage.sprite = _backgroundSprite;
                _item.gameObject.SetActive(true);
                _item.onClick.AddListener(new(OnTakeItem));
                Bus.SpecialDataTransmitted -= OnReceiveMessage;
            }
        }
    }

    private void OnTakeItem()
    {
        _inventory.AddItemFromStorage(_inventory.Names.Pruner);
        Finish();
    }

    protected override void OnInitialize(Image screen)
    {
        _item.gameObject.SetActive(false);
        Bus.SpecialDataTransmitted += OnReceiveMessage;

        var drops = new DragNDropElement[][] { _firstGarbage, _secondGarbage, _thirdGarbage };
        for(int i = 0; i < _containers.Length; i++)
        {
            InitializeGroup(i, drops[i]);
        }
    }

    private void InitializeGroup(int index, DragNDropElement[] drops)
    {
        foreach(var drop in drops)
        {
            drop.Droped += dr => OnDrop(dr, index);
            _maxItems++;
        }
    }
}
