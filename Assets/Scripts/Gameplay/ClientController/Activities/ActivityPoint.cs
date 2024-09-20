using UnityEngine;
using VContainer;

public class ActivityPoint : MonoBehaviour
{
    [SerializeField] private ActivityInfo _info;
    [SerializeField] private bool _destroyAfterStartingActivity;

    private ControllerNetworkBus _bus;
    public ActivityInfo Info => _info;

    [Inject]
    private void Construct(ControllerNetworkBus bus)
    {
        _bus = bus;
        if (_destroyAfterStartingActivity)
            _bus.ActivityStarted += OnActivityStarted;
    }

    public void ShowActivity(PlayerTypes type)
    {
        _bus.ShowActivity(Info, type);
    }

    public void HideActivity(PlayerTypes type)
    {
        _bus.HideActivity(type);
    }

    private void OnActivityStarted(ActivityInfo info)
    {
        Debug.Log(info == _info);
        if(info == _info)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if(_destroyAfterStartingActivity)
            _bus.ActivityStarted -= OnActivityStarted;
    }
}
