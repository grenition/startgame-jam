using System.Collections;
using TMPro;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(CharacterController))]
public class PlayerObject : MonoBehaviour
{
    public const float Speed = 5f;

    [SerializeField] private GameObject _dialogue;
    [SerializeField] private TMP_Text _dialogueText;

    private CharacterController _controller;
    private Vector3 _moveDirection = Vector3.zero;
    private int _prevIndex = -1;
    private ControllerNetworkBus _bus;
    private ActivityPoint _nearlyPoint;

    private Coroutine _dialogueCor;

    public bool CanControl { get; private set; } = true;
    public PlayerTypes PlayerType { get; private set; }

    [Inject]
    private void Construct(ControllerNetworkBus bus)
    {
        _bus = bus;
        _controller = GetComponent<CharacterController>();
        _bus.InteractedOnServer += OnPlayerInteracted;
    }

    private void OnPlayerInteracted(PlayerTypes type)
    {
        if (type == PlayerType)
        {
            ActivateNearlyPoint();
        }
    }

    public void SetPlayerType(PlayerTypes type)
    {
        PlayerType = type;
        if (type is PlayerTypes.Small)
            _bus.SmallPlayer = this;
        else
            _bus.BigPlayer = this;
    }

    public void SetMoveDirection(Vector3 moveDirection, int index)
    {
        if(index > _prevIndex || Mathf.Abs(index - _prevIndex) > 1000)
        {
            _moveDirection = moveDirection;
            _prevIndex = index;
        }
    }

    private void Update()
    {
        if(CanControl)
        {
            if(_moveDirection.sqrMagnitude > 0)
            {
                _controller.Move(_moveDirection * Speed * Time.deltaTime);
            }
        }
    }

    public void ShowMessage(string mess)
    {
        if (_dialogueCor != null)
            StopCoroutine(_dialogueCor);
        _dialogueCor = StartCoroutine(ShowMessageIE(mess));
    }

    private IEnumerator ShowMessageIE(string mess)
    {
        _dialogue.SetActive(true);
        _dialogueText.text = mess;
        yield return new WaitForSeconds(3);
        _dialogue.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ActivityPoint>(out var point))
        {
            _nearlyPoint = point;
            point.ShowActivity(PlayerType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<ActivityPoint>(out var point))
        {
            _nearlyPoint = null;
            point.HideActivity(PlayerType);
        }
    }

    public void ActivateNearlyPoint()
    {
        if(_nearlyPoint != null)
        {
            _nearlyPoint.Interaction.Interact();
        }
    }

    private void OnDestroy()
    {
        if (PlayerType is PlayerTypes.Small) _bus.SmallPlayer = null;
        else _bus.BigPlayer = null;
    }
}
