using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ToolsCollectStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent, _box;
    [SerializeField] private Button _light, _stone;

    [SerializeField] private Vector2 _lightXScaler, _lightYScaler;
    [SerializeField] private Vector2 _stoneXScaler, _stoneYScaler;

    public const int MaxItems = 2;
    private int _takedItems = 0;
    private Inventory _inventory;

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
        await UniTask.Yield();
        return;
    }

    protected override void OnInitialize(Image screen)
    {
        ChangeObjectsSize();

        _light.onClick.AddListener(new(TakeLight));
        _stone.onClick.AddListener(new(TakeStone));
        _inventory.RemoveItemByName(_inventory.Names.Key);
    }

    public void ChangeObjectsSize()
    {
        _light.image.rectTransform.offsetMax = new(_box.offsetMax.x * _lightXScaler.y, _box.offsetMax.y * _lightYScaler.x);
        _light.image.rectTransform.offsetMin = new(_box.offsetMin.x * _lightXScaler.x, _box.offsetMin.y * _lightYScaler.y);

        _stone.image.rectTransform.offsetMax = new(_box.offsetMax.x * _stoneXScaler.x, _box.offsetMax.y * _stoneYScaler.x);
        _stone.image.rectTransform.offsetMin = new(_box.offsetMin.x * _stoneXScaler.y, _box.offsetMin.y * _stoneYScaler.y);
    }

    private void TakeLight()
    {
        _takedItems++;
        Destroy(_light.gameObject);
        _inventory.AddItemFromStorage(_inventory.Names.Light);
        CheckForFinish();
    }

    private void TakeStone()
    {
        _takedItems++;
        Destroy(_stone.gameObject);
        _inventory.AddItemFromStorage(_inventory.Names.Stone);
        CheckForFinish();
    }

    private void CheckForFinish()
    {
        if(_takedItems == MaxItems)
        {
            StartCoroutine(FinishIE());
        }
    }

    private IEnumerator FinishIE()
    {
        yield return new WaitForSeconds(1);
        Finish(true);
    }
}
