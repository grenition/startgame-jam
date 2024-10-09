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

    private int GetScenarioIndex()
    {
        if(_inventory.HasItemByName(_inventory.Names.Key))
        {
            return 0;
        }
        else if(_inventory.HasItemByName(_inventory.Names.Toy))
        {
            return 3;
        }
        else if(_inventory.HasItemByName(_inventory.Names.Basket))
        {
            return 1;
        }
        else if(_tasks.Tasks.Contains(_rasberyInfo))
        {
            return 4;
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

        _scenarioIndex = GetScenarioIndex();
        if(_scenarioIndex == 3 && _inventory.HasItemByName(_inventory.Names.Toy))
        {
            _inventory.RemoveItemByName(_inventory.Names.Toy);
        }
        else if(_scenarioIndex == 4 && _inventory.HasItemByName(_inventory.Names.FullBasket))
        {
            _inventory.RemoveItemByName(_inventory.Names.FullBasket);
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
        Debug.Log("TEST");
        if(_scenarioIndex == 4 && _inventory.HasItemByName(_inventory.Names.FullBasket))
        {
            Debug.Log("If statement");
            _inventory.RemoveItemByName(_inventory.Names.FullBasket);
            Debug.Log("Send message");
            Bus.SendMessageToLevelMessageReceiver(FirstLevel.BearDoorsID, null);
        }
    }
}
