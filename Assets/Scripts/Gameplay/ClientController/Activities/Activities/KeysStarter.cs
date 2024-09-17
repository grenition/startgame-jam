using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class KeysStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private Button[] _keys;
    [SerializeField] private int _missingKeyIndex;

    private ClientIdentification _identification;
    private Inventory _inventory;
    private bool _isEnd = false;

    [Inject]
    private void Construct(ClientIdentification identification, Inventory inventory)
    {
        _identification = identification;
        _inventory = inventory;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        _inventory.AddItemFromStorage(_inventory.Names.Key);
        await UniTask.Yield();
    }

    protected override void OnInitialize(Image screen)
    {
        if(_identification.PlayerType is PlayerTypes.Big)
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
        yield return new WaitForSeconds(.3f);
        Bus.FinishActivity(PlayerTypes.All);
    }
}
