using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class KeysStarter : ActivityStarter
{
    [SerializeField] private ActivityInfo _keysActivity;
    [SerializeField] private RectTransform _parent;
    [SerializeField] private Button[] _keys;
    [SerializeField] private int _missingKeyIndex;
    [SerializeField] private string _oneKeyIsDifferent;

    private ClientController _controller;
    private Inventory _inventory;
    private bool _isEnd = false;

    [Inject]
    private void Construct(ClientController controller, Inventory inventory)
    {
        _controller = controller;
        _inventory = inventory;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        _controller.ActivityHided += ActivityHided;
        await UniTask.Yield();
        return;
    }

    private void ActivityHided()
    {
        _controller.ActivityHided -= ActivityHided;

        if(Identification.PlayerType is PlayerTypes.Small)
        {
            _controller.SpawnMiniGame(_keysActivity);
        }
    }

    protected override void OnInitialize(Image screen)
    {
        _controller.ShowMessage(_oneKeyIsDifferent);

        if(_inventory.HasItemByName(_inventory.Names.Key))
        {
            _inventory.RemoveItemByName(_inventory.Names.Key);
        }

        if(Identification.PlayerType is PlayerTypes.Big)
        {
            Destroy(_keys[_missingKeyIndex].gameObject);
        }
        else
        {
            for(int i = 0; i < _keys.Length; i++)
            {
                int index = i;
                _keys[i].onClick.AddListener(new(() => ClickButton(index)));
                _keys[i].transform.SetSiblingIndex(Random.Range(0, _keys.Length));
            }
        }
    }

    private void ClickButton(int index)
    {
        if (_isEnd) return;

        if(index == _missingKeyIndex)
        {
            _isEnd = true;
            StartCoroutine(WinAnimation());
        }
        else
        {
            StartCoroutine(WrongAnim(_keys[index]));
        }
    }

    private IEnumerator WrongAnim(Button button)
    {
        button.transform.DORotate(new Vector3(0, 0, 20), .1f);
        yield return new WaitForSeconds(.1f);
        button.transform.DORotate(new Vector3(0, 0, -20), .1f);
        yield return new WaitForSeconds(.1f);
        button.transform.DORotate(Vector3.zero, .2f);
    }

    private IEnumerator WinAnimation()
    {
        _keys[_missingKeyIndex].transform.DOScale(Vector3.one * 1.3f, .3f);
        yield return new WaitForSeconds(1f);
        Finish(false);
    }
}
