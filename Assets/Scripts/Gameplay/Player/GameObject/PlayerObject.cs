using UnityEngine;
using VContainer;

[RequireComponent(typeof(CharacterController))]
public class PlayerObject : MonoBehaviour
{
    public const float Speed = 5f;

    private CharacterController _controller;
    private Vector3 _moveDirection = Vector3.zero;
    private int _prevIndex = -1;
    private ControllerNetworkBus _bus;

    public bool CanControl { get; private set; } = true;
    public PlayerTypes PlayerType { get; private set; }

    [Inject]
    private void Construct(ControllerNetworkBus bus, ClientIdentification identification)
    {
        _bus = bus;
        _controller = GetComponent<CharacterController>();
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ActivityPoint>(out var point))
        {
            point.ShowActivity(PlayerType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<ActivityPoint>(out var point))
        {
            point.HideActivity(PlayerType);
        }
    }
}
