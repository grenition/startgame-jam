using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ComicsViewer : NetworkBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _image;
    [SerializeField] private Vector2 _resolution;

    public event Action Closed, ClosedClient, OpenedClient;
    public Sprite[] Pages { get; private set; }
    public int PageIndex { get; private set; } = -1;

    private void Start()
    {
        UpdateScreenSize();
        _canvas.gameObject.SetActive(false);
    }

    public void UpdateScreenSize()
    {
        float mp = _resolution.x / _resolution.y;
        float width = Screen.width / _canvas.scaleFactor;
        float height = Screen.height / _canvas.scaleFactor;

        if(mp > 1)
        {
            width = height * mp;
        }
        else
        {
            height = width / mp;
        }

        _image.rectTransform.offsetMin = new(-width / 2, -height / 2);
        _image.rectTransform.offsetMax = new(width / 2, height / 2);
    }

    public void CloseComics()
    {
        _canvas.gameObject.SetActive(false);
        Closed?.Invoke();
        InvokeClosedClientRpc();
    }

    public void OpenComics(Sprite[] sprites)
    {
        _canvas.gameObject.SetActive(true);
        Pages = sprites;
        PageIndex = -1;
        NextPage();
    }

    public void NextPage()
    {
        PageIndex++;
        if (PageIndex >= Pages.Length)
        {
            CloseComics();
            return;
        }

        _resolution = Pages[PageIndex].rect.size;
        UpdateScreenSize();

        _image.sprite = Pages[PageIndex];
    }

    public void SendSignalToNextPage()
    {
        SendSignalToNextPageServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendSignalToNextPageServerRpc()
    {
        NextPage();
    }

    public void SendSignalToSkip()
    {
        SendSignalToSkipServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendSignalToSkipServerRpc()
    {
        CloseComics();
    }

    [ClientRpc]
    private void InvokeClosedClientRpc()
    {
        ClosedClient?.Invoke();
    }

    public void OpenComicsWithControllers(Sprite[] images)
    {
        OpenComicsWithControllersClientRpc();
        OpenComics(images);
    }

    [ClientRpc]
    private void OpenComicsWithControllersClientRpc()
    {
        OpenedClient?.Invoke();
    }
}
