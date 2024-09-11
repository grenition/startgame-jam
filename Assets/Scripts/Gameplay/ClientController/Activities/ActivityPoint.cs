using UnityEngine;
using VContainer;

public abstract class ActivityPoint : MonoBehaviour
{
    [SerializeField] private ActivityInfo _info;

    private ControllerNetworkBus _bus;

    public enum State {  Idle, Playing, Finished }

    public State CurState { get; private set; } = State.Idle;
    public ActivityInfo Info => _info;

    [Inject]
    private void Construct(ControllerNetworkBus bus)
    {
        _bus = bus;
    }

    public void ShowActivity()
    {
        _bus.ShowActivity(Info);
    }

    public void HideActivity()
    {
        _bus.HideActivity();
    }

    public void Interact()
    {
        _bus.Interact(this);
    }
}
