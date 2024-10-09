using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class PlayIntroComics : MonoBehaviour
{
    [SerializeField] private Sprite[] _introComics;
    [SerializeField] private GameObject _text;

    private ComicsViewer _comicsViewer;
    private ClientIdentification _identification;

    [Inject]
    private void Construct(ComicsViewer viewer, ClientIdentification identification)
    {
        _comicsViewer = viewer;
        _identification = identification;
    }

    private void Start()
    {
        _ = StartAsync();
    }

    private async UniTask StartAsync()
    {
        while(_comicsViewer == null)
        {
            await UniTask.Yield();
        }

        if(_identification.PlayerType is PlayerTypes.Host)
        {
            _comicsViewer.Closed += OnComicsFinished;
            _comicsViewer.OpenComics(_introComics);
        }
        else
        {
            DestroyAll();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
            Input.GetMouseButtonDown(0))
        {
            _comicsViewer.NextPage();
            _text.SetActive(false);
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            _comicsViewer.CloseComics();
        }
    }

    private void OnComicsFinished()
    {
        _comicsViewer.Closed -= OnComicsFinished;
        DestroyAll();
    }

    private void DestroyAll()
    {
        Destroy(_text);
        Destroy(gameObject);
    }
}
