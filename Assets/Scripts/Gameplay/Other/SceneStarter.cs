using UnityEngine;
using VContainer;

public class SceneStarter : MonoBehaviour
{
    public enum LevelType { None, FirstLevel, Hub }

    [SerializeField] private LevelType _levelType;
    [SerializeField] private FirstLevel _firstLevel;
    [SerializeField] private HubLevel _hubLevel;

    [Inject]
    private void Construct(ControllerNetworkBus bus, IObjectResolver resolver)
    {
        if (_levelType is LevelType.FirstLevel)
        {
            bus.MessageReceiver = _firstLevel;
            resolver.Inject(_firstLevel);
        }
        else if(_levelType is LevelType.Hub)
        {
            bus.MessageReceiver = _hubLevel;
            resolver.Inject(_hubLevel);
        }

        Destroy(gameObject);
    }
}
