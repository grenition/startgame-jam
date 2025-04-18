using Gameplay.QuestSystem;
using UnityEngine;
using VContainer;

public class ActivityPoint : MonoBehaviour
{
    [SerializeField] private InteractionPoint _interactionPoint;
    [SerializeField] private ActivityInfo _info;
    [SerializeField] private bool _destroyAfterStartingActivity;
    [SerializeField] private bool _activateInteractionWhenStart;

    private ControllerNetworkBus _bus;
    public ActivityInfo Info => _info;
    public bool ActivateInteractionWhenStart => _activateInteractionWhenStart;
    public InteractionPoint Interaction => _interactionPoint;

    [Inject]
    private void Construct(ControllerNetworkBus bus)
    {
        _bus = bus;
        _bus.ActivityStarted += OnActivityStarted;
    }

    public void ShowActivity(PlayerTypes type)
    {
        if(Interaction == null || Interaction.CheckConditions())
        {
            _bus.ShowActivity(Info, type);
            _bus.ShowMarker(transform.position, Info);
        }
    }

    public void HideActivity(PlayerTypes type)
    {
        _bus.HideActivity(type);
        _bus.HideMarker(Info);
    }

    private void OnActivityStarted(ActivityInfo info)
    {
        _bus.HideMarker(_info);
        if(info == _info && _destroyAfterStartingActivity)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        _bus.ActivityStarted -= OnActivityStarted;
    }
}
