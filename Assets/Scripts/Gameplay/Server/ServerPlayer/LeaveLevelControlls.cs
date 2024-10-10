using Cysharp.Threading.Tasks;
using Gameplay.QuestSystem;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaveLevelControlls : MonoBehaviour
{
    [SerializeField] private InteractionPoint _leaveLevelPoint;
    [SerializeField] private GameObject _messageParent;
    [SerializeField] private bool _fullQuitFromApplication;

    private bool _cantQuit = false;

    private void Start()
    {
        _messageParent.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _messageParent.SetActive(true);
        }
    }

    //UnityEvent
    public void SubmitPressed()
    {
        _messageParent.SetActive(false);
        _leaveLevelPoint?.Interact();
        if(_fullQuitFromApplication && !_cantQuit)
        {
            _cantQuit = true;
            NetworkManager.Singleton.Shutdown();
            Application.Quit();
        }
    }

    //UnityEvent
    public void CancelPressed()
    {
        _messageParent.SetActive(false);
    }
}
