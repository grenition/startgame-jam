using TMPro;
using UnityEngine;

public class PlayIntroComics : MonoBehaviour
{
    [SerializeField] private Sprite[] _introComics;
    [SerializeField] private ComicsViewer _comicsViewer;
    [SerializeField] private GameObject _text;

    private void Start()
    {
        _comicsViewer.Closed += OnComicsFinished;
        _comicsViewer.OpenComics(_introComics);
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
        Destroy(_text);
        _comicsViewer.Closed -= OnComicsFinished;
        Destroy(gameObject);
    }
}
