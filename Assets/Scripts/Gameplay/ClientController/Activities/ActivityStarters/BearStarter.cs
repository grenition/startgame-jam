using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BearStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private ActivityInfo _rasberyInfo;
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
    private void Construct(CompletedTasks tasks, Inventory inventory, ClientController controller)
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
        else
        {
            return 2;
        }
    }

    public void TakeItem()
    {
        _itemBtn.gameObject.SetActive(false);
        _inventory.AddItemFromStorage(_scenarioItems[_scenarioIndex]);
        StartCoroutine(TakeItemIE());
    }

    private IEnumerator TakeItemIE()
    {
        yield return new WaitForSeconds(1);
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

        if(_scenarioIndex == 4 && _inventory.HasItemByName(_inventory.Names.FullBasket))
        {
            _inventory.RemoveItemByName(_inventory.Names.FullBasket);
        }
    }
}
