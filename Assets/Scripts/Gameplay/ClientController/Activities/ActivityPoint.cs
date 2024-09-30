using Gameplay.QuestSystem;
using UnityEngine;
using VContainer;

public class ActivityPoint : InteractionPoint
{
    [SerializeField] private ActivityInfo _info;
    [SerializeField] private bool _destroyAfterStartingActivity;
    [SerializeField] private bool _activateInteractionWhenStart;

    private ControllerNetworkBus _bus;
    private ClientIdentification _identification;
    public ActivityInfo Info => _info;

    [Inject]
    private void Construct(ControllerNetworkBus bus, ClientIdentification identification)
    {
        _bus = bus;
        _identification = identification;
        _bus.ActivityStarted += OnActivityStarted;
    }

    public void ShowActivity(PlayerTypes type)
    {
        if(CheckConditions())
        {
            _bus.ShowActivity(Info, type);
        }
    }

    public void HideActivity(PlayerTypes type)
    {
        _bus.HideActivity(type);
    }

    private void OnActivityStarted(ActivityInfo info)
    {
        if(info == _info && _destroyAfterStartingActivity)
        {
            Destroy(gameObject);
        }
        if(_activateInteractionWhenStart)
        {
            var player = _bus.SmallPlayer;
            if (_identification.PlayerType is PlayerTypes.Big)
                player = _bus.BigPlayer;

            player.ActivateNearlyPoint();
        }
    }

    private void OnDestroy()
    {
        _bus.ActivityStarted -= OnActivityStarted;
    }
}
