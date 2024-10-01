using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class InputCodeStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private Image _bg;
    [SerializeField] private Sprite _openedSprite;
    [SerializeField] private ActivityInfo _nextActivity;
    [SerializeField] private AudioClip _unlockSound;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private TMP_Text[] _buttonsTexts;

    [SerializeField] private Vector2 _buttonsXScale, _buttonsYScale;
    [SerializeField] private RectTransform _layoutGroup;

    public const int CorrectCode = 7405;

    private ClientController _controller;
    private AudioPool _audioPool;
    private int[] _curCode = new int[] { 0, 0, 0, 0 };

    private bool _isFinished = false;

    [Inject]
    private void Construct(ClientController controller, AudioPool pool)
    {
        _controller = controller;
        _audioPool = pool;
    }

    public override RectTransform GetScreenChild()
    {
        return _parent;
    }

    public override async UniTask OnFinish()
    {
        _controller.Interacted -= OnInteract;
        _controller.ActivityHided += OnActivityHided;
        await UniTask.Yield();
        return;
    }

    private void OnActivityHided()
    {
        _controller.ActivityHided -= OnActivityHided;

        _controller.ShowActivity(_nextActivity);
        _controller.Interact();
    }

    private void OnDigitClick(int index)
    {
        if (_isFinished)
            return;

        _curCode[index]++;
        if (_curCode[index] > 9)
        {
            _curCode[index] = 0;
        }
        _buttonsTexts[index].text = $"{_curCode[index]}";

        CheckCode();
    }

    private void CheckCode()
    {
        int code = 0;
        for(int i = 0; i < _curCode.Length; i++)
        {
            code += _curCode[i] * (int)Mathf.Pow(10, 3 - i);
        }
        if(code == CorrectCode)
        {
            StartCoroutine(FinishIE());
        }
    }

    private IEnumerator FinishIE()
    {
        _audioPool.PlaySound(_unlockSound);
        _isFinished = true;
        _bg.sprite = _openedSprite;
        foreach(var btn in _buttons)
        {
            btn.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        Finish(true);
    }

    private void OnInteract()
    {
        _controller.Interacted -= OnInteract;
        _controller.HideActivity();
    }

    protected override void OnInitialize(Image screen)
    {
        _controller.Interacted += OnInteract;

        for(int i = 0; i < _buttons.Length; i++)
        {
            int index = i;
            _buttons[i].onClick.AddListener(new(() => OnDigitClick(index)));
        }

        ChangeScale();
    }

    private void ChangeScale()
    {
        _layoutGroup.offsetMax = new(
            _bg.rectTransform.offsetMax.x * _buttonsXScale.x,
            _bg.rectTransform.offsetMax.y * _buttonsYScale.x);

        _layoutGroup.offsetMin = new(
            _bg.rectTransform.offsetMin.x * _buttonsXScale.y,
            _bg.rectTransform.offsetMin.y * _buttonsYScale.y);
    }
}
