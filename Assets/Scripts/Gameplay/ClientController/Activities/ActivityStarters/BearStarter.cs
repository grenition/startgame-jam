using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BearStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private ActivityInfo _rasberyInfo, _keysInfo;
    [SerializeField]
    private GameObject _wantsToyScreen, _wantsRasberryScreen,
                       _giveKeysScreen, _givesBaketScreen, _happyBearScreen;
    [SerializeField] private Button _itemBtn;
    [SerializeField] private Image _itemImage;
    [SerializeField] private GameObject _loadingScreen;

    private CompletedTasks _tasks;
    private Inventory _inventory;
    private ClientController _controller;
    private int _scenarioIndex;

    private GameObject[] _scenarioScreens;
    private string[] _scenarioItems;

    [Inject]
    private void Construct(
        CompletedTasks tasks,
        Inventory inventory,
        ClientController controller)
    {
        _tasks = tasks;
        _inventory = inventory;
        _controller = controller;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        if(_scenarioItems[_scenarioIndex] == null)
        {
            _controller.Interacted -= OnInteract;
        }
        await UniTask.Yield();
        return;
    }

    private async UniTask<int> GetScenarioIndex()
    {
        var asyncKey = _inventory.HasItemInAllPlayers(_inventory.Names.Key);
        var asyncToy = _inventory.HasItemInAllPlayers(_inventory.Names.Toy);
        var asyncBasket = _inventory.HasItemInAllPlayers(_inventory.Names.Basket);
        var asyncRasberry = _inventory.HasItemInAllPlayers(_inventory.Names.FullBasket);

        var results = await UniTask.WhenAll(asyncKey, asyncToy, asyncBasket, asyncRasberry);

        if (results.Item1)
        {
            return 0;
        }
        else if(results.Item2)
        {
            if(_inventory.HasItemByName(_inventory.Names.Toy))
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
        else if(results.Item3)
        {
            return 1;
        }
        else if(_tasks.Tasks.Contains(_rasberyInfo))
        {
            if(results.Item4)
            {
                return _inventory.HasItemByName(_inventory.Names.FullBasket) ? 4 : 1;
            }
            else
            {
                return 4;
            }
        }
        else if(!_tasks.Tasks.Contains(_keysInfo))
        {
            return 2;
        }
        else
        {
            return 0;
        }
    }

    public void TakeItem()
    {
        _itemBtn.gameObject.SetActive(false);
        _inventory.AddItemFromStorage(_scenarioItems[_scenarioIndex]);
        StartCoroutine(TakeItemIE());
    }

    private IEnumerator TakeItemIE(float seconds = 1)
    {
        yield return new WaitForSeconds(seconds);
        Finish();
    }

    private void OnInteract()
    {
        Finish();
    }

    protected override void OnInitialize(Image screen)
    {
        _scenarioScreens = new GameObject[] {
            _wantsToyScreen, _wantsRasberryScreen,
            _giveKeysScreen, _givesBaketScreen, _happyBearScreen
        };
        _scenarioItems = new string[] {
            null, null, _inventory.Names.Key, _inventory.Names.Basket, null
        };

        OnInitializeAsync();
    }

    private async void OnInitializeAsync()
    {
        _scenarioIndex = await GetScenarioIndex();
        if (_scenarioIndex == 3 && _inventory.HasItemByName(_inventory.Names.Toy))
        {
            _inventory.RemoveItemByName(_inventory.Names.Toy);
        }
        else if (_scenarioIndex == 4 && _inventory.HasItemByName(_inventory.Names.FullBasket))
        {
            _inventory.RemoveItemByName(_inventory.Names.FullBasket);
            Bus.SendMessageToLevelMessageReceiver(FirstLevel.BearDoorsID, null);
        }

        _scenarioScreens[_scenarioIndex].SetActive(true);
        if (_scenarioItems[_scenarioIndex] == null)
        {
            _itemBtn.gameObject.SetActive(false);
            _controller.Interacted += OnInteract;
        }
        else
        {
            _itemBtn.gameObject.SetActive(true);
            _itemBtn.image.sprite = _inventory.GetItemFromStorage(_scenarioItems[_scenarioIndex]).Icon;
            _itemBtn.onClick.AddListener(new(TakeItem));
        }

        Destroy(_loadingScreen);
    }
}
