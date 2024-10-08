using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ComicsControllerStarter : ActivityStarter
{
    [SerializeField] private RectTransform _parent;
    [SerializeField] private Button _nextPageBtn, _skipBtn;

    private ComicsViewer _viewer;
    private ClientController _controller;

    [Inject]
    private void Construct(ClientController controller, ComicsViewer viewer)
    {
        _viewer = viewer;
        _controller = controller;
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

    private void OnInteract()
    {
        _viewer.SendSignalToNextPage();
    }

    private void OnSkip()
    {
        _viewer.SendSignalToSkip();
    }

    private void OnCloseClient()
    {
        _viewer.ClosedClient -= OnCloseClient;
        _controller.Interacted -= OnInteract;
        Finish();
    }

    protected override void OnInitialize(Image screen)
    {
        _viewer.ClosedClient += OnCloseClient;
        _controller.Interacted += OnInteract;
        _nextPageBtn.onClick.AddListener(new(OnInteract));
        _skipBtn.onClick.AddListener(new(OnSkip));
    }
}
